namespace HlidacStatu.Lib.Data.Dotace
{
    public class DotacniProgram
    {
        [Nest.Keyword]
        public string Id { get; set; }
        [Nest.Text]
        public string Nazev { get; set; }
        [Nest.Keyword]
        public string Kod { get; set; }
        [Nest.Keyword]
        public string Url { get; set; }
    }
}
