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
            Ico = ico;
            Tokens = Tokenize($"{name} {ico}");
        }

        public static Devmasters.Cache.V20.File.FileCache<Company[]> CachedCompanies = 
            new Devmasters.Cache.V20.File.FileCache<Company[]>(
                Lib.Init.WebAppDataPath, TimeSpan.Zero, "KIndexCompanies",
                    (o) =>
                    {
                        return ListCompanies().ToArray();
                    });

        private static IEnumerable<Company> ListCompanies()
        {
            List<Company> companies = new List<Company>();
            int i = 0;
            foreach(var kindexRecord in KIndex.YieldExistingKindexes())
            {
                Console.WriteLine($"{i++} - {kindexRecord.Jmeno}");
                companies.Add(new Company(kindexRecord.Jmeno, kindexRecord.Ico));
            }

            return companies;
        }

        public static IEnumerable<Company> GetCompanies()
        {
            return CachedCompanies.Get();
        }

        public static IEnumerable<Company> FullTextSearch(string search, int take = 50)
        {
            if (string.IsNullOrWhiteSpace(search))
                return new List<Company>();
                
            var fullSearchNames = GetCompanies()
                .Where(c => c.Name.StartsWith(search, StringComparison.InvariantCultureIgnoreCase))
                .Take(take);
            IEnumerable<Company> totalResult = fullSearchNames;
            if (totalResult.Count() >= take)
                return totalResult;

            var fullSearchIcos = GetCompanies()
                .Where(c => c.Ico.StartsWith(search, StringComparison.InvariantCultureIgnoreCase))
                .Take(take);
            totalResult = totalResult.Union(fullSearchIcos).Take(take);
            if (totalResult.Count() >= take)
                return totalResult;

            var tokenizedSearchInput = Tokenize(search);

            var tokenSearchAll = GetCompanies()
                .Where(c => 
                    tokenizedSearchInput.All(txt => 
                        c.Tokens.Any(tkn => 
                            tkn.StartsWith(txt, StringComparison.InvariantCultureIgnoreCase)
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
                        c.Tokens.Any(tkn =>
                            tkn.StartsWith(txt, StringComparison.InvariantCultureIgnoreCase)
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
