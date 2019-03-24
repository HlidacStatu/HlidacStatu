using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HlidacStatu.Lib.Data.External.DataSets
{
    public static class Util
    {
        public static JObject CleanHsProcessTypeValuesFromObject(string sobj)
        {
            JObject jobj = JObject.Parse(sobj);
            var jpaths = jobj
                .SelectTokens("$..HsProcessType")
                .ToArray();
            var jpathObjs = jpaths.Select(j => j.Parent.Parent).ToArray();
            foreach (var jo in jpathObjs)
            {
                if (jo["HsProcessType"].Value<string>() == "person")
                {
                    var osobaIdAttrName = jo.Children()
                            .Select(c => c as JProperty)
                            .Where(c => c != null)
                            .Where(c => c.Name.ToLower() == "osobaid")
                            .FirstOrDefault()?.Name ?? "OsobaId";

                    jo[osobaIdAttrName] = null;
                }
                if (jo["HsProcessType"].Value<string>() == "document")
                {
                    jo["DocumentPlainText"] = null;
                }
            }

            return jobj;
        }

        public static List<string> CompareObjects(JObject source, JObject target)
        {
            List<string> diffs = new List<string>();
            foreach (KeyValuePair<string, JToken> sourcePair in source)
            {
                if (sourcePair.Value.Type == JTokenType.Object)
                {
                    if (target.GetValue(sourcePair.Key) == null)
                    {
                        diffs.Add("Key " + sourcePair.Key
                                            + " not found" + Environment.NewLine);
                    }
                    else if (target.GetValue(sourcePair.Key).Type != JTokenType.Object)
                    {
                        diffs.Add("Key " + sourcePair.Key
                                            + " is not an object in target" + Environment.NewLine);
                    }
                    else
                    {
                        diffs.AddRange(CompareObjects(sourcePair.Value.ToObject<JObject>(),
                            target.GetValue(sourcePair.Key).ToObject<JObject>()));
                    }
                }
                else if (sourcePair.Value.Type == JTokenType.Array)
                {
                    if (target.GetValue(sourcePair.Key) == null)
                    {
                        diffs.Add("Key " + sourcePair.Key
                                            + " not found" + Environment.NewLine);
                    }
                    else
                    {
                        diffs.AddRange(CompareArrays(sourcePair.Value.ToObject<JArray>(),
                            target.GetValue(sourcePair.Key).ToObject<JArray>(), sourcePair.Key));
                    }
                }
                else
                {
                    JToken expected = sourcePair.Value;
                    var actual = target.SelectToken(sourcePair.Key);
                    if (actual == null)
                    {
                        diffs.Add("Key " + sourcePair.Key
                                            + " not found" + Environment.NewLine);
                    }
                    else
                    {
                        if (expected.Type == JTokenType.String && actual.Type == JTokenType.String)
                        {
                            //compare strings, ignore,  ASCII 160  == ASCII 32
                            string sactual = actual.ToString().Replace((char)160, (char)32);
                            string sexpected = expected.ToString().Replace((char)160, (char)32);
                            if (sactual != sexpected)
                            {
                                diffs.Add("Key " + sourcePair.Key + ": "
                                                    + sourcePair.Value + " !=  "
                                                    + target.Property(sourcePair.Key).Value);
                            }
                        }
                        else
                        if (!JToken.DeepEquals(expected, actual))
                        {
                            diffs.Add("Key " + sourcePair.Key + ": "
                                                + sourcePair.Value + " !=  "
                                                + target.Property(sourcePair.Key).Value
                                                + Environment.NewLine);
                        }
                    }
                }
            }
            return diffs;
        }

        /// <summary>
        /// Deep compare two NewtonSoft JArrays. If they don't match, returns text diffs
        /// </summary>
        /// <param name="source">The expected results</param>
        /// <param name="target">The actual results</param>
        /// <param name="arrayName">The name of the array to use in the text diff</param>
        /// <returns>Text string</returns>

        public static List<string> CompareArrays(JArray source, JArray target, string arrayName = "")
        {
            List<string> returnString = new List<string>();
            for (var index = 0; index < source.Count; index++)
            {

                var expected = source[index];
                if (expected.Type == JTokenType.Object)
                {
                    var actual = (index >= target.Count) ? new JObject() : target[index];
                    returnString.AddRange(CompareObjects(expected.ToObject<JObject>(),
                        actual.ToObject<JObject>()));
                }
                else
                {

                    var actual = (index >= target.Count) ? "" : target[index];
                    if (!JToken.DeepEquals(expected, actual))
                    {
                        if (String.IsNullOrEmpty(arrayName))
                        {
                            returnString.Add("Index " + index + ": " + expected
                                                + " != " + actual + Environment.NewLine);
                        }
                        else
                        {
                            returnString.Add("Key " + arrayName
                                                + "[" + index + "]: " + expected
                                                + " != " + actual + Environment.NewLine);
                        }
                    }
                }
            }
            return returnString;
        }

    }
}
