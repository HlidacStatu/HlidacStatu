using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devmasters.Core;
using System.Text.RegularExpressions;

namespace HlidacSmluv.Lib.Data
{
    [ElasticsearchType(Name = "person")]
    public class Person_XX
    {
        public static string[] TitulyPred = new string[] { "Akad. mal.", "Akad. malíř", "arch.", "Bc.", "Bc. et Bc.", "BcA.", "Dip Mgmt.", "Doc.", "Dott.", "Dr.", "DrSc.", "Ing.", "JUDr.", "Mag.", "Mg.A.", "MgA.", "Mgr.", "MSc.", "MUDr.", "MVDr.", "PaedDr.", "Ph.Dr.", "PharmDr.", "PhDr.", "Prof.", "RNDr.", "RSDr.", "ThDr.", "ThMgr." };

        public static string[] TitulyPo = new string[] { "BA", "BBA.", "CSc.", "D.E.A.", "DiS.", "Dr.h.c.", "DrSc.", "FACP", "jr.", "LL.M.", "MBA", "MD", "MEconSc.", "MgA.", "MIM", "MPA", "MPH", "MSc.", "Ph.D.", "Th.D." };

        [Nest.Text]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Keyword()]
        public string NamedId { get; set; }

        [Keyword()]
        public string FirmoId { get; set; }


        string _jmeno = "";


        [Keyword()]
        public string Jmeno
        {
            get
            {
                return _jmeno;
            }
            set
            {
                _jmeno = value;
                this.JmenoAscii = Devmasters.Core.TextUtil.RemoveDiacritics(_jmeno) ?? "";
                //if (!string.IsNullOrWhiteSpace(this.JmenoAscii) && !string.IsNullOrWhiteSpace(this.PrijmeniAscii))
                //    this.NamedId = GetUniqueNamedId();
            }
        }


        string _prijmeni = "";
        [Keyword()]
        public string Prijmeni
        {
            get
            {
                return _prijmeni;
            }
            set
            {
                _prijmeni = value;
                this.PrijmeniAscii = Devmasters.Core.TextUtil.RemoveDiacritics(_prijmeni) ?? "";
                //if (!string.IsNullOrWhiteSpace(this.JmenoAscii) && !string.IsNullOrWhiteSpace(this.PrijmeniAscii))
                //    this.NamedId = GetUniqueNamedId();
            }
        }



        [Keyword()]
        public string PrijmeniAscii { get; private set; } = "";
        [Keyword()]
        public string JmenoAscii { get; private set; } = "";

        [Keyword()]
        public string PredchoziJmeno { get; set; } //TODO
        [Keyword()]
        public string TitulPred { get; set; }
        [Keyword()]
        public string TitulPo { get; set; }
        [Date()]
        public DateTime? Narozeni { get; set; }
        [Date()]
        public DateTime? Umrti { get; set; }
        public Relation[] Vazby { get; set; }

        [Keyword()]
        public string Pohlavi { get; set; } //M/Z

        [Text()]
        public string Description { get; set; }

        [Keyword()]
        public string MoreInfoUrl { get; set; }

        [Object(Enabled = false)]
        public string Zdroj { get; set; }

        [Text()]
        public string PolitickaStrana { get; set; }

        [Date()]
        public DateTime? AktivniOd { get; set; }
        [Date()]
        public DateTime? AktivniDo { get; set; }

        [Keyword()]
        public string Mesto { get; set; }
        [Keyword()]
        public string Ulice { get; set; }
        [Keyword()]
        public string PSC { get; set; }
        public int Found { get; set; }
        public string Angazovanost { get; set; }

        [Date()]
        public DateTime? Odhad_vek_narozenOd { get; set; }
        [Date()]
        public DateTime? Odhad_vek_narozenDo { get; set; }

        public int VersionUpdate { get; set; }
        [Date()]
        public DateTime? LastUpdate { get; set; }

        public int PersonStatus { get; set; } = 0;

        public StatusOsobyEnum StatusOsoby() { return (StatusOsobyEnum)this.PersonStatus; }

