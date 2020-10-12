using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HlidacStatu.Api.Dataset.Connector;

namespace Vybory_PSP
{
    public class Parse
    {
        private class _usneseni
        {
            public DateTime datum { get; set; }
            public string popis { get; set; }
            public string fileUrl { get; set; }
            public int cislo { get; set; }
        }

        public static Dictionary<int, string> Vybory = new Dictionary<int, string>();

        public const string datasetname = "Vybory-PSP";
        public static bool parallel = true;

        static string _listVybory = "https://www.psp.cz/sqw/hp.sqw?k=194";
        static string _vyborHP = "https://www.psp.cz/sqw/hp.sqw?k={0}"; //idVyboru +2
        static string _vyborJednaniHProot = "https://www.psp.cz/sqw/"; //idVyboru +6, 
        static string _vyborUsneseni = "https://www.psp.cz/sqw/hp.sqw?k={0}&kk=5&td=1&n={1}"; //idVyboru+5 , page

        public static void Vybor(DatasetConnector dsc, int vyborId, bool rewrite = false)
        {


            List<_usneseni> vsechnausneseni = VsechnaUsneseniVyboru(vyborId);

            Console.WriteLine($"Vybor {vyborId}");

            int lastJednani = 0;
            var xp = GetPage(string.Format(_vyborHP, vyborId + 2));
            var xpozvanky = xp.GetNodes("//div[@id='main-content']//h2")[0].NextSibling.SelectNodes(".//tr//a");
            string jednaniUrlTemplate = "";

            bool komplexni = false;

            if (xpozvanky != null && xpozvanky.Count > 0)
            {
                var link = xpozvanky[0].GetAttributeValue("href", "");
                if (link.StartsWith("hp.sqw"))
                {
                    komplexni = true;
                    var tmp = GetRegexGroupValue(link, @"&cu=(?<cislo>\d*)", "cislo");
                    int.TryParse(tmp, out lastJednani);
                    if (lastJednani > 0)
                    {
                        jednaniUrlTemplate = _vyborJednaniHProot + link.Replace("&cu=" + lastJednani, "&cu={0}");
                    }
                }
                else
                {
                    komplexni = false;
                    var tmp = GetRegexGroupValue(System.Net.WebUtility.HtmlDecode(xpozvanky[0].InnerText), @"č\. \s* (?<cislo>\d*)", "cislo");
                    int.TryParse(tmp, out lastJednani);

                }
            }

            if (komplexni && lastJednani > 0 && !string.IsNullOrEmpty(jednaniUrlTemplate))
            {
                //parse HP jednani
                //for (int cisloJednani = lastJednani; cisloJednani > 0; cisloJednani--)
                Devmasters.Batch.Manager.DoActionForAll<int>(Enumerable.Range(1, lastJednani).Reverse(),
                    (cisloJednani) =>
                {
                    var jednani = JednaniKomplexni(vyborId, cisloJednani, string.Format(jednaniUrlTemplate, cisloJednani));
                    if (jednani == null)
                        return new Devmasters.Batch.ActionOutputData();
                    //add usneseni
                    var docFromUsneseni = vsechnausneseni
                        .Where(m => m.datum == jednani.datum)
                        .Select(m => new jednani.dokument()
                        {
                            typ = "Usnesení",
                            DocumentUrl = m.fileUrl,
                            popis = m.popis,
                            jmeno = $"usneseni_{m.cislo}.docx"
                        }).ToList();
                    if (docFromUsneseni.Count() > 0)
                    {
                        jednani.dokumenty = jednani.dokumenty.Concat(docFromUsneseni).ToArray();

                    }
                    jednani.SetId();
                    //merge with existing

                    jednani exists = null;
                    try
                    {
                        exists = dsc.GetItemFromDataset<jednani>(datasetname, jednani.Id).Result;
                    }
                    catch (Exception)
                    {
                    }

                    bool changed = true;
                    if (exists != null)
                        jednani = jednani.Merge(exists, jednani, out changed);

                    //add OCR
                    Devmasters.Batch.Manager.DoActionForAll<jednani.dokument>(jednani.dokumenty,
                        d =>
                        {
                            if (string.IsNullOrWhiteSpace(d.DocumentPlainText))
                            {
                                Console.WriteLine("OCR for " + d.DocumentUrl);
                                var ocrRes = HlidacStatu.Lib.OCR.Api.Client.TextFromUrl(
                                    Devmasters.Config.GetWebConfigValue("OCRServerApiKey"),
                                    new Uri(d.DocumentUrl),
                                    "Vybory-PSP-parser", HlidacStatu.Lib.OCR.Api.Client.TaskPriority.High,
                                     HlidacStatu.Lib.OCR.Api.Client.MiningIntensity.Maximum,
                                    null, TimeSpan.FromMinutes(120));

                                if (ocrRes.IsValid == HlidacStatu.Lib.OCR.Api.Result.ResultStatus.Valid)
                                {
                                    d.DocumentPlainText = ocrRes.MergedDocuments().Text;
                                    changed = true;
                                }
                                else
                                {
                                    Console.WriteLine($"Invalid OCR {d.DocumentUrl} - {ocrRes.Error}");
                                }
                            }
                            return new Devmasters.Batch.ActionOutputData();

                        }, null,null, true, prefix:$"Jednani {cisloJednani} OCR:");


                    string jedn = "zápis";
                    if (jednani.dokumenty.Any(m => m.typ.ToLower().Contains(jedn)))
                    {
                        jednani.zapisJednani = string.Join(@"\n",
                            jednani.dokumenty.Where(m => m.typ.ToLower().Contains(jedn)).Select(m => m.DocumentPlainText)
                            );
                    }


                    string id = "";
                    if (changed)
                        id = dsc.AddItemToDataset(datasetname, jednani, DatasetConnector.AddItemMode.Rewrite).Result;
                    Console.WriteLine($"Saved vybor {jednani.vybor} jednani {jednani.Id} id {id}");
                    return new Devmasters.Batch.ActionOutputData();

                }, null, null, true, maxDegreeOfParallelism:5, prefix: "cisloJednani: ");
            }

        }

