using System;
using System.Data.Entity;
using System.IO;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using K9.DataAccess.Config;
using K9.DataAccess.Database;
using K9.DataAccess.Helpers;
using K9.DataAccess.Respositories;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
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
			builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IRepository<>)).InstancePerHttpRequest();
			builder.RegisterGeneric(typeof(DataTableAjaxHelper<>)).As(typeof(IDataTableAjaxHelper<>)).InstancePerHttpRequest();
			builder.RegisterType<ColumnsConfig>().As<IColumnsConfig>().SingleInstance();
			builder.RegisterType<DataSetsHelper>().As<IDataSetsHelper>().InstancePerHttpRequest();
			builder.RegisterType<DataSets.DataSets>().As<IDataSets>().SingleInstance();
			builder.RegisterType<Users>().As<IUsers>().InstancePerHttpRequest();
			builder.RegisterType<Roles>().As<IRoles>().InstancePerHttpRequest();
			builder.RegisterType<Mailer>().As<IMailer>().InstancePerHttpRequest();

			RegisterConfiguration(builder);

			var container = builder.Build();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
		}

		public static void RegisterStaticTypes()
		{
			HtmlHelpers.SetIgnoreColumns(new ColumnsConfig());
		}

		public static void RegisterConfiguration(ContainerBuilder builder)
		{
			var configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config/appsettings.json");
			var json = File.ReadAllText(configFilePath);

			builder.Register(c => ConfigHelper.GetConfiguration<SmtpConfiguration>(json)).SingleInstance();
			//builder.Register(c => ConfigHelper.GetConfiguration<OpenAuthConfiguration>(json)).As<ISmtpConfiguration>().SingleInstance();
		}
	}
}
