﻿using Devmasters;
using Devmasters.Batch;
using Devmasters.Enums;

using Nest;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public class AnalysisCalculation
    {


        public class VazbyFiremNaPolitiky
        {
            public Dictionary<string, List<int>> SoukromeFirmy = null;
            public Dictionary<string, List<int>> StatniFirmy = null;
        }

        public class VazbyFiremNaUradyStat
        {
            public IEnumerable<Analysis.BasicDataForSubject<List<Analysis.BasicData<string>>>> SoukromeFirmy = 
                new List<Analysis.BasicDataForSubject<List<Analysis.BasicData<string>>>>();
            public IEnumerable<Analysis.BasicDataForSubject<List<Analysis.BasicData<string>>>> StatniFirmy = 
                new List<Analysis.BasicDataForSubject<List<Analysis.BasicData<string>>>>();
        }
        public class IcoSmlouvaMinMax
        {
            public string ico;
            public string jmeno;
            DateTime? _minUzavreni = null;
            public DateTime? minUzavreni
            {
                get { return _minUzavreni; }
                set
                {
                    _minUzavreni = value;
                    setDays();
                }
            }
            public DateTime? maxUzavreni;

            DateTime? _vznikIco = null;
            public DateTime? vznikIco
            {
                get { return _vznikIco; }
                set
                {
                    _vznikIco = value;
                    setDays();
                }
            }

            double? _days = null;
            public double? days
            {
                get
                {
                    return _days;
                }
            }
            private void setDays()
            {
                if (minUzavreni.HasValue && vznikIco.HasValue)
                    _days = (minUzavreni.Value - vznikIco.Value).TotalDays;
                else
                    _days = null;

            }
        }

     

        private static List<Lib.Data.Smlouva> SimpleSmlouvyForIco(string ico, DateTime? from, DateTime? to)
        {
            Func<int, int, Nest.ISearchResponse<Lib.Data.Smlouva>> searchFunc = searchFunc = (size, page) =>
            {
                var sdate = "";
                if (from.HasValue)
                    sdate = $" AND podepsano:[{from?.ToString("yyyy-MM-dd") ?? "*"} TO {from?.ToString("yyyy-MM-dd") ?? DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")}]"; //podepsano:[2016-01-01 TO 2016-12-31]

                
                return Lib.ES.Manager.GetESClient().Search<Lib.Data.Smlouva>(a => a
                            .TrackTotalHits(page*size == 0)
                            .Size(size)
                            .From(page * size)
                            .Source(m => m.Excludes(e => e.Field(o => o.Prilohy)))
                            .Query(q => Lib.Data.Smlouva.Search.GetSimpleQuery("ico:" + ico + sdate))
                            .Scroll("1m")
                            );
            };

            List<Smlouva> smlouvy = new List<Smlouva>();
            Lib.Searching.Tools.DoActionForQuery<Lib.Data.Smlouva>(Lib.ES.Manager.GetESClient(), searchFunc,
                  (hit, o) =>
                  {
                      smlouvy.Add(hit.Source);
                      return new ActionOutputData();
                  }, null,
                null, null, false);


            return smlouvy;
        }

        public static Tuple<VazbyFiremNaUradyStat, VazbyFiremNaUradyStat> UradyObchodujiciSFirmami_NespolehlivymiPlatciDPH(bool showProgress = false)
        {
            var nespolehliveFirmy = StaticData.NespolehlivyPlatciDPH.Get();

            Dictionary<string, Analysis.BasicDataForSubject<List<Analysis.BasicData<string>>>> uradyData 
                = new Dictionary<string, Analysis.BasicDataForSubject<List<Analysis.BasicData<string>>>>();

            Dictionary<string, Analysis.BasicDataForSubject<List<Analysis.BasicData<string>>>> nespolehliveFirmyKontrakty 
                = new Dictionary<string, Analysis.BasicDataForSubject<List<Analysis.BasicData<string>>>>();

            var lockObj = new object();

            Devmasters.Batch.Manager.DoActionForAll<NespolehlivyPlatceDPH>(nespolehliveFirmy.Values,
                (nf) =>
                {
                    var nespolehlivaFirma = Firmy.Get(nf.Ico);
                    var ico = nf.Ico;
                    var smlouvy = SimpleSmlouvyForIco(ico, nf.FromDate, nf.ToDate);
                    foreach (var s in smlouvy)
                    {
                        var allIco = new List<string>(
                                s.Prijemce.Select(m => m.ico).Where(p => !string.IsNullOrEmpty(p))
                                );
                        allIco.Add(s.Platce.ico);
                        var urady = allIco.Select(i => Firmy.Get(i)).Where(f => f.PatrimStatu());

                        foreach (var urad in urady)
                        {
                            lock (lockObj)
                            {
                                if (!uradyData.ContainsKey(urad.ICO))
                                {
                                    uradyData.Add(urad.ICO, new Analysis.BasicDataForSubject<List<Analysis.BasicData<string>>>() { Item  =urad.ICO });

                                }
                                uradyData[urad.ICO].Add(1, s.CalculatedPriceWithVATinCZK);
                                if (!uradyData[urad.ICO].Detail.Any(m => m.Item == ico))
                                {
                                    uradyData[urad.ICO].Detail.Add(new Analysis.BasicData<string>() { Item = ico, CelkemCena = s.CalculatedPriceWithVATinCZK, Pocet = 1  });
                                }
                                else
                                {
                                    var item = uradyData[urad.ICO].Detail.First(m => m.Item == ico);
                                    item.Pocet++;
                                    item.CelkemCena += s.CalculatedPriceWithVATinCZK;
                                }

                                //--------------
                                if (!nespolehliveFirmyKontrakty.ContainsKey(ico))
                                {
                                    nespolehliveFirmyKontrakty.Add(ico, new Analysis.BasicDataForSubject<List<Analysis.BasicData<string>>>());
                                    nespolehliveFirmyKontrakty[ico].Ico = ico;
                                }
                                nespolehliveFirmyKontrakty[ico].Add(1, s.CalculatedPriceWithVATinCZK);
                                if (!nespolehliveFirmyKontrakty[ico].Detail.Any(m => m.Item == urad.ICO))
                                {
                                    nespolehliveFirmyKontrakty[ico].Detail.Add(new Analysis.BasicData<string>() { Item = urad.ICO, CelkemCena = s.CalculatedPriceWithVATinCZK, Pocet = 1 });
                                }
                                else
                                {
                                    var item = nespolehliveFirmyKontrakty[ico].Detail.First(m => m.Item == urad.ICO);
                                    item.Add(1, s.CalculatedPriceWithVATinCZK);
                                }


                            }
                        }
                    }

                    return new ActionOutputData();
                },
                showProgress ? Devmasters.Batch.Manager.DefaultOutputWriter : (Action<string>)null,
                showProgress ? new Devmasters.Batch.ActionProgressWriter().Write : (Action<ActionProgressData>)null,
                !System.Diagnostics.Debugger.IsAttached, maxDegreeOfParallelism:5);

            VazbyFiremNaUradyStat ret = new VazbyFiremNaUradyStat();
            ret.StatniFirmy = uradyData
            .Where(m => m.Value.Pocet > 0)
            .Select(kv => kv.Value)
            .OrderByDescending(o => o.Pocet)
            .ToList();

            VazbyFiremNaUradyStat retNespolehliveFirmy = new VazbyFiremNaUradyStat();
            retNespolehliveFirmy.SoukromeFirmy = nespolehliveFirmyKontrakty
                .Where(m => m.Value.Pocet > 0)
                .Select(kv => kv.Value)
                .OrderByDescending(o => o.Pocet)
                .ToList();

            return new Tuple<VazbyFiremNaUradyStat, VazbyFiremNaUradyStat>(ret, retNespolehliveFirmy);
        }


        public static AnalysisCalculation.VazbyFiremNaUradyStat UradyObchodujiciSFirmami_s_vazbouNaPolitiky(Relation.AktualnostType aktualnost, bool showProgress = false)
        {
            HlidacStatu.Lib.Data.AnalysisCalculation.VazbyFiremNaPolitiky vazbyNaPolitiky = null;
            List<Sponzoring> sponzorujiciFirmy = null;

            QueryContainer qc = null;

            switch (aktualnost)
            {
                case HlidacStatu.Lib.Data.Relation.AktualnostType.Aktualni:
                    vazbyNaPolitiky = StaticData.FirmySVazbamiNaPolitiky_aktualni_Cache.Get();
                    qc = new QueryContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>().Term(t=>t.Field(f=>f.SVazbouNaPolitikyAktualni).Value(true));
                    sponzorujiciFirmy = StaticData.SponzorujiciFirmy_Nedavne.Get();
                    break;
                case HlidacStatu.Lib.Data.Relation.AktualnostType.Nedavny:
                    vazbyNaPolitiky = StaticData.FirmySVazbamiNaPolitiky_nedavne_Cache.Get();
                    qc = new QueryContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>().Term(t => t.Field(f => f.SVazbouNaPolitikyNedavne).Value(true));
                    sponzorujiciFirmy = StaticData.SponzorujiciFirmy_Nedavne.Get();
                    break;
                case HlidacStatu.Lib.Data.Relation.AktualnostType.Neaktualni:
                case HlidacStatu.Lib.Data.Relation.AktualnostType.Libovolny:
                    vazbyNaPolitiky = StaticData.FirmySVazbamiNaPolitiky_vsechny_Cache.Get();
                    qc = new QueryContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>().Term(t => t.Field(f => f.SVazbouNaPolitiky).Value(true));
                    sponzorujiciFirmy = StaticData.SponzorujiciFirmy_Vsechny.Get();
                    break;
            }

                
            Func<int, int, Nest.ISearchResponse<Lib.Data.Smlouva>> searchFunc = null;
            searchFunc = (size, page) =>
            {
                return Lib.ES.Manager.GetESClient().Search<Lib.Data.Smlouva>(a => a
                            .TrackTotalHits(page * size == 0)
                            .Size(size)
                            .From(page * size)
                            .Source(m => m.Excludes(e => e.Field(o => o.Prilohy)))
                            .Query(q => qc)
                            .Scroll("1m")
                            );
            };


            //TODO predelat z projeti vsech smluv na hledani pres vsechna ICO  v RS, vybrani statnich firem, 
            //a dohlednai jejich statistiky vuci jednotlivym ostatnim firmam v RS
            Dictionary<string, Analysis.BasicDataForSubject<List<Analysis.BasicData<string>>>> uradyStatni = new Dictionary<string, Analysis.BasicDataForSubject<List<Analysis.BasicData<string>>>>();
            Dictionary<string, Analysis.BasicDataForSubject<List<Analysis.BasicData<string>>>> uradySoukr = new Dictionary<string, Analysis.BasicDataForSubject<List<Analysis.BasicData<string>>>>();
            object lockObj = new object();
            Lib.Searching.Tools.DoActionForQuery<Lib.Data.Smlouva>(Lib.ES.Manager.GetESClient(), searchFunc,
                  (hit, param) =>
                  {
                      Lib.Data.Smlouva s = hit.Source;
                      List<string> icos = new List<string>();
                      try
                      {
                          var objednatelIco = s.Platce.ico;
                          if (!string.IsNullOrEmpty(objednatelIco))
                          {
                              Firma ff = Firmy.Get(objednatelIco);
                              if (!ff.Valid || !ff.PatrimStatu())
                                  goto end;

                              //vsichni prijemci smlouvy statniho subjektu
                              icos.AddRange(s.Prijemce.Select(m => m.ico).Where(m => !string.IsNullOrEmpty(m)).Distinct());

                              lock (lockObj)
                              {
                                  foreach (var ico in icos)
                                  {
                                      if (vazbyNaPolitiky.SoukromeFirmy.ContainsKey(ico) || sponzorujiciFirmy.Any(m => m.IcoDarce == ico))
                                      {
                                          if (!uradySoukr.ContainsKey(objednatelIco))
                                          {
                                              uradySoukr.Add(objednatelIco, new Analysis.BasicDataForSubject<List<Analysis.BasicData<string>>>());
                                              uradySoukr[objednatelIco].Ico = objednatelIco;

                                          }
                                          uradySoukr[objednatelIco].Add(1, s.CalculatedPriceWithVATinCZK);
                                          if (!uradySoukr[objednatelIco].Detail.Any(m => m.Item == ico))
                                          {
                                              uradySoukr[objednatelIco].Detail.Add(new Analysis.BasicData<string>() { Item = ico, CelkemCena = s.CalculatedPriceWithVATinCZK, Pocet = 1 });
                                          }
                                          else
                                          {
                                              var item = uradySoukr[objednatelIco].Detail.First(m => m.Item == ico);
                                              item.Add(1,s.CalculatedPriceWithVATinCZK);

                                          }
                                      }
                                      else if (vazbyNaPolitiky.StatniFirmy.ContainsKey(ico))
                                      {
                                          if (!uradyStatni.ContainsKey(objednatelIco))
                                          {
                                              uradyStatni.Add(objednatelIco, new Analysis.BasicDataForSubject<List<Analysis.BasicData<string>>>());
                                              uradyStatni[objednatelIco].Ico = objednatelIco;

                                          }
                                          uradyStatni[objednatelIco].Add(1, s.CalculatedPriceWithVATinCZK);
                                          if (!uradyStatni[objednatelIco].Detail.Any(m => m.Item == ico))
                                          {
                                              uradyStatni[objednatelIco].Detail.Add(new Analysis.BasicData<string>() { Item = ico, CelkemCena = s.CalculatedPriceWithVATinCZK, Pocet = 1 });
                                          }
                                          else
                                          {
                                              var item = uradyStatni[objednatelIco].Detail.First(m => m.Item == ico);
                                              item.Add(1,s.CalculatedPriceWithVATinCZK);

                                          }
                                      }
                                  }

                              }
                          }
                      }
                      catch (Exception e)
                      {
                          HlidacStatu.Util.Consts.Logger.Error("ERROR UradyObchodujiciSFirmami_s_vazbouNaPolitiky", e);
                      }

                      end:
                      return new Devmasters.Batch.ActionOutputData() { CancelRunning = false, Log = null };
                  }, null,
                    showProgress ? Devmasters.Batch.Manager.DefaultOutputWriter : (Action<string>)null,
                    showProgress ? new Devmasters.Batch.ActionProgressWriter().Write : (Action<ActionProgressData>)null
                    ,true
                    , prefix: "UradyObchodujiciSFirmami_s_vazbouNaPolitiky " + aktualnost.ToNiceDisplayName()
            );


            AnalysisCalculation.VazbyFiremNaUradyStat ret = new VazbyFiremNaUradyStat();
            ret.SoukromeFirmy = uradySoukr
                .Where(m => m.Value.Pocet > 0)
                .Select(kv => kv.Value)
                .OrderByDescending(o => o.Pocet);

            return ret;
        }

        public static VazbyFiremNaPolitiky LoadFirmySVazbamiNaPolitiky(Relation.AktualnostType aktualnostVztahu, bool showProgress = false)
        {
            object lockObj = new object();
            Dictionary<string, List<int>> pol_SVazbami = new Dictionary<string, List<int>>();
            Dictionary<string, List<int>> pol_SVazbami_StatniFirmy = new Dictionary<string, List<int>>();

            Devmasters.Batch.Manager.DoActionForAll<Osoba>(StaticData.PolitickyAktivni.Get(),
            (p) =>
            {

                var vazby = p.AktualniVazby(aktualnostVztahu);
                if (vazby != null && vazby.Count() > 0)
                {

                    foreach (var v in vazby)
                    {
                        if (!string.IsNullOrEmpty(v.To.Id))
                        {
                            //check if it's GovType company
                            Firma f = Firmy.Get(v.To.Id);
                            //if (f == null)
                            //{
                            //    f = External.GovData.FromIco(v.To.Id);
                            //    if (f == null)
                            //        continue; //unknown company, skip
                            //}
                            if (!Firma.IsValid(f))
                                continue; //unknown company, skip
                            lock (lockObj)
                            {
                                if (f.PatrimStatu())
                                {
                                    //Gov Company
                                    if (pol_SVazbami_StatniFirmy.ContainsKey(v.To.Id))
                                    {
                                        var pol = pol_SVazbami_StatniFirmy[v.To.Id];
                                        if (!pol.Any(m => m == p.InternalId))
                                            pol.Add(p.InternalId);
                                    }
                                    else
                                    {
                                        pol_SVazbami_StatniFirmy.Add(v.To.Id, new List<int>());
                                        pol_SVazbami_StatniFirmy[v.To.Id].Add(p.InternalId);
                                    }


                                }
                                else
                                {
                                    //private company
                                    if (pol_SVazbami.ContainsKey(v.To.Id))
                                    {
                                        var pol = pol_SVazbami[v.To.Id];
                                        if (!pol.Any(m => m == p.InternalId))
                                            pol.Add(p.InternalId);
                                    }
                                    else
                                    {
                                        pol_SVazbami.Add(v.To.Id, new List<int>());
                                        pol_SVazbami[v.To.Id].Add(p.InternalId);
                                    }

                                }
                            } //lock

                        }

                    }

                }
                return new Devmasters.Batch.ActionOutputData() { CancelRunning = false, Log = null };
            },
            showProgress ? Devmasters.Batch.Manager.DefaultOutputWriter : (Action<string>)null,
            showProgress ? new Devmasters.Batch.ActionProgressWriter().Write : (Action<ActionProgressData>)null,
            true
            , prefix: "LoadFirmySVazbamiNaPolitiky " + aktualnostVztahu.ToNiceDisplayName()
            );

            return new VazbyFiremNaPolitiky() { SoukromeFirmy = pol_SVazbami, StatniFirmy = pol_SVazbami_StatniFirmy };
        }


        public static string[] SmlouvyIdSPolitiky(Dictionary<string, List<int>> politicisVazbami, List<OsobaEvent> sponzorujiciFirmy, bool showProgress = false)
        {
            HashSet<string> allIco = new HashSet<string>(
                politicisVazbami.Select(m => m.Key).Union(sponzorujiciFirmy.Select(m => m.Ico)).Distinct()
            );
            //smlouvy s politikama
            Func<int, int, Nest.ISearchResponse<Lib.Data.Smlouva>> searchFunc = null;
            searchFunc = (size, page) =>
            {
                return Lib.ES.Manager.GetESClient().Search<Lib.Data.Smlouva>(a => a
                            .TrackTotalHits(page * size == 0)
                            .Size(size)
                            .From(page * size)
                            .Source(m => m.Excludes(e => e.Field(o => o.Prilohy)))
                            .Query(q => q.MatchAll())
                            .Scroll("1m")
                            );
            };


            List<string> smlouvyIds = new List<string>();
            Searching.Tools.DoActionForQuery<Lib.Data.Smlouva>(Lib.ES.Manager.GetESClient(), searchFunc,
                  (hit, param) =>
                  {

                      Lib.Data.Smlouva s = hit.Source;
                      if (s.platnyZaznam)
                      {
                          if (!string.IsNullOrEmpty(s.Platce.ico) && allIco.Contains(s.Platce.ico))
                          {
                              smlouvyIds.Add(s.Id);
                          }
                          else
                              foreach (var ss in s.Prijemce)
                              {

                                  if (!string.IsNullOrEmpty(ss.ico) && allIco.Contains(ss.ico))
                                  {
                                      smlouvyIds.Add(s.Id);
                                      break;
                                  }

                              }
                      }
                      return new Devmasters.Batch.ActionOutputData() { CancelRunning = false, Log = null };
                  }, null,
                    showProgress ? Devmasters.Batch.Manager.DefaultOutputWriter : (Action<string>)null,
                    showProgress ? new Devmasters.Batch.ActionProgressWriter().Write : (Action<ActionProgressData>)null
                , false
                , prefix: "SmlouvyIdSPolitiky "
            );

            return smlouvyIds.ToArray();
        }

        public static IEnumerable<IcoSmlouvaMinMax> GetFirmyCasovePodezreleZalozene(Action<string> logOutputFunc = null, Action<ActionProgressData> progressOutputFunc = null)
        {
            HlidacStatu.Util.Consts.Logger.Debug("GetFirmyCasovePodezreleZalozene - getting all ico");
            var allIcos = Lib.Data.External.FirmyDB.AllIcoInRS();
            Dictionary<string, AnalysisCalculation.IcoSmlouvaMinMax> firmy = new Dictionary<string, AnalysisCalculation.IcoSmlouvaMinMax>();
            object lockFirmy = new object();
            var client = HlidacStatu.Lib.ES.Manager.GetESClient();
            AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva> aggs = new AggregationContainerDescriptor<HlidacStatu.Lib.Data.Smlouva>()
                .Min("minDate", m => m
                    .Field(f => f.datumUzavreni)
                );


            HlidacStatu.Util.Consts.Logger.Debug("GetFirmyCasovePodezreleZalozene - getting first smlouva for all ico from ES");
            Devmasters.Batch.Manager.DoActionForAll<string, object>(allIcos,
            (ico, param) =>
            {
                Firma ff = Firmy.Get(ico);
                if (Firma.IsValid(ff))
                {
                    if (ff.PatrimStatu()) //statni firmy tam nechci
                    {
                        return new Devmasters.Batch.ActionOutputData() { CancelRunning = false, Log = null };
                    }
                    else
                    {

                        var res = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch("ico:" + ico, 0, 0, HlidacStatu.Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, aggs, exactNumOfResults: true);
                        if (res.ElasticResults.Aggregations.Count > 0)
                        {
                            var epoch = ((Nest.ValueAggregate)res.ElasticResults.Aggregations.First().Value).Value;
                            if (epoch.HasValue)
                            {
                                var mindate = Devmasters.DT.Util.FromEpochTimeToUTC((long)epoch / 1000);

                                lock (lockFirmy)
                                {
                                    if (firmy.ContainsKey(ico))
                                    {
                                        if (firmy[ico].minUzavreni.HasValue == false)
                                            firmy[ico].minUzavreni = mindate;
                                        else if (firmy[ico].minUzavreni.Value > mindate)
                                            firmy[ico].minUzavreni = mindate;
                                    }
                                    else
                                    {
                                        firmy.Add(ico, new AnalysisCalculation.IcoSmlouvaMinMax()
                                        {
                                            ico = ico,
                                            minUzavreni = Devmasters.DT.Util.FromEpochTimeToUTC((long)epoch / 1000)
                                        });
                                    }
                                    if (ff.Datum_Zapisu_OR.HasValue)
                                    {
                                        firmy[ico].vznikIco = ff.Datum_Zapisu_OR.Value;
                                        firmy[ico].jmeno = ff.Jmeno;
                                    }

                                }


                            }
                        }
                    }
                }
                return new Devmasters.Batch.ActionOutputData() { CancelRunning = false, Log = null };
            },
            null,
            logOutputFunc ?? Devmasters.Batch.Manager.DefaultOutputWriter,
            progressOutputFunc ?? new Devmasters.Batch.ActionProgressWriter(0.1f).Write,
            true
            );


            //List<string> privateCompanyIcos = new List<string>();
            ////filter statni firmy && add vznik

            //Devmasters.Batch.Manager.DoActionForAll<string, object>(firmy.Keys,
            //(ico, param) =>
            //{
            //    Firma ff = Firmy.Get(ico);
            //    if (Firma.IsValid(ff))
            //    {
            //        if (ff.PatrimStatu()) //statni firmy tam nechci
            //        {
            //            return new Devmasters.Batch.ActionOutputData() { CancelRunning = false, Log = null };
            //        }
            //        else
            //        {
            //            if (ff.Datum_Zapisu_OR.HasValue)
            //            {
            //                firmy[ico].vznikIco = ff.Datum_Zapisu_OR.Value;
            //                firmy[ico].jmeno = ff.Jmeno;
            //                privateCompanyIcos.Add(ico);
            //            }
            //        }
            //    }

            //    return new Devmasters.Batch.ActionOutputData() { CancelRunning = false, Log = null };
            //},
            //null,
            //Devmasters.Batch.Manager.DefaultOutputWriter,
            //new Devmasters.Batch.ActionProgressWriter(1f).Write,
            //true, maxDegreeOfParallelism: 5
            //);

            HlidacStatu.Util.Consts.Logger.Debug("GetFirmyCasovePodezreleZalozene - filter with close dates");

            DateTime minDate = new DateTime(1990, 01, 01);
            var badF = firmy
                .Select(m => m.Value)
                .Where(f => f.minUzavreni > minDate)
                .Where(f => f.days.HasValue && f.days.Value < 60)
                .OrderBy(f => f.days.Value)
                .ToArray();
            //.Take(100)

            HlidacStatu.Util.Consts.Logger.Debug($"GetFirmyCasovePodezreleZalozene - returning {badF.Count()} records." );

            return badF;
        }

        public static IEnumerable<(string idDotace, string ico, int ageInDays )> CompanyAgeDuringSubsidy()
        {
            var dotSer = new Dotace.DotaceService();

            foreach (var dotace in dotSer.YieldAllDotace())
            {
                bool missingEssentialData = string.IsNullOrWhiteSpace(dotace.Prijemce?.Ico)
                    || !dotace.DatumPodpisu.HasValue;

                if (missingEssentialData)
                    continue;

                Firma firma = Firmy.Get(dotace.Prijemce.Ico);

                if (firma.PatrimStatu()) //nechceme státní firmy
                    continue;

                if (!firma.Datum_Zapisu_OR.HasValue) //nechceme firmy s chybějící hodnotou data zapisu do OR
                    continue;

                var companyAgeInDays = (dotace.DatumPodpisu.Value - firma.Datum_Zapisu_OR.Value).Days;

                yield return (idDotace: dotace.IdDotace, ico: firma.ICO, ageInDays: companyAgeInDays);
            }
        }
    }
}
