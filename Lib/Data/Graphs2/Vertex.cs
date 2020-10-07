using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HlidacStatu.Lib.Data.Graphs2
{
    public class Vertex<T> : IEquatable<Vertex<T>>
    {
        public T Value { get; }
        public List<Edge<T>> OutgoingEdges { get; } = new List<Edge<T>>();

        public Vertex(T value)
        {
            Value = value;
        }

        public void AddOutgoingEdge(Vertex<T> from, Vertex<T> to, string bindingName)
        {
            OutgoingEdges.Add(new Edge<T>(from, to, bindingName));
        }

        public bool Equals(Vertex<T> other)
        {
            return this.Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            return this.Value.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }
    }
}
