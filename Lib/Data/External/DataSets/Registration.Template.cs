using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Data.External.DataSets
{
    public partial class Registration
    {
        public partial class Template
        {
            public string header { get; set; } = null;
            public string body { get; set; } = null;
            public string footer { get; set; } = null;
            public string title { get; set; } = null;
            public string[] properties { get; set; } = null;

            public string ToPageContent()
            {
                if (!string.IsNullOrEmpty(body))
                    return (header ?? "") + body + (footer ?? "");
                else
                    return null;
            }

            public bool IsFullTemplate()
            {
                return !string.IsNullOrEmpty(body);
            }
            public bool IsNewTemplate()
            {
                return (this.body ?? "").Contains("{{");
            }

            public string Render(DataSet ds, string sModel)
            {
                dynamic model = Newtonsoft.Json.Linq.JObject.Parse(sModel);
                return Render(ds, model);
            }

            public List<string> GetTemplateErrors()
            {
                string template = "{{func fn_DatasetItemUrl" + "\n"
                    + $"    ret ('https://www.hlidacstatu.cz/data/Detail/any/' + $0)"
                    + "\n"
                    + "end}}";

                template = template + "\n\n" + this.body;
                var xTemp = Scriban.Template.Parse(template);
                if (xTemp.HasErrors)
                {
                    return xTemp
                        .Messages
                        .Select(m => m.ToString())
                        .ToList();
                }
                return new List<string>();
            }

            public string Render(DataSet ds, dynamic dmodel)
            {
                string template = "{{func fn_DatasetItemUrl" + "\n"
                    + $"    ret ('https://www.hlidacstatu.cz/data/Detail/{ds.DatasetId}/' + $0)" 
                    + "\n"
                    + "end}}";

                template = template + "\n\n" + this.body;
                var xTemp = Scriban.Template.Parse(template);
                if (xTemp.HasErrors)
                {
                    throw new System.ApplicationException(xTemp
                        .Messages
                        .Select(m=>m.ToString())
                        .Aggregate((f,s)=>f + "\n" + s)
                        );
                }

                var xmodel = new Scriban.Runtime.ScriptObject();
                xmodel.Import(new { model = dmodel }, renamer: member => member.Name);
                var xfn = new Scriban.Runtime.ScriptObject(); ;
                xfn.Import(typeof(HlidacStatu.Lib.Data.External.DataSets.Registration.Template.Functions)
                    , renamer: member => member.Name);
                var context = new Scriban.TemplateContext { MemberRenamer = member => member.Name };
                context.PushCulture(System.Globalization.CultureInfo.CurrentCulture);
                context.PushGlobal(xmodel);
                context.PushGlobal(xfn);
                var res = xTemp.Render(context);
                return res;
            }

            public class SearchTemplateResults
            {
                public long Total { get; set; }
                public System.Collections.Generic.IEnumerable<dynamic> Result { get; set; }
                public string Q { get; set; }
                public int Page { get; set; }
            }
        }
    }
}
