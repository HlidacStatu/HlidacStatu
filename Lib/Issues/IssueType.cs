using System;
using System.Linq;
using System.Collections.Generic;

namespace HlidacStatu.Lib.Issues
{
    public static class IssueType
    {
        public enum IssueTypes
        {
            Neuveden_dodavatel = 1,
            Budouci_datum_uzavreni = 2,
            Neplatny_datum_uzavreni_smlouvy = 3,

            Chybi_ICO = 5,
            Vadne_ICO = 6,
            Chybi_datova_schranka = 7,
            Chybi_predmet_smlouvy = 8,

            Firma_Cizi_Stat = 16,
            Osoba = 17,
            NeverejnyUdaj = 18,

            Neexistujici_ICO = 10,
            Chybi_identifikace_smluvni_strany = 11,
            Stejne_strany_smlouvy = 12,
            Chybne_strany_smlouvy = 13,
            Firma_vznikla_az_po = 14,
            Firma_vznikla_kratce_pred = 15,
            SmlouvaUzavrena_s_NespolehlivymPlatcemDPH = 19,
            SmlouvaZverejnenaPozde = 20,
            SmlouvaZverejnenaPozdeNezacalaPlatit = 21,
            Zcela_Chybi_identifikace_smluvni_strany = 22,
            SmlouvaZverejnenaPozde_DodatekVynutilPublikaci = 23,

            Nulova_hodnota_smlouvy = 100,
            Cena_bez_DPH_nulova = 101,
            Cena_s_DPH_nulova = 102,
            Zaporna_cena_bez_DPH = 103,
            Zaporna_cena_s_DPH = 104,
            Neplatna_cena = 105,
            bezDPH_x_DPH = 106,
            Neplatna_cena_vypocetDPH = 107,
            Nulova_hodnota_smlouvy_u_dodatku = 108,
            Nulova_hodnota_smlouvy_ostatni = 109,

            NecitelnostSmlouvy = 200,

        }
        public static ImportanceLevel IssueImportance(int issuetypeId, ImportanceLevel? forceDifferent = null)
        {
            return IssueImportance((IssueTypes)issuetypeId, forceDifferent);
        }
        public static ImportanceLevel IssueImportance(IssueTypes issuetype, ImportanceLevel? forceDifferent = null)
        {
            if (forceDifferent.HasValue)
                return forceDifferent.Value;
            else
            {
                switch (issuetype)
                {
                    case IssueTypes.Neuveden_dodavatel:
                    case IssueTypes.SmlouvaZverejnenaPozdeNezacalaPlatit:
                    case IssueTypes.Neplatny_datum_uzavreni_smlouvy:
                    case IssueTypes.Chybi_predmet_smlouvy:
                    case IssueTypes.Zcela_Chybi_identifikace_smluvni_strany:
                        return ImportanceLevel.Fatal;

                    case IssueTypes.Vadne_ICO:
                    case IssueTypes.Stejne_strany_smlouvy:
                    case IssueTypes.Chybne_strany_smlouvy:
                    case IssueTypes.Firma_vznikla_az_po:
                    case IssueTypes.SmlouvaZverejnenaPozde:
                    case IssueTypes.Neplatna_cena:
                        return ImportanceLevel.Major;

                    case IssueTypes.NecitelnostSmlouvy:
                    case IssueTypes.Budouci_datum_uzavreni:
                    case IssueTypes.NeverejnyUdaj:
                    case IssueTypes.Neexistujici_ICO:
                    case IssueTypes.Chybi_identifikace_smluvni_strany:
                    case IssueTypes.SmlouvaUzavrena_s_NespolehlivymPlatcemDPH:
                    case IssueTypes.Nulova_hodnota_smlouvy:
                        return ImportanceLevel.Minor;

                    case IssueTypes.Cena_bez_DPH_nulova:
                    case IssueTypes.Cena_s_DPH_nulova:
                    case IssueTypes.Zaporna_cena_bez_DPH:
                    case IssueTypes.Zaporna_cena_s_DPH:
                    case IssueTypes.Nulova_hodnota_smlouvy_u_dodatku:
                    case IssueTypes.Nulova_hodnota_smlouvy_ostatni:
                    case IssueTypes.SmlouvaZverejnenaPozde_DodatekVynutilPublikaci:
                        return ImportanceLevel.Formal;

                    case IssueTypes.Chybi_ICO:
                    case IssueTypes.Chybi_datova_schranka:
                    case IssueTypes.Firma_Cizi_Stat:
                    case IssueTypes.Osoba:
                    case IssueTypes.Firma_vznikla_kratce_pred:
                        return ImportanceLevel.Ok;


                    case IssueTypes.bezDPH_x_DPH:
                    case IssueTypes.Neplatna_cena_vypocetDPH:
                        return ImportanceLevel.NeedHumanReview;
                    default:
                        throw new NotImplementedException();
                }
            }
        }


    }
}
