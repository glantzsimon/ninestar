using System.Web.Optimization;

namespace K9.WebApplication
{
	public class BundleConfig
	{
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new StyleBundle("~/Content/lib").Include(
				"~/Content/fontawesome/*.css",
				"~/Content/bootstrap/*.css"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
				"~/Content/elements.css",
				"~/Content/classes.css",
				"~/Content/shared.css",
				"~/Content/validation.css",
				"~/Content/k9/*.css",
				"~/Content/desktop.css",
				"~/Content/tablet.css",
				"~/Content/mobile.css",
				"~/Content/botf.css"));

			bundles.Add(new StyleBundle("~/Content/tpl").Include(
				"~/Content/template/lsb.css",
				"~/Content/template/owl.carousel.css",
				"~/Content/template/owl.theme.css",
				"~/Content/template/style.css",
				"~/Content/template/style.1080.css",
				"~/Content/template/style.1024.css",
				"~/Content/template/style.991.css",
				"~/Content/template/style.736.css",
				"~/Content/template/style.480.css",
				"~/Content/template/style.414.css",
				"~/Content/template/style.384.css",
				"~/Content/template/style.375.css",
				"~/Content/template/style.320.css"));

			bundles.Add(new ScriptBundle("~/Scripts/js").Include(
				"~/Scripts/imageSwitcher/*.js",
				"~/Scripts/template/*.js",
				"~/Scripts/k9/*.js"));

			bundles.Add(new ScriptBundle("~/Scripts/lib").Include(
				"~/Scripts/library/*.js"));

			BundleTable.EnableOptimizations = true;
		}
	}
}