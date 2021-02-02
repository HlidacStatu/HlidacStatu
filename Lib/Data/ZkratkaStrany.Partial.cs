using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Data
{
    public partial class ZkratkaStrany
    {
        public static Dictionary<string,string> PopulateCache()
        {
            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {
                return db.ZkratkaStrany.ToDictionary(z => z.ICO, e => e.KratkyNazev);
            }
        }
    }
}
