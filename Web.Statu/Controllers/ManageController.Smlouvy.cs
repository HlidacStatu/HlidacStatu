using HlidacStatu.Lib.Data;
using HlidacStatu.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HlidacStatu.Web.Controllers
{
    [Authorize]
    public partial class ManageController : GenericAuthController
    {

        [Authorize(Roles = "canEditData")]
        public ActionResult EditClassification(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) 
                return RedirectToAction(nameof(HomeController.Index), nameof(HomeController));
            
            var smlouva = Smlouva.Load(id);
            
            if (smlouva is null)
                return RedirectToAction(nameof(HomeController.Index), nameof(HomeController));
            
            return View(smlouva);
        }

        // set classification
        [Authorize(Roles = "canEditData")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditClassification(string id, string typ1, string typ2)
        {
            var smlouva = Smlouva.Load(id);
            if (smlouva is null)
                return RedirectToAction(nameof(HomeController.Index), nameof(HomeController));

            List<int> typeVals = new List<int>();
            if (int.TryParse(typ1, out int t1))
                typeVals.Add(t1);
            if (int.TryParse(typ2, out int t2))
                typeVals.Add(t2);


            if(typeVals.Count > 0)
            {
                smlouva.OverrideClassification(typeVals.ToArray(), this.User.Identity.Name);

            }

            return RedirectToAction(nameof(HomeController.Detail),nameof(HomeController), new { Id = id});
        }
    }
}