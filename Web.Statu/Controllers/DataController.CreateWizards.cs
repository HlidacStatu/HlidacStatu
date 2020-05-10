using HlidacStatu.Lib.Data.External.DataSets;
using HlidacStatu.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace HlidacStatu.Web.Controllers
{
    public partial class DataController : GenericAuthController
    {
        public ActionResult CreateAdv()
        {
            var email = Request?.RequestContext?.HttpContext?.User?.Identity?.Name;
            if (Request.IsAuthenticated== false)
            {
                
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.PathAndQuery });
            }

            return View(new Registration());
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateAdv(Registration data, FormCollection form)
        {

            var email = Request?.RequestContext?.HttpContext?.User?.Identity?.Name;
            if (Request.IsAuthenticated== false)
            {
                
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.PathAndQuery });
            }

            datasetIndexStatCache.Invalidate();

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
        public ActionResult CreateFromBackup()
        {
            var email = Request?.RequestContext?.HttpContext?.User?.Identity?.Name;
            if (Request.IsAuthenticated == false)
            {

                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.PathAndQuery });
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateFromBackup(FormCollection form, int step, HttpPostedFileBase file)
        {
            var email = Request?.RequestContext?.HttpContext?.User?.Identity?.Name;
            if (Request.IsAuthenticated == false)
            {

                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.PathAndQuery });
            }

            string json = "";

            if (file != null && file.InputStream != null && file.InputStream.CanRead)
            {
                using (StreamReader reader = new StreamReader(file.InputStream))
                {
                    json = reader.ReadToEnd();
                }
            }
            Registration reg = null;
            try
            {
                 reg = Newtonsoft.Json.JsonConvert.DeserializeObject<Registration>(json);
            }
            catch  { }
            if (reg == null)
                ViewBag.ApiResponseError = ApiResponseStatus.Error(500, "Nekorektní záloha datasetu", "Nekorektní záloha datasetu");

            datasetIndexStatCache.Invalidate();

            if (reg != null)
            {
                var ds = DataSet.CachedDatasets.Get(reg.datasetId);
                if (ds != null)
                {
                    ViewBag.ExistsDS = ds.Registration();
                }
            }

            return View("CreateFromBackup"+step, reg);
        }

        [HttpGet]
        public ActionResult CreateSimple()
        {
            var email = Request?.RequestContext?.HttpContext?.User?.Identity?.Name;
            if (Request.IsAuthenticated== false)
            {
                
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.PathAndQuery });
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateSimple(string name, string delimiter, FormCollection form, HttpPostedFileBase file)
        {
            var email = Request?.RequestContext?.HttpContext?.User?.Identity?.Name;
            if (Request.IsAuthenticated== false)
            {
                
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.PathAndQuery });
            }


            Guid fileId = Guid.NewGuid();
            var uTmp = new Lib.IO.UploadedTmpFile();
            if (string.IsNullOrEmpty(name))
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
                return RedirectToAction("CreateSimple2", new { fileId = fileId, delimiter = CreateSimpleModel.GetValidDelimiter(delimiter), name = name });
            }
        }


        [HttpGet]
        public ActionResult CreateSimple2(CreateSimpleModel model)
        {

            var email = Request?.RequestContext?.HttpContext?.User?.Identity?.Name;
            if (Request.IsAuthenticated== false)
            {
                
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.PathAndQuery });
            }


            var uTmp = new Lib.IO.UploadedTmpFile();
            var path = uTmp.GetFullPath(model.FileId.ToString(), model.FileId.ToString() + ".csv");
            if (!System.IO.File.Exists(path))
                return RedirectToAction("CreateSimple");
            try
            {

                using (System.IO.StreamReader r = new System.IO.StreamReader(path))
                {
                    var config = new CsvHelper.Configuration.CsvConfiguration(Util.Consts.csCulture);
                    config.HasHeaderRecord = true; 
                    config.Delimiter = model.GetValidDelimiter();


                    var csv = new CsvHelper.CsvReader(r, config);
                    csv.Read(); csv.ReadHeader();
                    model.Headers = csv.Context.HeaderRecord.Where(m=> !string.IsNullOrEmpty(m?.Trim())).ToArray();

                    //read first lines with data and guest type
                    List<string[]> lines = new List<string[]>();
                    for (int row = 0; row < 10; row++)
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
            var email = Request?.RequestContext?.HttpContext?.User?.Identity?.Name;
            if (Request.IsAuthenticated== false)
            {
                
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.PathAndQuery });
            }

            var uTmp = new Lib.IO.UploadedTmpFile();
            var path = uTmp.GetFullPath(model.FileId.ToString(), model.FileId.ToString() + ".csv");
            var pathModels = uTmp.GetFullPath(model.FileId.ToString(), model.FileId.ToString() + ".json");

            model.Headers = (form["sheaders"] ?? "").Split('|');

            //check Keycolumn
            List<CreateSimpleModel.Column> cols = new List<CreateSimpleModel.Column>();
            int columns = model.Headers.Length;
            for (int i = 0; i < columns; i++)
            {

                string name = model.Headers[i];
                if (form[$"include_{i}"] == "1")
                {
                    cols.Add(
                        new CreateSimpleModel.Column()
                        {
                            Name = name,
                            NiceName = form[$"nicename_{i}"],
                            ValType = form[$"typ_{i}"],
                            ShowSearchFormat = form[$"show_search_{i}"] == "--" ? "string" : form[$"show_search_{i}"],
                            ShowDetailFormat = form[$"show_detail_{i}"] == "--" ? "string" : form[$"show_detail_{i}"],
                        }
                        );
                }
            }
            if (string.IsNullOrEmpty(model.KeyColumn) && !cols.Any(m => m.Name.ToLower() == "id"))
                cols.Add(new CreateSimpleModel.Column()
                {
                    Name = "id",
                    NiceName = "Id",
                    ValType = "string",
                    ShowSearchFormat = "show",
                    ShowDetailFormat = "hide",
                });

            model.Columns = cols.ToArray();
            model.Save(pathModels);


            bool addIcoCol = false;
            Dictionary<string, Type> properties = new Dictionary<string, Type>();
            //properties.Add("id", typeof(string));
            foreach (var c in model.Columns)
            {
                switch (c.ValType)
                {
                    case "number":
                        properties.Add(c.NormalizedName(), typeof(Nullable<decimal>));
                        break;
                    case "datetime":
                        properties.Add(c.NormalizedName(), typeof(Nullable<DateTime>));
                        break;
                    case "url":
                    case "ico":
                    default:
                        properties.Add(c.NormalizedName(), typeof(string));
                        break;
                }
            }
            if (addIcoCol && !model.Columns.Any(m => m.NormalizedName() == "ico"))
                properties.Add("ICO", typeof(string));

            if (!properties.Any(m => m.Key.ToLower() == "id"))
                properties.Add("id", typeof(string));

            RuntimeClassBuilder rcb = new RuntimeClassBuilder(properties);
            var rcbObj = rcb.CreateObject();
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
            reg.createdBy = email;

            HlidacStatu.Api.Dataset.Connector.ClassicTemplate.ClassicSearchResultTemplate search = new Api.Dataset.Connector.ClassicTemplate.ClassicSearchResultTemplate();
            HlidacStatu.Api.Dataset.Connector.ClassicTemplate.ClassicDetailTemplate detail = new Api.Dataset.Connector.ClassicTemplate.ClassicDetailTemplate();


            search.AddColumn("Detail", "<a href=\"{{ fn_DatasetItemUrl item.id }}\">Detail</a>");
            foreach (var col in model.Columns)
            {
                if (col.NormalizedName().ToLower() != "id")
                {
                    if (col.ShowSearchFormat == "price")
                        search.AddColumn(col.NiceName, "{{ fn_FormatPrice item." + col.NormalizedName() + " }}");
                    else if (col.ShowSearchFormat == "show")
                    {
                        if (col.ValType == "number")
                            search.AddColumn(col.NiceName, "{{ fn_FormatNumber item." + col.NormalizedName() + " }}");
                        else if (col.ValType == "datetime")
                            search.AddColumn(col.NiceName, "{{ fn_FormatDate item." + col.NormalizedName() + " }}");
                        else if (col.ValType == "ico")
                            search.AddColumn(col.NiceName, "{{ fn_RenderCompanyWithLink item." + col.NormalizedName() + " }}");
                        else if (col.ValType == "url")
                            search.AddColumn(col.NiceName, "<a href='{{ item." + col.NormalizedName() + " }}' target='_blank'>Odkaz</a>");
                        else
                            search.AddColumn(col.NiceName, "{{ item." + col.NormalizedName() + " }}");
                    }
                }

                if (col.ShowDetailFormat == "price")
                    detail.AddColumn(col.NiceName, "{{ fn_FormatPrice item." + col.NormalizedName() + " }}");
                else if (col.ShowDetailFormat == "show")
                {
                    if (col.ValType == "number")
                        detail.AddColumn(col.NiceName, "{{ fn_FormatNumber item." + col.NormalizedName() + " }}");
                    else if (col.ValType == "datetime")
                        detail.AddColumn(col.NiceName, "{{ fn_FormatDate item." + col.NormalizedName() + " }}");
                    else if (col.ValType == "ico")
                        detail.AddColumn(col.NiceName, "{{ fn_RenderCompanyWithLink item." + col.NormalizedName() + " }}");
                    else if (col.ValType == "url")
                        detail.AddColumn(col.NiceName, "<a href='{{ item." + col.NormalizedName() + " }}' target='_blank'>Odkaz</a>");
                    else
                        detail.AddColumn(col.NiceName, "{{ item." + col.NormalizedName() + " }}");
                }
            }

            reg.detailTemplate = new Registration.Template() { body = detail.Body };
            reg.searchResultTemplate = new Registration.Template() { body = search.Body };


            if (DataSet.ExistsDataset(reg.datasetId))
                reg.datasetId = reg.datasetId + "-" + Devmasters.Core.TextUtil.GenRandomString(5);

            datasetIndexStatCache.Invalidate();

            var status = DataSet.Api.Create(reg, email);

            if (status.valid == false)
            {
                if (DataSet.ExistsDataset((status.value?.ToString() ?? "")))
                    DataSetDB.Instance.DeleteRegistration(status.value?.ToString(), email);

                ViewBag.ApiResponseError = status;
                return View(model);
            }
            model.DatasetId = ((DataSet)status.value).DatasetId;
            model.Save(pathModels);
            return RedirectToAction("createSimple3", model);
        }


        [HttpGet]
        public ActionResult CreateSimple3(Guid? fileId)
        {
            var email = Request?.RequestContext?.HttpContext?.User?.Identity?.Name;
            if (Request.IsAuthenticated== false)
            {
                //https://www.hlidacstatu.cz/account/Login?returnUrl=%2F%3Frnd%3D0036bd9be9bc42d4bdf449492968846e
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.PathAndQuery });
            }

            if (!fileId.HasValue)
                return RedirectToAction("CreateSimple");

            var uTmp = new Lib.IO.UploadedTmpFile();
            var path = uTmp.GetFullPath(fileId.ToString(), fileId.ToString() + ".csv");
            var pathJson = uTmp.GetFullPath(fileId.ToString(), fileId.ToString() + ".json");
            if (!System.IO.File.Exists(path))
                return RedirectToAction("CreateSimple");
            if (!System.IO.File.Exists(pathJson))
                return RedirectToAction("CreateSimple");

            CreateSimpleModel model = CreateSimpleModel.Load(pathJson);
            if (model.NumOfRows == 0)
            {
                using (System.IO.StreamReader r = new System.IO.StreamReader(path))
                {
                    var csv = new CsvHelper.CsvReader(r, new CsvHelper.Configuration.CsvConfiguration(Util.Consts.csCulture) { HasHeaderRecord = true, Delimiter = model.GetValidDelimiter() });
                    csv.Read(); csv.ReadHeader();

                    while (csv.Read())
                    {
                        model.NumOfRows++;
                    }

                }
            }
            model.Save(pathJson);
            return View(model);

        }

        [HttpGet]
        public ActionResult ImportData(string id, CreateSimpleModel model)
        {
            var email = Request?.RequestContext?.HttpContext?.User?.Identity?.Name;
            if (Request.IsAuthenticated== false)
            {
                //https://www.hlidacstatu.cz/account/Login?returnUrl=%2F%3Frnd%3D0036bd9be9bc42d4bdf449492968846e
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.PathAndQuery });
            }

            model = model ?? new CreateSimpleModel();
            model.DatasetId = id;
            if (string.IsNullOrEmpty(id))
                return Redirect("/data");

            var ds = DataSet.CachedDatasets.Get(id);
            if (ds == null)
                return Redirect("/data");

            if (ds.HasAdminAccess(email) == false)
                return View("NoAccess");


            model.DatasetId = ds.DatasetId;

            if (ds.IsFlatStructure() == false)
            {
                ViewBag.Mode = "notflat";
                return View(model);
            }
            if (model.FileId.HasValue)
            {
                ViewBag.Mode = "mapping";
                Guid fileId = model.FileId.Value;
                var uTmp = new Lib.IO.UploadedTmpFile();
                var path = uTmp.GetFullPath(fileId.ToString(), fileId.ToString() + ".csv");
                if (!System.IO.File.Exists(path))
                    return RedirectToAction("ImportData", new { id = model.DatasetId });

                using (System.IO.StreamReader r = new System.IO.StreamReader(path))
                {
                    var csv = new CsvHelper.CsvReader(r, new CsvHelper.Configuration.CsvConfiguration(Util.Consts.csCulture) { HasHeaderRecord = true, Delimiter = model.GetValidDelimiter() });
                    csv.Read(); csv.ReadHeader();
                    model.Headers = csv.Context.HeaderRecord.Where(m => !string.IsNullOrEmpty(m?.Trim())).ToArray();
                }

            }
            else
                ViewBag.Mode = "upload";
            return View(model);
        }

        [HttpPost]
        public ActionResult ImportData(string id, string delimiter, FormCollection form, HttpPostedFileBase file)
        {
            var email = Request?.RequestContext?.HttpContext?.User?.Identity?.Name;
            if (Request.IsAuthenticated== false)
            {
                //https://www.hlidacstatu.cz/account/Login?returnUrl=%2F%3Frnd%3D0036bd9be9bc42d4bdf449492968846e
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.PathAndQuery });
            }

            if (string.IsNullOrEmpty(id))
                return Redirect("/data");

            var ds = DataSet.CachedDatasets.Get(id);
            if (ds == null)
                return Redirect("/data");

            if (ds.HasAdminAccess(email) == false)
                return View("NoAccess");

            datasetIndexStatCache.Invalidate();

            Guid fileId = Guid.NewGuid();
            var uTmp = new Lib.IO.UploadedTmpFile();
            if (file == null)
            {
                ViewBag.ApiResponseError = ApiResponseStatus.Error(-99, "Źádné CSV jste nenahráli");

                return View();
            }
            else
            {
                var path = uTmp.GetFullPath(fileId.ToString(), fileId.ToString() + ".csv");
                file.SaveAs(path);
                return RedirectToAction("ImportData", new { id = ds.DatasetId, fileId = fileId, delimiter = CreateSimpleModel.GetValidDelimiter(delimiter) });
            }
        }

        [HttpPost]
        public ActionResult ImportDataProcess(string id, CreateSimpleModel model, FormCollection form)
        {
            var email = Request?.RequestContext?.HttpContext?.User?.Identity?.Name;
            if (Request.IsAuthenticated== false)
            {
                //https://www.hlidacstatu.cz/account/Login?returnUrl=%2F%3Frnd%3D0036bd9be9bc42d4bdf449492968846e
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.PathAndQuery });
            }

            ViewBag.NumOfRows = 0;

            model.DatasetId = id;


            string[] csvHeaders = null;

            if (string.IsNullOrEmpty(id))
                return Redirect("/data");

            var ds = DataSet.CachedDatasets.Get(id);
            if (ds == null)
                return Redirect("/data");

            if (ds.HasAdminAccess(email) == false)
                return View("NoAccess");

            datasetIndexStatCache.Invalidate();


            if (ds.IsFlatStructure() == false)
            {
                return RedirectToAction("ImportData", new { id = ds.DatasetId, fileId = model.FileId, delimiter = model.GetValidDelimiter() });

            }

            var uTmp = new Lib.IO.UploadedTmpFile();
            var path = uTmp.GetFullPath(model.FileId.ToString(), model.FileId.ToString() + ".csv");
            if (!System.IO.File.Exists(path))
                return RedirectToAction("ImportData", new { id = ds.DatasetId });

            RuntimeClassBuilder rcb = new RuntimeClassBuilder(ds.GetPropertyNamesTypesFromSchema());

            string[] formsHeaders = form["sheaders"].Split('|');
            List<MappingCSV> mappingProps = new List<MappingCSV>();
            for (int i = 0; i < formsHeaders.Length+3; i++) //+3 a little bit more, at least +1 for id column
            {
                if (!string.IsNullOrEmpty(form["source_" + i])
                    && !string.IsNullOrEmpty(form["target_" + i])
                    && !string.IsNullOrEmpty(form["transform_" + i])
                    )
                {
                    mappingProps.Add(new MappingCSV()
                            {
                                sourceCSV = form["source_" + i],
                                TargetJSON = form["target_" + i],
                                Transform = form["transform_" + i]
                            }
                    );
                }
            }

            System.Collections.Concurrent.ConcurrentBag<Exception> errors = new System.Collections.Concurrent.ConcurrentBag<Exception>();

            List<Tuple<object,string>> items = new List<Tuple<object, string>>();

            using (System.IO.StreamReader r = new System.IO.StreamReader(path))
            {
                var csv = new CsvHelper.CsvReader(r, new CsvHelper.Configuration.CsvConfiguration(Util.Consts.csCulture) { HasHeaderRecord = true, Delimiter = model.GetValidDelimiter() });
                csv.Read(); csv.ReadHeader();
                csvHeaders = csv.Context.HeaderRecord.Where(m => !string.IsNullOrEmpty(m?.Trim())).ToArray(); //for future control

                while (csv.Read())
                {
                    var newObj = rcb.CreateObject();
                    for (int m = 0; m < mappingProps.Count; m++)
                    {
                        Type destType = ds.GetPropertyNameTypeFromSchema(mappingProps[m].TargetJSON).FirstOrDefault().Value;
                        object value = null;

                        string[] specialValues = new string[] { "-skip-", "-gen-", "--" };
                        if (specialValues.Contains(mappingProps[m].sourceCSV))
                        {
                            if (mappingProps[m].sourceCSV == "-gen-")
                                value = Guid.NewGuid().ToString("N");
                            else
                                continue; // -skip- skip 
                        }
                        else
                        {
                            string svalue = csv.GetField(mappingProps[m].sourceCSV);

                            if (destType == typeof(string))
                                value = svalue;
                            else if (destType == typeof(DateTime) || destType == typeof(DateTime?))
                                value = Util.ParseTools.ToDateTime(svalue);
                            else if (destType == typeof(decimal) || destType == typeof(decimal?))
                            {
                                value = Util.ParseTools.ToDecimal(svalue);
                                if (value == null)
                                    value = Util.ParseTools.FromTextToDecimal(svalue);
                            }
                            else if (destType == typeof(long) || destType == typeof(long?)
                                || destType == typeof(int) || destType == typeof(int?))
                                value = Util.ParseTools.ToDate(svalue);
                            else if (destType == typeof(bool) || destType == typeof(bool?))
                            {
                                if (bool.TryParse(svalue, out bool tryp))
                                    value = tryp;
                            }
                            else
                                value = svalue;
                        }
                        if (mappingProps[m].Transform == "normalize"
                            && destType == typeof(string)
                            )
                        {
                            value = DataSet.NormalizeValueForId((string)value);
                        }
                        else if (mappingProps[m].Transform == "findico"
                            && destType == typeof(string)
                            )
                        {
                            value = Lib.Validators.IcosInText((string)value).FirstOrDefault();
                        }
                        else //copy
                        { }
                        rcb.SetPropertyValue(newObj, mappingProps[m].TargetJSON, value);

                    } //for

                    string idPropName = "id";
                    string idVal = rcb.GetPropertyValue(newObj, "id")?.ToString();
                    if (string.IsNullOrEmpty(idVal))
                    {
                        idVal = rcb.GetPropertyValue(newObj, "Id")?.ToString();
                        idPropName = "Id";
                    }
                    if (string.IsNullOrEmpty(idVal))
                    {
                        idVal = rcb.GetPropertyValue(newObj, "iD")?.ToString();
                        idPropName = "iD";
                    }
                    if (string.IsNullOrEmpty(idVal))
                    {
                        idVal = rcb.GetPropertyValue(newObj, "ID")?.ToString();
                        idPropName = "ID";
                    }
                    try
                    {
                        //var debugJson = Newtonsoft.Json.JsonConvert.SerializeObject(newObj);
                        //normalize ID
                        idVal = DataSet.NormalizeValueForId(idVal);
                        rcb.SetPropertyValue(newObj, idPropName, idVal);

                        items.Add(new Tuple<object, string>(newObj, idVal));

                        model.NumOfRows++;
                    }
                    catch (DataSetException dex)
                    {
                        errors.Add(dex);
                    }
                    catch (Exception ex)
                    {
                        errors.Add(ex);
                    }
                }
            }

            try
            {

                Devmasters.Core.Batch.Manager.DoActionForAll<Tuple<object, string>>(items,
                    (item) => {

                        try
                        {
                            ds.AddData(item.Item1, item.Item2, email, true);

                        }
                        catch (Exception ex)
                        {
                            errors.Add(ex);
                        }

                        return new Devmasters.Core.Batch.ActionOutputData();
                    }, true
                    );

            }
            catch (Exception ex)
            {
                errors.Add(ex);
            }

            if (errors?.Count > 0)
            {
                HlidacStatu.Util.Consts.Logger.Error("ImportDataProcess exceptions \n"
                    + errors.Select(m => m.Message).Aggregate((f, s) => f + "\n" + s));
            }

            ViewBag.ApiResponseError = ApiResponseStatus.Error(-99, "Chyba při importu dat");
            ViewBag.Errors = errors.ToList();

            return View(model);
        }


        private string GuestBestCSVValueType(string value)
        {
            if (Util.ParseTools.ToDate(value).HasValue)
                return "date";
            else if (Util.ParseTools.ToDateTime(value).HasValue)
                return "datetime";
            else if (Util.ParseTools.ToDecimal(value).HasValue)
                return "number";
            else
                return "string";

        }


    }
}
