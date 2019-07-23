using System;
using System.Linq;
using System.Collections.Generic;
using HlidacStatu.Util;

namespace HlidacStatu.Lib.Data.Insolvence
{
    public class Rizeni
        : Bookmark.IBookmarkable, Util.ISocialInfo
    {
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
            string sumTxt = $"Zahájena {Util.RenderData.ToDate(this.DatumZalozeni)}.  Řeší ji " + this.SoudFullName();
            sumTxt += Devmasters.Core.Lang.Plural.GetWithZero(this.Dluznici.Count,
                "",
                "Dlužníkem je " + this.Dluznici.First().FullNameWithYear(),
                "Dlužníky jsou " + this.Dluznici.Select(m => m.FullNameWithYear()).Aggregate((f, s) => f + ", " + s),
                "Dlužníky jsou" + this.Dluznici.Take(3).Select(m => m.FullNameWithYear()).Aggregate((f, s) => f + ", " + s)
                    + "a " + Devmasters.Core.Lang.Plural.Get(this.Dluznici.Count - 3, " jeden další", "{0} další", "{0} dalších")
                    + ". "
                );
            sumTxt += Devmasters.Core.Lang.Plural.GetWithZero(this.Veritele.Count,
                "",
                "Evidujeme jednoho věřitele.",
                "Evidujeme {0} věřitele.",
                "Evidujeme {0} věřitelů."
                );

            data.Add(new InfoFact()
            {
                Level = InfoFact.ImportanceLevel.Summary,
                Text = sumTxt
            });


            return data.OrderByDescending(o => o.Level).ToArray();
        }

        public void SaveToDb(bool rewrite)
        {
            using (HlidacStatu.Lib.Db.Insolvence.InsolvenceEntities idb = new Db.Insolvence.InsolvenceEntities())
            {
                var exists = idb.Rizenis.Where(m => m.SpisovaZnacka == this.SpisovaZnacka).FirstOrDefault();
                if (exists != null && rewrite == false)
                    throw new System.ApplicationException($"Object Rizeni {this.SpisovaZnacka} already exists in db");
                else if (exists != null && rewrite == true)
                {
                    idb.Rizenis.Remove(exists);
                    foreach (var d in idb.Dokumenties.Where(m => m.RizeniId == exists.SpisovaZnacka))
                        idb.Dokumenties.Remove(d);                    
                    foreach (var d in idb.Dokumenties.Where(m => m.RizeniId == exists.SpisovaZnacka))
                        idb.Dokumenties.Remove(d);
                    foreach (var d in idb.Dluznicis.Where(m => m.RizeniId == exists.SpisovaZnacka))
                        idb.Dluznicis.Remove(d);

                    foreach (var d in idb.Veriteles.Where(m => m.RizeniId == exists.SpisovaZnacka))
                        idb.Veriteles.Remove(d);
                    foreach (var d in idb.Spravcis.Where(m => m.RizeniId == exists.SpisovaZnacka))
                        idb.Spravcis.Remove(d);
                    idb.SaveChanges();

                }

                var r = new HlidacStatu.Lib.Db.Insolvence.Rizeni();
                r.DatumZalozeni = this.DatumZalozeni ?? new DateTime(1990, 1, 1);
                r.SpisovaZnacka = this.SpisovaZnacka;
                r.OnRadar = this.OnRadar;
                r.PosledniZmena = this.PosledniZmena;
                r.Soud = this.Soud;
                r.Stav = this.Stav;

                idb.Rizenis.Add(r);

                foreach (var td in this.Dluznici)
                {
                    Db.Insolvence.Dluznici d = new Db.Insolvence.Dluznici();
                    d.DatumNarozeni = td.DatumNarozeni;
                    d.ICO = td.ICO;
                    d.IdOsoby = td.IdOsoby;
                    d.IdPuvodce = td.IdPuvodce;
                    d.Mesto = td.Mesto;
                    d.Okres = td.Okres;
                    d.PlneJmeno = td.PlneJmeno;
                    d.PSC = td.Psc;
                    d.RC = td.Rc;
                    d.RizeniId = this.SpisovaZnacka;
                    d.Role = td.Role;
                    d.Typ = td.Typ;
                    d.Zeme = td.Zeme;
                    idb.Dluznicis.Add(d);
                }
                foreach (var td in this.Veritele)
                {
                    Db.Insolvence.Veritele d = new Db.Insolvence.Veritele();
                    d.DatumNarozeni = td.DatumNarozeni;
                    d.ICO = td.ICO;
                    d.IdOsoby = td.IdOsoby;
                    d.IdPuvodce = td.IdPuvodce;
                    d.Mesto = td.Mesto;
                    d.Okres = td.Okres;
                    d.PlneJmeno = td.PlneJmeno;
                    d.PSC = td.Psc;
                    d.RC = td.Rc;
                    d.RizeniId = this.SpisovaZnacka;
                    d.Role = td.Role;
                    d.Typ = td.Typ;
                    d.Zeme = td.Zeme;
                    idb.Veriteles.Add(d);
                }
                foreach (var td in this.Spravci)
                {
                    Db.Insolvence.Spravci d = new Db.Insolvence.Spravci();
                    d.DatumNarozeni = td.DatumNarozeni;
                    d.ICO = td.ICO;
                    d.IdOsoby = td.IdOsoby;
                    d.IdPuvodce = td.IdPuvodce;
                    d.Mesto = td.Mesto;
                    d.Okres = td.Okres;
                    d.PlneJmeno = td.PlneJmeno;
                    d.PSC = td.Psc;
                    d.RC = td.Rc;
                    d.RizeniId = this.SpisovaZnacka;
                    d.Role = td.Role;
                    d.Typ = td.Typ;
                    d.Zeme = td.Zeme;
                    idb.Spravcis.Add(d);
                }
                foreach (var td in this.Dokumenty)
                {
                    Db.Insolvence.Dokumenty d = new Db.Insolvence.Dokumenty();
                    d.DatumVlozeni = td.DatumVlozeni;
                    d.Id = td.Id;
                    d.Length = (int)td.Lenght;
                    d.Oddil = td.Oddil;
                    d.Popis = td.Popis;
                    d.RizeniId = this.SpisovaZnacka;
                    d.TypUdalosti = td.TypUdalosti;
                    d.Url = td.Url;
                    d.WordCount = (int)td.WordCount;
                    idb.Dokumenties.Add(d);
                }

                idb.SaveChanges();
            }

        }
    }
}
