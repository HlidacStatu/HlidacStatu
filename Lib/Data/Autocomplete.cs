using Devmasters.Enums;
using HlidacStatu.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Data
{
    public class Autocomplete
    {
        public string Id { get; set; }
        [FullTextSearch.Search]
        public string Text { get; set; }
        public string ImageElement { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }

        /// <summary>
        /// Generates autocomplete data
        /// ! Slow, long running operation
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Autocomplete> GenerateAutocomplete()
        {
            var results = LoadCompanies();

            results.AddRange(LoadPeople());

            results.AddRange(LoadOblasti());

            return results;
        }

        //používá se v administraci eventů pro naše politiky
        public static IEnumerable<Autocomplete> GenerateAutocompleteFirmyOnly()
        {
            string sql = "select distinct Jmeno, ICO from Firma where LEN(ico) = 8 AND Kod_PF > 110;";
            var results = DirectDB.GetList<string, string>(sql)
                .Select(f => new Autocomplete()
                {
                    Id = f.Item2,
                    Text = f.Item1
                }).ToList();
            return results;
        }

        private static List<Autocomplete> LoadCompanies()
        {
            // Kod_PF < 110  - cokoliv co nejsou fyzické osoby, podnikatelé
            // Podnikatelé nejsou zařazeni, protože je jich poté moc a vznikají tam duplicity
            
            string sql = "select Jmeno, ICO from Firma where IsInRS = 1 AND LEN(ico) = 8 AND Kod_PF > 110;";
            var results = DirectDB.GetList<string, string>(sql)
                .Select(f => Firma.FromIco(f.Item2))
                .AsParallel()
                .Select(f => new Autocomplete()
                {
                    Id = $"ico:{f.ICO}",
                    Text = f.Jmeno,
                    Type = f.JsemOVM() ? "úřad" : "firma",
                    Description = FixKraj(f.KrajId),
                    ImageElement = "<i class='fas fa-industry-alt'></i>"
                }).ToList();
            return results;
        }

        private static List<Autocomplete> LoadPeople()
        {
            var results = new List<Autocomplete>();
            using (DbEntities db = new DbEntities())
            {
                results = db.Osoba
                    .Where(o => o.Status == (int)Osoba.StatusOsobyEnum.Politik
                        || o.Status == (int)Osoba.StatusOsobyEnum.Sponzor)
                    //.Take(100).ToList()
                    .AsParallel()
                    .Select(o => new Autocomplete()
                    {
                        Id = $"osobaid:{o.NameId}",
                        Text = $"{o.Prijmeni} {o.Jmeno} ({o.TitulPred} {o.TitulPo})",
                        Priority = o.Status == (int)Osoba.StatusOsobyEnum.Politik ? 1 : 0,
                        Type = o.StatusOsoby().ToNiceDisplayName(),
                        ImageElement = $"<img src='{o.GetPhotoUrl(false)}' />",
                        Description = InfoFact.RenderInfoFacts(
                            o.InfoFacts().Where(i => i.Level != InfoFact.ImportanceLevel.Stat).ToArray(),
                            2, true, false, "", "{0}", false)
                    }).ToList();
            }
            return results;
        }

        private static IEnumerable<Autocomplete> LoadOblasti()
        {
            var enumType = typeof(Smlouva.SClassification.ClassificationsTypes);
            var enumNames = Enum.GetNames(enumType);

            var oblasti = enumNames.Select(e => new Autocomplete()
            {
                Id = $"oblast:{e}",
                Text = $"oblast: {GetNiceNameForEnum(enumType, e)}",
                Type = "Oblast smluv - upřesnění dotazu",
                Description = $"Oblast {GetNiceNameForEnum(enumType, e)} - smlouvy z registru smluv",
                ImageElement = "<i class='fas fa-search'></i>"
            });
            return oblasti;
        }

        private static string GetNiceNameForEnum(Type enumType, string enumValue)
        {
            return ((Smlouva.SClassification.ClassificationsTypes)Enum.Parse(enumType, enumValue)).ToNiceDisplayName();
        }

        private static string FixKraj(string krajId)
        {
            if (krajId is null)
                return "neznámý kraj";

            return CZ_Nuts.Kraje.TryGetValue(krajId, out string kraj) ? kraj : krajId;
        }
    }


}
