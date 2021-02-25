using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HlidacStatu.Lib.Searching;

namespace HlidacStatu.Lib.Data
{
    public partial class Search
    {

        public class MultiResult
        {
            public System.TimeSpan TotalSearchTime { get; set; } = System.TimeSpan.Zero;
            public System.TimeSpan AddOsobyTime { get; set; } = System.TimeSpan.Zero;
            public string Query { get; set; }
            public Lib.Searching.SmlouvaSearchResult Smlouvy { get; set; } = null;
            public Lib.Searching.VerejnaZakazkaSearchData VZ { get; set; } = null;
            public OsobaSearchResult Osoby { get; set; } = null;
            public bool OsobaFtx = false;
            public GeneralResult<Firma> Firmy { get; set; } = null;
            public DatasetMultiResult Datasets { get; set; }
			public InsolvenceSearchResult Insolvence { get; set; } = null;
            public DotaceSearchResult Dotace { get; set; } = null;

            public bool HasSmlouvy { get { return (Smlouvy != null && Smlouvy.HasResult); } }
            public bool HasVZ { get { return (VZ != null && VZ.HasResult); } }
            public bool HasOsoby { get { return (Osoby != null && Osoby.HasResult); } }
            public bool HasFirmy { get { return (Firmy != null && Firmy.HasResult); } }
            public bool HasDatasets { get { return (Datasets != null && Datasets.HasResult); } }
			public bool HasInsolvence { get { return Insolvence != null && Insolvence.HasResult; } }
            public bool HasDotace { get { return Dotace != null && Dotace.HasResult; } }

            public Dictionary<string, System.TimeSpan> SearchTimes()
            {
                Dictionary<string, System.TimeSpan> times = new Dictionary<string, System.TimeSpan>();
                if (Smlouvy != null)
                    times.Add("Smlouvy", Smlouvy.ElapsedTime);
                if (VZ != null)
                    times.Add("VZ", VZ.ElapsedTime);
                if (Osoby != null)
                    times.Add("Osoby", Osoby.ElapsedTime);
                if (Firmy != null)
                    times.Add("Firmy", Firmy.ElapsedTime);
                if (Firmy != null)
                    times.Add("Dataset.Total", Datasets.ElapsedTime);
				if (Datasets != null)
                {
                    foreach (var ds in Datasets.Results)
                    {
                        times.Add("Dataset." + ds.DataSet.DatasetId, ds.ElapsedTime);
                    }
                }
				if (Insolvence != null)
					times.Add("Insolvence", Insolvence.ElapsedTime);
                if (Dotace != null)
                    times.Add("Dotace", Dotace.ElapsedTime);
                if (AddOsobyTime.Ticks > 0)
                    times.Add("AddOsobyTime", AddOsobyTime);

                if (TotalSearchTime.Ticks > 0)
                    times.Add("Total", TotalSearchTime);
				return times;
            }

            public string SearchTimesReport(string delimiter = "\n")
            {
                var times = SearchTimes();
                if (times == null || times.Count() == 0)
                    return string.Empty;

                return times
                    .Select(kv => $"{kv.Key}: {kv.Value.TotalMilliseconds.ToString()}")
                    .Aggregate((f, s) => f + delimiter + s);
            }

            public bool IsValid
            {
                get
                {
                    return (this.Smlouvy?.IsValid ?? false)
                        && (this.VZ?.IsValid ?? false)
                        && (this.Osoby?.IsValid ?? false)
                        && (this.Firmy?.IsValid ?? false)
                        && (this.Datasets?.IsValid ?? false)
						&& (this.Insolvence?.IsValid ?? false)
                        && (this.Dotace?.IsValid ?? false)
                        ;
                }
            }

            public bool HasResults
            {
                get
                {
                    return HasSmlouvy || HasVZ || HasOsoby || HasFirmy || HasDatasets || HasInsolvence || HasDotace;
                }
            }

            public long Total
            {
                get
                {
                    long t = 0;
                    if (HasSmlouvy)
                        t += Smlouvy.Total;
                    if (HasVZ)
                        t += VZ.Total;
                    if (HasOsoby)
                        t += Osoby.Total;
                    if (HasFirmy)
                        t += Firmy.Total;
                    if (HasDatasets)
                        t += Datasets.Total;
					if (HasInsolvence)
						t += Insolvence.Total;
                    if (HasDotace)
                        t += Dotace.Total;

                    return t;
                }
            }

        }



