using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace HlidacStatu.Lib.Data
{
    public partial class Graph
    {
        [System.Diagnostics.DebuggerDisplay("{debuggerdisplay,nq}")]
        public class Edge : IComparable<Edge>
        {
            [Newtonsoft.Json.JsonIgnore]
            public string UniqId
            {
                get
                {
                    var s = string.Format("{0} ==[{2} -> {3}]==> {1} {4}",
                        this.From?.UniqId ?? "Ø", this.To?.UniqId ?? "Ø",
                        this.RelFrom?.ToShortDateString() ?? "Ø", this.RelTo?.ToShortDateString() ?? "Ø",
                        this.Root ? "root" : ""
                        );
                    return s;
                }
            }
            public bool Root { get; set; }
            public Node From { get; set; }
            public Node To { get; set; }
            public DateTime? RelFrom { get; set; }
            public DateTime? RelTo { get; set; }
            public string Descr { get; set; }
            public int Distance { get; set; }
            public Relation.AktualnostType Aktualnost { get; set; }

            public void UpdateAktualnost()
            {
                if (this.RelTo.HasValue)
                {
                    if ((DateTime.Now.Date - this.RelTo.Value).TotalDays < 0)
                        this.Aktualnost = Relation.AktualnostType.Aktualni;
                    else if ((DateTime.Now.Date - this.RelTo.Value).TotalDays < Relation.NedavnyVztahDelka.TotalDays)
                        this.Aktualnost = Relation.AktualnostType.Nedavny;
                    else
                        this.Aktualnost = Relation.AktualnostType.Neaktualni;
                }
                else
                {
                    Aktualnost = Relation.AktualnostType.Aktualni;
                }
            }


            [Newtonsoft.Json.JsonIgnore]
            private string debuggerdisplay
            {
                get
                {
                    var s = string.Format("{0} ==[{2} -> {3}]==> {1}",
                        this.From?.UniqId ?? "Ø", this.To?.UniqId ?? "Ø",
                        this.RelFrom?.ToShortDateString() ?? "Ø", this.RelTo?.ToShortDateString() ?? "Ø");
                    return s;
                }
            }

            public string Doba(string format = null, string betweenDates = null)
            {
                if (string.IsNullOrEmpty(format))
                    format = "({0})";
                if (string.IsNullOrEmpty(betweenDates))
                    betweenDates = " - ";
                string datumy = string.Empty;

                if (this.RelFrom.HasValue && this.RelTo.HasValue)
                {
                    datumy = string.Format("{0}{2}{1}", this.RelFrom.Value.ToShortDateString(), this.RelTo.Value.ToShortDateString(), betweenDates);
                }
                else if (this.RelTo.HasValue)
                {
                    datumy = string.Format("do {0}", this.RelTo.Value.ToShortDateString());
                }
                else if (this.RelFrom.HasValue)
                {
                    datumy = string.Format("od {0}", this.RelFrom.Value.ToShortDateString());
                }
                if (string.IsNullOrEmpty(datumy))
                    return string.Empty;

                return string.Format(format, datumy);
            }

            public double NumberOfDaysFromToday()
            {
                var today = DateTime.Now;
                if (this.RelFrom.HasValue && this.RelTo.HasValue)
                    return (today - this.RelTo.Value).TotalDays;
                else if (RelFrom.HasValue == false && this.RelTo.HasValue)
                    return (today - this.RelTo.Value).TotalDays;
                else if (RelFrom.HasValue && this.RelTo.HasValue == false)
                    return 0;
                else
                    return 0;

            }

            public double LengthOfEdgeInDays()
            {
                if (this.RelFrom.HasValue && this.RelTo.HasValue)
                    return (this.RelTo.Value - this.RelFrom.Value).TotalDays;
                else if (RelFrom.HasValue == false && this.RelTo.HasValue)
                    return (this.RelTo.Value - new DateTime(1990, 1, 1)).TotalDays;
                else if (RelFrom.HasValue && this.RelTo.HasValue == false)
                    return (DateTime.Now - this.RelFrom.Value).TotalDays;
                else
                    return double.MaxValue;

            }

            public static List<Edge> Merge(List<Edge> relFirst, List<Edge> relSecond)
            {

                if (relFirst == null)
                    relFirst = new List<Edge>();
                if (relSecond == null)
                    relSecond = new List<Edge>();

                //merge old and new 
                //first clean old
                //relFromDB = relFromDB.Where(m => !string.IsNullOrEmpty(m.To.Id)).ToList();
                //relFromFirmo = relFromFirmo.Where(m => !string.IsNullOrEmpty(m.To.Id)).ToList();

                List<Edge> finalRelations = new List<Edge>();
                for (int i = 0; i < relSecond.Count(); i++)
                {
                    var v = relSecond[i];
                    int? same = null;
                    if (relFirst != null)
                    {
                        for (int j = 0; j < relFirst.Count(); j++)
                        {
                            var oldV = relFirst[j];
                            if (oldV.To.Id == v.To.Id && oldV.To.Type == oldV.To.Type)
                            {

                                if (
                                        (oldV.RelFrom == v.RelFrom || oldV.RelTo == v.RelTo)
                                    //&& (oldV.Relationship == v.Relationship)
                                    )
                                {
                                    same = j;
                                    break;
                                }

                            }
                        }
                    }
                    if (same.HasValue)
                    {
                        var vl = relFirst.ToList();
                        vl.RemoveAt(same.Value);
                        relFirst = vl.ToList();
                    }
                    finalRelations.Add(v);

                }

                return finalRelations.Concat(relFirst).ToList();

            }

            public static Edge[] GetLongestUniqueEdges(IEnumerable<Edge> relations)
            {
                if (relations == null)
                    return null;
                if (relations.Count() < 2)
                    return relations.ToArray();

                var longestE = new List<Edge>();

                var uniqEdges = relations
                                    .Select(r => string.Join("|", r.From.UniqId, r.To.UniqId))
                                    .Distinct();

                foreach (var uniq in uniqEdges)
                {
                    var eParts = uniq.Split('|');

                    var le = GetLongestEdge(relations
                                 .Where(r => r.From.UniqId == eParts[0] && r.To.UniqId == eParts[1]));

                    if (le != null)
                        longestE.Add(le);
                }

                return longestE.ToArray();
            }
            public static Edge[] GetMergedLongestEdgesBetweenSameNodes(IEnumerable<Edge> relations)
            {
                if (relations == null)
                    return null;
                if (relations.Count() < 2)
                    return relations.ToArray();

                var uniqNodes = relations
                    .Select(r => string.Join("|", r.From.UniqId, r.To.UniqId, r.Distance.ToString()))
                    .Distinct();
                if (uniqNodes.Count() > 1)
                    throw new ArgumentOutOfRangeException("only same nodes and distance in relations");

                var rels = relations.ToArray();
                var longestEdges = new List<Edge>();

                for (int i = 0; i < rels.Length; i++)
                {
                    long fromDateT = rels[i].RelFrom?.Ticks ?? long.MinValue;
                    long toDateT = rels[i].RelTo?.Ticks ?? long.MaxValue;

                    if (fromDateT == long.MinValue && toDateT == long.MaxValue)
                    {
                        var longest = GetLongestEdge(rels);
                        var e = new Edge()
                        {
                            Descr = longest.Descr, From = longest.From,To = longest.To,
                            Distance = longest.Distance,RelFrom = null,RelTo = null
                        };
                        e.UpdateAktualnost();
                        return new Edge[] { e };
                    }

                }

                throw new NotImplementedException();
            }

            public static Edge GetLongestEdge(IEnumerable<Edge> relations)
            {
                DateTime? fromDate = relations.Any(m => m.RelFrom == null) ? (DateTime?)null : relations.Min(m => m.RelFrom);
                DateTime? toDate = relations.Any(m => m.RelTo == null) ? (DateTime?)null : relations.Max(m => m.RelTo);
                Edge bestRelation = relations.Where(r => r.RelFrom == fromDate && r.RelTo == toDate).FirstOrDefault();
                if (bestRelation == null)
                {
                    return relations.OrderByDescending(m => m.LengthOfEdgeInDays()).FirstOrDefault();
                }
                else
                    return bestRelation;
            }

            public static Edge GetNewestPossibleEdge(IEnumerable<Edge> relations, DateTime? fromDate, DateTime? toDate)
            {
                Edge bestRelation = relations.Where(r =>
                                                            (fromDate.HasValue == false || fromDate <= r.RelFrom)
                                                            &&
                                                            (toDate.HasValue == false || r.RelTo <= toDate)
                                                        )
                                                .OrderByDescending(m => fromDate)
                                                .FirstOrDefault();
                if (bestRelation == null)
                {
                    //bestRelation = relations.Where(m => m.Relationship == RelationSimpleEnum.Souhrnny).FirstOrDefault();
                    return relations.OrderByDescending(m => m.LengthOfEdgeInDays()).FirstOrDefault();
                }
                else
                    return bestRelation;
            }

            public int CompareTo(Edge other)
            {
                if (other == null)
                    return 1;
                if (this.UniqId == other.UniqId)
                    return 0;
                else
                    return -1;
            }
        }

    }

}
