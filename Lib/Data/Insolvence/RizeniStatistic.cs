using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data.Insolvence
{
    public class RizeniStatistic
    {
        public RizeniStatistic(string spisovaZnacka, IEnumerable<string> filterDluzniciFromThisList = null)
            : this(Insolvence.LoadFromES(spisovaZnacka, false, false)?.Rizeni, filterDluzniciFromThisList)
        {
        }

        public RizeniStatistic(Rizeni rizeni, IEnumerable<string> filterDluzniciFromThisList = null)
        {
            if (rizeni == null)
                throw new ArgumentNullException("rizeni");
            this.SpisovaZnacka = rizeni.SpisovaZnacka;
            this.Zahajeni = rizeni.DatumZalozeni ?? new DateTime(1990, 1, 1);
            this.PosledniUpdate = rizeni.PosledniZmena;
            this.SpravciCount = rizeni.Spravci?.Count() ?? 0;
            this.DluzniciCount = rizeni.Dluznici?.Count() ?? 0;
            this.VeriteleCount = rizeni.Veritele?.Count() ?? 0;
            this.DokumentyCount = rizeni.Dokumenty?.Count() ?? 0;
            this.Stav = rizeni.StavRizeni();
            VybraniDluznici = rizeni.Dluznici
                .Where(m => filterDluzniciFromThisList == null || filterDluzniciFromThisList?.Contains(m.ICO) == true)
                .Select(m => m.ICO)
                .ToArray();
        }
        public string SpisovaZnacka { get; set; }
        public string Stav { get; set; }
        public DateTime Zahajeni { get; set; }
        public DateTime PosledniUpdate { get; set; }
        public TimeSpan DelkaRizeni { get { return PosledniUpdate - Zahajeni; } }

        public int DokumentyCount { get; set; }

        public int VeriteleCount { get; set; }
        public int DluzniciCount { get; set; }
        public int SpravciCount { get; set; }

        public string[] VybraniDluznici { get; set; }
    }
}
