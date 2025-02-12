using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Config;
using K9.Base.WebApplication.Services;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Services;
using NLog;

namespace K9.WebApplication.Controllers
{
    public class CssController : BaseNineStarKiController
    {
        private readonly IRepository<User> _userRepository;
        private readonly ILogger _logger;
        private readonly IAccountService _accountService;
        private readonly IAuthentication _authentication;
        private readonly IFacebookService _facebookService;
        private readonly IMembershipService _membershipService;
        private readonly IContactService _contactService;
        private readonly IUserService _userService;
        private readonly IRepository<PromoCode> _promoCodesRepository;
        private readonly IRecaptchaService _recaptchaService;
        private readonly RecaptchaConfiguration _recaptchaConfig;

        public CssController(IRepository<User> userRepository, ILogger logger, IMailer mailer, IOptions<WebsiteConfiguration> websiteConfig, IDataSetsHelper dataSetsHelper, IRoles roles, IAccountService accountService, IAuthentication authentication, IFileSourceHelper fileSourceHelper, IFacebookService facebookService, IMembershipService membershipService, IContactService contactService, IUserService userService, IRepository<PromoCode> promoCodesRepository, IOptions<RecaptchaConfiguration> recaptchaConfig, IRecaptchaService recaptchaService, IRepository<Role> rolesRepository, IRepository<UserRole> userRolesRepository)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService, rolesRepository, userRolesRepository)
        {
            _userRepository = userRepository;
            _logger = logger;
            _accountService = accountService;
            _authentication = authentication;
            _facebookService = facebookService;
            _membershipService = membershipService;
            _contactService = contactService;
            _userService = userService;
            _promoCodesRepository = promoCodesRepository;
            _recaptchaService = recaptchaService;
            _recaptchaConfig = recaptchaConfig.Value;

            websiteConfig.Value.RegistrationEmailTemplateText = Globalisation.Dictionary.WelcomeEmail;
            websiteConfig.Value.PasswordResetEmailTemplateText = Globalisation.Dictionary.PasswordResetEmail;
        }
    
    }
}