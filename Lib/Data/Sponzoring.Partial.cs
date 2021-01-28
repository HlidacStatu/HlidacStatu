using System;
using System.Collections.Generic;
using System.Linq;
using Devmasters.Enums;

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
        public enum TypDaru
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

        public static Sponzoring CreateOrUpdate(Sponzoring sponzoring, string user)
        {
            using (DbEntities db = new DbEntities())
            {

                Sponzoring sponzoringToUpdate = FindDuplicate(sponzoring, db);
                
                if(sponzoringToUpdate != null)
                {
                    return UpdateSponzoring(sponzoringToUpdate, sponzoring, user, db);
                }

                return CreateSponzoring(sponzoring, user, db);
            }
        }

        private static Sponzoring FindDuplicate(Sponzoring sponzoring, DbEntities db)
        {
            // u sponzoringu nekontrolujeme organizaci, ani rok, protože to není historicky konzistentní
            return db.Sponzoring
                .Where(s =>
                    s.Id == sponzoring.Id
                    || (s.IcoDarce == sponzoring.IcoDarce
                    && s.IcoPrijemce == sponzoring.IcoPrijemce
                    && s.OsobaIdDarce == sponzoring.OsobaIdDarce
                    && s.OsobaIdPrijemce == sponzoring.OsobaIdPrijemce
                    && s.Typ == sponzoring.Typ
                    && s.Hodnota == sponzoring.Hodnota
                    && s.DarovanoDne == sponzoring.DarovanoDne)
                )
                .FirstOrDefault();
        }

        private static Sponzoring CreateSponzoring(Sponzoring sponzoring, string user, DbEntities db)
        {
            if (sponzoring.OsobaIdDarce == 0 
                && string.IsNullOrWhiteSpace(sponzoring.IcoDarce))
                throw new Exception("Cant attach sponzoring to a person or to a company since their reference is empty");

            sponzoring.Created = DateTime.Now;
            sponzoring.Edited = DateTime.Now;
            sponzoring.UpdatedBy = user;
            
            db.Sponzoring.Add(sponzoring);
            db.SaveChanges();

            Audit.Add(Audit.Operations.Create, user, sponzoring, null);
            return sponzoring;
        }

        private static Sponzoring UpdateSponzoring(Sponzoring sponzoringToUpdate, Sponzoring sponzoring, string user, DbEntities db)
        {
            var sponzoringOriginal = sponzoringToUpdate.ShallowCopy();

            if (!string.IsNullOrWhiteSpace(sponzoring.IcoDarce))
                sponzoringToUpdate.IcoDarce = sponzoring.IcoDarce;
            if (sponzoring.OsobaIdDarce > 0)
                sponzoringToUpdate.OsobaIdDarce = sponzoring.OsobaIdDarce;

            sponzoringToUpdate.Edited = DateTime.Now;
            sponzoringToUpdate.UpdatedBy = user;

            sponzoringToUpdate.DarovanoDne = sponzoring.DarovanoDne;
            sponzoringToUpdate.Hodnota = sponzoring.Hodnota;
            sponzoringToUpdate.IcoPrijemce = sponzoring.IcoPrijemce;
            sponzoringToUpdate.OsobaIdPrijemce = sponzoring.OsobaIdPrijemce;
            sponzoringToUpdate.Popis = sponzoring.Popis;
            sponzoringToUpdate.Typ = sponzoring.Typ;
            sponzoringToUpdate.Zdroj = sponzoring.Zdroj;

            db.SaveChanges();
       
            Audit.Add<Sponzoring>(Audit.Operations.Update, user, sponzoringToUpdate, sponzoringOriginal);
            return sponzoringToUpdate;
        }

        public Sponzoring ShallowCopy()
        {
            return (Sponzoring)this.MemberwiseClone();
        }


        public void Delete(string user)
        {
            if (this.Id > 0)
            {
                using (DbEntities db = new Data.DbEntities())
                {
                    db.Sponzoring.Attach(this);
                    db.Entry(this).State = System.Data.Entity.EntityState.Deleted;
                    Audit.Add<Sponzoring>(Audit.Operations.Delete, user, this, null);
                    
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


