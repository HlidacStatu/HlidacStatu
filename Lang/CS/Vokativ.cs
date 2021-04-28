using System.Collections.Generic;
using System.Linq;


namespace HlidacStatu.Lang.CS
{
    public class Vokativ
    {
        public enum SexEnum
        {
            Unknown = 0,
            Woman = 1,
            Man = 2
        }

        internal class vokativDef
        {

            internal string NormalEnd { get; set; }
            internal string VokativEnd { get; set; }
            internal SexEnum Sex { get; set; }
            internal vokativDef(string normalend, string vokativend, SexEnum sex)
            {
                this.NormalEnd = normalend;
                this.VokativEnd = vokativend;
                this.Sex = sex;
            }
        }

        private volatile static Dictionary<string, vokativDef> vokativData = new Dictionary<string, vokativDef>();
        private volatile static int[] vokativDataLength = null;

        private volatile static List<string> postfixes = new List<string>() {
            "dis","mba","csc.","ph.d.","th.d.","drsc.","dr. h. c.","dr.h.c."
        };

        private static object lockObj = new object();
        static Vokativ()
        {
            Dictionary<string, vokativDef> tmpData = new Dictionary<string, vokativDef>();
            lock (lockObj)
            {
                #region zakladni pravidla
                tmpData.Clear();


                tmpData.Add("ac", new vokativDef("ac", "aci", SexEnum.Man));
                tmpData.Add("ad", new vokativDef("ad", "ade", SexEnum.Man));
                tmpData.Add("aj", new vokativDef("aj", "aji", SexEnum.Man));
                tmpData.Add("ak", new vokativDef("ak", "aku", SexEnum.Man));
                tmpData.Add("al", new vokativDef("al", "ale", SexEnum.Man));
                tmpData.Add("an", new vokativDef("an", "ane", SexEnum.Man));
                tmpData.Add("ar", new vokativDef("ar", "are", SexEnum.Man));
                tmpData.Add("ař", new vokativDef("ař", "aři", SexEnum.Man));
                tmpData.Add("as", new vokativDef("as", "asi", SexEnum.Man));
                tmpData.Add("ba", new vokativDef("ba", "bo", SexEnum.Man));
                tmpData.Add("bec", new vokativDef("bec", "bci", SexEnum.Man));
                tmpData.Add("ben", new vokativDef("ben", "bene", SexEnum.Man));
                tmpData.Add("bý", new vokativDef("bý", "bý", SexEnum.Man));
                tmpData.Add("ca", new vokativDef("ca", "co", SexEnum.Man));
                tmpData.Add("ča", new vokativDef("ča", "čo", SexEnum.Man));
                tmpData.Add("ch", new vokativDef("ch", "chu", SexEnum.Man));
                tmpData.Add("cha", new vokativDef("cha", "cho", SexEnum.Man));
                tmpData.Add("chý", new vokativDef("chý", "chý", SexEnum.Man));
                tmpData.Add("cl", new vokativDef("cl", "cle", SexEnum.Man));
                tmpData.Add("cí", new vokativDef("cí", "cí", SexEnum.Man));
                tmpData.Add("da", new vokativDef("da", "do", SexEnum.Man));
                tmpData.Add("dec", new vokativDef("dec", "dci", SexEnum.Man));
                tmpData.Add("dek", new vokativDef("dek", "deku", SexEnum.Man));
                tmpData.Add("del", new vokativDef("del", "deli", SexEnum.Man));
                tmpData.Add("dl", new vokativDef("dl", "dle", SexEnum.Man));
                tmpData.Add("dr", new vokativDef("dr", "dre", SexEnum.Man));
                tmpData.Add("dt", new vokativDef("dt", "dte", SexEnum.Man));
                tmpData.Add("dý", new vokativDef("dý", "dý", SexEnum.Man));
                tmpData.Add("ed", new vokativDef("ed", "ede", SexEnum.Man));
                tmpData.Add("ek", new vokativDef("ek", "ku", SexEnum.Man));
                tmpData.Add("el", new vokativDef("el", "li", SexEnum.Man));
                tmpData.Add("er", new vokativDef("er", "ere", SexEnum.Man));
                tmpData.Add("eš", new vokativDef("eš", "eši", SexEnum.Man));
                tmpData.Add("ga", new vokativDef("ga", "gp", SexEnum.Man));
                tmpData.Add("gr", new vokativDef("gr", "gre", SexEnum.Man));
                tmpData.Add("gy", new vokativDef("gy", "gy", SexEnum.Man));
                tmpData.Add("ha", new vokativDef("ha", "ho", SexEnum.Man));
                tmpData.Add("hl", new vokativDef("hl", "hle", SexEnum.Man));
                tmpData.Add("hm", new vokativDef("hm", "hme", SexEnum.Man));
                tmpData.Add("ht", new vokativDef("ht", "hte", SexEnum.Man));
                tmpData.Add("hý", new vokativDef("hý", "hý", SexEnum.Man));
                tmpData.Add("id", new vokativDef("id", "ide", SexEnum.Man));
                tmpData.Add("il", new vokativDef("il", "ile", SexEnum.Man));
                tmpData.Add("in", new vokativDef("in", "ine", SexEnum.Man));
                tmpData.Add("iš", new vokativDef("iš", "iši", SexEnum.Man));
                tmpData.Add("jl", new vokativDef("jl", "jle", SexEnum.Man));
                tmpData.Add("jš", new vokativDef("jš", "jši", SexEnum.Man));
                tmpData.Add("ka", new vokativDef("ka", "ko", SexEnum.Man));
                tmpData.Add("kl", new vokativDef("kl", "kle", SexEnum.Man));
                tmpData.Add("ko", new vokativDef("ko", "ko", SexEnum.Man));
                tmpData.Add("ky", new vokativDef("ky", "ky", SexEnum.Man));
                tmpData.Add("ký", new vokativDef("ký", "ký", SexEnum.Man));
                tmpData.Add("la", new vokativDef("la", "lo", SexEnum.Man));
                tmpData.Add("lc", new vokativDef("lc", "lci", SexEnum.Man));
                tmpData.Add("ld", new vokativDef("ld", "lde", SexEnum.Man));
                tmpData.Add("le", new vokativDef("le", "le", SexEnum.Man));
                tmpData.Add("lec", new vokativDef("lec", "lci", SexEnum.Man));
                tmpData.Add("lf", new vokativDef("lf", "lfe", SexEnum.Man));
                tmpData.Add("lk", new vokativDef("lk", "lku", SexEnum.Man));
                tmpData.Add("lt", new vokativDef("lt", "lte", SexEnum.Man));
                tmpData.Add("lu", new vokativDef("lu", "lu", SexEnum.Man));
                tmpData.Add("lý", new vokativDef("lý", "lý", SexEnum.Man));
                tmpData.Add("ma", new vokativDef("ma", "mo", SexEnum.Man));
                tmpData.Add("mec", new vokativDef("mec", "mci", SexEnum.Man));
                tmpData.Add("mel", new vokativDef("mel", "meli", SexEnum.Man));
                tmpData.Add("mý", new vokativDef("mý", "mý", SexEnum.Man));
                tmpData.Add("na", new vokativDef("na", "no", SexEnum.Man));
                tmpData.Add("ňa", new vokativDef("ňa", "ňo", SexEnum.Man));
                tmpData.Add("nc", new vokativDef("nc", "nci", SexEnum.Man));
                tmpData.Add("nd", new vokativDef("nd", "nde", SexEnum.Man));
                tmpData.Add("nec", new vokativDef("nec", "nci", SexEnum.Man));
                tmpData.Add("nek", new vokativDef("nek", "nku", SexEnum.Man));
                tmpData.Add("nk", new vokativDef("nk", "nku", SexEnum.Man));
                tmpData.Add("nn", new vokativDef("nn", "nne", SexEnum.Man));
                tmpData.Add("nt", new vokativDef("nt", "nte", SexEnum.Man));
                tmpData.Add("nš", new vokativDef("nš", "nši", SexEnum.Man));
                tmpData.Add("ný", new vokativDef("ný", "ný", SexEnum.Man));
                tmpData.Add("of", new vokativDef("of", "ofe", SexEnum.Man));
                tmpData.Add("on", new vokativDef("on", "oni", SexEnum.Man));
                tmpData.Add("op", new vokativDef("op", "ope", SexEnum.Man));
                tmpData.Add("or", new vokativDef("or", "ore", SexEnum.Man));
                tmpData.Add("oř", new vokativDef("oř", "oři", SexEnum.Man));
                tmpData.Add("oš", new vokativDef("oš", "oši", SexEnum.Man));
                tmpData.Add("pa", new vokativDef("pa", "po", SexEnum.Man));
                tmpData.Add("pl", new vokativDef("pl", "ple", SexEnum.Man));
                tmpData.Add("ra", new vokativDef("ra", "ro", SexEnum.Man));
                tmpData.Add("řa", new vokativDef("řa", "řo", SexEnum.Man));
                tmpData.Add("rb", new vokativDef("rb", "rbe", SexEnum.Man));
                tmpData.Add("rc", new vokativDef("rc", "rci", SexEnum.Man));
                tmpData.Add("rd", new vokativDef("rd", "rde", SexEnum.Man));
                tmpData.Add("rel", new vokativDef("rel", "rle", SexEnum.Man));
                tmpData.Add("rt", new vokativDef("rt", "rte", SexEnum.Man));
                tmpData.Add("ru", new vokativDef("ru", "ru", SexEnum.Man));
                tmpData.Add("rž", new vokativDef("rž", "rži", SexEnum.Man));
                tmpData.Add("rý", new vokativDef("rý", "rý", SexEnum.Man));
                tmpData.Add("sa", new vokativDef("sa", "so", SexEnum.Man));
                tmpData.Add("ss", new vokativDef("ss", "ssi", SexEnum.Man));
                tmpData.Add("st", new vokativDef("st", "ste", SexEnum.Man));
                tmpData.Add("ta", new vokativDef("ta", "to", SexEnum.Man));
                tmpData.Add("ter", new vokativDef("ter", "tre", SexEnum.Man));
                tmpData.Add("th", new vokativDef("th", "the", SexEnum.Man));
                tmpData.Add("tr", new vokativDef("tr", "tre", SexEnum.Man));
                tmpData.Add("ub", new vokativDef("ub", "ube", SexEnum.Man));
                tmpData.Add("uben", new vokativDef("uben", "ubne", SexEnum.Man));
                tmpData.Add("uk", new vokativDef("uk", "uku", SexEnum.Man));
                tmpData.Add("ul", new vokativDef("ul", "ule", SexEnum.Man));
                tmpData.Add("un", new vokativDef("un", "une", SexEnum.Man));
                tmpData.Add("us", new vokativDef("us", "usi", SexEnum.Man));
                tmpData.Add("uš", new vokativDef("uš", "uši", SexEnum.Man));
                tmpData.Add("va", new vokativDef("va", "vo", SexEnum.Man));
                tmpData.Add("vec", new vokativDef("vec", "vci", SexEnum.Man));
                tmpData.Add("věc", new vokativDef("věc", "vci", SexEnum.Man));
                tmpData.Add("vel", new vokativDef("vel", "vle", SexEnum.Man));
                tmpData.Add("vý", new vokativDef("vý", "vý", SexEnum.Man));
                tmpData.Add("xa", new vokativDef("xa", "xo", SexEnum.Man));
                tmpData.Add("yl", new vokativDef("yl", "yle", SexEnum.Man));
                tmpData.Add("ys", new vokativDef("ys", "ysi", SexEnum.Man));
                tmpData.Add("za", new vokativDef("za", "zo", SexEnum.Man));
                tmpData.Add("ža", new vokativDef("ža", "žo", SexEnum.Man));
                tmpData.Add("íž", new vokativDef("íž", "íži", SexEnum.Man));
                tmpData.Add("zd", new vokativDef("zd", "zde", SexEnum.Man));
                tmpData.Add("ša", new vokativDef("ša", "šo", SexEnum.Man));
                tmpData.Add("ší", new vokativDef("ší", "ší", SexEnum.Man));
                tmpData.Add("áb", new vokativDef("áb", "ábe", SexEnum.Man));
                tmpData.Add("ác", new vokativDef("ác", "áci", SexEnum.Man));
                tmpData.Add("ád", new vokativDef("ád", "áde", SexEnum.Man));
                tmpData.Add("áh", new vokativDef("áh", "áhu", SexEnum.Man));
                tmpData.Add("ák", new vokativDef("ák", "áku", SexEnum.Man));
                tmpData.Add("ál", new vokativDef("ál", "ále", SexEnum.Man));
                tmpData.Add("án", new vokativDef("án", "áne", SexEnum.Man));
                tmpData.Add("áp", new vokativDef("áp", "ápe", SexEnum.Man));
                tmpData.Add("ár", new vokativDef("ár", "áre", SexEnum.Man));
                tmpData.Add("ář", new vokativDef("ář", "áři", SexEnum.Man));
                tmpData.Add("át", new vokativDef("át", "áte", SexEnum.Man));
                tmpData.Add("áz", new vokativDef("áz", "ázi", SexEnum.Man));
                tmpData.Add("áš", new vokativDef("áš", "áši", SexEnum.Man));
                tmpData.Add("áž", new vokativDef("áž", "áži", SexEnum.Man));
                tmpData.Add("íc", new vokativDef("íc", "íci", SexEnum.Man));
                tmpData.Add("ík", new vokativDef("ík", "íku", SexEnum.Man));
                tmpData.Add("ín", new vokativDef("ín", "íne", SexEnum.Man));
                tmpData.Add("íp", new vokativDef("íp", "ípe", SexEnum.Man));
                tmpData.Add("ír", new vokativDef("ír", "íre", SexEnum.Man));
                tmpData.Add("íř", new vokativDef("íř", "íři", SexEnum.Man));
                tmpData.Add("ít", new vokativDef("ít", "íte", SexEnum.Man));
                tmpData.Add("íš", new vokativDef("íš", "íši", SexEnum.Man));
                tmpData.Add("ýn", new vokativDef("ýn", "ýne", SexEnum.Man));

                #endregion
                //zeny
                tmpData.Add("ová", new vokativDef("ová", "ová", SexEnum.Woman));
                tmpData.Add("ova", new vokativDef("ova", "ova", SexEnum.Woman));
                tmpData.Add("lá", new vokativDef("lá", "lá", SexEnum.Woman));


                //vyjimky
                tmpData.Add("švec", new vokativDef("svec", "sveci", SexEnum.Man));
                tmpData.Add("svec", new vokativDef("švec", "šveci", SexEnum.Man));
                tmpData.Add("kadlec", new vokativDef("kadlec", "kadleci", SexEnum.Man));

                //resort
                vokativData = tmpData.OrderByDescending(m => m.Key.Length).ToDictionary(k => k.Key, v => v.Value);
                //velikosti
                vokativDataLength = vokativData.Select(m => m.Key.Length).Distinct().OrderByDescending(m => m).ToArray();
            }
        }

