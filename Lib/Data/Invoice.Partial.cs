using Devmasters;
using Devmasters.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;


namespace HlidacStatu.Lib.Data
{
    public partial class Invoices
    {
        public enum InvoiceStatus
        {
            New = 1,
            Finished = 3,
            Cancelled = 4
        }

        public enum PaymentMethod
        {
            Invoice = 1,
            CreditCard = 2
        }

        public void UpdateTotalPrice()
        {
            if (this.InvoiceItems == null)
                this.TotalPriceNoVat = 0;
            else
            {
                this.TotalPriceNoVat = this.InvoiceItems.Sum(m => m.FinalPriceWithVat());
            }
        }

        public byte[] PrintToPDF()
        {
            //string url = string.Format(Devmasters.Config.GetWebConfigValue("SiteURL") + "api/InvoicePrint.ashx?id={0}&h={1}",
            //    this.ID, System.Web.HttpUtility.UrlEncode(this.GetInvoiceHash()));
            //url = url.Trim();
            //return Feedback.Lib.PDF.Net.PDFClient.GetURLinPDF(url);

            //SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
            //SelectPdf.PdfDocument doc = converter.ConvertHtmlString(Print2Html());
            //using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            //{

            //    doc.Save(ms);
            //    doc.Close();
            //    return ms.ToArray();
            //}
            throw new NotImplementedException();
        }
        private List<InvoiceItems> items = null;
        public List<InvoiceItems> GetItems(Lib.Data.DbEntities fc)
        {
            if (this.items != null)
                return this.items;

            bool dispose = fc == null;
            if (dispose)
                fc = new Lib.Data.DbEntities();

			try
			{
				this.items = fc.InvoiceItems.Where(m => m.ID_Invoice == this.ID).ToList();
			}
			finally
			{
				if (dispose)
				{
					fc?.Dispose();
				}
			}

            return this.items;
        }





        public string Print2Html()
        {

            System.Collections.Specialized.StringDictionary data = new System.Collections.Specialized.StringDictionary();

            data.Add("COMPANYNAME", this.Company);
            data.Add("COMPANYCONTACTNAME", this.FirstName + " " + this.LastName);
            data.Add("COMPANYADDRESS", this.Address);

            data.Add("COMPANYCITY", this.City);
            data.Add("COMPANYICO", this.CompanyID);
            data.Add("COMPANYVAT", this.VatID);
            data.Add("INVOICENUMBER", this.InvoiceNumber);
            data.Add("DATECREATED", this.Created.ToLongDateString());
            data.Add("DUEDATE", this.Created.AddDays(7).ToLongDateString());
            data.Add("SUMPRICEVAT", this.GetItems(null).Sum(m => m.FinalPriceWithVat()).ToString("0.00 Kč"));
            data.Add("PRICESUM", this.GetItems(null).Sum(m => m.FinalPriceWithoutVat()).ToString("0.00 Kč"));
            data.Add("SUMVAT", this.GetItems(null).Sum(m => (m.FinalPriceWithVat() - m.FinalPriceWithoutVat())).ToString("0.00 Kč"));

            //items
            System.Text.StringBuilder sbItems = new System.Text.StringBuilder(1024);
            foreach (var item in this.GetItems(null))
            {
                System.Collections.Specialized.StringDictionary dataItem = new System.Collections.Specialized.StringDictionary();
                dataItem.Add("TEXT", item.Name);
                dataItem.Add("AMOUNT", item.Amount.ToString());
                dataItem.Add("PRICE", item.Price.ToString("0.00 Kč"));
                dataItem.Add("DISCOUNT", item.Discount.ToString("0.00 Kč"));
                dataItem.Add("TOTALPRICE", item.FinalPriceWithoutVat().ToString("0.00 Kč"));
                dataItem.Add("VAT", item.VAT.ToString());
                dataItem.Add("TOTALVAT", (item.FinalPriceWithVat() - item.FinalPriceWithoutVat()).ToString("0.00 Kč"));
                dataItem.Add("TOTALPRICEVAT", item.FinalPriceWithVat().ToString("0.00 Kč"));
                sbItems.AppendLine(
                    new Devmasters.Render.Template.SimpleTemplate(invoiceItemTemplate, dataItem).Render()
                    );
            }

            data.Add("ITEMS", sbItems.ToString());

            Devmasters.Render.Template.SimpleTemplate render = new Devmasters.Render.Template.SimpleTemplate(invoiceTemplate, data);
            return render.Render();

        }

