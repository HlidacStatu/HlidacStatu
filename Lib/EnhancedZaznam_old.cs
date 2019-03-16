using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HlidacSmluv.Lib
{
    
    public class EnhancedZaznam : XSD.dumpZaznam
    {
        public class EnhancedPriloha : XSD.tPrilohaOdkaz
        {
            public class KeyVal
            {
                public string Key;
                public string Value;
            }
            //skip X-Parsed-By
            public KeyVal[] FileMetadata;
            public string PlainTextContent;
            public DataQualityEnum PlainTextContentQuality;
            public DateTime LastParsed { get; set; } = DateTime.MinValue;


            public string ContentType;
            public int Lenght;
            public int WordCount;

            public EnhancedPriloha()
            {
            }
            public EnhancedPriloha(XSD.tPrilohaOdkaz tpriloha)
            {
                this.hash = tpriloha.hash;
                this.nazevSouboru = tpriloha.nazevSouboru;
                this.odkaz = tpriloha.odkaz;
            }


        }


        public EnhancedPriloha[] EnhPrilohy;

        public DateTime LastSaved { get; set; } = DateTime.MinValue;
        public decimal CalculatedPriceWithVATinCZK { get; set; }
        public DataQualityEnum CalcutatedPriceQuality { get; set; }


        private Issues.Issue[] issues = new Lib.Issues.Issue[] { };
        public Issues.Issue[] Issues
        {
            get
            {
                return issues;
            }
            set //TODO IObservable
            {

                if (this.issues.Any(m => m.Permanent))
                {
                    //nech jen permanent
                    var newIss = this.issues.Where(m => m.Permanent).ToList();
                    //unique Ids, at se neopakuji
                    var existsIds = newIss.Select(m => m.IssueTypeId).Distinct();

                    //pridej vse krome existujicich Ids
                    newIss.AddRange(value.Where(m => !(existsIds.Contains(m.IssueTypeId))));
                    this.issues = newIss.ToArray();
                }
                else
                    this.issues = value;
                this.ConfidenceValue = GetConfidenceValue();
            }
        }

        public void AddSpecificIssue(Issues.Issue i)
        {
            if (!this.Issues.Any(m => m.IssueTypeId == i.IssueTypeId))
            {
                var oldIssues = this.Issues.ToList();
                oldIssues.Add(i);
                this.Issues = oldIssues.ToArray();
            }

        }

        object enhLock = new object();
        public void AddEnhancement(Enhancers.Enhancement enh)
        {
            lock (enhLock)
            {
                if (!this.Enhancements.Contains(enh))
                {
                    //add new to the array http://stackoverflow.com/a/31542691/1906880
                    Enhancers.Enhancement[] result = new Enhancers.Enhancement[this.Enhancements.Length + 1];
                    this.Enhancements.CopyTo(result, 0);
                    result[this.Enhancements.Length] = enh;
                }
                else
                {
                    var existingIdx = Array.FindIndex( this.Enhancements, e => e == enh);
                    if (existingIdx > -1)
                    {
                        this.Enhancements[existingIdx] = enh;
                    }
                    
                }
            }


        }
        public Enhancers.Enhancement[] Enhancements { get; set; } = new Enhancers.Enhancement[] { };
        public decimal ConfidenceValue { get; set; }

        public void PrepareBeforeSave()
        {
            this.LastSaved = DateTime.Now;

            this.ConfidenceValue = GetConfidenceValue();
        }
        private decimal GetConfidenceValue()
        {
            if (this.Issues != null)
                return this.Issues.Sum(m => (int)m.Importance);
            else
            {
                return 0;
            }

        }


        public Issues.ImportanceLevel GetConfidenceLevel()
        {
            if (ConfidenceValue <= 0 || this.Issues == null)
            {
                return Lib.Issues.ImportanceLevel.Ok;
            }
            if (this.Issues.Count() == 0)
            {
                return Lib.Issues.ImportanceLevel.Ok;
            }
            //pokud je tam min 1x fatal, je cele fatal
            if (this.Issues.Any(m => m.Importance == Lib.Issues.ImportanceLevel.Fatal))
            {
                return Lib.Issues.ImportanceLevel.Fatal;
            }
            if (ConfidenceValue > ((int)Lib.Issues.ImportanceLevel.Major) * 3)
            {
                return Lib.Issues.ImportanceLevel.Fatal;
            }


            //pokud je tam min 1x fatal, je cele fatal
            if (this.Issues.Any(m => m.Importance == Lib.Issues.ImportanceLevel.Major))
            {
                return Lib.Issues.ImportanceLevel.Major;
            }
            if (ConfidenceValue > ((int)Lib.Issues.ImportanceLevel.Major) * 2 && ConfidenceValue <= ((int)Lib.Issues.ImportanceLevel.Major) * 3)
            {
                return Lib.Issues.ImportanceLevel.Major;
            }

            if (ConfidenceValue > ((int)Lib.Issues.ImportanceLevel.Minor) * 2 && ConfidenceValue <= ((int)Lib.Issues.ImportanceLevel.Major) * 2)
            {
                return Lib.Issues.ImportanceLevel.Minor;
            }

            if (ConfidenceValue > 0 && ConfidenceValue <= ((int)Lib.Issues.ImportanceLevel.Minor) * 2)
            {
                return Lib.Issues.ImportanceLevel.Formal;
            }

            return Lib.Issues.ImportanceLevel.Ok;
        }

        public EnhancedZaznam()
        { }

        public EnhancedZaznam(XSD.dumpZaznam item)
        {
            this.casZverejneni = item.casZverejneni;
            this.identifikator = item.identifikator;
            this.odkaz = item.odkaz;
            this.platnyZaznam = item.platnyZaznam;
            this.prilohy = null;
            if (item.prilohy != null)
                this.EnhPrilohy = item.prilohy.Select(m => new EnhancedPriloha(m)).ToArray();
            this.smlouva = item.smlouva;
        }


        public static string NicePrice(int? number, string mena = "Kč", bool html = false, bool shortFormat = false)
        {
            if (number.HasValue)
                return NicePrice((decimal)number.Value, mena, html, shortFormat);
            else
                return string.Empty;
        }
        public static string NicePrice(double? number, string mena = "Kč", bool html = false, bool shortFormat = false)
        {
            if (number.HasValue)
                return NicePrice((decimal)number.Value, mena, html, shortFormat);
            else
                return string.Empty;
        }
        public static string NicePrice(decimal number, string mena = "Kč", bool html = false, bool shortFormat = false)
        {

            string s = string.Empty;
            if (number == 0)
            {
                s = "0 " + mena;
            }
            else if (shortFormat)
            {
                return ShortNicePrice(number, mena, html);
            }
            else
            {
                s = string.Format("{0:### ### ### ### ###} {1}", number, mena).Trim();
            }
            if (html)
            {
                return s.Trim().Replace(" ", "&nbsp;");
            }
            return s;
        }

        public static string ShortNicePrice(decimal number, string mena = "Kč", bool html = false)
        {
            decimal OneMil = 1000000;
            decimal OneMld = OneMil * 1000;
            decimal OneBil = OneMld * 1000;

            decimal n = number;

            string suffix;
            if (n > OneBil)
            {
                n /= OneBil;
                suffix = "bil.";
            }
            else if (n > OneMld)
            {
                n /= OneMld;
                suffix = "mld.";
            }
            else if (n > OneMil)
            {
                n /= OneMil;
                suffix = "mil.";
            }
            else
            {
                suffix = "";
            }

            string ret = string.Empty;
            if (!string.IsNullOrEmpty(suffix))
            {
                ret = String.Format("{0:### ### ### ### ###.##} {1} {2}", n, suffix, mena);
            }
            else
            {
                ret = String.Format("{0:### ### ### ### ###} {1}", n, mena);
            }

            if (html)
            {
                return String.Format("<span title=\"{2:### ### ### ### ###} {1}\">{0}</span>", ret.Trim().Replace(" ", "&nbsp;"), mena, number);

            }
            return ret;


        }



        public string NicePrice(bool html = false)
        {
            string res = "Neuvedena";
            if (this.CalculatedPriceWithVATinCZK == 0)
                return res;
            else
                return NicePrice(this.CalculatedPriceWithVATinCZK, html: html);
        }


    }
}