        [ShowNiceDisplayName()]
        public enum StatusOsobyEnum
        {
            [NiceDisplayName("Politik")]
            Politik = 0,
            [NiceDisplayName("Bývalí politik")]
            ByvalyPolitik = 1,
            [NiceDisplayName("Osoba s vazbami na politiky")]
            VazbyNaPolitiky = 2,
            [NiceDisplayName("Nepolitická osoba")]
            Jiny = 99,
        }

        public Relation[] VazbyProICO(string ico)
        {
            List<Relation> ret = new List<Relation>();
            if (Vazby == null)
                return ret.ToArray();
            if (Vazby.Count() == 0)
                return ret.ToArray();
            return Vazby.Where(m => m.RelationToSubjectICO == ico).ToArray();
        }

        public string GetUniqueNamedId()
        {
            string basic = Devmasters.Core.TextUtil.ShortenText(this.JmenoAscii, 10) + "-" + Devmasters.Core.TextUtil.ShortenText(this.PrijmeniAscii, 15).Trim();
            basic = basic.ToLowerInvariant().Replace(" ","-");
            bool exists = true;
            int num = 0;
            do
            {

                var res = Lib.ES.Manager.GetESClient()
                        .Search<Person>(s => s
                            .Size(1)
                            //.Source(m => m.Excludes(e => e.Field(o => o.Vazby)))
                            .Source(ss => ss.ExcludeAll())
                            .Query(q => q
                                    .MatchPhrase(m => m
                                        .Field(ff => ff.NamedId)
                                        .Query(basic)
                                    )
                            )

                    );

                string foundId = res.Total > 0 ? res.Hits.First().Id : null;
                if (foundId != null && foundId == this.Id.ToString())
                    exists = false;
                else if (res.Total > 0)
                {
                    num++;
                    basic = basic + num.ToString();
                    exists = true;
                }
                else
                    exists = false;
            } while (exists);

            return basic;
        }
        public Relation[] AktualniVazby(Relation.AktualnostType minAktualnost)
        {
            return Relation.AktualniVazby(this.Vazby, minAktualnost);
        }



        public void SetOdhadVek(int vek, DateTime kDatu)
        {
            this.Odhad_vek_narozenOd = kDatu.AddYears(-vek).AddYears(-1).AddDays(1);
            this.Odhad_vek_narozenDo = kDatu.AddYears(-vek).AddYears(1).AddDays(-1);

        }

        public string FullName(bool html = false)
        {
            string ret = string.Format("{0}{1} {2} {3}", this.TitulPred, this.Jmeno, this.Prijmeni, this.TitulPo).Trim();
            if (html)
                ret = ret.Replace(" ", "&nbsp;");
            return ret;
        }

        public void PrepareBeforeSave()
        {
            this.NamedId = this.GetUniqueNamedId();
            this.LastUpdate = DateTime.Now;
        }


