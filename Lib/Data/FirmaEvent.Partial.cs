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
                            return this.Title + " s " + string.Format("<a href=\"{0}\">{1}</a>", o.GetUrl(), o.FullName()) + zdroj;
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

        public static FirmaEvent CreateOrUpdate(FirmaEvent firmaEvent, string user)
        {
            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {
                // create
                if (firmaEvent.pk == 0 && firmaEvent.ICO.Length > 0)
                {
                    //firmaEvent.Organizace = ParseTools.NormalizaceStranaShortName(firmaEvent.Organizace);
                    firmaEvent.Created = DateTime.Now;

                    // check if event exists so we are not creating duplicities...
                    var oe = db.FirmaEvent
                        .Where(ev =>
                            ev.ICO == firmaEvent.ICO
                            && ev.AddInfo == firmaEvent.AddInfo
                            && ev.DatumOd == firmaEvent.DatumOd
                            && ev.Type == firmaEvent.Type)
                            //&& ev.Organizace == firmaEvent.Organizace)
                        .FirstOrDefault();

                    if (oe != null)
                        return oe;

                    db.FirmaEvent.Add(firmaEvent);
                    db.SaveChanges();
                    //Firmy.Get(firmaEvent.ICO).FlushCache();
                    Audit.Add(Audit.Operations.Update, user, firmaEvent, null);
                    return firmaEvent;
                }

                // update
                if (firmaEvent.pk > 0)
                {
                    var eventToUpdate = db.FirmaEvent
                    .Where(ev =>
                        ev.pk == firmaEvent.pk
                    ).FirstOrDefault();

                    var eventOriginal = eventToUpdate.ShallowCopy();

                    if (eventToUpdate != null)
                    {
                        eventToUpdate.DatumOd = firmaEvent.DatumOd;
                        eventToUpdate.DatumDo = firmaEvent.DatumDo;
                        //eventToUpdate.Organizace = ParseTools.NormalizaceStranaShortName(firmaEvent.Organizace);
                        eventToUpdate.AddInfoNum = firmaEvent.AddInfoNum;
                        eventToUpdate.AddInfo = firmaEvent.AddInfo;
                        eventToUpdate.Title = firmaEvent.Title;
                        eventToUpdate.Type = firmaEvent.Type;
                        eventToUpdate.Zdroj = firmaEvent.Zdroj;
                        //eventToUpdate.Status = firmaEvent.Status;

                        eventToUpdate.Created = DateTime.Now;

                        db.SaveChanges();
                        //Osoby.GetById.Get(firmaEvent.OsobaId).FlushCache();
                        Audit.Add(Audit.Operations.Update, user, eventToUpdate, eventOriginal);

                        return eventToUpdate;
                    }
                }
            }
            return firmaEvent;
        }

        public FirmaEvent ShallowCopy()
        {
            return (FirmaEvent)this.MemberwiseClone();
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


