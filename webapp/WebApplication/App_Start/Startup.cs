using Autofac;
using Autofac.Integration.Mvc;
using Hangfire;
using Hangfire.SqlServer;
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
using K9.WebApplication.Helpers;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.Services.Stripe;
using Microsoft.Owin;
using NLog;
using Owin;
using System;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Web.Mvc;
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

            builder.RegisterType<LocalDb>().As<DbContext>().InstancePerLifetimeScope();
            builder.Register(c => LogManager.GetCurrentClassLogger()).As<ILogger>().SingleInstance();
            builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(DataTableAjaxHelper<>)).As(typeof(IDataTableAjaxHelper<>)).InstancePerLifetimeScope();
            builder.RegisterType<Config.ColumnsConfig>().As<IColumnsConfig>().InstancePerLifetimeScope();
            builder.RegisterType<NineStarDataSetsHelper>().As<IDataSetsHelper>().InstancePerLifetimeScope();
            builder.RegisterType<DataSets>().As<IDataSets>().SingleInstance();
            builder.RegisterType<Users>().As<IUsers>().InstancePerLifetimeScope();
            builder.RegisterType<Roles>().As<IRoles>().InstancePerLifetimeScope();
            builder.RegisterType<Mailer>().As<IMailer>().InstancePerLifetimeScope();
            builder.RegisterType<Authentication>().As<IAuthentication>().InstancePerLifetimeScope();
            builder.RegisterType<PostedFileHelper>().As<IPostedFileHelper>().InstancePerLifetimeScope();
            builder.RegisterType<FileSourceHelper>().As<IFileSourceHelper>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(ControllerPackage<>)).As(typeof(IControllerPackage<>)).InstancePerLifetimeScope();
            builder.RegisterType<Services.AccountService>().As<Services.IAccountService>().InstancePerLifetimeScope();
            builder.RegisterType<NineStarKiPackage>().As<INineStarKiPackage>().InstancePerLifetimeScope();
            builder.RegisterType<NineStarKiBasePackage>().As<INineStarKiBasePackage>().InstancePerLifetimeScope();
            builder.RegisterType<FacebookService>().As<IFacebookService>().InstancePerLifetimeScope();
            builder.RegisterType<StripeService>().As<IStripeService>().InstancePerLifetimeScope();
            builder.RegisterType<DonationService>().As<IDonationService>().InstancePerLifetimeScope();
            builder.RegisterType<ConsultationService>().As<IConsultationService>().InstancePerLifetimeScope();
            builder.RegisterType<NineStarKiService>().As<INineStarKiService>().InstancePerLifetimeScope();
            builder.RegisterType<Services.AccountMailerService>().As<Services.IAccountMailerService>().InstancePerLifetimeScope();
            builder.RegisterType<MembershipService>().As<IMembershipService>().InstancePerLifetimeScope();
            builder.RegisterType<ContactService>().As<IContactService>().InstancePerLifetimeScope();
            builder.RegisterType<MailChimpService>().As<IMailChimpService>().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<RecaptchaService>().As<IRecaptchaService>().InstancePerLifetimeScope();
            builder.RegisterType<LogService>().As<ILogService>().InstancePerLifetimeScope();
            builder.RegisterType<BiorhythmsService>().As<IBiorhythmsService>().InstancePerLifetimeScope();
            builder.RegisterType<IChingService>().As<IIChingService>().InstancePerLifetimeScope();
            builder.RegisterType<EmailQueueService>().As<IEmailQueueService>().InstancePerLifetimeScope();
            builder.RegisterType<EmailTemplateService>().As<IEmailTemplateService>().InstancePerLifetimeScope();
            builder.RegisterType<PromotionService>().As<IPromotionService>().InstancePerLifetimeScope();
            builder.RegisterType<MailingListService>().As<IMailingListService>().InstancePerLifetimeScope();
            builder.RegisterType<MailerService>().As<IMailerService>().InstancePerLifetimeScope();
            builder.RegisterType<NumerologyService>().As<INumerologyService>().InstancePerLifetimeScope();
            builder.RegisterType<SwissEphemerisService>().As<ISwissEphemerisService>().InstancePerLifetimeScope();

            RegisterConfiguration(builder);

            RegisterStaticTypes();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // Configure Hangfire to use SQL Server storage and the Autofac job activator
            GlobalConfiguration.Configuration
                .UseAutofacActivator(container)
                .UseSqlServerStorage("DefaultConnection", new SqlServerStorageOptions
                {
                    JobExpirationCheckInterval = TimeSpan.FromMinutes(30), // Checks for expired jobs every 30 minutes
                });

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
            });

            app.UseHangfireServer();

            RecurringJob.AddOrUpdate<IEmailQueueService>(
                "ProcessEmailQueue",
                service => service.ProcessQueue(),
                Cron.MinuteInterval(10));

            RecurringJob.AddOrUpdate("CleanupOldJobs", () =>
                CleanupOldJobs(), Cron.Daily);
        }

        public static void CleanupOldJobs()
        {
            var monitoringApi = JobStorage.Current.GetMonitoringApi();

            // Fetch first 10,000 failed jobs
            var failedJobs = monitoringApi.FailedJobs(0, 10000);
            foreach (var job in failedJobs)
            {
                var jobData = monitoringApi.JobDetails(job.Key);
                if (jobData?.CreatedAt < DateTime.UtcNow.AddDays(-3))
                {
                    BackgroundJob.Delete(job.Key);
                }
            }

            // Fetch first 10,000 deleted jobs
            var deletedJobs = monitoringApi.DeletedJobs(0, 10000);
            foreach (var job in deletedJobs)
            {
                var jobData = monitoringApi.JobDetails(job.Key);
                if (jobData?.CreatedAt < DateTime.UtcNow.AddDays(-3))
                {
                    BackgroundJob.Delete(job.Key);
                }
            }
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
#endif

#if !Integration
#if DEBUG
        defaultConfig.Value.BaseImagesPath = "https://localhost/ninestar/Images";
        defaultConfig.Value.BaseVideosPath = "https://localhost/ninestar/Videos";
#endif
#endif
        }
    }
}
