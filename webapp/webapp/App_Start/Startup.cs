using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;

namespace K9.WebApplication
{
	public static class Startup
	{
		public static void Register()
		{
			var builder = new ContainerBuilder();

			builder.RegisterControllers(typeof(MvcApplication).Assembly);
			builder.RegisterModelBinders(typeof(MvcApplication).Assembly);
			builder.RegisterModelBinderProvider();
			builder.RegisterModule<AutofacWebTypesModule>();
			builder.RegisterSource(new ViewRegistrationSource());
			builder.RegisterFilterProvider();

			var container = builder.Build();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
		}
	}
}
