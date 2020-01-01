using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devmasters.Core;

namespace HlidacStatu.Lib.Data.TransparentniUcty
{
    public class BankovniUcty
    {
        class UcetWithExpiration
        {
            public BankovniUcet BU { get; set; }
            public DateTime Expiration { get; set; } = DateTime.Now.AddHours(2);
        }

        static Dictionary<string, UcetWithExpiration> uctyWithExpiration = new Dictionary<string, UcetWithExpiration>();

        static object lockObj = new object();
        public static BankovniUcet Get(string cisloUctu)
        {
            lock (lockObj)
            {
                if (uctyWithExpiration.ContainsKey(cisloUctu))
                {
                    var kv = uctyWithExpiration[cisloUctu];
                    if (kv.Expiration > DateTime.Now)
                        return kv.BU;
                    else
                        uctyWithExpiration.Remove(cisloUctu);
                }

                BankovniUcet bu = BankovniUcet.Get(cisloUctu);
                //ulozit vcetne neexistujicich uctu, obrana proti utoku pres fake ucty
                uctyWithExpiration.Add(cisloUctu, new UcetWithExpiration() { BU = bu });
                return bu;
            }
        }

        static Devmasters.Cache.V20.LocalMemory.LocalMemoryCache<IEnumerable<BankovniUcet>> content = new Devmasters.Cache.V20.LocalMemory.LocalMemoryCache<IEnumerable<BankovniUcet>>(
                    TimeSpan.FromHours(2),
                    "bankovniucty_all",
                    (p) => getAllFromDb()
                    );

        public static IEnumerable<BankovniUcet> GetAll()
        {
            return content.Get();
        }
        private static IEnumerable<BankovniUcet> getAllFromDb()
        {


            List<BankovniUcet> bu = new List<BankovniUcet>();
            Func<int, int, ISearchResponse<BankovniUcet>> searchFunc = 
                (size, page) =>
                {
                    return ES.Manager.GetESClient_BankovniUcty().Search<BankovniUcet>(a => a
                                .Source(ss => ss.ExcludeAll())
                                //.Fields(f => f.Field("Id"))
                                .Size(size)
                                .From(page * size)
                                .Scroll("1m")
                                );
                };

            Searching.Tools.DoActionForQuery<HlidacStatu.Lib.Data.TransparentniUcty.BankovniUcet>(
                ES.Manager.GetESClient_BankovniUcty(), searchFunc,
                (p, o) =>
                {
                    bu.Add(BankovniUcty.Get(p.Id));

                    return new Devmasters.Core.Batch.ActionOutputData();
                }, null, null, null, false, blockSize: 500
            );

            return bu.Where(m => m != null);

        }


        public static Nest.ISearchResponse<BankovniPolozka> SearchPolozkyForOsoba(string nameId,
            BankovniUcet bu = null, int size = 500,
            AggregationContainerDescriptor<BankovniPolozka> anyAggregation = null)
        {
            HlidacStatu.Lib.Data.Osoba o = HlidacStatu.Lib.Data.Osoby.GetByNameId.Get(nameId);
            if (o == null)
                return null;
            return SearchPolozkyForOsoba(o.InternalId);
        }
        public static Nest.ISearchResponse<BankovniPolozka> SearchPolozkyForOsoba(int osobaInternalId,
            BankovniUcet bu = null, int size = 500,
            AggregationContainerDescriptor<BankovniPolozka> anyAggregation = null)
        {
            return SearchPolozkyRaw(
                string.Format("comments.valueInt:{0} AND comments.typeId:1", osobaInternalId),
                bu, size);
        }
        public static Nest.ISearchResponse<BankovniPolozka> SearchPolozkyForFirma(string ico,
            BankovniUcet bu = null, int size = 500,
            AggregationContainerDescriptor<BankovniPolozka> anyAggregation = null)
        {
            return SearchPolozkyRaw(
                string.Format("comments.valueInt:{0} AND comments.typeId:2", ico),
                bu, size);
        }

        public static Nest.ISearchResponse<BankovniPolozka> SearchPolozkyRaw(string queryString,
            BankovniUcet bu = null, int size = 500,
            AggregationContainerDescriptor<BankovniPolozka> anyAggregation = null)
        {
            AggregationContainerDescriptor<BankovniPolozka> baseAggrDesc = null;
            baseAggrDesc = anyAggregation == null ?
                new AggregationContainerDescriptor<BankovniPolozka>().Sum("sumKc", m => m.Field(f => f.Castka))
                    : anyAggregation;

            Func<AggregationContainerDescriptor<BankovniPolozka>, AggregationContainerDescriptor<BankovniPolozka>> aggrFunc
                = (aggr) => { return baseAggrDesc; };


            Nest.ISearchResponse<BankovniPolozka> res = null;

            if (bu != null)
            {
                res = HlidacStatu.Lib.ES.Manager.GetESClient_BankovniPolozky()
                                        .Search<BankovniPolozka>(a => a
                                            .Size(500)
                                            .Aggregations(aggrFunc)
                                            .Query(qq => qq.Bool(b => b
                                                .Must(
                                                    m => m.Term(t => t.Field(f => f.CisloUctu).Value(bu.Id)),
                                                    m1 => m1.QueryString(qs => qs
                                                        .Query(queryString)
                                                        .DefaultOperator(Nest.Operator.And)
                                                        ))
                                                )
                                            )
                                        );
            }
            else
            {
                res = HlidacStatu.Lib.ES.Manager.GetESClient_BankovniPolozky()
                        .Search<HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka>(a => a
                            .Size(500)
                            .Aggregations(aggrFunc)
                            .Query(qq => qq.QueryString(qs => qs
                                    .Query(queryString)
                                    .DefaultOperator(Nest.Operator.And)
                                    )
                                )
                            );

            }
            return res;
        }


        public static decimal SearchPolozkyGetSum(Nest.ISearchResponse<BankovniPolozka> res)
        {
            decimal sum = 0;
            if (res.Aggregations != null && res.Aggregations.ContainsKey("sumKc"))
            {
                sum = (decimal)((Nest.ValueAggregate)res.Aggregations["sumKc"]).Value;

            }
            return sum;
        }


        public static string NormalizeCisloUctu(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            var ucet = System.Text.RegularExpressions.Regex.Replace(s, "[^0-9-/]", "").Trim();

            string prefix = "";
            string number = "";
            string bankNumber = "";

            string[] partBank = ucet.Split('/');
            if (partBank.Length == 2)
                bankNumber = partBank[1];

            string[] partNumber = partBank[0].Split('-');
            if (partBank[0].StartsWith("-")) //fix pro "minusove" ucty CSSD -9/2010
            {
                prefix = "";
                number = partBank[0];
            }
            else if (partNumber.Length == 2) 
            {
                prefix = partNumber[0];
                number = partNumber[1];
            }
            else
                number = partNumber[0];

            var prefix1 = prefix.TrimStart('0');
            var number1 = number.TrimStart('0');

            if (!string.IsNullOrEmpty(prefix1))
                return prefix1 + "-" + number1 + "/" + bankNumber;
            else
                return number1 + "/" + bankNumber;

        }

    }
}
