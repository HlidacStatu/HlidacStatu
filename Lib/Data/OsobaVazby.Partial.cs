using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public partial class OsobaVazby
    {

     

        public Relation.RelationEnum Vazba
        {
            get
            {
                return (Relation.RelationEnum)this.TypVazby;
            }
            set
            {
                this.TypVazby = (int)value;
            }
        }

        public static void AddOrUpdate(
            int osobaId, int vazbakOsobaId,
            int kod_angm, string funkce, decimal? share, DateTime? fromDate, DateTime? toDate, string zdroj = ""
            )
        {

            using (Lib.Data.DbEntities db = new DbEntities())
            {
                var existing = db.OsobaVazby
                    .Where(m =>
                        m.OsobaID == osobaId
                        && m.VazbakOsobaId == vazbakOsobaId
                        && m.DatumOd == fromDate
                        && m.DatumDo == toDate
                    )
                    .FirstOrDefault();
                if (existing == null)
                    existing = db.OsobaVazby
                    .Where(m =>
                        m.OsobaID == osobaId
                        && m.VazbakOsobaId == vazbakOsobaId
                        && m.DatumOd == fromDate
                    )
                    .FirstOrDefault();

                if (existing != null)
                {
                    //update
                    existing.TypVazby = kod_angm;
                    existing.PojmenovaniVazby = funkce;
                    
                    if (existing.podil != share)
                        existing.podil = share;
                    if (existing.DatumOd != fromDate)
                        existing.DatumOd = fromDate;
                    if (existing.DatumDo != toDate)
                        existing.DatumDo = toDate;
                    existing.LastUpdate = DateTime.Now;
                }
                else //new
                {
                    OsobaVazby af = new OsobaVazby();
                    af.OsobaID = osobaId;
                    af.VazbakICO = "";
                    af.VazbakOsobaId = vazbakOsobaId;
                    af.DatumOd = fromDate;
                    af.DatumDo = toDate;
                    af.TypVazby = kod_angm;
                    af.PojmenovaniVazby = funkce;
                    af.podil = share;
                    af.LastUpdate = DateTime.Now;
                    db.OsobaVazby.Add(af);

                }
                try
                {
                    db.SaveChanges();

                }
                catch (Exception e)
                {
                    HlidacStatu.Util.Consts.Logger.Error("",e);
                    throw;
                }
            }
        }

        public static void AddOrUpdate(
            int osobaId, string dcerinkaIco,
            int kod_angm, string funkce, decimal? share, DateTime? fromDate, DateTime? toDate, string zdroj = ""
            )
        {

            using (Lib.Data.DbEntities db = new DbEntities())
            {
                var existing = db.OsobaVazby
                    .Where(m =>
                        m.OsobaID == osobaId
                        && m.VazbakICO == dcerinkaIco
                        && m.DatumOd == fromDate
                        && m.DatumDo == toDate
                    )
                    .FirstOrDefault();
                if (existing == null)
                    existing = db.OsobaVazby
                    .Where(m =>
                        m.OsobaID == osobaId
                        && m.VazbakICO == dcerinkaIco
                        && m.DatumOd == fromDate
                    )
                    .FirstOrDefault();

                if (existing != null)
                {
                    //update
                    existing.TypVazby = kod_angm;
                    existing.PojmenovaniVazby = funkce;
                    if (existing.podil != share)
                        existing.podil = share;
                    if (existing.DatumOd != fromDate)
                        existing.DatumOd = fromDate;
                    if (existing.DatumDo != toDate)
                        existing.DatumDo = toDate;
                    existing.LastUpdate = DateTime.Now;
                }
                else //new
                {
                    OsobaVazby af = new OsobaVazby();
                    af.OsobaID = osobaId;
                    af.VazbakICO = dcerinkaIco;
                    af.DatumOd = fromDate;
                    af.DatumDo = toDate;
                    af.TypVazby = kod_angm;
                    af.PojmenovaniVazby = funkce;
                    af.podil = share;
                    af.LastUpdate = DateTime.Now;
                    db.OsobaVazby.Add(af);

                }
                db.SaveChanges();
            }
        }
    }
}
