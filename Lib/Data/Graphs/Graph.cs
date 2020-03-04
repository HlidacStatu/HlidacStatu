using Devmasters.Core;
using HlidacStatu.Util.Cache;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HlidacStatu.Lib.Data
{
    public partial class Graph
    {
        static DateTime minDate = new DateTime(1950, 1, 1);

        public static IEnumerable<string> UniqIds(IEnumerable<Edge> edges)
        {
            if (edges == null)
                return new string[] { };
            var uids = edges
                    .Select(m => m.From?.UniqId)
                    .Where(m => m != null)
                    .Union(edges
                        .Select(m => m.To?.UniqId)
                        .Where(m => m != null)
                    ).
                    Distinct();

            return uids;
        }

        private static CouchbaseCacheManager<List<Edge>, string> vazbyIcoCache
= CouchbaseCacheManager<List<Edge>, string>.GetSafeInstance("VsechnyDcerineVazby",
ico => vsechnyDcerineVazbyInternal(ico, 0, true, null),
TimeSpan.FromDays(3));

        private static CouchbaseCacheManager<List<Edge>, string> vazbyOsobaNameIdCache
            = CouchbaseCacheManager<List<Edge>, string>.GetSafeInstance("VsechnyDcerineVazbyOsoba",
            osobaNameId =>
            {
                if (string.IsNullOrEmpty(osobaNameId))
                    return new List<Edge>();
                Osoba o = Osoby.GetByNameId.Get(osobaNameId);
                return vsechnyDcerineVazbyInternal(o, 0, true, null);
            },
            TimeSpan.FromDays(3));

        public static List<Edge> VsechnyDcerineVazby(string ico, bool refresh = false)
        {
            if (refresh)
                vazbyIcoCache.Delete(ico);
            return vazbyIcoCache.Get(ico);
        }

        public static List<Edge> VsechnyDcerineVazby(Osoba person, bool refresh = false)
        {
            if (refresh)
                vazbyOsobaNameIdCache.Delete(person?.NameId);
            return vazbyOsobaNameIdCache.Get(person?.NameId);
        }

        public static List<Edge> vsechnyDcerineVazbyInternal(string ico, int level, bool goDeep, Edge parent,
ExcludeDataCol excludeICO = null, DateTime? datumOd = null, DateTime? datumDo = null,
Relation.AktualnostType aktualnost = Relation.AktualnostType.Libovolny)
        {
            string sql = @"select vazbakIco, datumOd, datumDo, typVazby, pojmenovaniVazby, podil from Firmavazby 
    where ico=@ico 
	and dbo.IsSomehowInInterval(@datumOd, @datumDo, datumOd, datumDo) = 1
";
            var p = new IDataParameter[] {
                new SqlParameter("ico",ico),
                new SqlParameter("datumOd",datumOd),
                new SqlParameter("datumDo",datumDo),
            };

            var rel = GetChildrenRelations(sql, Node.NodeType.Company, ico, datumOd, datumDo,
                p, level, goDeep, parent, excludeICO, aktualnost);
            return rel;
        }


        public static List<Edge> vsechnyDcerineVazbyInternal(Lib.Data.Osoba person, int level, bool goDeep, Edge parent,
            ExcludeDataCol excludeICO = null, IEnumerable<int> excludeOsobaId = null, DateTime? datumOd = null, DateTime? datumDo = null,
            Relation.AktualnostType aktualnost = Relation.AktualnostType.Libovolny)
        {
            if (excludeOsobaId == null)
                excludeOsobaId = new int[] { };

            string sql = @"select vazbakIco, vazbakOsobaId, datumOd, datumDo, typVazby, pojmenovaniVazby, podil from OsobaVazby 
                            where osobaId = @osobaId and
	(
		(datumOd <= @datumOd or @datumOd is null)
		OR (datumOd >= @datumOd and datumDo is null) 
	)
	and (datumDo >= @datumDo or datumDo is null or @datumDo is null)
";
            var p = new IDataParameter[] {
                new SqlParameter("osobaId",person.InternalId),
                new SqlParameter("datumOd",datumOd),
                new SqlParameter("datumDo",datumDo),
            };

            var relForPerson = GetChildrenRelations(sql, Node.NodeType.Person, person.InternalId.ToString(),
                datumOd, datumDo,
                p, level, goDeep, parent, excludeICO, aktualnost);

            var relForConnectedPersons = new List<Edge>();
            using (DbEntities db = new DbEntities())
            {
                var navazaneOsoby = db.OsobaVazby.Where(m => m.OsobaID == person.InternalId && m.VazbakOsobaId != null).ToList();
                if (navazaneOsoby.Count > 0)
                    foreach (var ov in navazaneOsoby)
                    {
                        Edge parentRelFound = relForPerson
                            .Where(r => r.To.Type == Node.NodeType.Person
                                    && r.To.Id == ov.VazbakOsobaId.Value.ToString())
                            .FirstOrDefault();

                        if (!excludeOsobaId.Contains(ov.VazbakOsobaId.Value))
                        {
                            Osoba o = Osoby.GetById.Get(ov.VazbakOsobaId.Value);
                            excludeOsobaId = excludeOsobaId.Union(new int[] { ov.VazbakOsobaId.Value });
                            var rel = vsechnyDcerineVazbyInternal(o, level + 1, true, parentRelFound, excludeOsobaId: excludeOsobaId, aktualnost: aktualnost);
                            relForConnectedPersons = Edge.Merge(relForConnectedPersons, rel);
                        }
                    }

            }

            var finalRel = Edge.Merge(relForConnectedPersons, relForPerson);
            return finalRel;
        }

        public static List<Graph.Edge> Holding(string ico, Relation.AktualnostType aktualnost)
        {
            DateTime? from = null;
            DateTime to = DateTime.Now.Date.AddDays(1);
            switch (aktualnost)
            {
                case Relation.AktualnostType.Aktualni:
                    from = to.AddDays(-1);
                    break;
                case Relation.AktualnostType.Nedavny:
                    from = to - Relation.NedavnyVztahDelka;
                    break;
                case Relation.AktualnostType.Neaktualni:
                case Relation.AktualnostType.Libovolny:
                    from = new DateTime(1950, 1, 1);
                    break;
                default:
                    break;
            }

            return Holding(ico, from.Value, to, aktualnost);
        }
        public static List<Graph.Edge> Holding(string ico, DateTime datumOd, DateTime datumDo, Relation.AktualnostType aktualnost)
        {
            var vazby = vsechnyDcerineVazbyInternal(ico, 9, true, null, datumOd: datumOd, datumDo: datumDo);
            var parents = GetParentRelations(ico, vazby, 0, datumOd, datumDo);
            var rootNode = new Node() { Id = ico, Type = Node.NodeType.Company };
            if (parents?.Count() > 0)
            {
                if (vazby.Any(m => m.Root))
                    if (vazby.Any(m => m.From == null && m.To.UniqId == rootNode.UniqId))
                    {
                        vazby = vazby
                            .Where(m => !(m.From == null && m.To.UniqId != rootNode.UniqId))
                            .ToList();
                    }
            }
            var finalRel = Edge.Merge(parents.ToList(), vazby);
            return finalRel;

        }

        public static List<Edge> GetChildrenRelations(string sql,
            Node.NodeType nodeType, string nodeId, DateTime? datumOd, DateTime? datumDo,
            IDataParameter[] parameters, int level, bool goDeep,
            Edge parent, ExcludeDataCol excludeICO, Relation.AktualnostType aktualnost)
        {
            if (excludeICO == null)
                excludeICO = new ExcludeDataCol();

            string cnnStr = Devmasters.Core.Util.Config.GetConfigValue("CnnString");

            List<Edge> relations = new List<Edge>();
            if (level == 0 && parent == null)
            {
                //add root node / edge
                relations.Add(
                    new Edge()
                    {
                        From = null,
                        Root = true,
                        To = new Node() { Id = nodeId, Type = nodeType },
                        RelFrom = datumOd,
                        RelTo = datumDo,
                        Distance = 0
                    }
                    );
            }
            //get zakladni informace o subj.

            //find politician in the DB
            var db = new Devmasters.Core.PersistLib();
            var sqlCall = HlidacStatu.Lib.DirectDB.GetRawSql(System.Data.CommandType.Text, sql, parameters);
            //string sqlFirma = "select top 1 stav_subjektu from firma where ico = @ico";

            var ds = db.ExecuteDataset(cnnStr, System.Data.CommandType.Text, sqlCall, null);

            if (ds.Tables[0].Rows.Count > 0)
            {
                List<AngazovanostData> rows = new List<AngazovanostData>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    AngazovanostData angaz = null;
                    var ico = (string)dr["VazbakIco"];
                    if (string.IsNullOrEmpty(ico))
                    {
                        if (dr.Table.Columns.Contains("vazbakOsobaId"))
                        {
                            var vazbakOsobaId = (int?)PersistLib.IsNull(dr["vazbakOsobaId"], null);
                            if (vazbakOsobaId != null)
                            {
                                Osoba o = Osoby.GetById.Get(vazbakOsobaId.Value);
                                angaz = new AngazovanostData()
                                {
                                    subjId = vazbakOsobaId.Value.ToString(),
                                    NodeType = Node.NodeType.Person,
                                    fromDate = (DateTime?)PersistLib.IsNull(dr["datumOd"], null),
                                    toDate = (DateTime?)PersistLib.IsNull(dr["datumDo"], null),
                                    kod_ang = Convert.ToInt32(dr["typVazby"]),
                                    descr = (string)PersistLib.IsNull(dr["PojmenovaniVazby"], ""),
                                    podil = (decimal?)PersistLib.IsNull(dr["podil"], null)
                                };

                            }

                        }
                    }
                    else
                    {
                        angaz = new AngazovanostData()
                        {
                            subjId = ico,
                            subjname = "",
                            NodeType = Node.NodeType.Company,
                            fromDate = (DateTime?)PersistLib.IsNull(dr["datumOd"], null),
                            toDate = (DateTime?)PersistLib.IsNull(dr["datumDo"], null),
                            kod_ang = Convert.ToInt32(dr["typVazby"]),
                            descr = (string)PersistLib.IsNull(dr["PojmenovaniVazby"], ""),
                            podil = (decimal?)PersistLib.IsNull(dr["podil"], null)
                        };
                    }
                    rows.Add(angaz);
                }

                List<AngazovanostData> filteredRels = new List<AngazovanostData>();
                //delete vazby ve stejnem obdobi
                if (rows.Count > 0)
                {
                    //per ico
                    foreach (var gIco in rows.Select(m => m.subjId).Distinct())
                    {
                        var relsForIco = rows.Where(m => m.subjId == gIco);
                        //find longest, or separate relation
                        foreach (var r in relsForIco)
                        {
                            if (relsForIco
                                .Any(rr =>
                                    rr != r
                                    && rr.fromDate < r.fromDate
                                    && (rr.toDate > r.toDate || rr.toDate.HasValue == false)
                                )
                            )
                            {
                                //skip
                            }
                            else
                                filteredRels.Add(r);
                        }

                    }
                }

                foreach (AngazovanostData ang in filteredRels.OrderBy(m => m.kod_ang))
                {
                    if (ang.kod_ang == 100) //souhrny (casove) vztah, zkontroluj, zda uz tam neni jiny vztah se stejnym rozsahem doby
                    {
                        if (
                            relations.Any(
                                    r => r.To.Id == ang.subjId
                                    && r.To.Type == ang.NodeType
                                    && r.RelFrom == ang.fromDate
                                    && r.RelTo == ang.toDate
                                )
                            )
                            continue;
                    }
                    var rel = AngazovanostDataToEdge(ang,
                        new Node() { Type = nodeType, Id = nodeId },
                        new Node() { Type = ang.NodeType, Id = ang.subjId },
                        level + 1
                        );


                    if (excludeICO.Contains(rel))
                        continue;//skip to the next

                    if (rel.Aktualnost >= aktualnost)
                        relations.Add(rel);
                }
            }

            if (goDeep && relations.Count > 0)
            {
                level++;
                List<Edge> deeperRels = new List<Edge>();
                List<Edge> excludeMore = new List<Edge>();
                if (parent != null)
                {
                    excludeMore = relations.ToList();
                }

                ////navazej na ten, ktery je nejdelsi
                //var parentRels = relations.GroupBy(x => new { x.To.Id }, (key, rels) =>
                //{
                //    DateTime? fromDate = rels.Any(m => m.FromDate == null) ? (DateTime?)null : rels.Min(m => m.FromDate);
                //    DateTime? toDate = rels.Any(m => m.ToDate == null) ? (DateTime?)null : rels.Max(m => m.ToDate);
                //    Relation bestRelation = Relation.GetLongestRelation(rels);

                //    return new
                //    {
                //        SubjIco = key.To.Id,
                //        FromDate = fromDate,
                //        ToDate = toDate,
                //        BestRelation = bestRelation,
                //    };

                //});





                foreach (var rel in relations.Where(m => m.Root == false))
                {
                    //old
                    deeperRels.AddRange(

                        vsechnyDcerineVazbyInternal(rel.To.Id, level, goDeep, rel,
                            excludeICO.AddItem(new ExcludeData(rel)),
                        rel.RelFrom, rel.RelTo, aktualnost)
                        );

                }
                relations.AddRange(deeperRels);
            }

            if (level == 0)
            {
                //remove inactive companies from last branches
                //TODO
            }
            return relations;
        }

        private static Edge AngazovanostDataToEdge(AngazovanostData ang, Node fromNode, Node toNode, int distance)
        {
            var rel = new Edge();
            rel.From = fromNode;
            rel.To = toNode;
            rel.Distance = distance;
            rel.RelFrom = (DateTime?)PersistLib.IsNull(ang.fromDate, null);
            if (rel.RelFrom < minDate)
                rel.RelFrom = null;

            rel.RelTo = (DateTime?)PersistLib.IsNull(ang.toDate, null);
            if (rel.RelTo < minDate)
                rel.RelTo = null;

            var relData = AngazovanostDataToRelationSimple(ang);
            rel.Descr = relData.Item2;
            if (string.IsNullOrEmpty(rel.Descr))
                rel.Descr = relData.Item1.ToNiceDisplayName();
            rel.UpdateAktualnost();
            return rel;
        }
        private static Tuple<Firma.RelationSimpleEnum, string> AngazovanostDataToRelationSimple(AngazovanostData ang)
        {
            Firma.RelationSimpleEnum relRelationship = Firma.RelationSimpleEnum.Jiny;
            string descr = ang.descr;
            /*
       3  - prokura
       4  - člen dozorčí rady    
       24 - spolecnik       
       5  - Jediný akcionář
       1 - jednatel        
    */


            switch (ang.kod_ang)
            {
                case 1:
                    relRelationship = Firma.RelationSimpleEnum.Statutarni_organ;
                    if (string.IsNullOrEmpty(descr))
                        descr = Firma.RelationSimpleEnum.Jednatel.ToNiceDisplayName();
                    break;
                case 3:
                    relRelationship = Firma.RelationSimpleEnum.Statutarni_organ;
                    if (string.IsNullOrEmpty(descr))
                        descr = Firma.RelationSimpleEnum.Prokura.ToNiceDisplayName();
                    break;
                case 4:
                case 7:
                case 2:
                case 18:
                case 25:
                case 26:
                case 28:
                case 31:
                    relRelationship = Firma.RelationSimpleEnum.Statutarni_organ;
                    if (string.IsNullOrEmpty(descr))
                        descr = Firma.RelationSimpleEnum.Dozorci_rada.ToNiceDisplayName();
                    break;
                case 33:
                case 34:
                case 35:
                    relRelationship = Firma.RelationSimpleEnum.Zakladatel;
                    if (string.IsNullOrEmpty(descr))
                        descr = Firma.RelationSimpleEnum.Dozorci_rada.ToNiceDisplayName();
                    break;
                case 5:
                case 9:
                case 10:
                case 15:
                case 19:
                case 24:
                    relRelationship = Firma.RelationSimpleEnum.Spolecnik;
                    if (string.IsNullOrEmpty(descr))
                        descr = Firma.RelationSimpleEnum.Spolecnik.ToNiceDisplayName();
                    break;
                case 100:
                    relRelationship = Firma.RelationSimpleEnum.Souhrnny;
                    if (string.IsNullOrEmpty(descr))
                        descr = Firma.RelationSimpleEnum.Jednatel.ToNiceDisplayName();
                    break;
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
                    relRelationship = Firma.RelationSimpleEnum.Jiny;
                    break;
                default:

                    if (ang.kod_ang < 0)
                        relRelationship = (Firma.RelationSimpleEnum)ang.kod_ang;
                    else
                    {
                        //rel.Relationship = Relation.RelationDescriptionEnum.Jednatel;
                        relRelationship = Firma.RelationSimpleEnum.Jiny;
                        if (string.IsNullOrEmpty(descr))
                            descr = Firma.RelationSimpleEnum.Jednatel.ToNiceDisplayName();
                    }
                    break;
            }

            return new Tuple<Firma.RelationSimpleEnum, string>(relRelationship, descr);
        }

        public static IEnumerable<Graph.Edge> GetDirectParentRelationsFirmy(string ico)
        {

            string sql = @"select ico, vazbakIco, datumOd, datumDo, typVazby, pojmenovaniVazby, podil  from Firmavazby 
            where vazbakico=@ico 
        ";

            string cnnStr = Devmasters.Core.Util.Config.GetConfigValue("CnnString");

            List<Graph.Edge> relations = new List<Graph.Edge>();
            //get zakladni informace o subj.

            //find politician in the DB
            var db = new Devmasters.Core.PersistLib();
            var sqlCall = HlidacStatu.Lib.DirectDB.GetRawSql(System.Data.CommandType.Text, sql, new IDataParameter[] {
                        new SqlParameter("ico",ico)
                    });
            //string sqlFirma = "select top 1 stav_subjektu from firma where ico = @ico";

            var ds = db.ExecuteDataset(cnnStr, System.Data.CommandType.Text, sqlCall, null);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var parents = ds.Tables[0].AsEnumerable()
                    .Where(m => (string)m["ico"] != ico)
                    .Select(dr => new AngazovanostData()
                    {
                        subjId = (string)dr["ico"],
                        subjname = "",
                        NodeType = Node.NodeType.Company,
                        fromDate = (DateTime?)PersistLib.IsNull(dr["datumOd"], null),
                        toDate = (DateTime?)PersistLib.IsNull(dr["datumDo"], null),
                        kod_ang = Convert.ToInt32(dr["typVazby"]),
                        descr = (string)PersistLib.IsNull(dr["PojmenovaniVazby"], ""),
                        podil = (decimal?)PersistLib.IsNull(dr["podil"], null)
                    })
                    .ToArray();
                var ret = new List<Edge>();
                for (int i = 0; i < parents.Length; i++)
                {
                    AngazovanostData ang = parents[i];
                    var rel = AngazovanostDataToEdge(ang,
                                new Node() { Type = Node.NodeType.Company, Id = ang.subjId },
                                new Node() { Type = Node.NodeType.Company, Id = ico },
                                -1
                                );
                    ret.Add(rel);
                }
                return Edge.GetLongestUniqueEdges(ret);
            }
            return new Graph.Edge[] { };
        }
        public static IEnumerable<Graph.Edge> GetDirectParentRelationsOsoby(string ico)
        {

            string sql = @"select OsobaID, datumOd, datumDo, typVazby, pojmenovaniVazby, podil from osobavazby 
                        where vazbakico=@ico 
                    ";

            string cnnStr = Devmasters.Core.Util.Config.GetConfigValue("CnnString");

            List<Graph.Edge> relations = new List<Graph.Edge>();
            //get zakladni informace o subj.

            //find politician in the DB
            var db = new Devmasters.Core.PersistLib();
            var sqlCall = HlidacStatu.Lib.DirectDB.GetRawSql(System.Data.CommandType.Text, sql, new IDataParameter[] {
                        new SqlParameter("ico",ico)
                    });
            //string sqlFirma = "select top 1 stav_subjektu from firma where ico = @ico";

            var ds = db.ExecuteDataset(cnnStr, System.Data.CommandType.Text, sqlCall, null);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var parents = ds.Tables[0].AsEnumerable()
                    .Select(dr => new AngazovanostData()
                    {
                        subjId = ((int)dr["osobaId"]).ToString(),
                        subjname = "",
                        NodeType = Node.NodeType.Company,
                        fromDate = (DateTime?)PersistLib.IsNull(dr["datumOd"], null),
                        toDate = (DateTime?)PersistLib.IsNull(dr["datumDo"], null),
                        kod_ang = Convert.ToInt32(dr["typVazby"]),
                        descr = (string)PersistLib.IsNull(dr["PojmenovaniVazby"], ""),
                        podil = (decimal?)PersistLib.IsNull(dr["podil"], null)
                    })
                    .ToArray();
                var ret = new List<Edge>();
                for (int i = 0; i < parents.Length; i++)
                {
                    AngazovanostData ang = parents[i];
                    var rel = AngazovanostDataToEdge(ang,
                                new Node() { Type = Node.NodeType.Person, Id = ang.subjId },
                                new Node() { Type = Node.NodeType.Person, Id = ico },
                                -1
                                );

                    ret.Add(rel);
                }
                return Edge.GetLongestUniqueEdges(ret);
            }
            return new Graph.Edge[] { };
        }

        public static IEnumerable<Graph.Edge> GetParentRelations(string ico,
            IEnumerable<Graph.Edge> currRelations, int distance,
            DateTime datumOd, DateTime datumDo)
        {

            string sql = @"select ico, vazbakIco, datumOd, datumDo, typVazby, pojmenovaniVazby, podil from Firmavazby 
            where vazbakico=@ico 
        	and dbo.IsSomehowInInterval(@datumOd, @datumDo, datumOd, datumDo) = 1
        ";

            string cnnStr = Devmasters.Core.Util.Config.GetConfigValue("CnnString");

            List<Graph.Edge> relations = currRelations?.ToList() ?? new List<Graph.Edge>();
            //get zakladni informace o subj.

            //find politician in the DB
            var db = new Devmasters.Core.PersistLib();
            var sqlCall = HlidacStatu.Lib.DirectDB.GetRawSql(System.Data.CommandType.Text, sql, new IDataParameter[] {
                        new SqlParameter("ico",ico),
                        new SqlParameter("datumOd",datumOd),
                        new SqlParameter("datumDo",datumDo),
                    });
            //string sqlFirma = "select top 1 stav_subjektu from firma where ico = @ico";

            var ds = db.ExecuteDataset(cnnStr, System.Data.CommandType.Text, sqlCall, null);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var parentIcos = ds.Tables[0].AsEnumerable()
                    .Select(dr => (string)dr["ico"])
                    .Where(m => m != ico);


                foreach (var parentIco in parentIcos)
                {

                    var parentRels = vsechnyDcerineVazbyInternal(parentIco, 0, true, null,
                        new ExcludeDataCol() { items = relations?.Select(m => new ExcludeData(m)).ToList() }
                        , datumOd: datumOd, datumDo: datumDo
                        );
                    //move

                    //add edge with parent 
                    var currEdge = new Edge()
                    {
                        From = new Node() { },
                        To = new Node() { },
                        RelFrom = datumOd,
                        RelTo = datumDo,
                    };
                    currEdge.UpdateAktualnost();
                    relations.Add(currEdge);

                    if ((parentRels?.Count() ?? 0) > 0)
                    {
                        relations.AddRange(parentRels);
                    }

                }
                foreach (var parentIco in parentIcos)
                {
                    var parents = GetParentRelations(parentIco, relations, distance + 1, datumOd, datumDo);
                    relations.AddRange(parents);
                }
                return relations;
            }
            return new Graph.Edge[] { };
        }



        private class AngazovanostData
        {
            public string subjId { get; set; }
            public string subjname { get; set; }
            public byte stav { get; set; }
            public DateTime? fromDate { get; set; }
            public DateTime? toDate { get; set; }
            public string descr { get; set; }
            public int kod_ang { get; set; }
            public decimal? podil { get; set; }
            public Graph.Node.NodeType NodeType { get; set; }

        }

        public class ExcludeData
        {
            public ExcludeData() { }
            public ExcludeData(Graph.Edge r)
            {
                this.parent = r.From;
                this.child = r.To;
                this.from = r.RelFrom;
                this.to = r.RelTo;
            }

            public Graph.Node parent { get; set; }
            public Graph.Node child { get; set; }
            public DateTime? from { get; set; }
            public DateTime? to { get; set; }

            public bool Mergable(ExcludeData newI)
            {
                if (newI.from.HasValue && this.to.HasValue
                    && this.to < newI.from
                    )
                    return false;

                if (newI.to.HasValue && this.from.HasValue
                    && this.from > newI.to
                    )
                    return false;

                if (newI.from == null && newI.to == null)
                    return true;

                //at least in one interval
                if (this.from.HasValue && this.to.HasValue)
                {
                    if (newI.from.HasValue && (newI.from >= this.from && newI.from <= this.to))
                        return true;
                    if (newI.to.HasValue && (newI.to >= this.from && newI.to <= this.to))
                        return true;
                }
                else if (this.from.HasValue == false && this.to.HasValue)
                {
                    if (newI.from.HasValue && (newI.from <= this.to))
                        return true;
                    if (newI.to.HasValue && (newI.to <= this.to))
                        return true;
                }
                else if (this.from.HasValue && this.to.HasValue == false)
                {
                    if (newI.from.HasValue && (newI.from >= this.from))
                        return true;
                    if (newI.to.HasValue && (newI.to >= this.from))
                        return true;
                }
                else if (this.from.HasValue == false && this.to.HasValue == false)
                    return true;

                return false;
            }

            public void Merge(ExcludeData i)
            {
                if (i.from == null)
                    this.from = i.from;
                if (i.to == null)
                    this.to = i.to;

                if (this.from.HasValue && i.from.HasValue && i.from < this.from)
                    this.from = i.from;
                if (this.to.HasValue && i.to.HasValue && i.to > this.to)
                    this.to = i.to;

            }
        }
        public class ExcludeDataCol
        {
            public List<ExcludeData> items { get; set; } = new List<ExcludeData>();

            public ExcludeDataCol AddItem(ExcludeData i)
            {
                var icoItems = items.Where(m => m.parent == i.parent && m.child == i.child);
                if (items.Count() > 0)
                {
                    foreach (var it in icoItems)
                    {
                        if (it.Mergable(i))
                        {
                            it.Merge(i);
                            return this;
                        }
                    }
                    items.Add(i);
                }
                else
                    items.Add(i);
                return this;
            }

            public ExcludeDataCol AddItems(IEnumerable<ExcludeData> items)
            {
                foreach (var i in items)
                    AddItem(i);

                return this;
            }

            public bool Contains(Graph.Edge r)
            {
                var ex = new ExcludeData(r);
                return items
                    .Where(m => m.parent?.UniqId == ex.parent?.UniqId && m.child?.UniqId == ex.child?.UniqId)
                    .Any(m => m.Mergable(ex));
            }

        }

    }

}
