using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HlidacStatu.Web.Controllers
{
    public class KindexController : GenericAuthController
    {
        public ActionResult Index()
        {
            if (!(this.User.Identity.IsAuthenticated == true))
            {
                return Redirect("/");
            }
            if (
                !(this.User.IsInRole("Admin") || this.User.IsInRole("BetaTester"))
                )
            {
                return Redirect("/");
            }
            return View();
        }
    }
}