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

                string[] specifiedIcosInQuery = Util.ParseTools.GetRegexGroupValues(modifQ, @"(ico\w{0,11}\: \s? (?<ic>\d{3,8}))", "ic");
                if (specifiedIcosInQuery != null && specifiedIcosInQuery.Length > 0)
                {
                    List<string> resIcos = new List<string>();
                    foreach (var ic in specifiedIcosInQuery)
                    {
                        Firma f = Firmy.Get(ic);
                        if (f.Valid)
                        {
                            ///nalezene ICO
                            resIcos.Add(ic);
                        }

                    }
                    if (resIcos.Count() > 0)
                        return resIcos.ToArray();
                }



                //
                modifQ = System.Text.RegularExpressions.Regex.Replace(modifQ, "(ico:|icoprijemce:|icoplatce:|icododavatel:|icozadavatel:)", "ico:", regexQueryOption);

                modifQ = System.Text.RegularExpressions.Regex.Replace(modifQ, "(jmenoPrijemce:|jmenoPlatce:|jmenododavatel:|jmenozadavatel:)", "jmeno:", regexQueryOption);

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
                            .TrackTotalHits(limit == 0 ? true : (bool?)null)
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
                        Lib.ES.Manager.LogQueryError<FirmaInElastic>(res,query);
                }
                catch (Exception e)
                {

                    if (res != null && res.ServerError != null)
                        Lib.ES.Manager.LogQueryError<FirmaInElastic>(res,query);
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


            public static IEnumerable<Firma> FindAllInMemory(string query, int limit)
            {
                return FindAllIco(query, limit)
                    .Select(m => Firmy.Get(m));
            }

        }
    }
}
