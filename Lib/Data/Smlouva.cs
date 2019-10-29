using HlidacStatu.Lib.XSD;
using HlidacStatu.Util;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace HlidacStatu.Lib.Data
{

    public partial class Smlouva
        : Bookmark.IBookmarkable, ISocialInfo
    {
        object enhLock = new object();

        public class Subjekt
        {
            [Keyword()]
            public string datovaSchranka { get; set; }
            [Keyword()]
            public string nazev { get; set; }

            [Keyword()]
            public string ico { get; set; }

            public string adresa { get; set; }
            [Keyword()]
            public string utvar { get; set; }

            public Subjekt() { }
            public Subjekt(tSmlouvaSmluvniStrana s)
            {
                this.adresa = s.adresa;
                this.datovaSchranka = s.datovaSchranka;
                this.ico = s.ico;
                this.nazev = s.nazev;
                this.utvar = s.utvar;
            }
            public Subjekt(tSmlouvaSubjekt s)
            {
                this.adresa = s.adresa;
                this.datovaSchranka = s.datovaSchranka;
                this.ico = s.ico;
                this.nazev = s.nazev;
                this.utvar = s.utvar;
            }
        }



        Smlouva[] _otherVersions = null;
        public Smlouva[] OtherVersions()
        {
            var result = new List<Smlouva>();
            if (_otherVersions == null)
            {
                var res = ES.SearchTools.SimpleSearch("identifikator.idSmlouvy:" + this.identifikator.idSmlouvy,
                    1, 50, ES.SearchTools.OrderResult.DateAddedDesc, null
                    );
                var resNeplatne = ES.SearchTools.SimpleSearch("identifikator.idSmlouvy:" + this.identifikator.idSmlouvy,
                    1, 50, ES.SearchTools.OrderResult.DateAddedDesc, null, platnyZaznam: 0
                    );

                if (res.IsValid == false)
                    HlidacStatu.Lib.ES.Manager.LogQueryError<Smlouva>(res.Result);
                else
                    result.AddRange(res.Result.Hits.Select(m => m.Source).Where(m => m.Id != this.Id));

                if (resNeplatne.IsValid == false)
                    HlidacStatu.Lib.ES.Manager.LogQueryError<Smlouva>(resNeplatne.Result);
                else
                    result.AddRange(resNeplatne.Result.Hits.Select(m => m.Source).Where(m => m.Id != this.Id));

                _otherVersions = result.ToArray();

                List<QueryContainer> mustQs = new List<QueryContainer>(sameContractSides());
                mustQs.AddRange(new QueryContainer[] {
                                new QueryContainerDescriptor<Lib.Data.Smlouva>().Match(qm=>qm.Field(f=>f.predmet).Query(this.predmet)),
                                new QueryContainerDescriptor<Lib.Data.Smlouva>().Term(t => t.Field(f=>f.datumUzavreni).Value(this.datumUzavreni)),
                                new QueryContainerDescriptor<Lib.Data.Smlouva>().Term(t => t.Field(f=>f.CalculatedPriceWithVATinCZK).Value(this.CalculatedPriceWithVATinCZK)),
                            });
                List<QueryContainer> optionalQs = new List<QueryContainer>();
                if (!string.IsNullOrEmpty(this.cisloSmlouvy))
                    optionalQs.Add(
                        new QueryContainerDescriptor<Lib.Data.Smlouva>().Term(t => t.Field(f => f.cisloSmlouvy).Value(this.cisloSmlouvy)));


                _otherVersions = _otherVersions
                    .Union(GetPodobneSmlouvy(mustQs, optionalQs, _otherVersions.Select(m => m.Id)))
                    .ToArray();


            }
            return _otherVersions;
        }


        private QueryContainer[] sameContractSides()
        {
            QueryContainer[] mustQs = new QueryContainer[] {
                        new QueryContainerDescriptor<Lib.Data.Smlouva>().Term(t => t.Field("platce.ico").Value(this.Platce.ico)),
                    };
            mustQs = mustQs.Concat(this.Prijemce
                        .Where(m => !string.IsNullOrEmpty(m.ico))
                        .Select(m =>
                            new QueryContainerDescriptor<Lib.Data.Smlouva>().Term(t => t.Field("prijemce.ico").Value(m.ico))
                        )
                    ).ToArray();
            return mustQs;
        }
        Smlouva[] _podobneSmlouvy = null;

        public Smlouva[] PodobneSmlouvy()
        {
            if (_podobneSmlouvy == null)
            {
                IEnumerable<QueryContainer> mustQs = sameContractSides().Union(new QueryContainer[] {                         new QueryContainerDescriptor<Lib.Data.Smlouva>().Match(qm=>qm.Field(f=>f.predmet).Query(this.predmet)),
                        new QueryContainerDescriptor<Lib.Data.Smlouva>().Match(qm=>qm.Field(f=>f.predmet).Query(this.predmet)),
                        });
                QueryContainer[] niceToHaveQs = new QueryContainer[] {
                        new QueryContainerDescriptor<Lib.Data.Smlouva>().Term(t => t.Field(f=>f.datumUzavreni).Value(this.datumUzavreni)),
                        new QueryContainerDescriptor<Lib.Data.Smlouva>().Term(t => t.Field(f=>f.CalculatedPriceWithVATinCZK).Value(this.CalculatedPriceWithVATinCZK)),
                    };

                _podobneSmlouvy = GetPodobneSmlouvy(sameContractSides(), niceToHaveQs, this.OtherVersions().Select(m => m.Id), 10);
            }

            return _podobneSmlouvy;
        }


        public Smlouva[] GetPodobneSmlouvy(IEnumerable<QueryContainer> mandatory, IEnumerable<QueryContainer> optional = null, IEnumerable<string> exceptIds = null, int numOfResults = 50)
        {
            optional = optional ?? new QueryContainer[] { };
            exceptIds = exceptIds ?? new string[] { };
            Smlouva[] _result = null;

            int tryNum = optional.Count();
            while (_podobneSmlouvy == null && tryNum >= 0)
            {
                var query = mandatory.Concat(optional.Take(tryNum)).ToArray();
                tryNum--;

                var tmpResult = new List<Smlouva>();
                var res = ES.SearchTools.RawSearch(
                    new QueryContainerDescriptor<Lib.Data.Smlouva>().Bool(b => b.Must(query)),
                        1, numOfResults, ES.SearchTools.OrderResult.DateAddedDesc, null
                    );
                var resN = ES.SearchTools.RawSearch(
                    new QueryContainerDescriptor<Lib.Data.Smlouva>().Bool(b => b.Must(query)),
                        1, numOfResults, ES.SearchTools.OrderResult.DateAddedDesc, null, platnyZaznam: 0
                    );

                if (res.IsValid == false)
                    HlidacStatu.Lib.ES.Manager.LogQueryError<Smlouva>(res);
                else
                    tmpResult.AddRange(res.Hits.Select(m => m.Source).Where(m => m.Id != this.Id));
                if (resN.IsValid == false)
                    HlidacStatu.Lib.ES.Manager.LogQueryError<Smlouva>(resN);
                else
                    tmpResult.AddRange(resN.Hits.Select(m => m.Source).Where(m => m.Id != this.Id));

                if (tmpResult.Count > 0)
                {
                    var resSml = tmpResult.Where(m =>
                        m.Id != this.Id
                        && !exceptIds.Any(id => id == m.Id)
                    ).ToArray();
                    if (resSml.Length > 0)
                        _result = resSml;
                }

            };
            if (_result == null)
                _result = new Smlouva[] { }; //not found anything

            return _result.Take(numOfResults).ToArray();
        }

        public enum PravniRamce
        {
            Undefined = 0,
            Do072017 = 1,
            Od072017 = 2,
            MimoRS = 3
        }

        static DateTime pravniRamce01072017 = new DateTime(2017, 7, 1);
        [Object(Ignore = true)]
        public PravniRamce PravniRamec
        {
            get
            {
                if (this.IsPartOfRegistrSmluv() == false)
                {
                    return PravniRamce.MimoRS;
                }
                else
                {
                    if (this.datumUzavreni < pravniRamce01072017)
                        return PravniRamce.Do072017;
                    else if (this.datumUzavreni >= pravniRamce01072017)
                        return PravniRamce.Od072017;
                    else
                        return PravniRamce.Undefined;
                }
            }
        }

        public Priloha[] Prilohy { get; set; }

        [Date]
        public DateTime LastUpdate { get; set; } = DateTime.MinValue;
        public decimal CalculatedPriceWithVATinCZK { get; set; }
        public DataQualityEnum CalcutatedPriceQuality { get; set; }

        public SClassification Classification { get; set; } = new SClassification();

        public bool SetClassification(SClassification classif) //true if changed
        {
            this.Classification = classif;
            return true;
        }

        public bool SetClassification(bool rewrite = false, bool addNewVersion = false) //true if changed
        {
            if (addNewVersion || rewrite || this.Classification?.LastUpdate == null)
            {
                var types = GetClassification();

                SClassification.Classification[] newClass = types
                                        .Select(m => new SClassification.Classification()
                                        {
                                            TypeValue = (int)m.Key,
                                            ClassifProbability = m.Value
                                        }
                                        )
                                        .ToArray();

                if (addNewVersion)
                    newClass = newClass
                        .Concat(this.Classification?.Types ?? new SClassification.Classification[] { })
                        .ToArray();

                SetClassification(new SClassification(newClass));
                return true;
            }
            else
                return false;
        }

        public bool Delete(ElasticClient client = null)
        {
            return Delete(this.Id);
        }
        public static bool Delete(string Id, ElasticClient client = null)
        {
            if (client == null)
                client = Lib.ES.Manager.GetESClient();
            var res = client
                .Delete<Lib.Data.Smlouva>(Id);
            return res.IsValid;
        }
        public bool Save(ElasticClient client = null)
        {
            var item = this;
            if (item == null)
                return false;

            item.PrepareBeforeSave();
            ElasticClient c = client;
            if (c == null)
            {
                if (item.platnyZaznam)
                    c = Lib.ES.Manager.GetESClient();
                else
                    c = Lib.ES.Manager.GetESClient_Sneplatne();
            }
            var res = c
                //.Update<Lib.Data.Smlouva>()
                .Index<Lib.Data.Smlouva>(item, m => m.Id(item.Id));

            if (item.platnyZaznam == false && res.IsValid)
            {
                //zkontroluj zda neni v indexu s platnymi. pokud ano, smaz ho tam
                var cExist = ES.Manager.GetESClient();
                var s = Load(item.Id, cExist);
                if (s != null)
                    Delete();
            }

            if (res.IsValid)
            {
                try
                {

                    DirectDB.NoResult("exec smlouvaId_save @id,@active, @created, @updated",
                        new System.Data.SqlClient.SqlParameter("id", item.Id),
                        new System.Data.SqlClient.SqlParameter("created", item.casZverejneni),
                        new System.Data.SqlClient.SqlParameter("updated", item.LastUpdate),
                        new System.Data.SqlClient.SqlParameter("active", item.znepristupnenaSmlouva() ? (int)0 : (int)1)
                        );
                }
                catch (Exception e)
                {
                    ES.Manager.ESLogger.Error("Manager Save", e);
                }



                if (!string.IsNullOrEmpty(item.Platce?.ico))
                {
                    DirectDB.NoResult("exec Firma_IsInRS_Save @ico",
                    new System.Data.SqlClient.SqlParameter("ico", item.Platce?.ico)
                    );
                }
                if (!string.IsNullOrEmpty(item.VkladatelDoRejstriku?.ico))
                {
                    DirectDB.NoResult("exec Firma_IsInRS_Save @ico",
                    new System.Data.SqlClient.SqlParameter("ico", item.VkladatelDoRejstriku?.ico)
                    );
                }
                foreach (var s in item.Prijemce ?? new Data.Smlouva.Subjekt[] { })
                {
                    if (!string.IsNullOrEmpty(s.ico))
                    {
                        DirectDB.NoResult("exec Firma_IsInRS_Save @ico",
                            new System.Data.SqlClient.SqlParameter("ico", s.ico)
                            );
                    }
                }

            }
            return res.IsValid;
        }
        public Dictionary<Lib.Data.Smlouva.SClassification.ClassificationsTypes, decimal> GetClassification()
        {
            Dictionary<Lib.Data.Smlouva.SClassification.ClassificationsTypes, decimal> data = new Dictionary<Smlouva.SClassification.ClassificationsTypes, decimal>();

            string baseUrl = Devmasters.Core.Util.Config.GetConfigValue("Classification.Service.Url");

            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new HlidacStatu.Util.FirstCaseLowercaseContractResolver();


            using (Devmasters.Net.Web.URLContent stem = new Devmasters.Net.Web.URLContent(baseUrl + "/stemmer"))
            {
                stem.Method = Devmasters.Net.Web.MethodEnum.POST;
                stem.Tries = 3;
                stem.TimeInMsBetweenTries = 5000;
                stem.Timeout = 45000;
                stem.ContentType = "application/json; charset=utf-8";
                stem.RequestParams.RawContent = Newtonsoft.Json.JsonConvert.SerializeObject(this, settings);
                Devmasters.Net.Web.TextContentResult stems = null;
                try
                {
                    stems = stem.GetContent();

                }
                catch (Exception e)
                {
                    Util.Consts.Logger.Error("Classification Stemmer API ", e);
                    throw;
                }
                using (Devmasters.Net.Web.URLContent classif = new Devmasters.Net.Web.URLContent(baseUrl + "/classifier"))
                {
                    classif.Method = Devmasters.Net.Web.MethodEnum.POST;
                    classif.Tries = 3;
                    classif.TimeInMsBetweenTries = 5000;
                    classif.Timeout = 180000;
                    classif.ContentType = "application/json; charset=utf-8";
                    classif.RequestParams.RawContent = stems.Text;
                    Devmasters.Net.Web.TextContentResult classifier = null;
                    try
                    {
                        classifier = classif.GetContent();
                    }
                    catch (Exception e)
                    {
                        Util.Consts.Logger.Error("Classification Classifier API ", e);
                        throw;
                    }

                    using (Devmasters.Net.Web.URLContent fin = new Devmasters.Net.Web.URLContent(baseUrl + "/finalizer"))
                    {
                        fin.Method = Devmasters.Net.Web.MethodEnum.POST;
                        fin.Tries = 3;
                        fin.TimeInMsBetweenTries = 5000;
                        fin.Timeout = 30000;
                        fin.ContentType = "application/json; charset=utf-8";
                        fin.RequestParams.RawContent = classifier.Text;
                        Devmasters.Net.Web.TextContentResult res = null;
                        try
                        {
                            res = fin.GetContent();
                        }
                        catch (Exception e)
                        {
                            Util.Consts.Logger.Error("Classification finalizer API ", e);
                            throw;
                        }


                        var jsonData = Newtonsoft.Json.Linq.JArray.Parse(res.Text);

                        for (int i = 0; i < jsonData.Count; i++)
                        {
                            string key = jsonData[i][0].Value<string>().Replace("-", "_");
                            decimal prob = jsonData[i][1].Value<decimal>();
                            if (Enum.TryParse<Smlouva.SClassification.ClassificationsTypes>(key, out var typ))
                            {
                                data.Add(typ, prob);
                            }
                            else
                            {
                                Util.Consts.Logger.Error("Invalid key " + key);
                                data.Add(Smlouva.SClassification.ClassificationsTypes.OSTATNI, prob);
                            }
                        }

                    }
                }
            }
            return data;
        }





        private Issues.Issue[] issues = new Lib.Issues.Issue[] { };
        public Issues.Issue[] Issues
        {
            get
            {
                return issues;
            }
            set
            {

                if (this.issues.Any(m => m.Permanent))
                {
                    //nech jen permanent
                    var newIss = this.issues.Where(m => m.Permanent).ToList();
                    //unique Ids, at se neopakuji
                    var existsIds = newIss.Select(m => m.IssueTypeId).Distinct();

                    //pridej vse krome existujicich Ids
                    newIss.AddRange(value.Where(m => !(existsIds.Contains(m.IssueTypeId))));
                    this.issues = newIss.ToArray();
                }
                else
                    this.issues = value;
                this.ConfidenceValue = GetConfidenceValue();
            }
        }


        public void ClearAllIssuesIncludedPermanent()
        {
            this.issues = new Lib.Issues.Issue[] { };
        }
        public void AddSpecificIssue(Issues.Issue i)
        {
            if (!this.Issues.Any(m => m.IssueTypeId == i.IssueTypeId))
            {
                var oldIssues = this.Issues.ToList();
                oldIssues.Add(i);
                this.Issues = oldIssues.ToArray();
            }

        }

        public void AddEnhancement(Enhancers.Enhancement enh)
        {
            lock (enhLock)
            {
                if (!this.Enhancements.Contains(enh))
                {
                    //add new to the array http://stackoverflow.com/a/31542691/1906880
                    Enhancers.Enhancement[] result = new Enhancers.Enhancement[this.Enhancements.Length + 1];
                    this.Enhancements.CopyTo(result, 0);
                    result[this.Enhancements.Length] = enh;
                }
                else
                {
                    var existingIdx = Array.FindIndex(this.Enhancements, e => e == enh);
                    if (existingIdx > -1)
                    {
                        this.Enhancements[existingIdx] = enh;
                    }

                }
            }


        }
        public Enhancers.Enhancement[] Enhancements { get; set; } = new Enhancers.Enhancement[] { };

        [Nest.Number]
        public decimal ConfidenceValue { get; set; }



        [Nest.Text]
        public string Id
        {
            get
            {
                return this.identifikator.idVerze;
            }
        }

        public tIdentifikator identifikator;

        [Nest.Keyword]
        public string odkaz { get; set; }

        [Date]
        public System.DateTime casZverejneni { get; set; }

        public Subjekt VkladatelDoRejstriku { get; set; }

        public Subjekt Platce { get; set; }

        public Subjekt[] Prijemce { get; set; }


        [Nest.Text]
        public string predmet { get; set; }

        [Date]
        public System.DateTime datumUzavreni { get; set; }

        [Nest.Keyword]
        public string cisloSmlouvy { get; set; }

        public string schvalil;

        public decimal? hodnotaBezDph;
        public decimal? hodnotaVcetneDph;
        public tSmlouvaCiziMena ciziMena;

        [Nest.Keyword]
        public string navazanyZaznam { get; set; }

        [Nest.Keyword]
        public string[] souvisejiciSmlouvy { get; set; } = null;

        //public tSmlouva smlouva;

        /// <remarks/>
        public bool platnyZaznam;

        [Nest.Boolean()]
        public bool? SVazbouNaPolitiky { get; set; } = false;
        [Nest.Boolean()]
        public bool? SVazbouNaPolitikyNedavne { get; set; } = false;
        [Nest.Boolean()]
        public bool? SVazbouNaPolitikyAktualni { get; set; } = false;

        public string SocialInfoTitle()
        {
            return Devmasters.Core.TextUtil.ShortenText(this.predmet, 45, "...");
        }

        public string SocialInfoSubTitle()
        {

            if (this.Issues.Any(m => m.Importance == HlidacStatu.Lib.Issues.ImportanceLevel.Fatal))
                return "Smlouva je formálně platná, ale <b>obsahuje závažné nedostatky v rozporu se zákonem!</b>";
            else
                return (this.znepristupnenaSmlouva() ? "Zneplatněná smlouva." : "Platná smlouva.");

        }

        public string SocialInfoBody()
        {
            return "<ul>" +
    HlidacStatu.Util.InfoFact.RenderInfoFacts(this.InfoFacts(), 4, true, true, "", "<li>{0}</li>", true)
    + "</ul>";
        }

        public string SocialInfoFooter()
        {
            return $"Smlouva byla podepsána {this.datumUzavreni.ToString("d.M.yyyy")}, zveřejněna {this.casZverejneni.ToString("d.M.yyyy")}";
        }

        public string SocialInfoImageUrl()
        {
            return string.Empty;
        }

        InfoFact[] _infofacts = null;
        object lockInfoObj = new object();
        public InfoFact[] InfoFacts()
        {
            lock (lockInfoObj)
            {
                if (_infofacts == null)
                {
                    List<InfoFact> f = new List<InfoFact>();

                    string hlavni = $"Smlouva mezi {Devmasters.Core.TextUtil.ShortenText(this.Platce.nazev, 60)} a "
                        + $"{Devmasters.Core.TextUtil.ShortenText(this.Prijemce.First().nazev, 60)}";
                    if (this.Prijemce.Count() == 0)
                        hlavni += $".";
                    else if (this.Prijemce.Count() == 1)
                        hlavni += $" a 1 dalším.";
                    else if (this.Prijemce.Count() > 1)
                        hlavni += $" a {this.Prijemce.Count() - 1} dalšími.";
                    hlavni += (this.CalculatedPriceWithVATinCZK == 0
                        ? " Hodnota smlouvy je utajena."
                        : " Hodnota smlouvy je " + HlidacStatu.Util.RenderData.ShortNicePrice(this.CalculatedPriceWithVATinCZK, html: true));

                    f.Add(new InfoFact(hlavni, InfoFact.ImportanceLevel.Summary));

                    //sponzori
                    foreach (var subj in this.Prijemce.Union(new HlidacStatu.Lib.Data.Smlouva.Subjekt[] { this.Platce }))
                    {
                        var firma = HlidacStatu.Lib.Data.Firmy.Get(subj.ico);
                        if (firma.Valid && firma.IsSponzor() && firma.JsemSoukromaFirma())
                        {
                            f.Add(new InfoFact(
                                $"{firma.Jmeno}: " +
                                firma.Description(true, m => m.Type == (int)HlidacStatu.Lib.Data.FirmaEvent.Types.Sponzor, itemDelimeter: ", ", numOfRecords: 2)
                                , InfoFact.ImportanceLevel.Medium)
                                );
                        }
                    }

                    //issues
                    if (this.IsPartOfRegistrSmluv() && this.znepristupnenaSmlouva() == false
                        && this.Issues != null && this.Issues.Any(m => m.Public && m.Public && m.Importance != HlidacStatu.Lib.Issues.ImportanceLevel.NeedHumanReview))
                    {
                        int count = 0;
                        foreach (var iss in this.Issues.Where(m => m.Public && m.Importance != HlidacStatu.Lib.Issues.ImportanceLevel.NeedHumanReview)
                            .OrderByDescending(m => m.Importance))
                        {
                            if (this.znepristupnenaSmlouva() && iss.IssueTypeId == -1) //vypis pouze info o znepristupneni
                            {
                                count++;
                                f.Add(new InfoFact(
                                    $"<b>{iss.Title}</b><br/><small>{iss.TextDescription}</small>"
                                    , InfoFact.ImportanceLevel.High)
                                    );
                            }
                            else if (iss.Public && iss.Importance != HlidacStatu.Lib.Issues.ImportanceLevel.NeedHumanReview)
                            {
                                count++;
                                string importance = "";
                                switch (iss.Importance)
                                {
                                    case Lib.Issues.ImportanceLevel.NeedHumanReview:
                                    case Lib.Issues.ImportanceLevel.Ok:
                                    case Lib.Issues.ImportanceLevel.Formal:
                                        importance = "";
                                        break;
                                    case Lib.Issues.ImportanceLevel.Minor:
                                    case Lib.Issues.ImportanceLevel.Major:
                                        importance = "Nedostatek: ";
                                        break;
                                    case Lib.Issues.ImportanceLevel.Fatal:
                                        importance = "Vážný nedostatek: ";
                                        break;
                                    default:
                                        break;
                                }
                                f.Add(
                                    new InfoFact(
                                    $"<b>{importance}{(string.IsNullOrEmpty(importance) ? iss.Title : iss.Title.ToLower())}</b><br/><small>{iss.TextDescription}</small>"
                                    , InfoFact.ImportanceLevel.Medium)
                                    );
                            }

                            if (count >= 2)
                                break;
                        }

                    }
                    else
                        f.Add(new InfoFact("Žádné nedostatky u smlouvy jsme nenalezli.", InfoFact.ImportanceLevel.Low));


                    //politici
                    foreach (var ss in this.Prijemce)
                    {
                        if (!string.IsNullOrEmpty(ss.ico) && HlidacStatu.Lib.StaticData.FirmySVazbamiNaPolitiky_nedavne_Cache.Get().SoukromeFirmy.ContainsKey(ss.ico))
                        {
                            var politici = StaticData.FirmySVazbamiNaPolitiky_nedavne_Cache.Get().SoukromeFirmy[ss.ico];
                            if (politici.Count > 0)
                            {
                                var sPolitici = Osoby.GetById.Get(politici[0]).FullNameWithYear();
                                if (politici.Count == 2)
                                {
                                    sPolitici = sPolitici + " a " + Osoby.GetById.Get(politici[1]).FullNameWithYear();
                                }
                                else if (politici.Count == 3)
                                {
                                    sPolitici = sPolitici
                                        + ", "
                                        + Osoby.GetById.Get(politici[1]).FullNameWithYear()
                                        + " a "
                                        + Osoby.GetById.Get(politici[2]).FullNameWithYear();

                                }
                                else if (politici.Count > 3)
                                {
                                    sPolitici = sPolitici
                                        + ", "
                                        + Osoby.GetById.Get(politici[1]).FullNameWithYear()
                                        + ", "
                                        + Osoby.GetById.Get(politici[2]).FullNameWithYear()
                                        + " a další";

                                }
                                f.Add(new InfoFact($"V dodavateli {Firmy.GetJmeno(ss.ico)} se "
                                    + Devmasters.Core.Lang.Plural.Get(politici.Count()
                                                                        , " angažuje jedna politicky angažovaná osoba - "
                                                                        , " angažují {0} politicky angažované osoby - "
                                                                        , " angažuje {0} politicky angažovaných osob - ")
                                    + sPolitici + "."
                                    , InfoFact.ImportanceLevel.Medium)
                                    );
                            }


                        }

                    }

                    _infofacts = f.OrderByDescending(o => o.Level).ToArray();
                }
            }
            return _infofacts;
        }




        static HashSet<string> ico_s_VazbouPolitik = new HashSet<string>(
            StaticData.FirmySVazbamiNaPolitiky_vsechny_Cache.Get().SoukromeFirmy.Select(m => m.Key)
                .Union(StaticData.SponzorujiciFirmy_Vsechny.Get().Select(m => m.ICO))
                .Distinct()
            );
        static HashSet<string> ico_s_VazbouPolitikNedavne = new HashSet<string>(
            StaticData.FirmySVazbamiNaPolitiky_nedavne_Cache.Get().SoukromeFirmy.Select(m => m.Key)
                .Union(StaticData.SponzorujiciFirmy_Nedavne.Get().Select(m => m.ICO))
                .Distinct()
            );
        static HashSet<string> ico_s_VazbouPolitikAktualni = new HashSet<string>(
            StaticData.FirmySVazbamiNaPolitiky_aktualni_Cache.Get().SoukromeFirmy.Select(m => m.Key)
                .Union(StaticData.SponzorujiciFirmy_Nedavne.Get().Select(m => m.ICO))
                .Distinct()
            );
        public bool JeSmlouva_S_VazbouNaPolitiky(Relation.AktualnostType aktualnost)
        {
            var icos = ico_s_VazbouPolitik;
            if (aktualnost == Relation.AktualnostType.Nedavny)
                icos = ico_s_VazbouPolitikNedavne;
            if (aktualnost == Relation.AktualnostType.Aktualni)
                icos = ico_s_VazbouPolitikAktualni;
            Firma f = null;
            if (this.platnyZaznam)
            {
                f = Firmy.Get(this.Platce.ico);
                if (f.Valid && !f.PatrimStatu())
                {
                    if (!string.IsNullOrEmpty(this.Platce.ico) && icos.Contains(this.Platce.ico))
                        return true;
                }

                foreach (var ss in this.Prijemce)
                {
                    f = Firmy.Get(ss.ico);
                    if (f.Valid && !f.PatrimStatu())
                    {
                        if (!string.IsNullOrEmpty(ss.ico) && icos.Contains(ss.ico))
                            return true;
                    }
                }
            }
            return false;
        }

        public string FullTitle()
        {
            return string.Format("Smlouva č. {0}: {1}", this.Id, Devmasters.Core.TextUtil.ShortenText(this.predmet ?? "", 70));
        }

        public void PrepareBeforeSave()
        {
            this.SVazbouNaPolitiky = this.JeSmlouva_S_VazbouNaPolitiky(Relation.AktualnostType.Libovolny);
            this.SVazbouNaPolitikyNedavne = this.JeSmlouva_S_VazbouNaPolitiky(Relation.AktualnostType.Nedavny);
            this.SVazbouNaPolitikyAktualni = this.JeSmlouva_S_VazbouNaPolitiky(Relation.AktualnostType.Aktualni);
            this.LastUpdate = DateTime.Now;

            this.ConfidenceValue = GetConfidenceValue();

        }
        private decimal GetConfidenceValue()
        {
            if (this.IsPartOfRegistrSmluv() == false)
                return 0;

            if (this.Issues != null)
                return this.Issues.Sum(m => (int)m.Importance);
            else
            {
                return 0;
            }

        }


        public Issues.ImportanceLevel GetConfidenceLevel()
        {
            if (ConfidenceValue <= 0 || this.Issues == null)
            {
                return Lib.Issues.ImportanceLevel.Ok;
            }
            if (this.Issues.Count() == 0)
            {
                return Lib.Issues.ImportanceLevel.Ok;
            }
            //pokud je tam min 1x fatal, je cele fatal
            if (this.Issues.Any(m => m.Importance == Lib.Issues.ImportanceLevel.Fatal))
            {
                return Lib.Issues.ImportanceLevel.Fatal;
            }
            if (ConfidenceValue > ((int)Lib.Issues.ImportanceLevel.Major) * 3)
            {
                return Lib.Issues.ImportanceLevel.Fatal;
            }


            //pokud je tam min 1x fatal, je cele fatal
            if (this.Issues.Any(m => m.Importance == Lib.Issues.ImportanceLevel.Major))
            {
                return Lib.Issues.ImportanceLevel.Major;
            }
            if (ConfidenceValue > ((int)Lib.Issues.ImportanceLevel.Major) * 2 && ConfidenceValue <= ((int)Lib.Issues.ImportanceLevel.Major) * 3)
            {
                return Lib.Issues.ImportanceLevel.Major;
            }

            if (ConfidenceValue > ((int)Lib.Issues.ImportanceLevel.Minor) * 2 && ConfidenceValue <= ((int)Lib.Issues.ImportanceLevel.Major) * 2)
            {
                return Lib.Issues.ImportanceLevel.Minor;
            }

            if (ConfidenceValue > 0 && ConfidenceValue <= ((int)Lib.Issues.ImportanceLevel.Minor) * 2)
            {
                return Lib.Issues.ImportanceLevel.Formal;
            }

            return Lib.Issues.ImportanceLevel.Ok;
        }

        public Smlouva()
        { }

        public Smlouva(XSD.dumpZaznam item)
        {
            this.casZverejneni = item.casZverejneni;
            this.identifikator = item.identifikator;
            this.odkaz = item.odkaz;
            this.platnyZaznam = item.platnyZaznam;
            if (item.prilohy != null)
                this.Prilohy = item.prilohy.Select(m => new Priloha(m)).ToArray();

            this.cisloSmlouvy = item.smlouva.cisloSmlouvy;
            this.ciziMena = item.smlouva.ciziMena;
            this.datumUzavreni = item.smlouva.datumUzavreni;
            this.hodnotaBezDph = item.smlouva.hodnotaBezDphSpecified ? item.smlouva.hodnotaBezDph : (decimal?)null;
            this.hodnotaVcetneDph = item.smlouva.hodnotaVcetneDphSpecified ? item.smlouva.hodnotaVcetneDph : (decimal?)null;

            this.navazanyZaznam = item.smlouva.navazanyZaznam;
            this.predmet = item.smlouva.predmet;
            this.schvalil = item.smlouva.schvalil;


            //smluvni strany

            //vkladatel je jasny
            this.VkladatelDoRejstriku = new Subjekt(item.smlouva.subjekt);

            //pokud je nastaven parametr
            //<xs:documentation xml:lang="cs">1 = příjemce, 0 = plátce</xs:documentation>
            bool platceSpecified = item.smlouva.smluvniStrana.Any(m => m.prijemce.HasValue && m.prijemce == false);
            bool prijemceSpecified = item.smlouva.smluvniStrana.Any(m => m.prijemce.HasValue && m.prijemce == true);

            if (platceSpecified)
                this.Platce =
                    new Subjekt(item.smlouva
                        .smluvniStrana
                        .Where(m => m.prijemce.HasValue && m.prijemce == false)
                        .First()
                        );
            else
            {
                //copy from subjekt
                this.Platce = new Subjekt(item.smlouva.subjekt);
            }

            if (prijemceSpecified)
            {
                this.Prijemce = item.smlouva.smluvniStrana
                    .Where(m => m.prijemce.HasValue && m.prijemce == true)
                    .Select(m => new Subjekt(m))
                    .ToArray();

                //add missing from source
                this.Prijemce = this.Prijemce
                                        .ToArray()
                                        .Union(
                                                item.smlouva.smluvniStrana
                                                    .Where(m => m.prijemce.HasValue == false)
                                                    .Select(m => new Subjekt(m))
                                        ).ToArray();


            }
            else
            {
                this.Prijemce = item.smlouva.smluvniStrana
                    //.Where(m => m.ico != this.Platce.ico || m.datovaSchranka != this.Platce.datovaSchranka)
                    .Where(m => m.prijemce.HasValue == false && m.prijemce != false)
                    .Select(m => new Subjekt(m))
                    .ToArray();
            }

            //add missing from subject
            if (platceSpecified)
            {
                if (this.Prijemce
                        .Where(m => m.ico == this.VkladatelDoRejstriku.ico || m.datovaSchranka != this.VkladatelDoRejstriku.datovaSchranka)
                        .Count() == 0
                    )
                {
                    this.Prijemce = this.Prijemce
                                        .ToArray()
                                        .Union(
                                                new Subjekt[] { this.VkladatelDoRejstriku }
                                        ).ToArray();
                }

            }

        }

        public bool IsPartOfRegistrSmluv()
        {
            if (this.Id.StartsWith("pre"))
                return false;
            else
                return true;
        }
        public bool znepristupnenaSmlouva()
        {
            var b = this.Issues != null && this.Issues.Any(m => m.IssueTypeId == -1);
            if (this.IsPartOfRegistrSmluv() == false)
                b = false;
            return (b || platnyZaznam == false);
        }

        public static string NicePrice(decimal? number, string mena = "Kč", bool html = false, bool shortFormat = false)
        {
            if (number.HasValue)
                return NicePrice(number.Value, mena, html, shortFormat);
            else
                return string.Empty;
        }
        public static string NicePrice(int? number, string mena = "Kč", bool html = false, bool shortFormat = false)
        {
            if (number.HasValue)
                return NicePrice((decimal)number.Value, mena, html, shortFormat);
            else
                return string.Empty;
        }
        public static string NicePrice(double? number, string mena = "Kč", bool html = false, bool shortFormat = false)
        {
            if (number.HasValue)
                return NicePrice((decimal)number.Value, mena, html, shortFormat);
            else
                return string.Empty;
        }

        public static string NicePrice(decimal number, string mena = "Kč", bool html = false, bool shortFormat = false)
        {
            return HlidacStatu.Util.RenderData.NicePrice(number, mena: mena, html: html, shortFormat: shortFormat);
        }

        public static string ShortNicePrice(decimal number, string mena = "Kč", bool html = false, bool shortFormat = false)
        {
            return HlidacStatu.Util.RenderData.ShortNicePrice(number, mena: mena, html: html);
        }

        public string NicePrice(bool html = false)
        {
            string res = "Neuvedena";
            if (this.CalculatedPriceWithVATinCZK == 0)
                return res;
            else
                return NicePrice(this.CalculatedPriceWithVATinCZK, html: html);
        }

        public string GetUrl(bool local = true)
        {
            return GetUrl(local, string.Empty);
        }

        public string GetUrl(bool local, string foundWithQuery)
        {
            string url = "/Detail/" + this.Id;// E49DE92B876B0C66C2F29457EB61C7B7
            if (!string.IsNullOrEmpty(foundWithQuery))
                url = url + "?qs=" + System.Net.WebUtility.UrlEncode(foundWithQuery);

            if (local == false)
                return "https://www.hlidacstatu.cz" + url;
            else
                return url;
        }



        public void SaveAttachmentsToDisk(bool rewriteExisting = false)
        {
            //string root = AppDomain.CurrentDomain.BaseDirectory + "\\Prilohy\\";
            //if (!System.IO.Directory.Exists(root))
            //    System.IO.Directory.CreateDirectory(root);


            //string dir = root + item.Id;
            //if (!System.IO.Directory.Exists(dir))
            //{
            //    System.IO.Directory.CreateDirectory(dir);
            //}
            var io = Lib.Init.PrilohaLocalCopy;

            int count = 0;
            string listing = "";
            if (this.Prilohy != null)
            {
                if (!System.IO.Directory.Exists(io.GetFullDir(this)))
                    System.IO.Directory.CreateDirectory(io.GetFullDir(this));

                foreach (var p in this.Prilohy)
                {
                    string attUrl = p.odkaz;
                    if (string.IsNullOrEmpty(attUrl))
                        continue;
                    count++;
                    string fullPath = io.GetFullPath(this, p);
                    listing = listing + string.Format("{0} : {1} \n", count, System.Net.WebUtility.UrlDecode(attUrl));
                    if (!System.IO.File.Exists(fullPath) || rewriteExisting)
                    {
                        try
                        {

                            using (Devmasters.Net.Web.URLContent url = new Devmasters.Net.Web.URLContent(attUrl))
                            {

                                url.Timeout = url.Timeout * 10;
                                byte[] data = url.GetBinary().Binary;
                                System.IO.File.WriteAllBytes(fullPath, data);
                                //p.LocalCopy = System.Text.UTF8Encoding.UTF8.GetBytes(io.GetRelativePath(item, p));
                            }
                        }
                        catch (Exception e)
                        {
                            HlidacStatu.Util.Consts.Logger.Error(attUrl, e);
                        }
                    }
                    if (p.hash == null)
                    {
                        using (FileStream filestream = new FileStream(fullPath, FileMode.Open))
                        {
                            using (SHA256 mySHA256 = SHA256Managed.Create())
                            {
                                filestream.Position = 0;
                                byte[] hashValue = mySHA256.ComputeHash(filestream);
                                p.hash = new Lib.XSD.tHash()
                                {
                                    algoritmus = "sha256",
                                    Value = BitConverter.ToString(hashValue).Replace("-", String.Empty)
                                };

                            }

                        }

                    }

                    //System.IO.File.WriteAllText(dir + "\\" + "content.nfo", listing);
                }
            }
        }


        public static Smlouva PrepareForDump(Smlouva s)
        {

            if (s.znepristupnenaSmlouva() && s.Prilohy != null)
            {
                foreach (var p in s.Prilohy)
                {
                    p.PlainTextContent = "-- anonymizovano serverem HlidacStatu.cz --";
                    p.odkaz = "";
                }
            }

            if (s.Prilohy != null)
            {
                foreach (var p in s.Prilohy)
                {
                    p.DatlClassification = null;
                    p.FileMetadata = null;
                }
            }
            s.SVazbouNaPolitiky = null;
            s.SVazbouNaPolitikyAktualni = null;
            s.SVazbouNaPolitikyNedavne = null;

            return s;
        }

        public static IEnumerable<string> AllIdsFromDB()
        {
            return AllIdsFromDB(null);
        }
        public static IEnumerable<string> AllIdsFromDB(bool? deleted)
        {
            List<string> ids = null;
            using (Lib.Data.DbEntities db = new DbEntities())
            {
                IQueryable<SmlouvyId> q = db.SmlouvyIds;
                if (deleted.HasValue)
                    q = q.Where(m => m.active == (deleted.Value ? 0 : 1));

                ids = q.Select(m => m.Id)
                    .ToList();
            }

            return ids;
        }

        public static bool ExistsZaznam(string id, ElasticClient client = null)
        {
            bool noSetClient = client == null;
            if (client == null)
                client = Lib.ES.Manager.GetESClient();
            var res = client
                    .DocumentExists<Lib.Data.Smlouva>(id);
            if (noSetClient)
            {
                if (res.Exists)
                    return true;
                client = ES.Manager.GetESClient_Sneplatne();
                res = client.DocumentExists<Lib.Data.Smlouva>(id);
                if (res.IsValid)
                    return res.Exists;
                else
                    throw new ApplicationException(res.DebugInformation, res.OriginalException);

            }
            else
                return res.Exists;

        }


        public static Smlouva Load(string idVerze, ElasticClient client = null)
        {
            try
            {
                ElasticClient c = client;
                if (client == null)
                    c = Lib.ES.Manager.GetESClient();

                var res = c
                    .Get<Lib.Data.Smlouva>(idVerze);
                if (res.Found)
                    return res.Source;
                else
                {
                    if (client == null)
                    {
                        c = ES.Manager.GetESClient_Sneplatne();
                    }
                    res = c.Get<Lib.Data.Smlouva>(idVerze);
                    if (res.Found)
                        return res.Source;
                    else
                    {
                        ES.Manager.ESLogger.Warning("Cannot load Smlouva Id " + idVerze, res.OriginalException);
                        DirectDB.NoResult("delete from SmlouvyIds where id = @id", new System.Data.SqlClient.SqlParameter("id", idVerze));
                    }

                    return null;
                }


            }
            catch (Exception e)
            {
                ES.Manager.ESLogger.Error("Cannot load Smlouva Id " + idVerze, e);
                return null;
            }
        }

        public static IEnumerable<string> AllIdsFromES()
        {
            return AllIdsFromES(null);
        }
        public static IEnumerable<string> AllIdsFromES(bool? deleted, Action<string> outputWriter = null, Action<Devmasters.Core.Batch.ActionProgressData> progressWriter = null)
        {
            if (deleted.HasValue)
                return _allIdsFromES(deleted.Value, outputWriter, progressWriter);
            else
                return
                    _allIdsFromES(false, outputWriter, progressWriter)
                    .Union(_allIdsFromES(true, outputWriter, progressWriter))
                    ;
        }
        public static IEnumerable<string> _allIdsFromES(bool deleted, Action<string> outputWriter = null, Action<Devmasters.Core.Batch.ActionProgressData> progressWriter = null)
        {
            List<string> ids = new List<string>();
            var client = deleted ? ES.Manager.GetESClient_Sneplatne() : ES.Manager.GetESClient();

            Func<int, int, Nest.ISearchResponse<Smlouva>> searchFunc =
                searchFunc = (size, page) =>
                {
                    return client.Search<Smlouva>(a => a
                                .Size(size)
                                .From(page * size)
                                .Source(ss => ss.ExcludeAll())
                                .Query(q => q.Term(t => t.Field(f => f.platnyZaznam).Value(deleted ? false : true)))
                                .Scroll("10m")
                                );
                };


            Lib.ES.SearchTools.DoActionForQuery<Smlouva>(client,
            searchFunc, (hit, param) =>
            {
                ids.Add(hit.Id);
                return new Devmasters.Core.Batch.ActionOutputData() { CancelRunning = false, Log = null };
            }, null, outputWriter, progressWriter, false, blockSize: 100);

            return ids;

        }


        public string ToAuditJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public string ToAuditObjectTypeName()
        {
            return "Smlouva";
        }

        public string ToAuditObjectId()
        {
            return this.Id;
        }

        public string BookmarkName()
        {
            return this.predmet;
        }



        public HlidacStatu.Lib.OCR.Api.CallbackData CallbackDataForOCRReq(int prilohaindex)
        {
            var url = Devmasters.Core.Util.Config.GetConfigValue("ESConnection");

            if (this.platnyZaznam)
                url = url + $"/{Lib.ES.Manager.defaultIndexName}/smlouva/{this.Id}/_update";
            else
                url = url + $"/{Lib.ES.Manager.defaultIndexName_Sneplatne}/smlouva/{this.Id}/_update";

            string callback = HlidacStatu.Lib.OCR.Api.CallbackData.PrepareElasticCallbackDataForOCRReq($"prilohy[{prilohaindex}].plainTextContent", true);
            callback = callback.Replace("#ADDMORE#", $"ctx._source.prilohy[{prilohaindex}].lastUpdate = '#NOW#';"
                + $"ctx._source.prilohy[{prilohaindex}].lenght = #LENGTH#;"
                + $"ctx._source.prilohy[{prilohaindex}].wordCount=#WORDCOUNT#;"
                + $"ctx._source.prilohy[{prilohaindex}].contentType='#CONTENTTYPE#'");

            return new HlidacStatu.Lib.OCR.Api.CallbackData(new Uri(url), callback, HlidacStatu.Lib.OCR.Api.CallbackData.CallbackType.LocalElastic);
        }


    }
}
