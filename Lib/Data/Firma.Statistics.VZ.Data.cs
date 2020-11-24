using HlidacStatu.Lib.Analytics;

namespace HlidacStatu.Lib.Data
{
    public partial class Firma
    {
        public partial class Statistics
        {
            public partial class VZ
            {
                public class Data : IAddable<Data>
                {
                    public long PocetVypsanychVZ { get; set; } = 0;
                    public decimal PocetUcastiVeVZ { get; set; } = 0;
                    public long PocetVyherVeVZ { get; set; }
                    public decimal CelkovaHodnotaVypsanychVZ { get; set; } = 0;
                    public decimal CelkoveHodnotaVyhranychVZ { get; set; }

                    public Data Add(Data other)
                    {
                        throw new System.NotImplementedException();
                    }
                }
            }
        }
    }
}
