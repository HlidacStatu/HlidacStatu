using Nest;

namespace HlidacStatu.Lib.Data.OsobyES
{
    [ElasticsearchType(IdProperty = nameof(OsobaId))]
    public class OsobaES
    {
        [Nest.Keyword]
        public string OsobaId { get; set; }
        [Nest.Text]
        public string FullName { get; set; }
        [Nest.Number]
        public int? BirthYear { get; set; }
        [Nest.Text]
        public string[] PoliticalFunctions { get; set; }
        [Nest.Text]
        public string PoliticalParty { get; set; }
        [Nest.Text]
        public string Status { get; set; }
    }
}
