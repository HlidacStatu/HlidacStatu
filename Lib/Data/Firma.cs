﻿using Devmasters;

using HlidacStatu.Lib.Analytics;
using HlidacStatu.Util;
using HlidacStatu.Util.Cache;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace HlidacStatu.Lib.Data
{
    public partial class Firma
        : Bookmark.IBookmarkable, ISocialInfo
    {
        public enum TypSubjektuEnum
        {
            Neznamy = -1,
            Soukromy = 0,
            PatrimStatu = 1,
            PatrimStatu25perc = 2,
            Ovm = 10,
            Obec = 20

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


        static string cnnStr = Devmasters.Config.GetWebConfigValue("CnnString");


        public static string[] StatniFirmyICO = new string[] { "45274649", "25702556", "70994226", "47114983", "42196451", "60193531", "00002739", "00514152", "00000493", "00001279", "24821993", "00000205", "60193468", "00000515", "49710371", "70890013", "70889953", "70890005", "25291581", "70889988", "70890021", "00007536", "26463318", "00024007", "25401726", "00015679", "00010669", "49241494", "14450216", "00001481", "00001490", "00002674", "43833560", "48204285", "00013251", "00014125", "27146235", "49973720", "00311391", "25125877", "00013455", "60197901", "60196696", "00251976", "62413376", "00577880", "44848943", "63078333", "45279314", "13695673", "27772683", "45273448", "28196678", "27786331", "61459445", "27364976", "24829871", "27257258", "17047234", "27378225", "27892646", "27195872", "45795908", "28244532", "61860336", "27145573", "25674285", "25085531", "27232433", "24729035", "27257517", "49901982", "27309941", "28786009", "47115726", "26871823", "26470411", "26206803", "28255933", "28707052", "26376547", "60698101", "27804721", "26840065", "25938924", "00128201", "26051818", "28861736" };

        public static int[] StatniFirmy_BasedKodPF = new int[] {
            301,302,312,313,314,325,331,352,353,361,362,381,382,521,771,801,804,805
        };

        /*
         * https://wwwinfo.mfcr.cz/ares/aresPrFor.html.cz
         * KOD_PF
         * 
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
        public int? Typ { get; set; }


        public TypSubjektuEnum TypSubjektu
        {
            get
            {
                if (this.Typ.HasValue)
                    return (TypSubjektuEnum)this.Typ;
                else
                    return TypSubjektuEnum.Neznamy;
            }
            set
            {
                this.Typ = (int)value;
            }
        }

        public void SetTyp()
        {
            bool obec = false;
            if (this._jsemOVM()) //Obec co neni v kategorii Obce s rozšířenou působností
            {
                if (this.KategorieOVM().Any(m => m.id == 14) == true //je obec
                   )
                {
                    obec = true;
                }
            }

            if (obec)
                this.TypSubjektu = TypSubjektuEnum.Obec;
            else if (this._jsemOVM())
                this.TypSubjektu = TypSubjektuEnum.Ovm;
            else if (this._patrimStatuAlespon25procent())
                this.TypSubjektu = TypSubjektuEnum.PatrimStatu25perc;
            else if (this._jsemStatniFirma())
                this.TypSubjektu = TypSubjektuEnum.PatrimStatu;
            else
                this.TypSubjektu = TypSubjektuEnum.Soukromy;
        }

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
            }

        }

        public string KrajId { get; set; }
        public string OkresId { get; set; }

        public short? IsInRS { get; set; }

        FirmaHint _firmaHint = null;
        public FirmaHint Hint
        {
            get
            {
                if (_firmaHint == null)
                {
                    _firmaHint = FirmaHint.Load(this.ICO);

                }
                return _firmaHint;
            }
        }




        private string _jmeno = string.Empty;
        public string Jmeno
        {
            get { return _jmeno; }
            set
            {
                this._jmeno = value;
                this.JmenoAscii = Devmasters.TextUtil.RemoveDiacritics(value);
            }
        }
        public string JmenoAscii { get; set; }

        public string JmenoOrderReady()
        {
            string[] prefixes = new string[] { "^Statutární\\s*město\\s", "^Město\\s ", "^Městská\\s*část\\s ", "^Obec\\s " };
            string jmeno = this.Jmeno.Trim();
            foreach (var pref in prefixes)
            {
                if (Regex.IsMatch(jmeno, pref, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant))
                    return jmeno.ReplaceWithRegex("", pref).Trim();
            }
            return this.Jmeno;
        }


        // https://wwwinfo.mfcr.cz/ares/aresPrFor.html.cz

        public int? Kod_PF { get; set; }

        // https://wwwinfo.mfcr.cz/ares/nace/ares_nace.html.cz
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
            if (_vazby == null || refresh == true)
            {
                updateVazby(refresh);
            }
            return _vazby;
        }
        public void Vazby(IEnumerable<Graph.Edge> value)
        {
            _vazby = value.ToArray();
        }

        Graph.Edge[] _parentVazbyFirmy = null;
        Firma[] _parents = null;
        public Firma[] ParentVazbyFirmy(Relation.AktualnostType minAktualnost)
        {
            if (_parents == null)
            {
                _parents = _getAllParents(this.ICO, minAktualnost).ToArray();
            }

            return _parents;
        }

        public List<Firma> _getAllParents(string ico, Relation.AktualnostType minAktualnost, List<Firma> currList = null)
        {
            currList = currList ?? new List<Firma>();

            var _parentVazbaFirma = Relation
                .AktualniVazby(Graph.GetDirectParentRelationsFirmy(ico).ToArray(), minAktualnost)
                .Select(m => Firmy.Get(m.From.Id))
                .Where(m => m != null)
                .ToArray();
            if (_parentVazbaFirma.Count() > 0)
            {
                bool added = false;
                foreach (var f in _parentVazbaFirma)
                {
                    if (currList.Any(m => m.ICO == f.ICO))
                    {
                        //skip
                    }
                    else
                    {
                        currList.Insert(0, f);
                        added = true;
                    }
                }
                if (added)
                    return _getAllParents(currList[0].ICO, minAktualnost, currList);
                else
                    return currList;
            }
            else
                return currList;
        }

        public Graph.Edge[] ParentVazbaFirmy(Relation.AktualnostType minAktualnost)
        {
            if (_parentVazbyFirmy == null)
                _parentVazbyFirmy = Graph.GetDirectParentRelationsFirmy(this.ICO).ToArray();
            return Relation.AktualniVazby(_parentVazbyFirmy, minAktualnost);
        }

        Graph.Edge[] _parentVazbyOsoby = null;
        public Graph.Edge[] ParentVazbyOsoby(Relation.AktualnostType minAktualnost)
        {
            if (_parentVazbyOsoby == null)
                _parentVazbyOsoby = Graph.GetDirectParentRelationsOsoby(this.ICO).ToArray();
            return Relation.AktualniVazby(_parentVazbyOsoby, minAktualnost);
        }

        public IEnumerable<SocialContact> GetSocialContacts()
        {
            return this.Events(oe => oe.Type == (int)OsobaEvent.Types.SocialniSite)
                .Select(oe => new SocialContact
                {
                    Network = EnumsNET.Enums.TryParse<OsobaEvent.SocialNetwork>(oe.Organizace, true, out var x)
                        ? EnumsNET.Enums.Parse<OsobaEvent.SocialNetwork>(oe.Organizace, true)
                        : (OsobaEvent.SocialNetwork?)null,
                    NetworkText = oe.Organizace,
                    Contact = oe.AddInfo
                });
        }


        public bool MaVztahySeStatem()
        {
            var ret = this.IsSponzor();
            if (ret) return ret;

            ret = this.StatistikaRegistruSmluv().Sum(s => s.PocetSmluv) > 0;
            if (ret) return ret;

            ret = HlidacStatu.Lib.Data.VZ.VerejnaZakazka.Searching.SimpleSearch("ico:" + this.ICO, null, 1, 1, "0").Total > 0;
            if (ret) return ret;

            ret = new HlidacStatu.Lib.Data.Dotace.DotaceService().SimpleSearch("ico:" + this.ICO, 1, 1, "0").Total > 0;
            return ret;

        }

        public bool MaVazbyNaPolitikyPred(DateTime date)
        {
            if (MaVazbyNaPolitiky())
            {
                var osoby = VazbyNaPolitiky();
                foreach (var o in osoby)
                {
                    var found = o.Sponzoring().Any(m => m.DarovanoDne < date);
                    if (found)
                        return true;
                }
            }
            return false;
        }


        static string[] vyjimky_v_RS_ = new string[] { "00006572", "63839407", "48136000", "48513687", "49370227", "70836981", "05553539" }; //§ 3 odst. 2 f) ... Poslanecká sněmovna, Senát, Kancelář prezidenta republiky, Ústavní soud, Nejvyšší kontrolní úřad, Kancelář veřejného ochránce práv a Úřad Národní rozpočtové rady
        public bool MusiPublikovatDoRS()
        {
            if (vyjimky_v_RS_.Contains(this.ICO))
                return false;

            bool musi = this.JsemOVM() || this.JsemStatniFirma();
            if (this.JsemOVM()) //Obec co neni v kategorii Obce s rozšířenou působností
            {
                if (this.KategorieOVM().Any(m => m.id == 14) == true && //je obec
                        this.KategorieOVM().Any(m => m.id == 11) == false //neni v kategorii Obce s rozšířenou působností
                   )
                {
                    musi = false;
                }
            }
            else if (this.JsemStatniFirma())
            {
                var parentOVM = this.ParentVazbyFirmy(Relation.AktualnostType.Aktualni)
                    .ToArray();

                musi = parentOVM
                    .Any(m => m.JsemOVM() && m.KategorieOVM().Any(k => k.id == 11) == true);
            }
            return musi;
        }

        Lib.Data.External.RPP.KategorieOVM[] _kategorieOVM = null;
        public Lib.Data.External.RPP.KategorieOVM[] KategorieOVM()
        {
            if (_kategorieOVM == null)
            {
                var res = Lib.ES.Manager.GetESClient_RPP_Kategorie().Search<Lib.Data.External.RPP.KategorieOVM>(
                     s => s
                     .Query(q => q.QueryString(qs => qs.Query($"oVM_v_kategorii.kodOvm:{this.ICO}")))
                     .Source(so => so.Excludes(ex => ex.Field("oVM_v_kategorii")))
                     .Size(1500)
                     );
                if (res.IsValid)
                    _kategorieOVM = res.Hits
                        .Select(m => m.Source)
                        .OrderByDescending(m => m.hlidac_preferred ? 1 : 0)
                        .ThenBy(m => m.nazev)
                        .ToArray();
                else
                    _kategorieOVM = new External.RPP.KategorieOVM[] { };
            }
            return _kategorieOVM;
        }

        public Analytics.StatisticsSubjectPerYear<Smlouva.Statistics.Data> StatistikaRegistruSmluv(Smlouva.SClassification.ClassificationsTypes classif)
        {
            return StatistikaRegistruSmluv((int)classif);
        }
        public Analytics.StatisticsSubjectPerYear<Smlouva.Statistics.Data> StatistikaRegistruSmluv(int? iclassif = null)
        {

            return Statistics.CachedStatistics(this, iclassif);
        }
        public Analytics.StatisticsSubjectPerYear<Statistics.Dotace> StatistikaDotaci()
        {
            return Statistics.DotaceCache().Get(this);
        }

        public Analysis.KorupcniRiziko.KIndexData Kindex(bool useTemp = false)
        {
            return Analysis.KorupcniRiziko.KIndex.Get(this.ICO, useTemp);
        }

        public bool MaVazbyNaPolitiky()
        {
            return HlidacStatu.Lib.StaticData.FirmySVazbamiNaPolitiky_nedavne_Cache.Get().SoukromeFirmy.ContainsKey(this.ICO);
        }

        public Osoba[] VazbyNaPolitiky()
        {
            return
                HlidacStatu.Lib.StaticData.FirmySVazbamiNaPolitiky_nedavne_Cache.Get()
                    .SoukromeFirmy[this.ICO]
                    .Select(pid => HlidacStatu.Lib.StaticData.PolitickyAktivni.Get().Where(m => m.InternalId == pid).FirstOrDefault())
                    .Where(p => p != null)
                    .OrderBy(p => p.Prijmeni)
                    .ToArray();
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

        public Graph.Edge[] AktualniVazby(Relation.AktualnostType minAktualnost, bool refresh = false)
        {
            return Relation.AktualniVazby(this.Vazby(refresh), minAktualnost);
        }


        public Graph.Edge[] VazbyProICO(string ico)
        {
            return _vazbyProIcoCache.Get((this, ico));
        }
        static private MemoryCacheManager<Graph.Edge[], (Firma f, string ico)> _vazbyProIcoCache
            = MemoryCacheManager<Graph.Edge[], (Firma f, string ico)>
                .GetSafeInstance("_vazbyFirmaProIcoCache", f =>
                {
                    return f.f._vazbyProICO(f.ico);
                },
                    TimeSpan.FromHours(2), k => (k.f.ICO + "-" + k.ico)
               );


        //Napojení na graf
        //Graph.Shortest.EdgePath shortestGraph = null;
        private Graphs2.UnweightedGraph _graph = null;
        private Graphs2.Vertex<string> _startingVertex = null; //not for other use except as a search starting point
        private void InitializeGraph()
        {
            _graph = new Graphs2.UnweightedGraph();
            foreach (var vazba in this.Vazby())
            {
                if (vazba.From is null)
                {
                    _startingVertex = new Graphs2.Vertex<string>(vazba.To.UniqId);
                    continue;
                }

                if (vazba.To is null)
                    continue;

                var fromVertex = new Graphs2.Vertex<string>(vazba.From.UniqId);
                var toVertex = new Graphs2.Vertex<string>(vazba.To.UniqId);

                _graph.AddEdge(fromVertex, toVertex, vazba);
            }
        }
        private Graph.Edge[] _vazbyProICO(string ico)
        {
            if (_graph is null || _graph.Vertices.Count == 0)
                InitializeGraph();

            if (_startingVertex is null)
                _startingVertex = new Graphs2.Vertex<string>(Graph.Node.Prefix_NodeType_Company + this.ICO);

            try
            {
                var shortestPath = _graph.ShortestPath(_startingVertex, CreateVertexFromIco(ico));
                if (shortestPath == null)
                    return Array.Empty<Graph.Edge>();

                var result = shortestPath.Select(x => ((Graphs2.Edge<Graph.Edge>)x).BindingPayload).ToArray();
                return result; // shortestGraph.ShortestTo(ico).ToArray();
            }
            catch (Exception e)
            {
                Util.Consts.Logger.Error("Vazby ERROR for " + this.ICO, e);
                return Array.Empty<Graph.Edge>();
            }
        }
        private static Graphs2.Vertex<string> CreateVertexFromIco(string ico)
        {
            return new Graphs2.Vertex<string>($"{Graph.Node.Prefix_NodeType_Company}{ico}");
        }


        public void UpdateVazbyFromDB()
        {

            List<Graph.Edge> oldRel = new List<Graph.Edge>();
            if (this.Vazby() != null)
            {
                //oldRel = this.GetVazby().Where(m => m.Permanent).ToList();
            }


            var firstRel = Graph.VsechnyDcerineVazby(this.ICO, true);


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

        public bool NotInterestingToShow()
        {
            return (this.MaVztahySeStatem() == false)
                        && (this.IsNespolehlivyPlatceDPH() == false)
                        && (this.MaVazbyNaPolitiky() == false);
        }


        public string SocialInfoTitle()
        {
            return Devmasters.TextUtil.ShortenText(this.Jmeno, 50);
        }

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

            this.JmenoAscii = Devmasters.TextUtil.RemoveDiacritics(this.Jmeno);
            SetTyp();

            string sql = @"exec Firma_Save @ICO,@DIC,@Datum_zapisu_OR,@Stav_subjektu,@Jmeno,@Jmenoascii,@Kod_PF,@Source, @Popis, @VersionUpdate, @krajId, @okresId, @status, @typ  ";
            string sqlNACE = @"INSERT into firma_NACE(ico, nace) values(@ico,@nace)";
            string sqlDS = @"INSERT into firma_DS(ico, DatovaSchranka) values(@ico,@DatovaSchranka)";

            string cnnStr = Devmasters.Config.GetWebConfigValue("CnnString");
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
                        new System.Data.SqlClient.SqlParameter("Typ", this.Typ),
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

        /// <summary>
        /// Find last known CEO
        /// </summary>
        /// <returns></returns>
        public (Osoba Osoba, DateTime? From, string Role) Ceo()
        {
            using (DbEntities db = new DbEntities())
            {
                var ceoEvent = db.OsobaEvent
                    .Where(oe => oe.CEO == 1 && oe.Ico == this.ICO)
                    .Where(oe => oe.DatumDo == null || oe.DatumDo >= DateTime.Now)
                    .Where(oe => oe.DatumOd != null && oe.DatumOd <= DateTime.Now)
                    .OrderByDescending(oe => oe.DatumOd)
                    .FirstOrDefault();

                if (ceoEvent is null)
                    return (null, null, null);

                var lastCeo = Osoba.GetByInternalId(ceoEvent.OsobaId);
                if (lastCeo is null || !lastCeo.IsValid())
                    return (null, null, null);

                return (lastCeo, ceoEvent.DatumOd, ceoEvent.AddInfo);
            }
        }

        public void RefreshDS()
        {
            this.DatovaSchranka = External.DatoveSchranky.ISDS.GetDatoveSchrankyForIco(this.ICO);
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

        /// <summary>
        /// Vrací firmy z holdingu ! KROMĚ mateřské firmy!
        /// </summary>
        /// <param name="aktualnost"></param>
        /// <returns></returns>
        public IEnumerable<Firma> Holding(Relation.AktualnostType aktualnost)
        {
            var icos = IcosInHolding(aktualnost);

            return icos.Select(ico => Firmy.Get(ico));
        }

        public Analytics.StatisticsSubjectPerYear<Statistics.Dotace> HoldingStatisticsDotace(Relation.AktualnostType aktualnost)
        {
            var firmy = Holding(aktualnost).ToArray();

            var statistiky = firmy.Select(f => f.StatistikaDotaci()).Append(this.StatistikaDotaci()).ToArray();

            var aggregate = Analytics.StatisticsSubjectPerYear<Statistics.Dotace>.Aggregate(statistiky);

            return aggregate;
        }

        public Analytics.StatisticsSubjectPerYear<Smlouva.Statistics.Data> HoldingStatisticsRegistrSmluv(
            Relation.AktualnostType aktualnost)
        {
            var firmy = Holding(aktualnost).ToArray();

            var statistiky = firmy.Select(f => f.StatistikaRegistruSmluv()).Append(this.StatistikaRegistruSmluv()).ToArray();

            var aggregate = Analytics.StatisticsSubjectPerYear<Smlouva.Statistics.Data>.Aggregate(statistiky);

            return aggregate;
        }

        public Analytics.StatisticsSubjectPerYear<Smlouva.Statistics.Data> HoldingStatisticsRegistrSmluvProObor(
            Relation.AktualnostType aktualnost,
            Smlouva.SClassification.ClassificationsTypes classification)
        {
            var firmy = Holding(aktualnost).ToArray();

            var statistiky = firmy.Select(f => f.StatistikaRegistruSmluv(classification)).Append(this.StatistikaRegistruSmluv()).ToArray();

            var aggregate = Analytics.StatisticsSubjectPerYear<Smlouva.Statistics.Data>.Aggregate(statistiky);

            return aggregate;
        }


        public bool JsemSoukromaFirma()
        {
            return JsemOVM() == false && JsemStatniFirma() == false;
        }

        static int[] Neziskovky_KOD_PF = new int[] { 116, 117, 118, 141, 161, 422, 423, 671, 701, 706, 736 };
        public bool JsemNeziskovka()
        {
            if (JsemSoukromaFirma() == false)
                return false;
            else if (this.Kod_PF.HasValue == false)
                return false;
            else
            {
                return Neziskovky_KOD_PF.Contains(this.Kod_PF.Value);

            }
        }

        public bool PatrimStatuAlespon25procent() => (TypSubjektu == TypSubjektuEnum.PatrimStatu25perc);

        private bool _patrimStatuAlespon25procent()
        {
            if (_jsemOVM())
                return true;

            if (
                (this.Kod_PF != null && Firma.StatniFirmy_BasedKodPF.Contains(this.Kod_PF.Value))
                || (StaticData.VsechnyStatniMestskeFirmy25percs.Contains(this.ICO))
                )
            {
                return true;
            }
            else
                return false;

        }

        public bool PatrimStatu()
        {
            return JsemOVM() || JsemStatniFirma();
        }

        /// <summary>
        /// Orgán veřejné moci
        /// </summary>
        /// <returns></returns>
        public bool JsemOVM() => this.TypSubjektu == TypSubjektuEnum.Ovm || TypSubjektu == TypSubjektuEnum.Obec;

        private bool _jsemOVM()
        {
            return StaticData.Urady_OVM.Contains(this.ICO);
        }
        public bool JsemStatniFirma() => this.TypSubjektu == TypSubjektuEnum.PatrimStatu || this.TypSubjektu == TypSubjektuEnum.PatrimStatu25perc;
        private bool _jsemStatniFirma()
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

        public bool IsSponzorBefore(DateTime date)
        {
            if (this.JsemOVM())
                return false;
            if (this.JsemStatniFirma())
                return false;
            return StaticData.SponzorujiciFirmy_Vsechny.Get()  //todo: nemyslím si, že primární entita by měla čerpat info z cache
                .Where(m => m.IcoDarce == this.ICO && m.DarovanoDne < date)
                .Any();
        }

        public bool IsSponzor()
        {
            return this.Sponzoring().Any();
        }

        public IEnumerable<OsobaEvent> Events()
        {
            return Events(m => true);
        }

        public IEnumerable<Sponzoring> Sponzoring()
        {
            return Sponzoring(m => true);
        }

        public string SponzoringToHtml(int take = int.MaxValue)
        {
            return string.Join("<br />",
                Sponzoring()
                    .OrderByDescending(s => s.DarovanoDne)
                    .Select(s => s.ToHtml())
                    .Take(take));
        }

        public string EventsToHtml(Expression<Func<OsobaEvent, bool>> predicate, int take = int.MaxValue)
        {
            return string.Join("<br />",
                Events(predicate)
                    .OrderByDescending(s => s.DatumDo == null ? s.DatumDo : s.DatumOd)
                    .Select(e => e.RenderHtml())
                    .Take(take));
        }

        public IEnumerable<OsobaEvent> Events(Expression<Func<OsobaEvent, bool>> predicate)
        {
            using (DbEntities db = new DbEntities())
            {
                return db.OsobaEvent
                    .AsNoTracking()
                    .Where(predicate)
                    .Where(m => m.Ico == this.ICO)
                    .ToArray();
            }
        }

        public IEnumerable<Sponzoring> Sponzoring(Expression<Func<Sponzoring, bool>> predicate)
        {
            using (DbEntities db = new DbEntities())
            {
                return db.Sponzoring
                    .AsNoTracking()
                    .Where(predicate)
                    .Where(s => s.IcoDarce == this.ICO)
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

        public OsobaEvent AddOrUpdateEvent(OsobaEvent ev, string user, bool rewrite = false, bool checkDuplicates = true)
        {
            if (ev == null)
                return null;

            //check duplicates
            using (DbEntities db = new Data.DbEntities())
            {
                OsobaEvent exists = null;
                if (ev.pk > 0 && checkDuplicates)
                    exists = db.OsobaEvent
                                .AsNoTracking()
                                .FirstOrDefault(m =>
                                    m.pk == ev.pk
                                );
                else if (checkDuplicates)
                {
                    exists = db.OsobaEvent
                                .AsNoTracking()
                                .FirstOrDefault(m =>
                                        m.Ico == this.ICO
                                        && m.Type == ev.Type
                                        && m.AddInfo == ev.AddInfo
                                        && m.AddInfoNum == ev.AddInfoNum
                                        && m.DatumOd == ev.DatumOd
                                        && m.DatumDo == ev.DatumDo
                                //&& m.Zdroj == ev.Zdroj
                                );
                    if (exists != null)
                        ev.pk = exists.pk;
                }
                if (exists != null && rewrite)
                {
                    db.OsobaEvent.Remove(exists);
                    Audit.Add<OsobaEvent>(Audit.Operations.Delete, user, exists, null);
                    db.SaveChanges();
                    exists = null;
                }

                if (exists == null)
                {
                    ev.Ico = this.ICO;
                    db.OsobaEvent.Add(ev);
                    Audit.Add(Audit.Operations.Create, user, ev, null);
                    db.SaveChanges();
                    return ev;
                }
                else
                {
                    ev.Ico = this.ICO;
                    ev.pk = exists.pk;
                    db.OsobaEvent.Attach(ev);
                    db.Entry(ev).State = System.Data.Entity.EntityState.Modified;
                    Audit.Add(Audit.Operations.Update, user, ev, exists);
                    db.SaveChanges();
                    return ev;
                }
            }
        }

        public Sponzoring AddSponsoring(Sponzoring sponzoring, string user)
        {
            sponzoring.IcoDarce = this.ICO;
            var result = Lib.Data.Sponzoring.CreateOrUpdate(sponzoring, user);
            return result;
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

            else if (getMissingFromExternal)
            {
                f = External.Merk.FromIco(ico);
                if (Firma.IsValid(f))
                    return f;

                if (!Firma.IsValid(f)) //try firmo
                {
                    f = External.RZP.FromIco(ico);
                }

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

        [Obsolete]
        public static Firma FromTransactionId(Data.TransparentniUcty.BankovniPolozka transaction)
        {
            using (DbEntities db = new Data.DbEntities())
            {
                string url = transaction.GetUrl(false);
                var found = db.OsobaEvent
                            .Where(m => m.Zdroj == url)
                            .FirstOrDefault();
                if (found != null)
                {
                    return FromIco(found.Ico);
                }

            }
            return null;
        }


        public string Description(bool html, string template = "{0}", string itemTemplate = "{0}", string itemDelimeter = "<br/>", int numOfRecords = int.MaxValue)
        {
            return Description(html, m => true, template, itemTemplate, itemDelimeter, numOfRecords);
        }

        public string Description(bool html, Expression<Func<OsobaEvent, bool>> predicate,
            string template = "{0}", string itemTemplate = "{0}",
            string itemDelimeter = "<br/>", int numOfRecords = int.MaxValue)
        {
            StringBuilder sb = new StringBuilder();
            var events = this.Events(predicate);
            if (events.Count() == 0)
                return string.Empty;
            else
            {
                List<string> evs = events
                    .OrderBy(e => e.DatumOd)
                    .Select(e => html ? e.RenderHtml(", ") : e.RenderText(", "))
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
                    //var stat = new HlidacStatu.Lib.Analysis.SubjectStatistic(this);
                    var stat = this.StatistikaRegistruSmluv();
                    int rok = DateTime.Now.Year;
                    if (DateTime.Now.Month < 2)
                        rok = rok - 1;

                    if (stat.Sum(stat.YearsAfter2016(), s => s.PocetSmluv) == 0)
                    {
                        f.Add(new InfoFact($"{sName} nemá žádné smluvní vztahy evidované v&nbsp;registru smluv. ", InfoFact.ImportanceLevel.Medium));
                        f.Add(new InfoFact($"{(sMuzsky ? "Byl založen" : "Byla založena")} <b>{this.Datum_Zapisu_OR?.ToString("d. M. yyyy")}</b>. ", InfoFact.ImportanceLevel.Medium));
                    }
                    else
                    {
                        f.Add(new InfoFact($"V roce <b>{rok}</b> uzavřel{(sMuzsky ? "" : "a")} {sName.ToLower()} " +
                            Devmasters.Lang.Plural.Get(stat[rok].PocetSmluv, "jednu smlouvu v&nbsp;registru smluv", "{0} smlouvy v&nbsp;registru smluv", "celkem {0} smluv v&nbsp;registru smluv")
                            + $" za <b>{HlidacStatu.Util.RenderData.ShortNicePrice(stat[rok].CelkovaHodnotaSmluv, html: true)}</b>. "
                            , InfoFact.ImportanceLevel.Summary)
                            );

                        (decimal zmena, decimal? procentniZmena) = stat.ChangeBetweenYears(rok - 1, rok, s => s.CelkovaHodnotaSmluv);
                        if (procentniZmena.HasValue)
                        {
                            string text = $"Mezi lety <b>{rok - 1}-{rok - 2000}</b> ";
                            switch (zmena)
                            {
                                case decimal n when n > 0:
                                    text += $"došlo k <b>nárůstu zakázek o&nbsp;{procentniZmena:P2}</b> v&nbsp;Kč. ";
                                    break;
                                case decimal n when n < 0:
                                    text += $"došlo k <b>poklesu zakázek o&nbsp;{procentniZmena:P2}</b> v&nbsp;Kč . ";
                                    break;
                                default:
                                    text += " nedošlo ke změně objemu zakázek. ";
                                    break;
                            }
                            f.Add(new InfoFact(text, InfoFact.ImportanceLevel.Medium));
                        }

                        if (stat[rok].PocetSmluvBezCeny > 0)
                        {
                            f.Add(new InfoFact(
                                $"V <b>{rok} utajil{(sMuzsky ? "" : "a")}</b> hodnotu kontraktu " +
                                Devmasters.Lang.Plural.Get(stat[rok].PocetSmluvBezCeny, "u&nbsp;jedné smlouvy", "u&nbsp;{0} smluv", "u&nbsp;{0} smluv")
                                + $", což je celkem <b>{stat[rok].PercentSmluvBezCeny.ToString("P2")}</b> ze všech. ",
                                 InfoFact.ImportanceLevel.Medium)
                                );
                        }
                        else if (stat[rok - 1].PocetSmluvBezCeny > 0)
                        {
                            f.Add(new InfoFact(
                                $"V <b>{rok - 1} utajil{(sMuzsky ? "" : "a")}</b> hodnotu kontraktů " +
                                Devmasters.Lang.Plural.Get(stat[rok - 1].PocetSmluvBezCeny, "u&nbsp;jedné smlouvy", "u&nbsp;{0} smluv", "u&nbsp;{0} smluv")
                                + $", což je celkem <b>{stat[rok - 1].PercentSmluvBezCeny.ToString("P2")}</b> ze všech. "
                                , InfoFact.ImportanceLevel.Medium)
                                );
                        }

                        long numFatalIssue = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch($"ico:{this.ICO} AND chyby:zasadni", 0, 0, HlidacStatu.Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, exactNumOfResults: true).Total;
                        long numVazneIssue = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch($"ico:{this.ICO} AND chyby:vazne", 0, 0, HlidacStatu.Lib.Data.Smlouva.Search.OrderResult.FastestForScroll, exactNumOfResults: true).Total;

                        if (numFatalIssue > 0)
                        {
                            f.Add(new InfoFact($@"Má v registru smluv
                                    <b>{Devmasters.Lang.Plural.GetWithZero((int)numFatalIssue, "0 smluv", "jednu smlouvu obsahující", "{0} smlouvy obsahující", "{0:### ### ##0} smluv obsahujících ")}
                                        tak závažné nedostatky v rozporu se zákonem,
                                    </b>že jsou velmi pravděpodobně neplatné. ", InfoFact.ImportanceLevel.High));
                        }
                        if (numVazneIssue > 0)
                        {
                            f.Add(new InfoFact($@"Má v registru smluv
                                    <b>{Devmasters.Lang.Plural.GetWithZero((int)numFatalIssue, "0 smluv", "jednu smlouvu obsahující", "{0} smlouvy obsahující", "{0:### ### ##0} smluv obsahujících ")}</b>
                                        vážné nedostatky. "
                                        , InfoFact.ImportanceLevel.Medium)
                                        );
                        }

                        if (PatrimStatu() == false)
                        {
                            DateTime datumOd = new DateTime(DateTime.Now.Year - 10, 1, 1);
                            var sponzoring = this.Sponzoring(s => s.IcoPrijemce != null && s.DarovanoDne >= datumOd); // sponzoring pol. stran
                            if (sponzoring != null && sponzoring.Count() > 0)
                            {
                                string[] strany = sponzoring.Select(m => m.IcoPrijemce).Distinct().ToArray();
                                int[] roky = sponzoring.Select(m => m.DarovanoDne.Value.Year).Distinct().OrderBy(y => y).ToArray();
                                decimal celkem = sponzoring.Sum(m => m.Hodnota) ?? 0;
                                decimal top = sponzoring.Max(m => m.Hodnota) ?? 0;
                                string prvniStrana = Firma.FromIco(strany[0]).Jmeno; //todo: přidat tabulku politických stran a změnit zde na název strany

                                f.Add(new InfoFact($"{sName} "
                                    + Devmasters.Lang.Plural.Get(roky.Count(), "v roce " + roky[0], $"mezi roky {roky.First()}-{roky.Last() - 2000}", $"mezi roky {roky.First()}-{roky.Last() - 2000}")
                                    + $" sponzoroval{(sMuzsky ? "" : "a")} " +
                                    Devmasters.Lang.Plural.Get(strany.Length, prvniStrana, "{0} polit.strany", "{0} polit.stran")
                                    + $" v&nbsp;celkové výši <b>{RenderData.ShortNicePrice(celkem, html: true)}</b>. "
                                    + $"Nejvyšší sponzorský dar byl ve výši {RenderData.ShortNicePrice(top, html: true)}. "
                                    , InfoFact.ImportanceLevel.Medium)
                                    );
                            }
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
                                    + Devmasters.Lang.Plural.Get(politici.Count()
                                                                        , " angažuje jedna politicky angažovaná osoba - "
                                                                        , " angažují {0} politicky angažované osoby - "
                                                                        , " angažuje {0} politicky angažovaných osob - ")
                                    + sPolitici + ". "
                                    , InfoFact.ImportanceLevel.Medium)
                                    );
                            }
                        }

                        if (PatrimStatu() && stat[rok].PocetSmluvSponzorujiciFirmy > 0)
                        {
                            f.Add(new InfoFact($"V <b>{rok}</b> uzavřel{(sMuzsky ? "" : "a")} {sName.ToLower()} " +
                                Devmasters.Lang.Plural.Get(stat[rok].PocetSmluvSponzorujiciFirmy, "jednu smlouvu; {0} smlouvy;{0} smluv")
                                + $" s firmama s vazbou na politiky za celkem <b>{HlidacStatu.Util.RenderData.ShortNicePrice(stat[rok].SumKcSmluvSponzorujiciFirmy, html: true)}</b> "
                                + $" (tj. {stat[rok].PercentKcSmluvPolitiky.ToString("P2")}). "
                                , InfoFact.ImportanceLevel.Medium)
                                );
                        }
                        else if (PatrimStatu() && stat[rok - 1].PocetSmluvSponzorujiciFirmy > 0)
                        {
                            f.Add(new InfoFact($"V <b>{rok - 1}</b> uzavřel{(sMuzsky ? "" : "a")} {sName.ToLower()} " +
                                Devmasters.Lang.Plural.Get(stat[rok - 1].PocetSmluvSponzorujiciFirmy, "jednu smlouvu; {0} smlouvy;{0} smluv")
                                + $" s firmama s vazbou na politiky za celkem <b>{HlidacStatu.Util.RenderData.ShortNicePrice(stat[rok - 1].SumKcSmluvSponzorujiciFirmy, html: true)}</b> "
                                + $" (tj. {stat[rok].PercentKcSmluvPolitiky.ToString("P2")}). "
                                , InfoFact.ImportanceLevel.Medium)
                                );
                        }

                        f.Add(new InfoFact($"Od roku <b>2016</b> uzavřel{(sMuzsky ? "" : "a")} {sName.ToLower()} " +
                            Devmasters.Lang.Plural.Get(stat.Sum(stat.YearsAfter2016(), s => s.PocetSmluv), "jednu smlouvu v&nbsp;registru smluv", "{0} smlouvy v&nbsp;registru smluv", "celkem {0} smluv v&nbsp;registru smluv")
                            + $" za <b>{HlidacStatu.Util.RenderData.ShortNicePrice(stat.Sum(stat.YearsAfter2016(), s => s.CelkovaHodnotaSmluv), html: true)}</b>. "
                            , InfoFact.ImportanceLevel.Low)
                            );

                    }

                    if (this.PatrimStatu() == false && this.IcosInHolding(Relation.AktualnostType.Aktualni).Count() > 2)
                    {
                        var statHolding = this.HoldingStatisticsRegistrSmluv(Relation.AktualnostType.Aktualni);
                        if (statHolding[rok].PocetSmluv > 3)
                        {
                            f.Add(new InfoFact($"V roce <b>{rok}</b> uzavřel celý holding " +
                                Devmasters.Lang.Plural.Get(statHolding[rok].PocetSmluv, "jednu smlouvu v&nbsp;registru smluv", "{0} smlouvy v&nbsp;registru smluv", "celkem {0} smluv v&nbspregistru smluv")
                                + $" za <b>{HlidacStatu.Util.RenderData.ShortNicePrice(statHolding[rok].CelkovaHodnotaSmluv, html: true)}</b>. "
                                , InfoFact.ImportanceLevel.Low)
                                );

                            string text = $"Mezi lety <b>{rok - 1}-{rok - 2000}</b> ";
                            (decimal zmena, decimal? procentniZmena) = statHolding.ChangeBetweenYears(rok - 1, rok, s => s.CelkovaHodnotaSmluv);

                            if (procentniZmena.HasValue)
                            {
                                switch (zmena)
                                {
                                    case decimal n when n > 0:
                                        text += $"celému holdingu narostla hodnota smluv o&nbsp;<b>{procentniZmena:P2}</b>. ";
                                        break;
                                    case decimal n when n < 0:
                                        text += $"celému holdingu poklesla hodnota smluv o&nbsp;<b>{procentniZmena:P2}</b>. ";
                                        break;
                                    default:
                                        text += "nedošlo pro celý holding ke změně hodnoty smluv. ";
                                        break;
                                }

                                f.Add(new InfoFact(text, InfoFact.ImportanceLevel.Low));
                            }
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
