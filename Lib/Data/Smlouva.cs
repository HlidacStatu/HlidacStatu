using HlidacStatu.Lib.XSD;
using HlidacStatu.Util;

using Nest;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace HlidacStatu.Lib.Data
{

    public partial class Smlouva
        : Bookmark.IBookmarkable, ISocialInfo, IFlattenedExport
    {

        public tIdentifikator identifikator;
        InfoFact[] _infofacts = null;
        Smlouva[] _otherVersions = null;
        Smlouva[] _podobneSmlouvy = null;
        object enhLock = new object();
        private Issues.Issue[] issues = new Lib.Issues.Issue[] { };
        object lockInfoObj = new object();
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

        public enum PravniRamce
        {
            Undefined = 0,
            Do072017 = 1,
            Od072017 = 2,
            MimoRS = 3
        }

        public decimal CalculatedPriceWithVATinCZK { get; set; }
        public DataQualityEnum CalcutatedPriceQuality { get; set; }
        [Date]
        public System.DateTime casZverejneni { get; set; }

        [Nest.Keyword]
        public string cisloSmlouvy { get; set; }

        public tSmlouvaCiziMena ciziMena { get; set; }
        public SClassification Classification { get; set; } = new SClassification();
        [Nest.Number]
        public decimal ConfidenceValue { get; set; }

        [Date]
        public System.DateTime datumUzavreni { get; set; }

        public Enhancers.Enhancement[] Enhancements { get; set; } = new Enhancers.Enhancement[] { };
        public decimal? hodnotaBezDph { get; set; }

        public decimal? hodnotaVcetneDph { get; set; }

        [Nest.Text]
        public string Id
        {
            get
            {
                return this.identifikator?.idVerze;  //todo: es7 __ ?. __ added, because identifikator was null
            }
        }

        // { get; set; }

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

        [Date]
        public DateTime LastUpdate { get; set; } = DateTime.MinValue;

        [Nest.Keyword]
        public string navazanyZaznam { get; set; }

        [Nest.Keyword]
        public string odkaz { get; set; }
        public Subjekt Platce { get; set; }
        /// <remarks/>
        public bool platnyZaznam { get; set; }

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

        [Nest.Text]
        public string predmet { get; set; }

        public Subjekt[] Prijemce { get; set; }
        public Priloha[] Prilohy { get; set; }
        [Nest.Keyword]
        public string schvalil { get; set; }

        [Nest.Keyword]
        public string[] souvisejiciSmlouvy { get; set; } = null;

        //public tSmlouva smlouva;
        public bool spadaPodRS { get; set; } = true;

        [Nest.Boolean()]
        public bool? SVazbouNaPolitiky { get; set; } = false;

        [Nest.Boolean()]
        public bool? SVazbouNaPolitikyAktualni { get; set; } = false;

        [Nest.Boolean()]
        public bool? SVazbouNaPolitikyNedavne { get; set; } = false;

        public HintSmlouva Hint { get; set; }

        public Subjekt VkladatelDoRejstriku { get; set; }
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

        public void AddSpecificIssue(Issues.Issue i)
        {
            if (!this.Issues.Any(m => m.IssueTypeId == i.IssueTypeId))
            {
                var oldIssues = this.Issues.ToList();
                oldIssues.Add(i);
                this.Issues = oldIssues.ToArray();
            }

        }

        public string BookmarkName()
        {
            return this.predmet;
        }

        public HlidacStatu.Lib.OCR.Api.CallbackData CallbackDataForOCRReq(int prilohaindex)
        {
            var url = Devmasters.Config.GetWebConfigValue("ESConnection");

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

        public void ClearAllIssuesIncludedPermanent()
        {
            this.issues = new Lib.Issues.Issue[] { };
        }

        public bool Delete(ElasticClient client = null)
        {
            return Delete(this.Id);
        }

        public string FullTitle()
        {
            return string.Format("Smlouva č. {0}: {1}", this.Id, Devmasters.TextUtil.ShortenText(this.predmet ?? "", 70));
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
                var res = Search.RawSearch(
                    new QueryContainerDescriptor<Lib.Data.Smlouva>().Bool(b => b.Must(query)),
                        1, numOfResults, Search.OrderResult.DateAddedDesc, null
                    );
                var resN = Search.RawSearch(
                    new QueryContainerDescriptor<Lib.Data.Smlouva>().Bool(b => b.Must(query)),
                        1, numOfResults, Search.OrderResult.DateAddedDesc, null, platnyZaznam: false
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

        public SClassification.Classification[] GetRelevantClassification()
        {
            this.Classification = this.Classification ?? new SClassification();
            var types = this.Classification.GetClassif();
            return types.ToArray();
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

        public InfoFact[] InfoFacts()
        {
            lock (lockInfoObj)
            {
                if (_infofacts == null)
                {
                    List<InfoFact> f = new List<InfoFact>();

                    string hlavni = $"Smlouva mezi {Devmasters.TextUtil.ShortenText(this.Platce.nazev, 60)} a "
                        + $"{Devmasters.TextUtil.ShortenText(this.Prijemce.First().nazev, 60)}";
                    if (this.Prijemce.Count() == 0)
                        hlavni += $".";
                    else if (this.Prijemce.Count() == 1)
                        hlavni += $" a 1 dalším.";
                    else if (this.Prijemce.Count() > 1)
                        hlavni += $" a {this.Prijemce.Count() - 1} dalšími.";
                    hlavni += (this.CalculatedPriceWithVATinCZK == 0
                        ? " Hodnota smlouvy je utajena."
                        : " Hodnota smlouvy je " + HlidacStatu.Util.RenderData
                        .ShortNicePrice(this.CalculatedPriceWithVATinCZK, html: true, showDecimal: RenderData.ShowDecimalVal.Show ));

                    f.Add(new InfoFact(hlavni, InfoFact.ImportanceLevel.Summary));

                    //sponzori
                    foreach (var subj in this.Prijemce.Union(new Subjekt[] { this.Platce }))
                    {
                        var firma = Firmy.Get(subj.ico);
                        if (firma.Valid && firma.IsSponzor() && firma.JsemSoukromaFirma())
                        {
                            f.Add(new InfoFact(
                                $"{firma.Jmeno}: " +
                                string.Join("<br />",
                                    firma.Sponzoring()
                                        .OrderByDescending(s => s.DarovanoDne)
                                        .Select(s => s.ToHtml())
                                        .Take(2)),
                                InfoFact.ImportanceLevel.Medium)
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
                                    + Devmasters.Lang.Plural.Get(politici.Count()
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

        public bool IsPartOfRegistrSmluv()
        {
            if (this.Id != null && this.Id.StartsWith("pre")) //todo: es7 __this.Id != null && __ added
                return false;
            else
                return true;
        }

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

        public string NicePrice(bool html = false)
        {
            string res = "Neuvedena";
            if (this.CalculatedPriceWithVATinCZK == 0)
                return res;
            else
                return NicePrice(this.CalculatedPriceWithVATinCZK, html: html);
        }

        public Smlouva[] OtherVersions()
        {
            var result = new List<Smlouva>();
            if (_otherVersions == null)
            {
                var res = Search.SimpleSearch("identifikator.idSmlouvy:" + this.identifikator.idSmlouvy,
                    1, 50, Search.OrderResult.DateAddedDesc, null
                    );
                var resNeplatne = Search.SimpleSearch("identifikator.idSmlouvy:" + this.identifikator.idSmlouvy,
                    1, 50, Search.OrderResult.DateAddedDesc, null, platnyZaznam: false
                    );

                if (res.IsValid == false)
                    HlidacStatu.Lib.ES.Manager.LogQueryError<Smlouva>(res.ElasticResults);
                else
                    result.AddRange(res.ElasticResults.Hits.Select(m => m.Source).Where(m => m.Id != this.Id));

                if (resNeplatne.IsValid == false)
                    HlidacStatu.Lib.ES.Manager.LogQueryError<Smlouva>(resNeplatne.ElasticResults);
                else
                    result.AddRange(resNeplatne.ElasticResults.Hits.Select(m => m.Source).Where(m => m.Id != this.Id));

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

        public void PrepareBeforeSave(bool updateLastUpdateValue = true)
        {
            this.SVazbouNaPolitiky = this.JeSmlouva_S_VazbouNaPolitiky(Relation.AktualnostType.Libovolny);
            this.SVazbouNaPolitikyNedavne = this.JeSmlouva_S_VazbouNaPolitiky(Relation.AktualnostType.Nedavny);
            this.SVazbouNaPolitikyAktualni = this.JeSmlouva_S_VazbouNaPolitiky(Relation.AktualnostType.Aktualni);

            if (updateLastUpdateValue)
                this.LastUpdate = DateTime.Now;

            this.ConfidenceValue = GetConfidenceValue();

            /////// HINTS

            if (this.Hint == null)
                this.Hint = new HintSmlouva();

            Firma fPlatce = Firmy.Get(this.Platce.ico);
            Firma[] fPrijemci = this.Prijemce.Select(m => m.ico)
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .Select(m => Firmy.Get(m))
                .Where(f => f.Valid)
                .ToArray();

            List<Firma> firmy = fPrijemci
                .Concat(new Firma[] { fPlatce })
                .Where(f => f.Valid)
                .ToList();

            this.Hint.DenUzavreni = (int)Devmasters.DT.Util.TypeOfDay(this.datumUzavreni);

            if (firmy.Count() == 0)
                this.Hint.PocetDniOdZalozeniFirmy = 9999;
            else
                this.Hint.PocetDniOdZalozeniFirmy = (int)firmy
                    .Select(f => (this.datumUzavreni - (f.Datum_Zapisu_OR ?? new DateTime(1990, 1, 1))).TotalDays)
                    .Min();

            this.Hint.SmlouvaSPolitickyAngazovanymSubjektem = (int)HintSmlouva.PolitickaAngazovanostTyp.Neni;
            if (firmy.Any(f => f.IsSponzorBefore(this.datumUzavreni)))
                this.Hint.SmlouvaSPolitickyAngazovanymSubjektem = (int)HintSmlouva.PolitickaAngazovanostTyp.PrimoSubjekt;
            else if (firmy.Any(f => f.MaVazbyNaPolitikyPred(this.datumUzavreni)))
                this.Hint.SmlouvaSPolitickyAngazovanymSubjektem = (int)HintSmlouva.PolitickaAngazovanostTyp.AngazovanyMajitel;

            if (fPlatce.Valid && fPlatce.PatrimStatu())
            {
                if (fPrijemci.All(f => f.PatrimStatu()))
                    this.Hint.VztahSeSoukromymSubjektem = (int)HintSmlouva.VztahSeSoukromymSubjektemTyp.PouzeStatStat;
                else if (fPrijemci.All(f => f.PatrimStatu() == false))
                    this.Hint.VztahSeSoukromymSubjektem = (int)HintSmlouva.VztahSeSoukromymSubjektemTyp.PouzeStatSoukr;
                else 
                    this.Hint.VztahSeSoukromymSubjektem = (int)HintSmlouva.VztahSeSoukromymSubjektemTyp.Kombinovane;
            }
            if (fPlatce.Valid && fPlatce.PatrimStatu()==false)
            {
                if (fPrijemci.All(f => f.PatrimStatu()))
                    this.Hint.VztahSeSoukromymSubjektem = (int)HintSmlouva.VztahSeSoukromymSubjektemTyp.PouzeStatSoukr;
                else if (fPrijemci.All(f => f.PatrimStatu() == false))
                    this.Hint.VztahSeSoukromymSubjektem = (int)HintSmlouva.VztahSeSoukromymSubjektemTyp.PouzeSoukrSoukr;
                else
                    this.Hint.VztahSeSoukromymSubjektem = (int)HintSmlouva.VztahSeSoukromymSubjektemTyp.Kombinovane;
            }

            //U limitu
            this.Hint.SmlouvaULimitu = (int)HintSmlouva.ULimituTyp.OK;
            if (
                    (
                    this.hodnotaBezDph >= Analysis.KorupcniRiziko.Consts.Limit1bezDPH_From
                    && this.hodnotaBezDph <= Analysis.KorupcniRiziko.Consts.Limit1bezDPH_To
                    )
                ||
                    (
                        this.CalculatedPriceWithVATinCZK > Analysis.KorupcniRiziko.Consts.Limit1bezDPH_From * 1.21m
                        && this.CalculatedPriceWithVATinCZK <= Analysis.KorupcniRiziko.Consts.Limit1bezDPH_To * 1.21m
                    )
                )
                this.Hint.SmlouvaULimitu = (int)HintSmlouva.ULimituTyp.Limit2M;

            if (
                    (
                    this.hodnotaBezDph >= Analysis.KorupcniRiziko.Consts.Limit2bezDPH_From
                    && this.hodnotaBezDph <= Analysis.KorupcniRiziko.Consts.Limit2bezDPH_To
                    )
                ||
                    (
                        this.CalculatedPriceWithVATinCZK > Analysis.KorupcniRiziko.Consts.Limit2bezDPH_From * 1.21m
                        && this.CalculatedPriceWithVATinCZK <= Analysis.KorupcniRiziko.Consts.Limit2bezDPH_To * 1.21m
                    )
                )
                this.Hint.SmlouvaULimitu = (int)HintSmlouva.ULimituTyp.Limit6M;

            if (this.Prilohy != null)
            {
                foreach (var p in this.Prilohy)
                    p.UpdateStatistics();
            }

        }

        public bool Save(ElasticClient client = null, bool updateLastUpdateValue = true)
        {
            var item = this;
            if (item == null)
                return false;

            item.PrepareBeforeSave(updateLastUpdateValue);
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
                    Delete(item.Id, cExist);
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

                            using (Devmasters.Net.HttpClient.URLContent url = new Devmasters.Net.HttpClient.URLContent(attUrl))
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

        public bool SetClassification(bool rewrite = false, bool rewriteStems = false) //true if changed
        {
            if (this.Prilohy != null
                    &&
                    this.Prilohy.Any(m => m.EnoughExtractedText))
            {

                if (rewrite
                || rewriteStems
                || this.Classification?.LastUpdate == null
                || (this.Classification?.GetClassif() != null && this.Classification.GetClassif().Count() == 0)

                )
                {
                    var types = SClassification.GetClassificationFromServer(this, rewriteStems);
                    if (types == null)
                    {
                        this.Classification = null;
                    }
                    else
                    {
                        SClassification.Classification[] newClass = types
                                                .Select(m => new SClassification.Classification()
                                                {
                                                    TypeValue = (int)m.Key,
                                                    ClassifProbability = m.Value
                                                }
                                                )
                                                .ToArray();

                        var newClassRelevant = relevantClassif(newClass);
                        this.Classification = new SClassification(newClassRelevant);
                        this.Classification.LastUpdate = DateTime.Now;
                    }
                    return true;
                } //class
                else
                    return false;
            } //prilohy
            else
                return false;
        }


        public bool NotInterestingToShow() { return false; }

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

        public string SocialInfoSubTitle()
        {

            if (this.Issues.Any(m => m.Importance == HlidacStatu.Lib.Issues.ImportanceLevel.Fatal))
                return "Smlouva je formálně platná, ale <b>obsahuje závažné nedostatky v rozporu se zákonem!</b>";
            else
                return (this.znepristupnenaSmlouva() ? "Zneplatněná smlouva." : "Platná smlouva.");

        }

        public string SocialInfoTitle()
        {
            return Devmasters.TextUtil.ShortenText(this.predmet, 45, "...");
        }

        public string ToAuditJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public string ToAuditObjectId()
        {
            return this.Id;
        }

        public string ToAuditObjectTypeName()
        {
            return "Smlouva";
        }

        public bool znepristupnenaSmlouva()
        {
            var b = this.Issues != null && this.Issues.Any(m => m.IssueTypeId == -1);
            if (this.IsPartOfRegistrSmluv() == false)
                b = false;
            return (b || platnyZaznam == false);
        }

        public void ZmenStavSmlouvyNa(bool platnyZaznam)
        {
            var issueTypeId = -1;
            var issue = new Lib.Issues.Issue(null, issueTypeId, "Smlouva byla znepřístupněna", "Na žádost subjektu byla tato smlouva znepřístupněna.", Lib.Issues.ImportanceLevel.Formal, permanent: true);

            if (platnyZaznam && this.znepristupnenaSmlouva())
            {
                this.platnyZaznam = platnyZaznam;
                //zmen na platnou
                if (this.Issues.Any(m => m.IssueTypeId == issueTypeId))
                {
                    this.Issues = this.Issues
                        .Where(m => m.IssueTypeId != issueTypeId)
                        .ToArray();

                }
                this.Save();
            }
            else if (platnyZaznam == false && this.znepristupnenaSmlouva() == false)
            {
                this.platnyZaznam = platnyZaznam;
                if (!this.Issues.Any(m => m.IssueTypeId == -1))
                    this.AddSpecificIssue(issue);
                this.Save();
            }
        }

        public bool? PlatnostZaznamuVRS()
        {
            if (this.IsPartOfRegistrSmluv())
            {
                try
                {

                    string html = "";
                    using (Devmasters.Net.HttpClient.URLContent net = new Devmasters.Net.HttpClient.URLContent(this.odkaz))
                    {
                        net.Timeout = 3000;
                        html = net.GetContent().Text;
                    }
                    bool existuje = html?.Contains("Smlouva byla znepřístupněna") == false;
                    if (existuje)
                    {
                        //download xml and check
                        //https://smlouvy.gov.cz/smlouva/9569679/xml/registr_smluv_smlouva_9569679.xml
                        //https://smlouvy.gov.cz/smlouva/13515796/xml/registr_smluv_smlouva_13515796.xml
                        var xmlUrl = $"https://smlouvy.gov.cz/smlouva/{this.identifikator.idVerze}/xml/registr_smluv_smlouva_{this.identifikator.idVerze}.xml";
                        using (Devmasters.Net.HttpClient.URLContent net = new Devmasters.Net.HttpClient.URLContent(xmlUrl))
                        {
                            net.Timeout = 3000;
                            html = net.GetContent().Text;
                            using (System.IO.StringReader sr = new StringReader(html))
                            {
                                try
                                {
                                    System.Xml.Serialization.XmlSerializer xmlsZaznam = new System.Xml.Serialization.XmlSerializer(typeof(Lib.XSD.zaznam));
                                    var zaznam = xmlsZaznam.Deserialize(sr) as Lib.XSD.zaznam;
                                    if (zaznam != null)
                                        return zaznam.data.platnyZaznam;
                                    else
                                        return null;
                                }
                                catch
                                {
                                    return null;
                                }

                            }

                        }

                    }
                    else
                        return false;

                }
                catch (Exception e)
                {
                    HlidacStatu.Util.Consts.Logger.Error($"checking StavSmlouvyVRS id:{this.Id} {this.odkaz}", e);
                    return null;
                }
            }
            else //pokud nespada pod RS, pak je vzdy platna
                return true;

        }

        internal SClassification.Classification[] relevantClassif(SClassification.Classification[] types)
        {
            types = types ?? new SClassification.Classification[] { };
            var firstT = types.OrderByDescending(m => m.ClassifProbability)
                .Where(m => m.ClassifProbability >= SClassification.MinAcceptablePoints)
                .FirstOrDefault();
            if (firstT == null)
                return new SClassification.Classification[] { };
            var secondT = types.OrderByDescending(m => m.ClassifProbability)
                .Skip(1)
                .Where(m => m.ClassifProbability >= SClassification.MinAcceptablePointsSecond)
                .FirstOrDefault();

            var thirdT = types.OrderByDescending(m => m.ClassifProbability)
                .Skip(2)
                .Where(m => m.ClassifProbability >= SClassification.MinAcceptablePointsThird)
                .FirstOrDefault();


            SClassification.Classification[] vals = new SClassification.Classification[] { firstT, secondT, thirdT };
            return vals.Where(m => m != null).ToArray();

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

        public static IEnumerable<string> _allIdsFromES(bool deleted, Action<string> outputWriter = null, Action<Devmasters.Batch.ActionProgressData> progressWriter = null)
        {
            List<string> ids = new List<string>();
            var client = deleted ? ES.Manager.GetESClient_Sneplatne() : ES.Manager.GetESClient();

            Func<int, int, Nest.ISearchResponse<Smlouva>> searchFunc =
                searchFunc = (size, page) =>
                {
                    return client.Search<Smlouva>(a => a
                                .Size(size)
                                .From(page * size)
                                .Source(false)
                                .Query(q => q.Term(t => t.Field(f => f.platnyZaznam).Value(deleted ? false : true)))
                                .Scroll("1m")
                                );
                };


            Searching.Tools.DoActionForQuery<Smlouva>(client,
            searchFunc, (hit, param) =>
            {
                ids.Add(hit.Id);
                return new Devmasters.Batch.ActionOutputData() { CancelRunning = false, Log = null };
            }, null, outputWriter, progressWriter, false, blockSize: 100);

            return ids;

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

        public static IEnumerable<string> AllIdsFromES()
        {
            return AllIdsFromES(null);
        }

        public static IEnumerable<string> AllIdsFromES(bool? deleted, Action<string> outputWriter = null, Action<Devmasters.Batch.ActionProgressData> progressWriter = null)
        {
            if (deleted.HasValue)
                return _allIdsFromES(deleted.Value, outputWriter, progressWriter);
            else
                return
                    _allIdsFromES(false, outputWriter, progressWriter)
                    .Union(_allIdsFromES(true, outputWriter, progressWriter))
                    ;
        }

        public static bool Delete(string Id, ElasticClient client = null)
        {
            if (client == null)
                client = Lib.ES.Manager.GetESClient();
            var res = client
                .Delete<Lib.Data.Smlouva>(Id);
            return res.IsValid;
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
                return res.Exists;
            }
            else
                return res.Exists;

        }

        public static System.Text.StringBuilder ExportData(string query, int count, string order,
            Lib.Searching.ExportDataFormat format, bool withPlainText, out string contenttype)
        {
            //TODO ignored format

            contenttype = "text/tab-separated-values";

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (count > 10000)
                count = 10000;

            Func<int, int, Nest.ISearchResponse<Lib.Data.Smlouva>> searchFunc =
                (size, page) =>
            {
                return Lib.ES.Manager.GetESClient().Search<Lib.Data.Smlouva>(a => a
                            .Size(size)
                            .Source(m => m.Excludes(e => e.Field(o => o.Prilohy)))
                            .From(page * size)
                            .Query(q => Lib.Data.Smlouva.Search.GetSimpleQuery(query))
                            .Scroll("1m")
                            );
            };

            sb.AppendLine("URL\tID smlouvy\tPodepsána\tZveřejněna\tHodnota smlouvy\tPředmět smlouvy\tPlátce\tPlatce IC\tDodavatele a jejich ICO");
            int c = 0;
            Searching.Tools.DoActionForQuery<Lib.Data.Smlouva>(Lib.ES.Manager.GetESClient(),
                searchFunc, (hit, param) =>
                {
                    var s = hit.Source;
                    sb.AppendLine(
                        s.GetUrl(false) + "\t"
                        + s.Id + "\t"
                        + s.datumUzavreni.ToString("dd.MM.yyyy") + "\t"
                        + s.casZverejneni.ToString("dd.MM.yyyy") + "\t"
                        + s.CalculatedPriceWithVATinCZK.ToString(Util.Consts.czCulture) + "\t"
                        + Devmasters.TextUtil.NormalizeToBlockText(s.predmet) + "\t"
                        + s.Platce.nazev + "\t"
                        + s.Platce.ico + "\t"
                        + ((s.Prijemce?.Count() > 0) ?
                            s.Prijemce.Select(p => p.nazev + "\t" + p.ico).Aggregate((f, sec) => f + "\t" + sec)
                            : "")
                        );

                    Console.Write(c++);
                    return new Devmasters.Batch.ActionOutputData() { CancelRunning = false, Log = null };
                }, null, null, null, false);





            return sb;
        }


        public static Smlouva Export(Smlouva smlouva, bool allData = false, bool docsContent = true)
        {
            Smlouva s = (Smlouva)smlouva.MemberwiseClone();
            if (s == null)
                return null;

            if (s.znepristupnenaSmlouva() && s.Prilohy != null)
            {
                foreach (var p in s.Prilohy)
                {
                    p.PlainTextContent = "-- anonymizovano serverem HlidacStatu.cz --";
                    p.odkaz = "";
                }
            }

            if (allData == false)
            {
                if (s.Prilohy != null)
                {
                    foreach (var p in s.Prilohy)
                    {
                        p.FileMetadata = null;
                    }
                }
                s.Classification = null;
                s.SVazbouNaPolitiky = null;
                s.SVazbouNaPolitikyAktualni = null;
                s.SVazbouNaPolitikyNedavne = null;
            }
            if (docsContent == false)
                if (s.Prilohy != null)
                {
                    foreach (var p in s.Prilohy)
                    {
                        p.PlainTextContent = null;
                    }
                }

            //var ret = Newtonsoft.Json.JsonConvert.SerializeObject(s,
            //    new JsonSerializerSettings()
            //    {
            //        Formatting = formatted ? Formatting.Indented : Formatting.None,
            //        //NullValueHandling = NullValueHandling.Ignore,         
            //        ContractResolver = new HlidacStatu.Util.FirstCaseLowercaseContractResolver()
            //    }

            //    );

            if (allData == false)
            {
                //Composite formatting { escaping

                //var licence = "\"{0}\":{{ \"note\":\"-- Tato data jsou dostupná pouze v komerční nebo speciální licenci. Kontaktujte nás. --\" }}";
                //ret = HlidacStatu.Util.ParseTools.GetStringReplaceWithRegex("\"classification\": \\s? null", ret, string.Format(licence, "classification"));
                //ret = HlidacStatu.Util.ParseTools.GetStringReplaceWithRegex("\"sVazbouNaPolitiky\": \\s? null", ret, string.Format(licence, "sVazbouNaPolitiky"));
                //ret = HlidacStatu.Util.ParseTools.GetStringReplaceWithRegex("\"sVazbouNaPolitikyNedavne\": \\s? null", ret, string.Format(licence, "sVazbouNaPolitikyNedavne"));
                //ret = HlidacStatu.Util.ParseTools.GetStringReplaceWithRegex("\"sVazbouNaPolitikyAktualni\": \\s? null", ret, string.Format(licence, "sVazbouNaPolitikyAktualni"));
                s.Classification = null;
                s.SVazbouNaPolitiky = null;
                s.SVazbouNaPolitikyAktualni = null;
                s.SVazbouNaPolitikyNedavne = null;
            }

            return s;
        }
        public static Smlouva Load(string idVerze, ElasticClient client = null, bool includePrilohy = true)
        {
            var s = _load(idVerze, client, includePrilohy);
            if (s == null)
                return s;
            var sclass = s.GetRelevantClassification();
            if (s.Classification?.Version == 1 &&  s.Classification?.Types != null)
            {
                s.Classification.ConvertToV2();
                s.Save(null, false);
            }
            return s;
        }
            private static Smlouva _load(string idVerze, ElasticClient client = null, bool includePrilohy = true)
        {
            bool specClient = client != null;
            try
            {
                ElasticClient c = null;
                if (specClient)
                    c = client;
                else
                    c = Lib.ES.Manager.GetESClient();

                //var res = c.Get<Lib.Data.Smlouva>(idVerze);

                var res = includePrilohy
                    ? c.Get<Smlouva>(idVerze)
                    : c.Get<Smlouva>(idVerze, s => s.SourceExcludes(sml => sml.Prilohy));


                if (res.Found)
                    return res.Source;
                else
                {
                    if (specClient == false)
                    {
                        var c1 = ES.Manager.GetESClient_Sneplatne();

                        res = includePrilohy
                            ? c1.Get<Smlouva>(idVerze)
                            : c1.Get<Smlouva>(idVerze, s => s.SourceExcludes(sml => sml.Prilohy));

                        if (res.Found)
                            return res.Source;
                        else if (res.IsValid)
                        {
                            ES.Manager.ESLogger.Warning("Valid Req: Cannot load Smlouva Id " + idVerze + "\nDebug:" + res.DebugInformation);
                            //DirectDB.NoResult("delete from SmlouvyIds where id = @id", new System.Data.SqlClient.SqlParameter("id", idVerze));
                        }
                        else if (res.Found == false)
                            return null;
                        else if (res.ServerError.Status == 404)
                            return null;
                        else
                        {
                            ES.Manager.ESLogger.Error("Invalid Req: Cannot load Smlouva Id " + idVerze + "\n Debug:" + res.DebugInformation + " \nServerError:" + res.ServerError?.ToString(), res.OriginalException);
                        }
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

        public ExpandoObject FlatExport()
        {
            dynamic v = new System.Dynamic.ExpandoObject();
            v.url = this.GetUrl(false);
            v.id = this.Id;
            v.predmet = this.predmet;
            v.datumUzavreni = this.datumUzavreni;
            v.casZverejneni = this.casZverejneni;
            v.hodnotaSmlouvy_sDPH = this.CalculatedPriceWithVATinCZK;
            v.platceJmeno = this.Platce.nazev;
            v.platceIco = this.Platce.ico;
            for (int i = 0; i < this.Prijemce.Count(); i++)
            {
                ((IDictionary<String, Object>)v).Add($"prijemceJmeno_{i + 1}", this.Prijemce[i].nazev);
                ((IDictionary<String, Object>)v).Add($"prijemceIco_{i + 1}", this.Prijemce[i].ico);
            }
            return v;
        }

        static HashSet<string> ico_s_VazbouPolitik = new HashSet<string>(
                                                                                                                                    StaticData.FirmySVazbamiNaPolitiky_vsechny_Cache.Get().SoukromeFirmy.Select(m => m.Key)
                .Union(StaticData.SponzorujiciFirmy_Vsechny.Get().Select(m => m.IcoDarce))
                .Distinct()
            );

        static HashSet<string> ico_s_VazbouPolitikAktualni = new HashSet<string>(
                    StaticData.FirmySVazbamiNaPolitiky_aktualni_Cache.Get().SoukromeFirmy.Select(m => m.Key)
                .Union(StaticData.SponzorujiciFirmy_Nedavne.Get().Select(m => m.IcoDarce))
                .Distinct()
            );

        static HashSet<string> ico_s_VazbouPolitikNedavne = new HashSet<string>(
                    StaticData.FirmySVazbamiNaPolitiky_nedavne_Cache.Get().SoukromeFirmy.Select(m => m.Key)
                .Union(StaticData.SponzorujiciFirmy_Nedavne.Get().Select(m => m.IcoDarce))
                .Distinct()
            );

        static DateTime pravniRamce01072017 = new DateTime(2017, 7, 1);
    }
}
