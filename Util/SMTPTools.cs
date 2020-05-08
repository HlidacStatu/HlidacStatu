using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Util
{
    public static class SMTPTools
    {


        public static bool SendEmail(string subject, 
            string htmlContent, string textContent,
            string toEmail,
            string fromEmail = "podpora@hlidacstatu.cz", bool bccToAdmin = false)
        {
            try
            {

                using (MailMessage msg = new MailMessage(fromEmail, toEmail))
                {
                    if (!string.IsNullOrEmpty(textContent))
                    {
                        var view = AlternateView.CreateAlternateViewFromString(textContent, new System.Net.Mime.ContentType("text/plain"));
                        view.ContentType.CharSet = Encoding.UTF8.WebName;
                        msg.AlternateViews.Add(view);
                    }
                    if (!string.IsNullOrEmpty(htmlContent))
                    {
                        var view = AlternateView.CreateAlternateViewFromString(htmlContent, new System.Net.Mime.ContentType("text/html"));
                        view.ContentType.CharSet = Encoding.UTF8.WebName;
                        msg.AlternateViews.Add(view);
                    }


                    msg.Subject = subject;
                    msg.BodyEncoding = System.Text.Encoding.UTF8;
                    msg.SubjectEncoding = System.Text.Encoding.UTF8;

                    using (SmtpClient smtp = new SmtpClient())
                    {
                        HlidacStatu.Util.Consts.Logger.Info("Sending email to " + msg.To);
                        if (bccToAdmin) 
                            msg.Bcc.Add("michal@michalblaha.cz");
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

        public static void SendSimpleMailToPodpora(string subject, string body, string replyTo )
        {
            string from = "podpora@hlidacstatu.cz";
            string to = "podpora@hlidacstatu.cz";

            using (var smtp = new System.Net.Mail.SmtpClient())
            {
                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage(from, to);
                msg.Bcc.Add("michal@michalblaha.cz");
                msg.Subject = subject;
                if (!string.IsNullOrEmpty(replyTo) && Devmasters.Core.TextUtil.IsValidEmail(replyTo))
                    msg.ReplyToList.Add(new System.Net.Mail.MailAddress(replyTo));
                msg.IsBodyHtml = false;
                msg.BodyEncoding = System.Text.Encoding.UTF8;
                msg.SubjectEncoding = System.Text.Encoding.UTF8;
                msg.Body = body;

                smtp.Send(msg);
            }
        }
    }
}
