using HlidacStatu.Lib.Data.External.DataSets;
using HlidacStatu.Web.Models;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;


namespace HlidacStatu.Web.Controllers
{
    public partial class DataController : Controller
    {
        public ActionResult CreateAdv()
        {
            return View(new Registration());
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateAdv(Registration data, FormCollection form)
        {

            var email = Request?.RequestContext?.HttpContext?.User?.Identity?.Name;

            var newReg = WebFormToRegistration(data, form);
            newReg.datasetId = form["datasetId"];
            newReg.created = DateTime.Now;

            var res = DataSet.Api.Create(newReg, email, form["jsonSchema"]);
            if (res.valid)
                return RedirectToAction("Manage", "Data", new { id = res.value });
            else
            {
                ViewBag.ApiResponseError = res;
                return View(newReg);
            }
        }

        [HttpGet]
        public ActionResult CreateSimple()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateSimple(string name, string delimiter, FormCollection form, HttpPostedFileBase file)
        {
            Guid fileId = Guid.NewGuid();
            var uTmp = new Lib.IO.UploadedTmpFile();
            if (string.IsNullOrEmpty(delimiter))
            {
                ViewBag.ApiResponseError = ApiResponseStatus.Error(-99, "Bez jména datasetu nemůžeme pokračovat. Prozraďte nám ho, prosím.");
                return View();
            }
            if (file == null)
            {
                ViewBag.ApiResponseError = ApiResponseStatus.Error(-99, "Źádné CSV jste nenahráli");

                return View();
            }
            else
            {
                var path = uTmp.GetFullPath(fileId.ToString(), fileId.ToString() + ".csv");
                file.SaveAs(path);
                return RedirectToAction("CreateSimple2", new { fileId = fileId, delimiter = delimiter, name = name });
            }
        }


        [HttpGet]
        public ActionResult CreateSimple2(CreateSimpleModel model)
        {
            model.Delimiter = model.Delimiter?.Trim() ?? ",";
            if (model.Delimiter == "," || model.Delimiter == ";")
                model.Delimiter = model.Delimiter;
            else if (model.Delimiter == "\\t")
                model.Delimiter = "\t";
            else
                model.Delimiter = ",";



            var uTmp = new Lib.IO.UploadedTmpFile();
            var path = uTmp.GetFullPath(model.FileId.ToString(), model.FileId.ToString() + ".csv");
            if (!System.IO.File.Exists(path))
                return RedirectToAction("CreateSimple");
            try
            {

                using (System.IO.StreamReader r = new System.IO.StreamReader(path))
                {
                    var csv = new CsvHelper.CsvReader(r, new CsvHelper.Configuration.Configuration() { HasHeaderRecord = true, Delimiter = model.Delimiter });
                    csv.Read(); csv.ReadHeader();
                    model.Headers = csv.Context.HeaderRecord;

                    //read first lines with data and guest type
                    List<string[]> lines = new List<string[]>();
                    for (int row = 0; row < 2; row++)
                    {
                        if (csv.Read())
                        {
                            lines.Add(csv.Context.Record);
                        }
                    }
                    List<string> colTypes = null;
                    if (lines.Count > 0)
                    {
                        colTypes = new List<string>();
                        for (int cx = 0; cx < lines[0].Length; cx++)
                        {
                            string t = GuestBestCSVValueType(lines[0][cx]);
                            for (int line = 1; line < lines.Count; line++)
                            {
                                var nextT = GuestBestCSVValueType(lines[line][cx]);
                                if (nextT != t)
                                    t = "string"; //kdyz jsou ruzne typy ve stejnem sloupci v ruznych radcich, 
                                                  //fallback na string
                            }
                            colTypes.Add(t);
                        }
                    }
                    ViewBag.ColTypes = colTypes.ToArray();

                    return View(model);
                }
            }
            catch (Exception e)
            {
                ViewBag.ApiResponseError = ApiResponseStatus.Error(-99, "Soubor není ve formátu CSV.", e.ToString());

                return View(model);

            }
        }

        [HttpPost]
        public ActionResult CreateSimple2(CreateSimpleModel model, FormCollection form)
        {

            model.Headers = (form["sheaders"] ?? "").Split('|');

            //check Keycolumn
            List<CreateSimpleModel.Column> cols = new List<CreateSimpleModel.Column>();
            int columns = model.Headers.Length;
            for (int i = 0; i < columns; i++)
            {

                cols.Add(
                    new CreateSimpleModel.Column()
                    {
                        Name = model.Headers[i],
                        NiceName = form[$"nicename_{i}"],
                        ValType = form[$"typ_{i}"],
                        ShowSearchFormat = form[$"show_search_{i}"] == "--" ? "string" : form[$"show_search_{i}"],
                        ShowDetailFormat = form[$"show_detail_{i}"] == "--" ? "string" : form[$"show_detail_{i}"],
                    }
                    );
            }
            model.Columns = cols.ToArray();

            var uTmp = new Lib.IO.UploadedTmpFile();
            var path = uTmp.GetFullPath(model.FileId.ToString(), model.FileId.ToString() + ".csv");
            var pathModels = uTmp.GetFullPath(model.FileId.ToString(), model.FileId.ToString() + ".json");
            System.IO.File.WriteAllText(pathModels, Newtonsoft.Json.JsonConvert.SerializeObject(model));

            return RedirectToAction("createSimple3", model);
        }


        [HttpGet]
        public ActionResult CreateSimple3(CreateSimpleModel model)
        {
            var uTmp = new Lib.IO.UploadedTmpFile();
            var path = uTmp.GetFullPath(model.FileId.ToString(), model.FileId.ToString() + ".csv");
            var pathJson = uTmp.GetFullPath(model.FileId.ToString(), model.FileId.ToString() + ".json");
            if (!System.IO.File.Exists(path))
                return RedirectToAction("CreateSimple");
            if (!System.IO.File.Exists(pathJson))
                return RedirectToAction("CreateSimple");

            model = Newtonsoft.Json.JsonConvert.DeserializeObject<CreateSimpleModel>(System.IO.File.ReadAllText(pathJson));


            Dictionary<string, Type> properties = new Dictionary<string, Type>();
            properties.Add("id", typeof(string));
            foreach (var c in model.Columns)
            {
                switch (c.ValType)
                {
                    case "number":
                        properties.Add(c.Normalized(c.Name), typeof(decimal));
                        break;
                    case "datetime":
                        properties.Add(c.Normalized(c.Name), typeof(DateTime));
                        break;
                    case "url":
                    case "ico":
                    default:
                        properties.Add(c.Normalized(c.Name), typeof(string));
                        break;
                }
            }
            RuntimeClassBuilder rcb = new RuntimeClassBuilder("customClass");
            var rcbObj = rcb.CreateObject(properties);
            Newtonsoft.Json.Schema.Generation.JSchemaGenerator jsonGen = new Newtonsoft.Json.Schema.Generation.JSchemaGenerator();
            jsonGen.DefaultRequired = Newtonsoft.Json.Required.Default;
            var schema = jsonGen.Generate(rcbObj.GetType()); //JSON schema 


            //create registration
            Registration reg = new Registration();
            reg.allowWriteAccess = false;
            reg.betaversion = true;
            reg.jsonSchema = schema.ToString();
            reg.name = model.Name;                        
            reg.NormalizeShortName();

            if (DataSet.ExistsDataset(reg.datasetId))
                reg.datasetId = reg.datasetId + "-" + Devmasters.Core.TextUtil.GenRandomString(5);

            var status = DataSet.Api.Create(reg, Request?.RequestContext?.HttpContext?.User?.Identity?.Name);
            if (status.valid == false)
                ViewBag.ApiResponseError = status;

            return View();

        }


        private string GuestBestCSVValueType(string value)
        {
            if (Util.ParseTools.ToDateFromCZ(value).HasValue)
                return "date";
            else if (Util.ParseTools.ToDateTimeFromCZ(value).HasValue)
                return "datetime";
            else if (Util.ParseTools.ToDecimal(value).HasValue)
                return "number";
            else
                return "string";

        }


    }
}
