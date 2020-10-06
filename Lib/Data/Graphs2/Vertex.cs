using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HlidacStatu.Lib.Data.Graphs2
{
    public class Vertex<T>
    {
        public Vertex(T value, IEnumerable<Vertex<T>> neighbors = null)
        {
            Value = value;
            Neighbors = neighbors?.ToList() ?? new List<Vertex<T>>();
        }

        public T Value { get; }

        public List<Vertex<T>> Neighbors { get; }

        public void AddNeighbor(Vertex<T> vertex)
        {
            Neighbors.Add(vertex);
        }

        //public void AddNeighbors(IEnumerable<Vertex<T>> newNeighbors)
        //{
        //    Neighbors.AddRange(newNeighbors);
        //}

        //public void RemoveNeighbor(Vertex<T> vertex)
        //{
        //    Neighbors.Remove(vertex);
        //}

        //public override string ToString()
        //{
        //    return Neighbors.Aggregate(
        //            new StringBuilder($"{Value}: "),
        //            (sb, n) => sb.Append($"{n.Value} "))
        //        .ToString();
        //}

    }
}
