using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data.External.RPP
{
    public class OVMSimple
    {

        public int id { get; set; }
        public string spravnost { get; set; }
        public string stavZarazeni { get; set; }
        public DateTime zarazeniOd { get; set; }
        public Ovm ovm { get; set; }


        public class Ovm
        {
            public int id { get; set; }
            public string kodOvm { get; set; }
            public string nazevOvm { get; set; }
            public DateTime platnostOd { get; set; }
            public DateTime pusobnostOd { get; set; }
            public string spravnost { get; set; }
            public Stav stav { get; set; }
        }

        public class Stav
        {
            public int id { get; set; }
            public string kod { get; set; }
            public string nazev { get; set; }
            public string popis { get; set; }
            public int poradi { get; set; }
        }


    }
}
