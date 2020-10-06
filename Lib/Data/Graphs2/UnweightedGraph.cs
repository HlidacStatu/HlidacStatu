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
            Vertex<T> vertex1 = GetOrAddVertex(from);
            Vertex<T> vertex2 = GetOrAddVertex(to);

            vertex1.AddNeighbor(vertex2);
            vertex2.AddNeighbor(vertex1);

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
        public IEnumerable<Vertex<T>> ShortestPath(Vertex<T> from, Vertex<T> to)
        {
            var visitHistory = new Dictionary<Vertex<T>, Vertex<T>>();

            var queue = new Queue<(Vertex<T> vertex, Vertex<T> parent)>();
            queue.Enqueue((from, null));

            var result = new List<Vertex<T>>();
            while (queue.Count > 0)
            {
                var (vertex, parent) = queue.Dequeue();
                visitHistory.Add(vertex, parent);

                if(vertex == to)
                {
                    result.Add(vertex);
                                        
                    while (visitHistory.TryGetValue(vertex, out Vertex<T> previous) && previous != null)
                    {
                        result.Add(previous);
                    }

                    return result.Reverse<Vertex<T>>();
                }

                foreach (var neighbor in vertex.Neighbors)
                {
                    if (!visitHistory.Keys.Contains(neighbor))
                    {
                        queue.Enqueue((vertex: neighbor, parent: vertex ));
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
            var visited = new HashSet<Vertex<T>>();
            
            Queue<Vertex<T>> queue = new Queue<Vertex<T>>();
            queue.Enqueue(from);

            while (queue.Count > 0)
            {
                var currentVertex = queue.Dequeue();
                visited.Add(currentVertex);
                yield return currentVertex;
                
                foreach (var neighbor in currentVertex.Neighbors)
                {
                    if(!visited.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

    }
}
