using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public partial class Search
    {

        public interface ISearchResult
        {
            long Total { get; }
            bool IsValid { get; }
            bool HasResult { get; }

            string DataSource { get; set; }

            TimeSpan ElapsedTime { get; set; }
        }


    }
}
