using System.Data.Entity;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using K9.DataAccess.Database;
using K9.DataAccess.Models;
using K9.DataAccess.Respositories;
using K9.SharedLibrary.Models;
using K9.WebApplication.DataSets;
using K9.WebApplication.Helpers;
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

			builder.RegisterType<Db>().As<DbContext>().InstancePerHttpRequest();
			builder.Register(c => LogManager.GetCurrentClassLogger()).As<ILogger>().SingleInstance();
			builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IRepository<>));
			builder.RegisterGeneric(typeof(DataTableAjaxHelper<>)).As(typeof(IDataTableAjaxHelper<>));
			builder.RegisterType<ColumnsConfig>().As<IColumnsConfig>().SingleInstance();
			builder.RegisterType<DataSetsHelper>().As<IDataSetsHelper>().InstancePerHttpRequest();
			builder.RegisterType<DataSets.DataSets>().As<IDataSets>().SingleInstance();

			var container = builder.Build();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
		}

		public static void RegisterStaticTypes()
		{
			HtmlHelpers.SetIgnoreColumns(new ColumnsConfig());
		}
	}
}
