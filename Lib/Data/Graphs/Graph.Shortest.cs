namespace HlidacStatu.Lib.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial class Graph
    {
        public partial class Shortest
        {
            public class EdgePath : Dijkstras<Node>
            {
                protected Node start = null;
                protected IEnumerable<Edge> edges = null;
                protected IEnumerable<Node> nodes = null;
                public long calls = 0;
                public EdgePath(IEnumerable<Edge> edges)
                    : base(new NodeComparer())
                {
                    start = edges.Where(m => m.Root).FirstOrDefault()?.To;
                    Init(start, edges);
                }

                public void Init(Node root, IEnumerable<Edge> edges)
                {
                    this.edges = edges;
                    this.nodes = edges.Select(m => m.From)
                        .Union(edges.Select(m => m.To))
                        //.Union(new Node[] { root })
                        .Where(n => n != null)
                        .Distinct(new NodeComparer());

                    foreach (var n in this.nodes)
                    {
                        add_vertex(n, edges
                            .Where(m => m.From != null && m.From.UniqId == n.UniqId)
                            .Select(m => m.To)
                            .Distinct(new NodeComparer())
                                , 1);
                    }

                }

                public IEnumerable<Edge> ShortestTo(string ico)
                {
                    var node = this.nodes
                        .Where(n => n.Type == Node.NodeType.Company && n.Id == ico)
                        .FirstOrDefault();
                    if (node == null)
                        return new Edge[] { };
                    return this.ShortestTo(node);
                }

                public IEnumerable<Edge> ShortestTo(Node node)
                {
                    calls++;
                    List<Edge> edges = new List<Edge>();
                    var nodes =  this.ShortestPath(start, node);
                    if (nodes == null)
                        return edges;
                     
                    //convert to edges
                    var prev = start;
                    foreach (var n in nodes)
                    {
                        //found corresponding edge
                        var foundE = this.edges
                            .Where(m => m.From?.UniqId == prev?.UniqId && m.To?.UniqId == n.UniqId)
                            //.Where(m=> Devmasters.DT.Util.IsOverlappingIntervals(overlapDateFrom,overlapDateTo,m.RelFrom, m.RelTo))
                            .OrderBy(e=>e.NumberOfDaysFromToday())
                            .ThenByDescending(e=>e.LengthOfEdgeInDays())
                            .FirstOrDefault(); //TODO zmena na vsechna obdobi
                        if (foundE != null)
                        {
                            edges.Add(foundE);
                            prev = n;
                        }
                        else
                        {
                            Util.Consts.Logger.Error("missing edge " + prev?.UniqId + " -> " + n?.UniqId);
                            //return edges;
                        }
                    }
                    return edges;
                }

            }
        }
    }
}