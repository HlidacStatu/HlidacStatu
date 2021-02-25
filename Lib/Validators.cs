using Devmasters;
using HlidacStatu.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EnumsNET;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Collections;

namespace HlidacStatu.Lib
{
    public class Validators
    {

        static System.Text.RegularExpressions.RegexOptions regexOptions = Util.Consts.DefaultRegexQueryOption;

        //    static System.Text.RegularExpressions.RegexOptions regexOptions =
        //System.Text.RegularExpressions.RegexOptions.CultureInvariant |
        //System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace |
        //System.Text.RegularExpressions.RegexOptions.IgnoreCase |
        //System.Text.RegularExpressions.RegexOptions.Multiline;

        public static bool IsOsoba(string text)
        {
            return JmenoInText(text) != null;
        }
        public static Lib.Data.Osoba JmenoInText(string text, bool preferAccurateResult = false)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            string normalizedText = Devmasters.TextUtil
                                        .ReplaceDuplicates(Regex.Replace(text, @"[,;\[\]:]", " "), ' ');
            // Zakomentováno 12.12.2019 - při nahrávání dotací byl problém se jménem Ĺudovit
            //fix errors Ĺ => Í,ĺ => í 
            //normalizedText = normalizedText.Replace((char)314, (char)237).Replace((char)313, (char)205);

            //remove tituly
            var titulyPo = Osoba.TitulyPo.Select(m => Devmasters.TextUtil.RemoveDiacritics(m).ToLower()).ToArray();
            var titulyPred = Osoba.TitulyPred.Select(m => Devmasters.TextUtil.RemoveDiacritics(m).ToLower()).ToArray();

            List<string> lWords = new List<string>();

            var titulPred = string.Empty;
            var titulPo = string.Empty;
            foreach (var w in normalizedText
                                .Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                )
            {
                var newW = w;
                string cw = TextUtil.RemoveDiacritics(w).ToLower();
                for (int i = 0; i < titulyPo.Length; i++)
                {
                    var t = titulyPo[i];
                    if (cw == t)
                    {
                        newW = "";
                        titulPo = titulPo + " " + Osoba.TitulyPo[i];
                        break;
                    }
                    else if (t.EndsWith(".") && cw == t.Substring(0, t.Length - 1))
                    {
                        newW = "";
                        titulPo = titulPo + " " + Osoba.TitulyPo[i];
                        break;
                    }
                }
                for (int i = 0; i < titulyPred.Length; i++)
                {
                    var t = titulyPred[i];

                    if (cw == t)
                    {
                        newW = "";
                        titulPred = titulPred + " " + Osoba.TitulyPred[i];
                        break;
                    }
                    else if (t.EndsWith(".") && cw == t.Substring(0, t.Length - 1))
                    {
                        newW = "";
                        titulPred = titulPred + " " + Osoba.TitulyPred[i];
                        break;
                    }
                }
                titulPo = titulPo.Trim();
                titulPred = titulPred.Trim();
                lWords.Add(newW);
            }

            //reset normalizedText after titulPred, titulPo
            normalizedText = lWords.Aggregate((f, s) => f + " " + s).Trim();
            normalizedText = Devmasters.TextUtil
                            .ReplaceDuplicates(normalizedText.Replace(".", ". "), ' ');
            normalizedText = Devmasters.TextUtil
                            .ReplaceDuplicates(normalizedText, " ");
            string[] wordsFromNormalizedText = normalizedText.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] words = wordsFromNormalizedText.Select(m=>TextUtil.RemoveDiacritics(m).ToLower()).ToArray();


