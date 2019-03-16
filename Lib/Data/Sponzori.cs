using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HlidacStatu.Lib.Render;
using HlidacStatu.Util;
using static HlidacStatu.Lib.RenderTools;

namespace HlidacStatu.Lib.Data
{
    public class Sponsors
    {
        public class Strany
        {
            public class AggSum
            {
                public int Num { get; set; } = 0;
                public decimal Sum { get; set; } = 0;
            }

            public class PerStrana : AggSum
            {
                public string Strana { get; set; }
            }
            public class StranaPerYear
            {
                public string Strana { get; set; }

                public int Rok { get; set; }
                public AggSum Osoby { get; set; } = new AggSum();

                public AggSum Firmy { get; set; } = new AggSum();
            }
            public static IEnumerable<StranaPerYear> PerYears(string strana)
            {
                using (HlidacStatu.Lib.Data.DbEntities db = new HlidacStatu.Lib.Data.DbEntities())
                {
                    var resultO = db.OsobaEvent
                        .Where(m => m.AddInfo == strana && m.Type == 3 && m.DatumOd.HasValue)
                        .ToArray()
                        .Select(m => new { rok = m.DatumOd.Value.Year, oe = m })
                        .GroupBy(g => g.rok, oe => oe.oe, (r, oe) => new StranaPerYear() { Rok = r, Osoby = new AggSum() { Num = oe.Count(), Sum = oe.Sum(s => s.AddInfoNum) ?? 0 } });
                    var resultF = db.FirmaEvent
                        .Where(m => m.AddInfo == strana && m.Type == 3 && m.DatumOd.HasValue)
                        .ToArray()
                        .Select(m => new { rok = m.DatumOd.Value.Year, oe = m })
                        .GroupBy(g => g.rok, oe => oe.oe, (r, oe) => new StranaPerYear() { Rok = r, Firmy = new AggSum() { Num = oe.Count(), Sum = oe.Sum(s => s.AddInfoNum) ?? 0 } });

                    var roky = resultO.FullOuterJoin(resultF, o => o.Rok, f => f.Rok,
                        (o, f, k) => new StranaPerYear()
                        {
                            Strana = strana,
                            Rok = k,
                            Osoby = o?.Osoby ?? new AggSum(),
                            Firmy = f?.Firmy ?? new AggSum()
                        });

                    return roky;
                }
            }

            public static Lib.Render.ReportDataSource<StranaPerYear> RenderPerYearsTable(IEnumerable<StranaPerYear> dataPerYear)
            {
                ReportDataSource<StranaPerYear> rokyTable = new ReportDataSource<StranaPerYear>(new ReportDataSource<StranaPerYear>.Column[]
              {
                    new ReportDataSource<StranaPerYear>.Column() { Name="Rok",
                        HtmlRender = (s) => {
                            return s.Rok.ToString();
                            }
                        }
                ,
                new ReportDataSource<StranaPerYear>.Column() { Name="Sponzoring osob",
                    HtmlRender = (s) => {
                             StranaPerYear data = (StranaPerYear)s;
                                if (data.Osoby.Num>0)
                                        return string.Format(@"<a href='/sponzori?strana={0}&rok={1}'>Dary osob pro {0} v roce {1}</a>, počet darů: {2} za {3}", data.Strana, data.Rok, data.Osoby.Num, HlidacStatu.Util.RenderData.NicePrice(data.Osoby.Sum, "výši neznáme"));
                                else
                                    return "";
                            }
                        },
                new ReportDataSource<StranaPerYear>.Column() { Name="Sponzoring firem",
                    HtmlRender = (s) => {
                             StranaPerYear data = (StranaPerYear)s;
                                if (data.Firmy.Num>0)
                                        return string.Format(@"<a href='/sponzori?strana={0}&rok={1}&typ=firma'>Dary firem pro {0} v roce {1}</a>, počet darů: {2} za {3}", data.Strana, data.Rok, data.Firmy.Num, HlidacStatu.Util.RenderData.NicePrice(data.Firmy.Sum, "výši neznáme"));
                                else
                                    return "";
                            }
                        }
                ,
              });


                foreach (var r in dataPerYear.OrderBy(m => m.Rok))
                {
                    rokyTable.AddRow(r);
                }

                return rokyTable;
            }

