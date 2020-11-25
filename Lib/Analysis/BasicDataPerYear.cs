using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Analysis
{
    public class BasicDataPerYear : DataPerYear<BasicData>
    {

        public static BasicDataPerYear Empty()
        {
            return new BasicDataPerYear(new Dictionary<int, BasicData>());
        }

        protected override BasicData EmptyRec()
        {
            return BasicData.Empty();
        }


        public void Add(BasicDataPerYear oth)
        {
            //Dictionary<int, BasicData> data = new Dictionary<int, BasicData>();
            foreach (var kv in oth.Data)
            {
                if (this.Data.Keys.Contains(kv.Key))
                    this.Data[kv.Key].Add(kv.Value.Pocet, kv.Value.CelkemCena);
                else
                    this.Data.Add(kv.Key, kv.Value);
            }

            //add summary
            this.Data.Remove(DataPerYear.AllYearsSummaryKey);
            Data.Add(DataPerYear.AllYearsSummaryKey, new BasicData()
            {
                Pocet = Data.Sum(m => m.Value.Pocet),
                CelkemCena = Data.Sum(m => m.Value.CelkemCena)
            });

        }
        public BasicDataPerYear()
        {
        }

        public BasicDataPerYear(IEnumerable<BasicDataPerYear> items)
        {
            int firstYear = Analysis.BasicDataPerYear.UsualFirstYear;
            if (items.Count() > 0)
                firstYear = items.Min(m => m.FirstYear);
            int lastYear = DateTime.Now.Year;

            if (items.Count() > 0)
                lastYear = items.Max(m => m.LastYear);
            Dictionary<int, BasicData> bdata = new Dictionary<int, BasicData>();
            for (int y = firstYear; y <= lastYear; y++)
            {

                bdata.Add(y, new BasicData()
                {
                    Pocet = items.Sum(s => s.Data.ContainsKey(y) ? s.Data[y].Pocet : 0),
                    CelkemCena = items.Sum(s => s.Data.ContainsKey(y) ? s.Data[y].CelkemCena : 0)
                });

            }
            Init(bdata);
        }

        public BasicDataPerYear(Dictionary<int, BasicData> dataFromDb)
        {
            Init(dataFromDb);
        }

        protected virtual void Init(Dictionary<int, BasicData> dataFromDb)
        {
            if (dataFromDb == null)
                dataFromDb = new Dictionary<int, BasicData>();

            foreach (var i in dataFromDb)
            {
                if (i.Key != AllYearsSummaryKey && (i.Key < FirstYear || i.Key > LastYear))
                {
                    Data.Add(i.Key, dataFromDb[i.Key]);
                }
            }
            //add missing years
            for (int year = FirstYear; year <= LastYear; year++)
            {
                if (dataFromDb.ContainsKey(year))
                    Data.Add(year, dataFromDb[year]);
                else
                    Data.Add(year, BasicData.Empty());
            }

            //add summary
            Data.Add(DataPerYear.AllYearsSummaryKey, new BasicData()
            {
                Pocet = Data.Sum(m => m.Value.Pocet),
                CelkemCena = Data.Sum(m => m.Value.CelkemCena)
            });
        }


        public BasicDataChange YearChange(int currYear)
        {
            var curr = this.Data.ContainsKey(currYear) ? this.Data[currYear] : BasicData.Empty();
            var prev = this.Data.ContainsKey(currYear - 1) ? this.Data[currYear - 1] : BasicData.Empty();
            return new BasicDataChange(prev, curr);
        }

        BasicData _summaryBefore2016 = null;
        public BasicData SummaryBefore2016()
        {
            if (_summaryBefore2016 == null)
            {
                _summaryBefore2016 = new BasicData()
                {
                    Pocet = this.Data.Where(k => k.Key > 0 && k.Key < 2016).Sum(m => m.Value.Pocet),
                    CelkemCena = this.Data.Where(k => k.Key > 0 && k.Key < 2016).Sum(m => m.Value.CelkemCena)
                };
                ;
            }
            return _summaryBefore2016;
        }

        BasicData _summaryAfter2016 = null;
        public BasicData SummaryAfter2016()
        {
            if (_summaryAfter2016 == null)
            {
                int currY = DateTime.Now.Year;
                _summaryAfter2016 = new BasicData()
                {
                    Pocet = this.Data.Where(k => k.Key >= 2016 && k.Key <= currY).Sum(m => m.Value.Pocet),
                    CelkemCena = this.Data.Where(k => k.Key >= 2016 && k.Key <= currY).Sum(m => m.Value.CelkemCena)
                };

            }
            return _summaryAfter2016;
        }


    }

}