        static object objGeneralSearchLock = new object();
        public static MultiResult GeneralSearch(string query, int page = 1, int pageSize = 10, bool showBeta = false, string order = null)  
        {
            MultiResult res = new MultiResult() { Query = query };

            if (string.IsNullOrEmpty(query))
                return res;

            if (!Lib.Searching.Tools.ValidateQuery(query))
            {
                res.Smlouvy = new Searching.SmlouvaSearchResult();
                res.Smlouvy.Q = query;
                res.Smlouvy.IsValid = false;
                
                return res;
            }

            var totalsw = new Devmasters.DT.StopWatchEx();
            totalsw.Start();

            ParallelOptions po = new ParallelOptions();
            //po.MaxDegreeOfParallelism = 20;
            po.MaxDegreeOfParallelism = System.Diagnostics.Debugger.IsAttached ? 1 : po.MaxDegreeOfParallelism;

            Parallel.Invoke(po,               
                () =>
                {
                    try
                    {
                            res.Smlouvy = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch(query, 1, pageSize, order,
                                anyAggregation: new Nest.AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>().Sum("sumKc", m => m.Field(f => f.CalculatedPriceWithVATinCZK))
                                );
                    }
                    catch (System.Exception e)
                    {
                        HlidacStatu.Util.Consts.Logger.Error("MultiResult GeneralSearch for Smlouvy query" + query, e);
                    }
                    finally
                    {
                    }

                },
                () =>
                {
                    try
                    {
                        Devmasters.DT.StopWatchEx sw = new Devmasters.DT.StopWatchEx();
                        sw.Start();

                        res.Firmy = Firma.Search.SimpleSearch(query, 0, 50);
                        sw.Stop();
                        res.Firmy.ElapsedTime = sw.Elapsed;
                    }
                    catch (System.Exception e)
                    {
                        HlidacStatu.Util.Consts.Logger.Error("MultiResult GeneralSearch for Firmy query" + query, e);
                    }

                },
                () =>
                {
                    try
                    {
                        res.VZ = VZ.VerejnaZakazka.Searching.SimpleSearch(query, null, 1, pageSize, order);
                    }
                    catch (System.Exception e)
                    {
                        HlidacStatu.Util.Consts.Logger.Error("MultiResult GeneralSearch for Verejne zakazky query" + query, e);
                    }

                },
                () =>
                {
                    try
                    {
                        Devmasters.DT.StopWatchEx sw = new Devmasters.DT.StopWatchEx();
                        sw.Start();

                        res.Osoby = Osoba.Search.SimpleSearch(query, 1, 10, Osoba.Search.OrderResult.Relevance);
                        sw.Stop();
                        res.Osoby.ElapsedTime = sw.Elapsed;

                    }
                    catch (System.Exception e)
                    {
                        HlidacStatu.Util.Consts.Logger.Error("MultiResult GeneralSearch for Osoba query" + query, e);
                    }


                },
				() =>
				{
					try
					{
                        var iqu = new Searching.InsolvenceSearchResult { Q = query, PageSize = pageSize, Order = order };
                        res.Insolvence = iqu;
                        //if (showBeta)
                        res.Insolvence = Insolvence.Insolvence.SimpleSearch(new Searching.InsolvenceSearchResult { Q = query, PageSize = pageSize, Order = order });

                    }
                    catch (System.Exception e)
					{
						Util.Consts.Logger.Error("MultiResult GeneralSearch for insolvence query" + query, e);
					}

				},
                () =>
                {
                    try
                    {
                        var dotaceService = new Dotace.DotaceService();
                        var iqu = new Searching.DotaceSearchResult { Q = query, PageSize = pageSize, Order = order };
                        res.Dotace = dotaceService.SimpleSearch(
                                new Searching.DotaceSearchResult { Q = query, PageSize = pageSize, Order = order },
                                anyAggregation: new Nest.AggregationContainerDescriptor<Lib.Data.Dotace.Dotace>().Sum("souhrn", s => s.Field(f => f.DotaceCelkem))
                            );
                    }
                    catch (System.Exception e)
                    {
                        Util.Consts.Logger.Error("MultiResult GeneralSearch for insolvence query" + query, e);
                    }

                },
                () =>
                 {
                     try
                     {
                             res.Datasets = Lib.Data.Search.DatasetMultiResult.GeneralSearch(query, null, 1, 5);
                             if (res.Datasets.Exceptions.Count > 0)
                             {
                                 HlidacStatu.Util.Consts.Logger.Error("MultiResult GeneralSearch for DatasetMulti query " + query,
                                     res.Datasets.GetExceptions());
                             }
                     }
                     catch (System.Exception e)
                     {
                         HlidacStatu.Util.Consts.Logger.Error("MultiResult GeneralSearch for DatasetMulti query " + query, e);
                     }
                     finally
                     {
                     }
                 }

            );

            //TODO too slow, temporarily disabled

            totalsw.Stop();
            res.TotalSearchTime = totalsw.Elapsed;

            return res;
        }


    }
}
