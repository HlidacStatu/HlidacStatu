using HlidacStatu.Lib.Search;
using Nest;

namespace HlidacStatu.Lib.Data.Dotace
{
    public partial class DotaceService
    {
        static string regex = "[^/]*\r\n/(?<regex>[^/]*)/\r\n[^/]*\r\n";
        static System.Text.RegularExpressions.RegexOptions options = ((System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace | System.Text.RegularExpressions.RegexOptions.Multiline)
                    | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        static System.Text.RegularExpressions.Regex regFindRegex = new System.Text.RegularExpressions.Regex(regex, options);


        // podle čeho můžeme vyhledat
        static string[] queryShorcuts = new string[] {
                "ico:",
                "jmeno:",
                "ucel:",
                "projekt:",
                "osobaid:","holding:",
                "id:"
            };
        static string[] queryOperators = new string[] { "AND", "OR" };


        public static QueryContainer GetSimpleQuery(string query)
        {
            return GetSimpleQuery(new DotaceSearchResult() { Q = query, Page = 1 });
        }
        public static QueryContainer GetSimpleQuery(DotaceSearchResult searchdata)
        {
            var query = searchdata.Q;
            
            //fix field prefixes
            //ds: -> 
            Lib.Search.Rule[] rules = new Lib.Search.Rule[] {
                   new Lib.Search.Rule(@"osobaid:(?<q>((\w{1,} [-]{1} \w{1,})([-]{1} \d{1,3})?)) ","ico"),
                   new Lib.Search.Rule(@"holding:(?<q>(\d{1,8})) (\s|$){1,}","ico"),
                   new Lib.Search.Rule(@"ico:","prijemceIco:"),
                   new Lib.Search.Rule("jmeno:","prijemceJmenoPrijemce:"),
                   new Lib.Search.Rule("ucel:","(obdobiDotaceTitulNazev:${q} OR obdobiUcelZnakNazev:${q}) "),
                   new Lib.Search.Rule("projekt:","dotaceProjektNazev:"),
                   new Lib.Search.Rule("id:","idDotace:"),
            };

            string modifiedQ = query; // Search.Tools.FixInvalidQuery(query, queryShorcuts, queryOperators) ?? "";
                                      //check invalid query ( tag: missing value)

            if (searchdata.LimitedView)
                modifiedQ = Lib.Search.Tools.ModifyQueryAND(modifiedQ, "onRadar:true");

            var qc = Lib.Search.Tools.GetSimpleQuery<Dotace>(modifiedQ, rules); ;

            return qc;

        }

    }
}