        string invoiceTemplate = @"
<html>
<HEAD>
<META HTTP-EQUIV=""Content-Type"" content=""text/html; charset=windows-1250"">
<TITLE>Faktura</TITLE>
</HEAD>

<BODY BGCOLOR=ffffff>

<FONT FACE=""Arial"">

<TABLE WIDTH=100% BORDER=0 CELLPADDING=0 >
<TR>
<TD WIDTH=46% Align=Left><B><FONT SIZE=3 COLOR=000080>Hlídač státu, z.ú.</B></FONT></TD>
<!TD WIDTH=29% Align=Center><!/TD>	
<TD WIDTH=8%></TD>
<TD WIDTH=46% Align=Right><B><FONT SIZE=3 COLOR=000080>FAKTURA č. #INVOICENUMBER#</B></FONT></TD>
</TR>
</TABLE>

<! --- Hlavní tabulka --- >
<TABLE WIDTH=100% BORDER=1 CELLPADDING=0 background=""Gray.jpg"">
<TR>
<TD>

<! --- Dodavatel / Odběratel --- >
<TABLE WIDTH=100% BORDER=0 CELLSPACING=0 CELLPADDING=0 >
<TR>
<TD WIDTH=5%></TD>

<TD WIDTH=50%><FONT SIZE=1 COLOR=000080><BR>Dodavatel:</FONT></TD>
<TD WIDTH=3%></TD>
<TD WIDTH=22%><FONT SIZE=1 COLOR=000080><BR>Odběratel:</FONT></TD>


<TD WIDTH=20%><FONT SIZE=1 COLOR=000080><BR>

</FONT></TD>
</TR>
</TABLE>

<TABLE WIDTH=100% BORDER=0 CELLPADDING=0 >
<TR>
<TD WIDTH=5%></TD>

<TD WIDTH=50%><B><FONT SIZE=2>Hlídac Státu z.ú.</B></FONT></TD>
<TD WIDTH=3%></TD>
<TD WIDTH=22%><B><FONT SIZE=2>#COMPANYNAME#</B></FONT></TD>


<TD WIDTH=20%><B><FONT SIZE=2>

</B></FONT></TD>
</TR>

<TR>
<TD></TD>
<TD><B><FONT SIZE=2></B></FONT></TD>

<TD></TD>
<TD><B><FONT SIZE=2></B></FONT></TD>
<TD><B><FONT SIZE=2>

</B></FONT></TD>
</TR>

<TR>
<TD></TD>
<TD><B><FONT SIZE=2>Velenovského  648</B></FONT></TD>

<TD></TD>
<TD><B><FONT SIZE=2>#COMPANYCONTACTNAME#</B></FONT></TD>
<TD><B><FONT SIZE=2>

</B></FONT></TD>
</TR>

<TR>
<TD></TD>
<TD><B><FONT SIZE=2>251 64 Mnichovice</B></FONT></TD>

<TD></TD>
<TD><B><FONT SIZE=2>#COMPANYADDRESS#</B></FONT></TD>
<TD><B><FONT SIZE=2>

</B></FONT></TD>
</TR>

<TR>
<TD></TD>
<TD><FONT SIZE=2>Telefon: </FONT></TD>

<TD></TD>
<TD><B><FONT SIZE=2>#COMPANYCITY#</B></FONT></TD>
<TD><B><FONT SIZE=2>

</B></FONT></TD>
</TR>

<TR>
<TD></TD>

<TD><FONT SIZE=2>Fax: </FONT></TD>


<TD></TD>
<TD><FONT SIZE=2></FONT></TD>
<TD><FONT SIZE=2></FONT></TD>
</TR>

<TR>
<TD></TD>

<TD><FONT SIZE=2>e-mail: <A HREF=""mailto:""></A>&nbsp</FONT></TD>


<TD></TD>
<TD><FONT SIZE=2></FONT></TD>
<TD><FONT SIZE=2></FONT></TD>
</TR>

<TR>
<TD></TD>

<TD><FONT SIZE=2><A HREF=""http://""></A>&nbsp</FONT></TD>


<TD></TD>
<TD><FONT SIZE=2></FONT></TD>
<TD><FONT SIZE=2></FONT></TD>
</TR>

<TR>
<TD></TD>

<TD><FONT SIZE=2>IČ: 05965527</FONT></TD>


<TD></TD>
<TD><FONT SIZE=2>IČ: #COMPANYICO#</FONT></TD>
<TD><FONT SIZE=2></FONT></TD>
</TR>

<TR>
<TD></TD>

<TD><FONT SIZE=2>DIČ: </FONT></TD>


<TD></TD>
<TD><FONT SIZE=2>DIČ: #COMPANYVAT#</FONT></TD>
<TD><FONT SIZE=2></FONT></TD>
</TR>



<TR>
<TD></TD>

<TD><FONT SIZE=2>Číslo účtu: 2701199023 / 2010<BR>&nbsp</FONT></TD>


<TD></TD>


<TD><FONT SIZE=1>&nbsp<BR>&nbsp</FONT></TD>

<TD><FONT SIZE=2></FONT></TD>
</TR>

</TABLE>


<! --- Informace o faktuře --- >
<TABLE WIDTH=100% BORDER=0 CELLSPACING=0 CELLPADDING=0 >
<TR>
<TD WIDTH=5%></TD>
<TD WIDTH=31%><FONT SIZE=2>Faktura č.:</FONT></TD>
<TD WIDTH=22%><B><FONT SIZE=2>#INVOICENUMBER#</B></FONT></TD>
<TD WIDTH=23%></TD>
<TD WIDTH=19%></TD>
</TR>
<TR>
<TD></TD>
<TD><FONT SIZE=2>Forma úhrady:</FONT></TD>
<TD><FONT SIZE=2>příkazem</FONT></TD>
<TD><FONT SIZE=2>Variabilní symbol:</FONT></TD>
<TD><FONT SIZE=2>#INVOICENUMBER#</FONT></TD>
</TR>
<TR>
<TD></TD>
<TD><FONT SIZE=2>Datum vystavení:</FONT></TD>
<TD><FONT SIZE=2>#DATECREATED#</FONT></TD>
<TD><FONT SIZE=2>Konstantní symbol:</FONT></TD>
<TD><FONT SIZE=2>0308</FONT></TD>
</TR>
<TR>
<TD></TD>
<TD><FONT SIZE=2>Datum splatnosti:</FONT></TD>
<TD><FONT SIZE=2>#DUEDATE#</FONT></TD>
<TD><FONT SIZE=2>Objednávka č.:</FONT></TD>
<TD><FONT SIZE=2></FONT></TD>
</TR>
<TR>
<TD></TD>

<TD><FONT SIZE=2>Datum uskutečnění plnění:</FONT></TD>
<TD><FONT SIZE=2>#DATECREATED#</FONT></TD>


<TD><FONT SIZE=2>Datum objednávky:</FONT></TD>
<TD><FONT SIZE=2>#DATECREATED#</FONT></TD>
<TR>
<TD></TD>
<TD><FONT SIZE=1>&nbsp</FONT></TD>
<TD><FONT SIZE=1>&nbsp</FONT></TD>
<TD><FONT SIZE=1>&nbsp</FONT></TD>
<TD><FONT SIZE=1>&nbsp</FONT></TD>
</TR>
</TABLE>

<! --- Čára --- >
<TABLE WIDTH=100% BORDER=0 CELLSPACING=0 CELLPADDING=0 >
<TR>
<TD WIDTH=100% Align=Center><HR Size=1 NoShade></TD>
</TR>
</TABLE>

<! --- Nadpis položek --- >
<TABLE WIDTH=100% BORDER=0 CELLPADDING=0 >

<TR>
<TD WIDTH=5%></TD>
<TD WIDTH=25%><B><FONT SIZE=1>Označení dodávky</B></FONT></TD>
<TD WIDTH=1%></TD>
<TD WIDTH=10% Align=Right><B><FONT SIZE=1>Množství</B></FONT></TD>
<TD WIDTH=1%></TD>
<TD WIDTH=3%><B><FONT SIZE=1>MJ</B></FONT></TD>
<TD WIDTH=10% Align=Right><FONT SIZE=1>J.cena</FONT></TD>
<TD WIDTH=5% Align=Right><FONT SIZE=1>Sleva</FONT></TD>
<TD WIDTH=10% Align=Right><FONT SIZE=1>Cena</FONT></TD>
<TD WIDTH=5% Align=Right><FONT SIZE=1>%DPH</FONT></TD>
<TD WIDTH=10% Align=Right><FONT SIZE=1>DPH</FONT></TD>
<TD WIDTH=10% Align=Right><FONT SIZE=1>Kč Celkem</FONT></TD>
<TD WIDTH=5%></TD>
</TR>

</TABLE>

<TABLE WIDTH=100% BORDER=0 CELLSPACING=0 CELLPADDING=0 >

<TR>
<TD WIDTH=100% Align=Center><HR Size=1 NoShade></TD>
</TR>

</TABLE>

<! --- Text faktury --- >
<TABLE WIDTH=100% BORDER=0 CELLSPACING=0 CELLPADDING=0 >

<TR>
<TD WIDTH=5%></TD>
<TD WIDTH=95%><FONT SIZE=2 COLOR=000080>Fakturujeme Vám:</FONT></TD>
</TR>

<TR>
<TD></TD>
<TD><FONT SIZE=1><BR></FONT></TD>
</TR>

</TABLE>

<! --- Položky --- >
#ITEMS#

<! --- Čára --- >
<TABLE WIDTH=100% BORDER=0 CELLSPACING=0 CELLPADDING=0 >
<TR>
<TD WIDTH=100% Align=Center><HR Size=1 NoShade></TD>
</TR>
</TABLE>

<! --- Součty --- >
<TABLE WIDTH=100% BORDER=0 CELLPADDING=0 >

</TABLE>

<TABLE WIDTH=100% BORDER=0 CELLPADDING=0 >
<TR>
<TD WIDTH=5%></TD>
<TD WIDTH=25%><FONT SIZE=2>CELKEM</FONT></TD>
<TD WIDTH=1%></TD>
<TD WIDTH=10% Align=Right></TD>
<TD WIDTH=1%></TD>
<TD WIDTH=3%></TD>
<TD WIDTH=10% Align=Right></TD>
<TD WIDTH=5% Align=Right></TD>
<TD WIDTH=10% Align=Right></TD>
<TD WIDTH=5% Align=Right></TD>
<TD WIDTH=10% Align=Right></TD>
<TD WIDTH=10% Align=Right><FONT SIZE=2>#SUMPRICEVAT#</FONT></TD>
<TD WIDTH=5%></TD>
</TR>
</TABLE>



<! --- Čára --- >
<TABLE WIDTH=100% BORDER=0 CELLSPACING=0 CELLPADDING=0 >
<TR>
<TD WIDTH=100% Align=Center><HR Size=1 NoShade></TD>
</TR>
</TABLE>

<! --- Poznámka faktury a Vystavil --- >
<TABLE WIDTH=100% BORDER=0 CELLSPACING=0 CELLPADDING=0 >


<TR>
<TD WIDTH=""5%""></TD>
<TD WIDTH=""95%""><FONT SIZE=""2"" COLOR=""#000080""></FONT></TD>
</TR>


<TR>
<TD WIDTH=5%></TD>
<TD WIDTH=95%><FONT SIZE=2><BR></FONT></TD>
</TR>
<TR>
<TD></TD>
<TD>
<TABLE BORDER=0 CELLSPACING=0 CELLPADDING=0 >
<TR>
<TD><FONT SIZE=2>Vystaveno strojově</FONT></TD>
<TD><FONT SIZE=2></FONT></TD>
</TR>
<TR>
<TD></TD>
<TD><FONT SIZE=2><A HREF=""mailto:""></A>&nbsp</FONT></TD>
</TR>
</TABLE>
</TD>
</TR>
<TR>
<TD></TD>
<TD><FONT SIZE=2><BR><BR><BR></FONT></TD>
</TR>
</TABLE>

<TABLE WIDTH=100% BORDER=0 CELLSPACING=0 CELLPADDING=0 >
<TR>
<TD WIDTH=5%></TD>
<TD WIDTH=95%><FONT SIZE=1>Ústav vedený u Městského soudu v Praze, spisová značka U 559.</FONT></TD>
</TR>
</TABLE>

<TABLE WIDTH=100% BORDER=0 CELLSPACING=0 CELLPADDING=0 >
<TR>
<TD WIDTH=5%></TD>
<TD WIDTH=95%><FONT SIZE=1></FONT></TD>
</TR>
</TABLE>


<! --- Čára --- >
<TABLE WIDTH=100% BORDER=0 CELLSPACING=0 CELLPADDING=0 >
<TR>
<TD WIDTH=100% Align=Center><HR Size=1 NoShade></TD>
</TR>
</TABLE>

<! --- Rekapitulace --- >
<TABLE WIDTH=100% BORDER=0 CELLSPACING=0 CELLPADDING=0 >
<TR>
<TD WIDTH=5%></TD>
<TD WIDTH=25%><FONT SIZE=1>Rekapitulace v Kč:</FONT></TD>
<TD WIDTH=1%></TD>
<TD WIDTH=10%></TD>
<TD WIDTH=1%></TD>
<TD WIDTH=3%></TD>
<TD WIDTH=15%></TD>
<TD WIDTH=10% Align=Right><FONT SIZE=1>Základ v Kč</FONT></TD>
<TD WIDTH=5%  Align=Right><FONT SIZE=1>Sazba</FONT></TD>
<TD WIDTH=10% Align=Right><FONT SIZE=1>DPH v Kč</FONT></TD>
<TD WIDTH=10% Align=Right><FONT SIZE=1>s DPH v Kč</FONT></TD>
<TD WIDTH=5%></TD>
</TR>

<TR>
<TD></TD>
<TD></TD>
<TD></TD>
<TD></TD>
<TD></TD>
<TD></TD>
<TD></TD>
<TD Align=Right><FONT SIZE=1>0,00<BR>0,00<BR>#PRICESUM#<BR> </FONT></TD>
<TD Align=Right><FONT SIZE=1>0%<BR>14%<BR>21%<BR> </FONT></TD>
<TD Align=Right><FONT SIZE=1><BR>0,00<BR>#SUMVAT#<BR> </FONT></TD>
<TD Align=Right><FONT SIZE=1><BR>0,00<BR>#SUMPRICEVAT#<BR> </FONT></TD>
<TD></TD>
</TR>

</TABLE>

<! --- Čára --- >
<TABLE WIDTH=100% BORDER=0 CELLSPACING=0 CELLPADDING=0 >
<TR>
<TD WIDTH=100% Align=Center><HR Size=1 NoShade></TD>
</TR>
</TABLE>


</TD>
</TR>
</TABLE>

</FONT>


</BODY>
</HTML>

";

