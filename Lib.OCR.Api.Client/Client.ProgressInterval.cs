namespace HlidacStatu.Lib.OCR.Api
{
    public partial class Client
    {
        public class ProgressInterval
        {
            decimal _from = 0;
            decimal _to = 1;
            public decimal Progress { get; private set; }
            public decimal ProgressInPercent { get { return Progress * 100; } }

            public ProgressInterval()
            { }
            public ProgressInterval(int splitByNumOfItems)
            {
                if (splitByNumOfItems == 0)
                    throw new System.ArgumentOutOfRangeException("splitByNumOfItems", "cannot by zero");
                this._from = 0;
                this._to = 1m / (decimal)splitByNumOfItems;
            }
            public ProgressInterval(decimal from, decimal to)
            {
                this._from = from;
                this._to = to;
            }

            public ProgressInterval SetProgressInPercent(int percent)
            {
                decimal b = (_to - _from) * ((decimal)percent / 100m);
                this.Progress = b;
                return this;
            }
        }



    }
}
