using System;

namespace HlidacStatu.ExportData
{
    public partial class Data
    {
        public class Column
        {
            public string Name { get; set; }
            public Type ValueType { get; set; }
            public string FormatString { get; set; }
        }


    }
}
