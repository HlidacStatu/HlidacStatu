using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Analysis
{
    //public class RatingDataPerYear : DataPerYear<RatingData>
    //{
    //    public static RatingDataPerYear Empty()
    //    {
    //        return new RatingDataPerYear(
    //            new Dictionary<int, RatingData>(), BasicDataPerYear.Empty()
    //        );
    //    }


    //    public static bool IsEmpty(RatingDataPerYear another)
    //    {
    //        if (another == null)
    //            return false;
    //        return another.Data.Count == 0;
    //    }

    //    protected override RatingData EmptyRec()
    //    {
    //        return RatingData.Empty();
    //    }

    //    public RatingDataPerYear() { }

    //    public RatingDataPerYear(IEnumerable<RatingDataPerYear> items, BasicDataPerYear basicData)
    //    {
    //        var validItems = items.Where(i => i != null);
    //        int firstYear = Analysis.BasicDataPerYear.UsualFirstYear;
    //        if (validItems.Count() > 0)
    //            firstYear = validItems.Min(m => m.FirstYear);
    //        int lastYear = DateTime.Now.Year;

    //        if (validItems.Count() > 0)
    //            lastYear = validItems.Max(m => m.LastYear);

    //        Dictionary<int, RatingData> rdata = new Dictionary<int, RatingData>();
    //        for (int i = firstYear; i <= lastYear; i++)
    //        {
    //            rdata.Add(i, new RatingData()
    //            {
    //                NumBezCeny = validItems.Sum(s => s.Data.ContainsKey(i) ? s.Data[i].NumBezCeny : 0),
    //                NumSPolitiky = validItems.Sum(s => s.Data.ContainsKey(i) ? s.Data[i].NumSPolitiky : 0),
    //                NumBezSmluvniStrany = validItems.Sum(s => s.Data.ContainsKey(i) ? s.Data[i].NumBezSmluvniStrany : 0),
    //                SumKcBezSmluvniStrany = validItems.Sum(s => s.Data.ContainsKey(i) ? s.Data[i].SumKcBezSmluvniStrany : 0),
    //                SumKcSPolitiky = validItems.Sum(s => s.Data.ContainsKey(i) ? s.Data[i].SumKcSPolitiky : 0)
    //            });
    //        }
    //        Init(rdata, basicData);

    //    }

    //    public RatingDataPerYear(Dictionary<int, RatingData> dataFromDb, BasicDataPerYear basicData)
    //    {
    //        Init(dataFromDb, basicData);
    //    }



    //    protected virtual void Init(Dictionary<int, RatingData> dataFromDb, BasicDataPerYear basicData)
    //    {
    //        if (dataFromDb == null)
    //            dataFromDb = new Dictionary<int, RatingData>();


    //        //add missing years
    //        for (int year = FirstYear; year <= LastYear; year++)
    //        {
    //            if (dataFromDb.ContainsKey(year))
    //                Data.Add(year, dataFromDb[year]);
    //            else
    //                Data.Add(year, RatingData.Empty());
    //        }

    //        //add summary
    //        Data.Add(DataPerYear.AllYearsSummaryKey, new RatingData()
    //        {
    //            NumBezCeny = Data.Sum(m => m.Value.NumBezCeny),
    //            NumBezSmluvniStrany = Data.Sum(m => m.Value.NumBezSmluvniStrany),
    //            NumSPolitiky = Data.Sum(m => m.Value.NumSPolitiky),
    //            SumKcSPolitiky = Data.Sum(m => m.Value.SumKcSPolitiky),
    //            SumKcBezSmluvniStrany = Data.Sum(m => m.Value.SumKcBezSmluvniStrany)
    //        });
    //        Data[DataPerYear.AllYearsSummaryKey].PercentBezCeny =
    //            basicData.Summary.Pocet == 0 ? 0 : (decimal)Data[DataPerYear.AllYearsSummaryKey].NumBezCeny / basicData.Summary.Pocet;
    //        Data[DataPerYear.AllYearsSummaryKey].PercentBezSmluvniStrany =
    //            basicData.Summary.Pocet == 0 ? 0 : (decimal)Data[DataPerYear.AllYearsSummaryKey].NumBezSmluvniStrany / basicData.Summary.Pocet;
    //        Data[DataPerYear.AllYearsSummaryKey].PercentKcBezSmluvniStrany =
    //            basicData.Summary.CelkemCena == 0 ? 0 : (decimal)Data[DataPerYear.AllYearsSummaryKey].SumKcBezSmluvniStrany / basicData.Summary.CelkemCena;
    //        Data[DataPerYear.AllYearsSummaryKey].PercentSPolitiky =
    //            basicData.Summary.Pocet == 0 ? 0 : (decimal)Data[DataPerYear.AllYearsSummaryKey].NumSPolitiky / basicData.Summary.Pocet;
    //        Data[DataPerYear.AllYearsSummaryKey].PercentKcSPolitiky =
    //            basicData.Summary.CelkemCena == 0 ? 0 : ((decimal)(Data[DataPerYear.AllYearsSummaryKey].SumKcSPolitiky) / (decimal)basicData.Summary.CelkemCena);

    //        //calculate percents
    //        for (int year = FirstYear; year <= LastYear; year++)
    //        {
    //            Data[year].PercentBezCeny =
    //                basicData[year].Pocet == 0 ? 0 : (decimal)Data[year].NumBezCeny / basicData[year].Pocet;
    //            Data[year].PercentBezSmluvniStrany =
    //                basicData[year].Pocet == 0 ? 0 : (decimal)Data[year].NumBezSmluvniStrany / basicData[year].Pocet;
    //            Data[year].PercentKcBezSmluvniStrany =
    //                basicData[year].CelkemCena == 0 ? 0 : (decimal)Data[year].SumKcBezSmluvniStrany / basicData[year].CelkemCena;
    //            Data[year].PercentSPolitiky =
    //                basicData[year].Pocet == 0 ? 0 : (decimal)Data[year].NumSPolitiky / basicData[year].Pocet;
    //            Data[year].PercentKcSPolitiky =
    //                basicData[year].CelkemCena == 0 ? 0 : ((decimal)(Data[year].SumKcSPolitiky) / (decimal)basicData[year].CelkemCena);

    //        }
    //    }
    //}

}
