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
				"~/Content/template/*.css"));

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