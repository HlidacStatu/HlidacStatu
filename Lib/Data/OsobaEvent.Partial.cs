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

        //private static ObjectsComparer.Comparer<OsobaEvent> comparer = new ObjectsComparer.Comparer<OsobaEvent>();
        static OsobaEvent()
        {
            //comparer.AddComparerOverride("pk", ObjectsComparer.DoNotCompareValueComparer.Instance);
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

        [ShowNiceDisplayName()]
        public enum Types
        {
            [NiceDisplayName("Speciální")]
            Specialni = 0,
            [NiceDisplayName("Volená funkce")]
            VolenaFunkce = 1,
            [NiceDisplayName("Soukromá pracovní")]
            SoukromaPracovni = 2,
            [NiceDisplayName("Sponzor")]
            Sponzor = 3,
            [NiceDisplayName("Osobní")]
            Osobni = 4,
            [NiceDisplayName("Veřejná správa pracovní")]
            VerejnaSpravaPracovni = 6,
            [NiceDisplayName("Politická")]
            Politicka = 7,
            [NiceDisplayName("Politická pracovní")]
            PolitickaPracovni = 9,
            [NiceDisplayName("Veřejná správa jiné")]
            VerejnaSpravaJine = 10,
            [NiceDisplayName("Jiné")]
            Jine = 11,
            [NiceDisplayName("Sociální sítě")]
            SocialniSite = 12,
            [NiceDisplayName("Centrální registr oznámení")]
            CentralniRegistrOznameni = 13
        }

        [Flags]
        [ShowNiceDisplayName()]
        public enum Statuses
        {
            [NiceDisplayName("Skryto NP")]
            NasiPoliticiSkryte = 1,
            //[NiceDisplayName("Jen pro tesst")]
            //HlidacSkryte = 2
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

        public static IEnumerable<string> GetOrganisations(string jmeno, int? eventTypeId, int maxNumOfResults = 1500)
        {
            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {
                var result = db.OsobaEvent
                    .Where(m => m.Type == eventTypeId)
                    .Where(m => m != null)
                    .Where(m => m.Organizace.Contains(jmeno))
                    //.OrderBy(m => m.AddInfo)
                    .Select(m => m.Organizace)
                    .Distinct()
                    .Take(maxNumOfResults)
                    .ToList();

                return result;
            }
        }

        public static OsobaEvent CreateOrUpdate(OsobaEvent osobaEvent, string user)
        {
            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {
                // create
                if (osobaEvent.pk == 0 && osobaEvent.OsobaId > 0)
                {
                    osobaEvent.Organizace = ParseTools.NormalizaceStranaShortName(osobaEvent.Organizace);
                    osobaEvent.Created = DateTime.Now;

                    // check if event exists so we are not creating duplicities...
                    var oe = db.OsobaEvent
                        .Where(ev =>
                            ev.OsobaId == osobaEvent.OsobaId
                            && ev.AddInfo == osobaEvent.AddInfo
                            && ev.DatumOd == osobaEvent.DatumOd
                            && ev.Type == osobaEvent.Type
                            && ev.Organizace == osobaEvent.Organizace)
                        .FirstOrDefault();

                    if (oe != null)
                        return oe;

                    db.OsobaEvent.Add(osobaEvent);
                    db.SaveChanges();
                    Osoby.GetById.Get(osobaEvent.OsobaId).FlushCache();
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
                        eventToUpdate.Status = osobaEvent.Status;

                        eventToUpdate.Created = DateTime.Now;

                        db.SaveChanges();
                        Osoby.GetById.Get(osobaEvent.OsobaId).FlushCache();
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
                //    sb.AppendFormat("Člen strany {1} {0} ", this.RenderDatum(txtOd:"od", txtDo:" do ", template:"({0})"), this.Organizace);
                //    return sb.ToString();
                //// poslanec a senátor sloučeni
                case Types.PolitickaPracovni:
                case Types.VerejnaSpravaJine:
                case Types.VerejnaSpravaPracovni:
                case Types.SoukromaPracovni:
                case Types.VolenaFunkce:
                    sb.Append($"{this.AddInfo} {this.RenderDatum(txtOd: "od", txtDo: " do ", template: "({0})")} ");
                    if (!string.IsNullOrEmpty(this.Organizace))
                        sb.Append(" - " + Organizace);
                    return sb.ToString();
                case Types.Sponzor:
                    return $"Sponzor {Organizace} v {this.RenderDatum()}" + (AddInfoNum.HasValue ? ", hodnota daru " + Smlouva.NicePrice(AddInfoNum) : "");
                    //return Title + " v " + this.RenderDatum() + (AddInfoNum.HasValue ? ", hodnota daru " + Smlouva.NicePrice(AddInfoNum) : "") ;
                case Types.Osobni:
                    if (!string.IsNullOrEmpty(AddInfo) && Devmasters.Core.TextUtil.IsNumeric(AddInfo))
                    {
                        Osoba o = Osoby.GetById.Get(Convert.ToInt32(AddInfo));
                        if (o != null)
                            return this.Title + " s " + o.FullName();
                    }
                    if (!string.IsNullOrEmpty(this.AddInfo + this.Organizace))
                    {
                        sb.Append($"{this.AddInfo} {this.RenderDatum(txtOd: "od", txtDo: " do ", template: "({0})")} ");
                        if (!string.IsNullOrEmpty(this.Organizace))
                            sb.Append(" - " + Organizace);
                        return sb.ToString();
                    }
                    else
                        return (this.Title + " " + Note).Trim();

                case Types.Specialni:
                default:
                    if (!string.IsNullOrEmpty(this.AddInfo + this.Organizace))
                    {
                        sb.Append($"{this.AddInfo} {this.RenderDatum(txtOd: "od", txtDo: " do ", template: "({0})")} ");
                        if (!string.IsNullOrEmpty(this.Organizace))
                            sb.Append(" - " + Organizace);
                        return sb.ToString();
                    }
                    if (!string.IsNullOrEmpty(this.Title) && !string.IsNullOrEmpty(this.Note))
                        return this.Title + delimeter + this.Note ;
                    else if (!string.IsNullOrEmpty(this.Title))
                        return this.Title ;
                    else if (!string.IsNullOrEmpty(this.Note))
                        return this.Note ;
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
                //    sb.AppendFormat("Člen strany {1} {0} ", this.RenderDatum(txtOd:"od", txtDo:" do ", template:"({0})"), this.Organizace);
                //    return sb.ToString();
                //// poslanec a senátor sloučeni
                case Types.PolitickaPracovni:
                case Types.VerejnaSpravaJine:
                case Types.VerejnaSpravaPracovni:
                case Types.SoukromaPracovni:
                case Types.VolenaFunkce:
                    sb.Append($"{this.AddInfo} {this.RenderDatum(txtOd: "od", txtDo: " do ", template: "({0})")} ");
                    if (!string.IsNullOrEmpty(this.Organizace))
                        sb.Append(" - " + Organizace);
                    return sb.ToString();
                case Types.Sponzor:
                    return $"Sponzor {Organizace} v {this.RenderDatum()}" + (AddInfoNum.HasValue ? ", hodnota daru " + Smlouva.NicePrice(AddInfoNum) : "") + zdroj;
                    //return Title + " v " + this.RenderDatum() + (AddInfoNum.HasValue ? ", hodnota daru " + Smlouva.NicePrice(AddInfoNum) : "") + zdroj;
                case Types.Osobni:
                    if (!string.IsNullOrEmpty(AddInfo) && Devmasters.Core.TextUtil.IsNumeric(AddInfo))
                    {
                        Osoba o = Osoby.GetById.Get(Convert.ToInt32(AddInfo));
                        if (o != null)
                            return this.Title + " s " + string.Format("<a href=\"{0}\">{1}</a>", o.GetUrl(), o.FullName());
                    }                    
                    if (!string.IsNullOrEmpty(this.AddInfo + this.Organizace))
                    {
                        sb.Append($"{this.AddInfo} {this.RenderDatum(txtOd: "od", txtDo: " do ", template: "({0})")} ");
                        if (!string.IsNullOrEmpty(this.Organizace))
                            sb.Append(" - " + Organizace);
                        return sb.ToString();
                    }
                    else
                        return (this.Title + " " + Note).Trim() ;

                case Types.Specialni:
                default:
                    if (!string.IsNullOrEmpty(this.AddInfo + this.Organizace))
                    {
                        sb.Append($"{this.AddInfo} {this.RenderDatum(txtOd: "od", txtDo: " do ", template: "({0})")} ");
                        if (!string.IsNullOrEmpty(this.Organizace))
                            sb.Append(" - " + Organizace);
                        return sb.ToString();
                    }
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

        public void Delete(string user)
        {
            if (this.pk > 0)
            {
                using (DbEntities db = new Data.DbEntities())
                {
                    db.OsobaEvent.Attach(this);
                    db.Entry(this).State = System.Data.Entity.EntityState.Deleted;
                    Audit.Add<OsobaEvent>(Audit.Operations.Delete, user, this, null);
                    Osoby.CachedEvents.Delete(this.OsobaId);
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
            return a.AddInfo == b.AddInfo
                && a.AddInfoNum == b.AddInfoNum
                && a.DatumDo == b.DatumDo
                && a.DatumOd == b.DatumOd
                && a.Organizace == b.Organizace
                && a.OsobaId == b.OsobaId
                && a.Status == b.Status
                && a.Title == b.Title
                && a.Type == b.Type;
        }

    }
}


