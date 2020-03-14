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
                ret.ContentTitle = "Smlouvy";

                return ret;
            }

            static string HtmlTemplate = @"
    <table border='0' cellpadding='5' class='' width='100%' >
        <thead>
            <tr>
                <th></th>
                <th>Plátce</th>
                <th>Příjemci</th>
                <th>Hodnota smlouvy</th>
            </tr>
        </thead>
        <tbody>
        {{ for item in model.Items limit:#LIMIT# }}
                <tr>
                    <td><a href='@Raw('https://www.hlidacstatu.cz/Detail/' + item.Id + '?utm_source=hlidac&utm_medium=emailtxt&utm_campaign=detail')'>Detail</a></td>
                    <td>{{item.Platce.nazev}}</td>
                    <td>{{for pp in item.Prijemce; ((fn_ShortenText pp.nazev 40) + ', '); end }}</td>
                    <td>{{ fn_FormatPrice item.CalculatedPriceWithVATinCZK html: true}}</td>
                </tr>
                <tr>
                    <td colspan='4' style='font-size:80%;border-bottom:1px #ddd solid'>{{ fn_ShortenText item.predmet 130 }}</td>
<!-- <hr size='1' style='border-width: 1px;' /> -->
                </tr>
        {{ end }}

        {{ if (model.Total > #LIMIT#) }}
            <tr><td colspan='4' height='30' style='line-height: 30px; min-height: 30px;'></td></tr>
            <tr><td colspan='4'>                   
                <a href='https://www.hlidacstatu.cz/HledatSmlouvy?Q={{ html.url_encode model.SpecificQuery }}&utm_source=hlidac&utm_medium=emailtxt&utm_campaign=more'>
                    {{ fn_Pluralize (model.Total - #LIMIT#) '' 'Další nalezená smlouva' 'Další {0} nalezené smlouvy' 'Dalších {0} nalezených smluv' }} 
                </a>.
            </td></tr>
            <tr><td colspan='4' height='30' style='line-height: 30px; min-height: 30px;'></td></tr>
        {{ end }}
        </tbody>
    </table>
";
            static string TextTemplate = @"
        {{ for item in model.Items limit:#LIMIT# }}
------------------------------------------------------
| {{item.Platce.nazev}} -> {{for pp in item.Prijemce; ((fn_ShortenText pp.nazev 40) + ', '); end }}
-  -  -  -  -  -  -  -  -  -  -  -  -  -  -  
{{ fn_FormatPrice item.CalculatedPriceWithVATinCZK html: false }} / {{ fn_ShortenText item.predmet 50 }}

Více: https://www.hlidacstatu.cz/Detail/{{item.Id}}?utm_source=hlidac&utm_medium=emailtxt&utm_campaign=detail
======================================================

        {{ end }}

{{ if (model.Total > #LIMIT#) }}
{{ fn_Pluralize (model.Total - #LIMIT#) '' 'Další nalezená smlouva' 'Další {0} nalezené smlouvy' 'Dalších {0} nalezených smluv' }} na  https://www.hlidacstatu.cz/HledatSmlouvy?Q=@(Raw(System.Web.HttpUtility.UrlEncode(model.SpecificQuery)))&utm_source=hlidac&utm_medium=emailtxt&utm_campaign=more'>
{{ end }}

";
        }
    }
}
