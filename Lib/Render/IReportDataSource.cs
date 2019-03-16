using System.Collections.Generic;

namespace HlidacSmluv.Lib.Render
{
    public interface IReportDataSource<T> where T : class
    {
        List<IColumn<T>> Columns { get; }
        List<ReportDataSource<T>.DataValue[]> Data { get; }
        string Title { get; set; }
    }

    public interface IColumn<T> where T : class
    {
        string Name { get; set; }
        string CssClass { get; set; }
        System.Func<T, string> HtmlRender { get; set; }
        System.Func<T, string> TextRender { get; set; }
        System.Func<T, object> ValueRender { get; set; }
        System.Func<T, string> OrderValueRender { get; set; }
    }

}