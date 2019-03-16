using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devmasters.Core;


namespace HlidacStatu.Lib.Data
{
    public partial class FirmaEvent
        : Audit.IAuditable
    {

        public FirmaEvent()
        {
            this.Created = DateTime.Now;
        }

        public FirmaEvent(string ICO, string title, string description, Types type)
        {
            this.ICO = ICO;
            this.Title = title;
            this.Description = description;
            this.Type = (int)type;
            this.Created = DateTime.Now;
        }

        public enum Types
        {
            Popis = 0,
            Sponzor = 3,
            OsobniVztah = 4,
            SponzorZuctu = 33,

        }

        public void SetYearInterval(int year)
        {
            this.DatumOd = new DateTime(year, 1, 1);
            this.DatumDo = new DateTime(year, 12, 31);
        }


        public string RenderText(string delimeter = "\n")
        {
            StringBuilder sb = new StringBuilder();
            switch ((Types)this.Type)
            {
                case Types.Sponzor:
                    return Title + " v " + this.RenderDatum() + (AddInfoNum.HasValue ? ", " + Smlouva.NicePrice(AddInfoNum) : "");
                case Types.SponzorZuctu:
                    return ""; // Title + " v " + this.RenderDatum() + (AddInfoNum.HasValue ? ", " + Smlouva.NicePrice(AddInfoNum) : "") + " (z transp.účtu)";
                case Types.OsobniVztah:
                    if (!string.IsNullOrEmpty(AddInfo) && Devmasters.Core.TextUtil.IsNumeric(AddInfo))
                    {
                        Osoba o = Osoby.GetById.Get(Convert.ToInt32(AddInfo));
                        if (o != null)
                            return this.Title + " s " + o.FullName();
                        else
                            return this.Title + " " + Description;
                    } 
                    else
                        return this.Title + " " + Description;

                case Types.Popis:
                default:
                    if (!string.IsNullOrEmpty(this.Title) && !string.IsNullOrEmpty(this.Description))
                        return this.Title + delimeter + this.Description;
                    else if (!string.IsNullOrEmpty(this.Title))
                        return this.Title;
                    else if (!string.IsNullOrEmpty(this.Description))
                        return this.Description;
                    else
                        return string.Empty;
                    
            }

        }

        public string RenderHtml(string delimeter = ", ")
        {
            string zdroj = "";
            if (!string.IsNullOrEmpty(this.Zdroj))
            {
                if (this.Zdroj.ToLower().StartsWith("http"))
                    zdroj = string.Format(" <a target='_blank' href='{0}'>{1}</a>", this.Zdroj, "<span class='text-muted' title='Jedná se o peněžní nebo nepeněžní dar' alt='Jedná se o peněžní nebo nepeněžní dar'>(<span class='glyphicon glyphicon-link' aria-hidden='true'></span> zdroj</span>)");
                else
                    zdroj = string.Format(" ({0})", this.Zdroj);

            }
            StringBuilder sb = new StringBuilder();
            switch ((Types)this.Type)
            {
                case Types.Sponzor:
                    return Title + " v " + this.RenderDatum() + (AddInfoNum.HasValue ? ", " + Smlouva.NicePrice(AddInfoNum) : "") + zdroj;
                case Types.SponzorZuctu:
                    return ""; // Title + " v " + this.RenderDatum() + (AddInfoNum.HasValue ? ", " + Smlouva.NicePrice(AddInfoNum) : "") + " (z transp.účtu)" + zdroj;
                case Types.OsobniVztah:
                    if (!string.IsNullOrEmpty(AddInfo) && Devmasters.Core.TextUtil.IsNumeric(AddInfo))
                    {
                        Osoba o = Osoby.GetById.Get(Convert.ToInt32(AddInfo));
                        if (o != null)
                            return this.Title + " s " + string.Format("<a href=\"{0}\">{1}</a>", o.GetUrlOnWebsite(), o.FullName()) + zdroj;
                        else
                            return this.Title + " " + Description + zdroj;
                    }
                    else
                        return this.Title + " " + Description + zdroj;

                case Types.Popis:
                default:
                    if (!string.IsNullOrEmpty(this.Title) && !string.IsNullOrEmpty(this.Description))
                        return this.Title + delimeter + this.Description + zdroj;
                    else if (!string.IsNullOrEmpty(this.Title))
                        return this.Title + zdroj;
                    else if (!string.IsNullOrEmpty(this.Description))
                        return this.Description + zdroj;
                    else
                        return string.Empty;

            }

        }

        public string ToAuditJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public string ToAuditObjectTypeName()
        {
            return "FirmaEvent";
        }

        public string ToAuditObjectId()
        {
            return this.pk.ToString();
        }
    }
}


