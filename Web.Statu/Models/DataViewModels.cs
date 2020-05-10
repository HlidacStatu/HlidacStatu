using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using static HlidacStatu.Lib.RenderTools;
using HlidacStatu.Lib.Data.External.DataSets;

namespace HlidacStatu.Web.Models
{

    public class DatasetIndexStat
    {
        public HlidacStatu.Lib.Data.External.DataSets.Registration Ds { get; set; }
        public long RecordNum { get; set; }
        public long RecordNumWeek { get; set; }

        public DateTime? LastRecord { get; set; }
    }

    public class DataDetailModel
    {
        public DataSet Dataset { get; set; }
        public string Data { get; set; }
    }

    public class CreateSimpleModel
    {
        public class Column
        {
            public string Name { get; set; }

            public string NormalizedName() { return Normalize(this.Name); }
            public static string Normalize(string s)
            {
                if (string.IsNullOrEmpty(s))
                    throw new System.ArgumentException("Cannot by null or empty", "s");
                s = Devmasters.Core.TextUtil.RemoveDiacritics(s).ToLower();
                s = System.Text.RegularExpressions.Regex.Replace(s, "[^a-z0-9]", " ");
                s = System.Text.RegularExpressions.Regex.Replace(s, "\\s", " ");
                s = Devmasters.Core.TextUtil.ReplaceDuplicates(s, ' ');
                s = s.Replace(" ", "_");
                if (!Char.IsLetter(s.FirstOrDefault()))
                    s = "p" + s;
                if (s.EndsWith("_") && s.Length > 2)
                    s = s.Substring(0, s.Length - 1);
                return s;
            }

            public string NiceName { get; set; }
            public string ValType { get; set; }
            public string ShowSearchFormat { get; set; }
            public string ShowDetailFormat { get; set; }
        }
        public Guid? FileId { get; set; }
        public string Name { get; set; }
        public string Delimiter { get; set; } = ",";

        public string GetValidDelimiter() { return GetValidDelimiter(this.Delimiter); }

        static string[] validDelimiters = new string[] { ",", ";", "\t" };
        public static string GetValidDelimiter(string del)
        {
            string delimiter = del ?? ",";
            if (!validDelimiters.Contains(delimiter))
            {
                if (delimiter == "\\t")
                    delimiter = "\t";
                else
                    delimiter = ",";
            }
            return delimiter;
        }
        public string[] Headers { get; set; }


        public string KeyColumn { get; set; }

        public void FillColumnsWithHeaders()
        {
            string[] head = this.Headers ?? new string[] { };
            this.Columns = head
                .Select(s => new Column() { Name = s, NiceName = s })
                .ToArray();
        }

        public Column[] Columns = new Column[] { };

        public int NumOfRows { get; set; } = 0;
        public string DatasetId { get; set; } = null;

        public void Save(string destinationFile)
        {
            System.IO.File.WriteAllText(destinationFile, Newtonsoft.Json.JsonConvert.SerializeObject(this));
        }
        public static CreateSimpleModel Load(string sourceFile)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<CreateSimpleModel>(
                System.IO.File.ReadAllText(sourceFile)
                );
        }
    }

    public class MappingCSV
    {
            public string sourceCSV { get; set; }
            public string TargetJSON { get; set; }
            public string Transform { get; set; }
        
    }
}