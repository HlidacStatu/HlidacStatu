using Devmasters;
using Devmasters.Enums;

using HlidacStatu.Lib.Data;
using HlidacStatu.Lib.Render;
using System;
using System.Collections.Generic;

namespace HlidacStatu.Lib.Analysis
{
    public static class ReportUtil
    {

        public static List<ReportDataSource<KeyValuePair<string, Lib.Analytics.StatisticsPerYear<Data.Smlouva.Statistics.Data>> >.Column>
            ComplexStatisticDefaultReportColumns<T>(HlidacStatu.Util.RenderData.MaxScale scale,
            int? minDateYear = null, int? maxDateYear = null, string query = null)
        {
            minDateYear = minDateYear ?? HlidacStatu.Lib.Analysis.BasicDataPerYear.UsualFirstYear;
            maxDateYear = maxDateYear ?? DateTime.Now.Year;

            var coreColumns = new List<ReportDataSource<KeyValuePair<string, Lib.Analytics.StatisticsPerYear<Data.Smlouva.Statistics.Data>>>.Column>();

            coreColumns.Add(
                new ReportDataSource<KeyValuePair<string, Lib.Analytics.StatisticsPerYear<Data.Smlouva.Statistics.Data>>>.Column()
                {
                    Id = "Title",
                    Name = "Plátci",
                    HtmlRender = (s) =>
                    {
                        var f = Firmy.Get(s.Key);
                        string html = string.Format("<a href='{0}'>{1}</a>", f.GetUrl(false), f.Jmeno);
                        if (!string.IsNullOrEmpty(query))
                        {
                            html += $" /<span class='small'>ukázat&nbsp;<a href='/hledat?q={System.Net.WebUtility.UrlEncode(Searching.Tools.ModifyQueryAND("ico:" + f.ICO, query))}'>smlouvy</a></span>/";
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
                    new ReportDataSource<KeyValuePair<string, Lib.Analytics.StatisticsPerYear<Data.Smlouva.Statistics.Data>>>.Column()
                    {
                        Id = "Cena_Y_" + year,
                        Name = $"Smlouvy {year} v {scale.ToNiceDisplayName()} Kč",
                        HtmlRender = (s) => HlidacStatu.Util.RenderData.ShortNicePrice(s.Value[year].CelkovaHodnotaSmluv, mena: "", html: true, showDecimal: HlidacStatu.Util.RenderData.ShowDecimalVal.Show, exactScale: scale, hideSuffix: true),
                        OrderValueRender = (s) => HlidacStatu.Util.RenderData.OrderValueFormat(s.Value[year].CelkovaHodnotaSmluv),
                        TextRender = (s) => HlidacStatu.Util.RenderData.ShortNicePrice(s.Value[year].CelkovaHodnotaSmluv, mena: "", html: false, showDecimal: HlidacStatu.Util.RenderData.ShowDecimalVal.Show, exactScale: scale, hideSuffix: true),
                        ValueRender = (s) => s.Value[year].CelkovaHodnotaSmluv,
                        CssClass = "number"
                    }
                    );
                coreColumns.Add(
                    new ReportDataSource<KeyValuePair<string, Lib.Analytics.StatisticsPerYear<Data.Smlouva.Statistics.Data>>>.Column()
                    {
                        Id = "Pocet_Y_" + year,
                        Name = $"Počet smluv v {year} ",
                        HtmlRender = (s) => HlidacStatu.Util.RenderData.NiceNumber(s.Value[year].PocetSmluv, html: true),
                        OrderValueRender = (s) => HlidacStatu.Util.RenderData.OrderValueFormat(s.Value[year].PocetSmluv),
                        TextRender = (s) => HlidacStatu.Util.RenderData.NiceNumber(s.Value[year].PocetSmluv, html: false),
                        ValueRender = (s) => s.Value[year].PocetSmluv,
                        CssClass = "number"
                    }
                    );
                coreColumns.Add(
                        new ReportDataSource<KeyValuePair<string, Lib.Analytics.StatisticsPerYear<Data.Smlouva.Statistics.Data>>>.Column()
                        {
                            Id = "PercentBezCeny_Y_" + year,
                            Name = $"Smluv bez ceny za {year} v %",
                            HtmlRender = (s) => s.Value[year].PercentSmluvBezCeny.ToString("P2"),
                            OrderValueRender = (s) => HlidacStatu.Util.RenderData.OrderValueFormat(s.Value[year].PercentSmluvBezCeny),
                            ValueRender = (s) => (s.Value[year].PercentSmluvBezCeny * 100).ToString(HlidacStatu.Util.Consts.enCulture),
                            TextRender = (s) => s.Value[year].PercentSmluvBezCeny.ToString("P2"),
                            CssClass = "number"
                        });
                coreColumns.Add(
                        new ReportDataSource<KeyValuePair<string, Lib.Analytics.StatisticsPerYear<Data.Smlouva.Statistics.Data>>>.Column()
                        {
                            Id = "PercentSPolitiky_Y_" + year,
                            Name = $"% smluv s politiky v {year} ",
                            HtmlRender = (s) => s.Value[year].PercentSmluvPolitiky.ToString("P2"),
                            OrderValueRender = (s) => HlidacStatu.Util.RenderData.OrderValueFormat(s.Value[year].PercentSmluvPolitiky),
                            ValueRender = (s) => (s.Value[year].PercentSmluvPolitiky * 100).ToString(HlidacStatu.Util.Consts.enCulture),
                            TextRender = (s) => s.Value[year].PercentSmluvPolitiky.ToString("P2"),
                            CssClass = "number"
                        });
                coreColumns.Add(
                        new ReportDataSource<KeyValuePair<string, Lib.Analytics.StatisticsPerYear<Data.Smlouva.Statistics.Data>>>.Column()
                        {
                            Id = "SumKcSPolitiky_Y_" + year,
                            Name = $"Hodnota smluv s politiky za {year}",
                            HtmlRender = (s) => s.Value[year].SumKcSmluvPolitiky.ToString("P2"),
                            OrderValueRender = (s) => HlidacStatu.Util.RenderData.OrderValueFormat(s.Value[year].SumKcSmluvPolitiky),
                            ValueRender = (s) => (s.Value[year].SumKcSmluvPolitiky * 100).ToString(HlidacStatu.Util.Consts.enCulture),
                            TextRender = (s) => s.Value[year].SumKcSmluvPolitiky.ToString("P2"),
                            CssClass = "number"
                        });

                if (year > minDateYear)
                {
                    coreColumns.Add(
                            new ReportDataSource<KeyValuePair<string, Lib.Analytics.StatisticsPerYear<Data.Smlouva.Statistics.Data>>>.Column()
                            {
                                Id = "CenaChangePercent_Y_" + year,
                                Name = $"Změna hodnoty smlouvy {year - 1}-{year}",
                                HtmlRender = (s) => Util.RenderData.ChangeValueSymbol(s.Value.ChangeBetweenYears(year,m=>m.CelkovaHodnotaSmluv).percentage,true),
                                OrderValueRender = (s) => HlidacStatu.Util.RenderData.OrderValueFormat(s.Value.ChangeBetweenYears(year, m => m.CelkovaHodnotaSmluv).percentage),
                                ValueRender = (s) => (s.Value.ChangeBetweenYears(year, m => m.CelkovaHodnotaSmluv).percentage * 100).ToString(HlidacStatu.Util.Consts.enCulture),
                                TextRender = (s) => (s.Value.ChangeBetweenYears(year, m => m.CelkovaHodnotaSmluv).percentage).ToString("P2"),
                                CssClass = "number"
                            }
                        );
                }
            };
            coreColumns.Add(
                new ReportDataSource<KeyValuePair<string, Lib.Analytics.StatisticsPerYear<Data.Smlouva.Statistics.Data> >>.Column()
                {
                    Id = "CenaCelkem",
                    Name = $"Smlouvy 2016-{DateTime.Now.Year} v {scale.ToNiceDisplayName()} Kč",
                    HtmlRender = (s) => HlidacStatu.Util.RenderData.ShortNicePrice(s.Value.Sum(m=>m.CelkovaHodnotaSmluv), mena: "", html: true, showDecimal: HlidacStatu.Util.RenderData.ShowDecimalVal.Show, exactScale: scale, hideSuffix: true),
                    OrderValueRender = (s) => HlidacStatu.Util.RenderData.OrderValueFormat(s.Value.Sum(m => m.CelkovaHodnotaSmluv)),
                    CssClass = "number"
                });

            return coreColumns;
        }
    }
}
