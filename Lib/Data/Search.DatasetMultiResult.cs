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
            public DatasetSumGeneralResult(long total, External.DataSets.DataSet dataset, TimeSpan searchElapsedTime)
                : base(total, new string[] { }, true)
            {
                this.Dataset = dataset;
                this.DataSource = "Dataset."+ this.Dataset.DatasetId;
                this.ElapsedTime = searchElapsedTime;
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


            public static DatasetMultiResult GeneralSearch(string query, IEnumerable<External.DataSets.DataSet> datasets = null)
            {
                DatasetMultiResult res = new DatasetMultiResult() { Query = query, DataSource= "DatasetMultiResult.GeneralSearch" };

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
                            var rds = ds.SearchData(query, 1, 1);
                            if (rds.IsValid)
                                res.Results.Add(new DatasetSumGeneralResult(rds.Total, ds, rds.ElapsedTime));
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
