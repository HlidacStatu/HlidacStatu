using Devmasters.Enums;
using HlidacStatu.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HlidacStatu.Lib.Data
{
    public class Autocomplete : IEquatable<Autocomplete>
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

            try
            {
                var results = LoadCompanies();
                results.AddRange(LoadStateCompanies());
                results.AddRange(LoadAuthorities());
                results.AddRange(LoadCities());
                results.AddRange(LoadPeople());
                results.AddRange(LoadOblasti());
                results.AddRange(LoadOperators());

                return results;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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

        //firmy
        private static List<Autocomplete> LoadCompanies()
        {
            // Kod_PF < 110  - cokoliv co nejsou fyzické osoby, podnikatelé
            // Podnikatelé nejsou zařazeni, protože je jich poté moc a vznikají tam duplicity
            string sql = $@"select Jmeno, ICO, KrajId 
                             from Firma 
                            where IsInRS = 1 
                              AND LEN(ico) = 8 
                              AND Kod_PF > 110
                              AND (Typ is null
                                OR Typ={(int) Firma.TypSubjektu.Soukromy});";
            var results = DirectDB.GetList<string, string, string>(sql)
                .AsParallel()
                .Select(f => new Autocomplete()
                {
                    Id = $"ico:{f.Item2}",
                    Text = f.Item1,
                    Type = "firma",
                    Description = FixKraj(f.Item3),
                    Priority = 0,
                    ImageElement = "<i class='fas fa-industry-alt'></i>"
                }).ToList();
            return results;
        }
        
        //státní firmy
        private static List<Autocomplete> LoadStateCompanies()
        {
            string sql = $@"select Jmeno, ICO, KrajId 
                             from Firma 
                            where IsInRS = 1 
                              AND LEN(ico) = 8 
                              AND Kod_PF > 110
                              AND Typ={(int) Firma.TypSubjektu.PatrimStatu};";
            var results = DirectDB.GetList<string, string, string>(sql)
                .AsParallel()
                .Select(f => new Autocomplete()
                {
                    Id = $"ico:{f.Item2}",
                    Text = f.Item1,
                    Type = "státní firma",
                    Description = FixKraj(f.Item3),
                    Priority = 1,
                    ImageElement = "<i class='fas fa-industry-alt'></i>"
                }).ToList();
            return results;
        }
        
        //úřady
        private static List<Autocomplete> LoadAuthorities()
        {
            string sql = $@"select Jmeno, ICO, KrajId 
                             from Firma 
                            where IsInRS = 1 
                              AND LEN(ico) = 8 
                              AND Kod_PF > 110
                              AND Typ={(int) Firma.TypSubjektu.Ovm};";
            var results = DirectDB.GetList<string, string, string>(sql)
                .AsParallel()
                .Select(f => new Autocomplete()
                {
                    Id = $"ico:{f.Item2}",
                    Text = f.Item1,
                    Type = "úřad",
                    Description = FixKraj(f.Item3),
                    Priority = 2,
                    ImageElement = "<i class='fas fa-university'></i>"
                }).ToList();
            return results;
        }
        
        //obce
        private static List<Autocomplete> LoadCities()
        {
            string sql = $@"select Jmeno, ICO, KrajId 
                             from Firma 
                            where IsInRS = 1 
                              AND LEN(ico) = 8
                              AND Stav_subjektu = 1 
                              AND Typ={(int) Firma.TypSubjektu.Obec};";
            var results = DirectDB.GetList<string, string, string>(sql)
                .AsParallel()
                .SelectMany(f =>
                {
                    var synonyms = new Autocomplete[2];
                    synonyms[0] = new Autocomplete()
                    {
                        Id = $"ico:{f.Item2}",
                        Text = f.Item1,
                        Type = "obec",
                        Description = FixKraj(f.Item3),
                        Priority = 2,
                        ImageElement = "<i class='fas fa-industry-alt'></i>"
                    };

                    synonyms[1] = (Autocomplete) synonyms[0].MemberwiseClone();
                    string synonymText = Regex.Replace(f.Item1,
                        @"^(Město|Městská část|Městys|Obec|Statutární město) ?",
                        "",
                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                    synonyms[1].Text = synonymText;
                    return synonyms;
                }).ToList();
            
            return results;
        }

        //lidi
        private static List<Autocomplete> LoadPeople()
        {
            List<Autocomplete> results;
            using (DbEntities db = new DbEntities())
            {
                results = db.Osoba
                    .Where(o => o.Status == (int)Osoba.StatusOsobyEnum.Politik
                        || o.Status == (int)Osoba.StatusOsobyEnum.Sponzor)
                    //.Take(100).ToList()
                    .AsParallel()
                    .SelectMany(o =>
                    {
                        var synonyms = new Autocomplete[2];
                        synonyms[0] = new Autocomplete()
                        {
                            Id = $"osobaid:{o.NameId}",
                            Text = $"{o.Prijmeni} {o.Jmeno}{AppendTitle(o.TitulPred, o.TitulPo)}",
                            Priority = o.Status == (int) Osoba.StatusOsobyEnum.Politik ? 2 : 0,
                            Type = o.StatusOsoby().ToNiceDisplayName(),
                            ImageElement = $"<img src='{o.GetPhotoUrl(false)}' />",
                            Description = InfoFact.RenderInfoFacts(
                                o.InfoFacts().Where(i => i.Level != InfoFact.ImportanceLevel.Stat).ToArray(),
                                2, true, false, "", "{0}", false)
                        };

                        synonyms[1] = (Autocomplete) synonyms[0].MemberwiseClone();
                        synonyms[1].Text = $"{o.Jmeno} {o.Prijmeni}{AppendTitle(o.TitulPred, o.TitulPo)}";
                        return synonyms;
                    }).ToList();
            }
            return results;
        }

        private static string AppendTitle(string titulPred, string titulPo)
        {
            var check = (titulPred + titulPo).Trim();
            if (string.IsNullOrWhiteSpace(check))
                return "";
            
            var sb = new StringBuilder();
            sb.Append(" (");
            sb.Append(titulPred);
            sb.Append(" ");
            sb.Append(titulPo);
            sb.Append(")");

            return sb.ToString();
        }

        private static IEnumerable<Autocomplete> LoadOblasti()
        {
            var enumType = typeof(Smlouva.SClassification.ClassificationsTypes);
            var enumNames = Enum.GetNames(enumType);

            var oblasti = enumNames.SelectMany(e =>
            {
                var synonyms = new Autocomplete[2];
                synonyms[0] = new Autocomplete()
                {
                    Id = $"oblast:{e}",
                    Text = $"oblast: {GetNiceNameForEnum(enumType, e)}",
                    Priority = 3,
                    Type = "Oblast smluv - upřesnění dotazu",
                    Description = $"Oblast {GetNiceNameForEnum(enumType, e)} - smlouvy z registru smluv",
                    ImageElement = $"<img src='/content/hlidacloga/Hlidac-statu-ctverec-norm.png' />",
                };

                synonyms[1] = (Autocomplete) synonyms[0].MemberwiseClone();
                synonyms[1].Text = $"oblast:{e}";
                return synonyms;
            });
            
            return oblasti;
        }

        private static IEnumerable<Autocomplete> LoadOperators()
        {
            return new List<Autocomplete>()
            {
                new Autocomplete()
                {
                    Id = $"OR",
                    Text = $"OR",
                    Type = "Logické operátory",
                    Description = $"Logický operátor OR (NEBO)",
                    ImageElement = $"<img src='/content/hlidacloga/Hlidac-statu-ctverec-norm.png' />",
                    Priority = 3,
                },
                new Autocomplete()
                {
                    Id = $"AND",
                    Text = $"AND",
                    Type = "Logické operátory",
                    Description = $"Logický operátor AND (A)",
                    ImageElement = $"<img src='/content/hlidacloga/Hlidac-statu-ctverec-norm.png' />",
                    Priority = 3,
                }
            };
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

        public bool Equals(Autocomplete other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Id, other.Id, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Autocomplete) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(Id) : 0);
        }
    }


}
