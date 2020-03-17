using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Models.Apiv2;
using Swashbuckle.Swagger.Annotations;
using System.Web.Mvc;
using Newtonsoft.Json;
using HlidacStatu.Web.Models.apiv2;
using HlidacStatu.Lib.Data.External.DataSets;
using System;
using System.Web;
using System.IO;
using HlidacStatu.Web.Framework;
using System.Linq;

namespace HlidacStatu.Web.Controllers
{
    [RoutePrefix("api/v2/firmy")]
    public class ApiV2FirmyController : GenericAuthController
    {

        [AuthorizeAndAudit]
        [HttpGet, Route("{companyName}")]
        public ActionResult CompanyID(string companyName)
        {
            try
            {
                if (string.IsNullOrEmpty(companyName))
                {
                    Response.StatusCode = 400;
                    return Content(new ErrorMessage($"Hodnota companyName chybí.").ToJson(), "application/json");
                }
                else
                {
                    var name = Lib.Data.Firma.JmenoBezKoncovky(companyName);
                    var found = Lib.Data.Firma.Search.FindAll(name, 1).FirstOrDefault();
                    if (found == null)
                    {
                        Response.StatusCode = 404;
                        return Content(new ErrorMessage($"Firma {companyName} nenalezena.").ToJson(), "application/json");
                    }
                    else
                        return Content(JsonConvert.SerializeObject(
                            new 
                            { 
                                ICO = found.ICO, Jmeno = found.Jmeno, DatovaSchranka = found.DatovaSchranka 
                            }), 
                            "application/json");
                }
            }
            catch (DataSetException dse)
            {
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"{dse.APIResponse.error.description}").ToJson(), "application/json");
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
