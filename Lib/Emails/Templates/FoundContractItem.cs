using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Emails.Templates
{
    public class FoundContractItem
    {
        public string PlatceTxt {get;set;}
        public string PlatceHtml { get; set; }
        public string PrijemceTxt { get; set; }
        public string PrijemceHtml { get; set; }
        public string Cena { get; set; }
        public string Predmet { get; set; }
        public string Id { get; set; }
        public string[] Issues { get; set; }

    }
}
