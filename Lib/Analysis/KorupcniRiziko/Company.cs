using HlidacStatu.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public class Company
    {
        public Company(string name, string ico, decimal? value)
            : this(name, ico)
        {
            Value4Sort = value;
        }
        public Company(string name, string ico)
        {
            Name = name;
            Ico = ico;
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
            foreach(var kindexRecord in KIndex.YieldExistingKindexes())
            {
                companies.Add(kindexRecord.Ico, new Company(kindexRecord.Jmeno, kindexRecord.Ico));
            }

            return companies;
        }

        public static Dictionary<string, Company> GetCompanies()
        {
            return CachedCompanies.Get();
        }

        public static IEnumerable<Company> FullTextSearch(string search, int take = 50)
        {

            IEnumerable<Company> fullSearchNames = GetCompanies().Values
                .Where(c => c.Name.ToLower().StartsWith(search.ToLower()))
                .Take(take);

            IEnumerable<Company> totalResult = fullSearchNames;
            if (totalResult.Count() >= take)
                return totalResult;

            var fullSearchIcos = GetCompanies()
                .Where(c => c.Key.StartsWith(search))
                .Select(c => c.Value)
                .Take(take);
            totalResult = totalResult.Union(fullSearchIcos).Take(take);
            if (totalResult.Count() >= take)
                return totalResult;

            var tokenizedSearchInput = Tokenize(search);

            var tokenSearchAll = GetCompanies().Values
                .Where(c => 
                    tokenizedSearchInput.All(txt => 
                        c.Tokens.Any(tkn => 
                            tkn.StartsWith(txt)
                        )
                    )
                )
                .Take(take);
            totalResult = totalResult.Union(tokenSearchAll).Take(take);
            if (totalResult.Count() >= take)
                return totalResult;

            var tokenSearchAny = GetCompanies().Values
                .Where(c =>
                    tokenizedSearchInput.Any(txt =>
                        c.Tokens.Any(tkn =>
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
        public decimal? Value4Sort { get; set; } = null;

        private static string[] Tokenize(string input)
        {
            return input.ToLower().KeepLettersNumbersAndSpace().RemoveAccents().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        
    }
}
