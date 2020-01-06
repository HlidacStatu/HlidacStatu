using HlidacStatu.Lib;
using HlidacStatu.Lib.Render;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static HlidacStatu.Lib.RenderTools;

namespace HlidacStatu.Web.Framework.Report
{
    public static class GlobalStatistics
    {
        public static HlidacStatu.Lib.Render.ReportDataSource PocetSmluvPerZverejneni(string query, Nest.DateInterval interval)
        {
            DateTime minDate = new DateTime(2012, 1, 1);
            DateTime maxDate = DateTime.Now.Date.AddDays(1);
            string datumFormat = "MMM yyyy";
            switch (interval)
            {
                case DateInterval.Day:
                    datumFormat = "dd.MM.yy";
                    break;
                case DateInterval.Week:
                    datumFormat = "dd.MM.yy";
                    break;
                case DateInterval.Month:
                    datumFormat = "MMM yyyy";
                    break;
                case DateInterval.Quarter:
                    datumFormat = "MMM yyyy";
                    break;
                case DateInterval.Year:
                    datumFormat = "yyyy";
                    break;
                default:
                    break;
            }

            AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva> aggs = new AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>()
                .DateHistogram("x-agg", h => h
                    .Field(f => f.datumUzavreni)
                    .Interval(interval)
                );

            //var res = HlidacStatu.Lib.Data.Smlouva.Search.RawSearch(
            //    "{\"query_string\": { \"query\": \"-id:pre* AND datumUzavreni:{" + HlidacStatu.Util.RenderData.ToElasticDate(minDate) + " TO "+ HlidacStatu.Util.RenderData.ToElasticDate(maxDate) + "}\" } }"
            //        , 1, 0, anyAggregation: aggs);
            var res = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch("( " + query + " ) AND datumUzavreni:{" + HlidacStatu.Util.RenderData.ToElasticDate(minDate) + " TO " + HlidacStatu.Util.RenderData.ToElasticDate(maxDate) + "}", 1, 0, HlidacStatu.Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, anyAggregation: aggs, exactNumOfResults: true);

            ReportDataSource rds = new ReportDataSource(new ReportDataSource.Column[]
                {
                    new ReportDataSource.Column() { Name="Datum",
                        TextRender = (s) => { return ((DateTime)s).ToString(datumFormat); },
                        OrderValueRender = (s) => { return ((DateTime)s).Ticks.ToString(); }
                    },
                    new ReportDataSource.Column() { Name="Počet smluv"},
                }
            );

            foreach (Nest.DateHistogramBucket val in ((BucketAggregate)res.ElasticResults.Aggregations["x-agg"]).Items)
            {
                if (val.Date>= minDate && val.Date <= maxDate)
                    rds.AddRow(
                        val.Date,
                        val.DocCount
                        );
            }


            return rds;
        }

        public static ReportDataSource HodnotaSmluvPerZverejneni(string query, DateInterval interval)
        {
            DateTime minDate = new DateTime(2012, 1, 1);
            DateTime maxDate = DateTime.Now.Date.AddDays(1);

            string datumFormat = "MMM yyyy";
            switch (interval)
            {
                case DateInterval.Day:
                    datumFormat = "dd.MM.yy";
                    break;
                case DateInterval.Week:
                    datumFormat = "dd.MM.yy";
                    break;
                case DateInterval.Month:
                    datumFormat = "MMM yyyy";
                    break;
                case DateInterval.Quarter:
                    datumFormat = "MMM yyyy";
                    break;
                case DateInterval.Year:
                    datumFormat = "yyyy";
                    break;
                default:
                    break;
            }

            AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva> aggs = new AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>()
                .DateHistogram("x-agg", h => h
                    .Field(f => f.datumUzavreni)
                    .Interval(interval)
                    .Format("yyyy-MM-dd")
                    .Aggregations(agg => agg
                        .Sum("sumincome", s => s
                            .Field(ff => ff.CalculatedPriceWithVATinCZK)
                        )
                    )
                );
            ReportDataSource rdsPerIntervalSumPrice = new ReportDataSource(new ReportDataSource.Column[]
            {
                new ReportDataSource.Column() { Name="Měsíc",
                    TextRender = (s) => {
                                            DateTime dt = ((DateTime)s);
                                            return string.Format("Date.UTC({0}, {1}, {2})", dt.Year, dt.Month-1, dt.Day);
                                        }
                },
                new ReportDataSource.Column() { Name="Součet cen", HtmlRender = (s) => { return HlidacStatu.Lib.Data.Smlouva.NicePrice((double?)s, html:true, shortFormat:true); } },
            });

            var res = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch("( " + query + " ) AND datumUzavreni:{" + HlidacStatu.Util.RenderData.ToElasticDate(minDate) + " TO " + HlidacStatu.Util.RenderData.ToElasticDate(maxDate) + "}", 1, 0, HlidacStatu.Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, anyAggregation: aggs, exactNumOfResults: true);

            foreach (Nest.DateHistogramBucket val in
                    ((BucketAggregate)res.Result.Aggregations["x-agg"]).Items
            )
            {
                if (val.Date >= minDate && val.Date <= maxDate)
                    rdsPerIntervalSumPrice.AddRow(
                    new DateTime(val.Date.Ticks, DateTimeKind.Utc).ToLocalTime(),
                    ((Nest.DateHistogramBucket)val).Sum("sumincome").Value
                    );

            }


            return rdsPerIntervalSumPrice;
        }

