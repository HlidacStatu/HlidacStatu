using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace HlidacStatu.Lib.Issues
{

    [Serializable]
    public class Issue
    {
        public Issue(
            IIssueAnalyzer analyzer, int issueTypeId, string title, string description, ImportanceLevel? importance = null, dynamic affectedParams = null, bool publicVisible = true, bool permanent = false)
        {
            this.IssueTypeId = issueTypeId;

            this.AnalyzerType = analyzer != null ? analyzer.GetType().FullName : "Manual";
            this.Title = title;
            this.TextDescription = description;
            this.Importance = (importance.HasValue ? importance.Value :  IssueType.IssueImportance(issueTypeId));
            this.Public = publicVisible;
            this.Permanent = permanent;
            if (affectedParams != null)
            {
                this.AffectedParams = ((object)affectedParams).GetType()
                        .GetProperties()
                        .ToDictionary(p => p.Name, p => p.GetValue(affectedParams, null))
                        .Select(m => new KeyValue() { Key= m.Key, Value= m.Value.ToString() })
                        .ToArray();
            }
        }
        public Issue() { }

        public int IssueTypeId { get; set; } = 0;

        [Nest.Date]
        public DateTime Created { get; set; } = DateTime.Now;

        [Nest.Keyword]
        public string Title { get; set; }

        [Nest.Text]
        public string TextDescription { get; set; }

        public bool Public { get; set; } = true;
        public bool Permanent { get; set; } = false;

        public ImportanceLevel Importance { get; set; }


        public class KeyValue
        {
            [Nest.Keyword]
            public string Key { get; set; }
            [Nest.Text]
            public string Value { get; set; }
        }
        public KeyValue[] AffectedParams { get; set; }

        [Nest.Keyword]
        public string AnalyzerType { get; set; }
    }
}
