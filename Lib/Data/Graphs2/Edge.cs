namespace HlidacStatu.Lib.Data.Graphs2
{
    public interface IEdge<T>
    {
        Vertex<T> From { get; }
        Vertex<T> To { get; }
    }

    public class Edge<T> : IEdge<T>
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
    }
}
