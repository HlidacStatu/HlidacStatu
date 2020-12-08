using HlidacStatu.Lib.Data;
using HlidacStatu.Web.Framework;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace HlidacStatu.Web.Controllers
{
    public partial class GenericAuthController : AsyncController, IAuthenticableController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public GenericAuthController()
        {
        }

        public GenericAuthController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }


        public Lib.Data.AspNetUser AuthUser()
        {
            Lib.Data.AspNetUser user = Lib.Data.AspNetUser.GetByEmail(this?.User?.Identity?.Name);
            return user;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }


        public string HostIpAddress { get => this.Request.UserHostAddress; }
        public string AuthToken
        {
            get
            {

                if (this.Request.Headers != null && this.Request.Headers.AllKeys.Contains("Authorization"))
                {
                    return this.Request.Headers["Authorization"];
                }
                else if (this.Request.QueryString["Authorization"] != null)
                {
                    return this.Request.QueryString["Authorization"];
                }

                return "";
            }
        }

        public ApiAuth.Result ApiAuth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

#if (!DEBUG)
        [OutputCache(VaryByParam = "*", Duration = 60 * 60 * 48)]
#endif
        [ChildActionOnly]
        public ActionResult CachedAction_Child_48H(object model, bool? auth, string nameOfView, string key, string param1, string param2, string param3, string param4, string param5, string param6, string param7, string param8)
        {
            ViewBag.NameOfView = nameOfView;
            ViewBag.keyParam = key;
            ViewBag.param1 = param1;
            ViewBag.param2 = param2;
            ViewBag.param3 = param3;
            ViewBag.param4 = param4;
            ViewBag.param5 = param5; ViewBag.param6 = param6; ViewBag.param7 = param7; ViewBag.param8 = param8;
            ViewBag.authParam = auth;
            return View(nameOfView, model);
        }

#if (!DEBUG)
        [OutputCache(VaryByParam = "*", Duration = 60 * 60 * 24)]
#endif
        [ChildActionOnly]
        public ActionResult CachedAction_Child_24H(object model, bool? auth, string nameOfView, string key, string param1, string param2, string param3, string param4, string param5, string param6, string param7, string param8)
        {
            ViewBag.NameOfView = nameOfView;
            ViewBag.keyParam = key;
            ViewBag.param1 = param1;
            ViewBag.param2 = param2;
            ViewBag.param3 = param3;
            ViewBag.param4 = param4;
            ViewBag.param5 = param5; ViewBag.param6 = param6; ViewBag.param7 = param7; ViewBag.param8 = param8;
            ViewBag.authParam = auth;
            return View(nameOfView, model);
        }

#if (!DEBUG)
        [OutputCache(VaryByParam = "*", Duration = 60 * 60 * 12)]
#endif
        [ChildActionOnly]
        public ActionResult CachedAction_Child_12H(object model, bool? auth, string nameOfView, string key, string param1, string param2, string param3, string param4, string param5, string param6, string param7, string param8)
        {
            ViewBag.NameOfView = nameOfView;
            ViewBag.keyParam = key;
            ViewBag.param1 = param1;
            ViewBag.param2 = param2;
            ViewBag.param3 = param3;
            ViewBag.param4 = param4;
            ViewBag.param5 = param5; ViewBag.param6 = param6; ViewBag.param7 = param7; ViewBag.param8 = param8;
            ViewBag.authParam = auth;
            return View(nameOfView, model);
        }

#if (!DEBUG)
        [OutputCache(VaryByParam = "*", Duration = 60 * 60 * 4)]
#endif
        [ChildActionOnly]
        public ActionResult CachedAction_Child_4H(object model, bool? auth, string nameOfView, string key, string param1, string param2, string param3, string param4, string param5, string param6, string param7, string param8)
        {
            ViewBag.NameOfView = nameOfView;
            ViewBag.keyParam = key;
            ViewBag.param1 = param1;
            ViewBag.param2 = param2;
            ViewBag.param3 = param3;
            ViewBag.param4 = param4;
            ViewBag.param5 = param5; ViewBag.param6 = param6; ViewBag.param7 = param7; ViewBag.param8 = param8;
            ViewBag.authParam = auth;
            return View(nameOfView, model);
        }

#if (!DEBUG)
        [OutputCache(VaryByParam = "*", Duration = 60 * 60 * 2)]
