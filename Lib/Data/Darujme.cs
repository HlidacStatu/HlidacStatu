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
                    public int cents { get; set; }
                    public string currency { get; set; }
                }

                public int projectId { get; set; }
                public Collectedamountestimate collectedAmountEstimate { get; set; }
                public int donorsCount { get; set; }
            }

        }

    }
}
