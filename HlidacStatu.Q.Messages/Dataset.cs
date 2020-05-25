namespace HlidacStatu.Q.Messages
{
    public class DataSet : BaseTask
    {
        public string DatasetId { get; set; }
        public string ItemId { get; set; }
        public bool Force { get; set; }
    }
}