        private static jednani JednaniKomplexni(int vyborId, int cisloJednani, string url)
        {
            Console.WriteLine($"Vybor {vyborId} jednani {cisloJednani}");

            var xp = GetPage(url);
            jednani j = new jednani();
            j.cisloJednani = cisloJednani;

            //simpledatum
            string sdatum = "";
            sdatum = GetRegexGroupValue(
                xp.GetNodeText("//div[@id='main-content']//h1")
                , @"\(\s*  (?<datum>\d{1,2}\. \s \w{4,15} \s \d{4})  \s* \)"
                , "datum");
            if (string.IsNullOrEmpty(sdatum))
            {
                string regexKombiDatum = @"\(\s* 

((?<den>\d{1,2})\. \s* ((?<mesic>\w{4,15})\s* a|a|až) )? 

\s* \d{1,2}\. \s (?<mesic>\w{4,15}) \s (?<rok>\d{4})

\s* \)";
                //kombinovany datum
                var sden = GetRegexGroupValue(
                    xp.GetNodeText("//div[@id='main-content']//h1")
                    , regexKombiDatum
                    , "den");
                var smesic = GetRegexGroupValue(
                    xp.GetNodeText("//div[@id='main-content']//h1")
                    , regexKombiDatum
                    , "mesic");
                var srok = GetRegexGroupValue(
                    xp.GetNodeText("//div[@id='main-content']//h1")
                    , regexKombiDatum
                    , "rok");

                sdatum = $"{sden}. {smesic} {srok}";
            }

            try
            {
                j.datum = DateTime.ParseExact(sdatum, "d. MMMM yyyy", System.Globalization.CultureInfo.GetCultureInfo("cs-CZ"));
            }
            catch (Exception)
            {
                return null;
            }
            j.vybor = Vybory[vyborId];
            j.vyborId = vyborId;
            j.vyborUrl = string.Format(_vyborHP, vyborId + 2);

            //typy dokumentu
            var xtypes = xp.GetNodes("//div[@id='main-content']//h4");
            List<jednani.dokument> docs = new List<jednani.dokument>();
            List<jednani.mp3> mp3s = new List<jednani.mp3>();

