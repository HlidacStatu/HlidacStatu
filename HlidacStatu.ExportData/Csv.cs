using System;
using System.Globalization;
using System.IO;

using CsvHelper;

namespace HlidacStatu.ExportData
{
    public class Csv : FlatFile
    {
        public Csv() : base(",", FlatFile.DefaultConfig.Quote)
        { }
    }
}
