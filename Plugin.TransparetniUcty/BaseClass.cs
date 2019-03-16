using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Plugin.TransparetniUcty
{
    public class SimpleBankovniUcet : IBankovniUcet
    {
        public string Subjekt { get; set; }
        public string Nazev { get; set; }
        public string TypSubjektu { get; set; }
        public string Url { get; set; }
        public string CisloUctu { get; set; }
        public string Mena { get; set; }
        public int Active { get; set; }
        public DateTime LastSuccessfullParsing { get; set; }
        public int numTypUctu { get; set; }
    }
    public class SimpleBankovniPolozka : IBankovniPolozka
    {
        public string Id { get; set; }
        public string CisloUctu { get; set; }
        public DateTime Datum { get; set; }
        public string PopisTransakce { get; set; }
        public string NazevProtiuctu { get; set; }
        public string CisloProtiuctu { get; set; }
        public string ZpravaProPrijemce { get; set; }
        public string VS { get; set; }
        public string KS { get; set; }
        public string SS { get; set; }
        public decimal Castka { get; set; }
        public string AddId { get; set; }
        public string ZdrojUrl { get; set; }
    }
}
