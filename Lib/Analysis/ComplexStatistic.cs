namespace HlidacStatu.Lib.Analysis
{
    /*
    public class SubjectStatisticWithDetail<T>
            where T : new()
    {
        public SubjectStatisticWithDetail(string ico)
        {
            Statistic = new SubjectStatistic(ico);
        }
        public SubjectStatisticWithDetail(Lib.Data.Firma subject)
        {
            Statistic = new SubjectStatistic(subject.ICO);
        }

        public SubjectStatistic Statistic { get; set; }
        public T Detail { get; set; } = new T();

    }
*/
    public abstract class ComplexStatistic<T>
    {

        public BasicDataPerYear BasicStatPerYear { get; set; } = BasicDataPerYear.Empty();
        public RatingDataPerYear RatingPerYear { get; set; } = RatingDataPerYear.Empty();

        public T Item { get; set; } = default(T);

        public ComplexStatistic() { }

        public ComplexStatistic(T item)
        {
            this.Item = item;
            InitData();
        }

        protected virtual void InitData()
        {
            this.BasicStatPerYear = getBasicStat();
            this.RatingPerYear = getRating();

        }
        protected abstract BasicDataPerYear getBasicStat();
        protected abstract RatingDataPerYear getRating();

        public abstract string ToNiceString(bool html = true, string delimiter = " - ", params string[] otherparams);

        protected virtual string ToNiceString(Data.Bookmark.IBookmarkable item, bool html = true, string delimiter = " - ", params string[] otherparams)
        {
            var c = "";
            if (html)
                c = string.Format("<a href='{0}'>{1}</a>{2}",
                    item.GetUrl(false), item.BookmarkName(), delimiter);
            else
                c = string.Format("{0}{1}", item.BookmarkName(), delimiter);
            return c + BasicStatPerYear.Summary.ToNiceString(item, html);
        }

        public virtual BasicData<T> ToBasicData()
        {
            var bd = new BasicData<T>(this.Item)
            {
                Pocet = this.BasicStatPerYear.Summary.Pocet,
                CelkemCena = this.BasicStatPerYear.Summary.CelkemCena
            };
            return bd;
        }
    }
}
