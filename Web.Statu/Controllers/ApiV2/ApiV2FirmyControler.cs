using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Models.Apiv2;
using System.Web.Mvc;
using Newtonsoft.Json;
using HlidacStatu.Lib.Data.External.DataSets;
using System;
using System.Linq;

namespace HlidacStatu.Web.Controllers
{
    [RoutePrefix("api/v2/firmy")]
    public class ApiV2FirmyController : GenericAuthController
    {

        [AuthorizeAndAudit]
        [HttpGet, Route("{jmenoFirmy}")]
        public ActionResult CompanyID(string jmenoFirmy)
        {
            try
            {
                if (string.IsNullOrEmpty(jmenoFirmy))
                {
                    Response.StatusCode = 400;
                    return Content(new ErrorMessage($"Hodnota companyName chybí.").ToJson(), "application/json");
                }
                else
                {
                    var name = Lib.Data.Firma.JmenoBezKoncovky(jmenoFirmy);
                    var found = Lib.Data.Firma.Search.FindAll(name, 1).FirstOrDefault();
                    if (found == null)
                    {
                        Response.StatusCode = 404;
                        return Content(new ErrorMessage($"Firma {jmenoFirmy} nenalezena.").ToJson(), "application/json");
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
