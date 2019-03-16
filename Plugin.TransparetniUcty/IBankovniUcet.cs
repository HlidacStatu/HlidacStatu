using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Plugin.TransparetniUcty
{
    public interface IBankovniUcet
    {
        string Subjekt { get; set; }
        string Nazev { get; set; }
        string TypSubjektu { get; set; }
        string Url { get; set; }
        string CisloUctu { get; set; }
        string Mena { get; set; }

        int Active { get; set; }
        DateTime LastSuccessfullParsing { get; set; }

        //[NiceDisplayName("Neznámý typ účtu")]
        //Neurcen = 0,
        //[NiceDisplayName("Provozní účet")]
        //ProvozniUcet = 1,
        //[NiceDisplayName("Volební účet")]
        //VolebniUcet = 2
        int numTypUctu { get; set; }


    }
}
