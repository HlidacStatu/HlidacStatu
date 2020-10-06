using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HlidacStatu.Lib.Data.Graphs2
{
    public class Vertex<T>
    {
        public T Value { get; }
        public List<Edge<T>> Edges { get; } = new List<Edge<T>>();

        public Vertex(T value)
        {
            Value = value;
        }

        public void AddEdge(Vertex<T> from, Vertex<T> to, string bindingName)
        {
            Edges.Add(new Edge<T>(from, to, bindingName));
        }

    }
}
