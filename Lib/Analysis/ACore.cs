using HlidacStatu.Util.Cache;
using Nest;
using System;
using System.Collections.Generic;

namespace HlidacStatu.Lib.Analysis
{

    public static class ACore
    {
        public static readonly TimeSpan ACoreExpiration = TimeSpan.FromHours(6);

        public static BasicDataPerYear GetBasicStatisticForICO(string ico)
        {

            if (Util.DataValidators.CheckCZICO(ico))
            {
                return basicStatForQuery.Get("ico:" + ico);

            }
            else
            {
                var s = new Dictionary<int, BasicData>();
                s.Add(DataPerYear.AllYearsSummaryKey, BasicData.Empty());
                return new BasicDataPerYear(s);
            }

        }


        public static RatingDataPerYear GetRatingForSimpleQuery(string query)
        {
            return basicRatingForQuery.Get(query);
        }


        private static CouchbaseCacheManager<RatingDataPerYear, string> basicRatingForQuery
            = CouchbaseCacheManager<RatingDataPerYear, string>.GetSafeInstance("BasicRatingForQuery",
                q => getRatingForSimpleQuery(q),
                ACoreExpiration,
                System.Configuration.ConfigurationManager.AppSettings["CouchbaseServers"].Split(','),
                System.Configuration.ConfigurationManager.AppSettings["CouchbaseBucket"],
                System.Configuration.ConfigurationManager.AppSettings["CouchbaseUsername"],
                System.Configuration.ConfigurationManager.AppSettings["CouchbasePassword"]
                );


        public static BasicDataPerYear GetBasicStatisticForSimpleQuery(string query)
        {
            return basicStatForQuery.Get(query);
        }


        private static CouchbaseCacheManager<BasicDataPerYear, string> basicStatForQuery
            = CouchbaseCacheManager<BasicDataPerYear, string>.GetSafeInstance("BasicStatisticForQuery",
                q => getBasicStatisticForSimpleQuery(q),
                ACoreExpiration,
                System.Configuration.ConfigurationManager.AppSettings["CouchbaseServers"].Split(','),
                System.Configuration.ConfigurationManager.AppSettings["CouchbaseBucket"],
                System.Configuration.ConfigurationManager.AppSettings["CouchbaseUsername"],
                System.Configuration.ConfigurationManager.AppSettings["CouchbasePassword"]);


        private static BasicDataPerYear getBasicStatisticForSimpleQuery(string query)
        {
            Dictionary<int, BasicData> result = new Dictionary<int, BasicData>();

            if (string.IsNullOrEmpty(query))
                return BasicDataPerYear.Empty();


            AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva> aggs = new AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>()
                .DateHistogram("x-agg", h => h
                    .Field(f => f.datumUzavreni)
                    .CalendarInterval(Nest.DateInterval.Year)
                    .Aggregations(agg => agg
                        .Sum("sumincome", s => s
                            .Field(ff => ff.CalculatedPriceWithVATinCZK)
                        )
                    )
                );



            var client = HlidacStatu.Lib.ES.Manager.GetESClient();
            var res = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch(query, 0, 0, 
                HlidacStatu.Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, aggs, true, exactNumOfResults: true);
            if (res.IsValid == false)
            {
                return BasicDataPerYear.Empty();
            }
            foreach (Nest.DateHistogramBucket val in ((BucketAggregate)res.ElasticResults.Aggregations["x-agg"]).Items)
            {
                var bdata = new BasicData()
                {
                    Pocet = val.DocCount ?? 0,
                    CelkemCena = (decimal)(((Nest.DateHistogramBucket)val).Sum("sumincome").Value ?? 0)
                };
                result.Add(val.Date.Year, bdata);
            }
            return new BasicDataPerYear(result);
        }

        public static RatingDataPerYear GetRatingForICO(string ico)
        {
            return ratingForIco.Get(ico);
        }


        private static CouchbaseCacheManager<RatingDataPerYear, string> ratingForIco
    = CouchbaseCacheManager<RatingDataPerYear, string>.GetSafeInstance("RatingForIco",
        ico => getRatingForICO(ico),
        ACoreExpiration,
        System.Configuration.ConfigurationManager.AppSettings["CouchbaseServers"].Split(','),
                System.Configuration.ConfigurationManager.AppSettings["CouchbaseBucket"],
                System.Configuration.ConfigurationManager.AppSettings["CouchbaseUsername"],
                System.Configuration.ConfigurationManager.AppSettings["CouchbasePassword"]);

