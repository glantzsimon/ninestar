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
				"~/Content/mobile.css",
				"~/Content/validation.css",
				"~/Content/k9/*.css"));
		}
	}
}