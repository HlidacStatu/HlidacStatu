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
        public IEnumerable<Edge<T>> Edges { get => Vertices.SelectMany(v => v.OutgoingEdges); }
        
        /// <summary>
        /// Přidá nové položky do grafu.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void AddEdge(Vertex<T> from, Vertex<T> to, string bindingName)
        {
            Vertex<T> vertex1 = GetOrAddVertex(from);
            Vertex<T> vertex2 = GetOrAddVertex(to);

            var edge = new Edge<T>(vertex1, vertex2, bindingName);

            vertex1.AddOutgoingEdge(edge);
            
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
            bool fromExists = Vertices.TryGetValue(from, out from);
            bool toExists = Vertices.TryGetValue(to, out to);
            if (!fromExists)
                throw new Exception("From parameter not found in vertices");
            if (!toExists)
                throw new Exception("To parameter was not found in vertices");

            var visitHistory = new List<Edge<T>>();
            var visitedVertices = new HashSet<Vertex<T>>();

            Queue<Vertex<T>> queuedVertices = new Queue<Vertex<T>>();
            queuedVertices.Enqueue(from);

            while (queuedVertices.Count > 0)
            {
                var currentVertex = queuedVertices.Dequeue();
                visitedVertices.Add(currentVertex);
                
                foreach (var edge in currentVertex.OutgoingEdges)
                {
                    if (visitedVertices.Contains(edge.To))
                        continue;
                    
                    visitHistory.Add(edge);
                    queuedVertices.Enqueue(edge.To);

                    bool isSearchedVertex = edge.To == to;
                    if (isSearchedVertex)
                        return GetPath(from, to, visitHistory);

                }
            }

            throw new Exception("No path was found");
            
        }

        private IEnumerable<Edge<T>> GetPath(Vertex<T> from, Vertex<T> to, List<Edge<T>> visitHistory)
        {
            var results = new List<Edge<T>>();
            var previousVertex = to;

            do
            {
                var edge = visitHistory.Where(e => e.To.Equals(previousVertex)).FirstOrDefault();
                results.Add(edge);
                previousVertex = edge.From;

            } while (!previousVertex.Equals(from));
            
            return results.Reverse<Edge<T>>();
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
                
                foreach (var edge in currentVertex.OutgoingEdges)
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