        string invoiceItemTemplate = @"
<TABLE WIDTH=100% BORDER=0 CELLPADDING=0 >
<TR>
<TD WIDTH=5%></TD>
<TD WIDTH=25%><FONT SIZE=1>#TEXT#</FONT></TD>
<TD WIDTH=1%></TD>
<TD WIDTH=10% Align=Right><FONT SIZE=1>#AMOUNT#</FONT></TD>
<TD WIDTH=1%></TD>
<TD WIDTH=3%><FONT SIZE=1></FONT></TD>
<TD WIDTH=10% Align=Right><FONT SIZE=1>#PRICE#</FONT></TD>
<TD WIDTH=5% Align=Right><FONT SIZE=1>#DISCOUNT#</FONT></TD>
<TD WIDTH=10% Align=Right><FONT SIZE=1>#TOTALPRICE#</FONT></TD>
<TD WIDTH=5% Align=Right><FONT SIZE=1>#VAT#</FONT></TD>
<TD WIDTH=10% Align=Right><FONT SIZE=1>#TOTALVAT#</FONT></TD>
<TD WIDTH=10% Align=Right><FONT SIZE=1>#TOTALPRICEVAT#</FONT></TD>
<TD WIDTH=5%></TD>
</TR>
</TABLE>
";



        public static InvoiceItems.ShopItem? GetCurrentOrderForUser(string userId)
        {
            DateTime now = DateTime.Now.Date;
            DateTime now_30 = DateTime.Now.Date.AddDays(-30);
            using (DbEntities db = new DbEntities())
            {
                var items = db.Invoices
                        .Where(m => m.ID_Customer == userId && (m.Status == 3 || m.Created > now_30))
                        .Join(db.InvoiceItems.Where(ii => ii.Expires > now),
                        inv => inv.ID,
                        ii => ii.ID_Invoice, (inv, ii) => ii)
                        .OrderByDescending(ii => ii.ID_ShopItem);

                if (items.Count() > 0)
                    return (InvoiceItems.ShopItem)items.First().ID_ShopItem;
                else
                    return null;
            }
        }

