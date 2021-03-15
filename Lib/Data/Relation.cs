using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devmasters;
using HlidacStatu.Util;
using Devmasters.Enums;

namespace HlidacStatu.Lib.Data
{
    public class Relation
    {

        public static TimeSpan NedavnyVztahDelka = TimeSpan.FromDays((365 * 5) + 2); //5 let


        [ShowNiceDisplayName()]
        public enum RelationEnum
        {
            [NiceDisplayName("Osobní vztah")]
            OsobniVztah = -3,
            [NiceDisplayName("Vliv")]
            Vliv = -2,
            [NiceDisplayName("Kontrola")]
            Kontrola = -1,

            [NiceDisplayName("Podnikatel z OR")]
            Podnikatel_z_OR = 00,
            [NiceDisplayName("Člen statutárního orgánu")]
            Clen_statutarniho_organu = 01,
            [NiceDisplayName("Likvidátor")]
            Likvidator = 02,
            [NiceDisplayName("Prokurista")]
            Prokurista = 03,
            [NiceDisplayName("Člen dozorčí rady")]
            Clen_dozorci_rady = 04,
            [NiceDisplayName("Jediný akcionář")]
            Jediny_akcionar = 05,
            [NiceDisplayName("Člen družstva s vkladem")]
            Clen_druzstva_s_vkladem = 06,
            [NiceDisplayName("Člen dozorčí rady v zastoupení")]
            Clen_dozorci_rady_v_zastoupeni = 07,
            [NiceDisplayName("Člen kontrolní komise v zastoupení")]
            Clen_kontrolni_komise_v_zastoupeni = 08,
            [NiceDisplayName("Komplementář")]
            Komplementar = 09,
            [NiceDisplayName("Komanditista")]
            Komanditista = 10,
            [NiceDisplayName("Správce konkursu")]
            Spravce_konkursu = 11,
            [NiceDisplayName("Likvidátor v zastoupení")]
            Likvidator_v_zastoupeni = 12,
            [NiceDisplayName("Oddělený insolvenční správce")]
            Oddeleny_insolvencni_spravce = 13,
            [NiceDisplayName("Pobočný spolek")]
            Pobocny_spolek = 14,
            [NiceDisplayName("Podnikatel")]
            Podnikatel = 15,
            [NiceDisplayName("Předběžný insolvenční správce")]
            Predbezny_insolvencni_spravce = 16,
            [NiceDisplayName("Předběžný správce")]
            Predbezny_spravce = 17,
            [NiceDisplayName("Představenstvo")]
            Predstavenstvo = 18,
            [NiceDisplayName("Podílník")]
            Podilnik = 19,
            [NiceDisplayName("Revizor")]
            Revizor = 20,
            [NiceDisplayName("Revizor v zastoupení")]
            Revizor_v_zastoupeni = 21,
            [NiceDisplayName("Člen rozhodčí komise")]
            Clen_rozhodci_komise = 22,
            [NiceDisplayName("Vedoucí odštěpného závodu")]
            Vedouci_odstepneho_zavodu = 23,
            [NiceDisplayName("Společník")]
            Spolecnik = 24,
            [NiceDisplayName("Člen správní rady v zastoupení")]
            Clen_spravni_rady_v_zastoupeni = 25,
            [NiceDisplayName("Člen statutárního orgánu zřizovatele")]
            Clen_statutarniho_organu_zrizovatele = 26,
            [NiceDisplayName("Člen statutárního orgánu v zastoupení")]
            Clen_statutarniho_organu_v_zastoupeni = 28,
            [NiceDisplayName("Insolvenční správce vyrovnávací")]
            Insolvencni_spravce_vyrovnavaci = 29,
            [NiceDisplayName("Člen správní rady")]
            Clen_spravni_rady = 31,
            [NiceDisplayName("Statutární orgán zřizovatele v zastoupení")]
            Statutarni_organ_zrizovatele_v_zastoupeni = 32,
            [NiceDisplayName("Zakladatel")]
            Zakladatel = 33,
            [NiceDisplayName("Nástupce zřizovatele")]
            Nastupce_zrizovatele = 34,
            [NiceDisplayName("Zakladatel s vkladem")]
            Zakladatel_s_vkladem = 35,
            [NiceDisplayName("Člen sdružení")]
            Clen_sdruzeni = 36,
            [NiceDisplayName("Zástupce insolvenčního správce")]
            Zastupce_insolvencniho_spravce = 37,
            [NiceDisplayName("Člen kontrolní komise")]
            Clen_kontrolni_komise = 38,
            [NiceDisplayName("Insolvenční správce")]
            Insolvencni_spravce = 39,
            [NiceDisplayName("Zástupce správce")]
            Zastupce_spravce = 40,
            [NiceDisplayName("Zvláštní insolvenční správce")]
            Zvlastni_insolvencni_spravce = 41,
            [NiceDisplayName("Zvláštní správce")]
            Zvlastni_spravce = 42,
            [NiceDisplayName("Podnikatel z RŽP")]
            Podnikatel_z_RZP = 400,
            [NiceDisplayName("Statutár")]
            Statutar = 401,
            [NiceDisplayName("Vedoucí org. složky")]
            Vedouci_org_slozky = 402,

        }


