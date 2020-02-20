namespace HlidacStatu.Lib.Data.Dotace
{
    public class Zdroj
    {
        [Nest.Text]
        public string Nazev { get; set; }
        [Nest.Keyword]
        public string Url { get; set; }
        [Nest.Boolean]
        public bool IsPrimary { get; set; }
    }
}
