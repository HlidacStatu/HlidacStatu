using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Data.TransparentniUcty
{
    public class BankovniPolozka : IEqualityComparer<BankovniPolozka>, Plugin.TransparetniUcty.IBankovniPolozka
    {
        private static External.DataSets.DataSet _client =
            External.DataSets.DataSet.CachedDatasets.Get("transparentni-ucty-transakce");

        public BankovniPolozka() { }
        public BankovniPolozka(Plugin.TransparetniUcty.IBankovniPolozka ip)
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

        //idu,majitel,nazev,datum,protiucet,popis,valuta,typ,castka,poznamka
        public string Id { get; set; } = null;

        public void InitId()
        {
            string[] data = new string[] {
                    this.Castka.ToString(Util.Consts.czCulture),
                    this.CisloUctu,
                    this.CisloProtiuctu,
                    this.Datum.ToString("dd.MM.yyyy", Util.Consts.czCulture),
                    this.VS };

            this.Id = Devmasters.Core.CryptoLib.Hash.ComputeHashToHex(data.Aggregate((f, s) => f + "|" + s));

        }

        private string _cisloUctu = string.Empty;
        public string CisloUctu
        {
            get
            {
                return _cisloUctu;
            }
            set
            {
                _cisloUctu = BankovniUcet.NormalizeCisloUctu(value);
            }
        }
        public DateTime Datum { get; set; }
        public string PopisTransakce { get; set; } = "";
        public string NazevProtiuctu { get; set; } = "";

        private string _cisloProtiUctu = string.Empty;

        public string CisloProtiuctu
        {
            get
            {
                return _cisloProtiUctu;
            }
            set
            {
                _cisloProtiUctu = BankovniUcet.NormalizeCisloUctu(value);
            }
        }

        public string ZpravaProPrijemce { get; set; } = "";

        public string VS { get; set; } = "";
        public string KS { get; set; } = "";
        public string SS { get; set; } = "";

        public decimal Castka { get; set; }

        public string AddId { get; set; } = "";

        public string ZdrojUrl { get; set; }

        private BankovniUcet _bu = null;
        public BankovniUcet GetBankovniUcet()
        {
            if (_bu == null)
                _bu = BankovniUcet.Get(this.CisloUctu);
            return _bu;
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
            if (GetBankovniUcet() == null)
                return "";
            if (onList)
                return GetBankovniUcet().GetUrl(local, foundWithQuery) + "#" + this.Id;
            else
            {
                string url = "/data/Detail/transparentni-ucty-transakce/" + System.Net.WebUtility.UrlEncode(this.Id);
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

        public bool Exists()
        {
            if (string.IsNullOrEmpty(this.Id))
                InitId();

            return _client.ItemExists(this.Id);
        }

        public bool Delete()
        {
            return _client.DeleteData(this.Id);
        }

        public void Save(string user, bool updateId = true)
        {
            if (updateId || string.IsNullOrEmpty(this.Id))
                InitId();

            _client.AddData(this, this.Id, user);
        }


        public static BankovniPolozka Get(string transactionId)
        {
            BankovniPolozka bu = _client.GetData<BankovniPolozka>(transactionId);
            return bu;
        }

        public static IEnumerable<BankovniPolozka> GetAll()
        {
            return _client.GetAllData<BankovniPolozka>();
        }

    }
}
