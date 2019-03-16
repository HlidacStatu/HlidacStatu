using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib
{
    /// <summary>
    ///     Represents a range of dates.
    /// </summary>
    public class DateTimeRange
    {

        private DateTimeRange() { }
        /// <summary>
        ///     Initializes a new instance of the <see cref="DateTimeRange" /> structure to the specified start and end date.
        /// </summary>
        /// <param name="startDate">A string that contains that first date in the date range.</param>
        /// <param name="endDate">A string that contains the last date in the date range.</param>
        /// <exception cref="System.ArgumentNullException">
        ///		endDate or startDate are <c>null</c>.
        /// </exception>
        /// <exception cref="System.FormatException">
        ///     endDate or startDate do not contain a vaild string representation of a date and time.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///		endDate is not greater than or equal to startDate
        /// </exception>
        public DateTimeRange(DateTime startDate, DateTime endDate) : this()
        {

            if (End < Start)
            {
                this.End = startDate;
                this.Start = endDate;
            }
            else
            {
                this.Start = startDate;
                this.End = endDate;
            }
        }

        /// <summary>
        ///     Gets the start date component of the date range.
        /// </summary>
        public DateTime Start { get; private set; }


        /// <summary>
        ///     Gets the end date component of the date range.
        /// </summary>
        public DateTime End { get; private set; }

        /// <summary>
        ///     Gets a collection of the dates in the date range.
        /// </summary>
        public IList<DateTime> Dates
        {
            get
            {
                var startDate = Start;

                return Enumerable.Range(0, Days)
                    .Select(offset => startDate.AddDays(offset))
                    .ToList();
            }
        }

        /// <summary>
        ///     Gets the number of whole days in the date range.
        /// </summary>
        public int Days
        {
            get { return (End - Start).Days + 1; }
        }

        public TimeSpan Length
        {
            get { return End - Start; }
        }
    }
}

