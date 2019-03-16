using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public partial class FirmaVazby
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


        //scompany_uid,tcompany_uid,company_role_id,company_function,capital,paid_up,share,from_date,to_date
        public static void AddOrUpdate(
            string vlastnikIco, string dcerinkaIco, 
            int kod_angm, string funkce, decimal? share, DateTime? fromDate, DateTime? toDate
            )
        {

            using (Lib.Data.DbEntities db = new DbEntities())
            {
                var existing = db.FirmaVazby
                    .Where(m =>
                        m.ICO == vlastnikIco
                        && m.VazbakICO == dcerinkaIco
                        && m.DatumOd == fromDate
                        && m.DatumDo == toDate
                    )
                    .FirstOrDefault();
                if (existing == null)
                    existing = db.FirmaVazby
                    .Where(m =>
                        m.ICO == vlastnikIco
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
                    if (existing.DatumOd != fromDate && fromDate.HasValue)
                        existing.DatumOd = fromDate;
                    if (existing.DatumDo != toDate && toDate.HasValue)
                        existing.DatumDo = toDate;
                    existing.LastUpdate = DateTime.Now;
                }
                else //new
                {
                    FirmaVazby af = new FirmaVazby();
                    af.ICO = vlastnikIco;
                    af.VazbakICO = dcerinkaIco;
                    af.DatumOd = fromDate;
                    af.DatumDo = toDate;
                    af.TypVazby = kod_angm;
                    af.PojmenovaniVazby = funkce;
                    af.podil = share;
                    af.LastUpdate = DateTime.Now;
                    db.FirmaVazby.Add(af);
                }
                db.SaveChanges();
            }
        }
    }
}
