using Devmasters.Core;
using HlidacStatu.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Devmasters.Core;
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
                public string Tag { get; set; } = "";
            }


            private static CouchbaseCacheManager<Zatrideni.Item[], StatniOrganizaceObor> instanceByZatrideni
           = CouchbaseCacheManager<Zatrideni.Item[], StatniOrganizaceObor>.GetSafeInstance("oboryByObor", GetSubjektyDirect,
                TimeSpan.FromDays(5)
           );


            [ShowNiceDisplayName()]
            public enum StatniOrganizaceObor
            {
                [Disabled]
                Ostatni = 0,

                Zdravotni_ustavy = 158,
                Hasicsky_zachranny_sbor = 135,
                Krajske_hygienicke_stanice = 113,
                Krajska_statni_zastupitelstvi = 143,
                Krajske_soudy = 107,
                Soudy = 123,
                Statutarni_mesta = 103,
                Verejne_vysoke_skoly = 1129,
                [NiceDisplayName("Krajská ředitelství policie")]
                Krajska_reditelstvi_policie = 145,
                [NiceDisplayName("Státní fondy")]
                Statni_fondy = 980,
                [NiceDisplayName("Okresní správy sociálního zabezpečení")]
                OSSZ = 128,
                [NiceDisplayName("Kraje a hl. m. Praha")]
                Kraje_Praha = 12,
                [NiceDisplayName("Obce s rozšířenou působností")]
                Obce_III_stupne = 11,
                [NiceDisplayName("Zdravotní pojišťovny")]
                Zdravotni_pojistovny = 186,
                [NiceDisplayName("Katastrální úřady")]
                Katastralni_urady = 127,
                Ministerstva = 2926,
                [NiceDisplayName("Organizační složky státu")]
                Organizacni_slozky_statu = 191,
                [NiceDisplayName("Další ústřední orgány státní správy")]
                Dalsi_ustredni_organy_statni_spravy = 104,
                [NiceDisplayName("Celní úřady")]
                Celni_urady = 105,

                [NiceDisplayName("Finanční úřady")]
                Financni_urady = 109,

                [NiceDisplayName("Konzervatoře")]
                Konzervatore = 1120,
                [NiceDisplayName("Městské části Prahy")]
                Mestske_casti_Prahy = 600,

                Nemocnice = 10001,
                Velke_nemocnice = 10002,
                Fakultni_nemocnice = 10003,
                [NiceDisplayName("Krajské správy silnic")]
                Krajske_spravy_silnic = 10004,
                [NiceDisplayName("Dopravní podniky měst")]
                Dopravni_podniky = 10005,



                [Disabled]
                OVM_pro_evidenci_skutecnych_majitelu = 3106,
            }

            /*
             Nemocnice:
             select distinct f.ICO, f.Jmeno from firma f inner join Firma_NACE fn on f.ICO = fn.ICO where nace like '86%' and f.IsInRS = 1

             */

            public static Zatrideni.Item[] Subjekty(StatniOrganizaceObor obor)
            {
                return instanceByZatrideni.Get(obor);
            }


            private static Zatrideni.Item[] GetSubjektyDirect(StatniOrganizaceObor obor)
            {
                string[] icos = null;
                switch (obor)
                {
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
                    case StatniOrganizaceObor.Obce_III_stupne:
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
                        icos = GetSubjektyFromRPP((int)obor);
                        break;
                    case StatniOrganizaceObor.Nemocnice:
                        icos = GetSubjektyFromSQL("select distinct fn.ICO from Firma_NACE where nace like '86%' and f.IsInRS = 1");
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
                        icos = new string[] { };
                        break;
                    case StatniOrganizaceObor.Ostatni:
                        icos = new string[] { };
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (icos.Count() == 0)
                    return new Item[] { };
                else
                {
                    var ret = new System.Collections.Concurrent.BlockingCollection<Item>();
                    Devmasters.Core.Batch.Manager.DoActionForAll<string>(icos,
                        ic =>
                        {
                            var f = Firmy.Get(ic);
                            if (f.PatrimStatu())
                            {
                                ret.Add(new Item() { Ico = f.ICO, Jmeno = f.Jmeno, Tag = Util.CZ_Nuts.Nace2Kraj(f.KrajId) });
                            }
                            return new Devmasters.Core.Batch.ActionOutputData();
                        }, true);

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
        }
    }
}
