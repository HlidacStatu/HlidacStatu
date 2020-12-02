using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Analytics
{
    public class SimpleStat : CoreStat, IAddable<SimpleStat>
    {
        public long Pocet { get; set; }
        public decimal CelkemCena { get; set; }

        public void Add(long pocet, decimal cena)
        {
            this.Pocet = this.Pocet + pocet;
            this.CelkemCena = this.CelkemCena + cena;
        }

        public SimpleStat Add(SimpleStat other)
        {
            return new SimpleStat() {
                CelkemCena = this.CelkemCena + other?.CelkemCena ?? 0,
                Pocet = this.Pocet + other?.Pocet ?? 0
                };
        }

        public override int NewSeasonStartMonth()
        {
            return 1;
        }

        public override int UsualFirstYear()
        {
            return 1990;
        }
    }
}