            for (int ityp = 0; ityp < xtypes.Count; ityp++)
            {
                var typDok = xtypes[ityp].InnerText;
                var xrows = xtypes[ityp].NextSibling.SelectNodes(".//tr");
                if (xrows != null)
                {
                    foreach (var xrow in xrows)
                    {
                        var rootUrl = "https://www.psp.cz/sqw/";
                        var fUrlNode = XPath.Tools.GetNode(xrow, "td[1]/a");
                        if (fUrlNode != null)
                        {
                            var fUrl = fUrlNode.GetAttributeValue("href", "");
                            if (fUrl.StartsWith("text/text")) //link to another page
                            {
                                FilePage(rootUrl + fUrl, out string title, out string fileUrl, out string ext);
                                if (!string.IsNullOrEmpty(fileUrl))
                                    docs.Add(new jednani.dokument()
                                    {
                                        DocumentUrl = fileUrl,
                                        jmeno = MakeValidFileName(title) + ext,
                                        popis = title,
                                        typ = typDok,
                                    });
                            }
                            else if (fUrl.StartsWith("text/orig"))
                            {
                                if (typDok.ToLower().Contains(".mp3"))
                                {
                                    //direct link to file
                                    mp3s.Add(new jednani.mp3()
                                    {
                                        DocumentUrl = rootUrl + fUrl,
                                        jmeno = MakeValidFileName(fUrlNode.InnerText),
                                    });
                                }
                                else
                                {
                                    //direct link to file
                                    docs.Add(new jednani.dokument()
                                    {
                                        DocumentUrl = rootUrl + fUrl,
                                        jmeno = MakeValidFileName(fUrlNode.InnerText),
                                        popis = "",
                                        typ = typDok,
                                    });
                                }
                            }
                        } //fUrlNode != null
                    }
                }
            }

            j.audio = mp3s.Count == 0 ? null : mp3s.ToArray();


            j.dokumenty = docs.Count == 0 ? null : docs.ToArray();


            return j;
        }


        private static List<_usneseni> VsechnaUsneseniVyboru(int vyborId)
        {
            Console.Write($"Vsechna usneseni vyboru {vyborId}");
            List<_usneseni> ret = new List<_usneseni>();
            int page = 1;
            while (true)
            {
                Console.Write($" p{page}");
                string url = string.Format(_vyborUsneseni, vyborId + 5, page);
                var xp = GetPage(url);

                var xrows = xp.GetNodes("//div[@id='main-content']//table//tr");
                if (xrows == null)
                    return ret;
                if (xrows.Count == 0)
                    return ret;

                foreach (var xrow in xrows)
                {
                    _usneseni j = new _usneseni();
                    var jdoc = XPath.Tools.GetNode(xrow, "td[1]/a");
                    j.popis = XPath.Tools.GetNodeText(xrow, "td[3]")?.Trim() ?? "";
                    if (!string.IsNullOrEmpty(j.popis))
                        j.popis += ": ";
                    j.popis += XPath.Tools.GetNodeText(xrow, "td[2]");
                    var pageUrl = "https://www.psp.cz/sqw/" + jdoc.GetAttributeValue("href", "");
                    Console.Write(".");
                    UsneseniPage(pageUrl, ref j);
                    ret.Add(j);
                }



                page++;
            }

            //return ret;
        }
        private static void FilePage(string url, out string title, out string fileUrl, out string ext)
        {
            var xp = GetPage(url);
            title = xp.GetNodeText("//div[@id='main-content']//div[@class='page-title']");
            fileUrl = "";
            ext = ".";
            var files = xp.GetNodes("//div[@id='main-content']//div[@class='document-media-attachments-x']//ul//li");
            if (files != null && files.Count > 0)
            {
                fileUrl = "https://www.psp.cz/sqw/text/" + XPath.Tools.GetNodeAttributeValue(files[0], "a", "href");
                ext += files[0].GetAttributeValue("class", "").ToLower();
            }
            if (ext == ".")
                ext += "docx";
        }

