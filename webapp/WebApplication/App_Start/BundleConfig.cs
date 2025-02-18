using System.Web.Optimization;

namespace K9.WebApplication
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/lib").Include(
                "~/Content/fontawesome/all.css",
                "~/Content/fontawesome/font-awesome-legacy.css",
                "~/Content/bootstrap/*.css"));

            bundles.Add(new StyleBundle("~/Content/sections").Include(
                "~/Content/main/elements.css",
                "~/Content/bootstrap-custom/*.css",
                "~/Content/device/*.css",
                "~/Content/sections/*.css",
                "~/Content/controls/*.css"));
            
            bundles.Add(new StyleBundle("~/Content/style").Include(
                "~/Content/main/style.css"));

            bundles.Add(new StyleBundle("~/Content/responsive").Include(
                "~/Content/main/style.1200.css",
                "~/Content/main/style.1080.css",
                "~/Content/main/style.1024.css",
                "~/Content/main/style-lg.css",
                "~/Content/main/style-md.css",
                "~/Content/main/style-sm.css",
                "~/Content/main/style-xs.css",
                "~/Content/main/style.991.css",
                "~/Content/main/style.956.css",
                "~/Content/main/style.768.css",
                "~/Content/main/style.760.css",
                "~/Content/main/style.736.css",
                "~/Content/main/style.610.css",
                "~/Content/main/style.525.css",
                "~/Content/main/style.480.css",
                "~/Content/main/style.414.css",
                "~/Content/main/style.384.css",
                "~/Content/main/style.375.css",
                "~/Content/main/style.320.css"));

            bundles.Add(new ScriptBundle("~/Scripts/js").Include(
                "~/Scripts/imageSwitcher/*.js",
                "~/Scripts/template/*.js",
                "~/Scripts/ajax/*.js",
                "~/Scripts/k9/*.js"));

            bundles.Add(new ScriptBundle("~/Scripts/lib").Include(
                "~/Scripts/library/*.js"));

            BundleTable.EnableOptimizations = false;
        }
    }
}