using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HlidacStatu.Lib.Issues;
using HlidacStatu.Lib;

namespace HlidacStatu.Plugin.IssueAnalyzers
{
    public class Price : IIssueAnalyzer
    {

        public string Description
        {
            get
            {
                return "Cena";
            }

        }

        public string Name
        {
            get
            {
                return "Kontrola cen";
            }

        }

        public IEnumerable<Issue> FindIssues(Lib.Data.Smlouva item)
        {
            List<Issue> issues = new List<Issue>();
            if (item.ciziMena != null && item.ciziMena.hodnota > 0)
            {
                return issues;
            }


            if (item.hodnotaBezDph.HasValue == false && item.hodnotaVcetneDph.HasValue == false)
            {
                issues.Add(
                    new Issue(this, (int)IssueType.IssueTypes.Nulova_hodnota_smlouvy,"Nulová hodnota smlouvy", "Smlouva nemá v metadatech uvedenu cenu. Utajení hodnoty smlouvy je možné pouze v odůvodněných případech, což při této kontrole nehodnotíme.")
                    );
            }

            if (item.hodnotaBezDph.HasValue && (item.hodnotaBezDph.HasValue == false || item.hodnotaBezDph == 0))
            {
                issues.Add(
                    new Issue(this, (int)IssueType.IssueTypes.Cena_bez_DPH_nulova, "Cena bez DPH nulová", "Smlouva uvádí nulovou cenu bez DPH.",null, new { hodnotaBezDph = item.hodnotaBezDph })
                    );
            }

            if (item.hodnotaVcetneDph.HasValue && (item.hodnotaVcetneDph.HasValue == false || item.hodnotaVcetneDph== 0))
            {
                issues.Add(
                    new Issue(this, (int)IssueType.IssueTypes.Cena_s_DPH_nulova, "Cena s DPH nulová", "Smlouva uvádí nulovou cenu s DPH.",null, new { hodnotaVcetneDph = item.hodnotaVcetneDph })
                    );
            }

            if (item.hodnotaBezDph < 0)
            {
                issues.Add(
                    new Issue(this, (int)IssueType.IssueTypes.Zaporna_cena_bez_DPH, "Záporná cena bez DPH", "Záporná cena je možná pouze u dodatků smlouvy, pouze se dodatkem snižuje hodnota smlouvy. Jinak jde o neplatnou smlouvu.", null, new { hodnotaBezDph = item.hodnotaBezDph })
                    );
            }
            if (item.hodnotaVcetneDph < 0)
            {
                issues.Add(
                    new Issue(this, (int)IssueType.IssueTypes.Zaporna_cena_s_DPH, "Záporna cena s DPH", "Záporná cena je možná pouze u dodatků smlouvy, pouze se dodatkem snižuje hodnota smlouvy. Jinak jde o neplatnou smlouvu.", null, new { hodnotaVcetneDph = item.hodnotaVcetneDph })
                    );
            }

            if (item.hodnotaBezDph.HasValue && item.hodnotaVcetneDph.HasValue)
            {
                //kontrola vypoctu DPH
                decimal bezDPH = item.hodnotaBezDph.Value;
                decimal sDPH = item.hodnotaVcetneDph.Value;

                if (bezDPH > sDPH && (bezDPH>0 && sDPH>0))
                {
                    issues.Add(
                        new Issue(this, (int)IssueType.IssueTypes.Neplatna_cena, "Neplatná cena", "Cena bez DPH je větší než cena s DPH.", null, new { hodnotaVcetneDph = item.hodnotaVcetneDph, hodnotaBezDph = item.hodnotaBezDph })
                        );

                }
                else if (bezDPH == sDPH) //stejne DPH
                {
                    //over, zda je v registru platcu DPH
                    issues.Add(
                        new Issue(this, (int)IssueType.IssueTypes.bezDPH_x_DPH, "bezDPH = sDPH", "", null, new { hodnotaVcetneDph = item.hodnotaVcetneDph, hodnotaBezDph = item.hodnotaBezDph })
                        );
                }
                else
                {
                    //TODO
                    bool valid = false;
                    decimal[] validVats = new decimal[] { 1.21m, 1.15m, 1.10m };
                    foreach (var vat in validVats)
                    {
                        var diff = Math.Round(bezDPH * vat);
                        if (Math.Abs(sDPH - diff) < 5)
                            valid = true;
                    }
                    if (valid == false)
                    {
                        issues.Add(
                            new Issue(this, (int)IssueType.IssueTypes.Neplatna_cena_vypocetDPH, "Neplatná cena", "Rozdíl ceny s DPH a bez DPH neodpovídá žádné sazbě DPH.", null, new { hodnotaVcetneDph = item.hodnotaVcetneDph, hodnotaBezDph = item.hodnotaBezDph })
                            );

                    }
                }                
            }

            return issues;
        }

    }
}