        public static bool ExistsInDb(Person person, out Guid? IdOfFound)
        {
            IdOfFound = null;

            if (person.Narozeni.HasValue)
            {
                var res = Lib.ES.Manager.GetESClient()
                        .Search<Person>(s => s
                            .Size(1)
                            .FielddataFields(f => f.Field(ff => ff.Id))
                            .Query(q => q
                                .Bool(b => b
                                    .Must(
                                        p => p.Term(f => f.Jmeno, person.Jmeno),
                                        p => p.Term(f => f.Prijmeni, person.Prijmeni),
                                        p => p.Term(f => f.Narozeni, person.Narozeni)
                                    )
                                )
                            )

                    );
                if (res.IsValid == false)
                    Lib.ES.Manager.LogQueryError(res.ServerError);

                if (res.Total > 0)
                {
                    IdOfFound = Guid.Parse(res.Hits.First().Id);
                }
                return res.Total > 0;

            }
            else if (person.Odhad_vek_narozenDo.HasValue && person.Odhad_vek_narozenOd.HasValue)
            {
                bool found = false;
                //hledam stejne
                var res = Lib.ES.Manager.GetESClient()
                        .Search<Person>(s => s
                            .Size(1)
                            .Source(ss => ss.ExcludeAll())
                            //.Fields(f => f.Field(ff => ff.Id))
                            .Query(q => q
                                .Bool(b => b
                                    .Must(
                                        p => p.Term(f => f.Jmeno, person.Jmeno),
                                        p => p.Term(f => f.Prijmeni, person.Prijmeni),
                                        p => p.Term(f => f.TitulPred, person.TitulPred),
                                        p => p.Term(f => f.TitulPo, person.TitulPo),
                                        p => p.Term(f => f.Odhad_vek_narozenOd, person.Odhad_vek_narozenOd),
                                        p => p.Term(f => f.Odhad_vek_narozenDo, person.Odhad_vek_narozenDo)
                                    )
                                )
                            )

                    );
                if (res.IsValid == false)
                    Lib.ES.Manager.LogQueryError(res.ServerError);

                if (res.Total > 0)
                {
                    IdOfFound = Guid.Parse(res.Hits.First().Id);
                    return true;
                }

                //hledam podle jmena
                res = Lib.ES.Manager.GetESClient()
                        .Search<Person>(s => s
                            .Size(100)
                            .Query(q => q
                                .Bool(b => b
                                    .Must(
                                        p => p.Term(f => f.Jmeno, person.Jmeno),
                                        p => p.Term(f => f.Prijmeni, person.Prijmeni),
                                        p => p.Term(f => f.TitulPred, person.TitulPred),
                                        p => p.Term(f => f.TitulPo, person.TitulPo)
                                    )
                                )
                            )

                    );
                if (res.IsValid == false)
                    Lib.ES.Manager.LogQueryError(res.ServerError);

                //mam hromadu politiku se stejnym jmenem. jedu po datech
                foreach (var hit in res.Hits)
                {
                    Person per = hit.Source;
                    if (per.Narozeni.HasValue)
                    {
                        if (
                            person.Odhad_vek_narozenOd.Value > per.Narozeni.Value
                            &&
                            per.Narozeni.Value < person.Odhad_vek_narozenOd.Value
                            )
                        {
                            IdOfFound = per.Id;
                            return true;
                        }
                    }
                    else
                    {
                        //rozdil zacatku a koncu odhadovanych veku neni vetsi nez 5 dni
                        found = (
                            Math.Abs((person.Odhad_vek_narozenOd.Value - per.Odhad_vek_narozenOd.Value).Ticks) < TimeSpan.FromDays(5).Ticks
                            && Math.Abs((person.Odhad_vek_narozenDo.Value - per.Odhad_vek_narozenDo.Value).Ticks) < TimeSpan.FromDays(5).Ticks
                            );
                        if (found)
                        {
                            IdOfFound = per.Id;
                            return true;
                        }
                    }

                }
                return false;

            }
            else //pouze podle jmena a titulu
            {
                var res = Lib.ES.Manager.GetESClient()
                        .Search<Person>(s => s
                            .Size(1)
                            .Source(ss => ss.ExcludeAll())
                            .Query(q => q
                                .Bool(b => b
                                    .Must(
                                        p => p.Term(f => f.Jmeno, person.Jmeno),
                                        p => p.Term(f => f.Prijmeni, person.Prijmeni),
                                        p => p.Term(f => f.TitulPred, person.TitulPred),
                                        p => p.Term(f => f.TitulPo, person.TitulPo)
                                    )
                                )
                            )

                    );
                if (res.IsValid == false)
                    Lib.ES.Manager.LogQueryError(res.ServerError);

                if (res.Total > 0)
                {
                    IdOfFound = Guid.Parse(res.Hits.First().Id);
                }
                return res.Total > 0;
            }
        }