        [ShowNiceDisplayName()]
        public enum RelationSimpleEnum
        {
            [NiceDisplayName("Osobní vztah")]
            OsobniVztah = -3,
            [NiceDisplayName("Vliv")]
            Vliv = -2,
            [NiceDisplayName("Kontrola")]
            Kontrola = -1,
            [NiceDisplayName("Neznámý")]
            Neznamy = 0,
            [NiceDisplayName("Společník")]
            Spolecnik = 1,
            [NiceDisplayName("Jednatel")]
            Jednatel = 2,
            [NiceDisplayName("Prokura")]
            Prokura = 3,
            [NiceDisplayName("Člen dozorčí rady")]
            Dozorci_rada = 4,
            [NiceDisplayName("Člen statut. orgáni")]
            Statutarni_organ = 5,
            [NiceDisplayName("OSVČ")]
            OSVC = 6,
            [NiceDisplayName("Zakladatel")]
            Zakladatel = 7,
            [NiceDisplayName("Jiný")]
            Jiny = 99,
            [NiceDisplayName("Souhrnný")]
            Souhrnny = 100,
        }

        [ShowNiceDisplayName()]
        public enum RelationTypeEnum
        {
            [NiceDisplayName("Neznámý vztah vztah")]
            Neznamy = 0,
            [NiceDisplayName("Přímý vztah")]
            Primy = 1,
            [NiceDisplayName("Nepřímý vztah")]
            Neprimy = 2,
            [NiceDisplayName("Neoficiální vztah")]
            Neoficialni = 3,
            [NiceDisplayName("Osobní vztah")]
            Osobni = 4,
        }


        [Sortable(SortableAttribute.SortAlgorithm.BySortValue)]
        [ShowNiceDisplayName()]
        public enum AktualnostType
        {

            [NiceDisplayName("Aktuálně podle obch.rejstříku")]
            Aktualni = 2,
            [NiceDisplayName("Aktuálně podle obch.rejstříku či posledních 4 letech")]
            Nedavny = 1,
            [NiceDisplayName("Aktuálně podle obch.rejstříku či kdykoliv v minulosti")]
            Neaktualni = 0,
            [NiceDisplayName("")]
            Libovolny = -1,

        }






        public enum TiskEnum
        {
            Text,
            Html,
            Json,
            Checkbox
        }

