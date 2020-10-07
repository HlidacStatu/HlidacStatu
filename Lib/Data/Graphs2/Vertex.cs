using sun.security.x509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HlidacStatu.Lib.Data.Graphs2
{
    public class Vertex<T> : IEquatable<Vertex<T>>
    {
        public T Value { get; }
        public HashSet<Edge<T>> OutgoingEdges { get; } = new HashSet<Edge<T>>();

        public Vertex(T value)
        {
            Value = value;
        }

        public void AddOutgoingEdge(Edge<T> edge)
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
    }
}
