using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public partial class Firma
    {
        public partial class Statistics
        {
            public partial class RegistrSmluv : HlidacStatu.Lib.Analytics.StatisticsSubjectPerYear<RegistrSmluv.Data>
            {
                static RegistrSmluv nullObj = new RegistrSmluv() { ICO = "--------" };

                private static Util.Cache.CouchbaseCacheManager<RegistrSmluv, string> instanceByIco
                    = Util.Cache.CouchbaseCacheManager<RegistrSmluv, string>.GetSafeInstance("Firma_SmlouvyStatistics", Create, TimeSpan.FromHours(12));

                public RegistrSmluv() : base() { }

                public RegistrSmluv(string ico, Dictionary<int, RegistrSmluv.Data> data)
                    : base(ico, data)
                {

                }
                public static RegistrSmluv Get(string ico)
                {
                    return instanceByIco.Get(ico);

                }
                public static RegistrSmluv Get(Firma f)
                {
                    //add cache logic

                    return instanceByIco.Get(f.ICO);
                }
                private static RegistrSmluv Create(string ico)
                {
                    Firma f = Firmy.Get(ico);
                    if (f.Valid == false)
                        return nullObj;
                    else
                        return Create(f);
                }
                private static RegistrSmluv Create(Firma f)
                {

                    Dictionary<int, Lib.Analysis.BasicData> _calc_SeZasadnimNedostatkem = Lib.ES.QueryGrouped.SmlouvyPerYear($"ico:{f.ICO} and chyby:zasadni", Lib.Analytics.Consts.RegistrSmluvYearsList);

                    Dictionary<int, Lib.Analysis.BasicData> _calc_UzavrenoOVikendu = Lib.ES.QueryGrouped.SmlouvyPerYear($"ico:{f.ICO} AND (hint.denUzavreni:>0)", Lib.Analytics.Consts.RegistrSmluvYearsList);

                    Dictionary<int, Lib.Analysis.BasicData> _calc_ULimitu = Lib.ES.QueryGrouped.SmlouvyPerYear($"ico:{f.ICO} AND ( hint.smlouvaULimitu:>0 )", Lib.Analytics.Consts.RegistrSmluvYearsList);

                    Dictionary<int, Lib.Analysis.BasicData> _calc_NovaFirmaDodavatel = Lib.ES.QueryGrouped.SmlouvyPerYear($"ico:{f.ICO} AND ( hint.pocetDniOdZalozeniFirmy:>-50 AND hint.pocetDniOdZalozeniFirmy:<30 )", Lib.Analytics.Consts.RegistrSmluvYearsList);


                    Dictionary<int, RegistrSmluv.Data> data = new Dictionary<int, RegistrSmluv.Data>();
                    foreach (var year in Lib.Analytics.Consts.RegistrSmluvYearsList)
                    {
                        var stat = f.Statistic().RatingPerYear[year];
                        data.Add(year, new RegistrSmluv.Data()
                        {
                            PocetSmluv = f.Statistic().BasicStatPerYear[year].Pocet,
                            CelkovaHodnotaSmluv = f.Statistic().BasicStatPerYear[year].CelkemCena,
                            PocetSmluvBezCeny = stat.NumBezCeny,
                            PocetSmluvBezSmluvniStrany = stat.NumBezSmluvniStrany,
                            PocetSmluvPolitiky = stat.NumSPolitiky,
                            PercentSmluvBezCeny = stat.PercentBezCeny,
                            PercentSmluvBezSmluvniStrany = stat.PercentBezSmluvniStrany,
                            PercentKcBezSmluvniStrany = stat.PercentKcBezSmluvniStrany,
                            PercentKcSmluvPolitiky = stat.PercentKcSPolitiky,
                            PercentSmluvPolitiky = stat.PercentSPolitiky,
                            SumKcSmluvBezSmluvniStrany = stat.SumKcBezSmluvniStrany,
                            SumKcSmluvPolitiky = stat.SumKcSPolitiky,
                            PocetSmluvULimitu = _calc_ULimitu[year].Pocet,
                            PocetSmluvOVikendu = _calc_UzavrenoOVikendu[year].Pocet,
                            PocetSmluvSeZasadnimNedostatkem = _calc_SeZasadnimNedostatkem[year].Pocet,
                            PocetSmluvNovaFirma = _calc_NovaFirmaDodavatel[year].Pocet,
                        }
                        );
                    }

                    return new RegistrSmluv(f.ICO, data);
                }
            }
        }
    }
}
