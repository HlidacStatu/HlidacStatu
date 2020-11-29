using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Analytics
{
    public abstract class CoreStat
    {
        /// <summary>
        /// Tenhle měsíc určuje, za který rok se AKTUÁLNÍ data mají zobrazovat.
        /// Pokud chceme pro nějakou datovou sadu nastavit sezónu jinak, je potřeba ho změnit
        /// </summary>
        public abstract int NewSeasonStartMonth();
        public abstract int UsualFirstYear();

        public enum UsualYearsInterval
        {
            All = 0,
            FromUsualFirstYearUntilNow = 1,
            FromUsualFirstYearUntilSeassonYear = 2
        }
        public int[] YearsFromUsual(UsualYearsInterval inteval)
        {
            switch (inteval)
            {
                case UsualYearsInterval.FromUsualFirstYearUntilNow:
                    return Enumerable.Range(UsualFirstYear(), DateTime.Now.Year - UsualFirstYear() + 1).ToArray();

                case UsualYearsInterval.FromUsualFirstYearUntilSeassonYear:
                    int untilY = DateTime.Now.Month >= NewSeasonStartMonth() ?
                            DateTime.Now.Year : DateTime.Now.Year - 1;

                    return Enumerable.Range(UsualFirstYear(), untilY - UsualFirstYear() + 1).ToArray();
                case UsualYearsInterval.All:
                default:
                    return null;
            }

        }
    }
}
