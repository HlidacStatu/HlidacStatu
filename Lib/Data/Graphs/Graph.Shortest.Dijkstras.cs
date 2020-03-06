namespace HlidacStatu.Lib.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial class Graph
    {
        public partial class Shortest
        {
            public class Dijkstras<T>
                where T : IComparable<T>
            {
                Dictionary<T, Dictionary<T, int>> vertices = null;
                IEqualityComparer<T> comparer = null;

                public Dijkstras() : this(null)
                { }

                public Dijkstras(IEqualityComparer<T> comparer)
                {
                    this.comparer = comparer;
                    vertices = new Dictionary<T, Dictionary<T, int>>(comparer);
                }

                protected void add_vertex(T name, Dictionary<T, int> edges)
                {
                    vertices[name] = edges;
                }
                protected void add_vertex(T name, IEnumerable<T> newvertices, int weight = 1)
                {
                    vertices[name] = newvertices.ToDictionary(k => k, v => weight);
                }

                public List<T> ShortestPath(T start, T finish)
                {
                    var previous = new Dictionary<T, T>(comparer);
                    var distances = new Dictionary<T, int>(comparer);
                    var lnodes = new List<T>();

                    List<T> path = null;

                    foreach (var vertex in vertices)
                    {
                        if (vertex.Key.CompareTo(start) == 0)
                        {
                            distances[vertex.Key] = 0;
                        }
                        else
                        {
                            distances[vertex.Key] = int.MaxValue;
                        }

                        lnodes.Add(vertex.Key);
                    }

                    while (lnodes.Count != 0)
                    {
                        lnodes.Sort((x, y) => distances[x] - distances[y]);

                        var smallest = lnodes[0];
                        lnodes.Remove(smallest);

                        if (smallest.CompareTo(finish) == 0)
                        {
                            path = new List<T>();
                            while (previous.ContainsKey(smallest))
                            {
                                path.Add(smallest);
                                smallest = previous[smallest];
                            }

                            break;
                        }

                        if (distances[smallest] == int.MaxValue)
                        {
                            break;
                        }

                        foreach (var neighbor in vertices[smallest])
                        {
                            var alt = distances[smallest] + neighbor.Value;
                            if (alt < distances[neighbor.Key])
                            {
                                distances[neighbor.Key] = alt;
                                previous[neighbor.Key] = smallest;
                            }
                        }
                    }

                    if (path != null)
                        path.Reverse();
                    return path;
                }


                public static void Test()
                {
                    Dijkstras<char> g = new Dijkstras<char>();
                    g.add_vertex('A', new Dictionary<char, int>() { { 'B', 7 }, { 'C', 8 } });
                    g.add_vertex('B', new Dictionary<char, int>() { { 'A', 7 }, { 'F', 2 } });
                    g.add_vertex('C', new Dictionary<char, int>() { { 'A', 8 }, { 'F', 6 }, { 'G', 4 } });
                    g.add_vertex('D', new Dictionary<char, int>() { { 'F', 8 }, { 'I', 3 } });
                    g.add_vertex('E', new Dictionary<char, int>() { { 'H', 1 } });
                    g.add_vertex('F', new Dictionary<char, int>() { { 'B', 2 }, { 'C', 6 }, { 'D', 8 }, { 'G', 9 }, { 'H', 3 } });
                    g.add_vertex('G', new Dictionary<char, int>() { { 'C', 4 }, { 'F', 9 } });
                    g.add_vertex('H', new Dictionary<char, int>() { { 'E', 1 }, { 'F', 3 } });
                    g.add_vertex('I', new Dictionary<char, int>() { });

                    g.ShortestPath('A', 'I').ForEach(x => Console.WriteLine(x));
                }
            }
        }
    }
}