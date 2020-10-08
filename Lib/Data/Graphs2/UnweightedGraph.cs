using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Data.Graphs2
{
    public class UnweightedGraph
    {
        public UnweightedGraph(IEnumerable<IVertex> initialNodes = null)
        {
            Vertices = initialNodes?.ToHashSet() ?? new HashSet<IVertex>();
        }

        public HashSet<IVertex> Vertices { get; }
        public IEnumerable<IEdge> Edges { get => Vertices.SelectMany(v => v.OutgoingEdges); }
        
        /// <summary>
        /// Add new directed unweighted edge to graph. If Vertices (from, to) doesn't exist. It adds them too.
        /// </summary>
        /// <param name="from">Starting vertex</param>
        /// <param name="to">Destination vertex</param>
        public void AddEdge<T>(IVertex from, IVertex to, T bindingPayload)
        {
            IVertex vertex1 = GetOrAddVertex(from);
            IVertex vertex2 = GetOrAddVertex(to);
            
            //dont like this, but can't think better solution right now
            var edge = new Edge<T>(vertex1, vertex2, bindingPayload); 

            // There can be only one direct outgoing edge from A to B
            // Other outgoing edges from A to B are skipped in graph
            vertex1.AddOutgoingEdge(edge);
        }

        private IVertex GetOrAddVertex(IVertex vertex)
        {
            if (Vertices.TryGetValue(vertex, out IVertex actual))
            {
                return actual;
            }
            
            Vertices.Add(vertex);
            return (vertex);
        }


        /// <summary>
        /// Finds shortest path from point A to point B.
        /// </summary>
        /// <param name="from">Starting vertex</param>
        /// <param name="to">Destination vertex</param>
        /// <returns>Edges in order along the path (from => to)</returns>
        public IEnumerable<IEdge> ShortestPath(IVertex from, IVertex to)
        {
            bool fromExists = Vertices.TryGetValue(from, out from);
            bool toExists = Vertices.TryGetValue(to, out to);
            if (!fromExists)
                throw new Exception("From parameter not found in vertices");
            if (!toExists)
                throw new Exception("To parameter was not found in vertices");

            var visitHistory = new List<IEdge>();
            var visitedVertices = new HashSet<IVertex>();

            Queue<IVertex> queuedVertices = new Queue<IVertex>();
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

        /// <summary>
        /// Function to find path in visitHistory
        /// </summary>
        private IEnumerable<IEdge> GetPath(IVertex from, IVertex to, List<IEdge> visitHistory)
        {
            var results = new List<IEdge>();
            var previousVertex = to;

            do
            {
                var edge = visitHistory.Where(e => e.To.Equals(previousVertex)).FirstOrDefault();
                results.Add(edge);
                previousVertex = edge.From;

            } while (!previousVertex.Equals(from));
            
            return results.Reverse<IEdge>();
        }
        

        /// <summary>
        /// Projde (do šířky) všechny vrcholy od konkrétního bodu a postupně je vypíše.
        /// </summary>
        /// <param name="from">Vrchol, ze kterého se bude graf procházet</param>
        /// <returns></returns>
        public IEnumerable<IVertex> BreathFirstIterator(IVertex from)
        {
            var visitedVertices = new HashSet<IVertex>();
            
            Queue<IVertex> queuedVertices = new Queue<IVertex>();
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