        private static RatingDataPerYear getRatingForICO(string ico)
        {
            if (!Util.DataValidators.CheckCZICO(ico))
            {
                var s = new Dictionary<int, RatingData>();
                s.Add(DataPerYear.AllYearsSummaryKey, RatingData.Empty());
                return new RatingDataPerYear(s, GetBasicStatisticForICO(ico));
            }
            return getRatingForSimpleQuery("ico:" + ico);
        }
        private static RatingDataPerYear getRatingForSimpleQuery(string query)
        {
            Dictionary<int, RatingData> result = new Dictionary<int, RatingData>();

            AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva> aggY =
                new AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>()
                    .DateHistogram("x-agg", h => h
                        .Field(f => f.datumUzavreni)
                        .CalendarInterval(Nest.DateInterval.Year)
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
            //init result
            for (int year = BasicDataPerYear.UsualFirstYear; year <= DateTime.Now.Year; year++)
            {
                result.Add(year, RatingData.Empty());
            }

            var bezCenyStat = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch("issues.issueTypeId:100 " + query, 1, 0, 
                HlidacStatu.Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, aggY, exactNumOfResults:true);
            if (bezCenyStat.IsValid)
                foreach (Nest.DateHistogramBucket val in ((BucketAggregate)bezCenyStat.ElasticResults.Aggregations["x-agg"]).Items)
                {
                    if (result.ContainsKey(val.Date.Year))
                    {
                        result[val.Date.Year].NumBezCeny = val.DocCount ?? 0;
                    }
                }

            var bezSmluvniStr = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch("(issues.issueTypeId:18 OR issues.issueTypeId:12) AND " + query, 1, 0, 
                HlidacStatu.Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, aggYSum, exactNumOfResults: true);
            if (bezSmluvniStr.IsValid)

                foreach (Nest.DateHistogramBucket val in ((BucketAggregate)bezSmluvniStr.ElasticResults.Aggregations["x-agg"]).Items)
                {
                    if (result.ContainsKey(val.Date.Year))
                    {
                        result[val.Date.Year].NumBezSmluvniStrany = val.DocCount ?? 0;
                        result[val.Date.Year].SumKcBezSmluvniStrany = (decimal)(((Nest.DateHistogramBucket)val).Sum("sumincome").Value ?? 0);
                    }
                }

            QueryContainer qc = new QueryContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>()
                .Bool(b => b
                    .Must(
                        m => m.Term(t => t.Field(f => f.SVazbouNaPolitikyNedavne).Value(true))
                        ,
                        m1 => Lib.Data.Smlouva.Search.GetSimpleQuery(query)
                    )
                );
            var vazbyNaSoukr = HlidacStatu.Lib.Data.Smlouva.Search.SearchRaw(qc, 1, 0, HlidacStatu.Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, aggYSum);
            if (vazbyNaSoukr.IsValid)
                foreach (Nest.DateHistogramBucket val in ((BucketAggregate)vazbyNaSoukr.ElasticResults.Aggregations["x-agg"]).Items)
                {
                    if (result.ContainsKey(val.Date.Year))
                    {
                        result[val.Date.Year].NumSPolitiky = val.DocCount ?? 0;
                        result[val.Date.Year].SumKcSPolitiky = (decimal)(((Nest.DateHistogramBucket)val).Sum("sumincome").Value ?? 0);
                    }
                }

            //calculate percents
            return new RatingDataPerYear(result, GetBasicStatisticForSimpleQuery(query));
        }


        public static BasicDataPerYear GetBezCenyStatForSimpleQuery(string query)
        {
            Dictionary<int, BasicData> result = new Dictionary<int, BasicData>();

            AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva> aggY =
                new AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>()
                    .DateHistogram("x-agg", h => h
                        .Field(f => f.datumUzavreni)
                        .CalendarInterval(Nest.DateInterval.Year)
                    );

            //init result
            for (int year = BasicDataPerYear.UsualFirstYear; year <= DateTime.Now.Year; year++)
            {
                result.Add(year, BasicData.Empty());
            }

            var bezCenyStat = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch("issues.issueTypeId:100 " + query, 1, 0, 
                HlidacStatu.Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, aggY, exactNumOfResults: true);
            foreach (Nest.DateHistogramBucket val in ((BucketAggregate)bezCenyStat.ElasticResults.Aggregations["x-agg"]).Items)
            {
                if (result.ContainsKey(val.Date.Year))
                {
                    result[val.Date.Year].Pocet = val.DocCount ?? 0;
                }
                else
                    result.Add(val.Date.Year, new BasicData() { Pocet = val.DocCount ?? 0 });
            }

            return new BasicDataPerYear(result);
        }



    }
}
