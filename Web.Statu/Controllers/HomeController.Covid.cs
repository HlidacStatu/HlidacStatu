using Devmasters.Core;
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

            NemocniceData last = client.Search<NemocniceData>(s => s
                    .Size(1)
                    .Sort(o => o.Descending(f => f.lastUpdated))
                    .Query(q => q.MatchAll())
                )
                .Hits
                .FirstOrDefault()?.Source;

            for (int i = 1; i < (first.lastUpdated - last.lastUpdated).TotalDays; i++)
            {
                DateTime dt = first.lastUpdated.Date.AddDays(i).AddHours(14).AddMinutes(55);

                NemocniceData n = client.Search<NemocniceData>(s => s
                        .Size(1)
                        .Sort(o => o.Ascending(f => f.lastUpdated))
                        .Query(q => q.DateRange(d => d.GreaterThanOrEquals(dt).Field(f => f.lastUpdated)))
                    )
                    .Hits
                    .FirstOrDefault()?.Source;

                if (n != null)
                {
                    days.Add(n.PoKrajich());
                }
            }

            days.Insert(0, first.PoKrajich());
            days.Add(last.PoKrajich());


            NemocniceData diffK = NemocniceData.Diff(first.PoKrajich(), last.PoKrajich());
            NemocniceData diff = NemocniceData.Diff(first, last);

            (NemocniceData diffK, List<NemocniceData> dny, NemocniceData last, NemocniceData diffAll) model 
                = (diffK, days, last, diff);
            return View(model);
        }


    }
}