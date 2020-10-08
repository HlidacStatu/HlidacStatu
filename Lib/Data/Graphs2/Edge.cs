using System;

namespace HlidacStatu.Lib.Data.Graphs2
{
    public interface IEdge<T>
    {
        Vertex<T> From { get; }
        Vertex<T> To { get; }
    }

    public class Edge<T> : IEdge<T>, IEquatable<Edge<T>>
    {
        public string BindingName { get; }
        public Vertex<T> From { get; }
        public Vertex<T> To { get; }

        public Edge(Vertex<T> from, Vertex<T> to, string bindingName)
        {
            From = from ?? throw new System.ArgumentNullException(nameof(from));
            To = to ?? throw new System.ArgumentNullException(nameof(to));
            BindingName = bindingName;
        }

        public bool Equals(Edge<T> other)
        {
            return this.GetHashCode() == other.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + this.From.GetHashCode();
            hash = hash * 23 + this.To.GetHashCode();
            return hash;
        }
    }
}
