using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HlidacStatu.Lib;
using HlidacStatu.Lib.Issues;

namespace HlidacStatu.Plugin.IssueAnalyzers
{
    public class Text : IIssueAnalyzer
    {
        public string Description
        {
            get
            {
                return "Texty";
            }

        }

        public string Name
        {
            get
            {
                return "Kontrola textu";
            }

        }

        public IEnumerable<Issue> FindIssues(Lib.Data.Smlouva item)
        {
            List<Issue> issues = new List<Issue>();

            if (item.Prilohy != null && item.Prilohy.Count() > 0)
            {
                List<string> files = new List<string>();
                foreach (var p in item.Prilohy)
                {
                    string fn = CheckAttachment(p,item);
                    if (!string.IsNullOrEmpty(fn))
                        files.Add(fn);
                }
                if (files.Count > 1)
                    issues.Add(
                        new Issue(this, (int)IssueType.IssueTypes.NecitelnostSmlouvy,"Nečitelnost smlouvy",
                        string.Format("Text příloh {0} není strojově čitelný, ze zákona být musí",
                        files.Aggregate((f, s) => f + ", " + s)
                        ))
                        );
                else if (files.Count == 1)
                {
                    issues.Add(
                        new Issue(this, (int)IssueType.IssueTypes.NecitelnostSmlouvy, "Nečitelnost smlouvy",
                            string.Format("Text přílohy {0} není strojově čitelný, ze zákona být musí", files.First()))
                        );

                }


            }
            return issues;
        }


        DateTime historyDate = new DateTime(1990, 1, 1);
        DateTime datumUzavreni = new DateTime(2016, 1, 7);
        private string CheckAttachment(Lib.Data.Smlouva.Priloha p, Lib.Data.Smlouva item)
        {
            List<Issue> issues = new List<Issue>();

            if (
                (!p.EnoughExtractedText || p.PlainTextContentQuality == DataQualityEnum.Estimated)
                && p.LastUpdate > historyDate
                && item.datumUzavreni > datumUzavreni
                && p.PlainTextContentQuality != DataQualityEnum.Unknown
                )

            {
                return p.nazevSouboru;
            }


            return string.Empty;

        }

    }
}
