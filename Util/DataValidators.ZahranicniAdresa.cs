using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HlidacStatu.Util
{
    public static partial class DataValidators
    {
        public class ZahranicniAdresa
        {
            static Dictionary<string, Regex> _zahr_adresa = new Dictionary<string, Regex>();

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

                        if (!_zahr_adresa.ContainsKey(stat3.Key))
                        {
                            string stat = "(\\s|,|;)" + stat3.Key.Replace(" ", "\\s*") + "($|\\s)$";
                            _zahr_adresa[stat3.Key] = new Regex(stat, RegexOptions.IgnoreCase
                                                                | RegexOptions.IgnorePatternWhitespace
                                                                | RegexOptions.Multiline
                                                                | RegexOptions.Compiled);
                        }
                        //(\s|,|;)mexiko($|\s)

                        if (_zahr_adresa[stat3.Key].IsMatch(dadresa))
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
