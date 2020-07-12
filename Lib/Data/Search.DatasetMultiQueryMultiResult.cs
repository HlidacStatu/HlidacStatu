using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public partial class Search
    {

        public class DatasetMultiQueryMultiResult : ISearchResult
        {
            public System.TimeSpan ElapsedTime { get; set; }
            public long Total { get { return Results.Sum(m => m.Total); } }
            public bool IsValid { get { return Results.All(m => m.IsValid || Exceptions.Any()); } }
            public bool HasResult { get { return IsValid && this.Total > 0; } }
            public string DataSource { get; set; }
            public int PageSize { get; set; }
            public int Page { get; set; }
            public string Order { get; set; }

            public string Query { get; set; }

            public virtual int MaxResultWindow() { return Lib.Searching.Tools.MaxResultWindow; }

            public virtual object ToRouteValues(int page)
            {
                return new
                {
                    Q = Query ,
                    Page = page,
                };
            }


            public System.Collections.Concurrent.ConcurrentBag<External.DataSets.DataSearchResult> Results { get; set; }
                = new System.Collections.Concurrent.ConcurrentBag<External.DataSets.DataSearchResult>();

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

           
            public static DatasetMultiQueryMultiResult GeneralSearch(Dictionary<External.DataSets.DataSet, string> datasetsWithQuery, int page, int pageSize)
            {
                DatasetMultiQueryMultiResult res = new DatasetMultiQueryMultiResult() {
                    DataSource = "DatasetMultiQueryMultiResult.GeneralSearch"
                };

                if (datasetsWithQuery == null || datasetsWithQuery.Count == 0)
                    return res;

                if (!Lib.Searching.Tools.ValidateQuery(datasetsWithQuery.First().Value))
                {
                    res.Exceptions.Add(new System.Exception($"Invalid Query: {datasetsWithQuery.First().Value}"));
                    return res;                   
                }

                ParallelOptions po = new ParallelOptions();
                po.MaxDegreeOfParallelism = System.Diagnostics.Debugger.IsAttached ? 1 : po.MaxDegreeOfParallelism;

                Parallel.ForEach(datasetsWithQuery, po,
                    ds =>
                    {
                        try
                        {
                            External.DataSets.DataSearchResult rds = ds.Key.SearchData(ds.Value, page, pageSize);

                            if (rds.IsValid)
                                res.Results.Add(rds);
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

                return res;
            }

        }
    }
}
