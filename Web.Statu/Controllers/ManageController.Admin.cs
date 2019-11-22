using HlidacStatu.Lib.Data;
using HlidacStatu.Util;
using HlidacStatu.Web.Framework;
using HlidacStatu.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HlidacStatu.Web.Controllers
{
    [Authorize]
    public partial class ManageController : GenericAuthController
    {

        [Authorize(Roles = "canEditData")]
        [HttpGet]
        public ActionResult ZmenaSmluvnichStran(string id)
        {
            ViewBag.SmlouvaId = id;
            return View();
        }


        [Authorize(Roles = "canEditData")]
        [HttpPost]
        public ActionResult ZmenaSmluvnichStran(string id, FormCollection form)
        {
            ViewBag.SmlouvaId = id;
            if (string.IsNullOrEmpty(form["platce"]) 
                || string.IsNullOrEmpty(form["prijemce"])
                )
                {
                ModelState.AddModelError("Check", "Nastav smluvni strany");
                return View();
            }
            Smlouva s = Smlouva.Load(id);
            if (s == null)
            {
                ModelState.AddModelError("Check", "smlouva neexistuje");
                return View();
            }
            else
            {
                HlidacStatu.Plugin.Enhancers.ManualChanges mch = new Plugin.Enhancers.ManualChanges();

                var allSubjList = s.Prijemce.ToList();
                allSubjList.Insert(0, s.Platce);

                var platce = allSubjList[Convert.ToInt32(form["platce"])];
                List<Smlouva.Subjekt> prijemci = new List<Smlouva.Subjekt>();
                var iprijemci = form["prijemce"].Split(',').Select(m => Convert.ToInt32(m));
                foreach (var i in iprijemci)
                {
                    prijemci.Add(allSubjList[i]);
                }

                mch.UpdateSmluvniStrany(ref s, platce, prijemci.ToArray());

                List<Lib.Issues.Issue> issues = new List<Lib.Issues.Issue>();
                foreach (var ia in Lib.Issues.Util.GetIssueAnalyzers())
                {
                    var iss = ia.FindIssues(s);

                    if (iss != null)
                        issues.AddRange(iss);
                }

                s.Issues = issues.ToArray();


                s.Save();
                return Redirect(s.GetUrl(true));
            }
        }

    }
}