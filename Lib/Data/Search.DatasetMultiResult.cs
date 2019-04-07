using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public partial class Search
    {
        public class DatasetSumGeneralResult : GeneralResult<string>
        {
            public Lib.Data.External.DataSets.DataSet Dataset { get; set; }
            public DatasetSumGeneralResult(long total, IEnumerable<string> results, External.DataSets.DataSet dataset, TimeSpan searchElapsedTime)
                : base(total, results, true)
            {
                this.Dataset = dataset;
                this.DataSource = "Dataset." + this.Dataset.DatasetId;
                this.ElapsedTime = searchElapsedTime;
            }


            public string RenderResultsInHtml(string query, int maxToRender = int.MaxValue)
            {
                var content = "";
                try
                {
                    if (this.Dataset.Registration()?.searchResultTemplate?.IsFullTemplate() == true)
                    {
                        var model = new HlidacStatu.Lib.Data.External.DataSets.Registration.Template.SearchTemplateResults();
                        model.Total = this.Total;
                        model.Page = 1;
                        model.Q = query;
                        model.Result = this.Result
                            .Take(maxToRender)
                            .Select(s => { dynamic d = Newtonsoft.Json.Linq.JObject.Parse(s); return d; })
                            .ToArray();

                        content = this.Dataset.Registration().searchResultTemplate.Render(this.Dataset, model);
                    }
                    else
                    {
                        //content = ControllerExtensions.RenderRazorViewToString(this.ViewContext.Controller, "HledatProperties_CustomdataTemplate", rds);
                        //Html.RenderAction("HledatProperties_CustomdataTemplate", rds);
                        content = "<h3>Nepodařilo se nám zobrazit vyhledané výsledky</h3>" +
                                    $"<div class=\"text-center\"><a class=\"btn btn-default btn-default-new\" href=\"{this.Dataset.DatasetSearchUrl(query)}\">zobrazit všechny nalezené záznamy zde</a></div>";

                    }
                    if (this.Total > 5)
                    {
                        content += $"<div class=\"text-center\"><a class=\"btn btn-default btn-default-new\" href=\"{this.Dataset.DatasetSearchUrl(query)}\">zobrazit všechny nalezené záznamy</a></div>";
                    }
                    return content;
                }
                catch (Exception)
                {
                    return "<h3>Nepodařilo se nám zobrazit vyhledané výsledky</h3>" +
                        $"<div class=\"text-center\"><a class=\"btn btn-default btn-default-new\" href=\"{this.Dataset.DatasetSearchUrl(query)}\">zobrazit všechny nalezené záznamy zde</a></div>";
                }
            }
        }

        public class DatasetMultiResult : ISearchResult
        {
            public string Query { get; set; }
            public long Total { get { return Results.Sum(m => m.Total); } }
            public bool IsValid { get { return Results.All(m => m.IsValid || Exceptions.Any()); } }
            public bool HasResult { get { return IsValid && this.Total > 0; } }
            public string DataSource { get; set; }
            public TimeSpan ElapsedTime { get; set; } = TimeSpan.Zero;

            public System.Collections.Concurrent.ConcurrentBag<DatasetSumGeneralResult> Results { get; set; }
                = new System.Collections.Concurrent.ConcurrentBag<DatasetSumGeneralResult>();

            public System.Collections.Concurrent.ConcurrentBag<System.Exception> Exceptions { get; set; }
                = new System.Collections.Concurrent.ConcurrentBag<System.Exception>();


            public System.AggregateException GetExceptions()
            {
                if (this.Exceptions.Count == 0)
                    return null;

                System.AggregateException agg = new System.AggregateException(
                    "Aggregated exceptions from DatasetMultiResult",
                    Exceptions
                    );
                return agg;
            }

            static object objGeneralSearchLock = new object();


            public static DatasetMultiResult GeneralSearch(string query, IEnumerable<External.DataSets.DataSet> datasets = null, int page = 1, int pageSize = 20, string sort = null)
            {
                DatasetMultiResult res = new DatasetMultiResult() { Query = query, DataSource = "DatasetMultiResult.GeneralSearch" };

                if (string.IsNullOrEmpty(query))
                    return res;

                if (datasets == null)
                    datasets = Lib.Data.External.DataSets.DataSetDB.ProductionDataSets.Get();

                ParallelOptions po = new ParallelOptions();
                po.MaxDegreeOfParallelism = System.Diagnostics.Debugger.IsAttached ? 1 : po.MaxDegreeOfParallelism;

                Devmasters.Core.StopWatchEx sw = new Devmasters.Core.StopWatchEx();
                sw.Start();
                Parallel.ForEach(datasets, po,
                    ds =>
                    {
                        try
                        {
                            var rds = ds.SearchDataRaw(query, page, pageSize, sort);
                            if (rds.IsValid)
                            {
                                var dssr = new DatasetSumGeneralResult(rds.Total, rds.Result.Select(s => s.Item2), ds, rds.ElapsedTime);
                                res.Results.Add(dssr);
                            }
                        }
                        catch (External.DataSets.DataSetException e)
                        {
                            res.Exceptions.Add(e);
                        }
                        catch (System.Exception e)
                        {
                            res.Exceptions.Add(e);
                            //HlidacStatu.Util.Consts.Logger.Warning("DatasetMultiResult GeneralSearch for query" + query, e);
                        }

                    });
                sw.Stop();
                res.ElapsedTime = sw.Elapsed;
                return res;
            }


        }
    }
}
