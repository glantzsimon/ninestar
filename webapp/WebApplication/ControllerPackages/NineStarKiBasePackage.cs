using System.Linq;
using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Config;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using NLog;
using System.Web.Mvc;

namespace K9.WebApplication.Packages
{
    public class NineStarKiBasePackage : INineStarKiBasePackage
    {
        private readonly IRepository<SystemSetting> _systemSettingsRepository;

        public NineStarKiBasePackage(ILogger logger, IDataSetsHelper datasetsHelper, IRoles roles, IFileSourceHelper fileSourceHelper, IAuthentication authentication, IMailer mailer, IRepository<User> usersRepository, IRepository<Role> rolesRepository, IRepository<UserRole> userRolesRepository, IRepository<Contact> contactsRepository, IRepository<SystemSetting> systemSettingsRepository, IOptions<DefaultValuesConfiguration> defaultValuesConfiguration, IOptions<SmtpConfiguration> smtpConfiguration,
            IOptions<ApiConfiguration> apiConfiguration, IOptions<WebsiteConfiguration> websiteConfiguration, IOptions<GoogleConfiguration> googleConfiguration)
        {
            _systemSettingsRepository = systemSettingsRepository;
            Logger = logger;
            DataSetsHelper = datasetsHelper;
            Roles = roles;
            FileSourceHelper = fileSourceHelper;
            Authentication = authentication;
            Mailer = mailer;

            UsersRepository = usersRepository;
            RolesRepository = rolesRepository;
            UserRolesRepository = userRolesRepository;
            ContactsRepository = contactsRepository;

            DefaultValuesConfiguration = defaultValuesConfiguration.Value;
            SmtpConfiguration = smtpConfiguration.Value;
            ApiConfiguration = apiConfiguration.Value;
            WebsiteConfiguration = websiteConfiguration.Value;
            GoogleConfiguration = googleConfiguration.Value;

            UrlHelper = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);

            SystemSettings = _systemSettingsRepository.List().FirstOrDefault() ?? new SystemSetting();
        }

        public ILogger Logger { get; }
        public IDataSetsHelper DataSetsHelper { get; }
        public IRoles Roles { get; }
        public IFileSourceHelper FileSourceHelper { get; }
        public IAuthentication Authentication { get; }
        public IMailer Mailer { get; }
        public UrlHelper UrlHelper { get; }

        public IRepository<User> UsersRepository { get; }
        public IRepository<Contact> ContactsRepository { get; }
        public IRepository<Role> RolesRepository { get; }
        public IRepository<UserRole> UserRolesRepository { get; }

        public DefaultValuesConfiguration DefaultValuesConfiguration { get; }
        public SmtpConfiguration SmtpConfiguration { get; }
        public ApiConfiguration ApiConfiguration { get; }
        public WebsiteConfiguration WebsiteConfiguration { get; }
        public GoogleConfiguration GoogleConfiguration { get; }

        public SystemSetting SystemSettings { get; }
    }
}
