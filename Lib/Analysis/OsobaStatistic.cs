using HlidacStatu.Util.Cache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Analysis
{


    public class OsobaStatistic
        : ComplexStatistic<string>
    {

        public Dictionary<string, SubjectStatistic> StatniFirmy { get; set; } = new Dictionary<string, SubjectStatistic>();
        public Dictionary<string, SubjectStatistic> SoukromeFirmy { get; set; } = new Dictionary<string, SubjectStatistic>();

        public Data.Relation.AktualnostType Aktualnost { get; set; }

        Lib.Data.Osoba _osoba = null;
        public Lib.Data.Osoba Osoba()
        {
            if (_osoba == null)
            {
                _osoba = Data.Osoby.GetByNameId.Get(this.Item);
            }
            return _osoba;
        }

        static OsobaStatistic Empty(Data.Relation.AktualnostType aktualnost = Data.Relation.AktualnostType.Nedavny)
        {
            OsobaStatistic os = new OsobaStatistic();
            os.Aktualnost = aktualnost;
            os.BasicStatPerYear = BasicDataPerYear.Empty();
            os.RatingPerYear = RatingDataPerYear.Empty();
            return os;
        }

        public OsobaStatistic() { }
        public OsobaStatistic(string nameId, Data.Relation.AktualnostType aktualnost, bool refresh = false)
            : this(Data.Osoby.GetByNameId.Get(nameId), aktualnost, refresh)
        { }

        public OsobaStatistic(Data.Osoba o, Data.Relation.AktualnostType aktualnost, bool refresh = false)
        {
            this.Item = o.NameId;
            this._osoba = o;
            this.Aktualnost = aktualnost;

            osq = new OsobaStatisticQuery() { NameId = this.Item, Aktualnost = this.Aktualnost };

            if (refresh)
                osobaStatcache.Delete(osq);

            this.InitData();
        }
        private OsobaStatisticQuery osq = null;

        protected override BasicDataPerYear getBasicStat() => StatPerYearSoukrome(); 
        protected override RatingDataPerYear getRating() => RatingPerYearSoukrome();

        Tuple<Dictionary<string, SubjectStatistic>, Dictionary<string, SubjectStatistic>> statData = null;
        protected override void InitData()
        {
            statData = osobaStatcache.Get(osq);
            if (statData == null)
            {
                System.Threading.Thread.Sleep(10);
                statData = osobaStatcache.Get(osq);
                //if (statData == null)
                //{
                //    statData = new Tuple<Dictionary<string, SubjectStatistic>, BasicDataPerYear, RatingDataPerYear>();
                //}
                HlidacStatu.Util.Consts.Logger.Error("Osoba statistic cache failed", new ArgumentException($"Osoba statistic cache failed: {this.Item}, {this.Aktualnost}"));

                this.StatniFirmy = new Dictionary<string, SubjectStatistic>();
                this.SoukromeFirmy = new Dictionary<string, SubjectStatistic>();

            }
            else
            {
                this.StatniFirmy = statData.Item1;
                this.SoukromeFirmy = statData.Item2;
            }

            base.InitData();
        }

        BasicDataPerYear _statPerYearSoukrome = null;
        public BasicDataPerYear StatPerYearSoukrome()
        {
            if (_statPerYearSoukrome == null)
            {
                _statPerYearSoukrome = new BasicDataPerYear(this.SoukromeFirmy.Select(m => m.Value.BasicStatPerYear));
            }
            return _statPerYearSoukrome;
        }

        BasicDataPerYear _statPerYearStatni = null;
        public BasicDataPerYear StatPerYearStatni()
        {
            if (_statPerYearStatni == null)
            {
                _statPerYearStatni = new BasicDataPerYear(this.StatniFirmy.Select(m => m.Value.BasicStatPerYear));
            }
            return _statPerYearStatni;
        }

        RatingDataPerYear _ratingPerYearSoukrome = null;
        public RatingDataPerYear RatingPerYearSoukrome()
        {
            if (_ratingPerYearSoukrome == null)
            {
                _ratingPerYearSoukrome = new RatingDataPerYear(this.SoukromeFirmy.Select(m => m.Value.RatingPerYear), this.StatPerYearSoukrome());
            }
            return _ratingPerYearSoukrome;
        }

        RatingDataPerYear _ratingPerYearStatni = null;
        public RatingDataPerYear RatingPerYearStatni()
        {
            if (_ratingPerYearStatni == null)
            {
                _ratingPerYearStatni = new RatingDataPerYear(this.StatniFirmy.Select(m => m.Value.RatingPerYear), this.StatPerYearStatni());
            }
            return _ratingPerYearStatni;
        }

        public void RefreshCache()
        {
            osobaStatcache.Delete(osq);
            this.InitData();
        }

        public override string ToNiceString(bool html = true, string delimiter = " - ", params string[] otherparams)
        {
            return base.ToNiceString(this.Osoba(), html, delimiter, otherparams);
        }







        private static CouchbaseCacheManager<Tuple<Dictionary<string, SubjectStatistic>, Dictionary<string, SubjectStatistic>>, OsobaStatisticQuery>
            osobaStatcache = CouchbaseCacheManager<Tuple<Dictionary<string, SubjectStatistic>, Dictionary<string, SubjectStatistic>>, OsobaStatisticQuery>.GetSafeInstance("OsobaStatistic2",
               q => OsobaStatistic.getStatisticForOsoba(q.NameId, q.Aktualnost),
               ACore.ACoreExpiration);

        private class OsobaStatisticQuery
        {
            public string NameId { get; set; }
            public Data.Relation.AktualnostType Aktualnost { get; set; }
            public override string ToString()
            {
                return NameId + "||" + Aktualnost.ToString();
            }
        }

        private static Tuple<Dictionary<string, SubjectStatistic>, Dictionary<string, SubjectStatistic>> getStatisticForOsoba(string osobaNameId, Data.Relation.AktualnostType aktualnost)
        {
            var o = Data.Osoby.GetByNameId.Get(osobaNameId);
            if (o == null)
            {
                HlidacStatu.Util.Consts.Logger.Error("Unknown osobaId " + osobaNameId);
                return new Tuple<Dictionary<string, SubjectStatistic>, Dictionary<string, SubjectStatistic>>(
                    new Dictionary<string, SubjectStatistic>(), new Dictionary<string, SubjectStatistic>()
                    );

            }

            var perIcoStat = o.AktualniVazby(aktualnost)
                .Where(v => !string.IsNullOrEmpty(v.To?.UniqId)
                            && v.To.Type == HlidacStatu.Lib.Data.Graph.Node.NodeType.Company)
                .Select(v => v.To)
                .Distinct(new HlidacStatu.Lib.Data.Graph.NodeComparer())
                .Select(f => new { ico = f.Id, ss = new Analysis.SubjectStatistic(f.Id) });
                //.OrderByDescending(or => or.ss.BasicStatPerYear.Summary.CelkemCena)
                //.ThenByDescending(or => or.ss.BasicStatPerYear.Summary.Pocet)
                //.ToDictionary(k => k.ico, v => v.ss);

            Dictionary<string, SubjectStatistic> statni = new Dictionary<string, SubjectStatistic>();
            Dictionary<string, SubjectStatistic> soukr = new Dictionary<string, SubjectStatistic>();

            foreach (var it in perIcoStat)
            {
                Lib.Data.Firma f = Lib.Data.Firmy.Get(it.ico);
                if (f.JsemStatniFirma())
                    statni.Add(it.ico, it.ss);
                else
                    soukr.Add(it.ico, it.ss);
            }


            return new Tuple<Dictionary<string, SubjectStatistic>, Dictionary<string, SubjectStatistic>>
                (statni, soukr);
        }

    }
}
