using RazorEngine.Templating; // For extension methods.
using System;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HlidacStatu.Lib.Emails
{
    public class EmailMessageFromTemplate
    {
        public EmailMessageFromTemplate(string htmltemplate, string texttemplate)
        {
            this.HtmlTemplate = htmltemplate;
            this.TextTemplate = texttemplate;
        }

        public string HtmlTemplate { get; set; }
        public string TextTemplate { get; set; }

        public string To { get; set; }
        public string From { get; set; } = "podpora@HlidacStatu.cz";

        public string Subject { get; set; }

        public dynamic Model { get; set; } = new System.Dynamic.ExpandoObject();


        private string RenderView(string template)
        {
            try
            {
                var t = new HlidacStatu.Lib.Render.ScribanT(template);
                return t.Render(this.Model);
            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Error("Scriban template render", e);
                throw;
            }
        }

        public void SendEmail()
        {
            try
            {
                using (MailMessage msg = new MailMessage(this.From, this.To))
                {
                    if (!string.IsNullOrEmpty(this.TextTemplate))
                        msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(RenderView(this.TextTemplate), new System.Net.Mime.ContentType("text/plain")));
                    if (!string.IsNullOrEmpty(this.HtmlTemplate))
                        msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(RenderView(this.HtmlTemplate), new System.Net.Mime.ContentType("text/html")));

                    msg.Subject = this.Subject;
                    msg.BodyEncoding = System.Text.Encoding.UTF8;
                    msg.SubjectEncoding = System.Text.Encoding.UTF8;

                    using (SmtpClient smtp = new SmtpClient())
                    {
                        HlidacStatu.Util.Consts.Logger.Info("Sending email to " + msg.To);
                        //msg.Bcc.Add("michal@michalblaha.cz");
                        smtp.Send(msg);
                    }
                }

            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Error("Send email", e);
#if DEBUG
                throw e;
#endif
            }
        }
        public string RenderText()
        {
            try
            {
                return RenderView(this.TextTemplate);
            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Error("Render text version", e);
                throw e;
            }
        }

        public string RenderHtml()
        {
            try
            {
                return RenderView(this.HtmlTemplate);
            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Error("Render html version", e);
                throw e;
            }
        }


        public static string DefaultEmailFooterHtml = @"<p style=""font-size:18px;""><b>Podpořte provoz Hlídače</b></p>
<p align=""left"">👉 <b>kontrolujeme politiky a úředníky</b>, zda s našimi penězi zacházejí správně.
<br>👉 <b>Stali jsme se důležitým zdroje informací pro novináře</b>.
<br>👉 <b>Pomáháme státu zavádět moderní e-government</b>.
<br>👉 <b>Zvyšujeme transparentnost českého státu.</b>
</p>

<p><a href=""https://www.darujme.cz/projekt/1200384"">Podpořte nás i malým příspěvkem. Díky!</a></p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p><i>&#8608; Hlídáme je, protože si to zaslouží</i></p>";
        public static string DefaultEmailFooterText = @"

PODPOŘTE PROVOZ HLÍDAČE

👉 kontrolujeme politiky a úředníky, zda s našimi penězi zacházejí správně.
👉 Stali jsme se důležitým zdroje informací pro novináře.
👉 Pomáháme státu zavádět moderní e-government.
👉 Zvyšujeme transparentnost českého státu.


Podpořte nás i malým příspěvkem na https://www.darujme.cz/projekt/1200384. Děkujeme!


→ Hlídáme je, protože si to zaslouží
";






    }

}

