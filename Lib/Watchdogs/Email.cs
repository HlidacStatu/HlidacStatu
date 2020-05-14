using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Watchdogs
{
    public class Email
    {
        public static bool SendEmail(string toEmail, string subject, RenderedContent content, string fromEmail = "podpora@hlidacstatu.cz")
        {
            return HlidacStatu.Util.SMTPTools.SendEmail( subject, content.ContentHtml, content.ContentText, toEmail, fromEmail, false);
        }
    }
}
