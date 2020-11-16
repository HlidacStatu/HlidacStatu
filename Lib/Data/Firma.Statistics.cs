using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public partial class Firma
    {
        public class SmlouvyStatistics : HlidacStatu.Lib.Analytics.PerYear<StatistickeUdaje.Smlouvy>
        {
            static SmlouvyStatistics nullObj = new SmlouvyStatistics() { ICO = "--------" };

            private static Util.Cache.CouchbaseCacheManager<SmlouvyStatistics, string> instanceByIco
                = Util.Cache.CouchbaseCacheManager<SmlouvyStatistics, string>.GetSafeInstance("Firma_SmlouvyStatistics", Create, TimeSpan.FromHours(1));

            public SmlouvyStatistics() : base() { }

            public SmlouvyStatistics(string ico, Dictionary<int, StatistickeUdaje.Smlouvy> data)
                : base(ico, data)
            {

            }
            public static SmlouvyStatistics Get(string ico)
            {
                                return instanceByIco.Get(ico);

            }
            public static SmlouvyStatistics Get(Firma f)
            {
                //add cache logic

                return instanceByIco.Get(f.ICO);
            }
            private static SmlouvyStatistics Create(string ico)
            {
                Firma f = Firmy.Get(ico);
                if (f.Valid == false)
                    return nullObj;
                else
                    return Create(f);
            }
            private static SmlouvyStatistics Create(Firma f)
            {

                Dictionary<int, Lib.Analysis.BasicData> _calc_SeZasadnimNedostatkem = Lib.ES.QueryGrouped.SmlouvyPerYear($"ico:{f.ICO} and chyby:zasadni", Lib.Analytics.Consts.RegistrSmluvYearsList);

                Dictionary<int, Lib.Analysis.BasicData> _calc_UzavrenoOVikendu = Lib.ES.QueryGrouped.SmlouvyPerYear($"ico:{f.ICO} AND (hint.denUzavreni:>0)", Lib.Analytics.Consts.RegistrSmluvYearsList);

                Dictionary<int, Lib.Analysis.BasicData> _calc_ULimitu = Lib.ES.QueryGrouped.SmlouvyPerYear($"ico:{f.ICO} AND ( hint.smlouvaULimitu:>0 )", Lib.Analytics.Consts.RegistrSmluvYearsList);

                Dictionary<int, Lib.Analysis.BasicData> _calc_NovaFirmaDodavatel = Lib.ES.QueryGrouped.SmlouvyPerYear($"ico:{f.ICO} AND ( hint.pocetDniOdZalozeniFirmy:>-50 AND hint.pocetDniOdZalozeniFirmy:<30 )", Lib.Analytics.Consts.RegistrSmluvYearsList);


                Dictionary<int, StatistickeUdaje.Smlouvy> data = new Dictionary<int, StatistickeUdaje.Smlouvy>();
                foreach (var year in Lib.Analytics.Consts.RegistrSmluvYearsList)
                {
                    var stat = f.Statistic().RatingPerYear[year];
                    data.Add(year, new StatistickeUdaje.Smlouvy()
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

                return new SmlouvyStatistics(f.ICO, data);
            }
        }
    }
}
