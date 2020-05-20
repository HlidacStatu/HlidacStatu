using Nest;

namespace HlidacStatu.Lib.Data.OsobyES
{
    [ElasticsearchType(IdProperty = nameof(NameId))]
    public class OsobaES
    {
        [Nest.Keyword]
        public string NameId { get; set; }
        [Nest.Text]
        public string ShortName { get; set; }
        [Nest.Text]
        public string FullName { get; set; }
        [Nest.Number]
        public int? BirthYear { get; set; }
        [Nest.Text]
        public string[] PoliticalFunctions { get; set; }
        [Nest.Text]
        public string PoliticalParty { get; set; }
        [Nest.Text]
        public string StatusText { get; set; }
        [Nest.Number]
        public int Status { get; set; }
        [Nest.Keyword]
        public string PhotoUrl { get; set; }
    }
}
