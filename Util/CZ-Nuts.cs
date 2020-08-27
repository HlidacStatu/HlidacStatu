using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Util
{
    public class CZ_Nuts
    {

        public static Dictionary<string, string> Kraje = new Dictionary<string, string>()
        {
{"CZ010","Hlavní město Praha"},
{"CZ031","Jihočeský kraj"},
{"CZ064","Jihomoravský kraj"},
{"CZ041","Karlovarský kraj"},
{"CZ063","Kraj Vysočina"},
{"CZ052","Královéhradecký kraj"},
{"CZ051","Liberecký kraj"},
{"CZ080","Moravskoslezský kraj"},
{"CZ071","Olomoucký kraj"},
{"CZ053","Pardubický kraj"},
{"CZ032","Plzeňský kraj"},
{"CZ020","Středočeský kraj"},
{"CZ042","Ústecký kraj"},
{"CZ072","Zlínský kraj"}
        };

        public static string Nace2Kraj(string nace, string ifUnknown = "")
        {
            nace = nace.ToUpper();
            if (Kraje.ContainsKey(nace))
                return Kraje[nace];
            else
                return "";

        }
    }
}
