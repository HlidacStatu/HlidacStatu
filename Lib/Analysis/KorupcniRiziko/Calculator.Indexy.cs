using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HlidacStatu.Lib.Data;

using Nest;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{

    public partial class Calculator
    {

        public class SmlouvyForIndex
        {
            public SmlouvyForIndex() { }
            public SmlouvyForIndex(string dodavatel, decimal hodnota, int uLimitu, int[] obory)
            {
                this.Dodavatel = dodavatel;
                this.HodnotaSmlouvy = hodnota;
                this.ULimitu = ULimitu;
                this.Obory = obory;
            }
            public string Dodavatel { get; set; }
            public decimal HodnotaSmlouvy { get; set; }
            public int ULimitu { get; set; }
            public int[] Obory { get; set; } = new int[] { };
            public bool ContainsObor(int oborId)
            {
                foreach (var o in Obory)
                {
                    if (oborId <= o && o < oborId + 99)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 0 - trh
        /// 1 - monopol
        /// nabyva hodnoty 1/N az 1
        /// </summary>
        /// <param name="smlouvy"></param>
        /// <returns></returns>
        public static decimal Herfindahl_Hirschman_Index(IEnumerable<SmlouvyForIndex> smlouvy, decimal prumHodnotaSmluv)
        {
            if (prumHodnotaSmluv == 0)
                prumHodnotaSmluv = 1;

            var groupedPerDodavatel = smlouvy
                .GroupBy(k => k.Dodavatel, m => m, (k, v) => new
                {
                    k = k,
                    sumVal = v.Sum(m => m.HodnotaSmlouvy == 0 ? prumHodnotaSmluv : m.HodnotaSmlouvy)
                })
                .ToDictionary(k => k.k, v => v.sumVal);

            return Herfindahl_Hirschman_Index(groupedPerDodavatel.Values);
        }

        /// <summary>
        /// 0 - trh
        /// 1 - monopol
        /// nabyva hodnoty 1/N az 1
        /// </summary>
        /// <param name="valuesGroupedByCompany"></param>
        /// <returns></returns>
        public static decimal Herfindahl_Hirschman_Index(IEnumerable<decimal> valuesGroupedByCompany)
        {
            decimal total = valuesGroupedByCompany.Sum();
            if (total == 0)
                return Herfindahl_Hirschman_Index(Enumerable.Repeat<decimal>(1m, valuesGroupedByCompany.Count())); ;
            decimal hindex = valuesGroupedByCompany
                .Select(v => v / total) //podil na trhu
                .Select(v => v * v) // ^2
                .Sum(); //SUM
            return hindex;
        }

        /// <summary>
        /// Nabyva hodnoty 0-1
        /// 0 - trh
        /// 1 - monopol
        /// ztraci se zde informace o poctu dodavatelu
        /// 
        /// example: Assume a market with two players and equally distributed market share; H = 1/N = 1/2 = 0.5 and H* = 0. 
        /// Now compare that to a situation with three players and again an equally distributed market share; H = 1/N = 1/3 = 0.333..., 
        /// note that H* = 0 like the situation with two players. The market with three players is less concentrated, 
        /// but this is not obvious looking at just H*. 
        /// Thus, the normalized Herfindahl index can serve as a measure for the equality of distributions, 
        /// but is less suitable for concentration.
        /// </summary>
        /// <param name="valuesGroupedByCompany"></param>
        /// <returns></returns>
        public static decimal Herfindahl_Hirschman_IndexNormalized(IEnumerable<SmlouvyForIndex> smlouvy, decimal prumHodnotaSmluv)
        {
            if (prumHodnotaSmluv == 0)
                prumHodnotaSmluv = 1;

            var groupedPerDodavatel = smlouvy
                .GroupBy(k => k.Dodavatel, m => m, (k, v) => new
                {
                    k = k,
                    sumVal = v.Sum(m => m.HodnotaSmlouvy == 0 ? prumHodnotaSmluv : m.HodnotaSmlouvy)
                })
                .ToDictionary(k => k.k, v => v.sumVal);

            if (groupedPerDodavatel.Count() == 1)
                return 1;
            decimal H = Herfindahl_Hirschman_Index(groupedPerDodavatel.Values);
            decimal N = (decimal)groupedPerDodavatel.Count();
            decimal hindexNorm = (H - 1 / N) / (1 - 1 / N);
            return hindexNorm;
        }

        /// <summary>
        /// Herfindahl index realny
        /// spocitame idealni Herfindahl index, nejlepsi mozny pro tuto sadu smluv (kazdou smlouvu ma jiny dodavatel)
        /// pak Herfindahl podle skutecnosti (seskupeny podle dodavatelu) a od skutecneho odecteme teorericky nejlepsi
        /// 
        /// 0 - trh
        /// 1 - monopol
        /// </summary>
        /// <param name="individualContractDodavatelCena"></param>
        /// <returns></returns>
        public static decimal Herfindahl_Hirschman_Modified(
            IEnumerable<SmlouvyForIndex> individualContractDodavatelCena, decimal prumHodnotaSmluv)
        {
            //idealni HHI je, pokud jsou smlouvy rovnomerne mezi jednotlivymi dodavateli a kazdy ma 1 smluvu na stejnou castku

            if (individualContractDodavatelCena.Count() == 0)
                return 0;
            if (individualContractDodavatelCena.Count() == 1)
                return 1;


            decimal idealHI = Herfindahl_Hirschman_Index(Enumerable.Repeat<decimal>(1m, individualContractDodavatelCena.Count()));

            //pokud vsechny nulove
            decimal total = individualContractDodavatelCena.Sum(m => m.HodnotaSmlouvy);

            //pokud vse nulove a kazdy dodavagtel jiny, vrat distribuci -> tnz. HHI
            if (total == 0 &&
                individualContractDodavatelCena.Select(m => m.Dodavatel).Distinct().Count() == individualContractDodavatelCena.Count())
                return idealHI;

            if (prumHodnotaSmluv == 0)
                prumHodnotaSmluv = 1;

            Dictionary<string, decimal> groupedPerDodavatel = individualContractDodavatelCena
                        .GroupBy(k => k.Dodavatel, m => m, (k, v) => new
                        {
                            k = k,
                            sumVal = v.Sum(m => m.HodnotaSmlouvy == 0 ? prumHodnotaSmluv : m.HodnotaSmlouvy)
                        })
                        .ToDictionary(k => k.k, v => v.sumVal);

            decimal HI = Herfindahl_Hirschman_Index(groupedPerDodavatel.Values);


            return HI - idealHI; //podle zindex hi/indexhi, ale to je divny
        }

        /// <summary>
        /// SvobodaP_AnalyzaTrzni_JP_2014.pdf
        /// 
        /// 0 - trh
        /// 1 - monopol
        /// Hall-Tidemanův index vychází z principů, které tito dva autoři popsali jako základní požadavky, které by měl splňovat koncentrační index. Především vyzdvihují potřebu do vzorce na výpočet indexu zakomponovat počet firem na trhu, protože to do určité míry odráží podmínky pro vstup nových firem do odvětví. Vzorec pro výpočet tohoto indexu je následující:
        /// 
        /// HT1 = 1 / ( 2* sum_1..n(i*si) -1 )
        /// n je pocet dodavatel, i je poradi dodavatele podle trzniho podilu, si je trzni podil dodavatele i, 
        /// Hodnota si značí tržní podíl i-té firmy a tato hodnota je vážena pořadím i, které bylo firmě přiřazeno na základě sestavení žebříčku firem podle jejich tržních podílů od největšího po nejmenší. 
        /// Písmeno n značí počet firem na trhu. Největší firma tak dostane váhu v hodnotě i=1. 
        /// Hall-Tidemanův index nabývá hodnot od 0 do 1, kde hodnota 0 značí, že na trhu je nekonečný počet stejně velkých firem, 
        /// a hodnota 1 znamená, že na trhu je monopol. 
        /// Hall- Tidemanův index je vhodné použít pro výpočet tržní koncentrace na trzích, kde je několik velkých firem, ale přesto je trh ovlivňován velkým počtem firem s malým tržním podílem.[1]
        /// </summary>
        /// <param name="individualContractDodavatelCena"></param>
        /// <returns></returns>
        public static decimal Hall_Tideman_Index(IEnumerable<SmlouvyForIndex> smlouvy, decimal prumHodnotaSmluv)
        {
            if (prumHodnotaSmluv == 0)
                prumHodnotaSmluv = 1;

            decimal total = smlouvy.Select(m => m.HodnotaSmlouvy == 0 ? prumHodnotaSmluv : m.HodnotaSmlouvy).Sum();

            var groupedPerDodavatel = smlouvy
                .GroupBy(k => k.Dodavatel, m => m, (k, v) => new
                {
                    dodavatel = k,
                    sumVal = v.Sum(m => m.HodnotaSmlouvy == 0 ? prumHodnotaSmluv : m.HodnotaSmlouvy)
                })
                .OrderByDescending(o => o.sumVal)
                .Select((m, i) => new { idx = i + 1, dodavatel = m.dodavatel, trznipodil = m.sumVal / total, sumVal = m.sumVal });

            decimal htSum = groupedPerDodavatel
                .Select(m => m.idx * m.trznipodil)
                .Sum();

            decimal HT = 1 / (2 * htSum - 1);

            return HT;
        }

        /// <summary>
        /// https://www.dnb.nl/binaries/Measures%20of%20Competition_tcm46-145799.pdf 2.2.4
        /// 
        /// </summary>
        public static decimal Comprehensive_Industrial_Concentration_Index(IEnumerable<SmlouvyForIndex> smlouvy, decimal prumHodnotaSmluv)
        {
            if (prumHodnotaSmluv == 0)
                prumHodnotaSmluv = 1;
            decimal total = smlouvy.Select(m => m.HodnotaSmlouvy == 0 ? prumHodnotaSmluv : m.HodnotaSmlouvy).Sum();
            if (total == 0)
                total = 1;

            var groupedPerDodavatel = smlouvy
                .GroupBy(k => k.Dodavatel, m => m, (k, v) => new
                {
                    dodavatel = k,
                    sumVal = v.Sum(m => m.HodnotaSmlouvy == 0 ? prumHodnotaSmluv : m.HodnotaSmlouvy)
                })
                .OrderByDescending(o => o.sumVal)
                .Select((m, i) => new { idx = i + 1, dodavatel = m.dodavatel, trznipodil = m.sumVal / total, sumVal = m.sumVal })
                .ToList();

            decimal cci1 = groupedPerDodavatel[0].trznipodil;
            decimal ccisum = groupedPerDodavatel.Skip(1)
                    .Select(m => (m.trznipodil * m.trznipodil) * (1 + (1 - m.trznipodil)))
                    .Sum();
            decimal cci = cci1 + ccisum;
            return cci;

        }

        /// <summary>
        /// 0 - trh
        /// 1 - monopol
        ///
        /// Kwoka's dominance index (D) is defined as the sum of the squared differences between each firm's share and the next largest share in a market:
        /// The values range from 1/n2 to 1. 
        /// A result of 1 represents a monopolistic market while values close to 0 mean 
        /// that no single com- pany can exercise power in the market.
        /// </summary>
        /// <param name="smlouvy"></param>
        /// <returns></returns>
        public static decimal Kwoka_Dominance_Index(IEnumerable<SmlouvyForIndex> smlouvy, decimal prumHodnotaSmluv)
        {
            if (smlouvy.Count() == 1)
                return 1;
            if (prumHodnotaSmluv == 0)
                prumHodnotaSmluv = 1;
            decimal total = smlouvy.Select(m => m.HodnotaSmlouvy == 0 ? prumHodnotaSmluv : m.HodnotaSmlouvy).Sum();

            var groupedPerDodavatel = smlouvy
                .GroupBy(k => k.Dodavatel, m => m, (k, v) => new { dodavatel = k, sumVal = v.Sum(m => m.HodnotaSmlouvy == 0 ? prumHodnotaSmluv : m.HodnotaSmlouvy) })
                .OrderByDescending(o => o.sumVal)
                .Select((m, i) => new { idx = i + 1, dodavatel = m.dodavatel, trznipodil = m.sumVal / total, sumVal = m.sumVal })
                .ToList();

            decimal kwoka = 0;
            for (int i = 0; i < groupedPerDodavatel.Count() - 1; i++)
            {
                decimal SiSi_1 = (groupedPerDodavatel[i].trznipodil - groupedPerDodavatel[i + 1].trznipodil);
                kwoka = kwoka + SiSi_1 * SiSi_1;
            }

            return kwoka;
        }

    }

}
