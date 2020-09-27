using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HlidacStatu.Lib
{
    public class Init
    {
        //public static Devmasters.Logging.Logger Logger = new Devmasters.Logging.Logger("HlidacSmluv");
        //public static System.Globalization.CultureInfo enCulture = System.Globalization.CultureInfo.InvariantCulture; //new System.Globalization.CultureInfo("en-US");
        //public static System.Globalization.CultureInfo czCulture = new System.Globalization.CultureInfo("cs-CZ");
        //public static Random Rnd = new Random();

        public static IO.PrilohaFile PrilohaLocalCopy = new IO.PrilohaFile();
        public static IO.OsobaFotkyFile OsobaFotky = new IO.OsobaFotkyFile();
        public static IO.UploadedTmpFile UploadedTmp = new IO.UploadedTmpFile();

        public static string WebAppDataPath = null;
        public static string WebAppRoot = null;


        static object lockObj = new object();
        static bool initialised = false;
        static Init()
        {
            lock (lockObj)
            {
                //TelemetryConfiguration.Active.InstrumentationKey = " your key ";
                if (!string.IsNullOrEmpty(Devmasters.Config.GetWebConfigValue("NewtonsoftJsonSchemaLicense")))
                    Newtonsoft.Json.Schema.License.RegisterLicense(Devmasters.Config.GetWebConfigValue("NewtonsoftJsonSchemaLicense"));

                if (!string.IsNullOrEmpty(Devmasters.Config.GetWebConfigValue("WebAppDataPath")))
                {
                    WebAppDataPath = Devmasters.Config.GetWebConfigValue("WebAppDataPath");
                }
                else if (System.Web.HttpContext.Current != null) //inside web app
                {
                    WebAppDataPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/");
                }
                else throw new System.IO.DirectoryNotFoundException("cannot find WebAppDataPath");

                if (!WebAppDataPath.EndsWith("\\"))
                    WebAppDataPath = WebAppDataPath + "\\";

                WebAppRoot = new System.IO.DirectoryInfo(WebAppDataPath).Parent.FullName;
                if (!WebAppRoot.EndsWith("\\"))
                    WebAppRoot = WebAppRoot + "\\";


            }
        }

        public static void Initialise()
        {
            lock (lockObj)
            {
                if (initialised == false)
                {

                    if (!string.IsNullOrEmpty(Devmasters.Config.GetWebConfigValue("WebAppDataPath")))
                    {
                        WebAppDataPath = Devmasters.Config.GetWebConfigValue("WebAppDataPath");
                    }
                    else if (System.Web.HttpContext.Current != null) //inside web app
                    {
                        WebAppDataPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/");
                    }
                    else throw new System.IO.DirectoryNotFoundException("cannot find WebAppDataPath");

                    if (!WebAppDataPath.EndsWith("\\"))
                        WebAppDataPath = WebAppDataPath + "\\";

                    WebAppRoot = new System.IO.DirectoryInfo(WebAppDataPath).Parent.FullName;
                    if (!WebAppRoot.EndsWith("\\"))
                        WebAppRoot = WebAppRoot + "\\";


                    HlidacStatu.DBUpgrades.DBUpgrader.UpgradeDatabases();
                    //if (loadStaticData)
                    //    Lib.StaticData.Init();
                    initialised = true;
                }
            }

        }



        public static string DatasetTemplateFunctions = @"
@helper fn_RenderPersonWithLink(dynamic osobaId)
{
    @Fn.RenderPersonWithLink(osobaId);
}
@helper fn_RenderCompanyWithLink(dynamic ico)
{
    @Fn.RenderCompanyWithLink(ico);
}


@functions
{
    public string fn_OsobaUrl(dynamic osobaId)
    {
        return ""https://www.hlidacstatu.cz/osoba/"" + osobaId;
    }
    public string fn_FirmaUrl(dynamic ico)
    {
        return ""https://www.hlidacstatu.cz/subjekt/"" + ico;
    }

    public string fn_DatasetUrl()
    {
        return Model.Dataset.DatasetUrl(false);
    }
    public string fn_DatasetItemUrl(dynamic id)
    {
        if (id == null)
            return string.Empty;
        string sid = id.ToString();
        return Model.Dataset.DatasetItemUrl(sid, false);
    }
    public string fn_ShortenText(dynamic value, int? length = null)
    {
        return Fn.ShortenText(value, length);
    }
    public string fn_FormatNumber(dynamic value, string format = null)
    {
        return Fn.FormatNumber(value, (string)format);
    }
    public string fn_FormatDate(dynamic value, string format = null, string[] inputformats = null)
    {
        return Fn.FormatDate(value, (string)format, (string[])inputformats);
    }
    public string fn_FixPlainText(dynamic text)
    {
        return Fn.FixPlainText(text);
    }
    public string fn_NormalizeText(dynamic text)
    {
        return Fn.NormalizeText(text);
    }
    public bool fn_IsNullOrEmpty(dynamic text)
    {
        return Fn.IsNullOrEmpty(text);
    }
}";

    }
}
