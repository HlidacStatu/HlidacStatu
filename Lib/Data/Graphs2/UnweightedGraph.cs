using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Data.Graphs2
{
    public class UnweightedGraph<T>
    {
        public UnweightedGraph(IEnumerable<Vertex<T>> initialNodes = null)
        {
            Vertices = initialNodes?.ToHashSet() ?? new HashSet<Vertex<T>>();
        }

        public HashSet<Vertex<T>> Vertices { get; }

        /// <summary>
        /// Přidá nové položky do grafu.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void AddEdge(Vertex<T> from, Vertex<T> to, string bindingName)
        {
            //Vertex<T> vertex1 = GetOrAddVertex(from);
            //Vertex<T> vertex2 = GetOrAddVertex(to);

            //vertex1.AddEdge(vertex2);
            //vertex2.AddEdge(vertex1);

        }

        private Vertex<T> GetOrAddVertex(Vertex<T> vertex)
        {
            if (Vertices.TryGetValue(vertex, out Vertex<T> actual))
            {
                return actual;
            }
            
            Vertices.Add(vertex);
            return (vertex);
        }


        /// <summary>
        /// Najde nejkratší cestu v neohodnoceném grafu. Vrátí seznam vrcholů.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public IEnumerable<Edge<T>> ShortestPath(Vertex<T> from, Vertex<T> to)
        {
            var visitHistory = new Stack<Edge<T>>();
            var visitedVertices = new HashSet<Vertex<T>>();

            Queue<Vertex<T>> queuedVertices = new Queue<Vertex<T>>();
            queuedVertices.Enqueue(from);

            while (queuedVertices.Count > 0)
            {
                var currentVertex = queuedVertices.Dequeue();
                visitedVertices.Add(currentVertex);

                bool isSearchedVertex = currentVertex == to;
                if (isSearchedVertex)
                {
                    var results = new List<Edge<T>>();
                    var previousVertex = to;

                    while(visitHistory.Count > 0)
                    {
                        var edge = visitHistory.Pop();

                        if(edge.To == previousVertex)
                        {
                            results.Add(edge);
                            previousVertex = edge.From;
                        }

                        if(edge.From == from)
                        {
                            break;
                        }
                    }

                    return results.Reverse<Edge<T>>();
                }

                foreach (var edge in currentVertex.Edges)
                {
                    bool unvisitedVertex = !visitedVertices.Contains(edge.To);
                    if (unvisitedVertex)
                    {
                        visitHistory.Push(edge);
                        queuedVertices.Enqueue(edge.To);
                    }
                }
            }

            throw new Exception("No path was found");
            
        }

        /// <summary>
        /// Projde (do šířky) všechny vrcholy od konkrétního bodu a postupně je vypíše.
        /// </summary>
        /// <param name="from">Vrchol, ze kterého se bude graf procházet</param>
        /// <returns></returns>
        public IEnumerable<Vertex<T>> BreathFirstIterator(Vertex<T> from)
        {
            var visitedVertices = new HashSet<Vertex<T>>();
            
            Queue<Vertex<T>> queuedVertices = new Queue<Vertex<T>>();
            queuedVertices.Enqueue(from);

            while (queuedVertices.Count > 0)
            {
                var currentVertex = queuedVertices.Dequeue();
                visitedVertices.Add(currentVertex);
                yield return currentVertex;
                
                foreach (var edge in currentVertex.Edges)
                {
                    if(!visitedVertices.Contains(edge.To))
                    {
                        queuedVertices.Enqueue(edge.To);
                    }
                }
            }
        }

    }
}
