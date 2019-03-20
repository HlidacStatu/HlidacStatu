using HlidacStatu.Api.Dataset.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Api.Dataset.Connector
{
    public class SearchResult<TData>
        where TData : IDatasetItem
    {
        public long total { get;  set; }
        public int page { get; set; }
        public TData[] results { get; set; }
    }
}
