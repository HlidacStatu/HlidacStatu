using HlidacStatu.Lib.Data;
using HlidacStatu.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SponzoriLoader
{

    class Program
    {
        private static readonly HttpClient _client = new HttpClient();

        private static readonly string[] _addresses = new string[]
        {
            "https://zpravy.udhpsh.cz/export/vfz2017-index.json",
            "https://zpravy.udhpsh.cz/zpravy/vfz2018.json",
            "https://zpravy.udhpsh.cz/zpravy/vfz2019.json"
        };

        private static readonly string _user = "sponzorLoader";
        private static readonly string _zdroj = "https://www.udhpsh.cz/vyrocni-financni-zpravy-stran-a-hnuti";

        private static Dictionary<string, string> _partyNames;

        static async Task Main(string[] args)
        {
            _partyNames = LoadPartyNames();
            var peopleDonations = new Donations(new DonorEqualityComparer());
            var companyDonations = new Donations(new DonorEqualityComparer());

            #region loading from web
            foreach (string indexUrl in _addresses)
            {
                var index = await LoadIndexAsync(indexUrl);
                string key = index.election.key;
                int year = GetYearFromText(key);

                foreach (var party in index.parties)
                {
                    IEnumerable<dynamic> files = party.files;
                    // osoby
                    string penizeFoUrl = files.Where(f => f.subject == "penizefo").Select(f => f.url).FirstOrDefault();
                    await LoadDonationsAsync(penizeFoUrl, peopleDonations, party, year);
                    string nepenizeFoUrl = files.Where(f => f.subject == "bupfo").Select(f => f.url).FirstOrDefault();
                    await LoadDonationsAsync(nepenizeFoUrl, peopleDonations, party, year);
                    //firmy
                    string penizePoUrl = files.Where(f => f.subject == "penizepo").Select(f => f.url).FirstOrDefault();
                    await LoadDonationsAsync(penizePoUrl, companyDonations, party, year);
                    string nepenizePoUrl = files.Where(f => f.subject == "buppo").Select(f => f.url).FirstOrDefault();
                    await LoadDonationsAsync(nepenizePoUrl, companyDonations, party, year);
                }


            }
            #endregion

            #region saving to db
            UploadPeopleDonations(peopleDonations);
            UploadCompanyDonations(companyDonations);

            #endregion
        }

        public static async Task<dynamic> LoadIndexAsync(string url)
        {
            string response = await _client.GetStringAsync(url);
            dynamic json = JsonConvert.DeserializeObject(response);
            return json;
        }

        public static Dictionary<string,string> LoadPartyNames()
        {
            using (DbEntities db = new DbEntities())
            {
                return db.ZkratkaStrany.ToDictionary(ks => ks.ICO, es => es.KratkyNazev);
            }
        }

        public static string NormalizePartyName(string name, string ico)
        {
            if (_partyNames.TryGetValue(ico, out string normalizedName))
            {
                return normalizedName;
            }
            return ParseTools.NormalizaceStranaShortName(name);

        }

        /// <summary>
        /// Loads all people donations from web
        /// </summary>
        public static async Task LoadDonationsAsync(string url, Donations donations, dynamic party, int year)
        {
            dynamic donationRecords = await LoadIndexAsync(url);
            foreach (var record in donationRecords)
            {
                Donor donor = new Donor()
                {
                    City = record.addrCity,
                    CompanyId = record.companyId ?? 0,
                    Name = record.firstName,
                    Surname = record.lastName,
                    TitleBefore = record.titleBefore,
                    TitleAfter = record.titleAfter,
                    DateOfBirth = record.birthDate
                };
                Gift gift = new Gift()
                {
                    Amount = record.money ?? record.value,
                    ICO = party.ic,
                    Party = party.longName,
                    Description = record.description,
                    Date = record.date ?? new DateTime(year, 1, 1),
                    GiftType = (record.money is null) ? Sponzoring.TypDaru.NefinancniDar : Sponzoring.TypDaru.FinancniDar
                };

                donations.AddDonation(donor, gift);
            }
        }


        /// <summary>
        /// Uploads new donations to OsobaEvent table
        /// </summary>
        public static void UploadPeopleDonations(Donations donations)
        {
            foreach (var personDonations in donations.GetDonations())
            {
                var donor = personDonations.Key;

                Osoba osoba = Osoba.GetOrCreateNew(donor.TitleBefore, donor.Name, donor.Surname, donor.TitleAfter,
                                                   donor.DateOfBirth, Osoba.StatusOsobyEnum.Sponzor, _user);

                // Výjimka pro Radek Jonke 24.12.1970
                if (osoba.Jmeno == "Radek"
                    && osoba.Prijmeni == "Jonke"
                    && osoba.Narozeni != null
                    && osoba.Narozeni.Value.Year == 1970
                    && osoba.Narozeni.Value.Month == 12
                    && osoba.Narozeni.Value.Day == 24)
                {
                    continue;
                }

                foreach (var donation in personDonations.Value)
                {
                    // add event
                    var sponzoring = new Sponzoring()
                    {
                        DarovanoDne = donation.Date,
                        Hodnota = donation.Amount,
                        IcoPrijemce = donation.ICO,
                        Zdroj = _zdroj,
                        Popis = donation.Description,
                        Typ = (int)donation.GiftType
                    };
                    osoba.AddOrUpdateSponsoring(sponzoring, _user);
                }

            }
        }

        /// <summary>
        /// Uploads new donations to FirmaEvent table
        /// </summary>
        public static void UploadCompanyDonations(Donations donations)
        {
            foreach (var companyDonations in donations.GetDonations())
            {
                var donor = companyDonations.Key;

                Firma firma = null;
                try
                {
                    firma = Firma.FromIco(donor.CompanyId);
                }
                catch (Exception)
                {
                }
                
                if (firma is null)
                {
                    Console.WriteLine($"Chybějící firma v db - ICO: {donor.CompanyId}, nazev: {donor.Name}");
                    continue;
                }

                foreach (var donation in companyDonations.Value)
                {
                    // add event
                    var sponzoring = new Sponzoring()
                    {
                        DarovanoDne = donation.Date,
                        Hodnota = donation.Amount,
                        IcoPrijemce = donation.ICO,
                        Zdroj = _zdroj,
                        Popis = donation.Description,
                        Typ = (int)donation.GiftType
                    };
                    firma.AddSponsoring(sponzoring, _user);
                }
            }
        }

        public static int GetYearFromText(string text)
        {
            string yearString = Regex.Match(text, @"\d+").Value;
            if (int.TryParse(yearString, out int year))
            {
                return year;
            }
            return 0;
        }
    }
}