        public static int GetMaxNumberOfWatchdogs(string userId)
        {
            var order = Invoices.GetCurrentOrderForUser(userId);

            switch (order)
            {
                case Data.InvoiceItems.ShopItem.NGO:
                    return int.MaxValue;
                case Data.InvoiceItems.ShopItem.Zakladni:
                    return 10;
                case Data.InvoiceItems.ShopItem.Kompletni:
                    return int.MaxValue;
                default:
                    return 3;
            }
        }

        static DateTime limit = new DateTime(2017, 8, 22);
        public static int GetAvailableWatchdogs(string userId)
        {
            return int.MaxValue;

            var max = Invoices.GetMaxNumberOfWatchdogs(userId);
            if (max == int.MaxValue)
                return int.MaxValue;

            if (DateTime.Now < limit)
                return int.MaxValue;

            //var userCreated = DateTime.Now.Date.AddDays(-40);
            //var usert = new Lib.Data.DbEntities().AspNetUserTokens.Where(m => m.Id == userId).FirstOrDefault();
            //if (usert != null)
            //    userCreated = usert.Created.AddDays(30);

            var curr = new Lib.Data.DbEntities().WatchDogs.Count(m => m.UserId == userId && m.StatusId > 0);

            return max - curr;
        }


        public static Invoices CreateNew(
            Lib.Data.InvoiceItems.ShopItem sluzba,
            string adresa,
            string mesto,
            string jmenoFirmy,
            string ICO,
            string jmenoOsoby,
            string DIC,
            string PSC,
            string userId,
            string username,
            bool sendEmail = true
            )
        {
            using (Lib.Data.DbEntities db = new DbEntities())
            {


                var obj = new Lib.Data.Invoices();
                obj.Address = adresa;
                obj.City = mesto;
                obj.Company = jmenoFirmy;
                obj.CompanyID = ICO;
                obj.Country = "CZ";
                obj.Created = DateTime.Now;
                obj.FirstName = jmenoOsoby;
                obj.InvoiceNumber = (db.Invoices.Count() + 1).ToString();
                obj.Status = (int)Lib.Data.Invoices.InvoiceStatus.New;
                obj.Text = "Služby serveru HlidacStatu.cz";
                obj.VatID = DIC;
                obj.Zip = PSC;
                obj.ID_Customer = userId;
                var ii = new InvoiceItems()
                {
                    ID_ShopItem = (int)sluzba,
                    Expires = DateTime.Now.AddYears(1),
                    Name = "HlidacStatu.cz - " + sluzba.ToNiceDisplayName(),
                    Invoices = obj,
                    VAT = 1.21m,
                    Created = DateTime.Now,

                };
                switch (sluzba)
                {
                    case Lib.Data.InvoiceItems.ShopItem.Zakladni:
                        ii.Price = 14900;
                        break;
                    case Lib.Data.InvoiceItems.ShopItem.Kompletni:
                        ii.Price = 29900;
                        break;
                    case Lib.Data.InvoiceItems.ShopItem.NGO:
                    default:
                        ii.Price = 0;
                        break;
                }

                obj.InvoiceItems = new InvoiceItems[] { ii };

                db.Invoices.Add(obj);
                db.InvoiceItems.Add(ii);

                if (sendEmail)
                {
                    try
                    {
                        using (MailMessage msg = new MailMessage("info@hlidacstatu.cz", "michal@michalblaha.cz"))
                        {
                            using (SmtpClient smtp = new SmtpClient())
                            {
                                msg.Subject = "Objednavka " + sluzba.ToNiceDisplayName();
                                msg.Body = @"ico:" + ICO + "\n"
                                            + "firma:" + jmenoFirmy + "\n"
                                            + "jmeno:" + jmenoOsoby + "\n"
                                            + "email:" + username + "\n"
                                            + "userid:" + userId + "\n"
                                            ;
                                msg.BodyEncoding = System.Text.Encoding.UTF8;
                                msg.SubjectEncoding = System.Text.Encoding.UTF8;

                                HlidacStatu.Util.Consts.Logger.Info("Sending email to " + msg.To);
                                //msg.Bcc.Add("michal@michalblaha.cz");
                                smtp.Send(msg);
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        HlidacStatu.Util.Consts.Logger.Error("Send email", e);
                    }
                }
                db.SaveChanges();
                return obj;
            }
        }
    }
}
