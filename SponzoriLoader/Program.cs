using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SponzoriLoader
{
    //enum SubjectOfInterest
    //{
    //    penizefo,
    //    bupfo,
    //    penizepo,
    //    buppo
    //}

    class Program
    {
        private static HttpClient _client = new HttpClient();

        private static string[] _addresses = new string[]
        {
            "https://zpravy.udhpsh.cz/export/vfz2017-index.json",
            "https://zpravy.udhpsh.cz/zpravy/vfz2018.json",
            "https://zpravy.udhpsh.cz/zpravy/vfz2019.json"
        };


        static async Task Main(string[] args)
        {
            var peopleDonations = new Donations();
            
            foreach(string indexUrl in _addresses)
            {
                var index = await LoadIndexAsync(indexUrl);
                string key = index.election.key;
                int year = GetYearFromText(key);

                foreach (var party in index.parties)
                {
                    IEnumerable<dynamic> files = party.files;
                    string penizeFoUrl = files.Where(f => f.subject == "penizefo").Select(f => f.url).FirstOrDefault();
                    await LoadDonorsAsync(penizeFoUrl, peopleDonations, party, year);
                    string nepenizeFoUrl = files.Where(f => f.subject == "bupfo").Select(f => f.url).FirstOrDefault();
                    await LoadDonorsAsync(nepenizeFoUrl, peopleDonations, party, year);
                }


            }

        }

        public static async Task<dynamic> LoadIndexAsync(string url)
        {
            string response = await _client.GetStringAsync(url);
            dynamic json = JsonConvert.DeserializeObject(response);
            return json;
        }

        public static async Task LoadDonorsAsync(string url, Donations donations, dynamic party, int year)
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
                    Amount = record.money,
                    ICO = party.ic,
                    Party = party.shortName,
                    Description = record.description,
                    Date = record.date ?? new DateTime(year, 1, 1)
                };
                
                donations.AddDonation(donor, gift);
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
