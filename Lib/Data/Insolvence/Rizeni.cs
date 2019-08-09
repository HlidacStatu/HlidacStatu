using System;
using System.Linq;
using System.Collections.Generic;
using HlidacStatu.Util;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Data.Entity.Infrastructure;
using static Devmasters.Core.DateTimeUtil;
using HlidacStatu.Lib.Db.Insolvence;

namespace HlidacStatu.Lib.Data.Insolvence
{
    public class Rizeni
        : Bookmark.IBookmarkable, Util.ISocialInfo
    {

        static DateTime MinSqlDate = new DateTime(1753, 1, 1); // 01/01/1753 
        public Rizeni()
        {
            Dokumenty = new List<Dokument>();
            Dluznici = new List<Osoba>();
            Veritele = new List<Osoba>();
            Spravci = new List<Osoba>();
        }

        [Nest.Object(Ignore = true)]
        public bool IsFullRecord { get; set; } = false;

        [Nest.Keyword]
        public string SpisovaZnacka { get; set; }
        [Nest.Keyword]
        public string Stav { get; set; }
        [Nest.Date]
        public DateTime? Vyskrtnuto { get; set; }
        [Nest.Keyword]
        public string Url { get; set; }
        [Nest.Date]
        public DateTime? DatumZalozeni { get; set; }
        [Nest.Date]
        public DateTime PosledniZmena { get; set; }
        [Nest.Keyword]
        public string Soud { get; set; }
        [Nest.Object]
        public List<Dokument> Dokumenty { get; set; }
        [Nest.Object]
        public List<Osoba> Dluznici { get; set; }
        [Nest.Object]
        public List<Osoba> Veritele { get; set; }
        [Nest.Object]
        public List<Osoba> Spravci { get; set; }

        [Nest.Boolean]
        public bool OnRadar { get; set; } = false;

        public string UrlId() => SpisovaZnacka.Replace(" ", "_").Replace("/", "-");


        public void PrepareForSave()
        {
            if (Dluznici.Any(m => m.Typ != "F"))
                this.OnRadar = true;
            else
            {
                foreach (var d in Dluznici)
                {

                    if (StaticData.Politici.Get().Any(m =>
                        m.JmenoAscii == Devmasters.Core.TextUtil.RemoveDiacritics(d.Jmeno())
                        && m.PrijmeniAscii == Devmasters.Core.TextUtil.RemoveDiacritics(d.Prijmeni())
                        && m.Narozeni == d.GetDatumNarozeni() && d.GetDatumNarozeni().HasValue
                        )
                    )
                        this.OnRadar = true;
                    break;
                }
            }
        }

        public HlidacStatu.Lib.OCR.Api.CallbackData CallbackDataForOCRReq(int prilohaindex)
        {
            var url = Devmasters.Core.Util.Config.GetConfigValue("ESConnection");

            url = url + $"/{Lib.ES.Manager.defaultIndexName_Insolvence}/rizeni/{System.Net.WebUtility.UrlEncode(this.SpisovaZnacka)}/_update";

            string callback = HlidacStatu.Lib.OCR.Api.CallbackData.PrepareElasticCallbackDataForOCRReq($"dokumenty[{prilohaindex}].plainText", true);
            callback = callback.Replace("#ADDMORE#", $"ctx._source.dokumenty[{prilohaindex}].lastUpdate = '#NOW#';"
                + $"ctx._source.dokumenty[{prilohaindex}].lenght = #LENGTH#;"
                + $"ctx._source.dokumenty[{prilohaindex}].wordCount=#WORDCOUNT#;");

            return new HlidacStatu.Lib.OCR.Api.CallbackData(new Uri(url), callback, HlidacStatu.Lib.OCR.Api.CallbackData.CallbackType.LocalElastic);
        }

        public string GetUrl(bool local = true)
        {
            return GetUrl(local, string.Empty);
        }

        public string GetUrl(bool local, string foundWithQuery)
        {
            string url = $"/Insolvence/Rizeni/{this.UrlId()}";
            if (!string.IsNullOrEmpty(foundWithQuery))
                url = url + "?qs=" + System.Net.WebUtility.UrlEncode(foundWithQuery);
            if (local == false)
                url = "https://www.hlidacstatu.cz" + url;

            return url;
        }

        public class ProgressItem
        {
            public enum ProgressStatus
            {
                Done,
                GoOn,
                InQueue
            }
            public string Text { get; set; }
            public DateTime Date { get; set; }
            public ProgressStatus Status { get; set; }
        }

        public ProgressItem[] StavRizeniProgress()
        {
            List<ProgressItem> l = new List<ProgressItem>();
            if (this.Stav == Insolvence.StavRizeni.Nevyrizena || this.Stav == Insolvence.StavRizeni.Obzivla)
                l.Add(new ProgressItem() { Text = "Dokazování", Status = ProgressItem.ProgressStatus.GoOn });
            else
                l.Add(new ProgressItem() { Text = "Dokazování", Status = ProgressItem.ProgressStatus.Done });

            if (this.Stav == Insolvence.StavRizeni.MylnyZapis)
                l.Add(new ProgressItem() { Text = "Řízení zrušeno", Status = ProgressItem.ProgressStatus.Done });
            else
            {
                if (this.Stav == Insolvence.StavRizeni.Moratorium)
                    l.Add(new ProgressItem() { Text = "Odklad splatnosti", Status = ProgressItem.ProgressStatus.GoOn });

                //add all other missing steps
                var s1 = new ProgressItem() { Text = "Řešení úpadku", Status = ProgressItem.ProgressStatus.InQueue };
                var s2 = new ProgressItem() { Text = "Řízení skončeno", Status = ProgressItem.ProgressStatus.InQueue };
                var s3 = new ProgressItem() { Text = "Odškrtnuto", Status = ProgressItem.ProgressStatus.InQueue };
                l.Add(s1); l.Add(s2); l.Add(s3);

                if (new[] { Insolvence.StavRizeni.Konkurs, Insolvence.StavRizeni.Oddluzeni, Insolvence.StavRizeni.Upadek, Insolvence.StavRizeni.Reorganizace, Insolvence.StavRizeni.Zruseno, Insolvence.StavRizeni.PostoupenaVec, Insolvence.StavRizeni.KonkursPoZruseni }
                    .Contains(this.Stav))
                {
                    s1.Status = ProgressItem.ProgressStatus.GoOn;
                }

                if (this.Stav == Insolvence.StavRizeni.Vyrizena || this.Stav == Insolvence.StavRizeni.Pravomocna)
                {
                    s1.Status = ProgressItem.ProgressStatus.Done;
                    s2.Status = ProgressItem.ProgressStatus.GoOn;
                }

                if (this.Stav == Insolvence.StavRizeni.Odskrtnuta)
                {
                    s1.Status = ProgressItem.ProgressStatus.Done;
                    s2.Status = ProgressItem.ProgressStatus.Done;
                    s3.Status = ProgressItem.ProgressStatus.GoOn;
                }


            }


            return l.ToArray();
        }



        public string SoudFullName()
        {
            //zdroj dat: https://ispis.cz/soudy

            switch (this.Soud)
            {
                case "NS":
                    return "Nejvyšší soud";

                case "US":
                    return "Ústavní soud";

                case "NSS":
                    return "Nej. Správní soud";

                case "KSJIMBM":
                    return "Krajský soud v Brně";

                case "KSJICCB":
                    return "Krajský soud v Českých Budějovicích";

                case "KSVYCHK":
                case "KSVYCHKP1":
                    return "Krajský soud v Hradci Králové";

                case "KSVYCPA":
                    return "Krajský soud v Hradci Králové pobočka Pardubice";

                case "KSSEMOS":
                case "KSSEMOSP1":
                    return "Krajský soud v Ostravě";

                case "KSSEMOC":
                    return "Krajský soud v Ostravě pobočka Olomouc";

                case "KSZPCPM":
                    return "Krajský soud v Plzni";

                case "KSSTCAB":
                    return "Krajský soud v Praze";

                case "KSSCEUL":
                    return "Krajský soud v Ústí nad Labem";

                case "KSSCELB":
                case "KSSECULP1":
                    return "Krajský soud v Ústí nad Labem pobočka Liberec";

                case "MSPHAAB":
                    return "Městský soud v Praze";

                case "VSSTCAB":
                    return "Vrchní soud v Praze";

                case "VSSEMOC":
                    return "Vrchní soud v Olomouci";

                case "OSJIMBM":
                    return "Městský soud v Brně";

                case "OSPHA01":
                    return "Obvodní soud pro Prahu 1";

                case "OSPHA02":
                    return "Obvodní soud pro Prahu 2";

                case "OSPHA03":
                    return "Obvodní soud pro Prahu 3";

                case "OSPHA04":
                    return "Obvodní soud pro Prahu 4";

                case "OSPHA05":
                    return "Obvodní soud pro Prahu 5";

                case "OSPHA06":
                    return "Obvodní soud pro Prahu 6";

                case "OSPHA07":
                    return "Obvodní soud pro Prahu 7";

                case "OSPHA08":
                    return "Obvodní soud pro Prahu 8";

                case "OSPHA09":
                    return "Obvodní soud pro Prahu 9";

                case "OSPHA10":
                    return "Obvodní soud pro Prahu 10";

                case "OSSTCBN":
                    return "Okresní soud v Benešově";

                case "OSSTCBE":
                    return "Okresní soud v Berouně";

                case "OSJIMBK":
                    return "Okresní soud v Blansku";

                case "OSJIMBO":
                    return "Okresní soud Brno-Venkov";

                case "OSSEMBR":
                    return "Okresní soud v Bruntále";

                case "OSSEMKR":
                    return "Okresní soud v Bruntále - poboèka Krnov";

                case "OSJIMBV":
                    return "Okresní soud v Břeclavi";

                case "OSSCECL":
                    return "Okresní soud v České Lípě";

                case "OSJICCK":
                    return "Okresní soud v Českém Krumlově";

                case "OSJICCB":
                    return "Okresní soud v Českých Budějovicích";

                case "OSZPCCH":
                    return "Okresní soud v Chebu";

                case "OSSCECV":
                    return "Okresní soud v Chomutově";

                case "OSVYCCR":
                    return "Okresní soud v Chrudimi";

                case "OSSCEDC":
                    return "Okresní soud v Děčíně";

                case "OSZPCDO":
                    return "Okresní soud v Domažlicích";

                case "OSSEMFM":
                    return "Okresní soud ve Frýdku-Místku";

                case "OSVYCHB":
                    return "Okresní soud v Havlíčkovì Brodě";

                case "OSJIMHO":
                    return "Okresní soud v Hodoníně";

                case "OSVYCHK":
                    return "Okresní soud v Hradci Králové";

                case "OSSCEJN":
                    return "Okresní soud v Jablonci nad Nisou";

                case "OSSEMJE":
                    return "Okresní soud v Jeseníku";

                case "OSJIMJI":
                    return "Okresní soud Jihlava";

                case "OSVYCJC":
                    return "Okresní soud v Jičíně";

                case "OSJICJH":
                    return "Okresní soud v Jindřichově Hradci";

                case "OSZPCKV":
                    return "Okresní soud v Karlových Varech";

                case "OSSEMKA":
                    return "Okresní soud v Karviné";

                case "OSSEMHA":
                    return "Okresní soud v Karviné pobočka Havířov";

                case "OSSTCKL":
                    return "Okresní soud v Kladně";

                case "OSZPCKT":
                    return "Okresní soud v Klatovech";

                case "OSSTCKO":
                    return "Okresní soud v Kolíně";

                case "OSJIMKM":
                    return "Okresní soud v Kroměříži";

                case "OSSTCKH":
                    return "Okresní soud v Kutné Hoře";

                case "OSSCELB":
                    return "Okresní soud v Liberci";

                case "OSSCELT":
                    return "Okresní soud v Litoměřicích";

                case "OSSCELN":
                    return "Okresní soud v Lounech";

                case "OSSTCME":
                    return "Okresní soud v Mělníku";

                case "OSSTCMB":
                    return "Okresní soud v Mladé Boleslavi";

                case "OSSCEMO":
                    return "Okresní soud v Mostě";

                case "OSVYCNA":
                    return "Okresní soud v Náchodě";

                case "OSSEMNJ":
                    return "Okresní soud v Novém Jičíně";

                case "OSSTCNB":
                    return "Okresní soud v Nymburce";

                case "OSSEMOC":
                    return "Okresní soud v Olomouci";

                case "OSSEMOP":
                    return "Okresní soud v Opavě";

                case "OSSEMOS":
                    return "Okresní soud v Ostravě";

                case "OSVYCPA":
                    return "Okresní soud v Pardubicích";

                case "OSJICPE":
                    return "Okresní soud v Pelhřimově";

                case "OSJICPI":
                    return "Okresní soud v Písku";

                case "OSSEMPR":
                    return "Okresní soud v Přerově";

                case "OSSTCPB":
                    return "Okresní soud v Příbrami";

                case "OSZPCPJ":
                    return "Okresní soud Plzeň-jih";

                case "OSZPCPM":
                    return "Okresní soud Plzeň-Město";

                case "OSZPCPS":
                    return "Okresní soud Plzeň-sever";

                case "OSJICPT":
                    return "Okresní soud Prachatice";

                case "OSSTCPY":
                    return "Okresní soud Praha-východ";

                case "OSSTCPZ":
                    return "Okresní soud Praha-západ";

                case "OSJIMPV":
                    return "Okresní soud v Prostějově";

                case "OSSTCRA":
                    return "Okresní soud v Rakovníku";

                case "OSZPCRO":
                    return "Okresní soud v Rokycanech";

                case "OSVYCRK":
                    return "Okresní soud v Rychnově nad Kněžnou";

                case "OSVYCSM":
                    return "Okresní soud v Semilech";

                case "OSZPCSO":
                    return "Okresní soud v Sokolově";

                case "OSJICST":
                    return "Okresní soud ve Strakonicích";

                case "OSSEMSU":
                    return "Okresní soud v Šumperku";

                case "OSJICTA":
                    return "Okresní soud v Táboře";

                case "OSZPCTC":
                    return "Okresní soud v Tachově";

                case "OSSCETP":
                    return "Okresní soud v Teplicích";

                case "OSJIMTR":
                    return "Okresní soud v Třebíči";

                case "OSVYCTU":
                    return "Okresní soud v Trutnově";

                case "OSJIMUH":
                    return "Okresní soud v Uherském Hradišti";

                case "OSSCEUL":
                    return "Okresní soud v Ústí nad Labem";

                case "OSVYCUO":
                    return "Okresní soud v Ústí nad Orlicí";

                case "OSVYCSY":
                    return "Okresní soud ve Svitavách";

                case "OSSEMVS":
                    return "Okresní soud ve Vsetíně";

                case "OSSEMVM":
                    return "Okresní soud ve Vsetíně pobočka Valašské Meziřičí";

                case "OSJIMVY":
                    return "Okresní soud ve Vyškově";

                case "OSJIMZR":
                    return "Okresní soud ve Žďáru nad Sázavou";

                case "OSJIMZL":
                    return "Okresní soud ve Zlíně";

                case "OSJIMZN":
                    return "Okresní soud ve Znojmě";

                default:

                    return "@Model.Soud";
            }

        }

        public void Save()
        {
            if (this.IsFullRecord == false)
                throw new ApplicationException("Cannot save partial Insolvence document");

            this.PrepareForSave();
            var res = ES.Manager.GetESClient_Insolvence().Index<Rizeni>(this, o => o.Id(this.SpisovaZnacka.ToString())); //druhy parametr musi byt pole, ktere je unikatni
            if (!res.IsValid)
            {
                throw new ApplicationException(res.ServerError?.ToString());
            }
        }

        public string StavRizeni()
        {

            switch (this.Stav)
            {
                case Insolvence.StavRizeni.Nevyrizena:
                    return "Nevyřízená";

                case Insolvence.StavRizeni.Moratorium:
                    return "Moratorium";

                case Insolvence.StavRizeni.Upadek:
                    return "Úpadek";

                case Insolvence.StavRizeni.Konkurs:
                    return "Konkurs";

                case Insolvence.StavRizeni.Oddluzeni:
                    return "Oddlužení";

                case Insolvence.StavRizeni.Reorganizace:
                    return "Reorganizace";

                case Insolvence.StavRizeni.Vyrizena:
                    return "Vyřízená";

                case Insolvence.StavRizeni.Pravomocna:
                    return "Pravomocná";

                case Insolvence.StavRizeni.Odskrtnuta:
                    return "Odškrtnutá";

                case Insolvence.StavRizeni.Zruseno:
                    return "Zrušeno vrchním soudem";

                case Insolvence.StavRizeni.KonkursPoZruseni:
                    return "Konkurs po zrušení";

                case Insolvence.StavRizeni.Obzivla:
                    return "Obživlá";

                case Insolvence.StavRizeni.MylnyZapis:
                    return "Mylný zápis";

                case Insolvence.StavRizeni.PostoupenaVec:
                    return "Postoupená věc";

                default:
                    return this.Stav;

            }

        }
        public string StavRizeniDetail()
        {
            switch (this.Stav)
            {
                case Insolvence.StavRizeni.Nevyrizena:
                    return "Před rozhodnutím o úpadku";

                case Insolvence.StavRizeni.Moratorium:
                    return "Povoleno moratorium";

                case Insolvence.StavRizeni.Upadek:
                    return "V úpadku";

                case Insolvence.StavRizeni.Konkurs:
                    return "Prohlášený konkurs";

                case Insolvence.StavRizeni.Oddluzeni:
                    return "Povoleno oddlužení";

                case Insolvence.StavRizeni.Reorganizace:
                    return "Povolena reorganizace";

                case Insolvence.StavRizeni.Vyrizena:
                    return "Vyřízená věc";

                case Insolvence.StavRizeni.Pravomocna:
                    return "Pravomocně skončená věc";

                case Insolvence.StavRizeni.Odskrtnuta:
                    return "Odškrtnutá - skončená věc";

                case Insolvence.StavRizeni.Zruseno:
                    return "Zrušeno vrchním soudem";

                case Insolvence.StavRizeni.KonkursPoZruseni:
                    return "Prohlášený konkurs po zrušení VS";

                case Insolvence.StavRizeni.Obzivla:
                    return "Obživlá věc";

                case Insolvence.StavRizeni.MylnyZapis:
                    return "Mylný zápis do rejstříku";

                case Insolvence.StavRizeni.PostoupenaVec:
                    return "Postoupená věc";

                default:
                    return this.Stav;


            }
        }


        public string BookmarkName()
        {
            return "Insolvence " + this.SpisovaZnacka;
        }

        public string ToAuditJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public string ToAuditObjectTypeName()
        {
            return "Insolvence";
        }

        public string ToAuditObjectId()
        {
            return this.SpisovaZnacka;
        }

        public string SocialInfoTitle()
        {
            return BookmarkName();
        }

        public string SocialInfoSubTitle()
        {
            return "Soud: " + this.SoudFullName();
        }

        public string SocialInfoBody()
        {
            return "<ul>" +
HlidacStatu.Util.InfoFact.RenderInfoFacts(this.InfoFacts(), 4, true, true, "", "<li>{0}</li>", true)
+ "</ul>";

        }

        public string SocialInfoFooter()
        {
            return "Údaje k " + Util.RenderData.ToDate(this.PosledniZmena);
        }

        public string SocialInfoImageUrl()
        {
            return string.Empty;
        }

        public InfoFact[] InfoFacts()
        {
            List<InfoFact> data = new List<InfoFact>();
             string sumTxt = $"Zahájena {Util.RenderData.ToDate(this.DatumZalozeni)}. {this.StavRizeniDetail()}. Řeší ji {this.SoudFullName()}.";

            data.Add(new InfoFact()
            {
                Level = InfoFact.ImportanceLevel.Summary,
                Text = sumTxt
            });

            sumTxt = Devmasters.Core.Lang.Plural.GetWithZero(this.Dluznici.Count,
    "",
    "Dlužníkem je " + this.Dluznici.First().FullNameWithYear(),
    "Dlužníky jsou " + this.Dluznici.Select(m => m.FullNameWithYear()).Aggregate((f, s) => f + ", " + s),
    "Dlužníky jsou" + this.Dluznici.Take(3).Select(m => m.FullNameWithYear()).Aggregate((f, s) => f + ", " + s)
        + "a " + Devmasters.Core.Lang.Plural.Get(this.Dluznici.Count - 3, " jeden další", "{0} další", "{0} dalších")
        + ". "
    );
            data.Add(new InfoFact()
            {
                Level = InfoFact.ImportanceLevel.High,
                Text = sumTxt
            });

            sumTxt = Devmasters.Core.Lang.Plural.GetWithZero(this.Veritele.Count,
                "",
                "Evidujeme jednoho věřitele.",
                "Evidujeme {0} věřitele.",
                "Evidujeme {0} věřitelů."
                );
            data.Add(new InfoFact()
            {
                Level = InfoFact.ImportanceLevel.High,
                Text = sumTxt
            });


            return data.OrderByDescending(o => o.Level).ToArray();
        }

        public bool ExistInDb()
        {
            using (HlidacStatu.Lib.Db.Insolvence.InsolvenceEntities idb = new Db.Insolvence.InsolvenceEntities())
            {
                return idb.Rizeni.Any(m => m.SpisovaZnacka == this.SpisovaZnacka);
            }
        }
        public void SaveToDb(bool rewrite)
        {
            using (HlidacStatu.Lib.Db.Insolvence.InsolvenceEntities idb = new Db.Insolvence.InsolvenceEntities())
            {
                var exists = idb.Rizeni.Where(m => m.SpisovaZnacka == this.SpisovaZnacka).FirstOrDefault();
                bool addNew = exists == null;

                if (exists != null && rewrite == true)
                {
                    foreach (var d in idb.Dokumenty.Where(m => m.RizeniId == exists.SpisovaZnacka))
                        idb.Dokumenty.Remove(d);

                    foreach (var d in idb.Dluznici.Where(m => m.RizeniId == exists.SpisovaZnacka))
                        idb.Dluznici.Remove(d);
                    foreach (var d in idb.Veritele.Where(m => m.RizeniId == exists.SpisovaZnacka))
                        idb.Veritele.Remove(d);
                    foreach (var d in idb.Spravci.Where(m => m.RizeniId == exists.SpisovaZnacka))
                        idb.Spravci.Remove(d);

                    idb.Rizeni.Remove(exists);

                    idb.SaveChanges();
                    addNew = true;
                }

                if (addNew)
                {
                    var r = new HlidacStatu.Lib.Db.Insolvence.Rizeni();
                    r.DatumZalozeni = this.DatumZalozeni ?? new DateTime(1990, 1, 1);
                    r.SpisovaZnacka = this.SpisovaZnacka;
                    r.OnRadar = this.OnRadar;
                    r.PosledniZmena = this.PosledniZmena;
                    r.Soud = this.Soud ?? "";
                    r.Stav = this.Stav ?? "";

                    idb.Rizeni.Add(r);

                    foreach (var td in this.Dluznici)
                    {
                        var d = ToIOsoba<Dluznici>(td);
                        idb.Dluznici.Add(d);
                    }
                    foreach (var td in this.Veritele)
                    {
                        var d = ToIOsoba<Veritele>(td);
                        idb.Veritele.Add(d);
                    }
                    foreach (var td in this.Spravci)
                    {
                        var d = ToIOsoba<Spravci>(td);
                        idb.Spravci.Add(d);
                    }
                    foreach (var td in this.Dokumenty)
                    {
                        var d = ToDbDokument(td);
                        idb.Dokumenty.Add(d);
                    }
                }
                else // update existing
                {
                    var sameR = exists.DatumZalozeni == (this.DatumZalozeni ?? new DateTime(1990, 1, 1))
                                && exists.SpisovaZnacka == this.SpisovaZnacka
                                && exists.OnRadar == this.OnRadar
                                && exists.PosledniZmena == this.PosledniZmena
                                && exists.Soud == (this.Soud ?? "")
                                && exists.Stav == (this.Stav ?? "");

                    if (sameR == false)
                    {
                        exists.DatumZalozeni = (this.DatumZalozeni ?? new DateTime(1990, 1, 1));
                        exists.SpisovaZnacka = this.SpisovaZnacka;
                        exists.OnRadar = this.OnRadar;
                        exists.PosledniZmena = this.PosledniZmena;
                        exists.Soud = (this.Soud ?? "");
                        exists.Stav = (this.Stav ?? "");
                    }

                    #region Dluznici
                    var dbDluznici = idb.Dluznici.Where(m => m.RizeniId == exists.SpisovaZnacka).ToArray();
                    //update existing
                    foreach (var d in this.Dluznici)
                    {
                        for (int i = 0; i < dbDluznici.Count(); i++)
                        {
                            var dd = dbDluznici[i];
                            if (d.IdOsoby == dd.IdOsoby && d.IdPuvodce == dd.IdPuvodce)
                            {
                                //same
                                if (!Validators.AreObjectsEqual(ToIOsoba<Dluznici>(d), dd, false, "pk"))
                                    dd = (Dluznici)UpdateIOsoba(d, dd);
                            }
                        }
                    }

                    if (this.Dluznici.Count() > dbDluznici.Count())
                    {
                        foreach (var d in this.Dluznici)
                            if (!dbDluznici.Any(m => m.IdOsoby == d.IdOsoby && m.IdPuvodce == d.IdPuvodce))
                                idb.Dluznici.Add(ToIOsoba<Dluznici>(d));
                    }

                    if (this.Dluznici.Count() < dbDluznici.Count())
                    {
                        //remove all and add orig
                        foreach (var d in dbDluznici)
                            idb.Dluznici.Remove(d);
                        foreach (var d in this.Dluznici)
                            idb.Dluznici.Add(ToIOsoba<Dluznici>(d));
                    }
                    #endregion

                    #region Veritele
                    var dbVeritele = idb.Veritele.Where(m => m.RizeniId == exists.SpisovaZnacka).ToArray();
                    //update existing
                    foreach (var d in this.Veritele)
                    {
                        for (int i = 0; i < dbVeritele.Count(); i++)
                        {
                            var dd = dbVeritele[i];
                            if (d.IdOsoby == dd.IdOsoby && d.IdPuvodce == dd.IdPuvodce)
                            {
                                //same
                                if (!Validators.AreObjectsEqual(ToIOsoba<Veritele>(d), dd, false, "pk"))
                                    dd = (Veritele)UpdateIOsoba(d, dd);
                            }
                        }
                    }

                    if (this.Veritele.Count() > dbVeritele.Count())
                    {
                        foreach (var d in this.Veritele)
                            if (!dbVeritele.Any(m => m.IdOsoby == d.IdOsoby && m.IdPuvodce == d.IdPuvodce))
                                idb.Veritele.Add(ToIOsoba<Veritele>(d));
                    }

                    if (this.Veritele.Count() < dbVeritele.Count())
                    {
                        //remove all and add orig
                        foreach (var d in dbVeritele)
                                idb.Veritele.Remove(d);
                        foreach (var d in this.Veritele)
                            idb.Veritele.Add(ToIOsoba<Veritele>(d));
                    }
                    #endregion

                    #region Spravci
                    var dbSpravci = idb.Spravci.Where(m => m.RizeniId == exists.SpisovaZnacka).ToArray();
                    //update existing
                    foreach (var d in this.Spravci)
                    {
                        for (int i = 0; i < dbSpravci.Count(); i++)
                        {
                            var dd = dbSpravci[i];
                            if (d.IdOsoby == dd.IdOsoby && d.IdPuvodce == dd.IdPuvodce)
                            {
                                //same
                                if (!Validators.AreObjectsEqual(ToIOsoba<Spravci>(d), dd, false, "pk"))
                                    dd = (Spravci)UpdateIOsoba(d, dd);
                            }
                        }
                    }

                    if (this.Spravci.Count() > dbSpravci.Count())
                    {
                        foreach (var d in this.Spravci)
                            if (!dbSpravci.Any(m => m.IdOsoby == d.IdOsoby && m.IdPuvodce == d.IdPuvodce))
                                idb.Spravci.Add(ToIOsoba<Spravci>(d));
                    }

                    if (this.Spravci.Count() < dbSpravci.Count())
                    {
                        //remove all and add orig
                        foreach (var d in dbSpravci)
                            idb.Spravci.Remove(d);
                        foreach (var d in this.Spravci)
                            idb.Spravci.Add(ToIOsoba<Spravci>(d));
                    }
                    #endregion

                    #region Dokumenty
                    var dbDokumenty = idb.Dokumenty.Where(m => m.RizeniId == exists.SpisovaZnacka).ToArray();
                    //update existing
                    foreach (var d in this.Dokumenty)
                    {
                        for (int i = 0; i < dbDokumenty.Count(); i++)
                        {
                            var dd = dbDokumenty[i];
                            if (d.Id == dd.Id)
                            {
                                //same
                                if (!Validators.AreObjectsEqual(ToDbDokument(d), dd, false))
                                {
                                    dd.Id = d.Id;
                                    dd.Length = (int)d.Lenght;
                                    dd.Oddil = d.Oddil;
                                    dd.Popis = d.Popis;
                                    dd.TypUdalosti = d.TypUdalosti;
                                    dd.Url = d.Url;
                                    dd.WordCount = (int)d.WordCount;
                                }
                            }
                        }
                    }

                    if (this.Dokumenty.Count() > dbDokumenty.Count())
                    {
                        foreach (var d in this.Dokumenty)
                            if (!dbDokumenty.Any(m => m.Id == d.Id))
                                idb.Dokumenty.Add(ToDbDokument(d));
                    }

                    if (this.Dokumenty.Count() < dbDokumenty.Count())
                    {
                        foreach (var d in dbDokumenty)
                            if (!this.Dokumenty.Any(m => m.Id == d.Id))
                                idb.Dokumenty.Remove(d);
                    }
                    #endregion

                }

                try
                {
                    if (System.Diagnostics.Debugger.IsAttached)
                        idb.Database.Log = Console.WriteLine;
                    if (idb.ChangeTracker.HasChanges())
                    {
                        HlidacStatu.Util.Consts.Logger.Info($"Updating Rizeni into DB {this.SpisovaZnacka}, {idb.ChangeTracker.Entries().Count(m=>m.State != System.Data.Entity.EntityState.Unchanged)} changes.");
                    }
                    idb.Database.CommandTimeout = 120;
                    idb.SaveChanges();

                }

                catch (DbEntityValidationException e)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    foreach (var eve in e.EntityValidationErrors)
                    {
                        sb.AppendFormat(@"Entity of type ""{0}"" in state ""{1}"" 
                   has the following validation errors:\n",
                            eve.Entry.Entity.GetType().Name,
                            eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            sb.AppendFormat(@"- Property: ""{0}"", Error: ""{1}""\n",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    Debug.WriteLine(sb.ToString());

                    throw new DbEntityValidationException(sb.ToString(), e);
                }
                catch (DbUpdateException e)
                {
                    //Add your code to inspect the inner exception and/or
                    //e.Entries here.
                    //Or just use the debugger.
                    //Added this catch (after the comments below) to make it more obvious 
                    //how this code might help this specific problem
                    throw;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    throw;
                }
            }
        }

        public IOsoba UpdateIOsoba(Osoba d, IOsoba dd)
        {
            dd.DatumNarozeni = d.DatumNarozeni < MinSqlDate ? MinSqlDate : d.DatumNarozeni;
            dd.ICO = d.ICO;
            dd.Mesto = d.Mesto;
            dd.Okres = d.Okres;
            dd.PlneJmeno = d.PlneJmeno;
            dd.PSC = d.Psc;
            dd.RC = d.Rc;
            dd.Role = d.Role;
            dd.Typ = d.Typ;
            dd.Zeme = d.Zeme;
            return dd;
        }

        private T ToIOsoba<T>(Osoba td)
            where T : IOsoba, new()
        {
            IOsoba d = new T();
            d.DatumNarozeni = td.DatumNarozeni < MinSqlDate ? MinSqlDate : td.DatumNarozeni;
            d.ICO = td.ICO;
            d.IdOsoby = td.IdOsoby;
            d.IdPuvodce = td.IdPuvodce;
            d.Mesto = Devmasters.Core.TextUtil.ShortenText(td.Mesto, 150);
            d.Okres = td.Okres;
            d.PlneJmeno = Devmasters.Core.TextUtil.ShortenText(td.PlneJmeno, 250);
            d.PSC = td.Psc;
            d.RC = td.Rc;
            d.RizeniId = this.SpisovaZnacka;
            d.Role = td.Role;
            d.Typ = td.Typ;
            d.Zeme = td.Zeme;

            return (T)d;
        }


        private Dokumenty ToDbDokument(Dokument td)
        {
            Db.Insolvence.Dokumenty d = new Db.Insolvence.Dokumenty();
            d.DatumVlozeni = td.DatumVlozeni < MinSqlDate
                                ? MinSqlDate : td.DatumVlozeni;
            d.Id = td.Id;
            d.Length = (int)td.Lenght;
            d.Oddil = td.Oddil;
            d.Popis = td.Popis;
            d.RizeniId = this.SpisovaZnacka;
            d.TypUdalosti = td.TypUdalosti;
            d.Url = td.Url;
            d.WordCount = (int)td.WordCount;
            return d;
        }

    }
}
