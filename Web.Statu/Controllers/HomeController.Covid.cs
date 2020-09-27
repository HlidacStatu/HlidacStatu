using Devmasters;
using Devmasters.Enums;

using HlidacStatu.Lib;
using HlidacStatu.Lib.Data;
using HlidacStatu.Lib.Data.VZ;
using HlidacStatu.Lib.Render;
using HlidacStatu.Web.Framework;
using HlidacStatu.Web.Models;

using Microsoft.ApplicationInsights;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace HlidacStatu.Web.Controllers
{
    public partial class HomeController : GenericAuthController
    {
        [OutputCache(VaryByParam = "id", Duration = 60 * 60 * 1)]
        public ActionResult KapacitaNemocnicData(string id)
        {
                        var client = NemocniceData.Client();

            NemocniceData[] n = client.Search<NemocniceData>(s => s
                    .Size(200)
                    .Sort(o => o.Descending(f => f.lastUpdated))
                    .Query(q => q.MatchAll())
                )
                .Hits
                .Select(m => m.Source)
                .ToArray();

            if (id=="last")
                this.Content(Newtonsoft.Json.JsonConvert.SerializeObject(n.First()),"text/json");
            return this.Content(Newtonsoft.Json.JsonConvert.SerializeObject(n),"text/json");
        }
        public ActionResult KapacitaNemocnic(string id, string nemocniceId)
        {

            var client = NemocniceData.Client();
            List<NemocniceData> days = new List<NemocniceData>();

            NemocniceData first = client.Search<NemocniceData>(s => s
                    .Size(1)
                    .Sort(o => o.Ascending(f => f.lastUpdated))
                    .Query(q => q.MatchAll())
                )
                .Hits
                .FirstOrDefault()?.Source;

            NemocniceData[] nAll = client.Search<NemocniceData>(s => s
                .Size(200)
                .Sort(o => o.Descending(f => f.lastUpdated))
                .Query(q => q.MatchAll())
            )
            .Hits
            .Select(m => m.Source)
            .ToArray();

            var filteredN = new List<NemocniceData>();
            filteredN.Add(first);
            foreach (var n in nAll.OrderBy(o=>o.lastUpdated))
            {
                if ((n.lastUpdated - filteredN.Last().lastUpdated).TotalHours > 3.5)
                    filteredN.Add(n);
            }
            days.Insert(0, first);
            days.AddRange(filteredN);
            if (days.Last().lastUpdated < nAll.First().lastUpdated)
                days.Add(nAll.First());

            NemocniceData last = days.Last();

            NemocniceData diffK = NemocniceData.Diff(first, last);
            NemocniceData diff = NemocniceData.Diff(first, last);

            (NemocniceData diffK, List<NemocniceData> dny, NemocniceData last, NemocniceData diffAll) model 
                = (diffK, days, last, diff);
            return View(model);
        }


    }
}