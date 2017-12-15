using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace K9.WebApplication
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			
			Startup.RegisterTypes();
			Startup.RegisterStaticTypes();

			DataConfig.InitialiseDatabase();
			AuthConfig.InitialiseWebSecurity();
			DataConfig.InitialiseUsersAndRoles();

		    Stripe.StripeConfiguration.SetApiKey(ConfigurationManager.AppSettings["SecretKey"]);
        }
		
	}
}