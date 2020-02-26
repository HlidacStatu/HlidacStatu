using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HlidacStatu.Lib.Watchdogs;

namespace HlidacStatu.Lib.Data
{
    public partial class Smlouva
    {
        public class WatchdogProcessor : Lib.Watchdogs.IWatchdogProcessor
        {
            public WatchDog OrigWD { get; private set; }
            public WatchdogProcessor(WatchDog wd)
            {
                this.OrigWD = wd;
            }

            public DateTime GetLatestRec(DateTime toDate)
            {
                var query = "zverejneno:" + string.Format("[* TO {0}]", Searching.Tools.ToElasticDate(toDate));
                var res = Lib.Data.Smlouva.Search.SimpleSearch(query, 0, 1, Smlouva.Search.OrderResult.DateAddedDesc);

                if (res.IsValid == false)
                    return DateTime.Now.Date.AddYears(-10);
                if (res.Total == 0)
                    return DateTime.Now;
                return res.ElasticResults.Hits.First().Source.LastUpdate;
            }


            public Results GetResults(DateTime? fromDate = null, DateTime? toDate = null, int? maxItems = null, string order = null)
            {
                maxItems = maxItems ?? 30;
                string query = this.OrigWD.SearchTerm;
                if (fromDate.HasValue || toDate.HasValue)
                {
                    query += " AND zverejneno:";
                    query += string.Format("[{0} TO {1}]", Searching.Tools.ToElasticDate(fromDate, "*"), Searching.Tools.ToElasticDate(toDate, "*"));
                }
                var res = Lib.Data.Smlouva.Search.SimpleSearch(query, 0, maxItems.Value,
                    (Smlouva.Search.OrderResult)Convert.ToInt32(order ?? "4")
                    );
                return new Watchdogs.Results(res.ElasticResults.Hits.Select(m => (dynamic)m.Source), res.Total,
                    query, fromDate, toDate, res.IsValid, typeof(Smlouva).Name);
            }

            public RenderedContent RenderResults(Results data, long numOfListed = 5)
            {
                RenderedContent ret = new RenderedContent();
                List<RenderedContent> items = new List<RenderedContent>();
                if (data.Total <= (numOfListed + 2))
                    numOfListed = data.Total;

                var renderH = new Lib.Render.ScribanT(HtmlTemplate.Replace("#LIMIT#", numOfListed.ToString()));
                ret.ContentHtml = renderH.Render(data);
                var renderT = new Lib.Render.ScribanT(TextTemplate.Replace("#LIMIT#", numOfListed.ToString()));
                ret.ContentText = renderT.Render(data);

                return ret;
            }

            static string HtmlTemplate = @"
    <table border='0' cellpadding='5' class=''>
        {{ for item in model.Items limit:#LIMIT# }}
                <tr>
                    <td><a href='@Raw('https://www.hlidacstatu.cz/Detail/' + item.Id + '?utm_source=hlidac&utm_medium=emailtxt&utm_campaign=detail')'>Detail</a></td>
                    <td>{{item.Platce.nazev}}</td>
                    <td>{{item.Prijemce.nazev}}</td>
                    <td>{{ fn_FormatPrice item.CalculatedPriceWithVATinCZK}}</td>
                </tr>
                <tr>
                    <td colspan='4'>{{ item.Predmet }}</td>
                </tr>
        {{ end }}

        {{ if (model.Items.size > #LIMIT#) }}

            <tr><td colspan='4'>    
                <hr/>
                <a href='https://www.hlidacstatu.cz/HledatSmlouvy?Q={{ html.url_encode Model.SpecificQuery }}&utm_source=hlidac&utm_medium=emailtxt&utm_campaign=more'>
                    {{ fn_Pluralize (model.Items.size - #LIMIT#) '' 'Další nalezená smlouva' 'Další {0} nalezené smlouvy' 'Dalších {0} nalezených smluv' }} 
                </a>.
            </td></tr>
        {{ end }}
    </table>
";
            static string TextTemplate = @"
        {{ for item in model.Items limit:#LIMIT# }}
------------------------------------------------------
| {{item.Platce.nazev}} ->  {{item.Prijemce.nazev}}
-  -  -  -  -  -  -  -  -  -  -  -  -  -  -  
{{ fn_FormatPrice item.CalculatedPriceWithVATinCZK }} / {{item.Predmet}}

Více: https://www.hlidacstatu.cz/Detail/{{item.Id}}?utm_source=hlidac&utm_medium=emailtxt&utm_campaign=detail
======================================================

        {{ end }}

{{ if (model.Items.size > #LIMIT#) }}
{{ fn_Pluralize (model.Items.size - #LIMIT#) '' 'Další nalezená smlouva' 'Další {0} nalezené smlouvy' 'Dalších {0} nalezených smluv' }} na  https://www.hlidacstatu.cz/HledatSmlouvy?Q=@(Raw(System.Web.HttpUtility.UrlEncode(Model.SpecificQuery)))&utm_source=hlidac&utm_medium=emailtxt&utm_campaign=more'>
{{ end }}

";
        }
    }
}
