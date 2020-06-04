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

        static async Task Main(string[] args)
        {
            var peopleDonations = new Donations(new PersonDonorEqualityComparer());
            var companyDonations = new Donations(new CompanyDonorEqualityComparer());

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
                    await LoadPeopleDonationsAsync(penizeFoUrl, peopleDonations, party, year);
                    string nepenizeFoUrl = files.Where(f => f.subject == "bupfo").Select(f => f.url).FirstOrDefault();
                    await LoadPeopleDonationsAsync(nepenizeFoUrl, peopleDonations, party, year);
                    //firmy
                    string penizePoUrl = files.Where(f => f.subject == "penizepo").Select(f => f.url).FirstOrDefault();
                    await LoadCompanyDonationsAsync(penizePoUrl, companyDonations, party, year);
                    string nepenizePoUrl = files.Where(f => f.subject == "buppo").Select(f => f.url).FirstOrDefault();
                    await LoadCompanyDonationsAsync(nepenizePoUrl, companyDonations, party, year);
                }


            }
            #endregion

            #region saving to db
            UploadPeopleDonations(peopleDonations);
            UploadCompanyDonations(companyDonations);

            #endregion
            // loop to upload events


        }

        public static async Task<dynamic> LoadIndexAsync(string url)
        {
            string response = await _client.GetStringAsync(url);
            dynamic json = JsonConvert.DeserializeObject(response);
            return json;
        }

        /// <summary>
        /// Loads all people donations from web
        /// </summary>
        /// <param name="url"></param>
        /// <param name="donations"></param>
        /// <param name="party"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static async Task LoadPeopleDonationsAsync(string url, Donations donations, dynamic party, int year)
        {
            dynamic donationRecords = await LoadIndexAsync(url);
            foreach (var record in donationRecords)
            {
                Donor donor = new Donor()
                {
                    City = record.addrCity,
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
                    Date = record.date ?? new DateTime(year, 1, 1)
                };

                donations.AddDonation(donor, gift);
            }
        }

        /// <summary>
        /// Loads all company donations from web
        /// </summary>
        /// <param name="url"></param>
        /// <param name="donations"></param>
        /// <param name="party"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static async Task LoadCompanyDonationsAsync(string url, Donations donations, dynamic party, int year)
        {
            dynamic donationRecords = await LoadIndexAsync(url);
            foreach (var record in donationRecords)
            {
                Donor donor = null;
                try
                {
                    donor = new Donor()
                    {
                        City = record.addrCity,
                        Name = record.company,
                        CompanyId = record.companyId
                    };
                }
                catch (Exception)
                {
                    donor = new Donor()
                    {
                        City = record.addrCity,
                        Name = record.company,
                    };
                    Console.WriteLine($"Špatný formát IČO: [{record.companyId}], url:[{url}]");
                }

                Gift gift = new Gift()
                {
                    Amount = record.money ?? record.value,
                    ICO = party.ic,
                    Party = party.longName,
                    Description = record.description,
                    Date = record.date ?? new DateTime(year, 1, 1)
                };

                donations.AddDonation(donor, gift);
            }
        }

        /// <summary>
        /// Uploads new donations to OsobaEvent table
        /// </summary>
        /// <param name="donations"></param>
        public static void UploadPeopleDonations(Donations donations)
        {
            foreach (var personDonations in donations.GetDonations())
            {
                var donor = personDonations.Key;

                Osoba osoba = Osoba.GetOrCreateNew(donor.TitleBefore, donor.Name, donor.Surname, donor.TitleAfter,
                                                   donor.DateOfBirth, Osoba.StatusOsobyEnum.Sponzor, _user);

                var osobaEvents = osoba.NoFilteredEvents(ev => ev.Type == (int)OsobaEvent.Types.Sponzor).ToList();

                foreach (var donation in personDonations.Value)
                {
                    var eventToRemove = osobaEvents.Where(oe => oe.AddInfoNum == donation.Amount
                                            && oe.AddInfo == donation.ICO
                                            && oe.DatumOd.HasValue
                                            && oe.DatumOd.Value.Year == donation.Date.Year).FirstOrDefault();
                    if (eventToRemove is null)
                    {
                        // add event
                        var newEvent = new OsobaEvent()
                        {
                            Organizace = ParseTools.NormalizaceStranaShortName(donation.Party),
                            DatumOd = donation.Date,
                            AddInfoNum = donation.Amount,
                            AddInfo = donation.ICO,
                            Zdroj = _user,
                            Note = donation.Description
                        };
                        osoba.AddOrUpdateEvent(newEvent, _user, checkDuplicates: false);
                    }
                    else
                    {
                        osobaEvents.Remove(eventToRemove);
                    }

                }

            }
        }

        /// <summary>
        /// Uploads new donations to FirmaEvent table
        /// </summary>
        /// <param name="donations"></param>
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

                var firmaEvents = firma.Events(ev => ev.Type == (int)FirmaEvent.Types.Sponzor).ToList();

                foreach (var donation in companyDonations.Value)
                {
                    var eventToRemove = firmaEvents.Where(oe => oe.AddInfoNum == donation.Amount
                                            && oe.Description == donation.ICO
                                            && oe.DatumOd.HasValue
                                            && oe.DatumOd.Value.Year == donation.Date.Year).FirstOrDefault();
                    if (eventToRemove is null)
                    {
                        // add event
                        var newEvent = new FirmaEvent()
                        {
                            AddInfo = ParseTools.NormalizaceStranaShortName(donation.Party),
                            DatumOd = donation.Date,
                            AddInfoNum = donation.Amount,
                            Description = donation.ICO,
                            Zdroj = _user,
                            Note = donation.Description
                        };
                        firma.AddOrUpdateEvent(newEvent, _user, checkDuplicates: false);
                    }
                    else
                    {
                        firmaEvents.Remove(eventToRemove);
                    }
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
