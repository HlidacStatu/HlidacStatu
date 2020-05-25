using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HlidacStatu.Api.Dataset.Connector;

namespace Vybory_PSP
{
    class Program
    {
        static HlidacStatu.Api.Dataset.Connector.DatasetConnector dsc;
        public static Dictionary<string, string> args = new Dictionary<string, string>();

        static void Main(string[] arguments)
        {
            dsc = new HlidacStatu.Api.Dataset.Connector.DatasetConnector(
                System.Configuration.ConfigurationManager.AppSettings["apikey"]
                );
            //dsc.SetDeveleperUrl("http://local.hlidacstatu.cz/api/v1/");
            args = arguments
                .Select(m => m.Split('='))
                .ToDictionary(m => m[0].ToLower(), v => v.Length == 1 ? "" : v[1]);

            if (args.ContainsKey("/debug"))
                Parse.parallel = false;

            int? from = null;
            if (args.ContainsKey("/from"))
                from = int.Parse(args["/from"]);

            //create dataset
            var dsDef = new HlidacStatu.Api.Dataset.Connector.Dataset<jednani>(
                "Jednání výborů PSP", Parse.datasetname, "https://www.psp.cz/sqw/hp.sqw?k=194", "Databáze neutajených jednání výborů Poslanecké sněmovny Parlamentu České republiky. Zápisy, audio záznamy, usnesení a další dokumenty, které nepodléhají režimu utajení.",
                "https://github.com/HlidacStatu/Datasety/tree/master/Vybory-PSP",
                true, true,
                new string[,] { { "Datum jednání", "datum" }, { "Výboru", "vybor" } },
                new Template() { Body= @"

<!-- scriban {{ date.now }} --> 
<table class='table table-hover'>
                        <thead>
                            <tr>
<th style='min-width:120px'>Jednání</th>
<th style='min-width:120px'>Výbor</th>
<th style='min-width:120px'>Datum jednání</th>
</tr>
</thead>
<tbody>
{{ for item in model.Result }}
<tr>
<td ><a href='{{ fn_DatasetItemUrl item.Id }}'>č. {{ item.cisloJednani }}</a></td>
<td >{{ item.vybor }}</td>
<td>{{ fn_FormatDate item.datum 'dd. MM. yyyy' }}</td>
</tr>
{{ end }}

</tbody></table>

" },
                new Template() { Body = @"
{{this.item = model}}

<table class='table table-hover'><tbody>

<tr><td>Datum jednání</td><td >{{ fn_FormatDate item.datum 'd. MMMM yyyy' }}</td></tr>
<tr><td>Číslo jednání</td><td >{{ item.cisloJednani }}</a></td></tr>

{{ if !(fn_IsNullOrEmpty item.vec)  }}
   <tr><td>Téma</td><td >{{ item.vec }}</td></tr>
{{end }}

{{ if item.dokumenty.size > 0  }}

  {{ zapis = ''
     for doc in item.dokumenty
        if (string.contains doc.typ 'Zápis')
           zapis = zapis + doc.DocumentPlainText
        end  
     end

     if ( (fn_IsNullOrEmpty zapis) == false )
     }}
   <tr><td style='vertical-align:top;'>Zápis z jednání</td>
   <td>
      <div class='panel-body'>                                                                       
      <pre style='font-size:90%;background:none;line-height:1.6em;'>
        {{ fn_HighlightText highlightingData zapis 'dokumenty.DocumentPlainText' | string.replace '\n' '\n\n' }}                                                                                                                                                                                                                                                                  
      </pre>
      </div>
   </td></tr>
    {{
     end 
  }}


   <tr><td>Projednávané dokumenty</td><td>
<ul>
{{ for doc in item.dokumenty }}

   <li> 
      {{ doc.jmeno }} - 
      {{fn_LinkTextDocumentWithHighlighting doc 'Vybory-PSP' item.Id 'Obsah dokumentu' highlightingData }}
   </li>

{{ end }}
</ul>
</td></tr>


{{end }}

{{ if item.audio.size > 0  }}

   <tr><td>Zvukové záznamy</td><td >
    <ul>
    {{ for doc in item.audio }}

       <li> 

      
          <a href='{{ doc.DocumentUrl }}'>
              {{ doc.jmeno }}</a>
          {{ end }}
       </li>

    {{ end }}
    </ul>
    </td></tr>
{{end }}

</table>
" }
                );

            //dsc.DeleteDataset(dsDef).Wait();
            if (!dsc.DatasetExists(dsDef).Result)
            {
                dsc.CreateDataset(dsDef).Wait();
            }

            //download vybory
            Parse.InitVybory();
            //int idVyboru = 500;
            foreach (var idVyboru in Parse.Vybory.Keys)
            {
                Parse.Vybor(dsc, idVyboru);
            }
        }


    }
}
