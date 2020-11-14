using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scriban.Runtime;

namespace HlidacStatu.Lib.Render
{
    public partial class ScribanT
    {
        private string template { get; set; }

        private Dictionary<string, object> globalVariables { get; set; }
        private Scriban.Template xTemplate = null;

        public ScribanT(string template, Dictionary<string, object> globalVar = null)
        {
            this.template = template;
            this.globalVariables = globalVar ?? new Dictionary<string, object>();

            this.xTemplate = Scriban.Template.Parse(template);
            if (xTemplate.HasErrors)
            {
                throw new System.ApplicationException(xTemplate
                    .Messages
                    .Select(m => m.ToString())
                    .Aggregate((f, s) => f + "\n" + s)
                    );
            }
        }

        public List<string> GetTemplateErrors()
        {
            if (xTemplate.HasErrors)
            {
                return xTemplate
                    .Messages
                    .Select(m => m.ToString())
                    .ToList();
            }
            return new List<string>();
        }

        public string Render(dynamic dmodel)
        {
            try
            {

                var xmodel = new Scriban.Runtime.ScriptObject();
                xmodel.Import(new { model = dmodel }, renamer: member => member.Name);
                var xfn = new Scriban.Runtime.ScriptObject(); ;
                xfn.Import(typeof(HlidacStatu.Lib.Render.ScribanT.Functions)
                    , renamer: member => member.Name);
                var context = new Scriban.TemplateContext { MemberRenamer = member => member.Name, LoopLimit = 65000 };
                context.PushCulture(System.Globalization.CultureInfo.CurrentCulture);
                context.PushGlobal(xmodel);
                context.PushGlobal(xfn);
                var scriptObjGlobalVariables = new ScriptObject();

                foreach (var kv in this.globalVariables)
                    scriptObjGlobalVariables[kv.Key] = kv.Value;

                context.PushGlobal(scriptObjGlobalVariables);

                var res = xTemplate.Render(context);
                return res;
            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Error($"ScribanT render error\nTemplate {this.template}\n\n"
                   + Newtonsoft.Json.JsonConvert.SerializeObject(dmodel)
                    , e);
                throw;
            }

        }

    }
}
