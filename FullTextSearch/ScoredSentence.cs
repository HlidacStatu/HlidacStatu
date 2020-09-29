namespace FullTextSearch
{
    internal class ScoredSentence<T>
    {
        public Sentence<T> Sentence { get; set; }
        public double Score { get; set; }

        public ScoredSentence(Sentence<T> sentence, double score)
        {
            Sentence = sentence;
            Score = score;
        }
    }
}
