using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Watchdogs
{
    public class Results
    {
        public string dataType = null;
        internal Results(IEnumerable<dynamic> results, long total, string searchUrl,
            DateTime? fromDate, DateTime? toDate, bool isValid, string datatype)
        {
            this.Items = results;
            this.SearchQuery = searchUrl;
            this.FromDate = fromDate;
            this.ToDate = toDate;
            this.Total = total;
            this.IsValid = isValid;
            this.dataType = datatype.ToLower();
        }
        public IEnumerable<dynamic> Items { get; private set; }
        public string SearchQuery { get; private set; }
        public long Total { get; set; }
        public bool IsValid { get; set; }
        public DateTime? FromDate { get; private set; }
        public DateTime? ToDate { get; private set; }


    }

}
