using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HlidacStatu.Lib.Analysis;
using HlidacStatu.Lib.Searching;

using Nest;

namespace HlidacStatu.Lib.ES
{
    public class QueryGrouped
    {
        public static Dictionary<int, Dictionary<int, BasicData>> OblastiPerYear(string query, int[] interestedInYearsOnly)
        {

            AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva> aggYSum =
                new AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>()
                    .DateHistogram("x-agg", h => h
                        .Field(f => f.datumUzavreni)
                        .CalendarInterval(Nest.DateInterval.Year)
                        .Aggregations(aggObor => aggObor
                            .Terms("x-obor", oborT => oborT
                                 .Field("classification.class1.typeValue")
                                 .Size(150)
                                 .Aggregations(agg => agg
                                     .Sum("sumincome", s => s
                                         .Field(ff => ff.CalculatedPriceWithVATinCZK)
                                     )
                                 )
                            )
                        )
                    );

            var res = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch(query, 1, 0,
                HlidacStatu.Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, aggYSum, exactNumOfResults: true);


            Dictionary<int, Dictionary<int, BasicData>> result = new Dictionary<int, Dictionary<int, BasicData>>();
            if (interestedInYearsOnly != null)
            {
                foreach (int year in interestedInYearsOnly)
                {
                    result.Add(year, new Dictionary<int, BasicData>());
                }

                foreach (Nest.DateHistogramBucket val in ((BucketAggregate)res.ElasticResults.Aggregations["x-agg"]).Items)
                {
                    if (result.ContainsKey(val.Date.Year))
                    {
                        Nest.BucketAggregate vals = (Nest.BucketAggregate)val.Values.FirstOrDefault();
                        var oblasti = vals.Items.Select(m =>
                            new
                            {
                                oblast = Convert.ToInt32(((Nest.KeyedBucket<object>)m).Key),
                                data = new BasicData()
                                {
                                    CelkemCena = (decimal)((Nest.ValueAggregate)((Nest.KeyedBucket<object>)m).Values.FirstOrDefault()).Value,
                                    Pocet = ((Nest.KeyedBucket<object>)m).DocCount ?? 0
                                }
                            }
                        ).ToArray();

                        result[val.Date.Year] = oblasti.ToDictionary(k=>k.oblast, v=>v.data);
                    }
                }
            }
            else
            {
                foreach (Nest.DateHistogramBucket val in ((BucketAggregate)res.ElasticResults.Aggregations["x-agg"]).Items)
                {
                    if (result.ContainsKey(val.Date.Year))
                    {
                        Nest.BucketAggregate vals = (Nest.BucketAggregate)val.Values.FirstOrDefault();
                        var oblasti = vals.Items.Select(m =>
                            new
                            {
                                oblast = Convert.ToInt32(((Nest.KeyedBucket<object>)m).Key),
                                data = new BasicData()
                                {
                                    CelkemCena = (decimal)((Nest.ValueAggregate)((Nest.KeyedBucket<object>)m).Values.FirstOrDefault()).Value,
                                    Pocet = ((Nest.KeyedBucket<object>)m).DocCount ?? 0
                                }
                            }
                        ).ToArray();
                        result.Add(val.Date.Year, new Dictionary<int, BasicData>());
                        result[val.Date.Year] = oblasti.ToDictionary(k => k.oblast, v => v.data);
                    }
                }

        }

            return result;
        }

        public static Dictionary<int, BasicData> SmlouvyPerYear(string query, int[] interestedInYearsOnly)
        {


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
            if (interestedInYearsOnly != null)
            {
                foreach (int year in interestedInYearsOnly)
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
            }
            else
            {
                foreach (Nest.DateHistogramBucket val in ((BucketAggregate)res.ElasticResults.Aggregations["x-agg"]).Items)
                {
                    result.Add(val.Date.Year, BasicData.Empty());
                    result[val.Date.Year].Pocet = val.DocCount ?? 0;
                    result[val.Date.Year].CelkemCena = (decimal)(((Nest.DateHistogramBucket)val).Sum("sumincome").Value ?? 0);
                }

            }

            return result;
        }

