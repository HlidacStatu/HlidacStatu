using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public class Darujme
    {


        public class Stats
        {
            public Projectstats projectStats { get; set; }
            public class Projectstats
            {
                public class Collectedamountestimate
                {
                    public long cents { get; set; }
                    public string currency { get; set; }
                }

                public long projectId { get; set; }
                public Collectedamountestimate collectedAmountEstimate { get; set; }
                public long donorsCount { get; set; }
            }

        }

    }
}
