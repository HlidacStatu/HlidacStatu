using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devmasters.Core;


namespace HlidacStatu.Lib.Data
{
    public partial class OsobaEvent
        : Audit.IAuditable
    {

        private static ObjectsComparer.Comparer<OsobaEvent> comparer = new ObjectsComparer.Comparer<OsobaEvent>();
        static OsobaEvent()
        {
            comparer.AddComparerOverride("pk", ObjectsComparer.DoNotCompareValueComparer.Instance);
        }
        public OsobaEvent()
        {
            this.Created = DateTime.Now;

        }

        public OsobaEvent(int osobaId, string title, string description, Types type)
        {
            this.OsobaId = osobaId;
            this.Title = title;
            this.Description = description;
            this.Type = (int)type;
            this.Created = DateTime.Now;
        }

        public enum Types
        {
            Popis = 0,
            Poslanec = 1,
            Senator = 2,
            Sponzor = 3,
            OsobniVztah = 4,
            Pribuzny = 5,
            StatniUrednik = 6,
            PolitickaFunkce = 7,
            ClenStrany = 8,
            SponzorZuctu = 33 //sponzor z transparentniho uctu
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
                case Types.ClenStrany:
                    sb.AppendFormat("Člen strany {1} {0} ", this.RenderDatum(), this.AddInfo);
                    return sb.ToString();
                case Types.Poslanec:
                    sb.AppendFormat("Poslanec {0} ", this.RenderDatum());
                    if (!string.IsNullOrEmpty(this.AddInfo))
                        sb.Append(" za " + AddInfo);
                    return sb.ToString();
                case Types.Senator:
                    sb.AppendFormat("Senator {0} ", this.RenderDatum());
                    if (!string.IsNullOrEmpty(this.AddInfo))
                        sb.Append(" za " + AddInfo);
                    return sb.ToString();
                case Types.Sponzor:
                    return Title + " v " + this.RenderDatum() + (AddInfoNum.HasValue ? ", hodnota daru " + Smlouva.NicePrice(AddInfoNum)  : "");
                case Types.OsobniVztah:
                case Types.Pribuzny:
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
                case Types.ClenStrany:
                    sb.AppendFormat("Člen strany {1} {0} ", this.RenderDatum(), this.AddInfo);
                    return sb.ToString();
                case Types.Poslanec:
                    sb.AppendFormat("Poslanec {0}", this.RenderDatum());
                    if (!string.IsNullOrEmpty(this.AddInfo))
                        sb.Append(" za " + AddInfo);
                    return sb.ToString() + zdroj;
                case Types.Senator:
                    sb.AppendFormat("Poslanec {0}", this.RenderDatum());
                    if (!string.IsNullOrEmpty(this.AddInfo))
                        sb.Append(" za " + AddInfo);
                    return sb.ToString() + zdroj;
                case Types.Sponzor:
                    return Title + " v " + this.RenderDatum() + (AddInfoNum.HasValue ? ", hodnota daru " + Smlouva.NicePrice(AddInfoNum) : "") + zdroj;
                case Types.OsobniVztah:
                case Types.Pribuzny:
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

        public void Delete(string user, bool skipTransactionDelete)
        {
            if (this.pk > 0)
            {
                using (DbEntities db = new Data.DbEntities())
                {

                    if (skipTransactionDelete == false && this.Zdroj?.StartsWith("https://www.hlidacstatu.cz/ucty/transakce/") == true)
                    {
                        //delete link in connected transaction
                        TransparentniUcty.BankovniPolozka bp = TransparentniUcty.BankovniPolozka.Get(this.Zdroj.Replace("https://www.hlidacstatu.cz/ucty/transakce/", ""));
                        if (bp != null)
                        {
                            if (bp.Comments.Length > 0)
                            {
                                bp.Comments = new TransparentniUcty.BankovniPolozka.Comment[] { };
                                bp.Save(user);
                            }
                        }
                    }
                    db.OsobaEvent.Attach(this);
                    db.Entry(this).State = System.Data.Entity.EntityState.Deleted;
                    Audit.Add<OsobaEvent>(Audit.Operations.Delete, user, this, null);
                    db.SaveChanges();




                }
            }
        }
        public string ToAuditJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public string ToAuditObjectTypeName()
        {
            return "OsobaEvent";
        }

        public string ToAuditObjectId()
        {
            return this.pk.ToString();
        }

        public static bool Compare(OsobaEvent a, OsobaEvent b)
        {
            return comparer.Compare(a, b);
        }
    }
}


