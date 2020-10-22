using System;
using System.Linq;

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
        protected readonly int _newSeasonStartMonth = 7;
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

        /// <summary>
        /// Gets whole Statistics for a last season. Season is changing every {_newSeasonStartMonth}.
        /// Statistics is still calculated for one calendarYear.
        /// </summary>
        /// <returns></returns>
        public virtual ContractsStatisticData? LatestSeasonStatistics()
        {
            // hledáme nejvhodnější rok pro zobrazení
            // pokud takový rok neexistuje, vybereme jakýkoliv společný rok
            var today = DateTime.Now;
            int bestYearToDisplay = (today.Month < _newSeasonStartMonth) ? today.Year - 1 : today.Year;

            var commonYears = BasicStatPerYear.Data.Keys
                .Intersect(RatingPerYear.Data.Keys)
                .Where(year => year != DataPerYear.AllYearsSummaryKey);

            if (commonYears.Count() == 0)
                return null;

            // hledáme nejvyšší možný rok, který je menší než bestYearToDisplay. 
            // Pokud není nic menšího, tak vrátíme první nalezný rok (většinou ten nejvyšší)
            int displayYear = commonYears
                .Aggregate((aggregate, current) =>
                    (current <= bestYearToDisplay 
                    && (current > aggregate || aggregate > bestYearToDisplay)
                    ) ? current : aggregate
                );

            var basicData = BasicStatPerYear.Data[displayYear];
            var ratingData = RatingPerYear.Data[displayYear];

            // přidat nárůst objemu ceny
            int precedentYear = displayYear - 1;
            decimal? volumeDifference = null;
            if (commonYears.Contains(precedentYear))
            {
                volumeDifference = basicData.CelkemCena / BasicStatPerYear.Data[precedentYear].CelkemCena * 100;
            }

            return new ContractsStatisticData()
            {
                Rok = displayYear,
                Pocet = basicData.Pocet,
                CelkemCena = basicData.CelkemCena,
                CelkemKcBezSmluvniStrany = ratingData.SumKcBezSmluvniStrany,
                CelkemKcSPolitiky = ratingData.SumKcSPolitiky,
                PocetBezCeny = ratingData.NumBezCeny,
                PocetBezSmluvniStrany = ratingData.NumBezSmluvniStrany,
                PocetSPolitiky = ratingData.NumSPolitiky,
                ProcentBezCeny = ratingData.PercentBezCeny,
                ProcentBezSmluvniStrany = ratingData.PercentBezSmluvniStrany,
                ProcentKcBezSmluvniStrany = ratingData.PercentKcBezSmluvniStrany,
                ProcentKcSPolitiky = ratingData.PercentKcSPolitiky,
                ProcentSPolitiky = ratingData.PercentSPolitiky,
                ProcentObjemCenyOprotiPredchozimuObdobi = volumeDifference
            };

        }
    }
}
