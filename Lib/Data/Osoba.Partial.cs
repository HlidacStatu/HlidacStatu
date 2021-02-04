using Devmasters;
using Devmasters.Enums;

using HlidacStatu.Lib.Analytics;
using HlidacStatu.Util;
using HlidacStatu.Util.Cache;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HlidacStatu.Lib.Data
{
    [MetadataType(typeof(OsobaMetadata))] // napojení na metadata (validace)
    public partial class Osoba
        : Bookmark.IBookmarkable, ISocialInfo
    {

        public static string[] TitulyPred = new string[] {
            "Akad. mal.","Akad. malíř","arch.","as.","Bc.","Bc. et Bc.","BcA.","Dip Mgmt.",
            "DiS.","Doc.","Dott.","Dr.","DrSc.","et","Ing.","JUDr.","Mag.",
            "MDDr.","Mg.","Mg.A.","MgA.","Mgr.","MSc.","MSDr.","MUDr.","MVDr.",
            "Odb. as.","PaedDr.","Ph.Dr.","PharmDr.","PhDr.","PhMr.","prof.",
            "Prof.","RCDr.","RNDr.","RSDr.","RTDr.","ThDr.","ThLic.","ThMgr." };

        public static string[] TitulyPo = new string[] {
            "BA","HONS", "BBA",
            "DBA","DBA.","CertHE","DipHE","BSc","BSBA","BTh","MIM","BBS","DiM","Di.M.",
            "CSc.", "D.E.A.", "DiS.", "Dr.h.c.", "DrSc.", "FACP", "jr.", "LL.M.",
            "MBA", "MD", "MEconSc.", "MgA.", "MIM", "MPA", "MPH", "MSc.", "Ph.D.", "Th.D." };


        public Osoba()
        {

        }


        public StatusOsobyEnum StatusOsoby() { return (StatusOsobyEnum)this.Status; }

        [ShowNiceDisplayName()]
        public enum StatusOsobyEnum
        {
            [NiceDisplayName("Nepolitická osoba")]
            NeniPolitik = 0,

            [NiceDisplayName("Bývalý politik")]
            ByvalyPolitik = 1,
            [NiceDisplayName("Osoba s vazbami na politiky")]
            VazbyNaPolitiky = 2,
            [NiceDisplayName("Politik")]
            Politik = 3,
            [NiceDisplayName("Sponzor polit.strany")]
            Sponzor = 4,
        }


        public bool IsValid()
        {
            return !string.IsNullOrEmpty(this.Jmeno) && !string.IsNullOrEmpty(this.Prijmeni) && this.InternalId > 0;
        }

        //public string JmenoAscii { get { return Devmasters.TextUtil.RemoveDiacritics(this.Jmeno) ?? ""; } }
        //public string PrijmeniAscii { get { return Devmasters.TextUtil.RemoveDiacritics(this.Prijmeni) ?? ""; } }

        OsobaExternalId[] _externalIds = null;
        public OsobaExternalId[] ExternalIds()
        {
            if (_externalIds == null)
            {
                using (Lib.Data.DbEntities db = new Data.DbEntities())
                {
                    _externalIds = db.OsobaExternalId.Where(o => o.OsobaId == this.InternalId).ToArray();
                }

            }
            return _externalIds;
        }

        public override string ToString()
        {
            //return base.ToString();
            return this.FullNameWithNarozeni();
        }

        private static DateTime minLookBack = new DateTime(DateTime.Now.Year - 5, 1, 1);
        public bool IsPolitikBasedOnEvents()
        {
            var ret = this.Events(ev =>
                        ev.Type == (int)OsobaEvent.Types.Politicka
                        || ev.Type == (int)OsobaEvent.Types.PolitickaPracovni
                        || ev.Type == (int)OsobaEvent.Types.VolenaFunkce
                    )
                    .Where(ev => (ev.DatumDo.HasValue && ev.DatumDo > minLookBack) || (ev.DatumDo.HasValue == false && ev.DatumOd > minLookBack.AddYears(-2)))
                    .ToArray();

            return ret.Count() > 0;
        }

        /// <summary>
        /// returns true if changed
        /// </summary>
        public bool RecalculateStatus()
        {
            switch (this.StatusOsoby())
            {
                case StatusOsobyEnum.NeniPolitik:
                    if (IsPolitikBasedOnEvents())
                    {
                        this.Status = (int)StatusOsobyEnum.Politik;
                        return true;
                    }

                    //TODO zkontroluj, ze neni politik podle eventu
                    break;

                case StatusOsobyEnum.VazbyNaPolitiky:
                case StatusOsobyEnum.ByvalyPolitik:
                case StatusOsobyEnum.Sponzor:
                    if (IsPolitikBasedOnEvents())
                    {
                        this.Status = (int)StatusOsobyEnum.Politik;
                        return true;
                    }
                    if (this.IsSponzor() == false && this.MaVztahySeStatem() == false)
                    {
                        this.Status = (int)StatusOsobyEnum.NeniPolitik;
                        return true;
                    }
                    break;
                case StatusOsobyEnum.Politik:
                    bool chgnd = false;
                    if (IsPolitikBasedOnEvents() == false)
                    {
                        this.Status = (int)StatusOsobyEnum.NeniPolitik;
                        chgnd = true;
                    }
                    if (chgnd && this.IsSponzor() == false && this.MaVztahySeStatem() == false)
                    {
                        this.Status = (int)StatusOsobyEnum.NeniPolitik;
                        chgnd = true;
                    }
                    else
                    {
                        this.Status = (int)StatusOsobyEnum.Politik;
                        chgnd = false;

                    }
                    return chgnd;
                default:
                    break;
            }
            return false;
        }
        public bool IsSponzor()
        {
            return this.Events(m => m.Type == (int)OsobaEvent.Types.Sponzor).Any();
        }
        public IEnumerable<OsobaEvent> Sponzoring()
        {
            return this.Events(m => m.Type == (int)OsobaEvent.Types.Sponzor);
        }

        public IEnumerable<OsobaEvent> Events()
        {
            return Events(m => true);
        }


        private static DateTime minBigSponzoringDate = new DateTime(DateTime.Now.Year - 10, 1, 1);
        private static DateTime minSmallSponzoringDate = new DateTime(DateTime.Now.Year - 5, 1, 1);
        private static decimal smallSponzoringThreshold = 10000;

        public static Expression<Func<OsobaEvent, bool>> _sponzoringLimitsPredicate = m =>
                             (m.Type != (int)OsobaEvent.Types.Sponzor
                                ||
                                (m.Type == (int)OsobaEvent.Types.Sponzor
                                && (
                                    (m.AddInfoNum > smallSponzoringThreshold && m.DatumOd >= minBigSponzoringDate)
                                    || (m.AddInfoNum <= smallSponzoringThreshold && m.DatumOd >= minSmallSponzoringDate)
                                    )
                                )
                            );

        public IEnumerable<SocialContact> GetSocialContact()
        {
            return this.Events(oe => oe.Type == (int)OsobaEvent.Types.SocialniSite)
                .Select(oe => new SocialContact
                {
                    Service = oe.Organizace,
                    Contact = oe.AddInfo
                });
        }

        public bool MaVztahySeStatem()
        {
            var ret = this.IsSponzor();
            if (ret) return ret;

            ret = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch("osobaid:" + this.NameId, 1, 1, 0).Total > 0;
            if (ret) return ret;

            ret = HlidacStatu.Lib.Data.VZ.VerejnaZakazka.Searching.SimpleSearch("osobaid:" + this.NameId, null, 1, 1, "0").Total > 0;
            if (ret) return ret;

            ret = new HlidacStatu.Lib.Data.Dotace.DotaceService().SimpleSearch("osobaid:" + this.NameId, 1, 1, "0").Total > 0;
            return ret;

        }

        public string CurrentPoliticalParty()
        {
            return Events(ev =>
                    ev.Type == (int)OsobaEvent.Types.Politicka
                    && (ev.AddInfo == "člen strany"
                        || ev.AddInfo == "předseda strany"
                        || ev.AddInfo == "předsedkyně strany"
                        || ev.AddInfo == "místopředseda strany"
                        || ev.AddInfo == "místopředsedkyně strany")
                    && (!ev.DatumDo.HasValue
                        || ev.DatumDo >= DateTime.Now)
                    )
                .OrderByDescending(ev => ev.DatumOd)
                .Select(ev => ev.Organizace)
                .FirstOrDefault();
        }


        public void FlushCache()
        {
            Osoby.CachedEvents.Delete(this.InternalId);
            Osoby.CachedFirmySponzoring.Delete(this.InternalId);
        }

        public IQueryable<OsobaEvent> NoFilteredEvents()
        {
            return NoFilteredEvents(m => true);
        }
        public IQueryable<OsobaEvent> NoFilteredEvents(Expression<Func<OsobaEvent, bool>> predicate)
        {
            IQueryable<OsobaEvent> oe = Osoby.CachedEvents.Get(this.InternalId)
                .AsQueryable();
            return oe.Where(predicate);
        }

        public IEnumerable<OsobaEvent> Events(Expression<Func<OsobaEvent, bool>> predicate)
        {
            List<OsobaEvent> events = this.NoFilteredEvents()
                                    .Where(_sponzoringLimitsPredicate)
                                    .Where(predicate)
                                    .ToList();


            using (DbEntities db = new DbEntities())
            {

                //sponzoring z navazanych firem kdyz byl statutar
                IEnumerable<OsobaEvent> firmySponzoring = Osoby.CachedFirmySponzoring.Get(this.InternalId)
                    .AsQueryable()
                    .Where(_sponzoringLimitsPredicate)
                    .Where(predicate)
                    .ToArray()
                    ;
                events.AddRange(firmySponzoring);
            }

            return events;
        }

        public static int[] VerejnopravniUdalosti = new int[] {
                (int)OsobaEvent.Types.VolenaFunkce,
                (int)OsobaEvent.Types.PolitickaPracovni,
                (int)OsobaEvent.Types.Politicka,
                (int)OsobaEvent.Types.VerejnaSpravaJine,
                (int)OsobaEvent.Types.VerejnaSpravaPracovni,
                (int)OsobaEvent.Types.Osobni,
                (int)OsobaEvent.Types.Jine
            };

        public IEnumerable<OsobaEvent> Events_VerejnopravniUdalosti()
        {
            return Events_VerejnopravniUdalosti(e => true);
        }
        public IEnumerable<OsobaEvent> Events_VerejnopravniUdalosti(Expression<Func<OsobaEvent, bool>> predicate)
        {
            return Events(predicate)
                .Where(e => VerejnopravniUdalosti.Contains(e.Type));
        }


        public string Description(bool html, string template = "{0}", string itemTemplate = "{0}", string itemDelimeter = "<br/>")
        {
            return Description(html, m => true, int.MaxValue, template, itemTemplate, itemDelimeter);
        }
        public string Description(bool html, Expression<Func<OsobaEvent, bool>> predicate,
            int numOfRecords = int.MaxValue, string template = "{0}",
            string itemTemplate = "{0}", string itemDelimeter = "<br/>")
        {
            var fixedOrder = new List<int>() {
                (int)OsobaEvent.Types.VolenaFunkce,
                (int)OsobaEvent.Types.PolitickaPracovni,
                (int)OsobaEvent.Types.Politicka,
                (int)OsobaEvent.Types.VerejnaSpravaJine,
                (int)OsobaEvent.Types.VerejnaSpravaPracovni,
                (int)OsobaEvent.Types.Osobni,
                (int)OsobaEvent.Types.Jine
            };

            var events = this.Events(predicate);
            if (events.Count() == 0)
                return string.Empty;
            else
            {
                List<string> evs = events
                    .OrderBy(o =>
                    {
                        var index = fixedOrder.IndexOf(o.Type);
                        return index == -1 ? int.MaxValue : index;
                    })
                    .ThenByDescending(o => o.DatumOd)
                    .Take(numOfRecords)
                    .Select(e => html ? e.RenderHtml(", ") : e.RenderText(" ")).ToList();

                if (html)
                {
                    return string.Format(template,
                         evs.Aggregate((f, s) => f + itemDelimeter + s)
                        );
                }
                else
                {
                    return string.Format(template,
                        evs.Aggregate((f, s) => f + itemDelimeter + s)
                        );
                }
            }
        }

        public void Delete(string user)
        {
            var tmpOsoba = Osoba.GetByInternalId(this.InternalId);
            tmpOsoba.FlushCache();
            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {
                db.Osoba.Attach(tmpOsoba);
                db.Entry(tmpOsoba).State = System.Data.Entity.EntityState.Deleted;

                foreach (var oe in tmpOsoba.NoFilteredEvents())
                {
                    oe.Delete(user);
                }
                Audit.Add<Osoba>(Audit.Operations.Delete, user, tmpOsoba, null);
                db.SaveChanges();
            }
        }


        public static Osoba GetOrCreateNew(string titulPred, string jmeno, string prijmeni, string titulPo,
            string narozeni, StatusOsobyEnum status, string user
        )
        {
            return GetOrCreateNew(titulPred, jmeno, prijmeni, titulPo, Devmasters.DT.Util.ToDate(narozeni), status, user);
        }
        public static Osoba GetOrCreateNew(string titulPred, string jmeno, string prijmeni, string titulPo,
            DateTime? narozeni, StatusOsobyEnum status, string user, DateTime? umrti = null)
        {
            var p = new Data.Osoba();
            p.TitulPred = NormalizeTitul(titulPred, true);
            p.TitulPo = NormalizeTitul(titulPo, false);
            p.Jmeno = NormalizeJmeno(jmeno);
            p.Prijmeni = NormalizePrijmeni(prijmeni);

            if (narozeni.HasValue == false)
            {
                p.Umrti = umrti;
                p.Status = (int)status;
                p.Save();
                Audit.Add(Audit.Operations.Create, user, p, null);
                return p;
            }
            var exiO = Osoba.Searching.GetByName(p.Jmeno, p.Prijmeni, narozeni.Value);


            if (exiO == null)
            {
                p.Umrti = umrti;
                p.Status = (int)status;
                p.Narozeni = narozeni;
                p.Save();
                Audit.Add(Audit.Operations.Create, user, p, null);
                return p;
            }
            else
            {
                return exiO;
            }

        }

        public Osoba MergeWith(Osoba duplicated, string user)
        {
            if (this.InternalId == duplicated.InternalId)
                return this;

            OsobaEvent[] dEvs = duplicated.NoFilteredEvents().ToArray();
            OsobaExternalId[] dEids = duplicated.ExternalIds().Where(m => m.ExternalSource != (int)OsobaExternalId.Source.HlidacSmluvGuid).ToArray();
            using (DbEntities db = new Data.DbEntities())
            {

                foreach (var dEv in dEvs)
                {
                    //check duplicates
                    var exists = this.NoFilteredEvents()
                        .Any(m =>
                        m.OsobaId == this.InternalId
                        && m.Type == dEv.Type
                        && m.AddInfo == dEv.AddInfo
                        && m.Organizace == dEv.Organizace
                        && m.AddInfoNum == dEv.AddInfoNum
                        && m.DatumOd == dEv.DatumOd
                        && m.DatumDo == dEv.DatumDo
                        && m.Status == dEv.Status
                        );
                    if (exists == false)
                    {
                        this.AddOrUpdateEvent(dEv, user, checkDuplicates: false);
                    }
                }
            }
            List<OsobaExternalId> addExternalIds = new List<OsobaExternalId>();
            foreach (var dEid in dEids)
            {
                bool exists = false;
                foreach (var eid in this.ExternalIds())
                {
                    exists = exists || (eid.ExternalId == dEid.ExternalId && eid.ExternalSource == dEid.ExternalSource && eid.OsobaId == dEid.OsobaId);
                }
                if (!exists)
                    addExternalIds.Add(dEid);
            }

            if (string.IsNullOrEmpty(this.TitulPred) && !string.IsNullOrEmpty(duplicated.TitulPred))
                this.TitulPred = duplicated.TitulPred;
            if (string.IsNullOrEmpty(this.Jmeno) && !string.IsNullOrEmpty(duplicated.Jmeno))
                this.Jmeno = duplicated.Jmeno;
            if (string.IsNullOrEmpty(this.Prijmeni) && !string.IsNullOrEmpty(duplicated.Prijmeni))
                this.Prijmeni = duplicated.Prijmeni;
            if (string.IsNullOrEmpty(this.TitulPo) && !string.IsNullOrEmpty(duplicated.TitulPo))
                this.TitulPo = duplicated.TitulPo;
            if (string.IsNullOrEmpty(this.Pohlavi) && !string.IsNullOrEmpty(duplicated.Pohlavi))
                this.Pohlavi = duplicated.Pohlavi;
            if (string.IsNullOrEmpty(this.Ulice) && !string.IsNullOrEmpty(duplicated.Ulice))
                this.Ulice = duplicated.Ulice;
            if (string.IsNullOrEmpty(this.Mesto) && !string.IsNullOrEmpty(duplicated.Mesto))
                this.Mesto = duplicated.Mesto;
            if (string.IsNullOrEmpty(this.PSC) && !string.IsNullOrEmpty(duplicated.PSC))
                this.PSC = duplicated.PSC;
            if (string.IsNullOrEmpty(this.CountryCode) && !string.IsNullOrEmpty(duplicated.CountryCode))
                this.CountryCode = duplicated.CountryCode;
            if (string.IsNullOrEmpty(this.PuvodniPrijmeni) && !string.IsNullOrEmpty(duplicated.PuvodniPrijmeni))
                this.PuvodniPrijmeni = duplicated.PuvodniPrijmeni;

            if (!this.Narozeni.HasValue && duplicated.Narozeni.HasValue)
                this.Narozeni = duplicated.Narozeni;
            if (!this.Umrti.HasValue && duplicated.Umrti.HasValue)
                this.Umrti = duplicated.Umrti;
            if (!this.OnRadar && duplicated.OnRadar)
                this.OnRadar = duplicated.OnRadar;

            if (this.Status != (int)Osoba.StatusOsobyEnum.Politik
                && this.Status < duplicated.Status)
                this.Status = duplicated.Status;

            if (string.IsNullOrWhiteSpace(this.WikiId)
                && !string.IsNullOrWhiteSpace(duplicated.WikiId))
                this.WikiId = duplicated.WikiId;

            //obrazek
            if (this.HasPhoto() == false && duplicated.HasPhoto())
            {
                foreach (var fn in new string[] { "small.jpg", "source.txt", "original.uploaded.jpg", "small.uploaded.jpg" })
                {
                    var from = Lib.Init.OsobaFotky.GetFullPath(duplicated, fn);
                    var to = Lib.Init.OsobaFotky.GetFullPath(this, fn);
                    if (System.IO.File.Exists(from))
                        System.IO.File.Copy(from, to);
                }
            }
            this.Save(addExternalIds.ToArray());

            if (duplicated.InternalId != 0)
                duplicated.Delete(user);

            return this;
        }

        //Napojení na graf
        //Graph.Shortest.EdgePath shortestGraph = null;
        #region shortest path
        private Graphs2.UnweightedGraph _graph = null;
        private Graphs2.Vertex<string> _startingVertex = null; //not for other use except as a search starting point

        public Graph.Edge[] VazbyProICO(string ico)
        {
            return _vazbyProIcoCache.Get((this, ico));
        }
        private Graph.Edge[] _vazbyProICO(string ico)
        {
            if (_graph is null || _graph.Vertices.Count == 0)
                InitializeGraph();

            if (_startingVertex is null)
                _startingVertex = new Graphs2.Vertex<string>("p-" + this.InternalId);

            try
            {
                var shortestPath = _graph.ShortestPath(_startingVertex, CreateVertexFromIco(ico));
                var result = shortestPath.Select(x => ((Graphs2.Edge<Graph.Edge>)x).BindingPayload).ToArray();
                return result; // shortestGraph.ShortestTo(ico).ToArray();
            }
            catch (Exception e)
            {
                Util.Consts.Logger.Error("Vazby ERROR for " + this.NameId, e);
                return Array.Empty<Graph.Edge>();
            }
        }
        
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

        private static Graphs2.Vertex<string> CreateVertexFromIco(string ico)
        {
            return new Graphs2.Vertex<string>($"c-{ico}");
        }
        #endregion

        object lockObj = new object();
        void updateVazby(bool refresh = false)
        {
            lock (lockObj)
            {
                try
                {
                    _vazby = Lib.Data.Graph.VsechnyDcerineVazby(this, refresh)
                        .ToArray();
                }
                catch (Exception)
                {
                    _vazby = new Graph.Edge[] { };
                }
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

        public Osoba.Statistics.RegistrSmluv StatistikaRegistrSmluv(Relation.AktualnostType minAktualnost, int? obor=null)
        {
            return Osoba.Statistics.CachedStatistics(this,minAktualnost,obor);
        }

        public Osoba Vazby(Graph.Edge[] vazby)
        {
            if (vazby == null)
                this._vazby = new Graph.Edge[] { };

            this._vazby = vazby;
            return this;
        }

        public Graph.Edge[] AktualniVazby(Relation.AktualnostType minAktualnost)
        {
            return Relation.AktualniVazby(this.Vazby(), minAktualnost);
        }
        public string NarozeniYear(bool html = false)
        {
            string result = "";
            if (this.Narozeni.HasValue || this.Umrti.HasValue)
            {
                string narozeni = this.Narozeni?.ToString("*yyyy") ?? "* ???";
                string umrti = this.Umrti?.ToString(" - ✝yyyy") ?? "";
                result = $" ({narozeni}{umrti})";
            }
            if (html)
                result = result.Replace(" ", "&nbsp;");
            return result;
        }

        public string NarozeniUmrtiFull(bool html = false)
        {
            string result = "";
            if (this.Narozeni.HasValue || this.Umrti.HasValue)
            {
                string narozeni = this.Narozeni?.ToString("*dd.MM.yyyy") ?? "* ???";
                string umrti = this.Umrti?.ToString("- ✝dd.MM.yyyy") ?? "";
                result = $" ({narozeni} {umrti})";
            }
            if (html)
                result = result.Replace(" ", "&nbsp;");
            return result;
        }

        public string FullNameToQuery(bool exact = true)
        {
            if (exact)
            {
                List<string> parts = new List<string>();
                parts.AddRange(this.Jmeno.Split(' '));
                parts.AddRange(this.Prijmeni.Split(' '));
                string q = $" ( {string.Join(" ", parts.Select(m => m + "~0"))} ) ";
                return q;
            }
            else
                return $"(\"{this.FullName()}\")";
        }

        public string FullName(bool html = false)
        {
            string ret = string.Format("{0} {1} {2}{3}", this.TitulPred, this.Jmeno, this.Prijmeni, string.IsNullOrEmpty(this.TitulPo) ? "" : ", " + this.TitulPo).Trim();
            if (html)
                ret = ret.Replace(" ", "&nbsp;");
            return ret;
        }

        public string FullNameWithYear(bool html = false)
        {
            var s = this.FullName(html);
            if (this.Narozeni.HasValue)
                s = s + NarozeniYear(html);
            if (html)
                s = s.Replace(" ", "&nbsp;");
            return s;
        }

        public string ShortName()
        {
            var f = this.Jmeno.FirstOrDefault();
            if (f == default(char))
                return this.Prijmeni;
            else
                return f + ". " + this.Prijmeni;
        }

        public string Inicialy()
        {
            return this.Jmeno.FirstOrDefault() + "" + this.Prijmeni.FirstOrDefault();
        }
        public string FullNameWithNarozeni(bool html = false)
        {
            var s = this.FullName(html);
            if (this.Narozeni.HasValue)
            {
                s = s + string.Format(" ({0}) ", this.Narozeni.Value.ToShortDateString());
            }
            if (html)
                s = s.Replace(" ", "&nbsp;");
            return s;
        }


        public static Osoba GetByNameId(string nameId)
        {
            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {
                return db.Osoba
                .Where(m =>
                    m.NameId == nameId
                )
                .FirstOrDefault();
            }
        }

        public static Osoba GetByInternalId(int id)
        {
            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {
                return db.Osoba
                .Where(m =>
                    m.InternalId == id
                )
                .FirstOrDefault();
            }
        }

        public static Osoba GetByTransactionId(Data.TransparentniUcty.BankovniPolozka transaction)
        {
            if (transaction == null)
                return null;
            using (DbEntities db = new Data.DbEntities())
            {
                string url = transaction.GetUrl(local: false);
                var found = db.OsobaEvent
                            .Where(m => m.Zdroj == url)
                            .FirstOrDefault();
                if (found != null)
                {
                    return GetByInternalId(found.OsobaId);
                }

            }
            return null;
        }
        public string PohlaviCalculated()
        {
            var sex = new Lang.CS.Vokativ(this.FullName()).Sex;
            switch (sex)
            {
                case Lang.CS.Vokativ.SexEnum.Woman:
                    return "f";
                case Lang.CS.Vokativ.SexEnum.Man:
                    return "m";
                case Lang.CS.Vokativ.SexEnum.Unknown:
                default:
                    return "";
            }
        }

        public Osoba Save(params OsobaExternalId[] externalIds)
        {

            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {

                this.JmenoAscii = Devmasters.TextUtil.RemoveDiacritics(this.Jmeno);
                this.PrijmeniAscii = Devmasters.TextUtil.RemoveDiacritics(this.Prijmeni);
                this.PuvodniPrijmeniAscii = Devmasters.TextUtil.RemoveDiacritics(this.PuvodniPrijmeni);

                if (string.IsNullOrEmpty(this.NameId))
                {
                    this.NameId = GetUniqueNamedId();
                }

                this.LastUpdate = DateTime.Now;

                if (this.Pohlavi != "f" || this.Pohlavi != "m")
                {
                    var sex = PohlaviCalculated();

                }

                db.Osoba.Attach(this);


                if (this.InternalId == 0)
                {
                    db.Entry(this).State = System.Data.Entity.EntityState.Added;
                }
                else
                    db.Entry(this).State = System.Data.Entity.EntityState.Modified;
                try
                {
                    db.SaveChanges();

                }
                catch (Exception e)
                {
                    Console.Write(e.ToString());
                }
                if (externalIds != null)
                {
                    foreach (var ex in externalIds)
                    {
                        ex.OsobaId = this.InternalId;
                        OsobaExternalId.Add(ex);
                    }
                }
            }
            return this;
        }


        public string GetUniqueNamedId()
        {
            if (string.IsNullOrWhiteSpace(this.JmenoAscii) || string.IsNullOrWhiteSpace(this.PrijmeniAscii))
                return "";
            if (!char.IsLetter(this.JmenoAscii[0]) || !char.IsLetter(this.PrijmeniAscii[0]))
                return "";

            string basic = Devmasters.TextUtil.ShortenText(this.JmenoAscii, 23) + "-" + Devmasters.TextUtil.ShortenText(this.PrijmeniAscii, 23).Trim();
            basic = basic.ToLowerInvariant().NormalizeToPureTextLower();
            basic = Devmasters.TextUtil.ReplaceDuplicates(basic, ' ').Trim();
            basic = basic.Replace(" ", "-");
            Osoba exists = null;
            int num = 0;
            string checkUniqueName = basic;
            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {
                do
                {
                    exists = db.Osoba
                        .Where(m => m.NameId.StartsWith(checkUniqueName))
                        .OrderByDescending(m => m.NameId)
                        .FirstOrDefault();
                    if (exists != null)
                    {
                        System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"(?<num>\d{1,})$", System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace);
                        var m = r.Match(exists.NameId);
                        if (m.Success)
                        {
                            num = Convert.ToInt32(m.Groups["num"].Value);
                        }
                        num++;
                        checkUniqueName = basic + "-" + num.ToString();
                    }
                    else
                        return checkUniqueName;
                } while (exists != null);
            }

            return basic;
        }

        public string GetPhotoPath()
        {
            if (HasPhoto())
            {
                var path = Lib.Init.OsobaFotky.GetFullPath(this, "small.jpg");
                return path;
            }
            else
                return Lib.Init.WebAppRoot + @"Content\Img\personNoPhoto.png";
        }
        public bool HasPhoto()
        {
            var path = Lib.Init.OsobaFotky.GetFullPath(this, "small.jpg");
            return System.IO.File.Exists(path);
        }
        public string GetPhotoSource()
        {
            var fn = HlidacStatu.Lib.Init.OsobaFotky.GetFullPath(this, "source.txt");
            if (System.IO.File.Exists(fn))
            {
                try
                {
                    var source = System.IO.File.ReadAllText(fn)?.Trim() ?? "";
                    if (!string.IsNullOrEmpty(source) && Uri.TryCreate(source, UriKind.Absolute, out var url))
                    {
                        return source;
                    }

                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }
        public string GetPhotoUrl(bool local = false)
        {
            if (local)
            {
                return "/Photo/" + this.NameId;
                //if (HasPhoto())
                //    return "/Photo/" + this.NameId;
                //else
                //    return "/Content/Img/personNoPhoto.png";
            }
            else
            {
                return "https://www.hlidacstatu.cz/Photo/" + this.NameId;
                //if (HasPhoto())
                //    return "https://www.hlidacstatu.cz/Photo/" + this.NameId;
                //else
                //    return "https://www.hlidacstatu.cz/Content/Img/personNoPhoto.png";
            }
        }
        public OsobaEvent AddDescription(string text, string strana, string zdroj, string user, bool deletePrevious = false)
        {
            if (deletePrevious)
            {
                var oes = this.Events(m => m.Type == (int)OsobaEvent.Types.Specialni);
                foreach (var o in oes)
                {
                    o.Delete(user);
                }
            }
            OsobaEvent oe = new OsobaEvent(this.InternalId, "", text, OsobaEvent.Types.Specialni);
            oe.Organizace = ParseTools.NormalizaceStranaShortName(strana);
            oe.Zdroj = zdroj;
            return AddOrUpdateEvent(oe, user);
        }

        public OsobaEvent AddFunkce(string pozice, string strana, int rokOd, int? rokDo, string zdroj, string user)
        {
            OsobaEvent oe = new OsobaEvent(this.InternalId, string.Format("{0}", pozice), "", OsobaEvent.Types.PolitickaPracovni);
            oe.Organizace = ParseTools.NormalizaceStranaShortName(strana);
            oe.Zdroj = zdroj;
            oe.DatumOd = new DateTime(rokOd, 1, 1, 0, 0, 0, DateTimeKind.Local);
            oe.DatumDo = rokDo == null ? (DateTime?)null : new DateTime(rokDo.Value, 12, 31, 0, 0, 0, DateTimeKind.Local);
            return AddOrUpdateEvent(oe, user);
        }

        public OsobaEvent AddClenStrany(string strana, int rokOd, int? rokDo, string zdroj, string user)
        {
            OsobaEvent oe = new OsobaEvent(this.InternalId, string.Format("Člen strany {0}", strana), "", OsobaEvent.Types.Politicka);
            oe.Organizace = ParseTools.NormalizaceStranaShortName(strana);
            oe.AddInfo = "člen strany";
            oe.Zdroj = zdroj;
            oe.DatumOd = new DateTime(rokOd, 1, 1, 0, 0, 0, DateTimeKind.Local);
            oe.DatumDo = rokDo == null ? (DateTime?)null : new DateTime(rokDo.Value, 12, 31, 0, 0, 0, DateTimeKind.Local);
            return AddOrUpdateEvent(oe, user);
        }
        public OsobaEvent AddOrUpdateEvent(OsobaEvent ev, string user, bool rewrite = false, bool checkDuplicates = true)
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
                OsobaEvent exists = null;
                if (ev.pk > 0)
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
                                        m.OsobaId == this.InternalId
                                        && m.Type == ev.Type
                                        && m.AddInfo == ev.AddInfo
                                        && m.Organizace == ev.Organizace
                                        && m.AddInfoNum == ev.AddInfoNum
                                        && m.DatumOd == ev.DatumOd
                                        && m.DatumDo == ev.DatumDo
                                //&& m.Status == ev.Status
                                //&& m.Title == ev.Title
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
                    ev.OsobaId = this.InternalId;
                    db.OsobaEvent.Add(ev);
                    Audit.Add(Audit.Operations.Create, user, ev, null);
                    db.SaveChanges();
                    return ev;
                }
                else
                {
                    ev.OsobaId = this.InternalId;
                    ev.pk = exists.pk;
                    db.OsobaEvent.Attach(ev);
                    db.Entry(ev).State = System.Data.Entity.EntityState.Modified;
                    Audit.Add(Audit.Operations.Create, user, ev, exists);
                    db.SaveChanges();
                    return ev;
                }
            }
        }
        public OsobaEvent AddSponsoring(string strana, string stranaico, int rok, decimal castka, string zdroj, string user, bool rewrite = false, bool checkDuplicates = true)
        {
            var t = OsobaEvent.Types.Sponzor;

            // Pokud dobře chápu, tak tohle zaniká
            //if (zdroj?.Contains("https://www.hlidacstatu.cz/ucty/transakce/") == true)
            //    t = OsobaEvent.Types.SponzorZuctu;

            OsobaEvent oe = new OsobaEvent(this.InternalId, string.Format("Sponzor {0}", strana), "", t);
            oe.AddInfoNum = castka;
            oe.Organizace = strana;
            oe.AddInfo = stranaico;
            oe.Zdroj = zdroj;
            oe.SetYearInterval(rok);
            return AddOrUpdateEvent(oe, user, checkDuplicates: checkDuplicates);
        }

        public static Osoba Get(int Id)
        {
            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {
                return db.Osoba.Where(m => m.InternalId == Id).AsNoTracking().FirstOrDefault();
            }
        }
        public static Osoba GetByExternalID(string exId, OsobaExternalId.Source source)
        {
            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {
                var oei = db.OsobaExternalId.Where(m => m.ExternalId == exId && m.ExternalSource == (int)source).FirstOrDefault();
                if (oei == null)
                    return null;
                else
                    return Get(oei.OsobaId);
            }
        }

        public static List<Osoba> GetByEvent(Expression<Func<OsobaEvent,bool>> predicate)
        {
            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {
                var events = db.OsobaEvent
                    .Where(predicate);

                var people = db.Osoba.Where(o => events.Any(e => e.OsobaId == o.InternalId));

                return people.Distinct().ToList();
            }
        }

        public static void SetManualTimeStamp(int osobaId, string author)
        {
            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {
                var osobaToUpdate = db.Osoba
                .Where(m =>
                    m.InternalId == osobaId
                ).FirstOrDefault();

                if (osobaToUpdate != null)
                {
                    osobaToUpdate.ManuallyUpdated = DateTime.Now;
                    osobaToUpdate.ManuallyUpdatedBy = author;
                    db.SaveChanges();
                }
            }
        }

        public static Osoba Update(Osoba osoba, string user)
        {
            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {
                var osobaToUpdate = db.Osoba
                .Where(m =>
                    m.InternalId == osoba.InternalId
                ).FirstOrDefault();

                var osobaOriginal = osobaToUpdate.ShallowCopy();

                if (osobaToUpdate != null)
                {
                    osobaToUpdate.Jmeno = osoba.Jmeno;
                    osobaToUpdate.Prijmeni = osoba.Prijmeni;
                    osobaToUpdate.TitulPo = osoba.TitulPo;
                    osobaToUpdate.TitulPred = osoba.TitulPred;
                    osobaToUpdate.Narozeni = osoba.Narozeni;
                    osobaToUpdate.Status = osoba.Status;
                    osobaToUpdate.Umrti = osoba.Umrti;

                    osobaToUpdate.JmenoAscii = Devmasters.TextUtil.RemoveDiacritics(osoba.Jmeno);
                    osobaToUpdate.PrijmeniAscii = Devmasters.TextUtil.RemoveDiacritics(osoba.Prijmeni);
                    osobaToUpdate.PuvodniPrijmeniAscii = Devmasters.TextUtil.RemoveDiacritics(osoba.PuvodniPrijmeni);
                    osobaToUpdate.LastUpdate = DateTime.Now;

                    if (!string.IsNullOrWhiteSpace(osoba.WikiId))
                        osobaToUpdate.WikiId = osoba.WikiId;

                    db.SaveChanges();
                    Audit.Add(Audit.Operations.Update, user, osobaToUpdate, osobaOriginal);

                    return osobaToUpdate;
                }
            }
            return osoba;
        }

        public Osoba ShallowCopy()
        {
            return (Osoba)this.MemberwiseClone();
        }

        public static string Capitalize(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            //return Regex.Replace(s, @"\b(\w)", m => m.Value.ToUpper());
            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s);

        }

        public static string NormalizeJmeno(string s)
        {
            return Capitalize(s?.Trim());
        }
        public static string NormalizePrijmeni(string s)
        {
            return Capitalize(s?.Trim());
        }

        public static string NormalizeTitul(string s, bool pred)
        {
            return s?.Trim();
        }


        static System.Text.RegularExpressions.RegexOptions defaultRegexOptions = ((System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace | System.Text.RegularExpressions.RegexOptions.Multiline)
| System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        public static string JmenoBezTitulu(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            string modifNazev = name.Trim().ToLower();
            foreach (var k in TitulyPo.Concat(TitulyPred))
            {
                string kRegex = @"(\b|^)" + System.Text.RegularExpressions.Regex.Escape(k) + @"(\b|$)";


                System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(modifNazev, kRegex, defaultRegexOptions);
                if (m.Success)
                    modifNazev = System.Text.RegularExpressions.Regex.Replace(modifNazev, kRegex, "", defaultRegexOptions);

            }
            modifNazev = System.Text.RegularExpressions.Regex
                .Replace(modifNazev, "[^a-z-. ěščřžýáíéůúďňťĚŠČŘŽÝÁÍÉŮÚĎŇŤÄäÖöÜüßÀàÂâÆæÇçÈèÊêËëÎîÏïÔôŒœÙùÛûÜüŸÿ]", "", defaultRegexOptions)
                .Trim();


            return Capitalize(modifNazev);

        }

        InfoFact[] _infofacts = null;
        object lockInfoObj = new object();
        public InfoFact[] InfoFacts()
        {
            lock (lockInfoObj)
            {
                int[] types = {
                        (int)HlidacStatu.Lib.Data.OsobaEvent.Types.VolenaFunkce,
                        (int)HlidacStatu.Lib.Data.OsobaEvent.Types.PolitickaPracovni,
                        (int)HlidacStatu.Lib.Data.OsobaEvent.Types.Politicka,
                        (int)HlidacStatu.Lib.Data.OsobaEvent.Types.VerejnaSpravaJine,
                        (int)HlidacStatu.Lib.Data.OsobaEvent.Types.VerejnaSpravaPracovni,
                        (int)HlidacStatu.Lib.Data.OsobaEvent.Types.Osobni,
                        (int)HlidacStatu.Lib.Data.OsobaEvent.Types.Jine
                    };

                if (_infofacts == null)
                {
                    List<InfoFact> f = new List<InfoFact>();
                    var stat = this.StatistikaRegistrSmluv(Relation.AktualnostType.Nedavny);
                    StatisticsPerYear<Smlouva.Statistics.Data> soukrStat = stat.SoukromeFirmy.Values.AggregateStats(); //StatisticsSubjectPerYear<Smlouva.Statistics.Data>.

                    int rok = DateTime.Now.Year;
                    if (DateTime.Now.Month <= 2)
                        rok = rok - 1;
                    var kdoje = this.Description(false,
                           m => types.Contains(m.Type),
                           2, itemDelimeter: ", ");

                    var descr = "";
                    if (this.NotInterestingToShow())
                        descr = $"<b>{this.FullName()}</b>";
                    else
                        descr = $"<b>{this.FullNameWithYear()}</b>";
                    if (!string.IsNullOrEmpty(kdoje))
                        descr += ", " + kdoje + (kdoje.EndsWith(". ") ? "" : ". ");
                    f.Add(new InfoFact(descr, InfoFact.ImportanceLevel.Summary));

                    var statDesc = "";
                    if (stat.StatniFirmy.Count > 0)
                        statDesc += $"Angažoval se v {Devmasters.Lang.Plural.Get(stat.StatniFirmy.Count, "jedné státní firmě", "{0} státních firmách", "{0} státních firmách")}. ";
                    //neziskovky
                    if (stat.SoukromeFirmy.Count > 0)
                    {
                        //ostatni
                        statDesc += $"Angažoval se {(stat.StatniFirmy.Count > 0 ? "také" : "")} v ";
                        if (stat.NeziskovkyCount() > 0 && stat.KomercniFirmyCount() == 0)
                        {
                            statDesc += $"{Devmasters.Lang.Plural.Get(stat.NeziskovkyCount(), "jedné neziskové organizaci", "{0} neziskových organizacích", "{0} neziskových organizacích")}";
                        }
                        else if (stat.NeziskovkyCount() > 0)
                        {
                            statDesc += $"{Devmasters.Lang.Plural.Get(stat.NeziskovkyCount(), "jedné neziskové organizaci", "{0} neziskových organizacích", "{0} neziskových organizacích")}";
                            statDesc += $" a {Devmasters.Lang.Plural.Get(stat.KomercniFirmyCount(), "jedné soukr.firmě", "{0} soukr.firmách", "{0} soukr.firmách")}";
                        }
                        else
                        {
                            statDesc += $"{Devmasters.Lang.Plural.Get(stat.SoukromeFirmy.Count, "jedné soukr.firmě", "{0} soukr.firmách", "{0} soukr.firmách")}";
                        }


                        statDesc += $". Tyto subjekty mají se státem od 2016 celkem "
                            + Devmasters.Lang.Plural.Get(soukrStat.Sum(m=>m.PocetSmluv), "jednu smlouvu", "{0} smlouvy", "{0} smluv")
                            + " v celkové výši " + HlidacStatu.Lib.Data.Smlouva.NicePrice(soukrStat.Sum(m => m.CelkovaHodnotaSmluv), html: true, shortFormat: true)
                            + ". ";
                    }

                    if (statDesc.Length > 0)
                        f.Add(new InfoFact(statDesc, InfoFact.ImportanceLevel.Stat));

                    DateTime datumOd = new DateTime(DateTime.Now.Year - 10, 1, 1);
                    if (this.IsSponzor() && this.Sponzoring().Where(m => m.DatumOd >= datumOd).Count() > 0)
                    {
                        var sponzoring = this.Sponzoring().Where(m => m.DatumOd >= datumOd).ToArray();
                        string[] strany = sponzoring.Select(m => m.Organizace).Distinct().ToArray();
                        DateTime[] roky = sponzoring.Select(m => m.DatumOd.Value).Distinct().OrderBy(y => y).ToArray();
                        decimal celkem = sponzoring.Sum(m => m.AddInfoNum) ?? 0;
                        decimal top = sponzoring.Max(m => m.AddInfoNum) ?? 0;

                        f.Add(new InfoFact($"{this.FullName()} "
                            + Devmasters.Lang.Plural.Get(roky.Count(), "v roce " + roky[0].Year, $"mezi roky {roky.First().Year} - {roky.Last().Year - 2000}", $"mezi roky {roky.First().Year} - {roky.Last().Year - 2000}")
                            + $" sponzoroval{(this.Muz() ? "" : "a")} " +
                            Devmasters.Lang.Plural.Get(strany.Length, "stranu " + strany[0], "{0} polit. strany", "{0} polit. stran")
                            + $" v&nbsp;celkové výši <b>{HlidacStatu.Util.RenderData.ShortNicePrice(celkem, html: true)}</b>. "
                            + $"Nejvyšší sponzorský dar byl ve výši {RenderData.ShortNicePrice(top, html: true)}. "
                            , InfoFact.ImportanceLevel.Medium)
                            );

                    }
                    bool zena = this.Pohlavi == "f";
                    if (soukrStat.Sum(m=>m.PocetSmluv)> 0)
                    {
                        if (soukrStat[rok].PocetSmluv > 0)
                        {
                            string ss = "";
                            //neziskovky a firmy
                            //string typy = "";
                            //if (ostat.NeziskovkyCount() > 0 && ostat.KomercniFirmyCount())
                            //{
                            //    typy = Devmasters.Lang.Plural.Get(ostat.SoukromeFirmy.Count,
                            //}
                            
                            if (soukrStat[rok].CelkovaHodnotaSmluv == 0)
                                ss = Devmasters.Lang.Plural.Get(stat.SoukromeFirmy.Count(m=>m.Value[rok].PocetSmluv>0),
                                        $"Jeden subjekt, ve kterém se angažoval{(this.Muz() ? "" : "a")}, v&nbsp;roce {rok} uzavřel",
                                        $"{{0}} subjekty, ve kterých se angažoval{(this.Muz() ? "" : "a")}, v&nbsp;roce {rok} uzavřely",
                                        $"{{0}} subjektů, ve kterých se angažuval{(this.Muz() ? "" : "a")}, v&nbsp;roce {rok} uzavřely"
                                        )
                                    + $" smlouvy v neznámé výši, protože <b>hodnota všech smluv byla utajena</b>. ";
                            else
                                ss = Devmasters.Lang.Plural.Get(stat.SoukromeFirmy.Count(m => m.Value[rok].PocetSmluv > 0),
                                            $"Jeden subjekt, ve které se angažoval{(this.Muz() ? "" : "a")}, v&nbsp;roce {rok} uzavřel",
                                            $"{{0}} subjekty, ve kterých se angažoval{(this.Muz() ? "" : "a")}, v&nbsp;roce {rok} uzavřely",
                                            $"{{0}} subjektů, ve kterých se angažoval{(this.Muz() ? "" : "a")}, v&nbsp;roce {rok} uzavřely"
                                        )
                                        + $" " +
                                        Devmasters.Lang.Plural.Get(soukrStat[rok].PocetSmluv," jednu smlouvu."," {0} smlouvy"," {0} smluv")
                                        +"</b>. ";

                            f.Add(new InfoFact(ss, InfoFact.ImportanceLevel.Medium));
                        }
                        else if (soukrStat[rok-1].CelkovaHodnotaSmluv == 0)
                        {
                            string ss = "";
                            if (soukrStat[rok].CelkovaHodnotaSmluv== 0)
                                ss = $"Je angažován{(this.Muz() ? "" : "a")} v&nbsp;" +
                                Devmasters.Lang.Plural.Get(stat.SoukromeFirmy.Count(m => m.Value[rok].PocetSmluv > 0),
                                        $"jednom subjektu, která v&nbsp;roce {rok - 1} uzavřela",
                                        $"{{0}} subjektech, které v&nbsp;roce {rok} uzavřely",
                                        $"{{0}} subjektech, které v&nbsp;roce {rok - 1} uzavřely"
                                    )
                                + $" smlouvy v neznámé výši, protože <b>hodnota všech smluv byla utajena</b>. ";
                            else
                                ss = Devmasters.Lang.Plural.Get(stat.SoukromeFirmy.Count(m => m.Value[rok-1].PocetSmluv > 0),
                                            $"Jeden subjekt, ve které se angažoval{(this.Muz() ? "" : "a")}, v&nbsp;roce {rok} uzavřel",
                                            $"{{0}} subjekty, ve kterých se angažoval{(this.Muz() ? "" : "a")}, v&nbsp;roce {rok} uzavřely",
                                            $"{{0}} subjektů, ve kterých se angažoval{(this.Muz() ? "" : "a")}, v&nbsp;roce {rok} uzavřely"
                                        )
                                        + $" " +
                                        Devmasters.Lang.Plural.Get(soukrStat[rok-1].PocetSmluv, " jednu smlouvu.", " {0} smlouvy", " {0} smluv")
                                        + "</b>. ";

                            f.Add(new InfoFact(ss, InfoFact.ImportanceLevel.Medium)
                            );
                        }
                    }
                    _infofacts = f.OrderByDescending(o => o.Level).ToArray();

                }
            }
            return _infofacts;
        }

        //Devmasters.Lang.Vokativ _vokativ = null;


        public bool NotInterestingToShow()
        {
            var res = this.StatusOsoby() == StatusOsobyEnum.NeniPolitik
                    && this.MaVztahySeStatem() == false;

            res = res || (this.StatusOsoby() == StatusOsobyEnum.Sponzor
                    && this.IsSponzor() == false
                );

            return res;
        }

        public string SocialInfoTitle()
        {
            return Devmasters.TextUtil.ShortenText(this.FullName(), 70);
        }
        public string SocialInfoSubTitle()
        {
            return this.NarozeniYear(true) + ", " + this.StatusOsoby().ToNiceDisplayName();
        }

        public string SocialInfoBody()
        {
            return "<ul>"
            + HlidacStatu.Util.InfoFact.RenderInfoFacts(this.InfoFacts(), 4, true, true, "", "<li>{0}</li>", true)
            + "</ul>";
        }
        public string SocialInfoFooter()
        {
            return "Údaje k " + DateTime.Now.ToString("d. M. yyyy");
        }
        public string SocialInfoImageUrl()
        {
            return this.GetPhotoUrl();
        }

        public bool Muz()
        {
            if (string.IsNullOrEmpty(this.Pohlavi))
                this.Pohlavi = PohlaviCalculated();

            return this.Pohlavi == "m";
        }

        public string ToAuditJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public string ToAuditObjectTypeName()
        {
            return "Osoba";
        }

        public string ToAuditObjectId()
        {
            return this.InternalId.ToString();
        }

        public string GetUrl(bool local = true)
        {
            return GetUrl(local, string.Empty);
        }

        public string GetUrl(bool local, string foundWithQuery)
        {

            if (string.IsNullOrEmpty(this.NameId))
                return "";

            string url = "/osoba/" + this.NameId;
            if (!string.IsNullOrEmpty(foundWithQuery))
                url = url + "?qs=" + System.Net.WebUtility.UrlEncode(foundWithQuery);

            if (local == false)
                url = "https://www.hlidacstatu.cz" + url;

            return url;
        }

        public string BookmarkName()
        {
            return this.FullNameWithYear();
        }


        static private MemoryCacheManager<Graph.Edge[], (Osoba o, string ico)> _vazbyProIcoCache
       = MemoryCacheManager<Graph.Edge[], (Osoba o, string ico)>
            .GetSafeInstance("_vazbyOsobaProIcoCache", key => {
                return key.o._vazbyProICO(key.ico);
            } ,
                TimeSpan.FromHours(2)
           );
    }
}