        public static string ExportTabData(IEnumerable<Graph.Edge> data)
        {
            if (data == null)
                return "";
            if (data.Count() == 0)
                return "";

            StringBuilder sb = new StringBuilder(1024);

            foreach (var i in data)
            {
                sb.AppendFormat($"{i.From?.Id}\t{i.From?.PrintName()}\t{i.To?.Id}\t{i.To?.PrintName()}\t{i.RelFrom?.ToShortDateString() ?? "Ø"}\t{i.RelTo?.ToShortDateString() ?? "Ø"}\t{i.Descr}\n");
            }
            return sb.ToString();
        }
        public static string ExportTabDataDebug(IEnumerable<Graph.Edge> data)
        {
            if (data == null)
                return "";
            if (data.Count() == 0)
                return "";

            StringBuilder sb = new StringBuilder(1024);

            foreach (var i in data)
            {
                sb.AppendFormat($"{i.From?.UniqId}\t{i.To?.UniqId}\t{i.Descr} {i.RelFrom?.ToShortDateString() ?? "Ø"} -> {i.RelTo?.ToShortDateString() ?? "Ø"}\t{i.Root}\n");
            }
            return sb.ToString();
        }
        public static string ExportGraphJsonData(IEnumerable<Graph.Edge> data)
        {
            if (data == null)
                return "{\"nodes\": [],\"edges\": []}";
            if (data.Count() == 0)
                return "{\"nodes\": [],\"edges\": []}";

            Dictionary<string, Graph.GraphJson> nodes = new Dictionary<string, Graph.GraphJson>();

            foreach (var i in data)
            {
                if (i.From != null && !nodes.ContainsKey(i.From.UniqId))
                {
                    nodes.Add(i.From.UniqId, new Graph.GraphJson(i.From, i.Distance - 1, i.Distance == 0));
                }
                if (i.To != null && !nodes.ContainsKey(i.To.UniqId))
                    nodes.Add(i.To.UniqId, new Graph.GraphJson(i.To, i.Distance));
            }

            var ret = new
            {
                nodes = nodes.Select(m => m.Value).ToArray(),
                edges = data
                    .Where(e => e.To != null && e.From != null)
                    .Select(e => new Graph.GraphJson(e)).ToArray()
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(ret);
        }

        public static string TiskVazeb(string rootName, IEnumerable<Graph.Edge> vazby, TiskEnum typ, bool withStats = true)
        {
            string htmlTemplate = "<ul id='nestedlist'><li>{0}</li>{1}</ul>";
            string textTemplate = "{0}\n|\n{1}";
            string jsonTemplate = "{{ \"text\": {{ \"name\": \"{0}\" }}, \"children\": [ {1} ] }}";
            string checkboxTemplate = "<ul>{0} {1}</ul>";
            switch (typ)
            {
                case TiskEnum.Text:
                    return string.Format(textTemplate, rootName, PrintFlatRelations((Graph.Edge)null, 0, vazby, typ, null, withStats));
                case TiskEnum.Html:
                    return string.Format(htmlTemplate, rootName, PrintFlatRelations((Graph.Edge)null, 0, vazby, typ, null, withStats));
                case TiskEnum.Json:
                    return string.Format(jsonTemplate, rootName, PrintFlatRelations((Graph.Edge)null, 0, vazby, typ, null, withStats));
                case TiskEnum.Checkbox:
                    return string.Format(checkboxTemplate, rootName, PrintFlatRelations((Graph.Edge)null, 0, vazby, typ, null, withStats));
                default:
                    return string.Empty;
            }

        }

        private static string PrintFlatRelations(Graph.Edge parent, int level, IEnumerable<Graph.Edge> relations, TiskEnum typ,
           List<string> renderedIds, bool withStats = true, string highlightSubjId = null)
        {
            if (parent == null)
                return PrintFlatRelations((Graph.MergedEdge)null, level, relations, typ,
                    renderedIds, withStats, highlightSubjId);
            else
                return PrintFlatRelations(new Graph.MergedEdge(parent), level, relations, typ,
                renderedIds, withStats, highlightSubjId);
        }
        private static string PrintFlatRelations(Graph.MergedEdge parent, int level, IEnumerable<Graph.Edge> relations, TiskEnum typ,
       List<string> renderedIds, bool withStats = true, string highlightSubjId = null)
        {
            int space = 2;
            string horizLine = "--";//new string('\u2500',2);
            string vertLine = "|";//new string('\u2502',1);
            string cross = "+"; //new string('\u251C', 1);

            if (renderedIds == null)
                renderedIds = new List<string>();

            var rels = relations
                .Where(m =>
                    (
                    (parent != null && m.From?.UniqId == parent.To?.UniqId)
                    ||
                    (parent == null && !relations.Any(r => r.To?.UniqId == m.From?.UniqId)) //do root urovne pridej vazby, ktere jsou sirotci bez parenta
                    )
                )
                .Distinct()
                .GroupBy(k => new { id = k.To.UniqId, type = k.To.Type }, (k, v) =>
                {
                    Graph.MergedEdge withChildren = Graph.Edge.MergeSameEdges(v);
                    if (withChildren == null)
                        withChildren = new Graph.MergedEdge(v.First());

                    return withChildren;
                })
                .OrderBy(m => m.To.PrintName())
                .ToArray();

            if (rels.Count() == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder(512);
            List<Graph.Edge> deepRels = new List<Graph.Edge>();
            switch (typ)
            {
                case TiskEnum.Text:
                    break;
                case TiskEnum.Html:
                case TiskEnum.Checkbox:
                    sb.AppendLine("<ul>");
                    break;
                case TiskEnum.Json:
                    break;
            }
            for (int i = 0; i < rels.Count(); i++)
            {
                var rel = rels[i];
                if (renderedIds.Contains(rel.To.UniqId))
                    continue;

                var last = i == (rels.Count() - 1);
                Analytics.StatisticsSubjectPerYear<Data.Smlouva.Statistics.Data> stat = null;
                if (withStats && rel.To.Type == Graph.Node.NodeType.Company)
                    stat = Firmy.Get(rel.To.Id).StatistikaRegistruSmluv(); //new Analysis.SubjectStatistic(rel.To.Id);

                string subjId = rel.To.Type == Graph.Node.NodeType.Company ? rel.To.Id : "Osoba";
                string subjName = rel.To.PrintName();
                renderedIds.Add(rel.To.UniqId);
                switch (typ)
                {
                    case TiskEnum.Text:
                        sb.AppendLine(string.Concat(Enumerable.Repeat(vertLine + new string(' ', space), level + 1)));
                        sb.Append(
                            string.Concat(
                                Enumerable.Repeat(
                                    vertLine + new string(' ', space)
                                    , (level))
                                    )
                                );
                        if (rel.To.Highlighted)
                            subjName = string.Format("!!{0}!!", subjName);

                        sb.AppendFormat("{0}{1}:{2} {3}\n",
                                cross + horizLine + " ",
                                subjId,
                                subjName,
                                rel.Doba()
                                );
                        sb.Append(PrintFlatRelations(rel, level + 1, relations, typ, renderedIds, withStats));
                        break;
                    case TiskEnum.Html:
                        if (withStats && stat != null)
                            sb.AppendFormat("<li class='{3} {6}'><a href='/subjekt/{0}'>{0}:{1}</a>{7}; {4}, celkem {5}. {2}</li>",
                                subjId,
                                subjName,
                                PrintFlatRelations(rel, level + 1, relations, typ, renderedIds, withStats),
                                last ? "" : "connect",
                                Devmasters.Lang.Plural.Get(stat.Summary().PocetSmluv, Util.Consts.csCulture, "{0} smlouva","{0} smlouvy","{0} smluv"),
                                Smlouva.NicePrice(stat.Summary().CelkovaHodnotaSmluv, html: true, shortFormat: true),
                                "aktualnost" + ((int)rel.Aktualnost).ToString(),
                                (rel.Aktualnost < AktualnostType.Aktualni) ? rel.Doba(format:"/{0}/") : string.Empty
                            );
                        else
                            sb.AppendFormat("<li class='{3} {4}'><a href='/subjekt/{0}'><span class=''>{0}:{1}</span></a>{5}.  {2}</li>",
                                subjId,
                                subjName,
                                PrintFlatRelations(rel, level + 1, relations, typ, renderedIds, withStats),
                                last ? "" : "connect",
                                "aktualnost" + ((int)rel.Aktualnost).ToString(),
                                (rel.Aktualnost < AktualnostType.Aktualni) ? rel.Doba(format: "/{0}/") : string.Empty,
                                (!string.IsNullOrEmpty(highlightSubjId) && subjId == highlightSubjId) ? "highlighted" : ""
                            );

                        break;
                    case TiskEnum.Checkbox:
                        sb.AppendFormat(@"<li class=""{0} {1}""><input type=""checkbox"" name=""ico"" value=""{2}"" /> <span><b>{2}</b> {3}</span>{4}</li>"
                        , (last ? "" : "connect"),
                        ("aktualnost" + ((int)rel.Aktualnost).ToString()),
                        subjId, subjName,
                        PrintFlatRelations(rel, level + 1, relations, typ, renderedIds, withStats)
                        );

                        break;
                    case TiskEnum.Json:
                        break;
                }

            }
            switch (typ)
            {
                case TiskEnum.Text:
                    break;
                case TiskEnum.Html:
                case TiskEnum.Checkbox:
                    sb.AppendLine("</ul>");
                    break;
                case TiskEnum.Json:
                    break;
            }

            return sb.ToString();
        }

        public static Graph.Edge[] AktualniVazby(IEnumerable<Graph.Edge> allRelations, Relation.AktualnostType minAktualnost)
        {
            if (allRelations == null)
                return new Graph.Edge[] { };

            return allRelations
                .Where(m => m.Aktualnost >= minAktualnost)
                .ToArray();
        }




        /*
     * Přehled kódů angažmá:
(00-99 jsou angažmá z OR, 400-499 z RŽP)
 00 - Podnikatel z OR
 01 - Člen statutárního orgánu
 02 - Likvidátor
 03 - Prokurista
 04 - Člen dozorčí rady
 05 - Jediný akcionář
 06 - Člen družstva s vkladem
 07 - Člen dozorčí rady v zastoupení
 08 - Člen kontrolní komise v zastoupení
 09 - Komplementář
 10 - Komanditista
 11 - Správce konkursu
 12 - Likvidátor v zastoupení
 13 - Oddělený insolvenční správce
 14 - Pobočný spolek
 15 - Podnikatel
 16 - Předběžný insolvenční správce
 17 - Předběžný správce
 18 - Představenstvo
 19 - Podílník
 20 - Revizor
 21 - Revizor v zastoupení
 22 - Člen rozhodčí komise
 23 - Vedoucí odštěpného závodu
 24 - Společník
 25 - Člen správní rady v zastoupení
 26 - Člen statutárního orgánu zřizovatele
 28 - Člen statutárního orgánu v zastoupení
 29 - Insolvenční správce - vyrovnávací
 31 - Člen správní rady
 32 - Statutární orgán zřizovatele v zastoupení
 33 - Zakladatel
 34 - Nástupce zřizovatele
 35 - Zakladatel s vkladem
 36 - Člen sdružení
 37 - Zástupce insolvenčního správce
 38 - Člen kontrolní komise
 39 - Insolvenční správce
 40 - Zástupce správce
 41 - Zvláštní insolvenční správce
 42 - Zvláštní správce
=400 - Podnikatel z RŽP
=401 - Statutár
=402 - Vedoucí org. složky
      */
        public static RelationSimpleEnum TypVazbyToRelationSimple(int typVazby)
        {
            switch (typVazby)
            {

                case -1:
                    return Relation.RelationSimpleEnum.Kontrola;
                case -2:
                    return Relation.RelationSimpleEnum.Vliv;
                case -3:
                    return Relation.RelationSimpleEnum.OsobniVztah;

                case 1:
                    //rel.Relationship = Relation.RelationDescriptionEnum.Jednatel;
                    return Relation.RelationSimpleEnum.Statutarni_organ;
                case 3:
                    //rel.Relationship = Relation.RelationDescriptionEnum.Prokura;
                    return Relation.RelationSimpleEnum.Statutarni_organ;
                case 4:
                case 7:
                case 2:
                case 18:
                case 25:
                case 26:
                case 28:
                case 31:
                    //rel.Relationship = Relation.RelationDescriptionEnum.Dozorci_rada;
                    return Relation.RelationSimpleEnum.Statutarni_organ;
                case 33:
                case 34:
                case 35:
                    //rel.Relationship = Relation.RelationDescriptionEnum.Dozorci_rada;
                    return Relation.RelationSimpleEnum.Zakladatel;
                case 5:
                case 9:
                case 10:
                case 15:
                case 19:
                case 24:
                    return Relation.RelationSimpleEnum.Spolecnik;
                case 100:
                    //rel.Relationship = Relation.RelationDescriptionEnum.Jednatel;
                    return Relation.RelationSimpleEnum.Souhrnny;
                case 23://
                case 29://
                case 11://
                case 12://
                case 13://
                case 16://
                case 17://
                case 37://
                case 40://
                case 41://
                case 42: //
                case 99:
                    return Relation.RelationSimpleEnum.Jiny;
                default:
                    //rel.Relationship = Relation.RelationDescriptionEnum.Jednatel;
                    return Relation.RelationSimpleEnum.Jiny;
            }
        }
        public static string TypVazbyToDescription(int typVazby)
        {
            RelationSimpleEnum simple = TypVazbyToRelationSimple(typVazby);

            switch (typVazby)
            {

                case 1:
                    return Relation.RelationSimpleEnum.Jednatel.ToNiceDisplayName();
                case 3:
                    return Relation.RelationSimpleEnum.Prokura.ToNiceDisplayName();
                case 4:
                case 7:
                case 2:
                case 18:
                case 25:
                case 26:
                case 28:
                case 31:
                    return Relation.RelationSimpleEnum.Dozorci_rada.ToNiceDisplayName();

                case 33:
                case 34:
                case 35:
                    return Relation.RelationSimpleEnum.Dozorci_rada.ToNiceDisplayName();
                case 5:
                case 9:
                case 10:
                case 15:
                case 19:
                case 24:
                    return Relation.RelationSimpleEnum.Spolecnik.ToNiceDisplayName();
                case 100:
                    return Relation.RelationSimpleEnum.Jednatel.ToNiceDisplayName();
                case 23://
                case 29://
                case 11://
                case 12://
                case 13://
                case 16://
                case 17://
                case 37://
                case 40://
                case 41://
                case 42: //
                case 99:
                default:
                    return simple.ToNiceDisplayName();
            }
        }
    }
}
