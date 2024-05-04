using Autofac;
using Autofac.Integration.Mvc;
using K9.Base.DataAccessLayer.Config;
using K9.Base.DataAccessLayer.Helpers;
using K9.Base.DataAccessLayer.Respositories;
using K9.Base.WebApplication.Config;
using K9.Base.WebApplication.DataSets;
using K9.Base.WebApplication.Helpers;
using K9.Base.WebApplication.Security;
using K9.Base.WebApplication.Services;
using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Database;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Services;
using K9.WebApplication.Services.Stripe;
using NLog;
using System;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Web.Mvc;

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

            builder.RegisterType<LocalDb>().As<DbContext>().InstancePerRequest();
            builder.Register(c => LogManager.GetCurrentClassLogger()).As<ILogger>().SingleInstance();
            builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IRepository<>)).InstancePerRequest();
            builder.RegisterGeneric(typeof(DataTableAjaxHelper<>)).As(typeof(IDataTableAjaxHelper<>)).InstancePerRequest();
            builder.RegisterType<Config.ColumnsConfig>().As<IColumnsConfig>().InstancePerRequest();
            builder.RegisterType<DataSetsHelper>().As<IDataSetsHelper>().InstancePerRequest();
            builder.RegisterType<DataSets>().As<IDataSets>().SingleInstance();
            builder.RegisterType<Users>().As<IUsers>().InstancePerRequest();
            builder.RegisterType<Roles>().As<IRoles>().InstancePerRequest();
            builder.RegisterType<Helpers.Mailer>().As<Helpers.IMailer>().InstancePerRequest();
            builder.RegisterType<Authentication>().As<IAuthentication>().InstancePerRequest();
            builder.RegisterType<PostedFileHelper>().As<IPostedFileHelper>().InstancePerRequest();
            builder.RegisterType<FileSourceHelper>().As<IFileSourceHelper>().InstancePerRequest();
            builder.RegisterGeneric(typeof(ControllerPackage<>)).As(typeof(IControllerPackage<>)).InstancePerRequest();
            builder.RegisterType<AccountService>().As<IAccountService>().InstancePerRequest();
            builder.RegisterType<ShopService>().As<IShopService>().InstancePerRequest();
            builder.RegisterType<FacebookService>().As<IFacebookService>().InstancePerRequest();
            builder.RegisterType<StripeService>().As<IStripeService>().InstancePerRequest();
            builder.RegisterType<DonationService>().As<IDonationService>().InstancePerRequest();
            builder.RegisterType<ConsultationService>().As<IConsultationService>().InstancePerRequest();
            builder.RegisterType<NineStarKiService>().As<INineStarKiService>().InstancePerRequest();
            builder.RegisterType<AccountMailerService>().As<IAccountMailerService>().InstancePerRequest();
            builder.RegisterType<MembershipService>().As<IMembershipService>().InstancePerRequest();
            builder.RegisterType<ContactService>().As<IContactService>().InstancePerRequest();
            builder.RegisterType<MailChimpService>().As<IMailChimpService>().InstancePerRequest();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerRequest();
            builder.RegisterType<RecaptchaService>().As<IRecaptchaService>().InstancePerRequest();
            builder.RegisterType<LogService>().As<ILogService>().InstancePerRequest();
            builder.RegisterType<BiorhythmsService>().As<IBiorhythmsService>().InstancePerRequest();

            RegisterConfiguration(builder);

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        public static void RegisterStaticTypes()
        {
            HtmlHelpers.SetIgnoreColumns(new Config.ColumnsConfig());
        }

        public static void RegisterConfiguration(ContainerBuilder builder)
        {
            var json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config/appsettings.json"));

            builder.Register(c => ConfigHelper.GetConfiguration<SmtpConfiguration>(json)).SingleInstance();
            builder.Register(c => ConfigHelper.GetConfiguration<DatabaseConfiguration>(json)).SingleInstance();
            builder.Register(c => ConfigHelper.GetConfiguration<OAuthConfiguration>(json)).SingleInstance();
            builder.Register(c => ConfigHelper.GetConfiguration<StripeConfiguration>(ConfigurationManager.AppSettings)).SingleInstance();
            builder.Register(c => ConfigHelper.GetConfiguration<MailChimpConfiguration>(ConfigurationManager.AppSettings)).SingleInstance();
            builder.Register(c => ConfigHelper.GetConfiguration<RecaptchaConfiguration>(ConfigurationManager.AppSettings)).SingleInstance();
            
            var websiteConfig = ConfigHelper.GetConfiguration<WebsiteConfiguration>(json);
            builder.Register(c => websiteConfig).SingleInstance();
            WebsiteConfiguration.Instance = websiteConfig.Value;

            var googleConfig = ConfigHelper.GetConfiguration<GoogleConfiguration>(json);
            builder.Register(c => googleConfig).SingleInstance();
            GoogleConfiguration.Instance = googleConfig.Value;
        }
    }
}
