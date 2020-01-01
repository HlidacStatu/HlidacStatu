using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Searching
{
    [Obsolete]
    public class Rule
    {
        public Rule(string lookFor, string replaceWith, bool fullReplace = true)
        {
            this.LookFor = lookFor;
            this.ReplaceWith = replaceWith;
            this.FullReplace = fullReplace;
        }


        public string LookFor { get; set; }
        public string ReplaceWith { get; set; }

        public bool FullReplace { get; set; } = true;
        public string AddLastCondition { get; set; } = "";
    }
}
