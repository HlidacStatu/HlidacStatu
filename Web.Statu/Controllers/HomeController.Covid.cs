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

            Lib.Data.External.DataSets.DataSet ds = Lib.Data.External.DataSets.DataSet.CachedDatasets.Get("kapacity-nemocnic");


            NemocniceData[] nAll = ds
                .SearchDataRaw("*", 1, 1000).Result
                .Select(s => Newtonsoft.Json.JsonConvert.DeserializeObject<NemocniceData>(s.Item2))
                .OrderByDescending(m=>m.lastUpdated)
                .Take(36)
                .Reverse()
                .ToArray();



            return View(nAll);
        }


    }
}