using System;
using System.Globalization;
using System.IO;

using CsvHelper;

namespace HlidacStatu.ExportData
{
    public class TabDelimited : FlatFile
    {
        public TabDelimited() : base("\t", FlatFile.DefaultConfig.Quote)
        {}  
    }
}
