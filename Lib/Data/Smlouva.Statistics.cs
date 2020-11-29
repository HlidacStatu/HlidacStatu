using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public partial class Smlouva
    {
        public partial class Statistics
        {

            public static Analytics.StatisticsPerYear<Data> Create(string query)
            {

                Dictionary<int, Lib.Analysis.BasicData> _calc_SeZasadnimNedostatkem =
                    Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) AND chyby:zasadni", Lib.Analytics.Consts.RegistrSmluvYearsList);

                Dictionary<int, Lib.Analysis.BasicData> _calc_UzavrenoOVikendu =
                    Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) AND (hint.denUzavreni:>0)", Lib.Analytics.Consts.RegistrSmluvYearsList);

                Dictionary<int, Lib.Analysis.BasicData> _calc_ULimitu =
                    Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) AND ( hint.smlouvaULimitu:>0 )", Lib.Analytics.Consts.RegistrSmluvYearsList);

                Dictionary<int, Lib.Analysis.BasicData> _calc_NovaFirmaDodavatel =
                    Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) AND ( hint.pocetDniOdZalozeniFirmy:>-50 AND hint.pocetDniOdZalozeniFirmy:<30 )", Lib.Analytics.Consts.RegistrSmluvYearsList);

                Dictionary<int, Lib.Analysis.BasicData> _calc_smlouvy =
                    Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) ", Lib.Analytics.Consts.RegistrSmluvYearsList);

                Dictionary<int, Lib.Analysis.BasicData> _calc_bezCeny =
                    Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) AND ( issues.issueTypeId:100 ) ", Lib.Analytics.Consts.RegistrSmluvYearsList);

                Dictionary<int, Lib.Analysis.BasicData> _calc_bezSmlStran =
                    Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) AND ( issues.issueTypeId:18 OR issues.issueTypeId:12 ) ", Lib.Analytics.Consts.RegistrSmluvYearsList);

                Dictionary<int, Lib.Analysis.BasicData> _calc_sVazbouNaPolitikyNedavne =
                    Lib.ES.QueryGrouped.SmlouvyPerYear($"({query}) AND ( sVazbouNaPolitikyNedavne:true ) ", Lib.Analytics.Consts.RegistrSmluvYearsList);

                Dictionary<int, Data> data = new Dictionary<int, Data>();
                foreach (var year in Lib.Analytics.Consts.RegistrSmluvYearsList)
                {
                    data.Add(year, new Data()
                    {
                        PocetSmluv = _calc_smlouvy[year].Pocet,
                        CelkovaHodnotaSmluv = _calc_smlouvy[year].CelkemCena,
                        PocetSmluvBezCeny = _calc_bezCeny[year].Pocet,
                        PocetSmluvBezSmluvniStrany = _calc_bezSmlStran[year].Pocet,
                        PocetSmluvPolitiky = _calc_sVazbouNaPolitikyNedavne[year].Pocet,
                        SumKcSmluvBezSmluvniStrany = _calc_bezSmlStran[year].CelkemCena,
                        SumKcSmluvPolitiky = _calc_sVazbouNaPolitikyNedavne[year].CelkemCena,
                        PocetSmluvULimitu = _calc_ULimitu[year].Pocet,
                        PocetSmluvOVikendu = _calc_UzavrenoOVikendu[year].Pocet,
                        PocetSmluvSeZasadnimNedostatkem = _calc_SeZasadnimNedostatkem[year].Pocet,
                        PocetSmluvNovaFirma = _calc_NovaFirmaDodavatel[year].Pocet,
                    }
                    );
                }
                return new Analytics.StatisticsPerYear<Statistics.Data>(query, data);

            }
        }
    }
}
