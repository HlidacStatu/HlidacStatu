using System;

namespace HlidacStatu.Lib.Data.External.RPP
{
    public class seznamkatprirazenychkovm
    {
        public Clenkategorieovm clenKategorieOvm { get; set; }
        public KategorieOVM kategorieOvm { get; set; }

        public class Clenkategorieovm
        {
            public int id { get; set; }
            public string spravnost { get; set; }
            public DateTime zarazeniOd { get; set; }
            public Ovm ovm { get; set; }
        }

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



