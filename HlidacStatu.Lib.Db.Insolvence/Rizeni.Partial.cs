using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Db.Insolvence
{
    public partial class Rizeni
    {
        public string Id
        {
            get
            {
                return this.SpisovaZnacka;
            }
            set
            {
                this.SpisovaZnacka = value;
            }
        }
    }
}
