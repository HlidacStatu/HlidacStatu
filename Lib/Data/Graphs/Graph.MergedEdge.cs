using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace HlidacStatu.Lib.Data
{
    public partial class Graph
    {
        public class MergedEdge
        {
            public MergedEdge() { }
            public MergedEdge(Edge e)
            {
                this.Aktualnost = e.Aktualnost;
                this.Distance = e.Distance;
                this.From = e.From;
                this.To = e.To;
                this.Intervals.Add(new Interval()
                {
                    Descr = e.Descr,
                    RelFrom = e.RelFrom,
                    RelTo = e.RelTo
                });

            }

            public class Interval
            {
                public DateTime? RelFrom { get; set; }
                public DateTime? RelTo { get; set; }
                public string Descr { get; set; }
                public string Doba(string format = null, string betweenDatesDelimiter = null)
                {
                    if (string.IsNullOrEmpty(format))
                        format = "({0})";
                    if (string.IsNullOrEmpty(betweenDatesDelimiter))
                        betweenDatesDelimiter = " - ";
                    string datumy = string.Empty;

                    if (this.RelFrom.HasValue && this.RelTo.HasValue)
                    {
                        datumy = string.Format("{0}{2}{1}", this.RelFrom.Value.ToShortDateString(), this.RelTo.Value.ToShortDateString(), betweenDatesDelimiter);
                    }
                    else if (this.RelTo.HasValue)
                    {
                        datumy = string.Format("do {0}", this.RelTo.Value.ToShortDateString());
                    }
                    else if (this.RelFrom.HasValue)
                    {
                        datumy = string.Format("od {0}", this.RelFrom.Value.ToShortDateString());
                    }
                    if (string.IsNullOrEmpty(datumy))
                        return string.Empty;

                    return string.Format(format, datumy);
                }

            }

            public Node From { get; set; }
            public Node To { get; set; }

            public List<Interval> Intervals { get; private set; } = new List<Interval>();

            public int Distance { get; set; }
            public Relation.AktualnostType Aktualnost { get; set; }

            public string Doba(string dateIntervalDelimiter = ",", string format = null, string betweenDatesDelimiter = null)
            {
                return string.Join(dateIntervalDelimiter, this.Intervals.Select(m => m.Doba(format, betweenDatesDelimiter)));
            }

            public MergedEdge MergeWith(Edge e)
            {
                if (e == null)
                    return this;

                if (this.To?.UniqId != e.To?.UniqId)
                    throw new ArgumentException("To and From properties should be same with merged Edge");
                if (this.From?.UniqId != e.From?.UniqId)
                    throw new ArgumentException("To and From properties should be same with merged Edge");

                List<Interval> newInterval = new List<Interval>();
                bool mergedInterval = false;
                foreach (var interv in this.Intervals)
                {
                    if (mergedInterval == false
                        && Devmasters.DT.Util.IsOverlappingIntervals(interv.RelFrom, interv.RelTo, e.RelFrom, e.RelTo)
                        )
                    {
                        Interval newI = new Interval
                        {
                            RelFrom = Devmasters.DT.Util.LowerDate(interv.RelFrom, e.RelFrom),
                            RelTo = Devmasters.DT.Util.HigherDate(interv.RelTo, e.RelTo),
                            Descr = interv.Descr + ", " + e.RelFrom
                        };
                        newInterval.Add(newI);
                        mergedInterval = true;
                    }
                    else
                        newInterval.Add(interv);
                }
                if (mergedInterval == false)
                    newInterval.Add(
                        new Interval() { 
                          Descr = e.Descr,
                          RelFrom = e.RelFrom,
                          RelTo = e.RelTo
                        }
                        );
                this.Intervals = newInterval;
                return this;
            }

        }

    }

}
