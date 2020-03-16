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

            public static string DefaultEmailFooterHtml = @"
<p style='font-size:18px;'><b>Podpořte provoz Hlídače</b></p>
<p>👉 <b>Kontrolujeme politiky a úředníky</b>, zda s našimi penězi zacházejí správně.
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
            public static string EmailBodyTemplateText = @"
#BODY#

#FOOTERMSG#
";
            public static string EmailBodyTemplateHtml = @"<!DOCTYPE html PUBLIC '-//W3C//DTD HTML 3.2//EN'>
<html>

<head>
    <!--[if gte mso 9]>
      <xml>
        <o:OfficeDocumentsettings>
          <o:AllowPNG/>
          <o:PixelsPerInch>96</o:PixelsPerInch>
        </o:OfficeDocumentsettings>
      </xml>
    <![endif]-->
    <meta http-equiv='Content-Type' content='text/html; charset=utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <meta http-equiv='X-UA-Compatible' content='IE=edge'>
    <meta name='format-detection' content='address=no'>
    <meta name='format-detection' content='telephone=no'>
    <meta name='format-detection' content='email=no'>
    <meta name='x-apple-disable-message-reformatting'>
    <!--[if !mso]>
      <!-->
    <style type='text/css'>
    @import url('https://fonts.googleapis.com/css?family=Lato:400,400i,700,700i%7CMerriweather:400,400i,700,700i%7CMontserrat:400,400i,700,700i%7CMontserrat+Alternates:400,400i,700,700i%7COpen+Sans:400,400i,700,700i%7CPT+Sans:400,400i,700,700i%7CRaleway:400,400i,700,700i%7CRoboto:400,400i,700,700i%7CSource+Sans+Pro:400,400i,700,700i%7CRoboto+Slab:400,700%7CUbuntu:400,400i,700,700i%7CTitillium+Web:400,400i,700,700i%7CNunito:400,400i,700,700i%7CCabin:400,400i,700,700i%7CExo:400,400i,700,700i%7CComfortaa:400,700%7CRaleway:400,400i,700,700i%7COxygen:400,700i%7CPoppins:400,700%7CPlayfair+Display:400,400i,700,700i');
    </style>
    <!--<![endif]-->
    <!-- RSS STYLE STARTS -->
    <!--[if mso]>
      <style type='text/css'>
        .content-MS .content img { width: 560px; }
      </style>
    <![endif]-->
    <!-- WINDOWS 10 HACKS FOR LINK AND BG COLOR -->
    <!--[if (mso)|(mso 16)]>
      <style type='text/css'>
        .mlContentButton a { text-decoration: none; }
      </style>
    <![endif]-->
    <!--[if !mso]>
      <!-- -->
    <style type='text/css'>
    .mlBodyBackgroundImage {
        background-image: url('');
    }
    </style>
    <!--<![endif]-->
    <!--[if (lt mso 16)]>
      <style type='text/css'>
        .mlBodyBackgroundImage { background-image: url(''); }
      </style>
    <![endif]-->
    <style type='text/css'>
    @media only screen and (max-width: 480px) {
        .social-LinksTable {
            width: 100% !important;
        }

        /* -- */
        .social-LinksTable td:first-child {
            padding-left: 0px !important;
        }

        /* -- */
        .social-LinksTable td:last-child {
            padding-right: 0px !important;
        }

        /* -- */
        .social-LinksTable td {
            padding: 0 10px !important;
        }

        /* -- */
        .social-LinksTable td img {
            height: auto !important;
            max-width: 48px;
            width: 100% !important;
        }

        /* -- */
    }
    </style>
    <!--[if mso]>
      <style type='text/css'>
        .bodyText { font-family: Arial, Helvetica, sans-serif!important ; }   .bodyText * { font-family: Arial, Helvetica, sans-serif!important; }    .bodyText a { font-family: Arial, Helvetica, sans-serif!important; }    .bodyText a span { font-family: Arial, Helvetica, sans-serif!important; }   .bodyText span { font-family: Arial, Helvetica, sans-serif!important; }   .bodyText p { font-family: Arial, Helvetica, sans-serif!important; }    .bodyText ul li { font-family: Arial, Helvetica, sans-serif!important; }    .bodyTitle { font-family: Arial, Helvetica, sans-serif!important ; }    .bodyTitle * { font-family: Arial, Helvetica, sans-serif!important; }   .bodyTitle a { font-family: Arial, Helvetica, sans-serif!important; }   .bodyTitle a span { font-family: Arial, Helvetica, sans-serif!important; }    .bodyTitle span { font-family: Arial, Helvetica, sans-serif!important; }    .bodyTitle p { font-family: Arial, Helvetica, sans-serif!important; }   .bodyFont { font-family: Arial, Helvetica,
sans-serif!important ; }    .bodyFont * { font-family: Arial, Helvetica, sans-serif!important; }    .bodyFont a { font-family: Arial, Helvetica, sans-serif!important; }    .bodyFont a span { font-family: Arial, Helvetica, sans-serif!important; }   .bodyFont span { font-family: Arial, Helvetica, sans-serif!important; }   .bodyFont p { font-family: Arial, Helvetica, sans-serif!important; }    .mlContentButton { font-family: Arial, Helvetica, sans-serif!important; }
      </style>
    <![endif]-->
    <style type='text/css'>
    @media only screen and (max-width: 640px) {
        #logoBlock-4 {
            max-width: 300px !important;
            width: 100% !important;
        }
    }
    </style>
    <title></title>
    <meta name='robots' content='noindex, nofollow'>
