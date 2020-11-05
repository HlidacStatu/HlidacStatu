using HlidacStatu.Lib.Searching;
using HlidacStatu.Lib.Searching.Rules;
using HlidacStatu.Util.Cache;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HlidacStatu.Lib.Data.VZ
{
    public partial class VerejnaZakazka
    {
        public static class Searching
        {
            private static string regex = "[^/]*\r\n/(?<regex>[^/]*)/\r\n[^/]*\r\n";

            private static RegexOptions options = (
                RegexOptions.IgnorePatternWhitespace 
                | RegexOptions.Multiline
                | RegexOptions.IgnoreCase);

            private static Regex regFindRegex = new Regex(regex, options);

            public static IRule[] Rules = new IRule[] {
                    new OsobaId("osobaid:","ico:" ),
                    new OsobaId("osobaiddodavatel:","icododavatel:" ),
                    new OsobaId("osobaidzadavatel:","icozadavatel:" ),

                    new Holding("holding:","ico:" ),
                    new Holding("holdingdodavatel:","icododavatel:" ),
                    new Holding("holdingzadavatel:","icozadavatel:" ),
                    new Holding("holdingprijemce:","icododavatel:" ),
                    new Holding("holdingplatce:","icozadavatel:" ),

                    new VZ_CPV(),
                    new VZ_Oblast(),
                    new VZ_Form(),

                    new TransformPrefixWithValue("zahajeny:","stavVZ:<=100","1" ),

                    new TransformPrefixWithValue("ico:","(zadavatel.iCO:${q} OR dodavatele.iCO:${q}) ",null ),
                    new TransformPrefix("icododavatel:","dodavatele.iCO:",null ),
                    new TransformPrefix("icoprijemce:","dodavatele.iCO:",null ),
                    new TransformPrefix("icozadavatel:","zadavatel.iCO:",null ),
                    new TransformPrefix("icoplatce:","zadavatel.iCO:",null ),
                    new TransformPrefix("jmenoprijemce:","dodavatele.jmeno:",null ),
                    new TransformPrefix("jmenoplatce:","zadavatel.jmeno:",null ),
                    new TransformPrefix("id:","id:",null ),

                    new TransformPrefixWithValue("popis:","(nazevZakazky:${q} OR popisZakazky:${q})  ",null ),

                    new TransformPrefixWithValue("cena:","(konecnaHodnotaBezDPH:<=${q} OR odhadovanaHodnotaBezDPH:<=${q}) ","<=\\d" ),
                    new TransformPrefixWithValue("cena:","(konecnaHodnotaBezDPH:>=${q} OR odhadovanaHodnotaBezDPH:>=${q}) ",">=\\d" ),
                    new TransformPrefixWithValue("cena:","(konecnaHodnotaBezDPH:<${q} OR odhadovanaHodnotaBezDPH:<${q}) ","<\\d" ),
                    new TransformPrefixWithValue("cena:","(konecnaHodnotaBezDPH:>${q} OR odhadovanaHodnotaBezDPH:>${q}) ",">\\d" ),
                    new TransformPrefixWithValue("cena:","(konecnaHodnotaBezDPH:${q} OR odhadovanaHodnotaBezDPH:${q}) ",null ),

                    new TransformPrefix("zverejneno:","datumUverejneni:", "[<>]?[{\\[]+" ),
                    new TransformPrefixWithValue("zverejneno:","datumUverejneni:[${q} TO ${q}||+1d}", "\\d+" ),
                    new TransformPrefix("podepsano:","datumUzavreniSmlouvy:", "[<>]?[{\\[]+" ),
                    new TransformPrefixWithValue("podepsano:","datumUzavreniSmlouvy:[${q} TO ${q}||+1d}", "\\d+" ),

                    new TransformPrefix("text:","prilohy.plainTextContent:"  ,null),
            };

            [Devmasters.Enums.ShowNiceDisplayName]
            [Devmasters.Enums.Sortable(Devmasters.Enums.SortableAttribute.SortAlgorithm.BySortValueAndThenAlphabetically)]
            public enum CPVSkupiny
            {
                [Devmasters.Enums.NiceDisplayName("IT, HW, SW"), Devmasters.Enums.SortValue(10)]
                IT = 1,

                [Devmasters.Enums.NiceDisplayName("Stavebnictví"), Devmasters.Enums.SortValue(10)]
                Stav = 2,

                [Devmasters.Enums.NiceDisplayName("Doprava"), Devmasters.Enums.SortValue(10)]
                Doprava = 3,

                [Devmasters.Enums.NiceDisplayName("Strojírenské produkty"), Devmasters.Enums.SortValue(10)]
                Stroje = 4,

                [Devmasters.Enums.NiceDisplayName("Telekomunikace"), Devmasters.Enums.SortValue(10)]
                Telco = 5,

                [Devmasters.Enums.NiceDisplayName("Zdravotnictví, medicína"), Devmasters.Enums.SortValue(10)]
                Zdrav = 6,

                [Devmasters.Enums.NiceDisplayName("Potraviny"), Devmasters.Enums.SortValue(10)]
                Jidlo = 7,

                [Devmasters.Enums.NiceDisplayName("Bezpečnost, vojsko, policie"), Devmasters.Enums.SortValue(10)]
                Bezpecnost = 8,

                [Devmasters.Enums.NiceDisplayName("Přírodní zdroje"), Devmasters.Enums.SortValue(10)]
                PrirodniZdroj = 9,

                [Devmasters.Enums.NiceDisplayName("Energetika"), Devmasters.Enums.SortValue(10)]
                Energie = 10,

                [Devmasters.Enums.NiceDisplayName("Zemědělství a lesnictví"), Devmasters.Enums.SortValue(10)]
                Agro = 11,

                [Devmasters.Enums.NiceDisplayName("Kancelářské služby a materiál"), Devmasters.Enums.SortValue(10)]
                Kancelar = 12,

                [Devmasters.Enums.NiceDisplayName("Řemeslné služby a výrobky"), Devmasters.Enums.SortValue(10)]
                Remeslo = 13,

                [Devmasters.Enums.NiceDisplayName("Zdravotní, sociální a vzdělávací služby"), Devmasters.Enums.SortValue(10)]
                Social = 14,

                [Devmasters.Enums.NiceDisplayName("Finanční služby"), Devmasters.Enums.SortValue(10)]
                Finance = 15,

                [Devmasters.Enums.NiceDisplayName("Právnické služby"), Devmasters.Enums.SortValue(10)]
                Legal = 16,

                [Devmasters.Enums.NiceDisplayName("Technické služby	"), Devmasters.Enums.SortValue(10)]
                TechSluzby = 17,

                [Devmasters.Enums.NiceDisplayName("Výzkum"), Devmasters.Enums.SortValue(10)]
                Vyzkum = 18,

                [Devmasters.Enums.NiceDisplayName("Marketing & PR"), Devmasters.Enums.SortValue(10)]
                Marketing = 20,

                [Devmasters.Enums.NiceDisplayName("Ostatní"), Devmasters.Enums.SortValue(99)]
                Jine = 19
            }

            //source
            public static Dictionary<string, string> cpvSearchGroups = new Dictionary<string, string> {
                {"it", "302,72,64216,791211,48,50312,516,"},
                {"stav","44,45,71,75123,34946,351131,4331,433,436,507,51541,7011,79993,909112," }, //stavebnictvi
                {"doprava","34,60,63,09132,091342,0913423,09211,501,502,5114"}, //doprava
                {"stroje","16,31,38,42,43,505,515"}, //strojírenské produkty
                {"telco","32,64,5033,513,"}, //telco
                {"zdrav","33,504,514,"}, //medicínské vybavení
                {"jidlo","03,15,4111"}, //potraviny,
                {"bezpecnost","35,506,5155,519,"}, //bezpecnost, vojsko, policie
                {"prirodnizdroj","14,24,41"},
                {"energie","09,65,3112,3113,3114,45251,71314,713231,"}, //energetika
                {"agro","16,77,5152"}, //zemedelstvi a lest
                {"kancelar","22,301,39,795,796,797,798,799,300,503"},//kancelářský materiál
                {"remeslo","18,19,374,375,378,373,3700"}, //oděvy, obuv a jiné vybavení
                {"social","80,85,92,98"}, //zdravotní, sociální a vzdělávací služby
                {"finance","66,792,794,"}, //financni služby
                {"legal","70,791,7524"}, //právní, poradenské a jiné komerční služby
                {"techsluzby","500,51,76,90"}, //technické služby
                {"vyzkum","73,79315,452146,45214,3897,3829,3012513"},
                {"marketing","7934,79341,793411,793412,793414,793415,79342,793421,793422,793423,7934231,79342311,7934232,79342321,794,79413,79416,794161"},
                {"jine","75,55,793,790,508"},
            };

            public static string[] CPVOblastToCPV(CPVSkupiny skupina)
            {
                var key = skupina.ToString().ToLower();
                if (cpvSearchGroups.ContainsKey(key))
                    return cpvSearchGroups[key].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                else
                    throw new ArgumentOutOfRangeException("CPVSkupinaToCPV failed for " + skupina);
            }

            public static string[] CPVOblastToCPV(string skupinaJmeno)
            {
                if (string.IsNullOrWhiteSpace(skupinaJmeno))
                    return null;
                if (Devmasters.TextUtil.IsNumeric(skupinaJmeno))
                {
                    if (int.TryParse(skupinaJmeno, out int iSkup))
                    {
                        try
                        {
                            return CPVOblastToCPV((CPVSkupiny)iSkup);
                        }
                        catch (Exception)
                        {
                            return null;
                        }
                    }
                }
                var key = skupinaJmeno.ToLower();
                if (cpvSearchGroups.ContainsKey(key))
                    return cpvSearchGroups[key].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                else
                    return null;
            }

            public static string NormalizeOblastValue(string value)
            {
                if (String.IsNullOrWhiteSpace(value))
                    return string.Empty;
                if (Devmasters.TextUtil.IsNumeric(value))
                {
                    int iSkup;
                    if (int.TryParse(value, out iSkup))
                    {
                        try
                        {
                            var skupina = (CPVSkupiny)iSkup;
                            return skupina.ToString();
                        }
                        catch (Exception)
                        {
                            return string.Empty;
                        }
                    }
                }
                return value;
            }

            public static QueryContainer GetRawQuery(string jsonQuery)
            {
                QueryContainer qc = null;
                if (string.IsNullOrEmpty(jsonQuery))
                    qc = new QueryContainerDescriptor<VerejnaZakazka>().MatchAll();
                else
                {
                    qc = new QueryContainerDescriptor<VerejnaZakazka>().Raw(jsonQuery);
                }

                return qc;
            }

            private static string[] queryShorcuts = new string[] {
                "ico:",
                "icododavatel:",
                "icoprijmece:",
                "icozadavatel:",
                "icoplatce:",
                "jmenozadavatel:",
                "jmenododavatel:",
                "id:",
                "osobaid:","osobaiddodavatel:","osobaidzadavatel:",
                "cpv:",
                "form:",
                "zahajeny:",
                "popis:",
                "cena:",
                "zverejneno:",
                "podepsano:",
                "text:",
                "oblast:",
                "holding:","holdingdodavatel:","holdingzadavatel:", "holdingprijemce:","holdingplatce:",
            };

            private static string[] queryOperators = new string[] {
                "AND","OR"
            };

            public static QueryContainer GetSimpleQuery(VerejnaZakazkaSearchData searchdata)
            {
                var query = searchdata.Q?.Trim();
                string modifiedQ = query; // Search.Tools.FixInvalidQuery(query, queryShorcuts, queryOperators) ?? "";
                //check invalid query ( tag: missing value)

                if (searchdata.Zahajeny)
                    modifiedQ = Tools.ModifyQueryAND(modifiedQ, "zahajeny:1");

                if (!string.IsNullOrWhiteSpace(searchdata.Oblast))
                {
                    var oblValue = NormalizeOblastValue(searchdata.Oblast);
                    if (!string.IsNullOrEmpty(oblValue))
                        modifiedQ = Tools.ModifyQueryAND(modifiedQ, "oblast:" + oblValue);
                }

                var qc = SimpleQueryCreator.GetSimpleQuery<VerejnaZakazka>(modifiedQ, Rules);

                return qc;
            }

            public static VerejnaZakazkaSearchData SimpleSearch(string query, string[] cpv,
                int page, int pageSize, string order, bool Zahajeny = false, bool withHighlighting = false,
                bool exactNumOfResults = false)
            {
                return SimpleSearch(
                    new VerejnaZakazkaSearchData()
                    {
                        Q = query,
                        OrigQuery = query,
                        CPV = cpv,
                        Page = page,
                        PageSize = pageSize,
                        Order = Devmasters.TextUtil.NormalizeToNumbersOnly(order),
                        Zahajeny = Zahajeny,
                        ExactNumOfResults = exactNumOfResults
                    }, withHighlighting: withHighlighting
                    );
            }

            public static VerejnaZakazkaSearchData SimpleSearch(
                VerejnaZakazkaSearchData search,
                AggregationContainerDescriptor<VerejnaZakazka> anyAggregation = null,
                bool logError = true, bool fixQuery = true, ElasticClient client = null,
                bool withHighlighting = false)
            {
                if (client == null)
                    client = HlidacStatu.Lib.ES.Manager.GetESClient_VZ();

                string query = search.Q ?? "";

                int page = search.Page - 1;
                if (page < 0)
                    page = 0;

                AggregationContainerDescriptor<VerejnaZakazka> baseAggrDesc = null;
                baseAggrDesc = anyAggregation == null ?
                            null //new AggregationContainerDescriptor<VerejnaZakazka>().Sum("sumKc", m => m.Field(f => f.Castka))
                        : anyAggregation;

                Func<AggregationContainerDescriptor<VerejnaZakazka>, AggregationContainerDescriptor<VerejnaZakazka>> aggrFunc
                    = (aggr) => { return baseAggrDesc; };

                Devmasters.DT.StopWatchEx sw = new Devmasters.DT.StopWatchEx();
                sw.Start();

                if (fixQuery)
                {
                    search.OrigQuery = query;
                    query = Lib.Searching.Tools.FixInvalidQuery(query, queryShorcuts, queryOperators);
                }

                search.Q = query;
                ISearchResponse<VerejnaZakazka> res = null;
                try
                {
                    res = client
                        .Search<VerejnaZakazka>(s => s
                            .Size(search.PageSize)
                            .Source(so => so.Excludes(ex => ex.Field("dokumenty.plainText")))
                            .From(page * search.PageSize)
                            .Query(q => GetSimpleQuery(search))
                            .Sort(ss => GetSort(search.Order))
                            .Aggregations(aggrFunc)
                            .Highlight(h => Lib.Searching.Tools.GetHighlight<VerejnaZakazka>(withHighlighting))
                            .TrackTotalHits(search.ExactNumOfResults || page * search.PageSize == 0 ? true : (bool?)null)
                    );
                    if (withHighlighting && res.Shards != null && res.Shards.Failed > 0) //if some error, do it again without highlighting
                    {
                        res = client
                            .Search<VerejnaZakazka>(s => s
                                .Size(search.PageSize)
                                .Source(so => so.Excludes(ex => ex.Field("dokumenty.plainText")))
                                .From(page * search.PageSize)
                                .Query(q => GetSimpleQuery(search))
                                .Sort(ss => GetSort(search.Order))
                                .Aggregations(aggrFunc)
                                .Highlight(h => Lib.Searching.Tools.GetHighlight<VerejnaZakazka>(false))
                                .TrackTotalHits(search.ExactNumOfResults || page * search.PageSize == 0 ? true : (bool?)null)
                        );
                    }
                }
                catch (Exception e)
                {
                    Audit.Add(Audit.Operations.Search, "", "", "VerejnaZakazka", "error", search.Q, null);
                    if (res != null && res.ServerError != null)
                        Lib.ES.Manager.LogQueryError<VerejnaZakazka>(res, "Exception, Orig query:"
                            + search.OrigQuery + "   query:"
                            + search.Q
                            + "\n\n res:" + search.ElasticResults.ToString()
                            , ex: e);
                    else
                        HlidacStatu.Util.Consts.Logger.Error("", e);
                    throw;
                }
                sw.Stop();

                Audit.Add(Audit.Operations.Search, "", "", "VerejnaZakazka", res.IsValid ? "valid" : "invalid", search.Q, null);

                if (res.IsValid == false && logError)
                    Lib.ES.Manager.LogQueryError<VerejnaZakazka>(res, "Exception, Orig query:"
                        + search.OrigQuery + "   query:"
                        + search.Q
                        + "\n\n res:" + search.ElasticResults?.ToString()
                        );

                search.Total = res?.Total ?? 0;
                search.IsValid = res?.IsValid ?? false;
                search.ElasticResults = res;
                search.ElapsedTime = sw.Elapsed;
                return search;
            }

            public static Nest.ISearchResponse<VerejnaZakazka> RawSearch(string jsonQuery, int page, int pageSize, VerejnaZakazkaSearchData.VZOrderResult order = VerejnaZakazkaSearchData.VZOrderResult.Relevance,
                AggregationContainerDescriptor<VerejnaZakazka> anyAggregation = null
                )
            {
                return RawSearch(GetRawQuery(jsonQuery), page, pageSize, order, anyAggregation);
            }

            public static Nest.ISearchResponse<VerejnaZakazka> RawSearch(Nest.QueryContainer query, int page, int pageSize, VerejnaZakazkaSearchData.VZOrderResult order = VerejnaZakazkaSearchData.VZOrderResult.Relevance,
                AggregationContainerDescriptor<VerejnaZakazka> anyAggregation = null
                )
            {
                page -= 1;
                if (page < 0)
                    page = 0;

                AggregationContainerDescriptor<VerejnaZakazka> baseAggrDesc = null;
                baseAggrDesc = anyAggregation == null ?
                            null //new AggregationContainerDescriptor<VerejnaZakazka>().Sum("sumKc", m => m.Field(f => f.Castka))
                        : anyAggregation;

                Func<AggregationContainerDescriptor<VerejnaZakazka>, AggregationContainerDescriptor<VerejnaZakazka>> aggrFunc
                    = (aggr) => { return baseAggrDesc; };

                var client = Lib.ES.Manager.GetESClient();
                Indices indexes = client.ConnectionSettings.DefaultIndex;

                var res = client
                        .Search<VerejnaZakazka>(s => s
                            .Index(indexes)
                            .Size(pageSize)
                            .From(page * pageSize)
                            .Query(q => query)
                            //.Source(m => m.Excludes(e => e.Field(o => o.Prilohy)))
                            .Sort(ss => GetSort(order))
                            .Aggregations(aggrFunc)
                            .TrackTotalHits(page * pageSize == 0 ? true : (bool?)null)
                        );
                if (res.IsValid == false)
                    Lib.ES.Manager.LogQueryError<VerejnaZakazka>(res);

                //Audit.Add(Audit.Operations.Search, "", "", "VerejnaZakazka", res.IsValid ? "valid" : "invalid", query., null);

                return res;
            }

            public static VerejnaZakazkaSearchData _Search(
            VerejnaZakazkaSearchData search,
            AggregationContainerDescriptor<VerejnaZakazka> anyAggregation, ElasticClient client)
            {
                if (string.IsNullOrEmpty(search.Q) && (search.CPV == null || search.CPV.Length == 0))
                    return null;

                if (client == null)
                    client = ES.Manager.GetESClient_VZ();

                AggregationContainerDescriptor<VerejnaZakazka> baseAggrDesc = null;
                baseAggrDesc = anyAggregation == null ?
                            null //new AggregationContainerDescriptor<VerejnaZakazka>().Sum("sumKc", m => m.Field(f => f.Castka))
                        : anyAggregation;

                Func<AggregationContainerDescriptor<VerejnaZakazka>, AggregationContainerDescriptor<VerejnaZakazka>> aggrFunc
                    = (aggr) => { return baseAggrDesc; };

                string queryString = search.Q;
                Nest.ISearchResponse<VerejnaZakazka> res = null;
                if (search.CPV != null && search.CPV.Length > 0)
                {
                    string cpvs = search.CPV.Select(c => c + "*").Aggregate((f, s) => f + " OR " + s);
                    if (!string.IsNullOrEmpty(queryString))
                        queryString = queryString + " AND (cPV:(" + cpvs + "))";
                    else
                        queryString = "cPV:(" + cpvs + ")";
                }

                int page = search.Page - 1;
                if (page < 0)
                    page = 0;
                try
                {
                    res = client
                        .Search<VerejnaZakazka>(a => a
                            .Size(search.PageSize)
                            .From(search.PageSize * page)
                            .Aggregations(aggrFunc)
                            .Query(qq => qq.QueryString(qs => qs
                                    .Query(queryString)
                                    .DefaultOperator(Nest.Operator.And)
                                    )
                                )
                            .Sort(ss => GetSort(search.Order))
                            .Aggregations(aggrFunc)
                            .TrackTotalHits(page * search.PageSize == 0 ? true : (bool?)null)
                            );
                }
                catch (Exception e)
                {
                    Audit.Add(Audit.Operations.Search, "", "", "VerejnaZakazka", "error", search.Q, null);
                    if (res != null && res.ServerError != null)
                    {
                        ES.Manager.LogQueryError<VerejnaZakazka>(res, "Exception, Orig query:"
                            + search.OrigQuery + "   query:"
                            + search.Q
                            + "\n\n res:" + search.ElasticResults.ToString()
                            , ex: e);
                    }
                    else
                    {
                        HlidacStatu.Util.Consts.Logger.Error("", e);
                    }
                    throw;
                }
                Audit.Add(Audit.Operations.Search, "", "", "VerejnaZakazka", res.IsValid ? "valid" : "invalid", search.Q, null);

                search.IsValid = res.IsValid;
                search.ElasticResults = res;
                search.Total = res.Total;

                return search;
            }

            public static SortDescriptor<Data.VZ.VerejnaZakazka> GetSort(string sorder)
            {
                VerejnaZakazkaSearchData.VZOrderResult order = VerejnaZakazkaSearchData.VZOrderResult.Relevance;
                Enum.TryParse<VerejnaZakazkaSearchData.VZOrderResult>(sorder, out order);
                return GetSort(order);
            }

            public static SortDescriptor<Data.VZ.VerejnaZakazka> GetSort(VerejnaZakazkaSearchData.VZOrderResult order)
            {
                SortDescriptor<Data.VZ.VerejnaZakazka> s = new SortDescriptor<Data.VZ.VerejnaZakazka>().Field(f => f.Field("_score").Descending());
                switch (order)
                {
                    case VerejnaZakazkaSearchData.VZOrderResult.DateAddedDesc:
                        s = new SortDescriptor<Data.VZ.VerejnaZakazka>().Field(m => m.Field(f => f.DatumUverejneni).Descending());
                        break;

                    case VerejnaZakazkaSearchData.VZOrderResult.DateAddedAsc:
                        s = new SortDescriptor<Data.VZ.VerejnaZakazka>().Field(m => m.Field(f => f.DatumUverejneni).Ascending());
                        break;

                    case VerejnaZakazkaSearchData.VZOrderResult.DateSignedDesc:
                        s = new SortDescriptor<Data.VZ.VerejnaZakazka>().Field(m => m.Field(f => f.DatumUzavreniSmlouvy).Descending());
                        break;

                    case VerejnaZakazkaSearchData.VZOrderResult.DateSignedAsc:
                        s = new SortDescriptor<Data.VZ.VerejnaZakazka>().Field(m => m.Field(f => f.DatumUzavreniSmlouvy).Ascending());
                        break;

                    case VerejnaZakazkaSearchData.VZOrderResult.PriceAsc:
                        s = new SortDescriptor<Data.VZ.VerejnaZakazka>().Field(m => m.Field(f => f.KonecnaHodnotaBezDPH).Ascending());
                        break;

                    case VerejnaZakazkaSearchData.VZOrderResult.PriceDesc:
                        s = new SortDescriptor<Data.VZ.VerejnaZakazka>().Field(m => m.Field(f => f.KonecnaHodnotaBezDPH).Descending());
                        break;

                    case VerejnaZakazkaSearchData.VZOrderResult.FastestForScroll:
                        s = new SortDescriptor<Data.VZ.VerejnaZakazka>().Field(f => f.Field("_doc"));
                        break;

                    case VerejnaZakazkaSearchData.VZOrderResult.CustomerAsc:
                        s = new SortDescriptor<Data.VZ.VerejnaZakazka>().Field(f => f.Field(ff => ff.Zadavatel.Jmeno).Ascending());
                        break;

                    case VerejnaZakazkaSearchData.VZOrderResult.ContractorAsc:
                        s = new SortDescriptor<Data.VZ.VerejnaZakazka>().Field(f => f.Field("dodavatele.jmeno").Ascending());
                        break;

                    case VerejnaZakazkaSearchData.VZOrderResult.LastUpdate:
                        s = new SortDescriptor<Data.VZ.VerejnaZakazka>().Field(f => f.Field("lastUpdated").Descending());
                        break;

                    case VerejnaZakazkaSearchData.VZOrderResult.PosledniZmena:
                        s = new SortDescriptor<Data.VZ.VerejnaZakazka>().Field(f => f.Field("posledniZmena").Descending());
                        break;

                    case VerejnaZakazkaSearchData.VZOrderResult.Relevance:
                    default:
                        break;
                }

                return s;
            }

            public static MemoryCacheManager<VerejnaZakazkaSearchData, string>
                cachedSearches = new MemoryCacheManager<VerejnaZakazkaSearchData, string>("VZsearch", cachedFuncSimpleSearch, TimeSpan.FromHours(24));

            public static VerejnaZakazkaSearchData CachedSimpleSearch(TimeSpan expiration, VerejnaZakazkaSearchData search,
                bool logError = true, bool fixQuery = true, ElasticClient client = null, bool exactNumOfResults = false)
            {
                FullSearchQuery q = new FullSearchQuery()
                {
                    search = search,
                    logError = logError,
                    fixQuery = fixQuery,
                    client = client
                };
                return cachedSearches.Get(Newtonsoft.Json.JsonConvert.SerializeObject(q), expiration);
            }

            private static VerejnaZakazkaSearchData cachedFuncSimpleSearch(string jsonFullSearchQuery)
            {
                var query = Newtonsoft.Json.JsonConvert.DeserializeObject<FullSearchQuery>(jsonFullSearchQuery);
                return SimpleSearch(query.search, query.anyAggregation, query.logError, query.fixQuery, query.client);
            }

            private class FullSearchQuery
            {
                public VerejnaZakazkaSearchData search;
                public AggregationContainerDescriptor<VerejnaZakazka> anyAggregation = null;
                public bool logError = true;
                public bool fixQuery = true;
                public ElasticClient client = null;
            }

            public static IEnumerable<VerejnaZakazka> GetVZForHolding(string holdingIco)
            {
                string query = Tools.FixInvalidQuery($"holding:{holdingIco}", queryShorcuts, queryOperators);
                var qc = SimpleQueryCreator.GetSimpleQuery<VerejnaZakazka>(query, Rules);

                return YieldAll(qc);
            }

            private static IEnumerable<VerejnaZakazka> YieldAll(QueryContainer query,
            string scrollTimeout = "2m",
            int scrollSize = 300)
            {
                var client = ES.Manager.GetESClient_VZ();
                ISearchResponse<VerejnaZakazka> initialResponse = null;
                if (query is null)
                {
                    initialResponse = client.Search<VerejnaZakazka>(scr => scr
                        .From(0)
                        .Take(scrollSize)
                        .MatchAll()
                        .Scroll(scrollTimeout));
                }
                else
                {
                    initialResponse = client.Search<VerejnaZakazka>(scr => scr
                        .From(0)
                        .Take(scrollSize)
                        .Query(q => query)
                        .Scroll(scrollTimeout));
                }

                if (!initialResponse.IsValid || string.IsNullOrEmpty(initialResponse.ScrollId))
                    throw new Exception(initialResponse.ServerError.Error.Reason);

                if (initialResponse.Documents.Any())
                    foreach (var vz in initialResponse.Documents)
                    {
                        yield return vz;
                    }

                string scrollid = initialResponse.ScrollId;
                bool isScrollSetHasData = true;
                while (isScrollSetHasData)
                {
                    ISearchResponse<VerejnaZakazka> loopingResponse = client.Scroll<VerejnaZakazka>(scrollTimeout, scrollid);
                    if (loopingResponse.IsValid)
                    {
                        foreach (var vz in loopingResponse.Documents)
                        {
                            yield return vz;
                        }
                        scrollid = loopingResponse.ScrollId;
                    }
                    isScrollSetHasData = loopingResponse.Documents.Any();
                }

                client.ClearScroll(new ClearScrollRequest(scrollid));
            }
        }
    }
}