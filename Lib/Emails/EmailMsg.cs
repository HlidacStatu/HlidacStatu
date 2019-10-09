using RazorEngine.Templating; // For extension methods.
using System;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HlidacStatu.Lib.Emails
{
    public class EmailMsg
    {
        public EmailMsg(string htmltemplate, string texttemplate)
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

        public string RenderView(string template)
        {
            if (string.IsNullOrEmpty(template))
                return "";
            var razorEngineConfig = new RazorEngine.Configuration.TemplateServiceConfiguration();
            razorEngineConfig.DisableTempFileLocking = true;
            razorEngineConfig.EncodedStringFactory = new RazorEngine.Text.RawStringFactory();
            var engineRazor = RazorEngine.Templating.RazorEngineService.Create(razorEngineConfig); // new API

            //config.EncodedStringFactory = new RawStringFactory(); // Raw string encoding.
            //config.EncodedStringFactory = new HtmlEncodedStringFactory(); // Html encoding.
            DynamicViewBag model = new DynamicViewBag(this.Model);

            try
            {
                var result = engineRazor.RunCompile(template, Devmasters.Core.CryptoLib.Hash.ComputeHashToHex(template), null, model);

                return result;

            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Error("Razor template render", e);
                throw;
            }


        }

        public void SendMe()
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

        public string RenderHtml()
        {
            try
            {
                return RenderView(this.HtmlTemplate);
            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Error("Send email", e);
                throw e;
            }
        }


        public string EmailFooterHtml = @"<p style=""font-size:18px;""><b>Podpořte provoz Hlídače</b></p>
<p align=""left"">👉 <b>kontrolujeme politiky a úředníky</b>, zda s našimi penězi zacházejí správně.
<br>👉 <b>Stali jsme se důležitým zdroje informací pro novináře</b>.
<br>👉 <b>Pomáháme státu zavádět moderní e-government</b>.
<br>👉 <b>Zvyšujeme transparentnost českého státu.</b>
</p>

<p><a href=""https://www.darujme.cz/projekt/1200384"">Podpořte nás i malým příspěvkem. Díky!</a></p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p><i>&#8608; Hlídáme je, protože si to zaslouží</i></p>";
        public string EmailFooterText = @"

PODPOŘTE PROVOZ HLÍDAČE

👉 kontrolujeme politiky a úředníky, zda s našimi penězi zacházejí správně.
👉 Stali jsme se důležitým zdroje informací pro novináře.
👉 Pomáháme státu zavádět moderní e-government.
👉 Zvyšujeme transparentnost českého státu.


Podpořte nás i malým příspěvkem na https://www.darujme.cz/projekt/1200384. Děkujeme!


→ Hlídáme je, protože si to zaslouží
";






        public static Emails.EmailMsg CreateEmailMsgFromPostalTemplate(string templatename)
        {
            var email = new Lib.Emails.EmailMsg(GetEmailResourceString(templatename, "Html"), GetEmailResourceString(templatename, "Text"));

            email.Subject = GetValueFromPostalTemplate(GetEmailResourceString(templatename, null), "Subject");
            return email;
        }

        public static string GetValueFromPostalTemplate(string template, string key)
        {
            foreach (var line in (template ?? "").Split('\n', '\r'))
            {
                if (!string.IsNullOrEmpty(line))
                {
                    string[] kv = line.Split(':');
                    if (kv.Length == 2)
                    {
                        if (kv[0].ToLower() == key.ToLower())
                            return kv[1].Trim();
                    }
                }
            }
            return string.Empty;
        }

        public static string GetEmailResourceString(string resourceKey, string format)
        {
            return GetEmailResourceString(typeof(Lib.Emails.EmailMsg).Assembly, @"HlidacStatu.Lib.Emails.Templates", resourceKey, format);
        }

        public static string GetEmailResourceString(Assembly sourceAssembly, string resourcePath, string resourceKey, string format)
        {
            var sFormat = format;
            if (sFormat != null)
                sFormat = "." + format;
            var name = string.Format("{0}.{1}{2}", resourcePath, resourceKey, sFormat);
            if (!name.EndsWith(".cshtml"))
                name = name + ".cshtml";

            if (sourceAssembly.GetManifestResourceNames().Contains(name))
            {
                using (var stream = sourceAssembly.GetManifestResourceStream(name))
                {
                    using (var reader = new System.IO.StreamReader(stream))
                    {
                        var content = reader.ReadToEnd();
                        if (!string.IsNullOrEmpty(format))
                            return RemoveFirstLines(content, 1);
                        else
                            return content;
                    }
                }
            }
            else
                return null;
        }

        public static string RemoveFirstLines(string text, int linesCount)
        {
            var lines = Regex.Split(text, "\r\n|\r|\n").Skip(linesCount);
            return string.Join(Environment.NewLine, lines.ToArray());
        }
    }

}

