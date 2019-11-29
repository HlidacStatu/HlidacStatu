
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lang
{
    public class Stemmer
    {
        private static object lockObj = new object();
        private static Dictionary<string, Stemmer> instances = new Dictionary<string, Stemmer>();
        static string rootDir = "";
        private static HashSet<string> stopWords = new HashSet<string>();

        const string WordSeparator = " ";
        public static string[] Separators = new string[] {
            WordSeparator, " -", ".", ",","!", "?","<",">","{","}","=", "\\", "#","*", "(", ")", "[", "]", "/",
            "\x10", "\x13", "\t","\r","\n", ":", "- "," - ","++",
        };

        private NHunspell.Hunspell hunspell = null;
        private Stemmer(string lang)
        {
            string dotnetLang = lang.Replace("_", "-");
            hunspell = new NHunspell.Hunspell(rootDir + "\\" + lang + ".aff", rootDir + "\\" + lang + ".dic");
            stopWords = new HashSet<string>();

            System.IO.File.ReadAllLines(rootDir + "\\" + lang + ".stopwords")
                .ToList()
                .ForEach(m => stopWords.Add(m));

        }

        static Stemmer()
        {
            string cb = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            if (cb.StartsWith("file:///"))
                cb = cb.Replace("file:///", "").Replace("/", "\\");
            rootDir = System.IO.Path.GetDirectoryName(cb);
        }


        public static Stemmer Instance(string lang)
        {
            lock (lockObj)
            {
                if (instances.Keys.Contains(lang))
                    return instances[lang];
                else
                {
                    instances.Add(lang, new Stemmer(lang));
                    return instances[lang];
                }
            }
        }

        public List<Stem> GetStems(string text)
        {
            List<Stem> result = new List<Stem>();
            if (string.IsNullOrEmpty(text))
                return result;
            text = text.Trim().ToLower();
            //check if its multiwords
            string[] words = text.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
            List<string> stems = hunspellStem(text);
            for (int i = 0; i < words.Count(); i++)
            {
                string word = words[i];
                if (stopWords.Contains(word) 
                    //|| word.Length < 2
                    )
                    continue;

                stems = hunspellStem(word);
                Stem s = null;
                if (stems.Count > 0)
                {
                    //stems.Insert(0, word);
                    s = new Stem(stems, i);
                }
                else
                    s = new Stem(word, i);

                result.Add(s);
            }
            return result;
        }

        private List<string> hunspellStem(string word)
        {
            try
            {
                lock (lockObj)
                {
                    return this.hunspell.Stem(word);
                }
            }
            catch (Exception)
            {
                System.Threading.Thread.Sleep(50);

                try
                {
                    lock (lockObj)
                    {
                        return this.hunspell.Stem(word);
                    }
                }
                catch (Exception)
                {

                    try
                    {
                        System.Threading.Thread.Sleep(10);
                        lock (lockObj)
                        {
                            return this.hunspell.Stem(word);
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("hunspellStem  err");
                        throw;
                    }
                }
            }
        }

        public string GetStemmedTextSimple(string text)
        {
            var stems = GetStems(text);
            if (stems == null || stems.Count == 0)
                return string.Empty;
            else
                return stems.Select(m => m.First).Aggregate((f, s) => f + " " + s);
        }
        public string GetStemmedTextAllVariants(string text, string delimiter = "|")
        {
            throw new NotImplementedException();
            var stems = GetStems(text);
            if (stems == null || stems.Count == 0)
                return string.Empty;
            else
            {
                List<string> variants = new List<string>();
                int maxVariants = stems.Max(m => m.NumOfStems + 1);
                for (int variant = 0; variant < maxVariants; variant++)
                {
                    string var = "";
                    for (int stem = 0; stem < stems.Count; stem++)
                    {
                        if (variant == 0)
                            var = var + stems[stem].First;
                        else
                        {
                            //if ()
                        }
                    }
                    //variants.Add(variant);


                }
            }
        }

        public NHunspell.Hunspell HunspellObj
        {
            get
            {
                return hunspell;
            }
        }
    }
}
