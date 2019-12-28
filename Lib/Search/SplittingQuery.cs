using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HlidacStatu.Util;

namespace HlidacStatu.Lib.Search
{
    public class SplittingQuery
    {
        [DebuggerDisplay("{ToQueryString}")]
        public class Part
        {
            public string Prefix { get; set; } = "";
            public string Value { get; set; } = "";

            public bool ExactValue { get; set; } = false;

            public string ToQueryString
            {
                get
                {
                    return ExportPartAsQuery(true);
                }
            }

            public string ExportPartAsQuery(bool encode = true)
            {
                //force not to encode
                encode = false;
                if (encode)
                    return EncodePart();
                else
                {
                    if (ExactValue)
                        return Value;
                    else
                        return (Prefix ?? "") + Value;
                }
            }


            static string[] reservedAll = new string[] { "&&", "||", "+", "-", "=", "!", "(", ")", "{", "}", "[", "]", "^", "\"", "~", "*", "?", ":", "\\", "/" };


            static char[] formulaStart = new char[] { '>', '<', '(', '{', '[' };
            static char[] formulaEnd = new char[] { ')', '}', ']' };
            static char[] ignored = new char[] { '>', '<' };
            public string EncodePart()
            {
                if (this.ExactValue)
                    return Value;
                if (string.IsNullOrWhiteSpace(Value))
                    return Value;

                //The reserved characters are:  + - = && || > < ! ( ) { } [ ] ^ " ~ * ? : \ /
                // https://www.elastic.co/guide/en/elasticsearch/reference/7.5/query-dsl-query-string-query.html
                //< and > can’t be escaped at all. The only way to prevent them from attempting to create a range query is to remove them from the query string entirely.

                if (string.IsNullOrWhiteSpace(Prefix))
                {
                    var val = Value.Trim();
                    if (reservedAll.Contains(val))
                        return val;
                    else
                        return string.Join("", 
                            Value.Select(c => ignored.Contains(c) ? "" :  (reservedAll.Contains(c.ToString()) ? @"\" + c.ToString() : c.ToString()))
                            );
                }
                else
                {//there is prefix, check []{}()- 
                    var val = Value.Trim();
                    if (formulaStart.Contains(val.First())
                        && formulaEnd.Contains(val.Last())
                        )
                        return (Prefix ?? "") + Value; //no change
                    else
                    {
                        return (Prefix ?? "") 
                            + string.Join("",
                                Value.Select(c => ignored.Contains(c) ? "" : (reservedAll.Contains(c.ToString()) ? @"\" + c.ToString() : c.ToString()))
                                );
                    }
                }

            }

        }
        public static SplittingQuery SplitQuery(string query)
        {
            return new SplittingQuery(query);
        }
        private SplittingQuery(string query)
        {
            _parts = Split(query);
        }
        public SplittingQuery()
            : this(new Part[] { })
        {
        }
        public SplittingQuery(Part[] parts)
        {
            _parts = parts ?? new Part[] { };
        }


        public string FullQuery(bool encode = true)
        {
            if (_parts.Length == 0)
            {
                return "";
            }
            else
            {
                return _parts
                    .Select(m => m.ToQueryString.Trim())
                    .Where(m => m.Length > 0)
                    .Aggregate((f, s) => f + " " + s)
                    .Trim();
            }
        }
        Part[] _parts = null;
        public Part[] Parts { get { return _parts; } }

        public void AddParts(Part[] parts)
        {
            var p = new List<Part>(_parts);
            p.AddRange(parts);
            _parts = p.ToArray();
        }
        public void InsertParts(int index, Part[] parts)
        {
            var p = new List<Part>(_parts);
            p.InsertRange(index, parts);
            _parts = p.ToArray();
        }
        public void ReplaceWith(int index, Part[] parts)
        {

            var p = new List<Part>(_parts);
            p.RemoveAt(index);
            p.InsertRange(index, parts);
            _parts = p.ToArray();
        }


        private Part[] Split(string query)
        {
            List<Part> parts = new List<Part>();
            if (string.IsNullOrWhiteSpace(query))
                return parts.ToArray();

            List<Part> tmpParts = new List<Part>();
            //prvni rozdelit podle ""
            var fixTxts = SplitQueryToParts(query, '\"');


            //spojit a rozdelit podle mezer
            for (int i = 0; i < fixTxts.Count; i++)
            {
                //fixed string
                if (fixTxts[i].Item2)
                {
                    if (i == 0)
                    {
                        tmpParts.Add(new Part()
                        {
                            ExactValue = true,
                            Value = fixTxts[i].Item1
                        }
                        );
                    }
                    else if (i > 0 && fixTxts[i - 1].Item1.EndsWith(":"))
                    {
                        tmpParts[tmpParts.Count - 1].Prefix = tmpParts[tmpParts.Count - 1].Prefix;
                        tmpParts[tmpParts.Count - 1].Value = fixTxts[i].Item1;
                    }
                    else
                    {
                        tmpParts.Add(new Part()
                        {
                            ExactValue = true,
                            Value = fixTxts[i].Item1
                        }
                        );
                    }
                }
                else
                {
                    //string[] mezery = fixTxts[i].Item1.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string tPart = fixTxts[i].Item1;
                    tPart = tPart.Replace("(", " ( ").Replace(")", " ) ")
                        .Replace(": (", ":(");//fix mezera za :
                    string[] mezery = tPart.Split(new char[] { ' ' });

                    foreach (var mt in mezery)
                    {
                        string findPrefixReg = @"(^|\s|[(]) (?<p>(\w|\.)*:) (?<q>(-|\w)* )\s*";
                        var prefix = HlidacStatu.Util.ParseTools.GetRegexGroupValue(mt, findPrefixReg, "p");
                        if (!string.IsNullOrEmpty(prefix))
                            tmpParts.Add(new Part()
                            {
                                ExactValue = false,
                                Prefix = prefix,
                                Value = mt.Replace(prefix, "")
                            }
                            );
                        else
                            tmpParts.Add(new Part()
                            {
                                ExactValue = false,
                                Value = mt
                            }
                            );
                    }
                }
            }
            //check prefix with xxx:[ ... ]  or xxx:{   }

            for (int pi = 0; pi < tmpParts.Count; pi++)
            {
                var p = tmpParts[pi];
                if (p.ExactValue)
                    parts.Add(p);
                else
                {
                    if (!string.IsNullOrEmpty(p.Prefix) && (p.Value.StartsWith("{") || p.Value.StartsWith("[")))
                    {
                        //looking until end to the next with ] } and join it together
                        for (int pj = pi + 1; pj < tmpParts.Count; pj++)
                        {

                            if (tmpParts[pi].ExactValue == false
                                && (tmpParts[pj].Value.EndsWith("}") || tmpParts[pj].Value.EndsWith("]"))
                                )
                            {
                                parts.Add(new Part()
                                {
                                    Prefix = p.Prefix,
                                    ExactValue = p.ExactValue,
                                    Value = tmpParts.Skip(pi).Take(pj - pi + 1).Select(m => m.Value).Aggregate((f, s) => f + " " + s)
                                });
                                pi = pj;
                                goto NextPart;
                            }

                        }
                        //no end found
                        parts.Add(new Part()
                        {
                            Prefix = p.Prefix,
                            ExactValue = p.ExactValue,
                            Value = tmpParts.Skip(pi).Take(tmpParts.Count - pi + 1).Select(m => m.Value).Aggregate((f, s) => f + " " + s)
                        });
                        pi = tmpParts.Count;
                    NextPart:
                        continue;
                    }
                    else
                        parts.Add(p);

                }

            }


            return parts.Where(m => m.ToQueryString.Length > 0).ToArray();
        }


        public static List<Tuple<string, bool>> SplitQueryToParts(string query, char delimiter)
        {
            //split newquery into part based on ", mark "xyz" parts
            //string , bool = true ...> part withint ""
            List<Tuple<string, bool>> textParts = new List<Tuple<string, bool>>();
            int[] found = CharacterPositionsInString(query, delimiter);
            if (found.Length > 0 && found.Length % 2 == 0)
            {
                int start = 0;
                bool withIn = false;
                foreach (var idx in found)
                {
                    int startIdx = start;
                    int endIdx = idx;
                    if (withIn)
                        endIdx++;
                    textParts.Add(new Tuple<string, bool>(query.Substring(startIdx, endIdx - startIdx), withIn));
                    start = endIdx;
                    withIn = !withIn;
                }
                if (start < query.Length)
                    textParts.Add(new Tuple<string, bool>(query.Substring(start), withIn));
            }
            else
            {
                textParts.Add(new Tuple<string, bool>(query, false));
            }
            return textParts;
        }

        public static int[] CharacterPositionsInString(string text, char lookingFor)
        {
            if (string.IsNullOrEmpty(text))
                return new int[] { };

            char[] textarray = text.ToCharArray();
            List<int> found = new List<int>();
            for (int i = 0; i < text.Length; i++)
            {
                if (textarray[i].Equals(lookingFor))
                {
                    found.Add(i);
                }
            }
            return found.ToArray();
        }


    }
}