using System;
using System.Collections.Generic;

namespace HlidacStatu.Lib.Data.Graphs2
{
    public interface IVertex
    {
        HashSet<IEdge> OutgoingEdges { get; }

        void AddOutgoingEdge(IEdge edge);
    }

    public class Vertex<T> : IEquatable<Vertex<T>>, IVertex where T : IEquatable<T>
    {
        public T Value { get; }
        public HashSet<IEdge> OutgoingEdges { get; } = new HashSet<IEdge>();

        public Vertex(T value)
        {
            Value = value;
        }

        public void AddOutgoingEdge(IEdge edge)
        {
            OutgoingEdges.Add(edge);
        }

        public bool Equals(Vertex<T> other)
        {
            return this.GetHashCode() == other.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
