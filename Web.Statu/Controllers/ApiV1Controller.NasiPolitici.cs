using HlidacStatu.Lib.Data.External.DataSets;
using HlidacStatu.Lib.Data.External.DataSets;
using HlidacStatu.Util;
using HlidacStatu.Lib.Data;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;

namespace HlidacStatu.Web.Controllers
{
    public partial class ApiV1Controller : Controllers.GenericAuthController
    {
        

        public ActionResult NasiPolitici_GetData(string id)
        {
            if (!Framework.ApiAuth.IsApiAuth(this,
                parameters: new Framework.ApiCall.CallParameter[] {
                    new Framework.ApiCall.CallParameter("id", id),
                })
                .Authentificated)
            {
                //Response.StatusCode = 401;
                return Json(ApiResponseStatus.ApiUnauthorizedAccess, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var o = Osoba.GetByNameId(id);
                if (o == null)
                {
                    //Response.StatusCode = 404;
                    return Json(ApiResponseStatus.Error(404,"Politik not found"), JsonRequestBehavior.AllowGet);
                }
                if (o.StatusOsoby() != Osoba.StatusOsobyEnum.Politik )
                {
                    //Response.StatusCode = 404;
                    return Json(ApiResponseStatus.Error(404, "Person is not marked as politician"), JsonRequestBehavior.AllowGet);
                }

                var statDescription =
                    HlidacStatu.Util.InfoFact.RenderInfoFacts(
                        o.InfoFacts().Where(i => i.Level != HlidacStatu.Util.InfoFact.ImportanceLevel.Stat).ToArray()
                            , 4, true, true, "", "<p>{0}</p>");

                int[] types = {
                    (int)HlidacStatu.Lib.Data.OsobaEvent.Types.VolenaFunkce,
                    (int)HlidacStatu.Lib.Data.OsobaEvent.Types.PolitickaPracovni,
                    (int)HlidacStatu.Lib.Data.OsobaEvent.Types.Politicka,
                    (int)HlidacStatu.Lib.Data.OsobaEvent.Types.VerejnaSpravaJine,
                    (int)HlidacStatu.Lib.Data.OsobaEvent.Types.VerejnaSpravaPracovni,
                };
                var funkceOsoba = o.Description(true,
                        m => types.Contains(m.Type),
                        20);

                string osobaInsQuery = $"{{0}}.osobaId:{o.NameId}";
                //var oinsRes = HlidacStatu.Lib.Data.Insolvence.Insolvence.SimpleSearch("osobaid:" + Model.NameId, 1, 5, (int)HlidacStatu.Lib.ES.InsolvenceSearchResult.InsolvenceOrderResult.LatestUpdateDesc, false, false);
                var oinsDluznik = HlidacStatu.Lib.Data.Insolvence.Insolvence.SimpleSearch(string.Format(osobaInsQuery, "dluznici"), 1, 1, (int)HlidacStatu.Lib.ES.InsolvenceSearchResult.InsolvenceOrderResult.FastestForScroll, false, false);
                var oinsVeritel = HlidacStatu.Lib.Data.Insolvence.Insolvence.SimpleSearch(string.Format(osobaInsQuery, "veritele"), 1, 1, (int)HlidacStatu.Lib.ES.InsolvenceSearchResult.InsolvenceOrderResult.FastestForScroll, false, false);
                var oinsSpravce = HlidacStatu.Lib.Data.Insolvence.Insolvence.SimpleSearch(string.Format(osobaInsQuery, "spravci"), 1, 1, (int)HlidacStatu.Lib.ES.InsolvenceSearchResult.InsolvenceOrderResult.FastestForScroll, false, false);

                Dictionary<string, long> oinsolv = new Dictionary<string, long>();
                oinsolv.Add("dluznici|dlužník|dlužníka|dlužníkem", oinsDluznik.Total);
                oinsolv.Add("veritele|věřitel|věřitele|veřitelem", oinsVeritel.Total);
                oinsolv.Add("spravci|insolvenční správce|insolvenčního správce|insolvenčním správcem", oinsSpravce.Total);

                var insRes = HlidacStatu.Lib.Data.Insolvence.Insolvence.SimpleSearch("osobaid:" + o.NameId, 1, 5, (int)HlidacStatu.Lib.ES.InsolvenceSearchResult.InsolvenceOrderResult.LatestUpdateDesc, false, false);
                var insDluznik = HlidacStatu.Lib.Data.Insolvence.Insolvence.SimpleSearch("osobaiddluznik:" + o.NameId, 1, 1, (int)HlidacStatu.Lib.ES.InsolvenceSearchResult.InsolvenceOrderResult.FastestForScroll, false, false);
                var insVeritel = HlidacStatu.Lib.Data.Insolvence.Insolvence.SimpleSearch("osobaidveritel:" + o.NameId, 1, 1, (int)HlidacStatu.Lib.ES.InsolvenceSearchResult.InsolvenceOrderResult.FastestForScroll, false, false);
                var insSpravce = HlidacStatu.Lib.Data.Insolvence.Insolvence.SimpleSearch("osobaidspravce:" + o.NameId, 1, 1, (int)HlidacStatu.Lib.ES.InsolvenceSearchResult.InsolvenceOrderResult.FastestForScroll, false, false);

                Dictionary<string, long> insolv = new Dictionary<string, long>();
                insolv.Add("dluznik|dlužník|dlužníka|dlužníkem", insDluznik.Total);
                insolv.Add("veritel|věřitel|věřitele|veřitelem", insVeritel.Total);
                insolv.Add("spravce|insolvenční správce|insolvenčního správce|insolvenčním správcem", insSpravce.Total);

                var photo = o.GetPhotoUrl(false);

                var sponzorstvi = o.Events(m => m.Type == (int)HlidacStatu.Lib.Data.OsobaEvent.Types.Sponzor)
                    .Select(m => new
                                {
                                    strana = m.Organizace,
                                    hodnotaDaru = m.AddInfoNum,
                                    rok = m.DatumOd?.Year,
                                    zdroj = m.Zdroj
                                }
                                ).ToArray();

                var result = new
                {
                    id = o.NameId,
                    titulPred = o.TitulPred,
                    titulPo = o.TitulPo,
                    jmeno = o.Jmeno,
                    prijmeni = o.Prijmeni,
                    narozeni = o.Narozeni,
                    umrti = o.Umrti,
                    status = o.StatusOsoby().ToString(),
                    photo = photo,
                    popis = statDescription,
                    funkce = funkceOsoba,
                    osoba_v_InsR_dluznik = oinsDluznik.Total,
                    osoba_v_InsR_veritel = oinsVeritel.Total,
                    osoba_v_InsR_spravce = oinsSpravce.Total,
                    firmy_osoby_v_InsR_dluznik = insDluznik.Total,
                    firmy_osoby_v_InsR_veritel = insVeritel.Total,
                    firmy_osoby_v_InsR_spravce = insSpravce.Total,
                    zdroj = o.GetUrl(false),
                    sponzoring = sponzorstvi
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }


    }
}