        public static Dictionary<int, (List<(string ico, BasicData stat)> topPodlePoctu, List<(string ico, BasicData stat)> topPodleKc)>
            TopOdberatelePerYear(
            string query,
            int[] interestedInYearsOnly,
            int maxList = 50
            )
        {
            return _topSmluvniStranyPerYear("prijemce.ico", query, interestedInYearsOnly, maxList);
        }


        public static Dictionary<int, (List<(string ico, BasicData stat)> topPodlePoctu, List<(string ico, BasicData stat)> topPodleKc)>
            TopDodavatelePerYear(
            string query,
            int[] interestedInYearsOnly,
            int maxList = 50
            )
        {
            return _topSmluvniStranyPerYear("platce.ico", query, interestedInYearsOnly, maxList);
        }

        private static Dictionary<int, (List<(string ico, BasicData stat)> topPodlePoctu, List<(string ico, BasicData stat)> topPodleKc)>
            _topSmluvniStranyPerYear(
                string property,
                string query,
                int[] interestedInYearsOnly,
                int maxList
                )
        {
            AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva> aggs = new AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>();
            aggs
                .Terms("perIco", m => m
                    .Field(property)
                    .Size(maxList)
                ).Terms("perPrice", m => m
                    .Order(o => o.Descending("sumincome"))
                    .Field(property)
                    .Size(maxList)
                    .Aggregations(agg => agg
                       .Sum("sumincome", s => s
                           .Field(ff => ff.CalculatedPriceWithVATinCZK)
                       )
                    )
                );

            Func<AggregationContainerDescriptor<Lib.Data.Smlouva>, AggregationContainerDescriptor<Lib.Data.Smlouva>> aggrFunc
                = (aggr) => { return aggs; };

            AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva> aggYSum =
                new AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>()
                    .DateHistogram("y-agg", h => h
                        .Field(f => f.datumUzavreni)
                        .CalendarInterval(Nest.DateInterval.Year)
                        .Aggregations(aggrFunc)
                    );


            var res = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch(query, 1, 0,
                HlidacStatu.Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, aggYSum, exactNumOfResults: true);


            Dictionary<int, (List<(string ico, BasicData stat)> topPodlePoctu, List<(string ico, BasicData stat)> topPodleKc)> result =
                new Dictionary<int, (List<(string ico, BasicData stat)> topPodlePoctu, List<(string ico, BasicData stat)> topPodleKc)>();

            if (interestedInYearsOnly != null)
                foreach (int year in interestedInYearsOnly)
                {
                    result.Add(year,
                        (new List<(string ico, BasicData stat)>(),
                        new List<(string ico, BasicData stat)>())
                        );
                }

            foreach (Nest.DateHistogramBucket val in ((BucketAggregate)res.ElasticResults.Aggregations["y-agg"]).Items)
            {
                if (interestedInYearsOnly == null)
                {
                    result.Add(val.Date.Year,
                        (new List<(string ico, BasicData stat)>(),
                        new List<(string ico, BasicData stat)>())
                        );
                }

                if (result.ContainsKey(val.Date.Year))
                {
                    result[val.Date.Year] =
                        (topPodlePoctu: ((BucketAggregate)val["perIco"]).Items
                            .Select(m => ((KeyedBucket<object>)m))
                            .Select(m => (m.Key.ToString(), new BasicData() { Pocet = m.DocCount ?? 0 }))
                            .ToList(),
                        topPodleKc: ((BucketAggregate)val["perPrice"]).Items
                            .Select(m => ((KeyedBucket<object>)m))
                            .Select(m => (m.Key.ToString(), new BasicData() { Pocet = m.DocCount ?? 0 }))
                            .ToList()
                           );

                }
            }

            return result;
        }


        //public static string ToElasticQuery(IEnumerable<DateTime> dates)
        //{
        //    if (dates == null)
        //        return string.Empty;
        //    if (dates.Count() == 0)
        //        return string.Empty;

        //    return "( " + string.Join(" OR ", dates) + " ) ";
        //}

    }
}
