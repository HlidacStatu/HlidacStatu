using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Models.Apiv2;
using Swashbuckle.Swagger.Annotations;
using System.Web.Mvc;
using Newtonsoft.Json;
using HlidacStatu.Web.Models.apiv2;
using HlidacStatu.Lib.Data.External.DataSets;
using System;

namespace HlidacStatu.Web.Controllers
{
    public class ApiV2DatasetyController : GenericAuthController
    {
        // /api/v2/datasety/seznam
        [HttpGet]
        [AuthorizeAndAudit]
        [SwaggerOperation("Seznam")]
        [SwaggerResponse(statusCode: 200, type: typeof(OsobaDetailDTO), description: "Úspěšně vrácena veřejná zakázka")]
        [SwaggerResponse(statusCode: 400, type: typeof(ErrorMessage), description: "Některé z předaných parametrů byly zadané nesprávně")]
        [SwaggerResponse(statusCode: 401, type: typeof(ErrorMessage), description: "Nesprávný autorizační token")]
        [SwaggerResponse(statusCode: 404, description: "Požadovaný dokument nebyl nalezen")]
        [SwaggerResponse(statusCode: 500, description: "Došlo k interní chybě na serveru")]
        public ActionResult Seznam()
        {
            return Content(JsonConvert.SerializeObject(
                    DataSetDB.Instance.SearchData("*", 1, 100).Result,
                    Formatting.None,
                    new JsonSerializerSettings() 
                    { 
                        ContractResolver = Serialization.PublicDatasetContractResolver.Instance 
                    }),
                "application/json");
        }

        // /api/v2/datasety/detail/{id}
        [HttpGet]
        [AuthorizeAndAudit]
        [SwaggerOperation("Detail")]
        [SwaggerResponse(statusCode: 200, type: typeof(OsobaDTO), description: "Úspěšně vrácen seznam smluv")]
        [SwaggerResponse(statusCode: 400, type: typeof(ErrorMessage), description: "Některé z předaných parametrů byly zadané nesprávně")]
        [SwaggerResponse(statusCode: 401, type: typeof(ErrorMessage), description: "Nesprávný autorizační token")]
        [SwaggerResponse(statusCode: 404, description: "Žádná veřejná zakázka nenalezena")]
        [SwaggerResponse(statusCode: 500, description: "Došlo k interní chybě na serveru")]
        public ActionResult Detail(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"Hodnota id chybí.").ToJson(), "application/json");
            }

            var ds = DataSet.CachedDatasets.Get(id);
            if (ds == null)
            {
                Response.StatusCode = 404;
                return Content(new ErrorMessage($"Dataset nenalezen.").ToJson(), "application/json");
            }
            
            return Content(JsonConvert.SerializeObject(ds.Registration()), "application/json");

        }

        [HttpPost]
        [AuthorizeAndAudit]
        public ActionResult Create(Registration registration)
        {
            if (registration == null)
            {
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"Chybí data.").ToJson(), "application/json");
            }
            try
            {
                var res = DataSet.Api.Create(registration, this.User.Identity.Name);

                if (res.valid)
                    return Json(new { datasetId = res.value });
                else
                    return Json(res);
            }
            catch (DataSetException dse)
            {
                Response.StatusCode = 400;
                return Content(JsonConvert.SerializeObject(dse.APIResponse), "application/json");
            }
            catch (Exception ex)
            {
                Util.Consts.Logger.Error("Dataset API", ex);
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"Obecná chyba - {ex.Message}").ToJson(), "application/json");
            }
        }

    }
}
