using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Data.Dotace
{
    public class Rozhodnuti
    {
        [Nest.Keyword]
        public string Id { get; set; }
        [Nest.Number]
        public decimal? CastkaPozadovana { get; set; }
        [Nest.Number]
        public decimal? CastkaRozhodnuta { get; set; }
        [Nest.Number]
        public decimal? CerpanoCelkem { get; set; }
        [Nest.Boolean]
        public bool JePujcka { get; set; }
        [Nest.Keyword]
        public string IcoPoskytovatele { get; set; }
        [Nest.Text]
        public string Poskytovatel { get; set; }
        [Nest.Number]
        public int? Rok { get; set; }
        [Nest.Keyword]
        public string ZdrojFinanci { get; set; }
        [Nest.Object]
        public List<Cerpani> Cerpani { get; set; }

        public void RecalculateCerpano()
        {
            if (Cerpani.Count == 0)
            {
                CerpanoCelkem = null;
            }
            else
            {
                CerpanoCelkem = Cerpani.Sum(c => c.CastkaSpotrebovana);
            }
        }
    }
}
