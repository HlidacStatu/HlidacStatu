using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Data.TransparentniUcty
{
    public class BankovniPolozka : IEqualityComparer<BankovniPolozka>,
        Bookmark.IBookmarkable, HlidacStatu.Plugin.TransparetniUcty.IBankovniPolozka
    {

        public BankovniPolozka() { }
        public BankovniPolozka(HlidacStatu.Plugin.TransparetniUcty.IBankovniPolozka ip)
        {
            this.AddId = ip.AddId;
            this.Castka = ip.Castka;
            this.CisloProtiuctu = ip.CisloProtiuctu;
            this.CisloUctu = ip.CisloUctu;
            this.Datum = ip.Datum;
            this.Id = ip.Id;
            this.KS = ip.KS;
            this.NazevProtiuctu = ip.NazevProtiuctu;
            this.PopisTransakce = ip.PopisTransakce;
            this.SS = ip.SS;
            this.VS = ip.VS;
            this.ZdrojUrl = ip.ZdrojUrl;
            this.ZpravaProPrijemce = ip.ZpravaProPrijemce;

        }

        public class Comment
        {
            public enum Types
            {
                Obecny = 0,
                VazbaNaOsobu = 1,
                VazbaNaFirmu = 2,
                Problem = 3,
                Temporary = -1,
                Analyzed = -2,
            }

            public int TypeId { get; set; }

            [Nest.Object(Ignore = true)]
            public Types Type
            {
                get { return (Types)this.TypeId; }
                set { this.TypeId = (int)value; }
            }

            [Nest.Keyword]
            public string Author { get; set; }

            [Nest.Text]
            public string Value { get; set; }

            [Nest.Number]
            public int ValueInt { get; set; }
            [Nest.Date]
            public DateTime Created { get; set; } = DateTime.Now;
        }

        //idu,majitel,nazev,datum,protiucet,popis,valuta,typ,castka,poznamka
        public string Id { get; set; } = null;

        public void InitId()
        {
            string[] data = new string[] {
                    this.Castka.ToString(HlidacStatu.Util.Consts.czCulture),
                    this.CisloUctu,
                    this.CisloProtiuctu,
                    this.Datum.ToString("dd.MM.yyyy", HlidacStatu.Util.Consts.czCulture),
                    this.VS };

            this.Id = Devmasters.Core.CryptoLib.Hash.ComputeHashToHex(data.Aggregate((f, s) => f + "|" + s));

        }

        private string _cisloUctu = string.Empty;
        [Nest.Keyword]
        public string CisloUctu
        {
            get
            {
                return _cisloUctu;
            }
            set
            {
                _cisloUctu = Lib.Data.TransparentniUcty.BankovniUcty.NormalizeCisloUctu(value);
            }
        }
        [Nest.Date]
        public DateTime Datum { get; set; }
        public string PopisTransakce { get; set; } = "";
        public string NazevProtiuctu { get; set; } = "";



        private string _cisloProtiUctu = string.Empty;

        [Nest.Keyword]
        public string CisloProtiuctu
        {
            get
            {
                return _cisloProtiUctu;
            }
            set
            {
                _cisloProtiUctu = Lib.Data.TransparentniUcty.BankovniUcty.NormalizeCisloUctu(value);
            }
        }

        public string ZpravaProPrijemce { get; set; } = "";

        [Nest.Keyword]
        public string VS { get; set; } = "";
        [Nest.Keyword]
        public string KS { get; set; } = "";
        [Nest.Keyword]
        public string SS { get; set; } = "";

        [Nest.Number]
        public decimal Castka { get; set; }

        [Nest.Keyword]
        public string AddId { get; set; } = "";

        public Comment[] Comments { get; set; } = new Comment[] { };

        public Comment AddComment(Comment c)
        {
            bool removeOld = false;
            switch (c.Type)
            {
                case Comment.Types.VazbaNaOsobu:
                case Comment.Types.VazbaNaFirmu:
                case Comment.Types.Analyzed:
                    removeOld = true;
                    break;
            }
            var comments = this.Comments.ToList();
            if (removeOld)
                comments = comments.Where(m => m.TypeId != c.TypeId).ToList();

            comments.Add(c);
            this.Comments = comments.ToArray();
            return c;
        }

        [Nest.Keyword]
        public string ZdrojUrl { get; set; }


        BankovniUcet _bu = null;
        [Nest.Object(Ignore = true)]
        public BankovniUcet BU
        {
            get
            {
                if (_bu == null)
                    _bu = BankovniUcty.Get(this.CisloUctu);
                return _bu;
            }
        }

        public string GetUrl(bool local = true)
        {
            return GetUrl(local, false, string.Empty);
        }

        public string GetUrl(bool local, string foundWithQuery)
        {
            return GetUrl(local, false,"");
        }
        public string GetUrl(bool local = true, bool onList = false, string foundWithQuery = "")
        {
            if (this.BU == null)
                return "";
            if (onList)
                return this.BU.GetUrl(local, foundWithQuery) + "#" + this.Id;
            else
            {
                string url = "/ucty/transakce/" + System.Net.WebUtility.UrlEncode(this.Id);
                if (!string.IsNullOrEmpty(foundWithQuery))
                    url = url + "?qs=" + System.Net.WebUtility.UrlEncode(foundWithQuery);

                if (!local)
                    return "https://www.hlidacstatu.cz" + url;
                return url;
            }
        }

        public bool EqualsBezProtiUctu(BankovniPolozka x, BankovniPolozka y)
        {
            return (
                x.Castka == y.Castka
                && x.CisloUctu == y.CisloUctu
                && (x.CisloProtiuctu == y.CisloProtiuctu || string.IsNullOrEmpty(x.CisloProtiuctu) || string.IsNullOrEmpty(y.CisloProtiuctu))
                && x.Datum == y.Datum
                && x.KS == y.KS
                && x.SS == y.SS
                && x.VS == y.VS
                && x.NazevProtiuctu == y.NazevProtiuctu
                && x.PopisTransakce == y.PopisTransakce
                && x.ZpravaProPrijemce == y.ZpravaProPrijemce
                );
        }

        public bool Equals(BankovniPolozka x, BankovniPolozka y)
        {
            return (
                x.Castka == y.Castka
                && x.CisloUctu == y.CisloUctu
                && x.CisloProtiuctu == y.CisloProtiuctu
                && x.Datum == y.Datum
                && x.KS == y.KS
                && x.SS == y.SS
                && x.VS == y.VS
                && x.NazevProtiuctu == y.NazevProtiuctu
                && x.PopisTransakce == y.PopisTransakce
                && x.ZpravaProPrijemce == y.ZpravaProPrijemce
                );
        }
        //public override bool Equals(object obj)
        //{
        //    return this.Equals(this,(BankovniPolozka)obj);
        //}
        //public override int GetHashCode()
        //{
        //    return BankovniPolozka.GetHashCode(this);
        //}
        public int GetHashCode(BankovniPolozka obj)
        {
            //http://stackoverflow.com/a/4630550
            return
                new
                {
                    obj.Castka,
                    obj.CisloUctu,
                    obj.CisloProtiuctu,
                    obj.Datum,
                    obj.KS,
                    obj.NazevProtiuctu,
                    obj.PopisTransakce,
                    obj.SS,
                    obj.VS,
                    obj.ZpravaProPrijemce
                }.GetHashCode();
        }

        public bool IsUnique(ElasticClient client = null)
        {
            if (string.IsNullOrEmpty(this.Id))
                InitId();
            var es = client ?? ES.Manager.GetESClient_Ucty();
            var res = es
                    .DocumentExists<BankovniPolozka>(this.Id);
            return !res.Exists;

        }

        public bool Delete(string audituser)
        {
            var zdroj = this.GetUrl(local: false);
            using (DbEntities db = new Data.DbEntities())
            {
                var oe = db.OsobaEvent.Where(m => m.Zdroj == zdroj).FirstOrDefault();
                if (oe != null)
                {
                    oe.Delete(audituser, true);
                }
                else
                {
                    var fe = db.FirmaEvent.Where(m => m.Zdroj == zdroj).FirstOrDefault();
                    if (fe != null)
                    {
                        db.FirmaEvent.Remove(fe);
                    }
                }
                db.SaveChanges();
            }
            var ret = ES.Manager.GetESClient_Ucty().Delete<BankovniPolozka>(this.Id);
            return ret.Result == Result.Deleted;
        }

        public void Save(string user, ElasticClient client = null, bool updateId = true)
        {
            if (updateId || string.IsNullOrEmpty(this.Id))
                InitId();
            var es = client ?? ES.Manager.GetESClient_Ucty();

            var prev = BankovniPolozka.Get(this.Id);

            Audit.Add<BankovniPolozka>(Audit.Operations.Update, user, this, prev);
            es.IndexDocument<BankovniPolozka>(this);
        }


        public static BankovniPolozka Get(string transactionId)
        {
            var resBU = HlidacStatu.Lib.ES.Manager.GetESClient_Ucty()
                .Get<BankovniPolozka>(transactionId);
            //.Search<HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka>(m => m
            //    .Query(qq => qq
            //        .Term(t => t.Field(ff => ff.CisloUctu).Value(cislo))
            //        )
            //);
            if (resBU.Found == false)
                return null;
            else
                return resBU.Source;

        }

        public string ToAuditJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public string ToAuditObjectTypeName()
        {
            return "BankovniPolozka";
        }

        public string ToAuditObjectId()
        {
            return this.Id;
        }


        public string BookmarkName()
        {
            return $"Transakce na účtu {this.BU.CisloUctu} ({this.BU.Subjekt}) z {this.Datum.ToShortDateString()} ve výši {HlidacStatu.Util.RenderData.NicePrice(this.Castka)}";
        }
    }
}
