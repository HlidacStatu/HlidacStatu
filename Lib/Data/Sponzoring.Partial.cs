using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Devmasters;
using Devmasters.Enums;

using HlidacStatu.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HlidacStatu.Lib.Data
{
    public partial class Sponzoring
        : Audit.IAuditable //, IValidatableObject
    {

        //private static ObjectsComparer.Comparer<OsobaEvent> comparer = new ObjectsComparer.Comparer<OsobaEvent>();
        static Sponzoring()
        {
        }

        public Sponzoring()
        {
            this.Created = DateTime.Now;
        }

        

        [ShowNiceDisplayName()]
        public enum Typ
        {
            [NiceDisplayName("Finanční dar")]
            FinancniDar = 0,
            [NiceDisplayName("Nefinanční dar")]
            NefinancniDar = 1,
            
        }

        public static IEnumerable<Sponzoring> GetByDarce(int osobaId)
        {
            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {
                return db.Sponzoring
                    .Where(s => s.OsobaIdDarce == osobaId);
            }
        }

        public static IEnumerable<Sponzoring> GetByDarce(string icoDarce)
        {
            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {
                return db.Sponzoring
                    .Where(s => s.IcoDarce == icoDarce);
            }
        }

        public static IEnumerable<Sponzoring> GetByPrijemce(int osobaId)
        {
            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {
                return db.Sponzoring
                    .Where(s => s.OsobaIdPrijemce == osobaId);
            }
        }

        public static IEnumerable<Sponzoring> GetByPrijemce(string icoPrijemce)
        {
            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {
                return db.Sponzoring
                    .Where(s => s.IcoPrijemce == icoPrijemce);
            }
        }

        public static OsobaEvent CreateOrUpdate(OsobaEvent osobaEvent, string user)
        {
            using (DbEntities db = new DbEntities())
            {
                OsobaEvent eventToUpdate = null;
                // známe PK
                if (osobaEvent.pk > 0)
                {
                    eventToUpdate = db.OsobaEvent
                        .Where(ev =>
                            ev.pk == osobaEvent.pk
                        )
                        .FirstOrDefault();

                    if(eventToUpdate != null)
                        return UpdateEvent(eventToUpdate, osobaEvent, user, db);
                }

                eventToUpdate = GetDuplicate(osobaEvent, db);

                if(eventToUpdate != null)
                {
                    return UpdateEvent(eventToUpdate, osobaEvent, user, db);
                }

                return CreateEvent(osobaEvent, user, db);
            }
        }

        private static OsobaEvent GetDuplicate(OsobaEvent osobaEvent, DbEntities db)
        {
            if (osobaEvent.Type == (int)OsobaEvent.Types.Sponzor)
            {
                // u sponzoringu nekontrolujeme organizaci, ani rok, protože to není historicky konzistentní
                return db.OsobaEvent
                    .Where(ev =>
                        ev.OsobaId == osobaEvent.OsobaId
                        && ev.Ico == osobaEvent.Ico
                        && ev.Type == osobaEvent.Type
                        && ev.AddInfo == osobaEvent.AddInfo
                        && ev.AddInfoNum == osobaEvent.AddInfoNum
                        && ev.DatumOd.HasValue
                        && ev.DatumOd.Value.Year == osobaEvent.DatumOd.Value.Year)
                    .FirstOrDefault();
            }
            
            return db.OsobaEvent
                .Where(ev =>
                    ev.OsobaId == osobaEvent.OsobaId
                    && ev.Ico == osobaEvent.Ico
                    && ev.AddInfo == osobaEvent.AddInfo
                    && ev.AddInfoNum == osobaEvent.AddInfoNum
                    && ev.DatumOd == osobaEvent.DatumOd
                    && ev.Type == osobaEvent.Type
                    && ev.Organizace == osobaEvent.Organizace)
                .FirstOrDefault();
        }

        private static OsobaEvent CreateEvent(OsobaEvent osobaEvent, string user, DbEntities db)
        {
            if (osobaEvent.OsobaId == 0 && string.IsNullOrWhiteSpace(osobaEvent.Ico))
                throw new Exception("Cant attach event to a person or to a company since their reference is empty");

            osobaEvent.Organizace = ParseTools.NormalizaceStranaShortName(osobaEvent.Organizace);
            osobaEvent.Created = DateTime.Now;
            db.OsobaEvent.Add(osobaEvent);
            db.SaveChanges();
            if (osobaEvent.OsobaId > 0)
            {
                Osoby.GetById.Get(osobaEvent.OsobaId).FlushCache();
            }
            Audit.Add(Audit.Operations.Update, user, osobaEvent, null);
            return osobaEvent;
            
        }

        private static OsobaEvent UpdateEvent(OsobaEvent eventToUpdate, OsobaEvent osobaEvent, string user, DbEntities db)
        {
            if (eventToUpdate is null)
                throw new ArgumentNullException(nameof(eventToUpdate), "Argument can't be null");
            if (osobaEvent is null)
                throw new ArgumentNullException(nameof(osobaEvent), "Argument can't be null");
            if (db is null)
                throw new ArgumentNullException(nameof(db), "Argument can't be null");

            var eventOriginal = eventToUpdate.ShallowCopy();

            if (!string.IsNullOrWhiteSpace(osobaEvent.Ico))
                eventToUpdate.Ico = osobaEvent.Ico;
            if (osobaEvent.OsobaId > 0)
                eventToUpdate.OsobaId = osobaEvent.OsobaId;
            
            eventToUpdate.DatumOd = osobaEvent.DatumOd;
            eventToUpdate.DatumDo = osobaEvent.DatumDo;
            eventToUpdate.Organizace = ParseTools.NormalizaceStranaShortName(osobaEvent.Organizace);
            eventToUpdate.AddInfoNum = osobaEvent.AddInfoNum;
            eventToUpdate.AddInfo = osobaEvent.AddInfo;
            eventToUpdate.Title = osobaEvent.Title;
            eventToUpdate.Type = osobaEvent.Type;
            eventToUpdate.Zdroj = osobaEvent.Zdroj;
            eventToUpdate.Status = osobaEvent.Status;
            eventToUpdate.CEO = osobaEvent.CEO;
            eventToUpdate.Created = DateTime.Now;

            db.SaveChanges();
            if (osobaEvent.OsobaId > 0)
            {
                Osoby.GetById.Get(osobaEvent.OsobaId).FlushCache();
            }
            Audit.Add(Audit.Operations.Update, user, eventToUpdate, eventOriginal);
            return eventToUpdate;
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

                case Types.Sponzor:
                    if (!string.IsNullOrEmpty(Note) 
                        && Note.StartsWith("Člen statut.", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return $"{Note} {Organizace} v {DatumOd?.Year}" + (AddInfoNum.HasValue ? ", hodnota daru " + Smlouva.NicePrice(AddInfoNum) : "");
                    }
                    return $"Sponzor {Organizace} v {DatumOd?.Year}" + (AddInfoNum.HasValue ? ", hodnota daru " + Smlouva.NicePrice(AddInfoNum) : "");
                
            }
        }

        public string RenderTextFirma(string delimeter = "\n") //přidat jako RenderTextFirma
        {
            StringBuilder sb = new StringBuilder();
            switch ((Types)this.Type)
            {
                case Types.Sponzor:
                    return Title + " v " + this.RenderDatum() + (AddInfoNum.HasValue ? ", " + Smlouva.NicePrice(AddInfoNum) : "");
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
                    if (!string.IsNullOrEmpty(Note) 
                        && Note.StartsWith("Člen statut.", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return $"{Note} {Organizace} v {DatumOd?.Year}" + (AddInfoNum.HasValue ? ", hodnota daru " + Smlouva.NicePrice(AddInfoNum) : "");
                    }
                    return $"Sponzor {Organizace} v {DatumOd?.Year}" + (AddInfoNum.HasValue ? ", hodnota daru " + Smlouva.NicePrice(AddInfoNum) : "") + zdroj;
            }
        }

        public string RenderHtmlFirma(string delimeter = ", ")
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
            return "Sponzoring";
        }

        public string ToAuditObjectId()
        {
            return this.Id.ToString();
        }

        //public static bool Compare(Sponzoring a, Sponzoring b)
        //{
        //    return a.AddInfo == b.AddInfo
        //        && a.AddInfoNum == b.AddInfoNum
        //        && a.DatumDo == b.DatumDo
        //        && a.DatumOd == b.DatumOd
        //        && a.Organizace == b.Organizace
        //        && a.OsobaId == b.OsobaId
        //        && a.Status == b.Status
        //        && a.Title == b.Title
        //        && a.Type == b.Type;
        //}

    }
}


