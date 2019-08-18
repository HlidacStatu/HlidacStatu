using System.Web.Optimization;

namespace HlidacStatu.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //BundleTable.EnableOptimizations = true;
            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Scripts/jquery-1.11.3.min.js")
                );

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/respond.js",
                        "~/Scripts/hlidac.v1.7.js",
                        "~/Scripts/fuckadblock.min.js",
                        "~/scripts/social-share-kit.min.js"
                      ));

            var highchartsBundle = new Bundle("~/bundles/highcharts");
            highchartsBundle.Include(
                "~/Scripts/Highcharts-6/js/highcharts.js",
                "~/Scripts/highcharts.global.options.js"
                //"~/Scripts/Highcharts-4.0.1/js/modules/exporting.js"
                );
            bundles.Add(highchartsBundle);

            var highcharts6Bundle = new Bundle("~/bundles/highcharts6");
            highcharts6Bundle.Include(
                "~/Scripts/Highcharts-6/js/highcharts.js"
                );
            bundles.Add(highcharts6Bundle);

            var typeaheadBundle = new Bundle("~/bundles/typeahead");
            typeaheadBundle.Include(
                "~/Scripts/typeahead.bundle.min.js",
                "~/Scripts/bloodhound.min.js"
                );
            bundles.Add(typeaheadBundle);

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/globalsite.v1.7.css",
                       "~/content/social-share-kit.css",
                      "~/Content/new.css"
                      ));


        }
    }
}
