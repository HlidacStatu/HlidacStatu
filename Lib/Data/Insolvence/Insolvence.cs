using Devmasters.Core;
using Devmasters.Enums;

using HlidacStatu.Lib.ES;
using HlidacStatu.Lib.Searching;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Data.Insolvence
{
    public partial class Insolvence
    {
        /*
         * https://www.creditcheck.cz/SlovnicekPojmuDetail.aspx?id=23
         Stavy v insolvenčním řízení
Insolvenční řízení (věc) prochází několika stavy:
nevyřízená = c
moratorium = povoleno moratorium
úpadek = v úpadku
konkurs = prohlášený konkurs
oddlužení = povoleno oddlužení
reorganizace = povolena reorganizace
vyřízená = vyřízená věc
pravomocná = pravomocně skončená věc
odškrtnutá = odškrtnutá - skončená věc
zrušeno vrchním soudem = zrušeno vrchním soudem
konkurs po zrušení = prohlášený konkurs po zrušení vrchním soudem
obživlá = obživlá věc
mylný zápis = mylný zápis do rejstříku
postoupená věc = postoupená věc

ODSKRTNUTA = 
ODDLUŽENÍ = 
PRAVOMOCNA = 
KONKURS = 
VYRIZENA = 
NEVYRIZENA = 
MYLNÝ ZÁP. = 
OBZIVLA = 
ÚPADEK = 
REORGANIZ = 
ZRUŠENO VS = 
NEVYR-POST = 
K-PO ZRUŠ. = 
MORATORIUM = 

*/
        public class StavRizeni
        {
            public const string Nevyrizena = "NEVYRIZENA";
            public const string Moratorium = "MORATORIUM";
            public const string Upadek = "ÚPADEK";
            public const string Konkurs = "KONKURS";
            public const string Oddluzeni = "ODDLUŽENÍ";
            public const string Reorganizace = "REORGANIZ";
            public const string Vyrizena = "VYRIZENA";
            public const string Pravomocna = "PRAVOMOCNA";
            public const string Odskrtnuta = "ODSKRTNUTA";
            public const string Zruseno = "ZRUŠENO VS";
            public const string KonkursPoZruseni = "K-PO ZRUŠ.";
            public const string Obzivla = "OBZIVLA";
            public const string MylnyZapis = "MYLNÝ ZÁP.";
            public const string PostoupenaVec = "NEVYR-POST";
        }


        [ShowNiceDisplayName()]
        public enum StavInsolvence : int
        {

            [NiceDisplayName("Neznámý")]
            Neznamy = 0,

            [NiceDisplayName("odškrtnutá - skončená věc")]
            Odskrtnuta = 1,

            [NiceDisplayName("povoleno oddlužení")]
            Oddluzeni = 2,

            [NiceDisplayName("pravomocně skončená věc")]
            Pravomocna = 3,

            [NiceDisplayName("prohlášený konkurs")]
            Konkurs = 4,

            [NiceDisplayName("vyřízená věc")]
            Vyrizena = 5,

            [NiceDisplayName("vyřízená věc")]
            Nevyrizena = 6,

            [NiceDisplayName("mylný zápis do rejstříku")]
            MylnyZapis = 7,

            [NiceDisplayName("obživlá věc")]
            Obzivla = 8,

            [NiceDisplayName("v úpadku")]
            Upadek = 9,

            [NiceDisplayName("povolena reorganizace")]
            Reorganizace = 10,

            [NiceDisplayName("zrušeno vrchním soudem")]
            ZrusenoVS = 11,

            [NiceDisplayName("postoupená věc")]
            Postoupena = 12,

            [NiceDisplayName("prohlášený konkurs po zrušení vrchním soudem")]
            KonkursPoZruseni = 13,

            [NiceDisplayName("povoleno moratorium")]
            Moratorium = 14,
        }

        public static StavInsolvence StavTextToStav(string stav)
        {
            stav = stav.Trim().ToUpper();
            switch (stav)
            {
                case StavRizeni.Odskrtnuta:
                    return StavInsolvence.Odskrtnuta;
                case StavRizeni.Oddluzeni:
                    return StavInsolvence.Oddluzeni;
                case StavRizeni.Pravomocna:
                    return StavInsolvence.Pravomocna;
                case StavRizeni.Konkurs:
                    return StavInsolvence.Konkurs;
                case StavRizeni.Vyrizena:
                    return StavInsolvence.Vyrizena;
                case StavRizeni.Nevyrizena:
                    return StavInsolvence.Nevyrizena;
                case StavRizeni.MylnyZapis:
                    return StavInsolvence.MylnyZapis;
                case StavRizeni.Obzivla:
                    return StavInsolvence.Obzivla;
                case StavRizeni.Upadek:
                    return StavInsolvence.Upadek;
                case StavRizeni.Reorganizace:
                    return StavInsolvence.Reorganizace;
                case StavRizeni.Zruseno:
                    return StavInsolvence.ZrusenoVS;
                case StavRizeni.PostoupenaVec:
                    return StavInsolvence.Postoupena;
                case StavRizeni.KonkursPoZruseni:
                    return StavInsolvence.KonkursPoZruseni;
                case StavRizeni.Moratorium:
                    return StavInsolvence.Moratorium;
                default:
                    return StavInsolvence.Neznamy;
            }
        }

        public static string StavInsolvenceDescription(StavInsolvence stav)
        {
            //https://www.cesr.cz/slovnicek-pojmu/
            //http://www.duverujaleproveruj.cz/13/75-4-insolvencni-zakon-a-insolvencni-rejstrik
            switch (stav)
            {
                case StavInsolvence.Neznamy:
                    return "Stav insolvence je nejasný.";
                case StavInsolvence.Odskrtnuta:
                    return "Ukončení evidence insolvence v administrativní agendě soudu.";
                case StavInsolvence.Oddluzeni:
                    return @"Způsob řešení úpadku dlužníka, 
                            ve kterém jsou dluhy v dohodnuté výši (obvykle pouze část dluhů) placeny dohodnutým způsobem (dohoda mezi dlužníkem a věřiteli). 
                            Maximální délka oddlužení je 5 let a týká se fyzické osoby či právnické osoby, která podle zákona není považována za podnikatele.";
                case StavInsolvence.Pravomocna:
                    return "Věc je pravomocná tehdy, nelze-li již proti ní podat opravný prostředek. Je tedy závazná.";
                case StavInsolvence.Konkurs:
                    return "Řešení údaku, kdy jsou zjištěné pohledávky věřitelů poměrně uspokojeny z výnosu prodeje majetku. " +
                        "Výše poměrného uspokojení věřitelů závisí na velikosti, zajištění a pořadí pohledávky." +
                        "Neuspokojené pohledávky nebo jejich části nezanikají. ";
                case StavInsolvence.Vyrizena:
                    return "Již bylo vydáno rozhodnutí, kterým bude dané řízení ukončeno, " +
                        "toto rozhodnutí ale ještě nenabylo právní moci.";
                case StavInsolvence.Nevyrizena:
                    return "Insolvence, v níž ještě nebylo vydáno tzv. vyřizující rozhodnutí, " +
                        "tedy rozhodnutí, kterým by byla věc ukončena (typicky rozsudek).";
                case StavInsolvence.MylnyZapis:
                    return "Chybný zápis do insolvenčního rejstříku; 'mylný zápis' nevyvolává právní následky.";
                case StavInsolvence.Obzivla:
                    return "Pouze vyřízená insolvence může změnit svůj stav na obživlou. " +
                        "Typicky se tak děje např. v případě, kdy je po podání opravného prostředku (odvolací řízení) zrušeno původní rozhodnutí (označené jako vyřízené) " +
                        "- původní řízení tím takzvaně 'obživne', protože se jím soud musí znovu zabývat.";
                case StavInsolvence.Upadek:
                    return "Způsob řešení majetkové situace dlužníka, který je buď v platební neschopnosti, nebo je předlužen. " +
                        "V zásadě je možné řešit úpadek konkursem, oddlužením či reorganizací.";
                case StavInsolvence.Reorganizace:
                    return "Řešení úpadku, ve kterém je dlužníkem (podnikatelem) plněn reorganizační plán, " +
                        "jímž jsou postupně uspokojovány pohledávky věřitelů.";
                case StavInsolvence.ZrusenoVS:
                    return " Rozhodnutí Krajského soudu zrušené Vrchním soudem se vrací zpět k tomuto soudu k dalšímu projednání a rozhodnutí.";
                case StavInsolvence.Postoupena:
                    return "Insolvence je postupována dál bez vyřízení. " +
                        "Stává se často v případech, kdy byla podána nepříslušnému soudu. " +
                        "Ten ji, aniž by v ní činil jakékoli úkony, zašle soudu příslušnému.";
                case StavInsolvence.KonkursPoZruseni:
                    return "Pravomocní zrušení konkursu,  je ukončeno insolveční řízení. Mezi důvody patří: " +
                        " nebyl osvědčen dlužníkův úpadek; " +
                        "došlo-li již ke zpeněžení podstatné části majetku; " +
                        "není žádný přihlášený věřitel a všechny pohledávky jsou uspokojeny; " +
                        "pro uspokojení věřitelů je majetek dlužníka zcela nepostačující; " +
                        "všichni věřitelé a insolvenční správce vyslovili se zrušením konkursu souhlas.";
                case StavInsolvence.Moratorium:
                    return "Insolvenční řízení, ve kterém je na odůvodněný návrh dlužníka či věřitele prohlášeno soudem moratorium; " +
                        "po dobu moratoria nelze na dlužníka prohlásit úpadek. Délka moratoria je maximálně 3 měsíce + 30 dní prodloužení.";
                default:
                    return "";
            }
        }

        public static InsolvenceDetail LoadFromES(string id, bool includeDocumentsPlainText, bool limitedView)
        {
            var client = Manager.GetESClient_Insolvence();
            var spisovaZnacka = ParseId(id);

            try
            {
                
                var rizeni = includeDocumentsPlainText
                    ? client.Get<Rizeni>(spisovaZnacka)
                    : client.Get<Rizeni>(spisovaZnacka, s => s
                            .SourceExcludes("dokumenty.plainText") 
                            //.SourceExclude("")
                            );

                if (rizeni.Found)
                {
                    var r = rizeni.Source;
                    if (limitedView && r.OnRadar == false)
                        return null;

                    r.IsFullRecord = includeDocumentsPlainText;
                    return new InsolvenceDetail
                    {
                        Rizeni = r,
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                // TODO: handle error
                throw;
            }
        }


        public static void SaveRizeni(Rizeni r)
        {
            r.Save();
        }

        public static DokumentSeSpisovouZnackou LoadDokument(string id, bool limitedView)
        {
            var client = Manager.GetESClient_Insolvence();

            try
            {
                var data = client.Search<Rizeni>(s => s
                        .Source(sr => sr.Includes(r => r.Fields("dokumenty.*").Fields("spisovaZnacka")))
                        .Query(q => q.Match(m => m.Field("dokumenty.id").Query(id)))); //TODO

                if (data.IsValid == false)
                    throw new ApplicationException(data.ServerError?.ToString() ?? "");

                if (data.Total == 0)
                    return null;

                return data.Hits.Select(h => new DokumentSeSpisovouZnackou
                    {
                        SpisovaZnacka = h.Source.SpisovaZnacka,
                        UrlId = h.Source.UrlId(),
                        Dokument = h.Source.Dokumenty.Single(d => d.Id == id),
                        Rizeni = h.Source
                    }).First();
            }
            catch (Exception e)
            {
                // TODO: handle error
                throw;
            }
        }

        public static InsolvenceSearchResult NewFirmyVInsolvenci(int count, bool limitedView)
        {
            return NewSubjektVInsolvenci(count, "P", limitedView);
        }

        public static InsolvenceSearchResult NewOsobyVInsolvenci(int count, bool limitedView)
        {
            return NewSubjektVInsolvenci(count, "F", limitedView);
        }

        private static InsolvenceSearchResult NewSubjektVInsolvenci(int count, string typ, bool limitedView)
        {
            var client = Manager.GetESClient_Insolvence();

            try
            {
                var rs = Insolvence.SimpleSearch("dluznici.typ:" + typ, 1, count,
                    (int)InsolvenceSearchResult.InsolvenceOrderResult.DateAddedDesc, false, limitedView, null);
                //client.Search<Rizeni>(s =>
                //s.Size(count)
                //.Sort(o => o.Field(f => f.Field(a => a.DatumZalozeni).Descending()))
                //.Query(q => q.Match(m => m.Field("dluznici.typ").Query(typ))));

                return rs;

            }
            catch (Exception e)
            {
                // TODO: handle error
                throw;
            }
        }

        private static string ParseId(string id)
        {
            return id.Replace("_", " ").Replace("-", "/");
        }

        private static QueryContainer SpisovaZnackaQuery(string spisovaZnacka)
        {
            return new QueryContainerDescriptor<Osoba>().QueryString(qs =>
                            qs.Query($"spisovaZnacka:\"{spisovaZnacka}\""));
        }

        public static IEnumerable<string> AllIdsFromDB()
        {
            //return AllIdsFromDB(null);
            using (Db.Insolvence.InsolvenceEntities db = new Db.Insolvence.InsolvenceEntities())
            {
                return db.Rizeni.AsNoTracking().Select(m => m.SpisovaZnacka)
                    .ToArray();//force query
            }
        }

        public static IEnumerable<string> AllIdsFromES(Action<string> outputWriter = null, Action<Devmasters.Core.Batch.ActionProgressData> progressWriter = null)
        {

            Func<int, int, Nest.ISearchResponse<Lib.Data.Insolvence.Rizeni>> searchFunc = (size, page) =>
            {
                return Lib.ES.Manager.GetESClient_Insolvence().Search<Lib.Data.Insolvence.Rizeni>(a => a
                           .Size(size)
                           .Source(false)
                           .From(page * size)
                           .Query(q => q.MatchAll())
                           .Scroll("1m")
                           .TrackTotalHits(page * size == 0 ? true : (bool?)null)
                            );
            };

            List<string> ids = new List<string>();
            Searching.Tools.DoActionForQuery<Lib.Data.Insolvence.Rizeni>(Lib.ES.Manager.GetESClient_Insolvence(),
                        searchFunc, (hit, param) =>
                        {
                            ids.Add(hit.Id);
                            return new Devmasters.Core.Batch.ActionOutputData() { CancelRunning = false, Log = null };
                        }, null, outputWriter, progressWriter, false, prefix: "get_id_Insolvence ", blockSize: 10);
            return ids;
        }

    }
}
