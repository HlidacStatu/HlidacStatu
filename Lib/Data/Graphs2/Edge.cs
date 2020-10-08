using System;

namespace HlidacStatu.Lib.Data.Graphs2
{
    public interface IEdge
    {
        IVertex From { get; }
        IVertex To { get; }
    }

    public class Edge<T> : IEdge, IEquatable<Edge<T>>
    {
        public T BindingPayload { get; }
        public IVertex From { get; }
        public IVertex To { get; }

        public Edge(IVertex from, IVertex to, T bindingPayload)
        {
            From = from ?? throw new ArgumentNullException(nameof(from));
            To = to ?? throw new ArgumentNullException(nameof(to));
            BindingPayload = bindingPayload;
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
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + this.From.GetHashCode();
                hash = hash * 23 + this.To.GetHashCode();
                return hash;
            }
        }

    }
}
