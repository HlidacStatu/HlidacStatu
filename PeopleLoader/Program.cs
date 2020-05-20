using System.Collections.Generic;
using System.Linq;
using HlidacStatu.Lib.Data;
using HlidacStatu.Lib.Data.OsobyES;

namespace PeopleLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            //List<OsobaES> osoby = null;

            using (DbEntities db = new DbEntities())
            {
                var osoby = db.Osoba
                    .Where(m => m.Status > 0)
                    .ToList();
                
                var osobyES = osoby.Select(os => new OsobaES()
                    {
                        NameId = os.NameId,
                        BirthYear = os.Narozeni.HasValue ? (int?) os.Narozeni.Value.Year : null,
                        ShortName = os.Jmeno + " " + os.Prijmeni,
                        FullName = os.FullName(false),
                        PoliticalParty = os.CurrentPoliticalParty(),
                        StatusText = os.StatusOsoby().ToString("G"),
                        Status = os.Status,
                        PoliticalFunctions = os.Events(ev => ev.Type == (int)OsobaEvent.Types.VolenaFunkce)
                            .Select(ev => ev.AddInfo).ToArray(),
                        PhotoUrl = os.HasPhoto()? os.GetPhotoUrl() : null

                    }).ToList();

                int i = 0;
                List<OsobaES> batch = new List<OsobaES>();
                foreach (var osoba in osobyES)
                {
                    batch.Add(osoba);
                    if(++i == 1000)
                    {
                        OsobyEsService.BulkSave(batch);
                        batch.Clear();
                        i = 0;
                    }
                }
                OsobyEsService.BulkSave(batch);
            }
        }
    }
}
