using System.Collections.Generic;
using System.Linq;
using HlidacStatu.Lib.Data;
using HlidacStatu.Lib.Data.OsobyES;
using Devmasters.Core;

namespace PeopleLoader
{
    class Program
    {

        public static Devmasters.Core.Batch.ActionProgressWriter progressWriter =
                new Devmasters.Core.Batch.ActionProgressWriter(0.1f, HlidacStatu.Lib.RenderTools.ProgressWriter_OutputFunc_EndIn);

        static void Main(string[] args)
        {
            //List<OsobaES> osoby = null;
            //OsobyEsService.Get("aaa");
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = HlidacStatu.Util.Consts.czCulture;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = HlidacStatu.Util.Consts.czCulture;

            using (DbEntities db = new DbEntities())
            {
                System.Console.WriteLine("Loading all records from db");

                var osoby = db.Osoba
                    .Where(m => m.Status > 0)
                    .ToList();

                //first fix people where is missing osoba.nameid
                foreach (var osoba in osoby.Where(o => o.NameId == null || o.NameId.Length < 1))
                {
                    osoba.Save();
                }

                System.Console.WriteLine("Converting all records");
                List<OsobaES> osobyES = new List<OsobaES>();
                Devmasters.Core.Batch.Manager.DoActionForAll<Osoba>(osoby,
                os =>
                {
                    var o = new OsobaES()
                    {
                        NameId = os.NameId,
                        BirthYear = os.Narozeni.HasValue ? (int?)os.Narozeni.Value.Year : null,
                        DeathYear = os.Umrti.HasValue ? (int?)os.Umrti.Value.Year : null,
                        ShortName = os.Jmeno + " " + os.Prijmeni,
                        FullName = os.FullName(false),
                        PoliticalParty = os.CurrentPoliticalParty(),
                        StatusText = os.StatusOsoby().ToString("G"),
                        Status = os.Status,
                        PoliticalFunctions = os.Events(ev => ev.Type == (int)OsobaEvent.Types.VolenaFunkce)
                            .Select(ev => ev.AddInfo).ToArray(),
                        PhotoUrl = os.HasPhoto() ? os.GetPhotoUrl() : null

                    };
                    osobyES.Add(o);

                    return new Devmasters.Core.Batch.ActionOutputData();
                }, null, progressWriter.Write, true, maxDegreeOfParallelism: 10);

                System.Console.WriteLine("Deleting all records");
                OsobyEsService.DeleteAll();
                foreach (var osoba in osobyES.Chunk(1000))
                {
                    System.Console.WriteLine($"Adding {osoba.Count()} records");
                    OsobyEsService.BulkSave(osoba);
                }
            }
        }
    }
}
