using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Searching
{
    public class Query
    {
        public static string Formatted(string property, DateTime exactDate)
        {
            return $"{property}:{exactDate:yyyy-MM-dd}";
        }
        public static string Formatted(string property, DateTime fromDate, DateTime toDate, bool includefromDate=true, bool includeToDate=true)
        {
            return $"{property}:{(includefromDate ? "[" : "{")}{fromDate:yyyy-MM-dd} TO {toDate:yyyy-MM-dd}{(includeToDate? "]" : "}")}";
        }

        public static string ModifyQueryAND(string origQuery, string anotherCondition)
        {
            if (string.IsNullOrEmpty(origQuery))
                return anotherCondition;
            else
            {
                var s1 = origQuery.Trim();
                var s2 = anotherCondition.Trim();
                if (!s1.StartsWith("(") && !s1.EndsWith("("))
                    s1 = $"( {s1} )";
                if (!s2.StartsWith("(") && !s2.EndsWith("("))
                    s2 = $"( {s2} )";
                return $"(  {s1}  AND  {s2}  )";
            }
        }
        public static string ModifyQueryOR(string origQuery, string anotherCondition)
        {
            if (string.IsNullOrEmpty(origQuery))
                return anotherCondition;
            else
            {
                var s1 = origQuery.Trim();
                var s2 = anotherCondition.Trim();
                if (!s1.StartsWith("(") && !s1.EndsWith("("))
                    s1 = $"( {s1} )";
                if (!s2.StartsWith("(") && !s2.EndsWith("("))
                    s2 = $"( {s2} )";
                return string.Format($"(  {s1}  OR  {s2}  )", origQuery, anotherCondition);
            }
        }
    }
}