        private static void UsneseniPage(string url, ref _usneseni j)
        {
            var xp = GetPage(url);
            var title = xp.GetNodeText("//div[@id='main-content']//div[@class='page-title']");
            var datum = GetRegexGroupValue(title, @"\((?<datum>\d{1,2}(\.)? \s* \w{4,15}\s*\d{4}) (\w|\s|,)*  \)", "datum");
            if (!string.IsNullOrEmpty(datum))
                j.datum = DateTime.ParseExact(datum, new string[] { "d MMMM yyyy", "d. MMMM yyyy", "d. MMMMyyyy" }, System.Globalization.CultureInfo.GetCultureInfo("cs-CZ"), System.Globalization.DateTimeStyles.AssumeLocal); //19. února 2020
            j.cislo = int.Parse(GetRegexGroupValue(title, @"č\. \s* (?<cislo>\d*) \s*", "cislo"));

            var files = xp.GetNodes("//div[@id='main-content']//div[@class='document-media-attachments-x']//ul//li");
            if (files != null && files.Count > 0)
            {
                j.fileUrl = "https://www.psp.cz/sqw/text/" + XPath.Tools.GetNodeAttributeValue(files[0], "a", "href");
            }
        }

        private static XPath GetPage(string url)
        {
            try
            {
                string html = "";
                using (Devmasters.Net.HttpClient.URLContent net = new Devmasters.Net.HttpClient.URLContent(url))
                {
                    net.IgnoreHttpErrors = false;
                    //Console.WriteLine($"Downloading {url} ");
                    net.Timeout = 60000; 
                    net.Tries = 5;
                    html = net.GetContent().Text;
                }
                return new XPath(html);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"{url} - {ex.Message}");
                throw ex;
            }

        }

        public static void InitVybory()
        {
            var xp = GetPage(_listVybory);
            var xlinks = xp.GetNodes("//div[@id='main-content']//div[@class='section']//ul//li/a[starts-with(@href,'hp.sqw?k=')]");
            foreach (var xl in xlinks)
            {
                Vybory.Add(
                    int.Parse(xl.GetAttributeValue("href", "").Replace("hp.sqw?k=", ""))
                    , xl.InnerText
                    );
            }
        }

        /// <summary>
        /// Strip illegal chars and reserved words from a candidate filename (should not include the directory path)
        /// </summary>
        /// <remarks>
        /// http://stackoverflow.com/questions/309485/c-sharp-sanitize-file-name
        /// </remarks>
        public static string MakeValidFileName(string filename)
        {
            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()) + "=()") + @"\s"; //pridano \s ()
            var invalidReStr = string.Format(@"[{0}]+", invalidChars);

            var reservedWords = new[]
            {
        "CON", "PRN", "AUX", "CLOCK$", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4",
        "COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4",
        "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
    };

            var sanitisedNamePart = Regex.Replace(filename, invalidReStr, "_");
            foreach (var reservedWord in reservedWords)
            {
                var reservedWordPattern = string.Format("^{0}\\.", reservedWord);
                sanitisedNamePart = Regex.Replace(sanitisedNamePart, reservedWordPattern, "_reservedWord_.", RegexOptions.IgnoreCase);
            }

            return Devmasters.TextUtil.RemoveDiacritics(sanitisedNamePart);
        }

//        static string regexRokFromTxt = @"č\. \s* (?<cislo>\d*)
//\s*
//(
//	(ze \s* dne \s* ) (\d* \. \d* \. (?<rok>\d{4})) 
//|
//	(z \s* roku \s* ) ( (?<rok>\d{4})) 

//)";


        private static string NormalizeUrl(string url, string prefix = "https://apps.odok.cz")
        {
            if (string.IsNullOrWhiteSpace(url))
                return url;
            if (url.ToLower().StartsWith("http"))
                return url;
            if (!url.StartsWith("/"))
                return prefix + "/" + url;
            else
                return prefix + url;
        }


        public static string GetRegexGroupValue(string txt, string regex, string groupname)
        {
            if (string.IsNullOrEmpty(txt))
                return null;
            Regex myRegex = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant);
            foreach (Match match in myRegex.Matches(txt))
            {
                if (match.Success)
                {
                    if (match.Groups[groupname].Captures.Count > 1)
                        return match.Groups[groupname].Captures[0].Value;
                    else
                        return match.Groups[groupname].Value;
                }
            }
            return string.Empty;
        }

    }
}

