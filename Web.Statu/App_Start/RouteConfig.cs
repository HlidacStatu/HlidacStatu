using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HlidacStatu.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                name: "ApiV1",
                url: "api/v1/{action}/{id}/{dataid}",
                defaults: new { controller = "ApiV1", action = "Index", id = UrlParameter.Optional, dataid = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "ApiV2Smlouvy",
                url: "api/v2/smlouvy/{action}/{id}/{dataid}",
                defaults: new { controller = "ApiV2Smlouvy", action = "Index", id = UrlParameter.Optional, dataid = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "ApiV2VerejneZakazky",
                url: "api/v2/verejnezakazky/{action}/{id}/{dataid}",
                defaults: new { controller = "ApiV2VZ", action = "Index", id = UrlParameter.Optional, dataid = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "AccountController",
                url: "account/{action}/{id}",
                defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "ManageController",
                url: "manage/{action}/{id}",
                defaults: new { controller = "Manage", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "RolesController",
                url: "Roles/{action}/{id}",
                defaults: new { controller = "Roles", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "UctyController",
                url: "ucty/{action}/{id}",
                defaults: new { controller = "Ucty", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "SponzoriController",
                url: "Sponzori/{action}/{id}",
                defaults: new { controller = "Sponzori", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "NasiPoliticiController",
                url: "PrototypNP/{action}/{id}",
                defaults: new { controller = "NasiPolitici", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "DotaceController",
                url: "Dotace/{action}/{id}",
                defaults: new { controller = "Dotace", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "WebyController",
                url: "StatniWeby/{action}/{id}",
                defaults: new { controller = "Weby", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "VZController",
                url: "VerejneZakazky/{action}/{id}",
                defaults: new { controller = "VZ", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "DataController",
                url: "Data/{action}/{id}/{dataid}",
                defaults: new { controller = "Data", action = "Index", id = UrlParameter.Optional, dataid = UrlParameter.Optional }
            );

			routes.MapRoute(
				name: "InsolvenceController",
				url: "Insolvence/{action}/{id}",
				defaults: new { controller = "Insolvence", action = "Index", id = UrlParameter.Optional }
			);

			routes.MapRoute(
                name: "Default",
                url: "{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
