using HlidacStatu.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Devmasters.Enums;
using HlidacStatu.Util.Cache;

namespace HlidacStatu.Lib.Data
{
    public partial class Firma
        : Bookmark.IBookmarkable, ISocialInfo
    {
        public class Zatrideni
        {
            public class Item
            {
                public string Ico { get; set; }
                public string Jmeno { get; set; }
                public string KrajId { get; set; } = "";
                public string Kraj { get; set; } = "";
                public string Group { get; set; } = "";
            }


            private static CouchbaseCacheManager<Zatrideni.Item[], StatniOrganizaceObor> instanceByZatrideni
           = CouchbaseCacheManager<Zatrideni.Item[], StatniOrganizaceObor>.GetSafeInstance("oboryByObor", GetSubjektyDirect,
                TimeSpan.FromDays(5)
           );

            [Groupable]
            [ShowNiceDisplayName()]
            [Sortable(SortableAttribute.SortAlgorithm.BySortValueAndThenAlphabetically)]
            public enum StatniOrganizaceObor
            {
                [Disabled]
                Ostatni = 0,

                [Disabled]
                [GroupValue("Zdravotnictví")]
                [NiceDisplayName("Zdravotní ústavy")]
                Zdravotni_ustavy = 158,
                [GroupValue("Zdravotnictví")]
                [NiceDisplayName("Zdravotní pojišťovny")]
                Zdravotni_pojistovny = 186,
                [GroupValue("Zdravotnictví")]
                [NiceDisplayName("Nemocnice")]
                Nemocnice = 10001,
                [GroupValue("Zdravotnictví")]
                [NiceDisplayName("Velké nemocnice v ČR")]
                Velke_nemocnice = 10002,
                [GroupValue("Zdravotnictví")]
                [NiceDisplayName("Fakultní nemocnice")]
                Fakultni_nemocnice = 10003,

                [Disabled]
                [GroupValue("Justice")]
                [NiceDisplayName("Krajská státní zastupitelství")]
                Krajska_statni_zastupitelstvi = 143,
                [GroupValue("Justice")]
                [NiceDisplayName("Krajské soudy")]
                Krajske_soudy = 107,
                [Disabled]
                [GroupValue("Justice")]
                [NiceDisplayName("Všechny soudy")]
                Soudy = 123,

                [GroupValue("Samospráva")]
                [NiceDisplayName("Kraje a hl. m. Praha")]
                Kraje_Praha = 12,
                [GroupValue("Samospráva")]
                [NiceDisplayName("Obce s rozšířenou působností")]
                Obce_III_stupne = 11,
                [GroupValue("Samospráva")]
                [NiceDisplayName("Statutární města")]
                Statutarni_mesta = 103,
                [GroupValue("Samospráva")]
                [NiceDisplayName("Městské části Prahy")]
                Mestske_casti_Prahy = 600,


                [GroupValue("Státní úřady a organizace")]
                [NiceDisplayName("Hasičscké záchranné sbory")]
                Hasicsky_zachranny_sbor = 135,
                [Disabled]
                [GroupValue("Státní úřady a organizace")]
                [NiceDisplayName("Krajské hygienické stanice")]
                Krajske_hygienicke_stanice = 113,
                [GroupValue("Státní úřady a organizace")]
                [NiceDisplayName("Krajská ředitelství policie")]
                Krajska_reditelstvi_policie = 145,
                [GroupValue("Státní úřady a organizace")]
                [NiceDisplayName("Státní fondy")]
                Statni_fondy = 980,
                [Disabled]
                [GroupValue("Státní úřady a organizace")]
                [NiceDisplayName("Okresní správy sociálního zabezpečení")]
                OSSZ = 128,
                [Disabled]
                [GroupValue("Státní úřady a organizace")]
                [NiceDisplayName("Katastrální úřady")]
                Katastralni_urady = 127,
                [GroupValue("Státní úřady a organizace")]
                [NiceDisplayName("Ministerstva")]
                Ministerstva = 2926,
                [GroupValue("Státní úřady a organizace")]
                [NiceDisplayName("Organizační složky státu")]
                Organizacni_slozky_statu = 191,

                [GroupValue("Státní úřady a organizace")]
                [NiceDisplayName("Všechny ústřední orgány státní správy")]
                Vsechny_ustredni_organy_statni_spravy = 10104,

                [Disabled()]
                [GroupValue("Státní úřady a organizace")]
                [NiceDisplayName("Další ústřední orgány státní správy")]
                Dalsi_ustredni_organy_statni_spravy = 104,
                [Disabled]
                [GroupValue("Státní úřady a organizace")]
                [NiceDisplayName("Celní úřady")]
                Celni_urady = 105,
                [Disabled]
                [GroupValue("Státní úřady a organizace")]
                [NiceDisplayName("Finanční úřady")]
                Financni_urady = 109,
                [Disabled]
                [GroupValue("Státní úřady a organizace")]
                [NiceDisplayName("Výjimky z registru smluv")]
                Vyjimky_RS = 10100,


                [GroupValue("Školství")]
                [NiceDisplayName("Veřejné vysoké školy")]
                Verejne_vysoke_skoly = 1129,

                [Disabled]
                [GroupValue("Školství")]
                [NiceDisplayName("Konzervatoře")]
                Konzervatore = 1120,

                [GroupValue("Služby")]
                [NiceDisplayName("Krajské správy silnic")]
                Krajske_spravy_silnic = 10004,
                [GroupValue("Služby")]
                [NiceDisplayName("Dopravní podniky měst")]
                Dopravni_podniky = 10005,
                [GroupValue("Služby")]
                [NiceDisplayName("Technické služby")]
                Technicke_sluzby = 10006,

                //Sportovni_zarizeni = 10007,

                [Disabled]
                [GroupValue("Služby")]
                [NiceDisplayName("Domovy důchodců")]
                Domov_duchodcu = 10008,
                //Veznice = 10009,
                //Pamatky = 10010, //narodni pamatkovy ustav
                //Zachranne_sluzby = 10011,

                //spolovny, prehrady, zasobarny pitne vody
                // vodarny
                // kulturni zarizeni, divadla, kina, strediska
                // spravy povodi
                // ZOO
                // hrbitovy, krematoria
                // lazne, blazince
                // regionalni rozvojove agentury, vinarstvi, ...
                // domy deti a mladeze


                [Disabled]
                OVM_pro_evidenci_skutecnych_majitelu = 3106,

                [Disabled]
                Vse = -1,
            }

            /*
             Nemocnice:
             select distinct f.ICO, f.Jmeno from firma f inner join Firma_NACE fn on f.ICO = fn.ICO where nace like '86%' and f.IsInRS = 1

             */

            public static Zatrideni.Item[] Subjekty(StatniOrganizaceObor obor)
            {
                return instanceByZatrideni.Get(obor);
            }


            static object _getSubjektyDirectLock = new object();
            private static Zatrideni.Item[] GetSubjektyDirect(StatniOrganizaceObor obor)
            {
                string[] icos = null;
                string sql = "";
                switch (obor)
                {
                    case StatniOrganizaceObor.Vse:
                        icos = GetAllSubjektyFromRPP();
                        break;
                    case StatniOrganizaceObor.Zdravotni_ustavy:
                    case StatniOrganizaceObor.Hasicsky_zachranny_sbor:
                    case StatniOrganizaceObor.Krajske_hygienicke_stanice:
                    case StatniOrganizaceObor.Krajska_statni_zastupitelstvi:
                    case StatniOrganizaceObor.Krajske_soudy:
                    case StatniOrganizaceObor.Soudy:
                    case StatniOrganizaceObor.Statutarni_mesta:
                    case StatniOrganizaceObor.Verejne_vysoke_skoly:
                    case StatniOrganizaceObor.Krajska_reditelstvi_policie:
                    case StatniOrganizaceObor.Statni_fondy:
                    case StatniOrganizaceObor.OSSZ:
                    case StatniOrganizaceObor.Kraje_Praha:
                    case StatniOrganizaceObor.Zdravotni_pojistovny:
                    case StatniOrganizaceObor.Katastralni_urady:
                    case StatniOrganizaceObor.Ministerstva:
                    case StatniOrganizaceObor.Organizacni_slozky_statu:
                    case StatniOrganizaceObor.Dalsi_ustredni_organy_statni_spravy:
                    case StatniOrganizaceObor.Celni_urady:
                    case StatniOrganizaceObor.Financni_urady:
                    case StatniOrganizaceObor.Konzervatore:
                    case StatniOrganizaceObor.Mestske_casti_Prahy:
                    case StatniOrganizaceObor.OVM_pro_evidenci_skutecnych_majitelu:
                    case StatniOrganizaceObor.Obce_III_stupne:
                        icos = GetSubjektyFromRPP((int)obor);
                        break;
                    case StatniOrganizaceObor.Vsechny_ustredni_organy_statni_spravy:
                        icos = GetSubjektyFromRPP((int)StatniOrganizaceObor.Dalsi_ustredni_organy_statni_spravy)
                            .Concat(GetSubjektyFromRPP((int)StatniOrganizaceObor.Ministerstva))
                            .ToArray();
                        break;
                    case StatniOrganizaceObor.Nemocnice:
                        sql = @"select distinct ico from (
                            select f.ICO from Firma f where Jmeno like N'%nemocnice%' and f.IsInRS = 1
                            union
                            select distinct f.ico
                                from Firma_NACE fn
                                join firma f on f.ICO = fn.ICO
                                where (nace like '861%' or NACE like '862%') and f.IsInRS = 1
                                and f.Kod_PF not in (105, 101, 801, 601)
                            ) as f";
                        icos = GetSubjektyFromSQL(sql);
                        break;
                    case StatniOrganizaceObor.Velke_nemocnice:
                        icos = "00064165,00064173,00064203,00098892,00159816,00179906,00669806,00843989,25488627,26365804,27283933,27661989,65269705,27283518,26000202,00023736,00023884,27256391,61383082,27256537,00023001,27520536,26068877,47813750,00064211,00209805,27660915,00635162,27256456,00090638,00092584,00064190"
                            .Split(',');
                        break;
                    case StatniOrganizaceObor.Fakultni_nemocnice:
                        icos = "65269705,00179906,00669806,00098892,00843989,00064165,00064203,00159816,00064173"
                            .Split(',');
                        break;
                    case StatniOrganizaceObor.Krajske_spravy_silnic:
                        icos = "00066001,00090450,70947023,70946078,72053119,00080837,70971641,27502988,00085031,70932581,70960399,00095711,26913453,03447286,25396544,60733098"
                            .Split(',');
                        break;
                    case StatniOrganizaceObor.Dopravni_podniky:
                        icos = "05792291,25095251,25136046,25137280,00005886,25166115,25164538,25220683,29099846,61058238,48364282,62240935,64053466,06231292,62242504,25013891,47311975,00079642,06873031,25267213,63217066,25512897,25508881,00100790,47676639,05724252,64610250,61974757,60730153"
                            .Split(',');
                        break;
                    case StatniOrganizaceObor.Technicke_sluzby:
                        sql = @"select ico from Firma f where Jmeno like N'technické služby%' and f.IsInRS = 1";
                        icos = GetSubjektyFromSQL(sql);
                        break;
                    case StatniOrganizaceObor.Domov_duchodcu:
                        sql = @"select ico from Firma f where Jmeno like N'%domov důchodců%' and f.IsInRs = 1";
                        icos = GetSubjektyFromSQL(sql);
                        break;
                    case StatniOrganizaceObor.Vyjimky_RS:

                        // Poslanecká sněmovna, Senát, Kancelář prezidenta republiky, Ústavní soud, Nejvyšší kontrolní úřad, 
                        //Kancelář veřejného ochránce práv a Úřad Národní rozpočtové rady
                        //CNB
                        icos = new string[] { "00006572", "63839407", "48136000", "48513687", "49370227", "70836981", "05553539", "48136450" };
                        break;
                    case StatniOrganizaceObor.Ostatni:
                        icos = new string[] { };
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                bool removeKraj = false;
                switch (obor)
                {
                    case StatniOrganizaceObor.Ostatni:
                    case StatniOrganizaceObor.Zdravotni_pojistovny:
                    case StatniOrganizaceObor.Fakultni_nemocnice:
                    case StatniOrganizaceObor.Krajska_statni_zastupitelstvi:
                    case StatniOrganizaceObor.Krajske_soudy:
                    case StatniOrganizaceObor.Kraje_Praha:
                    case StatniOrganizaceObor.Mestske_casti_Prahy:
                    case StatniOrganizaceObor.OSSZ:
                    case StatniOrganizaceObor.Ministerstva:
                    case StatniOrganizaceObor.Organizacni_slozky_statu:
                    case StatniOrganizaceObor.Vsechny_ustredni_organy_statni_spravy:
                    case StatniOrganizaceObor.Dalsi_ustredni_organy_statni_spravy:
                    case StatniOrganizaceObor.Financni_urady:
                    case StatniOrganizaceObor.Vyjimky_RS:
                    case StatniOrganizaceObor.Verejne_vysoke_skoly:
                    case StatniOrganizaceObor.Konzervatore:
                    case StatniOrganizaceObor.Krajske_spravy_silnic:
                    case StatniOrganizaceObor.OVM_pro_evidenci_skutecnych_majitelu:
                        removeKraj = true;
                        break;
                    default:
                        break;
                }

                if (icos.Count() == 0)
                    return new Item[] { };
                else
                {
                    var ret = new System.Collections.Generic.List<Item>();
                    Devmasters.Batch.Manager.DoActionForAll<string>(icos.Select(m=>m.Trim()).Distinct(),
                        ic =>
                        {
                            var f = Firmy.Get(ic);
                            
                            if (f.PatrimStatu())
                            {
                                lock (_getSubjektyDirectLock) 
                                {
                                    if (!ret.Any(ff => ff.Ico == f.ICO))
                                    {
                                        ret.Add(new Item()
                                        {
                                            Ico = f.ICO,
                                            Jmeno = f.Jmeno,
                                            KrajId = removeKraj ? "" : f.KrajId,
                                            Kraj = removeKraj ? "" : CZ_Nuts.Nace2Kraj(f.KrajId, "(neznamý)")
                                        });
                                    }
                                }
                            }
                            return new Devmasters.Batch.ActionOutputData();
                        }, !System.Diagnostics.Debugger.IsAttached);

                    return ret.ToArray();
                }
            }
            private static string[] GetSubjektyFromSQL(string sql)
            {
                return Lib.DirectDB
                    .GetList<string>(sql)
                    .ToArray();
            }

            private static string[] GetSubjektyFromRPP(int rpp_kategorie_id)
            {
                var res = Lib.ES.Manager.GetESClient_RPP_Kategorie()
                    .Get<Lib.Data.External.RPP.KategorieOVM>(rpp_kategorie_id.ToString());
                if (res.Found)
                    return res.Source.OVM_v_kategorii.Select(m => m.kodOvm).ToArray();

                return new string[] { };
            }

            private static string[] GetAllSubjektyFromRPP()
            {
                var res = Lib.ES.Manager.GetESClient_RPP_Kategorie()
                    .Search<Lib.Data.External.RPP.KategorieOVM>(s => s.Size(9000).Query(q => q.MatchAll()));
                if (res.IsValid)
                    return res.Hits
                        .Select(m => m.Source)
                        .SelectMany(m => m.OVM_v_kategorii.Select(ovm => ovm.kodOvm))
                        .Distinct()
                        .ToArray();

                return new string[] { };
            }

        }
    }
}
