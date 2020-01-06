using HlidacStatu.Lib.Data;
using HlidacStatu.Util;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Nest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static HlidacStatu.Web.Models.ApiV1Models;

namespace HlidacStatu.Web.Controllers
{
    public partial class GenericAuthController : AsyncController
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

        public Microsoft.AspNet.Identity.EntityFramework.IdentityUser AuthUser()
        {

            Microsoft.AspNet.Identity.EntityFramework.IdentityUser user = UserManager.FindByEmail(this.User.Identity.Name);
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

        [OutputCache(VaryByParam = "*", Duration = 60 * 60 * 1)]
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



    }
}