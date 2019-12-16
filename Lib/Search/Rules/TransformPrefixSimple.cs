using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Search.Rules
{
    public class TransformPrefix
        : TransformPrefixWithValue
    {

        public TransformPrefix(string prefix, string newPrefix, string valueConstrain, bool stopFurtherProcessing = false, string addLastCondition = "")
            : base(prefix, $"{newPrefix}${{q}}",valueConstrain,stopFurtherProcessing, addLastCondition)
        {
            if (newPrefix.Contains("${q}"))
                throw new ArgumentException("Use class TransformPrefixWithValue");
        }


    }
}
