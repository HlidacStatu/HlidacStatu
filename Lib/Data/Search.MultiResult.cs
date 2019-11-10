using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public partial class Search
    {

        public class MultiResult
        {
            public System.TimeSpan TotalSearchTime { get; set; } = System.TimeSpan.Zero;
            public System.TimeSpan AddOsobyTime { get; set; } = System.TimeSpan.Zero;
            public string Query { get; set; }
            public Lib.ES.SmlouvaSearchResult Smlouvy { get; set; } = null;
            public Lib.ES.VerejnaZakazkaSearchData VZ { get; set; } = null;
            public GeneralResult<Osoba> Osoby { get; set; } = null;
            public bool OsobaFtx = false;
            public GeneralResult<TransparentniUcty.BankovniPolozka> Transakce { get; set; } = null;
            public GeneralResult<string> Firmy { get; set; } = null;
            public DatasetMultiResult Datasets { get; set; }
			public ES.InsolvenceSearchResult Insolvence { get; set; } = null;

            public bool HasSmlouvy { get { return (Smlouvy != null && Smlouvy.HasResult); } }
            public bool HasVZ { get { return (VZ != null && VZ.HasResult); } }
            public bool HasOsoby { get { return (Osoby != null && Osoby.HasResult); } }
            public bool HasTransakce { get { return (Transakce != null && Transakce.HasResult); } }
            public bool HasFirmy { get { return (Firmy != null && Firmy.HasResult); } }
            public bool HasDatasets { get { return (Datasets != null && Datasets.HasResult); } }
			public bool HasInsolvence { get { return Insolvence != null && Insolvence.HasResult; } }

            public Dictionary<string, System.TimeSpan> SearchTimes()
            {
                Dictionary<string, System.TimeSpan> times = new Dictionary<string, System.TimeSpan>();
                if (Smlouvy != null)
                    times.Add("Smlouvy", Smlouvy.ElapsedTime);
                if (VZ != null)
                    times.Add("VZ", VZ.ElapsedTime);
                if (Osoby != null)
                    times.Add("Osoby", Osoby.ElapsedTime);
                if (Transakce != null)
                    times.Add("Transakce", Transakce.ElapsedTime);
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
                    return this.Smlouvy.IsValid
                        && this.VZ.IsValid
                        && this.Osoby.IsValid
                        && this.Transakce.IsValid
                        && this.Firmy.IsValid
                        && this.Datasets.IsValid
						//&& this.Insolvence.IsValid
                        ;
                }
            }

            public bool HasResults
            {
                get
                {
                    return HasSmlouvy || HasVZ || HasOsoby || HasTransakce || HasFirmy || HasDatasets || HasInsolvence;
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
                    if (HasTransakce)
                        t += Transakce.Total;
                    if (HasFirmy)
                        t += Firmy.Total;
                    if (HasDatasets)
                        t += Datasets.Total;
					if (HasInsolvence)
						t += Insolvence.Total;

                    return t;
                }
            }

        }


        public static MultiResult GeneralSearch(string query, int page = 1, int pageSize = 10, bool showBeta = false)
        {
            return Elastic.Apm.Agent.Tracer.CaptureTransaction<MultiResult>("GeneralSearch", "search",
                (tran) =>
                {
                    tran.Labels.Add("query", query);
                    return GeneralSearch(tran, query, page, pageSize, showBeta);
                }
            );
        }


        static object objGeneralSearchLock = new object();
        private static MultiResult GeneralSearch(Elastic.Apm.Api.ITransaction apmtran, string query, int page = 1, int pageSize = 10, bool showBeta = false)
        {
            MultiResult res = new MultiResult() { Query = query };

            if (string.IsNullOrEmpty(query))
                return res;

            if (!Lib.Search.Tools.ValidateQuery(query))
            {
                res.Smlouvy = new ES.SmlouvaSearchResult();
                res.Smlouvy.Q = query;
                res.Smlouvy.IsValid = false;
                
                return res;
            }

            var totalsw = new Devmasters.Core.StopWatchEx();
            totalsw.Start();

            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = 20;
            //po.MaxDegreeOfParallelism = System.Diagnostics.Debugger.IsAttached ? 1 : po.MaxDegreeOfParallelism;

            Parallel.Invoke(po,               
                () =>
                {
                    Elastic.Apm.Api.ISpan sp = null;
                    try
                    {
                        apmtran.CaptureSpan("Smlouvy", "search", () =>
                        {
                            res.Smlouvy = HlidacStatu.Lib.ES.SearchTools.SimpleSearch(query, 1, 20, ES.SearchTools.OrderResult.Relevance,
                                anyAggregation: new Nest.AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>().Sum("sumKc", m => m.Field(f => f.CalculatedPriceWithVATinCZK))
                                );
                        });
                    }
                    catch (System.Exception e)
                    {
                        HlidacStatu.Util.Consts.Logger.Error("MultiResult GeneralSearch for Smlouvy query" + query, e);
                    }
                    finally
                    {
                        sp?.End();
                    }

                },
                () =>
                {
                    try
                    {
                        Devmasters.Core.StopWatchEx sw = new Devmasters.Core.StopWatchEx();
                        sw.Start();

                        res.Firmy = new GeneralResult<string>(Firma.Search.FindAllIco(query, 50));
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
                        res.VZ = VZ.VerejnaZakazka.Searching.SimpleSearch(query, null, 1, 5, (int)ES.VerejnaZakazkaSearchData.VZOrderResult.Relevance);
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
                        Devmasters.Core.StopWatchEx sw = new Devmasters.Core.StopWatchEx();
                        sw.Start();

                        if (!string.IsNullOrEmpty(query) && query.Length > 2)
                            res.Osoby = new GeneralResult<Osoba>(
                                HlidacStatu.Lib.Data.Osoba.GetPolitikByNameFtx(query, 100)
                                .OrderBy(m => m.Prijmeni)
                                .ThenBy(m => m.Jmeno)
                                );
                        else
                            res.Osoby = new GeneralResult<Osoba>(new Osoba[] { });
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
                        Devmasters.Core.StopWatchEx sw = new Devmasters.Core.StopWatchEx();
                        sw.Start();
                        var buRes = HlidacStatu.Lib.Data.TransparentniUcty.BankovniUcty.SearchPolozkyRaw(query, null, 20);
                        if (buRes != null && buRes.IsValid)
                            res.Transakce = new GeneralResult<TransparentniUcty.BankovniPolozka>(
                                buRes.Total,
                                buRes.Hits.Select(m => m.Source),
                                buRes.IsValid
                                );
                        else
                            res.Transakce = new GeneralResult<TransparentniUcty.BankovniPolozka>(new TransparentniUcty.BankovniPolozka[] { });
                        sw.Stop();
                        res.Transakce.ElapsedTime = sw.Elapsed;
                    }
                    catch (System.Exception e)
                    {
                        HlidacStatu.Util.Consts.Logger.Error("MultiResult GeneralSearch for bankovni ucty query" + query, e);
                    }

                },
				() =>
				{
					try
					{
                        var iqu = new ES.InsolvenceSearchResult { Q = query, PageSize = 5 };
                        res.Insolvence = iqu;
                        //if (showBeta)
                        res.Insolvence = Insolvence.Insolvence.SimpleSearch(new ES.InsolvenceSearchResult { Q = query, PageSize = 5 });

                    }
                    catch (System.Exception e)
					{
						Util.Consts.Logger.Error("MultiResult GeneralSearch for insolvence query" + query, e);
					}

				},
                () =>
                 {
                     Elastic.Apm.Api.ISpan sp = null;
                     try
                     {
                         apmtran.CaptureSpan("Dataset GeneralSearch", "search", () =>
                         {

                             res.Datasets = Lib.Data.Search.DatasetMultiResult.GeneralSearch(query, null, 1, 5);
                             if (res.Datasets.Exceptions.Count > 0)
                             {
                                 HlidacStatu.Util.Consts.Logger.Error("MultiResult GeneralSearch for DatasetMulti query " + query,
                                     res.Datasets.GetExceptions());
                             }
                         });
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
            if (false && res.HasFirmy && (res.Osoby == null || res.Osoby.Total < 5))
            {
                var sw = new Devmasters.Core.StopWatchEx();
                sw.Start();
                if (res.Osoby == null)
                    res.Osoby = new GeneralResult<Osoba>(new Osoba[] { });

                res.Osoby = new GeneralResult<Osoba>(res.Osoby.Result
                            .Concat(Osoba.GetPolitikByQueryFromFirmy(query, (int)(10 - (res.Osoby?.Total ?? 0)), res.Firmy.Result)
                            )
                        );
                res.OsobaFtx = true;
                sw.Stop();
                res.AddOsobyTime = sw.Elapsed;
            }

            totalsw.Stop();
            res.TotalSearchTime = totalsw.Elapsed;

            return res;
        }


    }
}
