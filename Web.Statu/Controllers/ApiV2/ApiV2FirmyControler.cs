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
using System.Collections.Generic;
using HlidacStatu.Lib.Data;
using System.Linq.Expressions;

namespace HlidacStatu.Web.Controllers
{

    [SwaggerControllerTag("Firmy")]

    [RoutePrefix("api/v2/firmy")]
    public class ApiV2FirmyController : ApiV2AuthController
    {

        /// <summary>
        /// Vyhledá firmu v databázi Hlídače státu.
        /// </summary>
        /// <param name="ico">ico firmy</param>
        /// <returns>Ico, jméno a datová schránka</returns>
        [AuthorizeAndAudit]
        [HttpGet, Route("ico/{ico}")]
        public FirmaDTO CompanyPerIco(string ico)
        {
            try
            {
                var f = Lib.Data.Firmy.Get(ico);
                if (f.Valid == false)
                {
                    throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.NotFound, $"Firma {ico} nenalezena."));
                }
                else
                    return new FirmaDTO()
                    {
                        ICO = f.ICO,
                        Jmeno = f.Jmeno,
                        DatoveSchranky = f.DatovaSchranka
                    };

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
                            ICO = found.ICO,
                            Jmeno = found.Jmeno,
                            DatoveSchranky = found.DatovaSchranka
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

        [AuthorizeAndAudit(Roles = "KomercniLicence,PrivateApi,Admin")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet, Route("vsechny")]
        public HttpResponseMessage Vsechny()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(1024 * 500);
            string cnnStr = Devmasters.Config.GetWebConfigValue("CnnString");
            using (Devmasters.PersistLib p = new Devmasters.PersistLib())
            {

                var reader = p.ExecuteReader(cnnStr, CommandType.Text, "select ico, jmeno from firma where ISNUMERIC(ico) = 1", null);
                while (reader.Read())
                {
                    string ico = reader.GetString(0).Trim();
                    string name = reader.GetString(1).Trim();
                    if (!string.IsNullOrWhiteSpace(ico)
                        && !string.IsNullOrWhiteSpace(name)
                        && Devmasters.TextUtil.IsNumeric(ico))
                    {
                        ico = Util.ParseTools.NormalizeIco(ico);
                        sb.AppendLine(ico + "\t" + name);
                    }
                }
            }

            var res = new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new StringContent(sb.ToString()) };
            return res;
        }

        // /api/v2/firmy/social?typ=WWW&typ=Youtube
        [AuthorizeAndAudit]
        [HttpGet, Route("social")]
        public List<FirmaSocialDTO> Social([FromUri] OsobaEvent.SocialNetwork[] typ)
        {

            var socials = (typ is null || typ.Length == 0)
                ? Enum.GetNames(typeof(OsobaEvent.SocialNetwork))
                : typ.Select(t => t.ToString("G"));

            Expression<Func<OsobaEvent, bool>> socialNetworkFilter = e =>
                e.Type == (int)OsobaEvent.Types.SocialniSite
                && e.Ico.Length == 8
                && socials.Contains(e.Organizace);

            var events = OsobaEvent.GetByEvent(socialNetworkFilter)
                .GroupBy(e => e.Ico)
                .Select(g => TransformEventsToFirmaSocial(g))
                .Where(r => r != null)
                .ToList();

            return events;

        }

        private FirmaSocialDTO TransformEventsToFirmaSocial(IGrouping<string, OsobaEvent> groupedEvents)
        {
            var firma = Firma.FromIco(groupedEvents.Key);
            if (firma is null || !firma.Valid)
            {
                return null;
            }

            return new FirmaSocialDTO(firma, groupedEvents.ToList());
        }
    }
}