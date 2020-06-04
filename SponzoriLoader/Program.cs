using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HlidacStatu.Lib.Data;
using HlidacStatu.Util;

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
                    await LoadDonorsPersonAsync(penizeFoUrl, peopleDonations, party, year);
                    string nepenizeFoUrl = files.Where(f => f.subject == "bupfo").Select(f => f.url).FirstOrDefault();
                    await LoadDonorsPersonAsync(nepenizeFoUrl, peopleDonations, party, year);
                    //firmy
                    string penizePoUrl = files.Where(f => f.subject == "penizepo").Select(f => f.url).FirstOrDefault();
                    await LoadDonorsCompanyAsync(penizePoUrl, companyDonations, party, year);
                    string nepenizePoUrl = files.Where(f => f.subject == "buppo").Select(f => f.url).FirstOrDefault();
                    await LoadDonorsCompanyAsync(nepenizePoUrl, companyDonations, party, year);
                }


            }
            #endregion

            #region saving to db
            UploadPeopleDonations(peopleDonations);

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
        /// Loads all donations from web
        /// </summary>
        /// <param name="url"></param>
        /// <param name="donations"></param>
        /// <param name="party"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static async Task LoadDonorsPersonAsync(string url, Donations donations, dynamic party, int year)
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

        public static async Task LoadDonorsCompanyAsync(string url, Donations donations, dynamic party, int year)
        {
            dynamic donationRecords = await LoadIndexAsync(url);
            foreach (var record in donationRecords)
            {
                Donor donor = new Donor()
                {
                    City = record.addrCity,
                    Name = record.company,
                    CompanyId = record.companyId
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
