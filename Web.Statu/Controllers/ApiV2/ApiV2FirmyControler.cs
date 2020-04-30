using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Models.Apiv2;
using System.Web.Http;
using Newtonsoft.Json;
using HlidacStatu.Lib.Data.External.DataSets;
using System;
using System.Linq;
using HlidacStatu.Web.Framework;
using System.Web.Http.Description;
using System.Data;
using System.Net.Http;

namespace HlidacStatu.Web.Controllers
{

    [SwaggerControllerTag("Firmy")]

    [RoutePrefix("api/v2/firmy")]
    public class ApiV2FirmyController : ApiV2AuthController
    {
        /// <summary>
        /// Vyhledá firmu v databázi Hlídače státu.
        /// </summary>
        /// <param name="jmenoFirmy">název firmy</param>
        /// <returns>Ico, jméno a datová schránka</returns>
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
            catch (HttpResponseException)
            {
                throw;
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

        [AuthorizeAndAudit()]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet, Route("vsechny")]
        public HttpResponseMessage Vsechny()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(1024 * 500);
            string cnnStr = Devmasters.Core.Util.Config.GetConfigValue("CnnString");
            using (Devmasters.Core.PersistLib p = new Devmasters.Core.PersistLib())
            {

                var reader = p.ExecuteReader(cnnStr, CommandType.Text, "select ico, jmeno from firma where ISNUMERIC(ico) = 1", null);
                while (reader.Read())
                {
                    string ico = reader.GetString(0).Trim();
                    string name = reader.GetString(1).Trim();
                    if (Devmasters.Core.TextUtil.IsNumeric(ico))
                    {
                        ico = Util.ParseTools.NormalizeIco(ico);
                        sb.AppendLine(ico + "\t" + name);
                    }
                }
            }

            var res = new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new StringContent(sb.ToString()) };
            return res;
        }

    }
}