#endif
        [ChildActionOnly]
        public ActionResult CachedAction_Child_2H(object model, bool? auth, string nameOfView, string key, string param1, string param2, string param3, string param4, string param5, string param6, string param7, string param8)
        {
            ViewBag.NameOfView = nameOfView;
            ViewBag.keyParam = key;
            ViewBag.param1 = param1;
            ViewBag.param2 = param2;
            ViewBag.param3 = param3;
            ViewBag.param4 = param4;
            ViewBag.param5 = param5; ViewBag.param6 = param6; ViewBag.param7 = param7; ViewBag.param8 = param8;
            ViewBag.authParam = auth;
            return View(nameOfView, model);
        }

#if (!DEBUG)
        [OutputCache(VaryByParam = "*", Duration = 20 * 60 * 1)]
#endif
        [ChildActionOnly]
        public ActionResult CachedAction_Child_20min(object model, bool? auth, string nameOfView, string key, string param1, string param2, string param3, string param4, string param5, string param6, string param7, string param8)
        {
            ViewBag.NameOfView = nameOfView;
            ViewBag.keyParam = key;
            ViewBag.param1 = param1;
            ViewBag.param2 = param2;
            ViewBag.param3 = param3;
            ViewBag.param4 = param4;
            ViewBag.param5 = param5; ViewBag.param6 = param6; ViewBag.param7 = param7; ViewBag.param8 = param8;
            ViewBag.authParam = auth;
            return View(nameOfView, model);
        }

#if (!DEBUG)
        [OutputCache(VaryByParam = "*", Duration = 60 * 60 * 1)]
#endif
        [ChildActionOnly]
        public ActionResult CachedAction_Child_1H(object model, bool? auth, string nameOfView, string key, string param1, string param2, string param3, string param4, string param5, string param6, string param7, string param8)
        {
            ViewBag.NameOfView = nameOfView;
            ViewBag.keyParam = key;
            ViewBag.param1 = param1;
            ViewBag.param2 = param2;
            ViewBag.param3 = param3;
            ViewBag.param4 = param4;
            ViewBag.param5 = param5; ViewBag.param6 = param6; ViewBag.param7 = param7; ViewBag.param8 = param8;
            ViewBag.authParam = auth;
            return View(nameOfView, model);
        }

        [OutputCache(VaryByParam = "*", Duration = 60 * 1)] //1min
        [ChildActionOnly]
        public ActionResult CachedAction_Child_debug(object model, bool? auth, string nameOfView, string key, string param1, string param2, string param3, string param4, string param5, string param6, string param7, string param8)
        {
            ViewBag.NameOfView = nameOfView;
            ViewBag.keyParam = key;
            ViewBag.param1 = param1;
            ViewBag.param2 = param2;
            ViewBag.param3 = param3;
            ViewBag.param4 = param4;
            ViewBag.param5 = param5; ViewBag.param6 = param6; ViewBag.param7 = param7; ViewBag.param8 = param8;
            ViewBag.authParam = auth;
            return View(nameOfView, model);
        }


        [ChildActionOnly]
        public ActionResult Action_Child(object model, bool? auth, string nameOfView, string key, string param1, string param2, string param3, string param4, string param5, string param6, string param7, string param8)
        {
            ViewBag.NameOfView = nameOfView;
            ViewBag.keyParam = key;
            ViewBag.param1 = param1;
            ViewBag.param2 = param2;
            ViewBag.param3 = param3;
            ViewBag.param4 = param4;
            ViewBag.param5 = param5; ViewBag.param6 = param6; ViewBag.param7 = param7; ViewBag.param8 = param8;
            ViewBag.authParam = auth;
            return View(nameOfView, model);
        }

        public ActionResult NotFound(string nextUrl = null, string nextUrlText = null)
        {
            ViewBag.NextText = nextUrl;
            ViewBag.NextUrlText = nextUrlText;
            Response.StatusCode = 404;


            HlidacStatu.Util.Consts.Logger.Warning(new Devmasters.Logging.LogMessage()
                .SetMessage("Url not found")
                .SetCustomKeyValue("URL", Request.RawUrl)
                );

            return View("Error404");

        }


        protected override void HandleUnknownAction(string actionName)
        {
            string url = null;
            using (DbEntities db = new DbEntities())
            {
                url = db.TipUrl.Where(m => m.Name == actionName).Select(m => m.Url).FirstOrDefault();
            }
            if (!string.IsNullOrWhiteSpace(url))
            {
                Redirect(url).ExecuteResult(this.ControllerContext);
                return;
            }

            HlidacStatu.Util.Consts.Logger.Warning(new Devmasters.Logging.LogMessage()
                .SetMessage("Url not found")
                .SetCustomKeyValue("URL", Request.RawUrl)
                );


            RedirectToAction("Error404", "Home").ExecuteResult(this.ControllerContext);

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

    }
}