        public static HlidacStatu.Lib.Render.ReportDataSource PocetSmluvPerZverejneni(Nest.DateInterval interval)
        {
            return PocetSmluvPerZverejneni("-id:pre* ", interval);
        }
        public static ReportDataSource HodnotaSmluvPerZverejneni(DateInterval interval)
        {
            return HodnotaSmluvPerZverejneni("-id:pre* ", interval);
        }


        public static ReportDataSource TopListPerCount(bool platce)
        {
            string field = "prijemce.ico";
            if (platce)
                field = "platce.ico";

            int size = 300;
            AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva> aggs = new AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>()
                .Terms("perIco", m => m
                    .Field(field)
                    .Size(size)
                );

            var res = HlidacStatu.Lib.Data.Smlouva.Search.RawSearch("{\"query_string\": { \"query\": \"-id:pre*\" } }", 1, 0, anyAggregation: aggs);

            ReportDataSource rdsPerIco = new ReportDataSource(new ReportDataSource.Column[]
            {
                new ReportDataSource.Column() { Name="IČO",
                    HtmlRender = (s) => {
                                            System.Tuple<string,string> data = (System.Tuple<string,string>)s;
                                            return string.Format("<a href='/subjekt/{0}'>{1}</a>", data.Item2, data.Item1.Replace("&","&amp;"));
                                        },
                    TextRender = (s) => { return ((System.Tuple<string,string>)s).Item1.ToString(); }
                },
                new ReportDataSource.Column() { Name="Počet smluv",
                HtmlRender = (s) => { return HlidacStatu.Util.RenderData.NiceNumber((long)s, html:true); },
                OrderValueRender = (s) => { return HlidacStatu.Util.RenderData.OrderValueFormat(((double?)s)??0); }

                },
            }
                );

            foreach (Nest.KeyedBucket<object> val in ((BucketAggregate)res.Aggregations["perIco"]).Items)
            {
                HlidacStatu.Lib.Data.Firma f = HlidacStatu.Lib.Data.Firmy.Get((string)val.Key);
                if (f != null &&  (!f.PatrimStatu() || platce))
                {
                    rdsPerIco.AddRow(
                        new Tuple<string, string>(HlidacStatu.Lib.Data.Firmy.GetJmeno((string)val.Key), (string)val.Key),
                        val.DocCount.ToString()
                        );
                }
            }

            return rdsPerIco;
        }

