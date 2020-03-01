using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HlidacStatu.Lib.Data
{
    public partial class Firma
    {
        public class Search
        {

            [ElasticsearchType(IdProperty = nameof(Ico))]

            public class FirmaInElastic
            {
                public string Jmeno { get; set; }

                public string JmenoBezKoncovky { get; set; }

                [Keyword]
                public string JmenoBezKoncovkyAscii { get; set; }

                [Keyword()]
                public string Ico { get; set; }

                private FirmaInElastic() { }

                public FirmaInElastic(Firma f)
                {
                    this.Ico = f.ICO;
                    this.Jmeno = f.Jmeno;
                    this.JmenoBezKoncovky = f.JmenoBezKoncovky();
                    this.JmenoBezKoncovkyAscii = Devmasters.Core.TextUtil.RemoveDiacritics(this.JmenoBezKoncovky);
                }
                public FirmaInElastic(string ico, string jmeno)
                {
                    this.Ico = ico;
                    this.Jmeno = jmeno;
                    this.JmenoBezKoncovky = Firma.JmenoBezKoncovky(jmeno); ;
                    this.JmenoBezKoncovkyAscii = Devmasters.Core.TextUtil.RemoveDiacritics(this.JmenoBezKoncovky);
                }

                public void Save()
                {
                    ElasticClient c = ES.Manager.GetESClient_Firmy();
                    var res = c.Index<FirmaInElastic>(this, m => m.Id(this.Ico));

                }
            }


            static RegexOptions regexQueryOption = RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline;
            static string[] queryOperators = new string[] {
            "AND","OR"
        };

            public static IEnumerable<string> FindAllIco(string query, int limit)
            {

                string modifQ = query;

                //
                modifQ = System.Text.RegularExpressions.Regex.Replace(modifQ, "(ico:|icoprijemce:|icoplatce:|icododavatel:|icozadavatel:)", "ico:", regexQueryOption);

                modifQ = System.Text.RegularExpressions.Regex.Replace(modifQ, "(jmenoPrijemce:|jmenoPlatce:|jmenododavatel:|icozadavatel:)", "jmeno:", regexQueryOption);

                //modifQ = System.Text.RegularExpressions.Regex.Replace(modifQ, "\\s(AND|OR)\\s", " ", regexQueryOption);

                List<string> found = new List<string>();


                var qc = new QueryContainerDescriptor<Lib.Data.Smlouva>()
                    .QueryString(qs => qs
                        .Query(modifQ)
                        .DefaultOperator(Operator.And)
                    );

                ISearchResponse<FirmaInElastic> res = null;
                try
                {
                     res = ES.Manager.GetESClient_Firmy()
                        .Search<FirmaInElastic>(s => s
                            .Size(limit)
                            .From(0)
                            .Query(q => qc)
                        );

                    if (res.IsValid)
                    {
                        foreach (var i in res.Hits)
                        {
                            found.Add(i.Source.Ico);
                        }
                        return found;
                    }
                    else
                        Lib.ES.Manager.LogQueryError<FirmaInElastic>(res);
                }
                catch (Exception e)
                {

                    if (res != null && res.ServerError != null)
                        Lib.ES.Manager.LogQueryError<FirmaInElastic>(res);
                    else
                        HlidacStatu.Util.Consts.Logger.Error("", e);
                    throw;
                }
                return found;
            }
            public static IEnumerable<Firma> FindAll(string query, int limit)
            {
                return FindAllIco(query, limit)
                    .Select(m => Firmy.Get(m));
            }


            public static IEnumerable<string> FindAllIcoInMemory(string query, int limit)
            {
                string findAllIcoTimes = $"FindAllIco {query}\n";
                if (string.IsNullOrEmpty(query))
                    return new string[] { };

                var items = new List<Tuple<string, decimal>>();
                Devmasters.Core.StopWatchEx sw = new Devmasters.Core.StopWatchEx();
                sw.Start();

                var resExact = StaticData.FirmyNazvy.Get()
                                .Where(m => m.Value.Jmeno == query) // Data.External.FirmyDB.AllFromName(query)
                                .Select(m => new Tuple<string, decimal>(m.Key, 1m));

                if (resExact.Count() > 0)
                    items.AddRange(resExact);
                sw.Stop();
                findAllIcoTimes += $"step1:{sw.ElapsedMilliseconds}\n";

                string aQuery = Devmasters.Core.TextUtil.RemoveDiacritics(query).ToLower();
                if (items.Count < limit)
                {
                    sw.Restart();
                    //add more
                    if (StaticData.FirmyNazvyOnlyAscii.ContainsKey(aQuery))
                    {
                        var res = StaticData.FirmyNazvyOnlyAscii[aQuery]
                                    .Where(ico => !string.IsNullOrEmpty(ico)).Select(ico => new Tuple<string, decimal>(ico, 0.9m))
                                    .GroupBy(g => g.Item1, v => v.Item2, (g, v) => new Tuple<string, decimal>(g, v.Max()));
                        items.AddRange(res);
                    }
                    sw.Stop();
                    findAllIcoTimes += $"step2:{sw.ElapsedMilliseconds}\n";


                }
                if (items.Count < limit)
                {
                    sw.Restart();
                    //add more
                    var res = StaticData.FirmyNazvyOnlyAscii
                                .Where(m => m.Key.StartsWith(aQuery, StringComparison.Ordinal))
                                .Take(limit - items.Count)
                                .SelectMany(m => m.Value.Where(ico => !string.IsNullOrEmpty(ico)).Select(ico => new Tuple<string, decimal>(ico, 0.5m)))
                                .GroupBy(g => g.Item1, v => v.Item2, (g, v) => new Tuple<string, decimal>(g, v.Max()));
                    items.AddRange(res);
                    sw.Stop();
                    findAllIcoTimes += $"step3:{sw.ElapsedMilliseconds}\n";

                }
                if (items.Count < limit && aQuery.Length >= 5)
                {

                    sw.Restart();
                    //add more
                    var res = StaticData.FirmyNazvyOnlyAscii
                                .Where(m => m.Key.Contains(aQuery))
                                .OrderBy(m => Validators.LevenshteinDistanceCompute(m.Key, aQuery))
                                .Take(limit - items.Count)
                                .Where(m => Validators.LevenshteinDistanceCompute(m.Key, aQuery) < 10)
                                .SelectMany(m => m.Value.Where(ico => !string.IsNullOrEmpty(ico)).Select(ico => new Tuple<string, decimal>(ico, 0.5m)))
                                .GroupBy(g => g.Item1, v => v.Item2, (g, v) => new Tuple<string, decimal>(g, v.Max()));
                    items.AddRange(res);
                    sw.Stop();
                    findAllIcoTimes += $"step4:{sw.ElapsedMilliseconds}\n";

                }

                if (Devmasters.Core.Util.Config.GetConfigValue("LogSearchTimes") == "true")
                {
                    HlidacStatu.Util.Consts.Logger.Info(findAllIcoTimes);
                }
                return items
                    .Take(limit)
                    .Select(m => m.Item1);
            }
            public static IEnumerable<Firma> FindAllInMemory(string query, int limit)
            {
                return FindAllIco(query, limit)
                    .Select(m => Firmy.Get(m));
            }

        }
    }
}
