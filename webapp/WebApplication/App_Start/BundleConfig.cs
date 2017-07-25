using System.Web.Optimization;

namespace K9.WebApplication
{
	public class BundleConfig
	{
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new StyleBundle("~/Content/css").Include(
				"~/Content/elements.css",
				"~/Content/classes.css",
				"~/Content/validation.css",
				"~/Content/botf.css",
				"~/Content/k9/*.css",
				"~/Content/tablet.css",
				"~/Content/mobile.css"));

			bundles.Add(new ScriptBundle("~/Scripts/k9").Include(
				"~/Scripts/imageSwitcher/*.js",
				"~/Scripts/k9/*.js"));
		}
	}
}