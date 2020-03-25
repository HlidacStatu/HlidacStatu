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
            try
            {

                using (MailMessage msg = new MailMessage(fromEmail, toEmail))
                {
                    if (!string.IsNullOrEmpty(content.ContentText))
                    {
                        var view = AlternateView.CreateAlternateViewFromString(content.ContentText, new System.Net.Mime.ContentType("text/plain"));
                        view.ContentType.CharSet = Encoding.UTF8.WebName;
                        msg.AlternateViews.Add(view);
                    }
                    if (!string.IsNullOrEmpty(content.ContentHtml))
                    {
                        var view = AlternateView.CreateAlternateViewFromString(content.ContentHtml, new System.Net.Mime.ContentType("text/html"));
                        view.ContentType.CharSet = Encoding.UTF8.WebName;
                        msg.AlternateViews.Add(view);
                    }


                    msg.Subject = subject;
                    msg.BodyEncoding = System.Text.Encoding.UTF8;
                    msg.SubjectEncoding = System.Text.Encoding.UTF8;

                    using (SmtpClient smtp = new SmtpClient())
                    {
                        HlidacStatu.Util.Consts.Logger.Info("Sending email to " + msg.To);
                        //msg.Bcc.Add("michal@michalblaha.cz");
                        smtp.Send(msg);
                        return true;
                    }

                }
            }
            catch (Exception e)
            {

                HlidacStatu.Util.Consts.Logger.Error("WatchDogs sending email error", e);
                return false;
            }

        }
    }
}