        public static Person GetByNamedId(string namedId)
        {
            if (string.IsNullOrEmpty(namedId))
                return null;
            //pouze podle jmena a titulu
            var res = Lib.ES.Manager.GetESClient()
                    .Search<Person>(s => s
                        .Size(50)
                        .Query(q => q
                                .MatchPhrase(m=>m
                                    .Field(ff=>ff.NamedId)
                                    .Query(namedId.ToLowerInvariant())
                                )
                        )

                );
            if (res.Total == 1)
                return res.Hits.First().Source;
            else if (res.Total > 1)
            {
                Lib.Init.Logger.Error("Too many results for " + namedId);
                return res.Hits.First().Source;
            }
            else
                return null;
        }
        public static IEnumerable<Person> GetByName(string titulPred, string jmeno, string prijmeni, string titulPo)
        {
            ISearchResponse<Person> res = null;
            if (!string.IsNullOrEmpty(titulPred) || !string.IsNullOrEmpty(titulPo))
            //pouze podle jmena a titulu
            {
                res = Lib.ES.Manager.GetESClient()
                        .Search<Person>(s => s
                            .Size(50)
                            //.Fields(f => f.Field(ff => ff.Id))
                            .Query(q => q
                                .Bool(b => b
                                    .Must(
                                        p => p.Term(f => f.Jmeno, jmeno),
                                        p => p.Term(f => f.Prijmeni, prijmeni),
                                        p => p.Term(f => f.TitulPred, titulPred),
                                        p => p.Term(f => f.TitulPo, titulPo)
                                    )
                                )
                            )

                    );
            }
            else
            {
                res = Lib.ES.Manager.GetESClient()
                        .Search<Person>(s => s
                            .Size(50)
                            //.Fields(f => f.Field(ff => ff.Id))
                            .Query(q => q
                                .Bool(b => b
                                    .Must(
                                        p => p.Term(f => f.Jmeno, jmeno),
                                        p => p.Term(f => f.Prijmeni, prijmeni)
                                    )
                                )
                            )

                    );

            }
            if (res.IsValid == false)
                Lib.ES.Manager.LogQueryError(res.ServerError);

            if (res.Total > 0)
            {
                return res.Hits.Select(m => m.Source);
            }
            return new Person[] { };

        }

        public static Nest.IIndexResponse Import(Person p, bool findRelations)
        {
            List<Relation> oldRel = new List<Relation>();
            if (p.Vazby != null)
            {
                oldRel = p.Vazby.Where(m => m.Relationship == Relation.RelationSimpleEnum.OSVC || m.Permanent).ToList();
            }


            if (findRelations)
            {
                var firstRel = Lib.Data.External.FirmyDB.VsechnyDcerineVazby(p, 0, true, null);
                p.Vazby = Relation.Merge(oldRel, firstRel).ToArray();
            }
            var res = Lib.ES.Manager.Save(p);
            return res;

        }

        public static Person CreateNewPerson(
            string titulPred, string jmeno, string prijmeni, string titulPo,
            string narozeni, StatusOsobyEnum status
            )
        {
            Person p = new Data.Person();
            p.TitulPred = NormalizeTitul(titulPred, true);
            p.TitulPo = NormalizeTitul(titulPo, false);
            p.Jmeno = NormalizeJmeno(jmeno);
            p.Prijmeni = NormalizePrijmeni(prijmeni);
            p.PersonStatus = (int)status;
            p.Narozeni = null;
            DateTime o;
            if (DateTime.TryParseExact(narozeni, "d.M.yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out o)) //28.3.1955
            {
                p.Narozeni = o;
            }
            else if (DateTime.TryParseExact(narozeni, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out o)) //28.03.1955
            {
                p.Narozeni = o;
            }
            else if (DateTime.TryParseExact(narozeni, "dd.MM.yy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out o)) //28.03.55
            {
                p.Narozeni = o;
            }



            return p;
        }

        public static string Capitalize(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            //return Regex.Replace(s, @"\b(\w)", m => m.Value.ToUpper());
            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s);

        }

        public static string NormalizeJmeno(string s)
        {
            return Capitalize(s); //TODO
        }
        public static string NormalizePrijmeni(string s)
        {
            return Capitalize(s); //TODO
        }

        public static string NormalizeTitul(string s, bool pred)
        {
            return s; //TODO
        }
    }
}
