using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Util
{
    public static partial class DataValidators
    {
        public class ZahranicniAdresa
        {
            public ZahranicniAdresa(string adresa)
            {
                this.Adresa = adresa;
                Country = zahranicniAdresa(adresa);
            }

            public string Adresa { get; private set; }
            public string Country { get; set; }

            public  bool IsZahranicniAdresa()
            {

                return Country != null;
            }

            private string zahranicniAdresa(string adresa)
            {
                if (!string.IsNullOrEmpty(adresa))
                {
                    string dadresa = Devmasters.TextUtil.RemoveDiacritics(adresa).ToLower().Trim();



                    foreach (var stat3 in ciziStaty.Where(v => v.Value != "CZ"))
                    {
                        string stat = "(\\s|,|;)" + stat3.Key.Replace(" ", "\\s*") + "($|\\s)$";
                        if (System.Text.RegularExpressions.Regex.IsMatch(dadresa, stat, Util.Consts.DefaultRegexQueryOption))
                        {
                            if (CeskaAdresaObec(dadresa) != null)
                                return null;

                            return stat3.Value;
                        }
                    }
                }
                //czech sk adresa
                if (SKAdresaObec(adresa) != null && CeskaAdresaObec(adresa) == null)
                {
                    return "SK";
                }
                return null;
            }
        }

    }
}
