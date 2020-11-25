namespace HlidacStatu.Lib.Analysis
{

    public class QueryStatistic : ComplexStatistic<string>
    {

        public QueryStatistic() { }

        public QueryStatistic(string simpleQuery)
            : base(simpleQuery)
        {
        }

        protected override BasicDataPerYear getBasicStat() => ACore.GetBasicStatisticForSimpleQuery(this.Item);
        protected override RatingDataPerYear getRating() => ACore.GetRatingForSimpleQuery(this.Item);

        public override string ToNiceString(bool html = true, string delimiter = " - ", params string[] otherparams)
        {
            throw new System.NotImplementedException();
        }

    }
}
