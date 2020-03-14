using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Watchdogs
{
    public partial class SingleEmailPerUserProcessor
    {
        public static class Template
        {
            static string topHeaderTemplateHtml = @"
<table style='width:100%;border:2px solid #FFC76D;font-family: Cabin, sans-serif;' cellspacing=0 cellpadding=0 border=0>
<tr><td bgcolor='#FFC76D' color='black' style='background:#FFC76D;color:black'>
<h3 style='text-align: center;padding-top: 20px;padding-bottom: 0;margin: 0;'>Hlídač nových informací</h3>
<h2 style='text-align: center;padding-top: 0;margin-top: 0;'>„{0}“</h2>
<center><p style='text-align:center'>období {1}</p></center>
</td></tr>
<tr><td bgcolor='#FFC76D' style='background:#FFC76D;color:black' height='20' style='line-height: 20px; min-height: 20px;'></td></tr>
</table>
";
            static string topHeaderTemplateText = ">>> {0}  <<<\n za období {1}\n";
            public static RenderedContent TopHeader(string value, string dateinterval)
            {
                return new RenderedContent()
                {
                    ContentHtml = string.Format(topHeaderTemplateHtml, value, dateinterval),
                    ContentText = string.Format(topHeaderTemplateText, value, dateinterval)
                    + new string('=', value.Length + 8)
                };
            }

            static string headerTemplateHtml = @"
<table style='width:100%;border:2px solid #3669AA;font-family: Cabin, sans-serif;' cellspacing=0 cellpadding=0 border=0>
<tr><td style='background:#3669AA;color:white;padding-top:20px;'>
<h4 style='text-align: center;'>Data: <b>{0} ({1})</b> </h4>
</td></tr>
<tr><td style='background:#EFF4FB;color:black;padding-top:20px;' bgcolor='#EFF4FB'>
{2}
</td></tr>
</table>
";
            static string headerTemplateText = "{0} ({1})\n{2}\n{3}";

            public static RenderedContent DataContent(long total, RenderedContent data)
            {
                string stotal = Devmasters.Core.Lang.Plural.Get((int)total, "jeden záznam", "{0} články", "{0} článků");
                return new RenderedContent()
                {
                    ContentHtml = string.Format(headerTemplateHtml, data.ContentTitle, stotal, FixTable(data).ContentHtml),
                    ContentText = string.Format(headerTemplateText, data.ContentTitle, stotal, new string('=', data.ContentText.Length + 3), data.ContentText)
                    
                };
            }

            public static RenderedContent Margin(int pixels = 20)
            {
                return new RenderedContent()
                {
                    ContentHtml = string.Format(@"
<table cellpadding='0' cellspacing='0' border='0' align='center' width='80%' style='width: 80%; min-width: 640px;'><tr>
        <td height='{0}' style='line-height: {0}px; min-height: {0}px;'></td>
    </tr></table>", pixels),
                    ContentText = new string('\n', (pixels / 20) + 1)
                };
            }


            public static RenderedContent FixTable(RenderedContent cont)
            {
                cont.ContentHtml = cont.ContentHtml.Replace("<table ", "<table cellpadding='3' style='font-family: Cabin, sans-serif;' ");
                return cont;
            }

            public static RenderedContent MailFooter()
            {
                return new RenderedContent() { 
                ContentHtml = DefaultEmailFooterHtml,
                ContentText = DefaultEmailFooterText
                };
            }

            public static string DefaultEmailFooterHtml = @"<p style=""font-size:18px;""><b>Podpořte provoz Hlídače</b></p>
<p align=""left"">👉 <b>Kontrolujeme politiky a úředníky</b>, zda s našimi penězi zacházejí správně.
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

👉 Kontrolujeme politiky a úředníky, zda s našimi penězi zacházejí správně.
👉 Stali jsme se důležitým zdroje informací pro novináře.
👉 Pomáháme státu zavádět moderní e-government.
👉 Zvyšujeme transparentnost českého státu.


Podpořte nás i malým příspěvkem na https://www.darujme.cz/projekt/1200384. Děkujeme!


→ Hlídáme je, protože si to zaslouží
";
        }
    }
}
