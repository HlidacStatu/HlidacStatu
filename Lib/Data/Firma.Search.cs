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
            public const int MaxResultWindow = 10000;

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

            public static Lib.Data.Search.GeneralResult<Firma> SimpleSearch(string query, int page, int size)
            {
                List<Firma> found = new List<Firma>();

                string modifQ = Lib.Searching.SimpleQueryCreator
                    .GetSimpleQuery(query, new Searching.Rules.IRule[] { new Searching.Rules.Firmy_OVMKategorie() })
                    .FullQuery();
                
                string[] specifiedIcosInQuery = Util.ParseTools.GetRegexGroupValues(modifQ, @"(ico\w{0,11}\: \s? (?<ic>\d{3,8}))", "ic");
                if (specifiedIcosInQuery != null && specifiedIcosInQuery.Length > 0)
                {
                    foreach (var ic in specifiedIcosInQuery.Skip((page-1)*size).Take(size))
                    {
                        Firma f = Firmy.Get(ic);
                        if (f.Valid)
                        {
                            ///nalezene ICO
                            found.Add(f);
                        }

                    }
                    if (found.Count() > 0)
                        return new Data.Search.GeneralResult<Firma>(query, specifiedIcosInQuery.Count(), found, size,true) { Page = page };
                }



                //
                modifQ = System.Text.RegularExpressions.Regex.Replace(modifQ, "(ico:|icoprijemce:|icoplatce:|icododavatel:|icozadavatel:)", "ico:", regexQueryOption);

                modifQ = System.Text.RegularExpressions.Regex.Replace(modifQ, "(jmenoPrijemce:|jmenoPlatce:|jmenododavatel:|jmenozadavatel:)", "jmeno:", regexQueryOption);

                //modifQ = System.Text.RegularExpressions.Regex.Replace(modifQ, "\\s(AND|OR)\\s", " ", regexQueryOption);

                page = page - 1;
                if (page < 0)
                    page = 0;

                if (page * size >= MaxResultWindow) //elastic limit
                {
                    page = 0; size = 0; //return nothing
                }


                var qc = GetSimpleQuery(query);
                //var qc = new QueryContainerDescriptor<Lib.Data.Smlouva>()
                //    .QueryString(qs => qs
                //        .Query(modifQ)
                //        .DefaultOperator(Operator.And)
                //    );

                ISearchResponse<FirmaInElastic> res = null;
                try
                {
                    res = ES.Manager.GetESClient_Firmy()
                       .Search<FirmaInElastic>(s => s
                            .Size(size)
                            .From(page * size)
                            .TrackTotalHits(size == 0 ? true : (bool?)null)
                            .Query(q => qc)
                       );

                    if (res.IsValid)
                    {
                        foreach (var i in res.Hits)
                        {
                            found.Add(Firmy.Get(i.Source.Ico));
                        }
                        return new Data.Search.GeneralResult<Firma>(query, res.Total, found, size,true) { Page = page };
                    }
                    else
                    {
                        Lib.ES.Manager.LogQueryError<FirmaInElastic>(res, query);
                        return new Data.Search.GeneralResult<Firma>(query, 0, found, size,false) { Page = page };
                    }
                }
                catch (Exception e)
                {

                    if (res != null && res.ServerError != null)
                        Lib.ES.Manager.LogQueryError<FirmaInElastic>(res, query);
                    else
                        HlidacStatu.Util.Consts.Logger.Error("", e);
                    throw;
                }
                return new Data.Search.GeneralResult<Firma>(query, 0, found, size, false) { Page = page };
            }

            public static QueryContainer GetSimpleQuery(string query)
            {
                var qc = Lib.Searching.SimpleQueryCreator.GetSimpleQuery<FirmaInElastic>(query, new Searching.Rules.IRule[] { });
                return qc;
            }


            public static IEnumerable<Firma> FindAll(string query, int limit)
            {
                return SimpleSearch(query, 0,limit)
                    .Result;
            }


            public static IEnumerable<Firma> FindAllInMemory(string query, int limit)
            {
                return SimpleSearch(query,0, limit)
                    .Result;
            }

        }
    }
}
