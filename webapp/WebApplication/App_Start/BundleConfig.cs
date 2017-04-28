using System.Web.Optimization;

namespace K9.WebApplication
{
	public class BundleConfig
	{
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new StyleBundle("~/Content/css").Include(
				"~/Content/site.css",
				"~/Content/k9/*.css"));

		}
	}
}