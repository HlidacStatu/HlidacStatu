using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public partial class FirmaHint
    {
        public static FirmaHint Load(string ico)
        {
            using (DbEntities db = new DbEntities())
            {
                return db.FirmaHint
                    .AsNoTracking()
                    .Where(m => m.Ico == ico)
                    .FirstOrDefault() ?? new FirmaHint() { Ico = ico };                    
                    ;
            }
        }

        public void Recalculate()
        {
            var resMinDate = Smlouva.Search.SimpleSearch("ico:" + this.Ico, 1, 1, Smlouva.Search.OrderResult.DateSignedAsc, platnyZaznam: true);
            if (resMinDate.Total > 0)
            {
                DateTime firstSmlouva = resMinDate.Results.First().datumUzavreni;
                DateTime zalozena = Firmy.Get(this.Ico).Datum_Zapisu_OR ?? new DateTime(1990, 1, 1);
                this.PocetDni_k_PrvniSmlouve = (int)((firstSmlouva - zalozena).TotalDays);
            }

        }


        public void Save()
        {
            using (DbEntities db = new DbEntities())
            {
                db.FirmaHint.Attach(this);
                if (db.FirmaHint.Any(m=>m.Ico == this.Ico))
                    db.Entry(this).State = System.Data.Entity.EntityState.Modified;
                else
                    db.Entry(this).State = System.Data.Entity.EntityState.Added;

                db.SaveChanges();
            }
        }
    }
}
