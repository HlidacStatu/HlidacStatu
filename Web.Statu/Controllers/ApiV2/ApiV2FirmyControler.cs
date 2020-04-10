using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Models.Apiv2;
using System.Web.Http;
using Newtonsoft.Json;
using HlidacStatu.Lib.Data.External.DataSets;
using System;
using System.Linq;

namespace HlidacStatu.Web.Controllers
{
    [RoutePrefix("api/v2/firmy")]
    public class ApiV2FirmyController : ApiV2AuthController
    {

        [AuthorizeAndAudit]
        [HttpGet, Route("{jmenoFirmy}")]
        public FirmaDTO CompanyID(string jmenoFirmy)
        {
            try
            {
                if (string.IsNullOrEmpty(jmenoFirmy))
                {
                    throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Hodnota companyName chybí."));
                }
                else
                {
                    var name = Lib.Data.Firma.JmenoBezKoncovky(jmenoFirmy);
                    var found = Lib.Data.Firma.Search.FindAll(name, 1).FirstOrDefault();
                    if (found == null)
                    {
                        throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.NotFound, $"Firma {jmenoFirmy} nenalezena."));
                    }
                    else
                        return new FirmaDTO()
                        { 
                            ICO = found.ICO, Jmeno = found.Jmeno, DatoveSchranky = found.DatovaSchranka 
                        };
                }
            }
            catch (DataSetException dse)
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"{dse.APIResponse.error.description}"));
            }
            catch (Exception ex)
            {
                Util.Consts.Logger.Error("Dataset API", ex);
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Obecná chyba - {ex.Message}"));
            }
        }


    }
}
