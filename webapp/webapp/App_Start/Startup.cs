using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Autofac;
using System.Web.Mvc;
using Autofac.Integration.Mvc;

namespace webapp
{
	public static class Starup
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
