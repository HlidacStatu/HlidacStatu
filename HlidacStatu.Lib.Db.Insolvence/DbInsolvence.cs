using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Db.Insolvence
{
    public class DbInsolvence
    {
        public Rizeni Rizeni { get; set; }
        public IEnumerable<Dokumenty> Dokumenty { get; set; } = new Dokumenty[] { };
        public IEnumerable<Dluznici> Dluznici { get; set; } = new Dluznici[] { };
        public IEnumerable<Veritele> Veritele { get; set; } = new Veritele[] { };
        public IEnumerable<Spravci> Spravci { get; set; } = new Spravci[] { };

    }
}
