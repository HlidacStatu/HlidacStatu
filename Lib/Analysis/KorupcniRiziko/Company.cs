using HlidacStatu.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public class Company
    {
        public Company(string name, string ico)
        {
            Name = name;
            Tokens = Tokenize($"{name} {ico}");
        }

        public static Devmasters.Cache.V20.File.FileCache<Dictionary<string,Company>> CachedCompanies = 
            new Devmasters.Cache.V20.File.FileCache<Dictionary<string, Company>>(
                Lib.Init.WebAppDataPath, TimeSpan.Zero, "KIndexCompanies",
                    (o) =>
                    {
                        return ListCompanies();
                    });

        private static Dictionary<string,Company> ListCompanies()
        {
            Dictionary<string, Company> companies = new Dictionary<string, Company>();
            int i = 0;
            foreach(var kindexRecord in KIndex.YieldExistingKindexes())
            {
                //Console.WriteLine($"{i++} - {kindexRecord.Jmeno}");
                companies.Add(kindexRecord.Ico, new Company(kindexRecord.Jmeno, kindexRecord.Ico));
            }

            return companies;
        }

        public static Dictionary<string, Company> GetCompanies()
        {
            return CachedCompanies.Get();
        }

        public static IEnumerable<KeyValuePair<string,Company>> FullTextSearch(string search, int take = 50)
        {

            IEnumerable<KeyValuePair<string, Company>> fullSearchNames = GetCompanies()
                .Where(c => c.Value.Name.ToLower().StartsWith(search.ToLower()))
                .Take(take);

            IEnumerable<KeyValuePair<string, Company>> totalResult = fullSearchNames;
            if (totalResult.Count() >= take)
                return totalResult;

            var fullSearchIcos = GetCompanies()
                .Where(c => c.Key.StartsWith(search))
                .Take(take);
            totalResult = totalResult.Union(fullSearchIcos).Take(take);
            if (totalResult.Count() >= take)
                return totalResult;

            var tokenizedSearchInput = Tokenize(search);

            var tokenSearchAll = GetCompanies()
                .Where(c => 
                    tokenizedSearchInput.All(txt => 
                        c.Value.Tokens.Any(tkn => 
                            tkn.StartsWith(txt)
                        )
                    )
                )
                .Take(take);
            totalResult = totalResult.Union(tokenSearchAll).Take(take);
            if (totalResult.Count() >= take)
                return totalResult;

            var tokenSearchAny = GetCompanies()
                .Where(c =>
                    tokenizedSearchInput.Any(txt =>
                        c.Value.Tokens.Any(tkn =>
                            tkn.StartsWith(txt)
                        )
                    )
                )
                .Take(take);
            totalResult = totalResult.Union(tokenSearchAny).Take(take);
            return totalResult;
        }


        public string Name { get; set; }
        public string Ico { get; set; }
        private string[] Tokens { get; set; }

        private static string[] Tokenize(string input)
        {
            return input.ToLower().KeepLettersNumbersAndSpace().RemoveAccents().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        
    }
}
