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
            int PageSize { get; }
            int Page { get; }
            string Order { get; set; }
            string DataSource { get; set; }
            string Query { get; set; }

            int MaxResultWindow();

            object ToRouteValues(int page);

            TimeSpan ElapsedTime { get; set; }
        }


    }
}