        public static ReportDataSource TopListPerSum(bool platce)
        {

            string field = "prijemce.ico";
            if (platce)
                field = "platce.ico";

            int size = 300;
            AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva> aggs = new AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>()
            .Terms("perPrice", m => m
                        .Order(o => o.Descending("sumincome"))
                        .Field(field)
                        .Size(size)
                        .Aggregations(agg => agg
                           .Sum("sumincome", s => s
                               .Field(ff => ff.CalculatedPriceWithVATinCZK)
                           )
                        )
                    );

            var res = HlidacStatu.Lib.Data.Smlouva.Search.RawSearch("{\"query_string\": { \"query\": \"-id:pre*\" } }", 1, 0, anyAggregation: aggs);

            ReportDataSource rdsPerPrice = new ReportDataSource(new ReportDataSource.Column[]
            {
                new ReportDataSource.Column() { Name="IČO",
                    HtmlRender = (s) => {
                                            System.Tuple<string,string> data = (System.Tuple<string,string>)s;
                                            return string.Format("<a href='/subjekt/{0}'>{1}</a>", data.Item2, data.Item1.Replace("&","&amp;"));
                                        },
                    TextRender = (s) => { return ((System.Tuple<string,string>)s).Item1.ToString(); }
                },
                new ReportDataSource.Column() { Name="Součet cen", 
                    HtmlRender = (s) => { return HlidacStatu.Lib.Data.Smlouva.NicePrice((double?)s, html:true, shortFormat:true); },
                    OrderValueRender = (s) => { return HlidacStatu.Util.RenderData.OrderValueFormat(((double?)s)??0); }
                },
            }
                ); ; ;
            foreach (Nest.KeyedBucket<object> val in ((BucketAggregate)res.Aggregations["perPrice"]).Items)
            {
                HlidacStatu.Lib.Data.Firma f = HlidacStatu.Lib.Data.Firmy.Get((string)val.Key);
                if (f != null && (!f.PatrimStatu() || platce))
                {
                    rdsPerPrice.AddRow(
                            new Tuple<string, string>(HlidacStatu.Lib.Data.Firmy.GetJmeno((string)val.Key), (string)val.Key),
                            val.Sum("sumincome").Value
                        );
                }
            }


            return rdsPerPrice;

        }


        public static ReportDataSource SmlouvyPodleCeny()
        {
            return SmlouvyPodleCeny("-id:pre* ");
        }
            public static ReportDataSource SmlouvyPodleCeny(string query)
        {
            DateTime minDate = new DateTime(2012, 1, 1);
            DateTime maxDate = DateTime.Now.Date.AddDays(1);

            double[] ranks = new double[] { 0, 50000, 100000, 500000, 1000000, 5000000, 10000000 };
            AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva> aggs = new AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>()
                .PercentileRanks("x-agg", h => h
                    .Field(f => f.CalculatedPriceWithVATinCZK)
                    .Values(ranks)
                );

            //var res = HlidacStatu.Lib.Data.Smlouva.Search.RawSearch("{\"query_string\": { \"query\": \"-id:pre*\" } }", 1, 0, anyAggregation: aggs);
            var res = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch("(" + query + ") AND datumUzavreni:{" + HlidacStatu.Util.RenderData.ToElasticDate(minDate) + " TO " + HlidacStatu.Util.RenderData.ToElasticDate(maxDate) + "}", 1, 0, HlidacStatu.Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, anyAggregation: aggs, exactNumOfResults: true);

            ReportDataSource rds = new ReportDataSource(new ReportDataSource.Column[]
                {
                    new ReportDataSource.Column() { Name="Hodnota smlouvy",
                        TextRender = (s) => { return s.ToString(); }
                    },
                    new ReportDataSource.Column() {
                        Name ="% smluv",
                        TextRender = (s) => { return ((double)s).ToString("N2") + "%"; },
                        HtmlRender = (s) => { return ((double)s).ToString("N2") + "%"; },
                        OrderValueRender = (s) => { return ((double)s).ToString("N2"); }
                    },
                }
            );
            rds.Title = "Smlouvy podle hodnoty";
            var data = ((PercentilesAggregate)res.ElasticResults.Aggregations["x-agg"]).Items.ToArray();
            double prevVal = 0;
            double prevPercent = 0;
            for (int i = 0; i < data.Count(); i++)
            {
                string x = data[i].Percentile.ToString("N0") + " Kč";
                if (i > 0)
                {
                    x = data[i - 1].Percentile.ToString("N0") + " Kč -" + x;
                }
                else
                    x = "Bez ceny";
                rds.AddRow(x, data[i].Value - prevVal);
                prevVal = data[i].Value ?? 0; //todo: es7 check
            }
            rds.AddRow("nad " + data[data.Count() - 1].Percentile.ToString("N0") + " Kč", 100 - prevVal);
            return rds;
        }



    }
}