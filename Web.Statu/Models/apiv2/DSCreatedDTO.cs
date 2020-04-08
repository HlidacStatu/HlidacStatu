namespace HlidacStatu.Web.Models.Apiv2
{
    public class DSCreatedDTO
    {
        public DSCreatedDTO(string datasetId)
        {
            DatasetId = datasetId;
        }
        public string DatasetId { get; set; }
    }
}