using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devmasters.Core;

namespace HlidacStatu.Lib.Data.TransparentniUcty
{
    public class BankovniUcet
        : Bookmark.IBookmarkable, HlidacStatu.Plugin.TransparetniUcty.IBankovniUcet
    {
        private object lockObj = new object();

        [ShowNiceDisplayName()]
        public enum TypUctuEnum
        {
            [NiceDisplayName("Neznámý typ účtu")]
            Neurcen = 0,
            [NiceDisplayName("Provozní účet")]
            ProvozniUcet = 1,
            [NiceDisplayName("Volební účet")]
            VolebniUcet = 2
        }

        public string Id
        {
            get
            {
                return this.CisloUctu;
            }
        }
        [Nest.Keyword]
        public string Subjekt { get; set; }

        public string Nazev { get; set; }

        [Nest.Keyword]
        public string TypSubjektu { get; set; }

        [Nest.Keyword]
        public string Url { get; set; }
        [Nest.Keyword]
        public string CisloUctu { get; set; }

        [Nest.Keyword]
        public string Mena { get; set; }

        [Nest.Object(Ignore = true)]
        public string ApiUrl { get { return "https://www.hlidacstatu.cz/api/v1/ucetexport?id=" + System.Net.WebUtility.UrlEncode(this.CisloUctu); } }

        private int _numOfTrans = -1;
        public int NumOfTransactions()
        {
            lock (lockObj)
            {
                if (_numOfTrans < 0)
                {
                    var countRes = HlidacStatu.Lib.ES.Manager.GetESClient_BankovniPolozky()
                       .Search<HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka>(a => a
                           .Size(0)
                           .Query(q => q.Term(t => t.Field(f => f.CisloUctu).Value(this.Id)))
                           );
                    if (countRes.IsValid)
                        _numOfTrans = (int)countRes.Total;
                    else
                        _numOfTrans = 0;
                }
            }
            return _numOfTrans;
        }

        [Nest.Date]
        public DateTime LastSuccessfullParsing { get; set; }

        public string GetUrl(bool local = true)
        {
            return GetUrl(local, string.Empty);
        }

        public string GetUrl(bool local, string foundWithQuery)
        {
            string url = "/ucty/ucet?id=" + System.Net.WebUtility.UrlEncode(this.Id);

            if (!string.IsNullOrEmpty(foundWithQuery))
                url = url + "?qs=" + System.Net.WebUtility.UrlEncode(foundWithQuery);

            if (!local)
                return "https://www.hlidacstatu.cz" + url;


            return url;
        }
        public string GetSubjektUrl(bool local = true)
        {
            string pref = "";
            if (!local)
                pref = "https://www.hlidacstatu.cz";
            return pref + "/ucty/ucty?id=" + System.Net.WebUtility.UrlEncode(this.Subjekt);
        }

        public int Active { get; set; } = 1;
        public TypUctuEnum TypUctu { get; set; }

        public int numTypUctu
        {
            get { return (int)this.TypUctu; }
            set { this.TypUctu = (TypUctuEnum)value; }
        }

        public void Save(ElasticClient client = null)
        {
            var es = client ?? ES.Manager.GetESClient_BankovniUcty();

            es.IndexDocument<BankovniUcet>(this);
        }

        public static BankovniUcet Get(string cislo)
        {
            var resBU = HlidacStatu.Lib.ES.Manager.GetESClient_BankovniUcty()
                .Search<HlidacStatu.Lib.Data.TransparentniUcty.BankovniUcet>(m => m
                    .Query(qq => qq
                        .Term(t => t.Field(ff => ff.CisloUctu).Value(cislo))
                        )
                );
            if (resBU.Total == 0)
                return null;
            else
                return resBU.Hits.First().Source;

        }

        public static bool DeleteUcet(BankovniUcet bu, string user)
        {
            if (bu == null)
                return false;

            Func<int, int, Nest.ISearchResponse<HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka>> searchFunc = (size, page) =>
            {
                return HlidacStatu.Lib.ES.Manager.GetESClient_BankovniPolozky()
                        .Search<HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka>(a => a
                            .Size(size)
                            .From(page * size)
                            .Query(q => q.Term(t => t.Field(f => f.CisloUctu).Value(bu.Id)))
                            .Scroll("2m")
                            );
            };

            List<BankovniPolozka> transactionIds = new List<BankovniPolozka>();

            HlidacStatu.Lib.ES.SearchTools.DoActionForQuery<HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka>(
                HlidacStatu.Lib.ES.Manager.GetESClient_BankovniPolozky(),
                searchFunc,
                (p, o) =>
                {
                    transactionIds.Add(p.Source);

                    return new Devmasters.Core.Batch.ActionOutputData();
                }, null, null, null, false, blockSize: 500

            );
            bool ok = true;
            foreach (var t in transactionIds)
            {
                ok = ok && t.Delete(user);
            }

            ok = ok && ES.Manager.GetESClient_BankovniUcty().Delete<BankovniUcet>(bu.Id).IsValid;
            return ok;
        }

        public string ToAuditJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public string ToAuditObjectTypeName()
        {
            return "BankovniUcet";
        }

        public string ToAuditObjectId()
        {
            return this.Id;
        }


        public string BookmarkName()
        {
            return $"Bankovní účet {this.CisloUctu} ({this.Subjekt})";
        }
    }

}