</head>

<body class='mlBodyBackground' style='padding: 0; margin: 0; -webkit-font-smoothing:antialiased; background-color:#ffffff; -webkit-text-size-adjust:none;'>
    <style>
    div.OutlookMessageHeader,
    table.moz-email-headers-table,
    blockquote #_t,
    </style>
    <!--[if !mso]>
      <!-- -->
    <table width='100%' border='0' cellspacing='0' cellpadding='0' bgcolor='#ffffff' class='mainTable mlBodyBackground' dir='ltr' background=''>
        <tr>
            <td class='mlTemplateContainer' align='center'>
                <!--<![endif]-->
                <!--[if mso 16]>
            <table width='100%' border='0' cellspacing='0' cellpadding='0' align='center'>
              <tr>
                <td bgcolor='#ffffff' align='center'>
                <!--<![endif]-->
                <!-- Content starts here -->
                <table align='center' border='0' cellpadding='0' cellspacing='0' class='mlContentTable' width='100%'>
                    <tr>
                        <td>
                            <!--  -->
                            <table align='center' border='0' bgcolor='' class='mlContentTable mlContentTableDefault' cellpadding='0' cellspacing='0' width='100%'>
                                <tr>
                                    <td class='mlContentTableCardTd'>
                                        <table align='center' bgcolor='' border='0' cellpadding='0' cellspacing='0' class='mlContentTable ml-fullwidth' style='width: 80%; min-width: 640px;' width='80%'>
                                            <tr>
                                                <td>
                                                    <table cellpadding='0' cellspacing='0' border='0' align='center' width='640' style='width: 640px; min-width: 640px;' class='mlContentTable'>
                                                        <tr>
                                                            <td height='20' class='spacingHeight-20' style='line-height: 20px; min-height: 20px;'></td>
                                                        </tr>
                                                    </table>
                                                    <table cellpadding='0' cellspacing='0' border='0' align='center' width='640' style='width: 640px; min-width: 640px;' class='mlContentTable'>
                                                        <tr>
                                                            <td align='center' style='padding: 0px 40px;' class='mlContentOuter'>
                                                                <table cellpadding='0' cellspacing='0' border='0' align='center' width='100%'>
                                                                    <tr>
                                                                        <td align='center'>
                                                                            <img src='https://www.hlidacstatu.cz/content/img/Logo666.png' id='logoBlock-4' border='0' title='Logo Hlidac Statu' alt='Logo Hlidac Statu' width='333' height='auto' style='display: block;'>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table cellpadding='0' cellspacing='0' border='0' align='center' width='640' style='width: 640px; min-width: 640px;' class='mlContentTable'>
                                                        <tr>
                                                            <td height='20' class='spacingHeight-20' style='line-height: 20px; min-height: 20px;'></td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!--  -->
                            <!--  -->
                            <table align='center' border='0' bgcolor='#ffffff' class='mlContentTable mlContentTableDefault' cellpadding='0' cellspacing='0' width='100%'>
                                <tr>
                                    <td class='mlContentTableCardTd'>
                                        <table align='center' bgcolor='#ffffff' border='0' cellpadding='0' cellspacing='0' class='mlContentTable ml-fullwidth' style='width: 80%; min-width: 640px;' width='80%'>
                                            <tr>
                                                <td>
                                                    <selector>
                                                        <table cellpadding='0' cellspacing='0' border='0' align='center' width='80%' style='width: 80%; min-width: 640px;' class='mlContentTable'>
                                                            <tr>
                                                                <td height='20' class='spacingHeight-20' style='line-height: 20px; min-height: 20px;'></td>
                                                            </tr>
                                                        </table>
                                                        <table cellpadding='0' cellspacing='0' border='0' align='center' width='95%' style='width: 95%; min-width: 640px;' class='mlContentTable'>
                                                            <tr>
                                                                <td class='bodyTitle' id='bodyText-6' style='font-family: Georgia, serif; font-size: 16px; line-height: 26px; color: #111111;'>

                                                                <table cellpadding='0' cellspacing='0' border='0' align='center' width='80%' style='width: 80%; min-width: 640px;' class='mlContentTable'>
                                                                    <tr>
                                                                        <td>
                                                                          <p style='font-family: Georgia, serif; font-size: 16px; line-height: 26px; color: #111111;'>
                                                                            <b style='color:red'>NOVINKA!</b> Pokud máte nastaveno více hlídačů nových informací na Hlídači státu, nebudeme vám je posílat v jednotlivých mailech, ale vše najednou v jednom. Napište nám, jak se vám to líbí, co bychom měli změnit a vylepšit.
                                                                          </p>
                                                                        </td>
                                                                    </tr>
                                                                </table>

                                                                <table cellpadding='0' cellspacing='0' border='0' align='center' width='80%' style='width: 80%; min-width: 640px;' class='mlContentTable'>
                                                                    <tr>
                                                                        <td height='20' class='spacingHeight-20' style='line-height: 20px; min-height: 20px;'></td>
                                                                    </tr>
                                                                </table>

                                                                #BODY#


                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <table cellpadding='0' cellspacing='0' border='0' align='center' width='640' style='width: 640px; min-width: 640px;' class='mlContentTable'>
                                                            <tr>
                                                                <td height='40' class='spacingHeight-40' style='line-height: 40px; min-height: 40px;'></td>
                                                            </tr>
                                                        </table>
                                                    </selector>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!--  -->
                            <table align='center' border='0' bgcolor='#f9f9f9' class='mlContentTable mlContentTableFooterDefault' cellpadding='0' cellspacing='0' width='100%'>
                                <tr>
                                    <td class='mlContentTableFooterCardTd'>
                                        <table align='center' bgcolor='#f9f9f9' border='0' cellpadding='0' cellspacing='0' class='mlContentTable ml-fullwidth' style='width: 70%; min-width: 640px;' width='70%'>
                                            <tr>
                                                <td>
                                                    <selector>
                                                        <table cellpadding='0' cellspacing='0' border='0' align='center' width='100%' style='width: 100%; min-width: 640px;' class='mlContentTable'>
                                                            <tr>
                                                                <td align='center' style='padding: 0px 150px;' class='mlContentOuter'>
                                                                    <table cellpadding='0' cellspacing='0' border='0' align='center' width='100%'>
                                                                        <tr>
                                                                            <td height='10'></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align='center' class='bodyTitle' style='font-family: Georgia, serif; font-size: 12px; line-height: 18px; color: #000000;'>
                                                                                <a href='https://www.hlidacstatu.cz/manage/Watchdogs'>Chcete změnit nastavení tohoto hlídače nových smluv?</a>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td height='25' class='spacingHeight-20'></td>
                                                            </tr>
                                                            <tr>
                                                                <td align='center' class='bodyTitle' id='footerText-8' style='font-family: Georgia, serif; font-size: 12px; line-height: 18px; color: #000;'>
                                                                    <p style='margin-top: 0px; margin-bottom: 0px;text-align:center'>
                                                                        #FOOTERMSG#
                                                                        <br></p>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <table cellpadding='0' cellspacing='0' border='0' align='center' width='640' style='width: 640px; min-width: 640px;' class='mlContentTable'>
                                                            <tr>
                                                                <td height='40' class='spacingHeight-40' style='line-height: 40px; min-height: 40px;'></td>
                                                            </tr>
                                                        </table>
                                                    </selector>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <!-- Content ends here -->
                <!--[if mso 16]>
                </td>
              </tr>
            </table>
          <!--<![endif]-->
                <!--[if !mso]>
            <!-- -->
            </td>
        </tr>
    </table>
    <!--<![endif]-->
</body>

</html>
";


        }
    }
}
