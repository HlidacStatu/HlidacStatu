using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Util
{
    public static partial class RenderData
    {
        public static class Vysledky
        {
            public static string PocetVysledku(Nest.TotalHits hits, bool shortText = true, CapitalizationStyle style = CapitalizationStyle.NoChange)
            {
                if (shortText)
                {
                    if (hits == null)
                        return "0";
                    if (hits.Relation == Nest.TotalHitsRelation.EqualTo)
                        return hits.Value.ToString();
                    else
                        return $"{hits.Value}+";
                }
                else
                {
                    if (hits == null)
                        return PocetVysledku(0);
                    if (hits.Relation == Nest.TotalHitsRelation.EqualTo)
                        return PocetVysledku(hits.Value);
                    else
                        return Capitalize("více než " + PocetVysledku(hits.Value),style);
                }
            }
            public static string NalezenoPocetVysledku(Nest.TotalHits hits, CapitalizationStyle style = CapitalizationStyle.NoChange)
            {
                if (hits == null)
                    return NalezenoPocetVysledku(0);
                if (hits.Relation == Nest.TotalHitsRelation.EqualTo)
                    return NalezenoPocetVysledku(hits.Value);
                else
                    return Capitalize(Devmasters.Core.Lang.Plural.GetWithZero((int)hits.Value, "nebyl nalezen žádný výsledek", "byl nalezen jeden výsledek", "byly nalezeny {0:### ### ##0} výsledky", "bylo nalezeno více než {0:### ### ##0} výsledků"), style);
            }
            public static string NasliJsmeVysledky(Nest.TotalHits hits, CapitalizationStyle style = CapitalizationStyle.NoChange)
            {
                if (hits == null)
                    return NasliJsmeVysledky(0);
                if (hits.Relation == Nest.TotalHitsRelation.EqualTo)
                    return NasliJsmeVysledky(hits.Value, style);
                else
                    return Capitalize(Devmasters.Core.Lang.Plural.GetWithZero((int)hits.Value, "nenašli jsme žádný výsledek", "našli jsme jeden výsledek", "našli jsme {0:### ### ##0} výsledky", "našli jsme více než {0:### ### ##0} výsledků"), style);
            }
            public static string PocetZaznamu(Nest.TotalHits hits, CapitalizationStyle style = CapitalizationStyle.NoChange)
            {
                if (hits == null)
                    return PocetZaznamu(0);
                if (hits.Relation == Nest.TotalHitsRelation.EqualTo)
                    return PocetZaznamu(hits.Value);
                else
                    return Capitalize(Devmasters.Core.Lang.Plural.GetWithZero((int)hits.Value, "žádný záznam", "jeden záznam", "{0:### ### ##0} záznamy", "více než {0:### ### ##0} záznamů"), style);
            }
            public static string PocetSmluv(Nest.TotalHits hits, CapitalizationStyle style = CapitalizationStyle.NoChange)
            {
                if (hits == null)
                    return PocetSmluv(0);
                if (hits.Relation == Nest.TotalHitsRelation.EqualTo)
                    return PocetSmluv(hits.Value);
                else
                    return Capitalize(Devmasters.Core.Lang.Plural.GetWithZero((int)hits.Value, "0 smluv", "jedna smlouva", "{0:### ### ##0} smlouvy", "více než {0:### ### ##0} smluv"),style);
            }

            public static string PocetVysledku(long pocet, CapitalizationStyle style = CapitalizationStyle.NoChange)
            {
                return Capitalize(
                    Devmasters.Core.Lang.Plural.GetWithZero((int)pocet, "žádný výsledek", "jeden výsledek", "{0:### ### ##0} výsledky", "{0:### ### ##0} výsledků")
                    , style);
            }
            public static string NalezenoPocetVysledku(long pocet, CapitalizationStyle style = CapitalizationStyle.NoChange)
            {
                return Capitalize(
                    Devmasters.Core.Lang.Plural.GetWithZero((int)pocet, "nebyl nalezen žádný výsledek", "byl nalezen jeden výsledek", "byly nalezeny {0:### ### ##0} výsledky", "bylo nalezeno {0:### ### ##0} výsledků")
                    , style);
            }
            public static string NasliJsmeVysledky(long pocet, CapitalizationStyle style = CapitalizationStyle.NoChange)
            {
                return Capitalize(
                    Devmasters.Core.Lang.Plural.GetWithZero((int)pocet, "nenašli jsme žádný výsledek", "našli jsme jeden výsledek", "našli jsme {0:### ### ##0} výsledky", "našli jsme {0:### ### ##0} výsledků")
                    , style);
            }
            public static string PocetZaznamu(long pocet, CapitalizationStyle style = CapitalizationStyle.NoChange)
            {
                return Capitalize(
                    Devmasters.Core.Lang.Plural.GetWithZero((int)pocet, "žádný záznam", "jeden záznam", "{0:### ### ##0} záznamy", "{0:### ### ##0} záznamů")
                    , style);
            }
            public static string PocetSmluv(long pocet, CapitalizationStyle style = CapitalizationStyle.NoChange)
            {
                return Capitalize(
                    Devmasters.Core.Lang.Plural.GetWithZero((int)pocet, "0 smluv", "jedna smlouva", "{0:### ### ##0} smlouvy", "{0:### ### ##0} smluv")
                    , style);
            }


        }
    }
}
