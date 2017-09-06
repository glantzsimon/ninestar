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
using K9.WebApplication.Services;
using K9.WebApplication.UnitsOfWork;
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

			builder.RegisterType<Db>().As<DbContext>().InstancePerRequest();
			builder.Register(c => LogManager.GetCurrentClassLogger()).As<ILogger>().SingleInstance();
			builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IRepository<>)).InstancePerRequest();
			builder.RegisterGeneric(typeof(DataTableAjaxHelper<>)).As(typeof(IDataTableAjaxHelper<>)).InstancePerRequest();
			builder.RegisterType<ColumnsConfig>().As<IColumnsConfig>().SingleInstance();
			builder.RegisterType<DataSetsHelper>().As<IDataSetsHelper>().InstancePerRequest();
			builder.RegisterType<DataSets.DataSets>().As<IDataSets>().SingleInstance();
			builder.RegisterType<Users>().As<IUsers>().InstancePerRequest();
			builder.RegisterType<Roles>().As<IRoles>().InstancePerRequest();
			builder.RegisterType<Mailer>().As<IMailer>().InstancePerRequest();
			builder.RegisterType<PostedFileHelper>().As<IPostedFileHelper>().InstancePerRequest();
			builder.RegisterType<FileSourceHelper>().As<IFileSourceHelper>().InstancePerRequest();
			builder.RegisterGeneric(typeof(ControllerPackage<>)).As(typeof(IControllerPackage<>)).InstancePerRequest();
			builder.RegisterType<AccountService>().As<IAccountService>().InstancePerRequest();

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
			var json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config/appsettings.json"));

			builder.Register(c => ConfigHelper.GetConfiguration<SmtpConfiguration>(json)).SingleInstance();
			builder.Register(c => ConfigHelper.GetConfiguration<DatabaseConfiguration>(json)).SingleInstance();

			var websiteConfig = ConfigHelper.GetConfiguration<WebsiteConfiguration>(json);
			builder.Register(c => websiteConfig).SingleInstance();
			WebsiteConfiguration.Instance = websiteConfig.Value;
		}
	}
}
