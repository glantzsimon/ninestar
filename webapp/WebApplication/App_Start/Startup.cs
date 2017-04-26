using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using K9.DataAccess.Database;
using K9.DataAccess.Models;
using K9.DataAccess.Respositories;
using NLog;

namespace K9.WebApplication
{
	public static class Startup
	{
		public static void RegisterTypes()
		{
			var builder = new ContainerBuilder();

			builder.RegisterControllers(typeof(MvcApplication).Assembly);
			builder.RegisterModelBinders(typeof(MvcApplication).Assembly);
			builder.RegisterModelBinderProvider();
			builder.RegisterModule<AutofacWebTypesModule>();
			builder.RegisterSource(new ViewRegistrationSource());
			builder.RegisterFilterProvider();

			builder.RegisterType<Db>().As<DbContext>().InstancePerDependency();
			builder.Register(c => LogManager.GetCurrentClassLogger()).As<ILogger>();
			builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IRepository<>));

			var container = builder.Build();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
		}
	}
}
