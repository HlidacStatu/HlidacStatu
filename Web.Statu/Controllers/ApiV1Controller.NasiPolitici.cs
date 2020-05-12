using HlidacStatu.Lib.Data.External.DataSets;
using HlidacStatu.Util;
using HlidacStatu.Lib.Data;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace HlidacStatu.Web.Controllers
{
    public partial class ApiV1Controller : Controllers.GenericAuthController
    {
        class FindPersonDTO
        {
            public string NameId { get; set; }
            public string Jmeno { get; set; }
            public string Prijmeni { get; set; }
            public string JmenoAscii { get; set; }
            public string PrijmeniAscii { get; set; }
            public int? RokNarozeni { get; set; }
            public int? RokUmrti { get; set; }
            public string Aktpolstr { get; set; }
            public int? Pocet { get; set; }
        }

        class StatementDTO
        {
            public string Type { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public dynamic LegalBusinessAssociates { get; set; }
            public dynamic OrganizationMember { get; set; }
        }

        public ActionResult NasiPolitici_GetList()
        {
            if (!Framework.ApiAuth.IsApiAuth(this)
                .Authentificated)
            {
                Response.StatusCode = 401;
                return Json(ApiResponseStatus.ApiUnauthorizedAccess, JsonRequestBehavior.AllowGet);
            }
            else
            {
                using (var db = new HlidacStatu.Lib.Data.DbEntities())
                {
                    string sql = @"
                        select distinct os.NameId, os.Jmeno, os.Prijmeni
                             , os.JmenoAscii, os.PrijmeniAscii, year(os.Narozeni) RokNarozeni, year(os.Umrti) RokUmrti
	                         , FIRST_VALUE(oes.organizace) OVER(partition by oes.osobaid order by oes.datumod desc) Aktpolstr
	                         , oec.pocet
                          from Osoba os
                          left join OsobaEvent oes on os.InternalId = oes.OsobaId and oes.AddInfo in (N'člen strany',N'předseda strany',N'místopředseda strany') and oes.Type = 7
                          left join (select COUNT(pk) pocet, OsobaId from OsobaEvent group by osobaid) oec on oec.OsobaId = os.InternalId
                         where os.Status = 3";

                    var result = db.Database.SqlQuery<FindPersonDTO>(sql)
                        .Select( r => new
                        {
                            id = r.NameId,
                            name = r.Jmeno,
                            surname = r.Prijmeni,
                            asciiName = r.JmenoAscii,
                            asciiSurname = r.PrijmeniAscii,
                            birthYear = r.RokNarozeni,
                            deathYear = r.RokUmrti,
                            currentParty = r.Aktpolstr,
                            eventCount = r.Pocet
                        });

                    string osoby = JsonConvert.SerializeObject(result);

                    return Content(osoby, "application/json");
                }

            }
        }

        public ActionResult NasiPolitici_GetData(string _id)
        {
            string id = _id;

            if (!Framework.ApiAuth.IsApiAuth(this,
                parameters: new Framework.ApiCall.CallParameter[] {
                    new Framework.ApiCall.CallParameter("id", id),
                })
                .Authentificated)
            {
                Response.StatusCode = 401;
                return Json(ApiResponseStatus.ApiUnauthorizedAccess, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var o = Osoba.GetByNameId(id);
                if (o == null)
                {
                    Response.StatusCode = 404;
                    return Json(ApiResponseStatus.Error(404,"Politik not found"), JsonRequestBehavior.AllowGet);
                }
                if (o.StatusOsoby() != Osoba.StatusOsobyEnum.Politik )
                {
                    Response.StatusCode = 404;
                    return Json(ApiResponseStatus.Error(404, "Person is not marked as politician"), JsonRequestBehavior.AllowGet);
                }

                var vazby = o.AktualniVazby(Relation.AktualnostType.Nedavny)
                    .Where(v => v.Distance == 1 && v.To?.Type == Graph.Node.NodeType.Company)
                    .Take(10)
                    .Select(v => new
                    {
                        company = Firma.FromIco(v.To.Id).Jmeno,
                        ico = v.To.Id,
                        since = v.RelFrom,
                        until = v.RelTo,
                        description = v.Descr

                    }).ToList();
                

                var statDescription =
                    InfoFact.RenderInfoFacts(
                        o.InfoFacts().Where(i => i.Level != InfoFact.ImportanceLevel.Stat).ToArray()
                        , 4, true, true, "", "{0}");

                var angazovanost =
                    InfoFact.RenderInfoFacts(
                        o.InfoFacts().Where(m => m.Level == InfoFact.ImportanceLevel.Stat).ToArray()
                        , 4, true, true, "", "{0}");


                int[] types = {
                    (int)OsobaEvent.Types.VolenaFunkce,
                    (int)OsobaEvent.Types.PolitickaPracovni,
                    (int)OsobaEvent.Types.Politicka,
                    (int)OsobaEvent.Types.VerejnaSpravaJine,
                    (int)OsobaEvent.Types.VerejnaSpravaPracovni,
                };
                //var funkceOsoba = o.Description(true,
                //        m => types.Contains(m.Type),
                //        20);

                var roleOsoba = o.Events(m => 
                        types.Contains(m.Type)
                        && m.Status != (int)OsobaEvent.Statuses.NasiPoliticiSkryte)
                    .Select(e => new { 
                        role = e.AddInfo,
                        dateFrom = e.DatumOd,
                        dateTo = e.DatumDo,
                        organisation = e.Organizace
                    })
                    .ToArray();

                string registrOznameni = o.Events(m => 
                            m.Type == (int)OsobaEvent.Types.CentralniRegistrOznameni
                            && m.Status != (int)OsobaEvent.Statuses.NasiPoliticiSkryte)
                        .Select(m => m.AddInfo).FirstOrDefault();

                List<StatementDTO> statements = null;
                if (!string.IsNullOrWhiteSpace(registrOznameni))
                {

                    var ds = DataSet.CachedDatasets.Get("centralniregistroznameni");
                    var value = ds.GetDataObj(registrOznameni);


                    //StatementDTO[] statements = (StatementDTO[])value.statements;
                    statements = value.statements.ToObject<List<StatementDTO>>();
                }

                string osobaInsQuery = $"{{0}}.osobaId:{o.NameId}";
                //var oinsRes = HlidacStatu.Lib.Data.Insolvence.Insolvence.SimpleSearch("osobaid:" + Model.NameId, 1, 5, (int)HlidacStatu.Lib.Searching.InsolvenceSearchResult.InsolvenceOrderResult.LatestUpdateDesc, false, false);
                //query: dluznici.osobaId:{o.NameId}
                var oinsDluznik = Lib.Data.Insolvence.Insolvence.SimpleSearch(string.Format(osobaInsQuery, "dluznici"), 1, 1, (int)Lib.Searching.InsolvenceSearchResult.InsolvenceOrderResult.FastestForScroll, false, false);
                //query: veritele.osobaId:{o.NameId}
                var oinsVeritel = Lib.Data.Insolvence.Insolvence.SimpleSearch(string.Format(osobaInsQuery, "veritele"), 1, 1, (int)Lib.Searching.InsolvenceSearchResult.InsolvenceOrderResult.FastestForScroll, false, false);
                //query: spravci.osobaId:{o.NameId}
                var oinsSpravce = Lib.Data.Insolvence.Insolvence.SimpleSearch(string.Format(osobaInsQuery, "spravci"), 1, 1, (int)Lib.Searching.InsolvenceSearchResult.InsolvenceOrderResult.FastestForScroll, false, false);

                Dictionary<string, long> oinsolv = new Dictionary<string, long>();
                oinsolv.Add("dluznici|dlužník|dlužníka|dlužníkem", oinsDluznik.Total);
                oinsolv.Add("veritele|věřitel|věřitele|veřitelem", oinsVeritel.Total);
                oinsolv.Add("spravci|insolvenční správce|insolvenčního správce|insolvenčním správcem", oinsSpravce.Total);

                var insRes = Lib.Data.Insolvence.Insolvence.SimpleSearch("osobaid:" + o.NameId, 1, 5, (int)Lib.Searching.InsolvenceSearchResult.InsolvenceOrderResult.LatestUpdateDesc, false, false);
                var insDluznik = Lib.Data.Insolvence.Insolvence.SimpleSearch("osobaiddluznik:" + o.NameId, 1, 1, (int)Lib.Searching.InsolvenceSearchResult.InsolvenceOrderResult.FastestForScroll, false, false);
                var insVeritel = Lib.Data.Insolvence.Insolvence.SimpleSearch("osobaidveritel:" + o.NameId, 1, 1, (int)Lib.Searching.InsolvenceSearchResult.InsolvenceOrderResult.FastestForScroll, false, false);
                var insSpravce = Lib.Data.Insolvence.Insolvence.SimpleSearch("osobaidspravce:" + o.NameId, 1, 1, (int)Lib.Searching.InsolvenceSearchResult.InsolvenceOrderResult.FastestForScroll, false, false);

                Dictionary<string, long> insolv = new Dictionary<string, long>();
                insolv.Add("dluznik|dlužník|dlužníka|dlužníkem", insDluznik.Total);
                insolv.Add("veritel|věřitel|věřitele|veřitelem", insVeritel.Total);
                insolv.Add("spravce|insolvenční správce|insolvenčního správce|insolvenčním správcem", insSpravce.Total);

                var photo = o.GetPhotoUrl(false);

                var sponzorstvi = o.Events(m => 
                        m.Type == (int)OsobaEvent.Types.Sponzor
                        && m.Status != (int)OsobaEvent.Statuses.NasiPoliticiSkryte)
                    .Select(m => new
                                {
                                    party = m.Organizace,
                                    donatedAmount = m.AddInfoNum,
                                    year = m.DatumOd?.Year,
                                    source = m.Zdroj
                                }
                                ).ToArray();

                var insPerson = new
                {
                    debtorCount = oinsDluznik.Total,
                    debtorLink = $"https://www.hlidacstatu.cz/insolvence/hledat?Q=dluznici.osobaId:{o.NameId}",
                    creditorCount = oinsVeritel.Total,
                    creditorLink = $"https://www.hlidacstatu.cz/insolvence/hledat?Q=veritele.osobaId:{o.NameId}",
                    bailiffCount = oinsSpravce.Total,
                    bailiffLink = $"https://www.hlidacstatu.cz/insolvence/hledat?Q=spravci.osobaId:{o.NameId}"
                };

                var insCompany = new
                {
                    debtorCount = insDluznik.Total,
                    debtorLink = $"https://www.hlidacstatu.cz/insolvence/hledat?Q=osobaiddluznik:{o.NameId}",
                    creditorCount = insVeritel.Total,
                    creditorLink = $"https://www.hlidacstatu.cz/insolvence/hledat?Q=osobaidveritel:{o.NameId}",
                    bailiffCount = insSpravce.Total,
                    bailiffLink = $"https://www.hlidacstatu.cz/insolvence/hledat?Q=osobaidspravce:{o.NameId}"
                };

                var lastUpdate = o.Events(m => true)
                    .OrderByDescending(e => e.Created)
                    .Select(e => e.Created)
                    .FirstOrDefault();


                var result = new
                {
                    id = o.NameId,
                    lastUpdate = lastUpdate,
                    namePrefix = o.TitulPred,
                    nameSuffix = o.TitulPo,
                    name = o.Jmeno,
                    surname = o.Prijmeni,
                    birthDate = o.Narozeni,
                    deathDate = o.Umrti,
                    status = o.StatusOsoby().ToString(),
                    hasPhoto = o.HasPhoto(),
                    photo = photo,
                    description = statDescription,
                    companyConnection = angazovanost,
                    notificationRegisterId = registrOznameni,
                    notificationRegisterStatements = statements,
                    //funkce = funkceOsoba,
                    roles = roleOsoba,
                    insolvencyPerson = insPerson,
                    insolvencyCompany = insCompany,
                    source = o.GetUrl(false),
                    sponsor = sponzorstvi,
                    currentParty = o.CurrentPoliticalParty(),
                    contacts = o.GetSocialContact(),
                    connections = vazby
                };

                return Content(JsonConvert.SerializeObject(result), "application/json");
                // return Json(result, JsonRequestBehavior.AllowGet);
            }
        }


    }
}


