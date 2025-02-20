using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Config;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Services;
using NLog;

namespace K9.WebApplication.Packages
{
    public class NineStarKiPackage : NineStarKiBasePackage, INineStarKiPackage
    {
        public NineStarKiPackage(ILogger logger, IDataSetsHelper datasetsHelper, IRoles roles, IFileSourceHelper fileSourceHelper, IAuthentication authentication, IMailer mailer, IMembershipService membershipService, IAccountService accountService, IUserService userService,
           IContactService contactService, IRepository<User> usersRepository, IRepository<Role> rolesRepository, IRepository<UserRole> userRolesRepository, IRepository<Contact> contactsRepository, IOptions<DefaultValuesConfiguration> defaultValuesConfiguration, IOptions<SmtpConfiguration> smtpConfiguration,
            IOptions<ApiConfiguration> apiConfiguration, IOptions<WebsiteConfiguration> websiteConfiguration, IOptions<GoogleConfiguration> googleConfiguration)
        : base(logger, datasetsHelper, roles, fileSourceHelper, authentication, mailer, usersRepository, rolesRepository, userRolesRepository, contactsRepository,
            defaultValuesConfiguration, smtpConfiguration, apiConfiguration, websiteConfiguration, googleConfiguration)
        {
            MembershipService = membershipService;
            AccountService = accountService;
            UserService = userService;
            ContactService = contactService;
        }
        
        public IMembershipService MembershipService { get; set; }
        public IAccountService AccountService { get; set; }
        public IUserService UserService { get; set; }
        public IContactService ContactService { get; set; }
        
    }
}
