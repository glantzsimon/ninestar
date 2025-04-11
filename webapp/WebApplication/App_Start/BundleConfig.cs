using K9.WebApplication.Extensions;
using System.Web.Optimization;

namespace K9.WebApplication
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            var lib = new StyleBundle("~/Content/lib");
            lib.IncludeWithRewrite(
                "~/Content/fontawesome/all.css",
                "~/Content/fontawesome/font-awesome-legacy.css");
            lib.IncludeAllCssWithRewrite("~/Content/bootstrap");
            lib.IncludeWithRewrite("~/Content/bootstrap-custom");
            bundles.Add(lib);

            var sections = new StyleBundle("~/Content/sections");
            sections.IncludeAllCssWithRewrite("~/Content/sections");
            bundles.Add(sections);

            var main = new StyleBundle("~/Content/main");
            main.IncludeWithRewrite("~/Content/main/elements.css");
            main.IncludeAllCssWithRewrite("~/Content/device");
            main.IncludeAllCssWithRewrite("~/Content/controls");
            main.IncludeWithRewrite("~/Content/main/style.css");
            bundles.Add(main);
            
            var responsive = new StyleBundle("~/Content/responsive");
            responsive.IncludeWithRewrite(
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
                "~/Content/main/style.320.css"
            );
            bundles.Add(responsive);

            // JavaScript – wildcards are fine here, no transform needed
            bundles.Add(new ScriptBundle("~/Scripts/js").Include(
                "~/Scripts/imageSwitcher/*.js",
                "~/Scripts/template/*.js",
                "~/Scripts/ajax/*.js",
                "~/Scripts/k9/*.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/lib").Include(
                "~/Scripts/library/*.js"
            ));

#if DEBUG
            BundleTable.EnableOptimizations = true;
#else
            BundleTable.EnableOptimizations = false;
#endif
        }
    }
}