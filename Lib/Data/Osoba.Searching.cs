using Devmasters.Core;
using HlidacStatu.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HlidacStatu.Lib.Data
{
    public partial class Osoba
    {
        public static partial class Searching
        {
            public static Osoba GetByName(string jmeno, string prijmeni, DateTime narozeni)
            {
                return GetAllByName(jmeno, prijmeni, narozeni).FirstOrDefault();
            }
            public static IEnumerable<Osoba> GetAllByName(string jmeno, string prijmeni, DateTime? narozeni)
            {
                using (Lib.Data.DbEntities db = new Data.DbEntities())
                {
                    if (narozeni.HasValue)
                        return db.Osoba.AsNoTracking()
                        .Where(m =>
                            m.Jmeno == jmeno
                            && m.Prijmeni == prijmeni
                            && m.Narozeni == narozeni
                        ).ToArray();
                    else
                        return db.Osoba.AsNoTracking()
                            .Where(m =>
                                m.Jmeno == jmeno
                                && m.Prijmeni == prijmeni
                            ).ToArray();
                }
            }

            // search all people by name, surname and dob
            public static IEnumerable<Osoba> FindAll(string name, string birthYear, bool extendedSearch = true)
            {
                if (string.IsNullOrWhiteSpace(name)
                    && string.IsNullOrWhiteSpace(birthYear))
                {
                    return new Osoba[0];
                }

                string nquery = Devmasters.Core.TextUtil.RemoveDiacritics(name.NormalizeToPureTextLower());
                birthYear = birthYear?.Trim();
                bool isValidYear = int.TryParse(birthYear, out int validYear);
                // diakritika, velikost

                if (extendedSearch)
                {
                    using (Lib.Data.DbEntities db = new Data.DbEntities())
                    {
                        return db.Osoba.AsNoTracking()
                            .Where(m =>
                                (
                                    m.PrijmeniAscii.StartsWith(nquery) == true
                                   || m.JmenoAscii.StartsWith(nquery) == true
                                   || (m.JmenoAscii + " " + m.PrijmeniAscii).StartsWith(nquery) == true
                                   || (m.PrijmeniAscii + " " + m.JmenoAscii).StartsWith(nquery) == true
                                )
                                && (!isValidYear || m.Narozeni.Value.Year == validYear)
                            ).Take(200).ToArray();
                    }
                }
                else
                {
                    return Lib.StaticData.Politici.Get()
                        .Where(m =>
                           (
                               m.PrijmeniAscii.StartsWith(nquery, StringComparison.InvariantCultureIgnoreCase) == true
                              || m.JmenoAscii.StartsWith(nquery, StringComparison.InvariantCultureIgnoreCase) == true
                              || (m.JmenoAscii + " " + m.PrijmeniAscii).StartsWith(nquery, StringComparison.InvariantCultureIgnoreCase) == true
                              || (m.PrijmeniAscii + " " + m.JmenoAscii).StartsWith(nquery, StringComparison.InvariantCultureIgnoreCase) == true
                           )
                           && (!isValidYear || m.Narozeni.Value.Year == validYear)
                        )
                        .Take(200);
                }

            }

            public static IEnumerable<Osoba> GetPolitikByNameFtx(string jmeno, int maxNumOfResults = 1500)
            {
                string nquery = Devmasters.Core.TextUtil.RemoveDiacritics(jmeno.NormalizeToPureTextLower());

                var res = Lib.StaticData.PolitickyAktivni.Get()
               .Where(m => m != null)
               .Where(m =>
                   m.PrijmeniAscii?.StartsWith(nquery, StringComparison.InvariantCultureIgnoreCase) == true
                   || m.JmenoAscii?.StartsWith(nquery, StringComparison.InvariantCultureIgnoreCase) == true
                   || (m.JmenoAscii + " " + m.PrijmeniAscii)?.StartsWith(nquery, StringComparison.InvariantCultureIgnoreCase) == true
                   || (m.PrijmeniAscii + " " + m.JmenoAscii)?.StartsWith(nquery, StringComparison.InvariantCultureIgnoreCase) == true
                   )
               .OrderByDescending(m => m.Status)
               .ThenBy(m => m.Prijmeni)
               .Take(maxNumOfResults);
                return res;
            }

            public static IEnumerable<Osoba> GetPolitikByQueryFromFirmy(string jmeno, int maxNumOfResults = 50, IEnumerable<string> alreadyFoundFirmyIcos = null)
            {
                var res = new Osoba[] { };

                var firmy = alreadyFoundFirmyIcos;
                if (firmy == null)
                    firmy = Firma.Search.FindAllIco(jmeno, maxNumOfResults * 10);

                if (firmy != null && firmy.Count() > 0)
                {
                    Dictionary<int, int> osoby = new Dictionary<int, int>();
                    bool skipRest = false;
                    foreach (var fico in firmy)
                    {
                        if (skipRest)
                            break;

                        if (StaticData.FirmySVazbamiNaPolitiky_nedavne_Cache.Get().SoukromeFirmy.ContainsKey(fico))
                        {
                            foreach (var osobaId in StaticData.FirmySVazbamiNaPolitiky_nedavne_Cache.Get().SoukromeFirmy[fico])
                            {
                                if (osoby.ContainsKey(osobaId))
                                    osoby[osobaId]++;
                                else
                                    osoby.Add(osobaId, 1);

                                if (osoby.Count > maxNumOfResults)
                                {
                                    skipRest = true;
                                    break;
                                }
                            }
                        }

                        if (skipRest == false)
                        {
                            var fvazby = Firmy.Get(fico).AktualniVazby(Relation.AktualnostType.Nedavny);
                            foreach (var fv in fvazby)
                            {
                                if (fv.To.Type == Graph.Node.NodeType.Company)
                                {
                                    int osobaId = Convert.ToInt32(fv.To.Id);
                                    if (osoby.ContainsKey(osobaId))
                                        osoby[osobaId]++;
                                    else
                                        osoby.Add(osobaId, 1);

                                }
                                if (osoby.Count > maxNumOfResults)
                                {
                                    skipRest = true;
                                    break;
                                }

                                if (skipRest == false && StaticData.FirmySVazbamiNaPolitiky_nedavne_Cache.Get().SoukromeFirmy.ContainsKey(fv.To.Id))
                                {
                                    foreach (var osobaId in StaticData.FirmySVazbamiNaPolitiky_nedavne_Cache.Get().SoukromeFirmy[fv.To.Id])
                                    {
                                        if (osoby.ContainsKey(osobaId))
                                            osoby[osobaId]++;
                                        else
                                            osoby.Add(osobaId, 1);
                                        if (osoby.Count > maxNumOfResults)
                                        {
                                            skipRest = true;
                                            break;
                                        }
                                    }
                                }

                            }
                        }
                    }
                    res = osoby
                            .OrderByDescending(o => o.Value)
                            .Take(maxNumOfResults - res.Length)
                            .Select(m => Osoby.GetById.Get(m.Key))
                            .Where(m => m != null)
                            .Where(m => m.IsValid()) //not empty (nullObj from OsobaCache)
                            .ToArray();

                }
                return res;
            }

            public static List<int> PolitikImportanceOrder = new List<int>() { 3, 4, 2, 1, 0 };
            public static int[] PolitikImportanceEventTypes = new int[] { (int)OsobaEvent.Types.Politicka, (int)OsobaEvent.Types.PolitickaPracovni, (int)OsobaEvent.Types.VolenaFunkce };

            public static IEnumerable<Osoba> GetAllPoliticiFromText(string text)
            {
                var parsedName = Politici.FindCitations(text); //Lib.Validators.JmenoInText(text);

                var oo = parsedName.Select(nm => Osoby.GetByNameId.Get(nm))
                            .Where(o => o != null)
                            .OrderPoliticiByImportance();
                return oo;
            }
            public static IEnumerable<Osoba> GetBestPoliticiFromText(string text)
            {
                List<Osoba> uniqO = new List<Osoba>();
                var oo = GetAllPoliticiFromText(text);
                foreach (var o in oo)
                {
                    if (
                        !uniqO.Any(m => (m.NameId!=o.NameId && m.Jmeno == o.Jmeno && m.Prijmeni == o.Prijmeni))                        
                        )
                        uniqO.Add(o);
                }

                var ret = uniqO.OrderPoliticiByImportance();

                return ret;
            }

            public static Osoba GetFirstPolitikFromText(string text)
            {
                var osoby = GetBestPoliticiFromText(text);
                if (osoby.Count() == 0)
                    return null;

                return osoby.First();
            }

            public static Osoba GetByNameAscii(string jmeno, string prijmeni, DateTime narozeni)
            {
                return GetAllByNameAscii(jmeno, prijmeni, narozeni).FirstOrDefault();
            }
            public static IEnumerable<Osoba> GetAllByNameAscii(string jmeno, string prijmeni, DateTime? narozeni)
            {
                jmeno = Devmasters.Core.TextUtil.RemoveDiacritics(jmeno);
                prijmeni = Devmasters.Core.TextUtil.RemoveDiacritics(prijmeni);
                using (Lib.Data.DbEntities db = new Data.DbEntities())
                {
                    if (narozeni.HasValue)
                        return db.Osoba.AsNoTracking()
                            .Where(m =>
                                m.JmenoAscii == jmeno
                                && m.PrijmeniAscii == prijmeni
                                && (m.Narozeni == narozeni)
                            ).ToArray();
                    else
                        return db.Osoba.AsNoTracking()
                            .Where(m =>
                                m.JmenoAscii == jmeno
                                && m.PrijmeniAscii == prijmeni
                            ).ToArray();

                }
            }

        }
    }
}
