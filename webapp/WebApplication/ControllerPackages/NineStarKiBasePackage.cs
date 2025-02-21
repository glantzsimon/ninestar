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
        public NineStarKiBasePackage(ILogger logger, IDataSetsHelper datasetsHelper, IRoles roles, IFileSourceHelper fileSourceHelper, IAuthentication authentication, IMailer mailer, IRepository<User> usersRepository, IRepository<Role> rolesRepository, IRepository<UserRole> userRolesRepository, IRepository<Contact> contactsRepository, IOptions<DefaultValuesConfiguration> defaultValuesConfiguration, IOptions<SmtpConfiguration> smtpConfiguration,
            IOptions<ApiConfiguration> apiConfiguration, IOptions<WebsiteConfiguration> websiteConfiguration, IOptions<GoogleConfiguration> googleConfiguration)
        {
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
        }

        public ILogger Logger { get; set; }
        public IDataSetsHelper DataSetsHelper { get; set; }
        public IRoles Roles { get; set; }
        public IFileSourceHelper FileSourceHelper { get; set; }
        public IAuthentication Authentication { get; set; }
        public IMailer Mailer { get; set; }
        public UrlHelper UrlHelper { get; set; }

        public IRepository<User> UsersRepository { get; set; }
        public IRepository<Contact> ContactsRepository { get; set; }
        public IRepository<Role> RolesRepository { get; set; }
        public IRepository<UserRole> UserRolesRepository { get; set; }

        public DefaultValuesConfiguration DefaultValuesConfiguration { get; set; }
        public SmtpConfiguration SmtpConfiguration { get; set; }
        public ApiConfiguration ApiConfiguration { get; set; }
        public WebsiteConfiguration WebsiteConfiguration { get; set; }
        public GoogleConfiguration GoogleConfiguration { get; set; }
    }
}
