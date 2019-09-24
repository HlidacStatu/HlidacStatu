using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devmasters.Core;
using HlidacStatu.Util;

namespace HlidacStatu.Lib.Data
{
    [MetadataType(typeof(OsobaEventMetadata))]
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
            Jina = 0,
            VolenaFunkce = 1,
            SoukromaPracovni = 2,
            Sponzor = 3,
            Osobni = 4,
            VerejnaSpravaPracovni = 6,
            Politicka = 7,
            PolitickaPracovni = 9
        }

        public enum SubTypes
        {
            Prezident = 1,
            Poslanec = 2,
            Senator = 3,
            ClenKrajskehoZastupitelstva = 4,
            ClenObecnihoZastupitelstva = 5,
            ClenNarodnihoVyboru = 6,
            PoslanecEvropskehoParlamentu = 7,
            PredsedaPoslaneckeSnemovnyParlamentuCR = 8,
            PredsedaSenatuParlamentuCR = 9,
            MistopredsedaPoslaneckeSnemovnyParlamentuCR = 10,
            MistopredsedaSenatuParlamentuCR = 11,
            PredsedaVladyCR = 12,
            MistopredsedaVladyCR = 13,
            MinistrVladyCR = 14,
            StarostaPrimator = 15
        }

        public void SetYearInterval(int year)
        {
            this.DatumOd = new DateTime(year, 1, 1);
            this.DatumDo = new DateTime(year, 12, 31);
        }

        // není nejrychlejší, ale asi stačí
        public string TypeName
        {
            get
            {
                using (Lib.Data.DbEntities db = new Data.DbEntities())
                {
                    string result = db.EventType
                    .Where(type =>
                        type.Id == this.Type
                    )
                    .Select(type => type.Name)
                    .FirstOrDefault();

                    return result;
                }
            }
        }

        // není nejrychlejší
        public string SubTypeName
        {
            get
            {
                if (this.SubType is null)
                    return "";
                using (Lib.Data.DbEntities db = new Data.DbEntities())
                {
                    string result = db.EventSubType
                    .Where(subType =>
                        subType.EventTypeId == this.Type &&
                        subType.Id == this.SubType
                    )
                    .Select(subtype => subtype.NameMale)
                    .FirstOrDefault();

                    return result;
                }
            }
        }


        public static OsobaEvent GetById(int id)
        {
            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {
                return db.OsobaEvent
                .Where(m =>
                    m.pk == id
                )
                .FirstOrDefault();
            }
        }

        public static OsobaEvent Create(OsobaEvent osobaEvent, string user)
        {
            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {
                if (osobaEvent.OsobaId > 0)
                {
                    osobaEvent.PolitickaStrana = ParseTools.NormalizaceStranaShortName(osobaEvent.PolitickaStrana);
                    osobaEvent.Created = DateTime.Now;
                    db.OsobaEvent.Add(osobaEvent);
                    Audit.Add(Audit.Operations.Update, user, osobaEvent, null);
                    db.SaveChanges();
                }
                return osobaEvent;
            }
        }

        public static OsobaEvent Update(OsobaEvent osobaEvent, string user)
        {
            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {
                var eventToUpdate = db.OsobaEvent
                .Where(ev =>
                    ev.pk == osobaEvent.pk
                ).FirstOrDefault();

                var eventOriginal = eventToUpdate.ShallowCopy();

                if (eventToUpdate != null)
                {
                    eventToUpdate.DatumOd = osobaEvent.DatumOd;
                    eventToUpdate.DatumDo = osobaEvent.DatumDo;
                    eventToUpdate.PolitickaStrana = ParseTools.NormalizaceStranaShortName(osobaEvent.PolitickaStrana);
                    eventToUpdate.AddInfoNum = osobaEvent.AddInfoNum;
                    eventToUpdate.Title = osobaEvent.Title;
                    eventToUpdate.Type = osobaEvent.Type;
                    eventToUpdate.Zdroj = osobaEvent.Zdroj;

                    eventToUpdate.Created = DateTime.Now;

                    Audit.Add(Audit.Operations.Update, user, eventToUpdate, eventOriginal);
                    db.SaveChanges();

                    return eventToUpdate;
                }
            }
            return osobaEvent;
        }

        public OsobaEvent ShallowCopy()
        {
            return (OsobaEvent)this.MemberwiseClone();
        }


        public string RenderText(string delimeter = "\n")
        {
            StringBuilder sb = new StringBuilder();
            switch ((Types)this.Type)
            {
                case Types.Politicka:
                    sb.AppendFormat("Člen strany {1} {0} ", this.RenderDatum(), this.PolitickaStrana);
                    return sb.ToString();
                // poslanec a senátor sloučeni
                case Types.VolenaFunkce:
                    sb.Append($"{this.SubTypeName} {this.RenderDatum()} ");
                    if (!string.IsNullOrEmpty(this.PolitickaStrana))
                        sb.Append(" za " + PolitickaStrana);
                    return sb.ToString();
                case Types.Sponzor:
                    return Title + " v " + this.RenderDatum() + (AddInfoNum.HasValue ? ", hodnota daru " + Smlouva.NicePrice(AddInfoNum) : "");
                case Types.Osobni:
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

                case Types.Jina:
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
                case Types.Politicka:
                    sb.AppendFormat("Člen strany {1} {0} ", this.RenderDatum(), this.PolitickaStrana);
                    return sb.ToString();
                // poslanec a senátor sloučeni
                case Types.VolenaFunkce:
                    sb.Append($"{this.SubTypeName} {this.RenderDatum()} ");
                    if (!string.IsNullOrEmpty(this.PolitickaStrana))
                        sb.Append(" za " + PolitickaStrana);
                    return sb.ToString() + zdroj;
                case Types.Sponzor:
                    return Title + " v " + this.RenderDatum() + (AddInfoNum.HasValue ? ", hodnota daru " + Smlouva.NicePrice(AddInfoNum) : "") + zdroj;
                case Types.Osobni:
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

                case Types.Jina:
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


