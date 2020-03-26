using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HlidacStatu.Lib.Analysis;
using HlidacStatu.Lib.Searching;
using Nest;

namespace HlidacStatu.Analysis.KorupcniRiziko
{
    public class AdvancedQuery
    {

        public static Dictionary<int, BasicData> PerYear(string query, int[] yearsInterval = null)
        {
            yearsInterval = yearsInterval ?? Analysis.Calendar.CalculationYears;

            AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva> aggY =
                new AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>()
                    .DateHistogram("x-agg", h => h
                        .CalendarInterval(DateInterval.Year)
                        .Field(f => f.datumUzavreni)
                    );
            AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva> aggYSum =
                new AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>()
                    .DateHistogram("x-agg", h => h
                        .Field(f => f.datumUzavreni)
                        .CalendarInterval(Nest.DateInterval.Year)
                        .Aggregations(agg => agg
                            .Sum("sumincome", s => s
                                .Field(ff => ff.CalculatedPriceWithVATinCZK)
                            )
                        )
                    );

            var res = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch(query, 1, 0,
                HlidacStatu.Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, aggYSum, exactNumOfResults: true);


            Dictionary<int, BasicData> result = new Dictionary<int, BasicData>();
            foreach (int year in yearsInterval)
            {
                result.Add(year, BasicData.Empty());
            }

            foreach (Nest.DateHistogramBucket val in ((BucketAggregate)res.ElasticResults.Aggregations["x-agg"]).Items)
            {
                if (result.ContainsKey(val.Date.Year))
                {
                    result[val.Date.Year].Pocet = val.DocCount ?? 0;
                    result[val.Date.Year].CelkemCena = (decimal)(((Nest.DateHistogramBucket)val).Sum("sumincome").Value ?? 0);
                }
            }

            return result;
        }
    }
}
