using Devmasters.Core;
using HlidacStatu.Lib.Data.External.NespolehlivyPlatceDPH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;


namespace HlidacStatu.Lib.Data
{
    public partial class NespolehlivyPlatceDPH
    {
        public static Dictionary<string, NespolehlivyPlatceDPH> GetAllFromDb()
        {
            //Dictionary<string, NespolehlivyPlatceDPH> = new Dictionary<string, NespolehlivyPlatceDPH>();
            using (Lib.Data.DbEntities db = new DbEntities())
            {
                return db.NespolehlivyPlatceDPH.AsNoTracking().ToDictionary(k => k.Ico, k => k);
            }
        }
        public static Dictionary<string, NespolehlivyPlatceDPH> GetDataFromGFR()
        {
            var client = new HlidacStatu.Lib.Data.External.NespolehlivyPlatceDPH.rozhraniCRPDPHClient();
            InformaceOPlatciType[] firmy = null;
            var result = client.getSeznamNespolehlivyPlatce(out firmy);
            if (result.statusCode == 0)
            {
                return firmy
                    .Select(f =>
                        new NespolehlivyPlatceDPH()
                        {
                            Ico = f.dic.Trim(),
                            FromDate = f.datumZverejneniNespolehlivostiSpecified ? f.datumZverejneniNespolehlivosti : (DateTime?)null
                        })
                    .ToDictionary(k => k.Ico, v => v);
            }
            else
                return null;
        }

        public static void UpdateData()
        {
            var newData = GetDataFromGFR();
            if (newData != null)
            {
                using (Lib.Data.DbEntities db = new DbEntities())
                {
                    foreach (var key in newData.Keys)
                    {
                        var exist = db.NespolehlivyPlatceDPH.Where(i => i.Ico == key).FirstOrDefault();
                        if (exist != null)
                        {
                            if (exist.FromDate.HasValue && newData[key].FromDate.HasValue && exist.FromDate != newData[key].FromDate)
                                exist.FromDate = newData[key].FromDate;

                            if (exist.ToDate.HasValue) //is back on the list, remove end
                                exist.ToDate = null;
                        }
                        else
                        {
                            var newItem = new NespolehlivyPlatceDPH()
                            {
                                Ico = newData[key].Ico,
                                FromDate = newData[key].FromDate
                            };
                            db.NespolehlivyPlatceDPH.Add(newItem);
                        }

                    }
                    db.SaveChanges();
                    //check ico removed from newData
                    var inDb = db.NespolehlivyPlatceDPH.Where(m => m.ToDate == null).Select(m => m.Ico).ToArray();
                    var missingInNewData = inDb.Except(newData.Keys);
                    foreach (var ico in missingInNewData)
                    {
                        var exist = db.NespolehlivyPlatceDPH.Where(i => i.Ico == ico).FirstOrDefault();
                        if (exist != null)
                        {
                            if (exist.ToDate == null)
                                exist.ToDate = DateTime.Now.Date;
                        }

                    }
                    db.SaveChanges();

                }
            }
        }

    }
}
