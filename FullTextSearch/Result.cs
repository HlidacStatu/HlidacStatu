namespace FullTextSearch
{
    public class Result<T>
    {
        public T Original { get; set; }
        public double Score { get; set; }
    }
}