            int maxWords = 2;
            int minWords = 2;
            for (int firstWord = 0; firstWord < words.Length - 1; firstWord++)
            {
                for (int takeWords = Math.Min(maxWords, words.Count() - firstWord); takeWords >= minWords; takeWords--)
                {
                    var currWords = words.Skip(firstWord).Take(takeWords).ToArray();
                    var origWords = wordsFromNormalizedText.Skip(firstWord).Take(takeWords).ToArray();


                    CompareResult w0 = CompareName(currWords[0]);
                    CompareResult w1 = CompareName(currWords[1]);
                    if (w0==w1 && w0!= CompareResult.NotFound && currWords[0] == currWords[1]) //stejne jmeno a prijmeni MUDr. Tomáš Tomáš, PH.D.
                        return new Osoba() { Jmeno = origWords[0], Prijmeni = origWords[1], TitulPred = titulPred, TitulPo = titulPo };

                    if (
                        w0.HasAnyFlags(CompareResult.FoundInTopJmeno | CompareResult.FoundInJmeno)
                        && !w0.HasAnyFlags(CompareResult.FoundInTopPrijmeni)
                        && w0 != CompareResult.NotFound
                        &&
                        w1.HasAnyFlags(CompareResult.FoundInTopPrijmeni | CompareResult.FoundInPrijmeni)
                        && !w1.HasAnyFlags(CompareResult.FoundInTopJmeno)
                        && w1 != CompareResult.NotFound
                    )
                        return new Osoba() { Jmeno = origWords[0], Prijmeni = origWords[1], TitulPred = titulPred, TitulPo = titulPo };
                    else if (
                        w1.HasAnyFlags(CompareResult.FoundInTopJmeno | CompareResult.FoundInJmeno)
                        && !w1.HasAnyFlags(CompareResult.FoundInTopPrijmeni)
                        && w1 != CompareResult.NotFound
                        &&
                        w0.HasAnyFlags(CompareResult.FoundInTopPrijmeni | CompareResult.FoundInPrijmeni)
                        && !w0.HasAnyFlags(CompareResult.FoundInTopJmeno)
                        && w0 != CompareResult.NotFound
                    )
                        return new Osoba() {  Jmeno = origWords[1], Prijmeni = origWords[0], TitulPred = titulPred, TitulPo = titulPo };

                    //situace ala
                    //w0 FoundInTopPrijmeni | FoundInTopJmeno    
                    //w1  FoundInPrijmeni 
                    if (
                        w0.HasAnyFlags(CompareResult.FoundInTopJmeno | CompareResult.FoundInTopPrijmeni)
                        && !w1.HasAnyFlags(CompareResult.FoundInTopJmeno | CompareResult.FoundInJmeno)
                        && w1 != CompareResult.NotFound
                        )
                        return new Osoba() { Jmeno = origWords[0], Prijmeni = origWords[1], TitulPred = titulPred, TitulPo = titulPo };
                    if (
                        w1.HasAnyFlags(CompareResult.FoundInTopJmeno | CompareResult.FoundInTopPrijmeni)
                        && !w0.HasAnyFlags(CompareResult.FoundInTopJmeno | CompareResult.FoundInJmeno)
                        && w0 != CompareResult.NotFound
                        )
                        return new Osoba() { Jmeno = origWords[1], Prijmeni = origWords[0], TitulPred = titulPred, TitulPo = titulPo };



                    if (preferAccurateResult)
                        return null;

                    Osoba o = new Data.Osoba();
                    o.TitulPred = titulPred; o.TitulPo = titulPo;

                    if (w0.HasFlag(CompareResult.FoundInTopJmeno))
                        o.Jmeno = origWords[0];
                    if (string.IsNullOrEmpty(o.Jmeno)
                        && origWords.Length > 1
                        && w1.HasFlag(CompareResult.FoundInTopJmeno)
                        )
                        o.Jmeno = origWords[1];
                    if (w1.HasFlag(CompareResult.FoundInTopPrijmeni)
                        && origWords.Length > 1
                        && o.Jmeno != origWords[1]
                        )
                        o.Prijmeni = origWords[1];
                    if (string.IsNullOrEmpty(o.Prijmeni)
                        && w0.HasFlag(CompareResult.FoundInTopPrijmeni)
                        && o.Jmeno != origWords[0]
                        )
                        o.Prijmeni = origWords[0];

                    if (string.IsNullOrEmpty(o.Jmeno)
                        && w0.HasFlag(CompareResult.FoundInJmeno)
                        && o.Prijmeni != origWords[0]
                        )
                        o.Jmeno = origWords[0];
                    if (string.IsNullOrEmpty(o.Jmeno)
                        && w1.HasFlag(CompareResult.FoundInJmeno)
                        && origWords.Length>1
                        && o.Prijmeni != origWords[1]
                        )
                        o.Jmeno = origWords[1];

                    if (string.IsNullOrEmpty(o.Prijmeni)
                        && w1.HasFlag(CompareResult.FoundInPrijmeni)
                        && origWords.Length > 1
                        && o.Jmeno != origWords[1]
                        )
                        o.Prijmeni = origWords[1];

                    if (string.IsNullOrEmpty(o.Prijmeni)
                        && w0.HasFlag(CompareResult.FoundInPrijmeni)
                        && o.Jmeno != origWords[0]
                        )
                        o.Prijmeni = origWords[0];
                    if (!string.IsNullOrEmpty(o.Jmeno) && !string.IsNullOrEmpty(o.Prijmeni))
                        return o;


                    //if (w1.HasFlag(CompareResult.FoundInTopJmeno))

                }
            }
            return null;
        }


        [Flags]
        private enum CompareResult
        {
            NotFound = 0,
            FoundInPrijmeni = 1,
            FoundInTopPrijmeni = 2,

            FoundInJmeno = 4,
            FoundInTopJmeno = 8,
        }
        private static CompareResult CompareName(string word)
        {
            var ret = CompareResult.NotFound;
            if (StaticData.TopJmena.Contains(word))
                ret = ret | CompareResult.FoundInTopJmeno;
            if (StaticData.TopPrijmeni.Contains(word))
                ret = ret | CompareResult.FoundInTopPrijmeni;
            if (StaticData.Jmena.Contains(word) && !(ret.HasFlag(CompareResult.FoundInTopJmeno)))
                ret = ret | CompareResult.FoundInJmeno;
            if (StaticData.Prijmeni.Contains(word) && !(ret.HasFlag(CompareResult.FoundInTopPrijmeni)))
                ret = ret | CompareResult.FoundInPrijmeni;

            return ret;

        }

        static System.Text.RegularExpressions.RegexOptions defaultRegexOptions = ((System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace | System.Text.RegularExpressions.RegexOptions.Multiline)
            | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        public static Lib.Data.Firma FirmaInText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            string value = Devmasters.TextUtil.RemoveDiacritics(TextUtil.NormalizeToBlockText(text).ToLower());

            foreach (var k in Firma.Koncovky.Select(m => m.ToLower()).OrderByDescending(m => m.Length))
            {
                if (value.Contains(k))
                {
                    value = value.Replace(k, k.Replace(' ', (char)160)); //nahrad mezery char(160) - non breaking space, aby to tvorilo 1 slovo
                }
                else if (k.EndsWith(".") && value.EndsWith(k.Substring(0, k.Length - 1)))
                {
                    value = value.Replace(k.Substring(0, k.Length - 1), k.Replace(' ', (char)160)); //nahrad mezery char(160) - non breaking space, aby to tvorilo 1 slovo
                }
            }
            //find company name
            string[] words = value.Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            //get back space instead of #160
            words = words.Select(m => m.Replace((char)160, ' ')).ToArray();

            for (int firstWord = 0; firstWord < words.Length; firstWord++)
            {
                for (int skipWord = 0; skipWord < words.Length - firstWord; skipWord++)
                {
                    string[] cutWords = words.Skip(firstWord) //preskoc slovo na zacatku
                        .Reverse().Skip(skipWord).Reverse() // a ubirej od konce
                        .ToArray();
                    string wordCombination = cutWords.Aggregate((f, s) => f + " " + s);
                    string koncovka;
                    string firmaBezKoncovky = Lib.Data.Firma.JmenoBezKoncovkyFull(wordCombination, out koncovka);
                    string simpleName = Devmasters.TextUtil.RemoveDiacritics(firmaBezKoncovky).ToLower().Trim();
                    //+ "|" + koncovka;


                    if (firmaBezKoncovky.Length > 3
                        && StaticData.FirmyNazvyOnlyAscii.Get().ContainsKey(simpleName)
                        )
                    {
                        //nasel jsem ico?
                        foreach (var ico in StaticData.FirmyNazvyOnlyAscii.Get()[simpleName])
                        {

                            Firma f = Firmy.Get(ico); //TODO StaticData.FirmyNazvyAscii.Get()[simpleName]);
                            if (f.Valid)
                            {
                                var firmaFromText = TextUtil.ReplaceDuplicates(System.Text.RegularExpressions.Regex.Replace(wordCombination, @"[,;_""']", " ", defaultRegexOptions), ' ');
                                var firmaFromDB = TextUtil.ReplaceDuplicates(System.Text.RegularExpressions.Regex.Replace(f.Jmeno, @"[,;_""']", " ", defaultRegexOptions), ' ');
                                var rozdil = LevenshteinDistanceCompute(
                                        TextUtil.RemoveDiacritics(firmaFromDB).ToLower(),
                                        firmaFromText.ToLower()
                                        );
                                var fKoncovka = f.KoncovkaFirmy();
                                var nextWord = "";
                                if (firstWord + cutWords.Length < words.Length - 1)
                                    nextWord = words[firstWord + cutWords.Length];

                                if (string.IsNullOrEmpty(fKoncovka))
                                    return f;
                                if (!string.IsNullOrEmpty(fKoncovka) && LevenshteinDistanceCompute(cutWords.Last(), fKoncovka) < 2)
                                    return f;
                                if (!string.IsNullOrEmpty(fKoncovka) && LevenshteinDistanceCompute(nextWord, fKoncovka) < 2)
                                    return f;
                            }
                        }
                        //looking for next
                        //return null;
                    }
                }
            }

            return null;
        }


        public static string[] IcosInText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new string[] { };

            var numbers =  Devmasters.RegexUtil.GetRegexGroupValues(text, @"(\s|\D|^)(?<ico>\d{8})(\s|\D|$)", "ico");
            List<string> icos = new List<string>();
            foreach (var num in numbers)
            {
                if (Util.DataValidators.CheckCZICO(num))
                    icos.Add(num);
            }
            return icos.ToArray();
        }


        public static DateTime? DateInText(string value)
        {
            var ret = DatesInText(value);
            if (ret == null)
                return null;
            else
                return ret.First().Value;
        }
        public static Dictionary<int, DateTime> DatesInText(string value)
        {
            Dictionary<int, DateTime> ret = new Dictionary<int, DateTime>();
            if (string.IsNullOrEmpty(value))
                return null;
            var ms = Lib.Validators.dateInTextRegex.Matches(value);

            if (ms.Count > 0)
            {
                foreach (Match m in ms)
                {
                    try
                    {
                        int day = Convert.ToInt32(m.Groups["d"].Value);
                        int month = Convert.ToInt32(m.Groups["m"].Value);
                        int year = Convert.ToInt32(m.Groups["y"].Value);
                        if (m.Groups["y"].Value.Length == 2)
                            year = 1900 + year;

                        ret.Add(m.Index, new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Local));
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                return ret;
            }
            else
                return null;
        }

        static Regex dateInTextRegex = new Regex(
           @"(?<date>
				(
					(?<d>(\d{1,2}))
					\s?[.]
					(?<m>(1|2|3|4|5|6|7|8|9|01|02|03|04|05|06|07|08|09|10|11|12))
					\s?[.]
					(?<y>(\d{2}(\s|$)|(20\d{2}|19\d{2})(\s|$) ) )
					)
				)", defaultRegexOptions);

        /// <summary>
        /// difference between two sequences. the Levenshtein distance between two words is the minimum number of single-character edits (i.e. insertions, deletions, or substitutions) required to change one word into the other.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int LevenshteinDistanceCompute(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (string.IsNullOrEmpty(t))
                    return 0;
                return t.Length;
            }

            if (string.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // initialize the top and right of the table to 0, 1, 2, ...
            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 1; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    int min1 = d[i - 1, j] + 1;
                    int min2 = d[i, j - 1] + 1;
                    int min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[n, m];
        }


        /// <summary>
        /// Compares the properties of two objects of the same type and returns if all properties are equal.
        /// </summary>
        /// <param name="objectA">The first object to compare.</param>
        /// <param name="objectB">The second object to compre.</param>
        /// <param name="ignoreList">A list of property names to ignore from the comparison.</param>
        /// <returns><c>true</c> if all property values are equal, otherwise <c>false</c>.</returns>
        public static bool AreObjectsEqual(object objectA, object objectB, bool debug = false, params string[] ignoreList)
        {
            bool result;

            if (objectA != null && objectB != null)
            {
                Type objectType;

                objectType = objectA.GetType();

                result = true; // assume by default they are equal

                foreach (PropertyInfo propertyInfo in objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && !ignoreList.Contains(p.Name)))
                {
                    object valueA;
                    object valueB;

                    valueA = propertyInfo.GetValue(objectA, null);
                    valueB = propertyInfo.GetValue(objectB, null);

                    // if it is a primative type, value type or implements IComparable, just directly try and compare the value
                    if (CanDirectlyCompare(propertyInfo.PropertyType))
                    {
                        if (!AreValuesEqual(valueA, valueB))
                        {
                            if (debug) Console.WriteLine("Mismatch with property '{0}.{1}' found.", objectType.FullName, propertyInfo.Name);
                            result = false;
                        }
                    }
                    // if it implements IEnumerable, then scan any items
                    else if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        IEnumerable<object> collectionItems1;
                        IEnumerable<object> collectionItems2;
                        int collectionItemsCount1;
                        int collectionItemsCount2;

                        // null check
                        if (valueA == null && valueB != null || valueA != null && valueB == null)
                        {
                            if (debug) Console.WriteLine("Mismatch with property '{0}.{1}' found.", objectType.FullName, propertyInfo.Name);
                            result = false;
                        }
                        else if (valueA != null && valueB != null)
                        {
                            collectionItems1 = ((IEnumerable)valueA).Cast<object>();
                            collectionItems2 = ((IEnumerable)valueB).Cast<object>();
                            collectionItemsCount1 = collectionItems1.Count();
                            collectionItemsCount2 = collectionItems2.Count();

                            // check the counts to ensure they match
                            if (collectionItemsCount1 != collectionItemsCount2)
                            {
                                if (debug) Console.WriteLine("Collection counts for property '{0}.{1}' do not match.", objectType.FullName, propertyInfo.Name);
                                result = false;
                            }
                            // and if they do, compare each item... this assumes both collections have the same order
                            else
                            {
                                for (int i = 0; i < collectionItemsCount1; i++)
                                {
                                    object collectionItem1;
                                    object collectionItem2;
                                    Type collectionItemType;

                                    collectionItem1 = collectionItems1.ElementAt(i);
                                    collectionItem2 = collectionItems2.ElementAt(i);
                                    collectionItemType = collectionItem1.GetType();

                                    if (CanDirectlyCompare(collectionItemType))
                                    {
                                        if (!AreValuesEqual(collectionItem1, collectionItem2))
                                        {
                                            if (debug) Console.WriteLine("Item {0} in property collection '{1}.{2}' does not match.", i, objectType.FullName, propertyInfo.Name);
                                            result = false;
                                        }
                                    }
                                    else if (!AreObjectsEqual(collectionItem1, collectionItem2,debug, ignoreList))
                                    {
                                        if (debug) Console.WriteLine("Item {0} in property collection '{1}.{2}' does not match.", i, objectType.FullName, propertyInfo.Name);
                                        result = false;
                                    }
                                }
                            }
                        }
                    }
                    else if (propertyInfo.PropertyType.IsClass)
                    {
                        if (!AreObjectsEqual(propertyInfo.GetValue(objectA, null), propertyInfo.GetValue(objectB, null),debug, ignoreList))
                        {
                            if (debug) Console.WriteLine("Mismatch with property '{0}.{1}' found.", objectType.FullName, propertyInfo.Name);
                            result = false;
                        }
                    }
                    else
                    {
                        if (debug) Console.WriteLine("Cannot compare property '{0}.{1}'.", objectType.FullName, propertyInfo.Name);
                        result = false;
                    }
                }
            }
            else
                result = object.Equals(objectA, objectB);

            return result;
        }

        /// <summary>
        /// Determines whether value instances of the specified type can be directly compared.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if this value instances of the specified type can be directly compared; otherwise, <c>false</c>.
        /// </returns>
        private static bool CanDirectlyCompare(Type type)
        {
            return typeof(IComparable).IsAssignableFrom(type) || type.IsPrimitive || type.IsValueType;
        }

        /// <summary>
        /// Compares two values and returns if they are the same.
        /// </summary>
        /// <param name="valueA">The first value to compare.</param>
        /// <param name="valueB">The second value to compare.</param>
        /// <returns><c>true</c> if both values match, otherwise <c>false</c>.</returns>
        private static bool AreValuesEqual(object valueA, object valueB)
        {
            bool result;
            IComparable selfValueComparer;

            selfValueComparer = valueA as IComparable;

            if (valueA == null && valueB != null || valueA != null && valueB == null)
                result = false; // one of the values is null
            else if (selfValueComparer != null && selfValueComparer.CompareTo(valueB) != 0)
                result = false; // the comparison using IComparable failed
            else if (!object.Equals(valueA, valueB))
                result = false; // the comparison using Equals failed
            else
                result = true; // match

            return result;
        }

    }
}
