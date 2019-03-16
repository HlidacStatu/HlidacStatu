using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HlidacStatu.Lib.Emails
{
    public class EmailEngine
    {
        public static ViewEngineCollection Engines = null;
        static RazorEngine.Configuration.TemplateServiceConfiguration razorEngineConfig 
            = new RazorEngine.Configuration.TemplateServiceConfiguration();

        static EmailEngine()
        {
            //razorEngineConfig.EncodedStringFactory = new RazorEngine.Text.RawStringFactory();
            razorEngineConfig.DisableTempFileLocking = true;
            var engineRazor = RazorEngine.Templating.RazorEngineService.Create(razorEngineConfig); // new API

            var postViewEngine = new Postal.ResourceRazorViewEngine(typeof(Lib.Emails.EmailMsg).Assembly, @"HlidacStatu.Lib.Emails.Templates",razorEngine:engineRazor);

            //var postViewEngine = new Postal.StringRazorViewEngine(typeof(Lib.Emails.EmailMsg).Assembly, @"HlidacStatu.Lib.Emails.Templates", razorEngine: engineRazor);

            Engines = new ViewEngineCollection
                {
                   postViewEngine
                };

        }

    }
}
