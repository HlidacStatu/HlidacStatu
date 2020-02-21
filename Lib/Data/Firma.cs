using Devmasters.Core;
using HlidacStatu.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HlidacStatu.Lib.Data
{
    public partial class Firma
        : Bookmark.IBookmarkable, ISocialInfo
    {
        public class ZiskyZtraty
        {
            public int Rok { get; set; }
            public string Ico { get; set; }
            public double Vynos { get; set; }
            public double Naklad { get; set; }
        }

        public static bool IsValid(Firma f)
        {

            if (f == null)
                return false;
            else
                return f.Valid;
        }

        public static Firma NotFound = new Firma() { ICO = "notfound", Valid = false };
        public static Firma LoadError = new Firma() { ICO = "error", Valid = false };


        static string cnnStr = Devmasters.Core.Util.Config.GetConfigValue("CnnString");


        public static string[] StatniFirmyICO = new string[] { "45274649", "25702556", "70994226", "47114983", "42196451", "60193531", "00002739", "00514152", "00000493", "00001279", "24821993", "00000205", "60193468", "00000515", "49710371", "70890013", "70889953", "70890005", "25291581", "70889988", "70890021", "00007536", "26463318", "00024007", "25401726", "00015679", "00010669", "49241494", "14450216", "00001481", "00001490", "00002674", "43833560", "48204285", "00013251", "00014125", "27146235", "49973720", "00311391", "25125877", "00013455", "60197901", "60196696", "00251976", "62413376", "00577880", "44848943", "63078333", "45279314", "13695673", "27772683", "45273448", "28196678", "27786331", "61459445", "27364976", "24829871", "27257258", "17047234", "27378225", "27892646", "27195872", "45795908", "28244532", "61860336", "27145573", "25674285", "25085531", "27232433", "24729035", "27257517", "49901982", "27309941", "28786009", "47115726", "26871823", "26470411", "26206803", "28255933", "28707052", "26376547", "60698101", "27804721", "26840065", "25938924", "00128201", "26051818", "28861736" };

        public static int[] StatniFirmy_BasedKodPF = new int[] {
            301,302,312,313,314,325,331,352,353,361,362,381,382,521,771,801,804,805
        };

        /*
301	Státní podnik
302	Národní podnik
312	Banka-státní peněžní ústav
313	Česká národní banka
314	Česká konsolidační agentura
325	Organizační složka státu
331	Příspěvková organizace
352	Správa železniční dopravní cesty, státní organizace
353	Rada pro veřejný dohled nad auditem
361	Veřejnoprávní instituce (ČT,ČRo,ČTK)
362	Česká tisková kancelář
381	Fond (ze zákona)
382	Státní fond ze zákona
 
521	Samostatná drobná provozovna obecního úřadu
771	Svazek obcí
801	Obec nebo městská část hlavního města Prahy
804	Kraj
805	Regionální rada regionu soudržnosti    

 */


        public string ICO { get; set; }
        public string DIC { get; set; }
        public string[] DatovaSchranka { get; set; } = new string[] { };
        public DateTime? Datum_Zapisu_OR { get; set; }
        public int? Stav_subjektu { get; set; }
        public int? Status { get; set; }
        public string StatusFull(bool shortText = false)
        {
            switch (this.Status)
            {
                case 1:
                    return shortText ? "" : "";
                //Subjekt bez omezení v činnosti
                case 2:
                    return shortText ? "Subjekt v likvidaci" : "v likvidaci";
                case 3:
                    return shortText ? "Subjekt v insolvenčním řízení" : "v insolvenci";
                case 4:
                    return shortText ? "Subjekt v likvidaci a v insolvenčním řízení" : "v likvidaci";
                case 5:
                    return shortText ? "Subjekt v nucené správě" : "v nucené správě";
                case 6:
                    return shortText ? "Zaniklý subjekt" : "zaniklý subjekt";
                case 7:
                    return shortText ? "Subjekt s pozastavenou, přerušenou činností" : "pozastavená činností";
                case 8:
                    return shortText ? "Dosud nezahájil činnost" : "nezahájená činnost";

                default:
                    return "";
                    break;
            }

        }

        public string KrajId { get; set; }
        public string OkresId { get; set; }

        public short? IsInRS { get; set; }


        private string _jmeno = string.Empty;
        public string Jmeno
        {
            get { return _jmeno; }
            set
            {
                this._jmeno = value;
                this.JmenoAscii = Devmasters.Core.TextUtil.RemoveDiacritics(value);
            }
        }
        public string JmenoAscii { get; set; }
        public int? Kod_PF { get; set; }
        public string[] NACE { get; set; } = new string[] { };
        public int VersionUpdate { get; set; }

        public string Source { get; set; }
        public string Popis { get; set; }

        void updateVazby(bool refresh = false)
        {
            try
            {
                _vazby = Lib.Data.Graph.VsechnyDcerineVazby(this.ICO, refresh)
                    .ToArray();

            }
            catch (Exception)
            {
                _vazby = new Graph.Edge[] { };
            }
        }
        Graph.Edge[] _vazby = null;
        public Graph.Edge[] Vazby(bool refresh = false)
        {
            if (_vazby == null)
            {
                updateVazby(refresh);
            }
            return _vazby;
        }
        public void Vazby(IEnumerable<Graph.Edge> value)
        {
            _vazby = value.ToArray();
        }

        public bool MaVztahySeStatem()
        {
            var ret = this.IsSponzor();
            if (ret) return ret;

            ret = this.Statistic().ToBasicData().Pocet> 0;
            if (ret) return ret;

            ret = HlidacStatu.Lib.Data.VZ.VerejnaZakazka.Searching.SimpleSearch("ico:" + this.ICO, null, 1, 1, 0).Total > 0;
            if (ret) return ret;

            ret = new HlidacStatu.Lib.Data.Dotace.DotaceService().SimpleSearch("ico:" + this.ICO, 1, 1, 0).Total > 0;
            return ret;

        }

        public Analysis.BasicDataForSubject<List<Analysis.BasicData<string>>> NespolehlivyPlatceDPH_obchodSuradyStat()
        {
            return Lib.StaticData.NespolehlivyPlatciDPH_obchodySurady_Cache.Get().SoukromeFirmy
                .FirstOrDefault(f => f.Ico == this.ICO);
        }

        public bool IsNespolehlivyPlatceDPH()
        {
            return NespolehlivyPlatceDPH() != null;
        }

        public Data.NespolehlivyPlatceDPH NespolehlivyPlatceDPH()
        {
            if (Lib.StaticData.NespolehlivyPlatciDPH.Get().ContainsKey(this.ICO))
                return Lib.StaticData.NespolehlivyPlatciDPH.Get()[this.ICO];
            else
                return null;
        }

        public Graph.Edge[] AktualniVazby(Relation.AktualnostType minAktualnost)
        {
            return Relation.AktualniVazby(this.Vazby(), minAktualnost);
        }

        public Graph.Edge[] VazbyProICO(string ico)
        {
            List<Graph.Edge> ret = new List<Graph.Edge>();
            if (Vazby() == null)
                return ret.ToArray();
            if (Vazby().Count() == 0)
                return ret.ToArray();
            return Vazby().Where(m => m.To.Id == ico).ToArray();
        }



        public void UpdateVazbyFromDB()
        {

            List<Graph.Edge> oldRel = new List<Graph.Edge>();
            if (this.Vazby() != null)
            {
                //oldRel = this.GetVazby().Where(m => m.Permanent).ToList();
            }


            var firstRel = Graph.VsechnyDcerineVazby(this.ICO,true);


            this.Vazby(Graph.Edge.Merge(oldRel, firstRel).ToArray());

        }

        bool? _valid = null;
        public bool Valid
        {
            get
            {
                if (_valid == null)
                    _valid = !(this.Jmeno == NotFound.Jmeno
                         || this.Jmeno == LoadError.Jmeno);

                return _valid.Value;
            }

            private set
            {
                _valid = value;
            }

        }

        public string SocialInfoTitle()
        {
            return Devmasters.Core.TextUtil.ShortenText(this.Jmeno, 50); }

        public string SocialInfoSubTitle()
        {
            return this.JsemOVM() ? "Úřad" : (this.JsemStatniFirma() ? "Firma (spolu)vlastněná státem" : "Soukromá firma");
        }

        public string SocialInfoBody()
        {
            return "<ul>" +
            HlidacStatu.Util.InfoFact.RenderInfoFacts(this.InfoFacts(), 4, true, true, "", "<li>{0}</li>", true)
            + "</ul>";
        }

        public string SocialInfoFooter()
        {
            return "Údaje k " + DateTime.Now.ToString("d. M. yyyy");
        }

        public string SocialInfoImageUrl()
        {
            return string.Empty;
        }

        public void Save()
        {

            this.JmenoAscii = Devmasters.Core.TextUtil.RemoveDiacritics(this.Jmeno);

            string sql = @"exec Firma_Save @ICO,@DIC,@Datum_zapisu_OR,@Stav_subjektu,@Jmeno,@Jmenoascii,@Kod_PF,@Source, @Popis, @VersionUpdate, @krajId, @okresId, @status  ";
            string sqlNACE = @"INSERT into firma_NACE(ico, nace) values(@ico,@nace)";
            string sqlDS = @"INSERT into firma_DS(ico, DatovaSchranka) values(@ico,@DatovaSchranka)";

            string cnnStr = Devmasters.Core.Util.Config.GetConfigValue("CnnString");
            try
            {

                using (PersistLib p = new PersistLib())
                {
                    p.ExecuteNonQuery(cnnStr, System.Data.CommandType.Text, sql, new IDataParameter[] {
                        new System.Data.SqlClient.SqlParameter("ico", this.ICO),
                        new System.Data.SqlClient.SqlParameter("dic", this.DIC),
                        new System.Data.SqlClient.SqlParameter("Datum_zapisu_OR", this.Datum_Zapisu_OR),
                        new System.Data.SqlClient.SqlParameter("Stav_subjektu", this.Stav_subjektu),
                        new System.Data.SqlClient.SqlParameter("Jmeno", this.Jmeno),
                        new System.Data.SqlClient.SqlParameter("Jmenoascii", this.JmenoAscii),
                        new System.Data.SqlClient.SqlParameter("Kod_PF", this.Kod_PF),
                        new System.Data.SqlClient.SqlParameter("Source", this.Source),
                        new System.Data.SqlClient.SqlParameter("Popis", this.Popis),
                        new System.Data.SqlClient.SqlParameter("VersionUpdate", this.VersionUpdate),
                        new System.Data.SqlClient.SqlParameter("KrajId", this.KrajId),
                        new System.Data.SqlClient.SqlParameter("OkresId", this.OkresId),
                        new System.Data.SqlClient.SqlParameter("Status", this.Status),
                        });


                    if (this.DatovaSchranka != null)
                    {
                        p.ExecuteNonQuery(cnnStr, System.Data.CommandType.Text, "delete from firma_DS where ico=@ico", new IDataParameter[] {
                            new System.Data.SqlClient.SqlParameter("ico", this.ICO) });
                        foreach (var ds in this.DatovaSchranka.Distinct())
                        {
                            p.ExecuteNonQuery(cnnStr, System.Data.CommandType.Text, sqlDS, new IDataParameter[] {
                            new System.Data.SqlClient.SqlParameter("ico", this.ICO),
                            new System.Data.SqlClient.SqlParameter("DatovaSchranka", ds),
                        });
                        }
                    }

                    if (this.NACE != null)
                    {
                        p.ExecuteNonQuery(cnnStr, System.Data.CommandType.Text, "delete from firma_NACE where ico=@ico", new IDataParameter[] {
                            new System.Data.SqlClient.SqlParameter("ico", this.ICO) });
                        foreach (var nace in this.NACE.Distinct())
                        {
                            p.ExecuteNonQuery(cnnStr, System.Data.CommandType.Text, sqlNACE, new IDataParameter[] {
                            new System.Data.SqlClient.SqlParameter("ico", this.ICO),
                            new System.Data.SqlClient.SqlParameter("nace", nace),
                        });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                //throw;
            }


        }
        public void RefreshDS()
        {
            this.DatovaSchranka = External.DatoveSchranky.ISDS.GetDatoveSchrankyForIco(this.ICO);
        }


        Analysis.SubjectStatistic _statistic = null;
        public Analysis.SubjectStatistic Statistic()
        {
                if (_statistic == null)
                {
                    _statistic = new Analysis.SubjectStatistic(this);

                }
            return _statistic;
        }

        public IEnumerable<string> IcosInHolding(Data.Relation.AktualnostType aktualnost)
        {
            return this.AktualniVazby(aktualnost)
                .Where(v => !string.IsNullOrEmpty(v.To?.UniqId)
                            && v.To.Type == HlidacStatu.Lib.Data.Graph.Node.NodeType.Company)
                .Select(v => v.To)
                .Distinct(new HlidacStatu.Lib.Data.Graph.NodeComparer())
                .Select(m => m.Id);
        }

        public Analysis.BasicDataPerYear StatisticForHolding(Data.Relation.AktualnostType aktualnost)
        {
            Analysis.BasicDataPerYear myStat = Analysis.ACore.GetBasicStatisticForICO(this.ICO);

            Dictionary<string, Analysis.BasicDataPerYear> PerIcoStat =
                IcosInHolding(aktualnost)
                .Select(ico => new { ico = ico, ss = Analysis.ACore.GetBasicStatisticForICO(ico) })
                .ToDictionary(k => k.ico, v => v.ss);

            foreach (var kv in PerIcoStat)
                myStat.Add(kv.Value);

            return myStat;
        }


        public Analysis.RatingDataPerYear RatingPerYearForHolding(Data.Relation.AktualnostType aktualnost)
        {
            Analysis.RatingDataPerYear myStat = Analysis.ACore.GetRatingForICO(this.ICO);

            Dictionary<string, Analysis.RatingDataPerYear> PerIcoStat =
                IcosInHolding(aktualnost)
                .Select(ico => new { ico = ico, ss = Analysis.ACore.GetRatingForICO(ico) })
                .ToDictionary(k => k.ico, v => v.ss);

            foreach (var icodata in PerIcoStat)
            {
                foreach (var kv in icodata.Value.Data)
                {
                    if (myStat.Data.Keys.Contains(kv.Key))
                    {
                        myStat.Data[kv.Key].NumBezCeny += kv.Value.NumBezCeny;
                        myStat.Data[kv.Key].NumBezSmluvniStrany += kv.Value.NumBezSmluvniStrany;
                        myStat.Data[kv.Key].NumSPolitiky += kv.Value.NumSPolitiky;
                        myStat.Data[kv.Key].SumKcBezSmluvniStrany += kv.Value.SumKcBezSmluvniStrany;
                        myStat.Data[kv.Key].SumKcSPolitiky += kv.Value.SumKcSPolitiky;

                    }
                    else
                        myStat.Data.Add(kv.Key, kv.Value);
                }
            }

            return new Analysis.RatingDataPerYear(new Analysis.RatingDataPerYear[] { myStat }, StatisticForHolding(aktualnost));
        }

        public bool JsemSoukromaFirma()
        {
            return JsemOVM() == false && JsemStatniFirma() == false;
        }


        public bool PatrimStatu()
        {
            return JsemOVM() || JsemStatniFirma();
        }
        public bool JsemOVM()
        {
            return StaticData.Urady_OVM.Contains(this.ICO);
        }
        public bool JsemStatniFirma()
        {

            if (
                (this.Kod_PF != null && Firma.StatniFirmy_BasedKodPF.Contains(this.Kod_PF.Value))
                || (StaticData.VsechnyStatniMestskeFirmy.Contains(this.ICO))
                )
            {
                return true;
            }
            else
                return false;

        }
        public bool IsSponzor()
        {
            return this.Events(m =>
                m.Type == (int)FirmaEvent.Types.Sponzor
            ).Any();
        }
        public IEnumerable<FirmaEvent> Events()
        {
            return Events(m => true);
        }
        public IEnumerable<FirmaEvent> Events(Expression<Func<FirmaEvent, bool>> predicate)
        {
            using (DbEntities db = new DbEntities())
            {
                return db.FirmaEvent
                    .AsNoTracking()
                    .Where(predicate)
                    .Where(m => m.ICO == this.ICO)
                    .ToArray();
            }
        }

        public string JmenoBezKoncovky()
        {
            return Firma.JmenoBezKoncovky(this.Jmeno);
        }

        

        public string KoncovkaFirmy()
        {

            string koncovka;
            Firma.JmenoBezKoncovkyFull(this.Jmeno, out koncovka);
            return koncovka;

        }

        public FirmaEvent AddSponsoring(string strana, int rok, decimal castka, string zdroj, string user, bool rewrite = false, bool checkDuplicates = true)
        {
            strana = ParseTools.NormalizaceStranaShortName(strana);
            var t = FirmaEvent.Types.Sponzor;
            if (zdroj?.Contains("https://www.hlidacstatu.cz/ucty/transakce/") == true)
                t = FirmaEvent.Types.SponzorZuctu;

            FirmaEvent oe = new FirmaEvent(this.ICO, string.Format("Sponzor {0}", strana), "", t);
            oe.AddInfoNum = castka;
            oe.Zdroj = zdroj;
            oe.SetYearInterval(rok);
            oe.AddInfo = strana;
            return AddOrUpdateEvent(oe, user, checkDuplicates: false);

        }

        public FirmaEvent AddOrUpdateEvent(FirmaEvent ev, string user, bool rewrite = false, bool checkDuplicates = true)
        {
            if (ev == null)
                return null;

            //if (ev.Type == (int)OsobaEvent.Types.Sponzor)
            //{
            //    return AddSponsoring(ev.AddInfo, ev.DatumOd.Value.Year, ev.AddInfoNum.Value, ev.Zdroj, user);
            //}

            //check duplicates
            using (DbEntities db = new Data.DbEntities())
            {
                FirmaEvent exists = null;
                if (ev.pk > 0 && checkDuplicates)
                    exists = db.FirmaEvent
                                .AsNoTracking()
                                .FirstOrDefault(m =>
                                    m.pk == ev.pk
                                );
                else if (checkDuplicates)
                {
                    exists = db.FirmaEvent
                                .AsNoTracking()
                                .FirstOrDefault(m =>
                                        m.ICO == this.ICO
                                        && m.Type == ev.Type
                                        && m.AddInfo == ev.AddInfo
                                        && m.AddInfoNum == ev.AddInfoNum
                                        && m.DatumOd == ev.DatumOd
                                        && m.DatumDo == ev.DatumDo
                                        && m.Zdroj == ev.Zdroj
                                );
                    if (exists != null)
                        ev.pk = exists.pk;
                }
                if (exists != null && rewrite)
                {
                    db.FirmaEvent.Remove(exists);
                    Audit.Add<FirmaEvent>(Audit.Operations.Delete, user, exists, null);
                    db.SaveChanges();
                    exists = null;
                }

                if (exists == null)
                {
                    ev.ICO = this.ICO;
                    db.FirmaEvent.Add(ev);
                    Audit.Add(Audit.Operations.Create, user, ev, null);
                    db.SaveChanges();
                    return ev;
                }
                else
                {
                    ev.ICO = this.ICO;
                    ev.pk = exists.pk;
                    db.FirmaEvent.Attach(ev);
                    db.Entry(ev).State = System.Data.Entity.EntityState.Modified;
                    Audit.Add(Audit.Operations.Create, user, ev, exists);
                    db.SaveChanges();
                    return ev;
                }
            }
        }

        public static string[] Koncovky = new string[] {
            "a.s.",
            "a. s.",
            "akciová společnost",
            "akc. společnost",
            "s.r.o.","s r.o.","s. r. o.",
            "spol. s r.o.","spol.s r.o.","spol.s.r.o.","spol. s r. o.",
            "v.o.s.","v. o. s.",
            "veřejná obchodní společnost",
            "s.p.","s. p.",
            "státní podnik",
            "odštepný závod",
            "o.z.","o. z.",
            "o.s.","o. s.",
            "z.s.","z. s.",
            "z.ú.","z. ú.",

        };

        public static Firma FromDS(string ds, bool getMissingFromExternal = false)
        {
            Firma f = External.FirmyDB.FromDS(ds);
            if (!Firma.IsValid(f) && getMissingFromExternal)
            {
                var d = External.DatoveSchranky.ISDS.GetSubjektyForDS(ds);
                if (d != null)
                {
                    return Firma.FromIco(d.ICO, getMissingFromExternal);
                }
            }

            return f;

        }
        public static Firma FromName(string jmeno, bool getMissingFromExternal = false)
        {
            Firma f = External.FirmyDB.FromName(jmeno);
            if (f != null)
                return f;

            //f = External.OR_Elastic.FromName(jmeno);
            //if (Firma.IsValid(f))
            //    return f;

            else if (getMissingFromExternal)
            {
                f = External.Merk.FromName(jmeno);
                if (Firma.IsValid(f))
                    return f;

                //f = Lib.Data.External.Firmo.GetFirmaFromName(jmeno);
                //if (!Firma.IsValid(f)) //try GovData
                //{
                //    //f = Lib.Data.External.GovData.FromIco("");
                //}

                if (f == null)
                    return Firma.NotFound;
                else if (f == Firma.NotFound || f == Firma.LoadError)
                    return f;

                f.RefreshDS();
                f.Save();
                return f;
            }
            else
            {
                return null;
            }

        }


        public static Firma FromIco(int ico, bool getMissingFromExternal = false)
        {
            return FromIco(ico.ToString().PadLeft(8, '0'), getMissingFromExternal);
        }
        public static Firma FromIco(string ico, bool getMissingFromExternal = false)
        {
            Firma f = External.FirmyDB.FromIco(ico);

            if (Firma.IsValid(f))
                return f;


            //f = External.OR_Elastic.FromIco(ico);
            //if (Firma.IsValid(f))
            //    return f;

            else if (getMissingFromExternal)
            {
                f = External.Merk.FromIco(ico);
                if (Firma.IsValid(f))
                    return f;

                //f = External.Firmo.GetFirmaFromIco(ico);
                //if (Firma.IsValid(f))
                //    return f;
                //f = Lib.Data.External.GovData.FromIco(ico);

                if (!Firma.IsValid(f)) //try firmo
                {
                    f = External.RZP.FromIco(ico);
                }
                //if (!Firma.IsValid(f)) //try firmo
                //{
                //    f = Lib.Data.External.Firmo.GetFirmaFromIco(ico);
                //}
                if (f == null)
                    return Firma.NotFound;
                else if (f == Firma.NotFound || f == Firma.LoadError)
                    return f;

                f.RefreshDS();
                f.Save();
                return f;
            }
            else
            {
                return Firma.NotFound;
            }
        }
        public static Firma FromTransactionId(Data.TransparentniUcty.BankovniPolozka transaction)
        {
            using (DbEntities db = new Data.DbEntities())
            {
                string url = transaction.GetUrl(false);
                var found = db.FirmaEvent
                            .Where(m => m.Zdroj == url)
                            .FirstOrDefault();
                if (found != null)
                {
                    return FromIco(found.ICO);
                }

            }
            return null;
        }



        public string Description(bool html, string template = "{0}", string itemTemplate = "{0}", string itemDelimeter = "<br/>", int numOfRecords = int.MaxValue)
        {
            return Description(html, m => true, template, itemTemplate, itemDelimeter, numOfRecords);
        }
        public string Description(bool html, Expression<Func<FirmaEvent, bool>> predicate, string template = "{0}", string itemTemplate = "{0}", string itemDelimeter = "<br/>",int numOfRecords = int.MaxValue)
        {
            StringBuilder sb = new StringBuilder();
            var events = this.Events(predicate);
            if (events.Count() == 0)
                return string.Empty;
            else
            {
                List<string> evs = events
                    .OrderBy(e => e.DatumOd)
                    .Select(e => html ? e.RenderHtml(", ") : e.RenderText(" "))
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Take(numOfRecords)
                    .ToList();

                if (html && evs.Count > 0)
                {
                    return string.Format(template,
                         evs.Aggregate((f, s) => f + itemDelimeter + s)
                        );
                }
                else if (evs.Count > 0)
                {
                    return string.Format(template,
                        evs.Aggregate((f, s) => f + "\n" + s)
                        );
                }
                else return string.Empty;
            }


        }

        public static string JmenoBezKoncovky(string name)
        {
            string ret;
            return JmenoBezKoncovkyFull(name, out ret);
        }
        public static string JmenoBezKoncovkyFull(string name, out string koncovka)
        {
            koncovka = string.Empty;
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            string modifNazev = name.Trim().Replace((char)160, ' ');
            foreach (var k in Lib.Data.Firma.Koncovky.OrderByDescending(m => m.Length))
            {
                if (modifNazev.EndsWith(k))
                {
                    modifNazev = modifNazev.Replace(k, "").Trim();
                    koncovka = k;
                    break;
                }
                if (k.EndsWith("."))
                {
                    if (modifNazev.EndsWith(k.Substring(0, k.Length - 1)))
                    {
                        modifNazev = modifNazev.Replace(k.Substring(0, k.Length - 1), "").Trim();
                        koncovka = k;
                        break;
                    }

                }

            }
            if (modifNazev.EndsWith(",") || modifNazev.EndsWith(",") || modifNazev.EndsWith(";"))
                modifNazev = modifNazev.Substring(0, modifNazev.Length - 1);

            return modifNazev.Trim();

        }

        const string uradName = "Úřad";
        const string statniName = "Státní firma";
        const string firmaName = "Firma";
        public string ObecneJmeno()
        {
            return JsemOVM() ? uradName : (JsemStatniFirma() ? statniName : firmaName);
        }
        public bool ObecneJmenoRodMuzsky()
        {
            return ObecneJmeno() == uradName;
        }

        InfoFact[] _infofacts = null;
        object lockInfoObj = new object();
        public InfoFact[] InfoFacts()
        {
            var sName = ObecneJmeno();
            bool sMuzsky = sName == uradName;


            lock (lockInfoObj)
            {
                if (_infofacts == null)
                {
                    List<InfoFact> f = new List<InfoFact>();
                    var stat = new HlidacStatu.Lib.Analysis.SubjectStatistic(this);
                    int rok = DateTime.Now.Year;
                    if (DateTime.Now.Month < 2)
                        rok = rok - 1;
                    if (stat.BasicStatPerYear.SummaryAfter2016().Pocet == 0)
                    {
                        f.Add(new InfoFact($"{sName} nemá žádné smluvní vztahy evidované v&nbsp;registru smluv. ", InfoFact.ImportanceLevel.Medium));
                        f.Add(new InfoFact($"{(sMuzsky ? "Byl založen" : "Byla založena")} <b>{this.Datum_Zapisu_OR?.ToString("d. M. yyyy")}</b>. ", InfoFact.ImportanceLevel.Medium));
                    }
                    else
                    {
                        f.Add(new InfoFact($"V roce <b>{rok}</b> uzavřel{(sMuzsky ? "" : "a")} {sName.ToLower()} " +
                            Devmasters.Core.Lang.Plural.Get((int)stat.BasicStatPerYear[rok].Pocet, "jednu smlouvu v&nbsp;registru smluv", "{0} smlouvy v&nbsp;registru smluv", "celkem {0} smluv v&nbsp;registru smluv")
                            + $" za <b>{HlidacStatu.Util.RenderData.ShortNicePrice(stat.BasicStatPerYear[rok].CelkemCena, html: true)}</b>. "
                            , InfoFact.ImportanceLevel.Summary)
                            );

                        f.Add(new InfoFact( $"Mezi lety <b>{rok - 1}-{rok - 2000}</b> "
                            + stat.BasicStatPerYear.YearChange(rok).RenderChangeWord(false, stat.BasicStatPerYear.YearChange(rok).CenaChangePerc,
                            "došlo k <b>poklesu zakázek o&nbsp;{0:P2}</b> v&nbsp;Kč . ", " nedošlo ke změně objemu zakázek. ", "došlo k <b>nárůstu zakázek o&nbsp;{0:P2}</b> v&nbsp;Kč. ")
                            , InfoFact.ImportanceLevel.Medium)
                            );

                        if (stat.RatingPerYear[rok].NumBezCeny > 0)
                        {
                            f.Add(new InfoFact(
                                $"V <b>{rok} utajil{(sMuzsky ? "" : "a")}</b> hodnotu kontraktu " +
                                Devmasters.Core.Lang.Plural.Get((int)stat.RatingPerYear[rok].NumBezCeny, "u&nbsp;jedné smlouvy", "u&nbsp;{0} smluv", "u&nbsp;{0} smluv")
                                + $", což je celkem <b>{stat.RatingPerYear[rok].PercentBezCeny.ToString("P2")}</b> ze všech. ",
                                 InfoFact.ImportanceLevel.Medium)
                                );
                        }
                        else if (stat.RatingPerYear[rok - 1].NumBezCeny > 0)
                        {
                            f.Add(new InfoFact (
                                $"V <b>{rok - 1} utajil{(sMuzsky ? "" : "a")}</b> hodnotu kontraktů " +
                                Devmasters.Core.Lang.Plural.Get((int)stat.RatingPerYear[rok - 1].NumBezCeny, "u&nbsp;jedné smlouvy", "u&nbsp;{0} smluv", "u&nbsp;{0} smluv")
                                + $", což je celkem <b>{stat.RatingPerYear[rok - 1].PercentBezCeny.ToString("P2")}</b> ze všech. "
                                , InfoFact.ImportanceLevel.Medium)
                                );
                        }

                        long numFatalIssue = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch($"ico:{this.ICO} AND chyby:zasadni", 0, 0, HlidacStatu.Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, exactNumOfResults: true).Total;
                        long numVazneIssue = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch($"ico:{this.ICO} AND chyby:vazne", 0, 0, HlidacStatu.Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, exactNumOfResults: true).Total;

                        if (numFatalIssue > 0)
                        {
                            f.Add(new InfoFact($@"Má v registru smluv
                                    <b>{Devmasters.Core.Lang.Plural.GetWithZero((int)numFatalIssue, "0 smluv", "jednu smlouvu obsahující", "{0} smlouvy obsahující", "{0:### ### ##0} smluv obsahujících ")}
                                        tak závažné nedostatky v rozporu se zákonem,
                                    </b>že jsou velmi pravděpodobně neplatné. ", InfoFact.ImportanceLevel.High));
                        }
                        if (numVazneIssue > 0)
                        {
                            f.Add(new InfoFact($@"Má v registru smluv
                                    <b>{Devmasters.Core.Lang.Plural.GetWithZero((int)numFatalIssue, "0 smluv", "jednu smlouvu obsahující", "{0} smlouvy obsahující", "{0:### ### ##0} smluv obsahujících ")}</b>
                                        vážné nedostatky. "
                                        , InfoFact.ImportanceLevel.Medium)
                                        );
                        }

                        DateTime datumOd = new DateTime(DateTime.Now.Year - 10, 1, 1);
                        if (PatrimStatu() == false
                            && IsSponzor()
                            && Events(m => m.Type == (int)FirmaEvent.Types.Sponzor && m.DatumOd >= datumOd).Count() > 0
                            )
                        {
                            var sponzoring = this.Events(m => m.Type == (int)FirmaEvent.Types.Sponzor && m.DatumOd >= datumOd);
                            string[] strany = sponzoring.Select(m => m.AddInfo).Distinct().ToArray();
                            DateTime[] roky = sponzoring.Select(m => m.DatumOd.Value).Distinct().OrderBy(y => y).ToArray();
                            decimal celkem = sponzoring.Sum(m => m.AddInfoNum) ?? 0;
                            decimal top = sponzoring.Max(m => m.AddInfoNum) ?? 0;

                            f.Add(new InfoFact($"{sName} "
                                + Devmasters.Core.Lang.Plural.Get(roky.Count(), "v roce " + roky[0].Year, $"mezi roky {roky.First().Year}-{roky.Last().Year - 2000}", $"mezi roky {roky.First().Year}-{roky.Last().Year - 2000}")
                                + $" sponzoroval{(sMuzsky ? "" : "a")} " +
                                Devmasters.Core.Lang.Plural.Get(strany.Length, strany[0], "{0} polit.strany", "{0} polit.stran")
                                + $" v&nbsp;celkové výši <b>{HlidacStatu.Util.RenderData.ShortNicePrice(celkem, html: true)}</b>. "
                                + $"Nejvyšší sponzorský dar byl ve výši {RenderData.ShortNicePrice(top, html: true)}. "
                                , InfoFact.ImportanceLevel.Medium)
                                );
                        }

                        if (PatrimStatu() == false
                            && StaticData.FirmySVazbamiNaPolitiky_nedavne_Cache.Get().SoukromeFirmy.ContainsKey(this.ICO)
                           )
                        {
                            var politici = StaticData.FirmySVazbamiNaPolitiky_nedavne_Cache.Get().SoukromeFirmy[this.ICO];
                            if (politici.Count > 0)
                            {
                                var sPolitici = Osoby.GetById.Get(politici[0]).FullNameWithYear();
                                if (politici.Count == 2)
                                {
                                    sPolitici = sPolitici + " a " + Osoby.GetById.Get(politici[1]).FullNameWithYear();
                                }
                                else if (politici.Count == 3)
                                {
                                    sPolitici = sPolitici
                                        + ", "
                                        + Osoby.GetById.Get(politici[1]).FullNameWithYear()
                                        + " a "
                                        + Osoby.GetById.Get(politici[2]).FullNameWithYear();

                                }
                                else if (politici.Count > 3)
                                {
                                    sPolitici = sPolitici
                                        + ", "
                                        + Osoby.GetById.Get(politici[1]).FullNameWithYear()
                                        + ", "
                                        + Osoby.GetById.Get(politici[2]).FullNameWithYear()
                                        + " a další";

                                }
                                f.Add(new InfoFact(
                                    $"Ve firmě se "
                                    + Devmasters.Core.Lang.Plural.Get(politici.Count()
                                                                        , " angažuje jedna politicky angažovaná osoba - "
                                                                        , " angažují {0} politicky angažované osoby - "
                                                                        , " angažuje {0} politicky angažovaných osob - ")
                                    + sPolitici + ". "
                                    , InfoFact.ImportanceLevel.Medium)                                    
                                    );
                            }
                        }

                        if (PatrimStatu() && stat.RatingPerYear[rok].NumSPolitiky > 0)
                        {
                            f.Add(new InfoFact($"V <b>{rok}</b> uzavřel{(sMuzsky ? "" : "a")} {sName.ToLower()} " +
                                Devmasters.Core.Lang.Plural.Get((int)stat.RatingPerYear[rok].NumSPolitiky, "jednu smlouvu; {0} smlouvy;{0} smluv")
                                + $" s firmama s vazbou na politiky za celkem <b>{HlidacStatu.Util.RenderData.ShortNicePrice(stat.RatingPerYear[rok].SumKcSPolitiky, html: true)}</b> "
                                + $" (tj. {stat.RatingPerYear[rok].PercentKcSPolitiky.ToString("P2")}). "
                                ,InfoFact.ImportanceLevel.Medium)
                                );
                        }
                        else if (PatrimStatu() && stat.RatingPerYear[rok - 1].NumSPolitiky > 0)
                        {
                            f.Add(new InfoFact($"V <b>{rok - 1}</b> uzavřel{(sMuzsky ? "" : "a")} {sName.ToLower()} " +
                                Devmasters.Core.Lang.Plural.Get((int)stat.RatingPerYear[rok - 1].NumSPolitiky, "jednu smlouvu; {0} smlouvy;{0} smluv")
                                + $" s firmama s vazbou na politiky za celkem <b>{HlidacStatu.Util.RenderData.ShortNicePrice(stat.RatingPerYear[rok - 1].SumKcSPolitiky, html: true)}</b> "
                                + $" (tj. {stat.RatingPerYear[rok - 1].PercentKcSPolitiky.ToString("P2")}). "
                                , InfoFact.ImportanceLevel.Medium)
                                );
                        }


                        f.Add(new InfoFact($"Od roku <b>2016</b> uzavřel{(sMuzsky ? "" : "a")} {sName.ToLower()} " +
                            Devmasters.Core.Lang.Plural.Get((int)stat.BasicStatPerYear.SummaryAfter2016().Pocet, "jednu smlouvu v&nbsp;registru smluv", "{0} smlouvy v&nbsp;registru smluv", "celkem {0} smluv v&nbsp;registru smluv")
                            + $" za <b>{HlidacStatu.Util.RenderData.ShortNicePrice(stat.BasicStatPerYear.SummaryAfter2016().CelkemCena, html: true)}</b>. "
                            , InfoFact.ImportanceLevel.Low)
                            );


                    }

                    if (this.PatrimStatu() == false && this.IcosInHolding(Relation.AktualnostType.Aktualni).Count() > 2)
                    {
                        var statHolding = this.StatisticForHolding(Relation.AktualnostType.Aktualni);
                        if (statHolding.Summary.Pocet > 3)
                        {
                            f.Add(new InfoFact($"V roce <b>{rok}</b> uzavřel celý holding " +
                                Devmasters.Core.Lang.Plural.Get((int)statHolding.Data[rok].Pocet, "jednu smlouvu v&nbsp;registru smluv", "{0} smlouvy v&nbsp;registru smluv", "celkem {0} smluv v&nbspregistru smluv")
                                + $" za <b>{HlidacStatu.Util.RenderData.ShortNicePrice(statHolding.Data[rok].CelkemCena, html: true)}</b>. "
                                , InfoFact.ImportanceLevel.Low)
                                );

                            f.Add(new InfoFact($"Mezi lety <b>{rok - 1}-{rok - 2000}</b> "
                                + stat.BasicStatPerYear.YearChange(rok).RenderChangeWord(false, stat.BasicStatPerYear.YearChange(rok).CenaChangePerc,
                                "celému holdingu poklesla hodnota smluv o&nbsp;<b>{0:P2}</b>. ", " nedošlo pro celý holding ke změně hodnoty smluv. ", "celému holdingu narostla hodnota smluv o&nbsp;<b>{0:P2}</b>. ")
                                , InfoFact.ImportanceLevel.Low)
                                );
                        }
                    }

                    _infofacts = f.OrderByDescending(o => o.Level).ToArray();
                }
            }
            return _infofacts;
        }


        public string ToAuditJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public string ToAuditObjectTypeName()
        {
            return "Subjekt";
        }

        public string ToAuditObjectId()
        {
            return this.ICO;
        }

        public string GetUrl(bool local = true)
        {
            return GetUrl(local, string.Empty);
        }
            public string GetUrl(bool local, string foundWithQuery)
        {
            string url = "/subjekt/" + this.ICO;
            if (!string.IsNullOrEmpty(foundWithQuery))
                url = url + "?qs=" + System.Net.WebUtility.UrlEncode(foundWithQuery);
            if (!local)
                url = "https://www.hlidacstatu.cz" + url;

            return url;
        }

        public string BookmarkName()
        {
            return this.Jmeno;
        }
    }
}
