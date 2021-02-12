using System.Web;
using System.Web.Optimization;

namespace archivesystemWebUI
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Content/assets/js/select2.min.js"
                        ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));



            // Custom theme Bundles


                // Scripts

            bundles.Add(new ScriptBundle("~/bundles/custom/jquery").Include(
                "~/Content/assets/js/jquery-3.5.1.min.js"));


            bundles.Add(new ScriptBundle("~/bundles/custom/bootstrap").Include(
                "~/Content/assets/js/popper.min.js",
                "~/Content/assets/js/bootstrap.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/custom/js").Include(
                "~/Content/assets/js/jquery.slimscroll.min.js",
                "~/Content/assets/js/app.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/charts/js").Include(
                "~/Content/assets/plugins/morris/morris.min.js",
                "~/Content/assets/plugins/raphael/raphael.min.js",
                "~/Content/assets/js/chart.js"
            ));


            // Styles

            bundles.Add(new StyleBundle("~/Content/custom/css").Include(
                "~/Content/assets/css/bootstrap.min.css",
                "~/Content/assets/css/font-awesome.min.css",
                "~/Content/assets/css/line-awesome.min.css",
                "~/Content/assets/css/select2.min.css",
                "~/Content/assets/css/style.css"
                ));

            bundles.Add(new StyleBundle("~/Content/charts/css").Include(
                "~/Content/assets/plugins/morris/morris.css"
            ));

        }
    }
}