        public string OrigFullname { get; private set; }
        public string LastName { get; private set; }
        public bool CleanUp { get; private set; }
        public string NameVokativ { get; private set; }
        public SexEnum Sex { get; private set; }
        public Vokativ(string fullName, bool cleanUp = true)
        {
            this.OrigFullname = fullName.Trim();
            this.CleanUp = cleanUp;
            GetVokativ();
        }

        private void GetVokativ()
        {
            if (CleanUp)
                LastName = CleanUpText();
            else
            {
                LastName = GetLastWord(OrigFullname);
            }

            this.NameVokativ = LastName;
            this.Sex = SexEnum.Unknown;

            for (int l = 0; l < vokativDataLength.Count(); l++)
            {
                if (LastName.Length >= vokativDataLength[l])
                {
                    string end = LastName.Substring(LastName.Length - vokativDataLength[l]).ToLower();
                    if (vokativData.ContainsKey(end))
                    {
                        this.NameVokativ = LastName.Substring(0, LastName.Length - vokativDataLength[l]) + vokativData[end].VokativEnd;
                        this.Sex = vokativData[end].Sex;
                        break;
                    }
                }
            }

            //make name nice, first UpperCase
            if (!string.IsNullOrEmpty(this.NameVokativ))
            {
                this.NameVokativ = this.NameVokativ[0].ToString().ToUpper() + this.NameVokativ.Substring(1).ToLower();
            }
        }

        private string CleanUpText()
        {
            string tmp = OrigFullname.Replace(",", " ").Replace("  ", " ").Replace("  ", " ");
            string lastWord = GetLastWord(tmp).ToLower().Trim();
            while (postfixes.Contains(lastWord)
                || lastWord.Length < 3
                )
            {
                if (tmp.Length < 3)
                    break;
                tmp = RemoveLastWord(tmp);
                lastWord = GetLastWord(tmp);
            }
            return GetLastWord(tmp);
        }



        private string GetLastWord(string t)
        {
            if (!string.IsNullOrEmpty(t) && t.IndexOf(" ") > -1)
            {
                return t.Substring(t.LastIndexOf(" ") + 1);
            }
            else
                return t;
        }
        private string RemoveLastWord(string t)
        {
            if (!string.IsNullOrEmpty(t) && t.IndexOf(" ") > -1)
            {
                return t.Substring(0, t.LastIndexOf(" "));
            }
            else
                return string.Empty;
        }
    }

}
