using System;
using System.Collections.Generic;

namespace HlidacStatu.Api.Dataset.Connector
{
    public partial class ClassicTemplate
    {
        public class column
        {
            public string header { get; set; }
            public string content { get; set; }
            public string style { get; set; }
        }

        public class ClassicSearchResultTemplate : Template
        {
            private List<column> columns { get; set; } = new List<column>();


            public ClassicSearchResultTemplate AddColumn(string columnHeader, string columnTemplateValue, string style = null)
            {
                columns.Add(new column() { header = columnHeader, content = columnTemplateValue, style = style });
                this.Header = GenerateHeader();
                this.Body = GenerateBody();
                this.Footer = GenerateFooter();
                return this;
            }

            private string GenerateHeader()
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(512);
                sb.Append(@"<table class=""table table-hover"">
                        <thead>
                            <tr>");
                foreach (var c in columns)
                {
                    sb.AppendLine($"<th>{c.header}</th>");
                }
                sb.Append(@"</tr>
                        </thead>
                        <tbody>");
                return sb.ToString();
            }

            private string GenerateBody()
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(512);
                sb.Append(@"<tr>");
                foreach (var c in columns)
                {
                    sb.Append($"<td ");
                    if (!string.IsNullOrEmpty(c.style))
                        sb.Append($"style=\"{c.style}\" ");
                    sb.AppendLine($">{c.content}</td>");
                }
                sb.Append(@"</tr>");
                return sb.ToString();
            }

            private string GenerateFooter()
            {
                return "</tbody></table>";
            }

        }
    }
}