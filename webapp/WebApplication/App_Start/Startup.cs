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
using Microsoft.Owin;
using NLog;
using System;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Web.Mvc;
using Hangfire;
using K9.WebApplication.Helpers;
using Owin;
using HtmlHelpers = K9.Base.WebApplication.Helpers.HtmlHelpers;

[assembly: OwinStartup(typeof(K9.WebApplication.Startup))]

namespace K9.WebApplication
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
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
            builder.RegisterType<NineStarDataSetsHelper>().As<IDataSetsHelper>().InstancePerRequest();
            builder.RegisterType<DataSets>().As<IDataSets>().SingleInstance();
            builder.RegisterType<Users>().As<IUsers>().InstancePerRequest();
            builder.RegisterType<Roles>().As<IRoles>().InstancePerRequest();
            builder.RegisterType<Mailer>().As<IMailer>().InstancePerRequest();
            builder.RegisterType<Authentication>().As<IAuthentication>().InstancePerRequest();
            builder.RegisterType<PostedFileHelper>().As<IPostedFileHelper>().InstancePerRequest();
            builder.RegisterType<FileSourceHelper>().As<IFileSourceHelper>().InstancePerRequest();
            builder.RegisterGeneric(typeof(ControllerPackage<>)).As(typeof(IControllerPackage<>)).InstancePerRequest();
            builder.RegisterType<Services.AccountService>().As<Services.IAccountService>().InstancePerRequest();
            builder.RegisterType<ShopService>().As<IShopService>().InstancePerRequest();
            builder.RegisterType<FacebookService>().As<IFacebookService>().InstancePerRequest();
            builder.RegisterType<StripeService>().As<IStripeService>().InstancePerRequest();
            builder.RegisterType<DonationService>().As<IDonationService>().InstancePerRequest();
            builder.RegisterType<ConsultationService>().As<IConsultationService>().InstancePerRequest();
            builder.RegisterType<NineStarKiService>().As<INineStarKiService>().InstancePerRequest();
            builder.RegisterType<Services.AccountMailerService>().As<Services.IAccountMailerService>().InstancePerRequest();
            builder.RegisterType<MembershipService>().As<IMembershipService>().InstancePerRequest();
            builder.RegisterType<ContactService>().As<IContactService>().InstancePerRequest();
            builder.RegisterType<MailChimpService>().As<IMailChimpService>().InstancePerRequest();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerRequest();
            builder.RegisterType<RecaptchaService>().As<IRecaptchaService>().InstancePerRequest();
            builder.RegisterType<LogService>().As<ILogService>().InstancePerRequest();
            builder.RegisterType<BiorhythmsService>().As<IBiorhythmsService>().InstancePerRequest();
            builder.RegisterType<IChingService>().As<IIChingService>().InstancePerRequest();
            builder.RegisterType<EmailQueueService>().As<IEmailQueueService>().InstancePerRequest();
            builder.RegisterType<EmailTemplateService>().As<IEmailTemplateService>().InstancePerRequest();
            builder.RegisterType<PromoCodeService>().As<IPromoCodeService>().InstancePerRequest();

            RegisterConfiguration(builder);

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // Configure Hangfire to use SQL Server storage and the Autofac job activator
            GlobalConfiguration.Configuration
                .UseAutofacActivator(container)
                .UseSqlServerStorage("DefaultConnection"); // replace with your connection string name

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
            });

            RecurringJob.AddOrUpdate<EmailQueueService>(
                "ProcessEmailQueue",
                service => service.ProcessQueue(),
                Cron.MinuteInterval(10));        // Cron expression to run every 10 minutes
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

            var defaultConfig = ConfigHelper.GetConfiguration<DefaultValuesConfiguration>(json);
            builder.Register(c => defaultConfig).SingleInstance();
            DefaultValuesConfiguration.Instance = defaultConfig.Value;

            defaultConfig.Value.BaseEmailTemplateImagesPath = defaultConfig.Value.BaseImagesPath;
            defaultConfig.Value.BaseBaseEmailTemplateVideosPath = defaultConfig.Value.BaseVideosPath;

            var apiConfig = ConfigHelper.GetConfiguration<ApiConfiguration>(json);
            builder.Register(c => apiConfig).SingleInstance();
            ApiConfiguration.Instance = apiConfig.Value;

#if DEBUG
            Helpers.Environment.IsDebug = true;

            defaultConfig.Value.BaseImagesPath = "https://localhost/ninestar/Images";
            defaultConfig.Value.BaseVideosPath = "https://localhost/ninestar/Videos";
#endif
        }
    }
}