            public static IEnumerable<PerStrana> AllSponsored()
            {
                using (HlidacStatu.Lib.Data.DbEntities db = new HlidacStatu.Lib.Data.DbEntities())
                {
                    PerStrana[] oSponzori = db.OsobaEvent
                            .Where(t => t.Type == (int)HlidacStatu.Lib.Data.OsobaEvent.Types.Sponzor)
                            .GroupBy(g => g.AddInfo, e => e, (k, e) => new PerStrana() { Strana = k, Num = e.Count(), Sum = e.Sum(s => s.AddInfoNum) ?? 0 })
                            .ToArray()
                            ;//.Select(m => new Tuple<string, int, int>(m.strana, m.pocet, 0));
                    PerStrana[] fSponzori = db.FirmaEvent
                            .Where(t => t.Type == (int)HlidacStatu.Lib.Data.FirmaEvent.Types.Sponzor)
                            .GroupBy(g => g.AddInfo, e => e, (k, e) => new PerStrana() { Strana = k, Num = e.Count(), Sum = e.Sum(s => s.AddInfoNum) ?? 0 })
                            .ToArray()
                            ;//.Select(m => new Tuple<string, int, int>(m.strana, 0, m.pocet));
                    var sponzori = oSponzori
                                .FullOuterJoin(fSponzori, o => o.Strana, f => f.Strana, (o, f, k) => new PerStrana()
                                {
                                    Strana = k,
                                    Num = f?.Num ?? 0 + o?.Num ?? 0,
                                    Sum = f?.Sum ?? 0 + o?.Sum ?? 0
                                })
                            //.GroupBy(g => g.Item1, e => e, (k, e) => new { strana = k, pocet = e.Sum(sum=>sum.Item2) })
                            .OrderBy(o => o.Strana)
                            .ToArray();

                    return sponzori;
                }
            }

        }

        public class GenericSponzorItem
        {
            public string Name { get; set; }
            public decimal Amount { get; set; }
            public string SubjectUrl { get; set; }
            public string TransactionUrl { get; set; }
            public DateTime Date { get; set; }
            public string Strana { get; set; }
            public Lib.Data.TransparentniUcty.BankovniPolozka Transaction { get; set; } = null;

            public GenericSponzorItem() { }
            public GenericSponzorItem(FirmaEvent fe)
            {
                this.Name = Firmy.GetJmeno(fe.ICO);
                this.Amount = fe.AddInfoNum ?? 0;
                this.SubjectUrl = Firmy.Get(fe.ICO).GetUrlOnWebsite(false);
                this.Strana = fe.AddInfo;

                //transaction
                if (!string.IsNullOrEmpty(fe.Zdroj) && fe.Zdroj.ToLower().StartsWith("https://www.hlidacstatu.cz/ucty/transakce/"))
                {
                    //https://www.hlidacstatu.cz/ucty/transakce/7CCEEC74486A0B58A13DE15369B3CE74
                    var res = HlidacStatu.Lib.ES.Manager.GetESClient_Ucty()
                        .Get<HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka>(fe.Zdroj.ToLower().Replace("https://www.hlidacstatu.cz/ucty/transakce/", ""));
                    if (res.Found)
                    {
                        this.Transaction = res.Source;
                        this.TransactionUrl = this.Transaction.GetUrl(false);
                        this.Date = this.Transaction.Datum;
                    }
                }
                if (this.Transaction == null)
                {
                    this.Date = fe.DatumOd ?? fe.Created;
                }

            }
            public GenericSponzorItem(OsobaEvent oe)
            {
                this.Name = Osoba.GetByInternalId(oe.OsobaId).FullNameWithYear();
                this.Amount = oe.AddInfoNum ?? 0;
                this.SubjectUrl = Osoba.GetByInternalId(oe.OsobaId).GetUrlOnWebsite();
                this.Strana = oe.AddInfo;
                //transaction
                if (!string.IsNullOrEmpty(oe.Zdroj) && oe.Zdroj.ToLower().StartsWith("https://www.hlidacstatu.cz/ucty/transakce/"))
                {
                    //https://www.hlidacstatu.cz/ucty/transakce/7CCEEC74486A0B58A13DE15369B3CE74
                    var res = HlidacStatu.Lib.ES.Manager.GetESClient_Ucty()
                        .Get<HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka>(oe.Zdroj.Substring("https://www.hlidacstatu.cz/ucty/transakce/".Length));
                    if (res.Found)
                    {
                        this.Transaction = res.Source;
                        this.TransactionUrl = this.Transaction.GetUrl(false);
                        this.Date = this.Transaction.Datum;
                    }
                }
                if (this.Transaction == null)
                {
                    this.Date = oe.DatumOd ?? oe.Created;
                }

            }
        }

        public static Dictionary<int, Osoba> GetAllSponsors()
        {
            using (DbEntities db = new Data.DbEntities())
            {
                var ids = db.OsobaEvent
                        .Where(m => m.Type == (int)OsobaEvent.Types.Sponzor).Distinct().Select(m => m.OsobaId)
                        .Distinct()
                        .ToArray();

                return ids
                    .Join(StaticData.Politici.Get(), i => i, p => p.InternalId, (i, p) => p)
                    .ToDictionary(k => k.InternalId, o => o);
            }
        }



        public class Sponzorstvi<T>
            where T : class //T Osoba nebo Firma
        {
            public T Sponzor { get; set; }
            public String Strana { get; set; }
            public decimal CastkaCelkem { get; set; }
            public int? Rok { get; set; }
        }

    }
}
