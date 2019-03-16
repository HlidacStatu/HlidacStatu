using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public partial class OsobaExternalId
    {
        public enum Source
        {
            Merk = 1,
            Firmo = 2,
            HlidacSmluvGuid = 3,


        }
        public OsobaExternalId()
        {
        }
        public OsobaExternalId(int osobaId, string externalId, Source externalsource)
        {
            this.OsobaId = osobaId;
            this.ExternalId = externalId;
            this.ExternalSource = (int)externalsource;
        }

        public static void Add(OsobaExternalId externalId)
        {
            if (externalId == null)
                return;
            Add(externalId.OsobaId, externalId.ExternalId, (Source)externalId.ExternalSource);
        }
        public static void Add(int osobaId, string externalId, Source externalsource)
        {
            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {
                var exist = db.OsobaExternalId
                    .Where(m => m.OsobaId == osobaId && m.ExternalId == externalId && m.ExternalSource == (int)externalsource)
                    .FirstOrDefault();
                if (exist == null)
                {
                    OsobaExternalId oei = new Data.OsobaExternalId(osobaId, externalId, externalsource );
                    db.OsobaExternalId.Add(oei);
                    db.SaveChanges();
                }
            }
        }
        }
}
