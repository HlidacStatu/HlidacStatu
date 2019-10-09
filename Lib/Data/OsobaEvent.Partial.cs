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
        : Audit.IAuditable //, IValidatableObject
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

        public OsobaEvent(int osobaId, string title, string note, Types type)
        {
            this.OsobaId = osobaId;
            this.Title = title;
            this.Note = note;
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

        // todo: delete
        // není nejrychlejší
        //public string SubTypeName
        //{
        //    get
        //    {
        //        if (this.SubType is null)
        //            return "";
        //        using (Lib.Data.DbEntities db = new Data.DbEntities())
        //        {
        //            string result = db.EventSubType
        //            .Where(subType =>
        //                subType.EventTypeId == this.Type &&
        //                subType.Id == this.SubType
        //            )
        //            .Select(subtype => subtype.NameMale)
        //            .FirstOrDefault();

        //            return result;
        //        }
        //    }
        //}


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

        // tohle by ještě sneslo optimalizaci - ale až budou k dispozici data
        public static IEnumerable<string> GetAddInfos(string jmeno, int? eventTypeId, int maxNumOfResults = 1500)
        {
            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {
                var result = db.OsobaEvent
                    .Where(m => m.Type == eventTypeId )
                    .Where(m => m != null)
                    .Where(m => m.AddInfo.Contains(jmeno))
                    //.OrderBy(m => m.AddInfo)
                    .Select(m => m.AddInfo)
                    .Distinct()
                    .Take(maxNumOfResults)
                    .ToList();

                return result;
            }
        }


        public static OsobaEvent Update(OsobaEvent osobaEvent, string user)
        {
            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {
                // create
                if (osobaEvent.pk == 0 && osobaEvent.OsobaId > 0)
                {
                    osobaEvent.Organizace = ParseTools.NormalizaceStranaShortName(osobaEvent.Organizace);
                    osobaEvent.Created = DateTime.Now;
                    db.OsobaEvent.Add(osobaEvent);
                    db.SaveChanges();
                    Audit.Add(Audit.Operations.Update, user, osobaEvent, null);
                    return osobaEvent;
                }

                // update
                if (osobaEvent.pk > 0)
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
                        eventToUpdate.Organizace = ParseTools.NormalizaceStranaShortName(osobaEvent.Organizace);
                        eventToUpdate.AddInfoNum = osobaEvent.AddInfoNum;
                        eventToUpdate.AddInfo = osobaEvent.AddInfo;
                        eventToUpdate.Title = osobaEvent.Title;
                        eventToUpdate.Type = osobaEvent.Type;
                        eventToUpdate.Zdroj = osobaEvent.Zdroj;

                        eventToUpdate.Created = DateTime.Now;

                        db.SaveChanges();
                        Audit.Add(Audit.Operations.Update, user, eventToUpdate, eventOriginal);

                        return eventToUpdate;
                    }
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
                    sb.AppendFormat("Člen strany {1} {0} ", this.RenderDatum(), this.Organizace);
                    return sb.ToString();
                // poslanec a senátor sloučeni
                case Types.VolenaFunkce:
                    sb.Append($"{this.AddInfo} {this.RenderDatum()} ");
                    if (!string.IsNullOrEmpty(this.Organizace))
                        sb.Append(" za " + Organizace);
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
                            return this.Title + " " + Note;
                    }
                    else
                        return this.Title + " " + Note;

                case Types.Jina:
                default:
                    if (!string.IsNullOrEmpty(this.Title) && !string.IsNullOrEmpty(this.Note))
                        return this.Title + delimeter + this.Note;
                    else if (!string.IsNullOrEmpty(this.Title))
                        return this.Title;
                    else if (!string.IsNullOrEmpty(this.Note))
                        return this.Note;
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
                    sb.AppendFormat("Člen strany {1} {0} ", this.RenderDatum(), this.Organizace);
                    return sb.ToString();
                // poslanec a senátor sloučeni
                case Types.VolenaFunkce:
                    sb.Append($"{this.AddInfo} {this.RenderDatum()} ");
                    if (!string.IsNullOrEmpty(this.Organizace))
                        sb.Append(" za " + Organizace);
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
                            return this.Title + " " + Note + zdroj;
                    }
                    else
                        return this.Title + " " + Note + zdroj;

                case Types.Jina:
                default:
                    if (!string.IsNullOrEmpty(this.Title) && !string.IsNullOrEmpty(this.Note))
                        return this.Title + delimeter + this.Note + zdroj;
                    else if (!string.IsNullOrEmpty(this.Title))
                        return this.Title + zdroj;
                    else if (!string.IsNullOrEmpty(this.Note))
                        return this.Note + zdroj;
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

        // todo: delete
        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (!EventSubType.IsValidSubtype(this.Type, this.SubType))
        //    {
        //        yield return new ValidationResult(
        //            $"Zvolený podtyp nepatří ke zvolenému typu.",
        //            new[] { nameof(Type), nameof(SubType) });
        //    }
        //}
    }
}


