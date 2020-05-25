using Devmasters.Core;
using HlidacStatu.Lib.Data;
using HlidacStatu.Lib.Render;
using System;
using System.Collections.Generic;

namespace HlidacStatu.Lib.Analysis
{
    public static class ReportUtil
    {

        public static List<ReportDataSource<KeyValuePair<string, HlidacStatu.Lib.Analysis.ComplexStatistic<T>>>.Column>
            ComplexStatisticDefaultReportColumns<T>(HlidacStatu.Util.RenderData.MaxScale scale, 
            int? minDateYear = null, int? maxDateYear = null, string query =null)
        {
            minDateYear = minDateYear ?? HlidacStatu.Lib.Analysis.BasicDataPerYear.UsualFirstYear;
            maxDateYear = maxDateYear ?? DateTime.Now.Year;

            var coreColumns = new List<ReportDataSource<KeyValuePair<string, HlidacStatu.Lib.Analysis.ComplexStatistic<T>>>.Column>();

            coreColumns.Add(
                new ReportDataSource<KeyValuePair<string, HlidacStatu.Lib.Analysis.ComplexStatistic<T>>>.Column()
                {
                    Id = "Title",
                    Name = "Plátci",
                    HtmlRender = (s) =>
                    {
                        var f = Firmy.Get(s.Key);
                        string html = string.Format("<a href='{0}'>{1}</a>", f.GetUrl(false), f.Jmeno);
                        if (!string.IsNullOrEmpty(query))
                        {
                            html += $" /<span class='small'>ukázat&nbsp;<a href='/hledat?q={System.Net.WebUtility.UrlEncode(Searching.Tools.ModifyQueryAND("ico:"+f.ICO ,query))}'>smlouvy</a></span>/";
                        }
                        return html;
                    },
                    OrderValueRender = (s) => HlidacStatu.Lib.Data.Firmy.GetJmeno(s.Key),
                    ValueRender = (s) => HlidacStatu.Lib.Data.Firmy.GetJmeno(s.Key),
                    TextRender = (s) => HlidacStatu.Lib.Data.Firmy.GetJmeno(s.Key)
                });

            for (int y = minDateYear.Value; y <= maxDateYear.Value; y++)
            {
                int year = y;
                coreColumns.Add(
                    new ReportDataSource<KeyValuePair<string, HlidacStatu.Lib.Analysis.ComplexStatistic<T>>>.Column()
                    {
                        Id = "Cena_Y_" + year,
                        Name = $"Smlouvy {year} v {scale.ToNiceDisplayName()} Kč",
                        HtmlRender = (s) => HlidacStatu.Util.RenderData.ShortNicePrice(s.Value.BasicStatPerYear[year].CelkemCena, mena: "", html: true, showDecimal: HlidacStatu.Util.RenderData.ShowDecimalVal.Show, exactScale: scale, hideSuffix: true),
                        OrderValueRender = (s) => HlidacStatu.Util.RenderData.OrderValueFormat(s.Value.BasicStatPerYear[year].CelkemCena),
                        TextRender = (s) => HlidacStatu.Util.RenderData.ShortNicePrice(s.Value.BasicStatPerYear[year].CelkemCena, mena: "", html: false, showDecimal: HlidacStatu.Util.RenderData.ShowDecimalVal.Show, exactScale: scale, hideSuffix: true),
                        ValueRender = (s) => s.Value.BasicStatPerYear[year].CelkemCena,
                        CssClass = "number"
                    }
                    );
                coreColumns.Add(
                    new ReportDataSource<KeyValuePair<string, HlidacStatu.Lib.Analysis.ComplexStatistic<T>>>.Column()
                    {
                        Id = "Pocet_Y_" + year,
                        Name = $"Počet smluv v {year} ",
                        HtmlRender = (s) => HlidacStatu.Util.RenderData.NiceNumber(s.Value.BasicStatPerYear[year].Pocet, html: true),
                        OrderValueRender = (s) => HlidacStatu.Util.RenderData.OrderValueFormat(s.Value.BasicStatPerYear[year].Pocet),
                        TextRender = (s) => HlidacStatu.Util.RenderData.NiceNumber(s.Value.BasicStatPerYear[year].Pocet, html: false),
                        ValueRender = (s) => s.Value.BasicStatPerYear[year].Pocet,
                        CssClass = "number"
                    }
                    );
                coreColumns.Add(
                        new ReportDataSource<KeyValuePair<string, HlidacStatu.Lib.Analysis.ComplexStatistic<T>>>.Column()
                        {
                            Id = "PercentBezCeny_Y_" + year,
                            Name = $"Smluv bez ceny za {year} v %",
                            HtmlRender = (s) => s.Value.RatingPerYear.Data[year].PercentBezCeny.ToString("P2"),
                            OrderValueRender = (s) => HlidacStatu.Util.RenderData.OrderValueFormat(s.Value.RatingPerYear.Data[year].PercentBezCeny),
                            ValueRender = (s) => (s.Value.RatingPerYear.Data[year].PercentBezCeny * 100).ToString(HlidacStatu.Util.Consts.enCulture),
                            TextRender = (s) => s.Value.RatingPerYear.Data[year].PercentBezCeny.ToString("P2"),
                            CssClass = "number"
                        });
                coreColumns.Add(
                        new ReportDataSource<KeyValuePair<string, HlidacStatu.Lib.Analysis.ComplexStatistic<T>>>.Column()
                        {
                            Id = "PercentSPolitiky_Y_" + year,
                            Name = $"Smluv bez ceny za {year} v %",
                            HtmlRender = (s) => s.Value.RatingPerYear.Data[year].PercentSPolitiky.ToString("P2"),
                            OrderValueRender = (s) => HlidacStatu.Util.RenderData.OrderValueFormat(s.Value.RatingPerYear.Data[year].PercentSPolitiky),
                            ValueRender = (s) => (s.Value.RatingPerYear.Data[year].PercentSPolitiky * 100).ToString(HlidacStatu.Util.Consts.enCulture),
                            TextRender = (s) => s.Value.RatingPerYear.Data[year].PercentSPolitiky.ToString("P2"),
                            CssClass = "number"
                        });
                coreColumns.Add(
                        new ReportDataSource<KeyValuePair<string, HlidacStatu.Lib.Analysis.ComplexStatistic<T>>>.Column()
                        {
                            Id = "SumKcSPolitiky_Y_" + year,
                            Name = $"Smluv bez ceny za {year} v %",
                            HtmlRender = (s) => s.Value.RatingPerYear.Data[year].SumKcSPolitiky.ToString("P2"),
                            OrderValueRender = (s) => HlidacStatu.Util.RenderData.OrderValueFormat(s.Value.RatingPerYear.Data[year].SumKcSPolitiky),
                            ValueRender = (s) => (s.Value.RatingPerYear.Data[year].SumKcSPolitiky * 100).ToString(HlidacStatu.Util.Consts.enCulture),
                            TextRender = (s) => s.Value.RatingPerYear.Data[year].SumKcSPolitiky.ToString("P2"),
                            CssClass = "number"
                        });

                if (year > minDateYear)
                {
                    coreColumns.Add(
                            new ReportDataSource<KeyValuePair<string, HlidacStatu.Lib.Analysis.ComplexStatistic<T>>>.Column()
                            {
                                Id = "CenaChangePercent_Y_" + year,
                                Name = $"Změna hodnoty smlouvy {year - 1}-{year}",
                                HtmlRender = (s) => s.Value.BasicStatPerYear.YearChange(year).CenaChangePercentToSymbolString(true),
                                OrderValueRender = (s) => HlidacStatu.Util.RenderData.OrderValueFormat(s.Value.BasicStatPerYear.YearChange(year).CenaChangePerc),
                                ValueRender = (s) => (s.Value.BasicStatPerYear.YearChange(year).CenaChangePerc * 100).ToString(HlidacStatu.Util.Consts.enCulture),
                                TextRender = (s) => (s.Value.BasicStatPerYear.YearChange(year).CenaChangePerc).ToString("P2"),
                                CssClass = "number"
                            }
                        );
                }
            };
            coreColumns.Add(
                new ReportDataSource<KeyValuePair<string, HlidacStatu.Lib.Analysis.ComplexStatistic<T>>>.Column()
                {
                    Id = "CenaCelkem",
                    Name = $"Smlouvy 2016-{DateTime.Now.Year} v {scale.ToNiceDisplayName()} Kč",
                    HtmlRender = (s) => HlidacStatu.Util.RenderData.ShortNicePrice(s.Value.BasicStatPerYear.Summary.CelkemCena, mena: "", html: true, showDecimal: HlidacStatu.Util.RenderData.ShowDecimalVal.Show, exactScale: scale, hideSuffix: true),
                    OrderValueRender = (s) => HlidacStatu.Util.RenderData.OrderValueFormat(s.Value.BasicStatPerYear.Summary.CelkemCena),
                    CssClass = "number"
                });

            return coreColumns;
        }
    }
}
