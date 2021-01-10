using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Config;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.ViewModels;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace K9.WebApplication.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _usersRepository;
        private readonly IRepository<PromoCode> _promoCodesRepository;
        private readonly IRepository<UserPromoCode> _userPromoCodeRepository;
        private readonly IAuthentication _authentication;
        private readonly IMailer _mailer;
        private readonly IContactService _contactService;
        private readonly WebsiteConfiguration _config;
        private readonly UrlHelper _urlHelper;

        public UserService(IRepository<User> usersRepository, IRepository<PromoCode> promoCodesRepository, IRepository<UserPromoCode> userPromoCodeRepository, IAuthentication authentication, IMailer mailer, IOptions<WebsiteConfiguration> config, IContactService contactService)
        {
            _usersRepository = usersRepository;
            _promoCodesRepository = promoCodesRepository;
            _userPromoCodeRepository = userPromoCodeRepository;
            _authentication = authentication;
            _mailer = mailer;
            _contactService = contactService;
            _config = config.Value;
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        public void UpdateActiveUserEmailAddressIfFromFacebook(Contact contact)
        {
            if (_authentication.IsAuthenticated)
            {
                var activeUser = _usersRepository.Find(_authentication.CurrentUserId);
                var defaultFacebookAddress = $"{activeUser.FirstName}.{activeUser.LastName}@facebook.com";
                if (activeUser.IsOAuth && activeUser.EmailAddress == defaultFacebookAddress && activeUser.EmailAddress != contact.EmailAddress)
                {
                    if (!_usersRepository.Find(e => e.EmailAddress == contact.EmailAddress).Any())
                    {
                        activeUser.EmailAddress = contact.EmailAddress;
                        _usersRepository.Update(activeUser);
                    }
                }
            }
        }

        public void CheckPromoCode(int userId, string code)
        {
            var promoCode = _promoCodesRepository.Find(e => e.Code == code).FirstOrDefault();
            if (promoCode == null)
            {
                throw new Exception("Invalid promo code");
            }

            var userPromoCode = _userPromoCodeRepository.Find(e => e.UserId == userId && e.PromoCodeId == promoCode.Id)
                .FirstOrDefault();
            if (userPromoCode != null)
            {
                throw new Exception("Promo code has already been used");
            }

            var newUserPromo = new UserPromoCode
            {
                UserId = userId,
                PromoCodeId = promoCode.Id
            };

            _userPromoCodeRepository.Create(newUserPromo);

            promoCode.UsedOn = DateTime.Now;
            _promoCodesRepository.Update(promoCode);
        }

        public void SendPromoCode(EmailPromoCodeViewModel model)
        {
            var template = Dictionary.PromoCode;
            var title = Dictionary.PromoCode;
            var contact = _contactService.GetOrCreateContact("", model.Name, model.EmailAddress);
            
            _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
            {
                Title = title,
                model.FirstName,
                model.EmailAddress,
                ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl),
                PrivacyPolicyLink = _urlHelper.AbsoluteAction("PrivacyPolicy", "Home"),
                UnsubscribeLink = _urlHelper.AbsoluteAction("Unsubscribe", "Account", new { code = contact.Name }),
                PromoLink = _urlHelper.AbsoluteAction("Register", "Account", new { promoCode = model.PromoCode.Code }),
                PromoDetails = model.PromoCode.Details,
                DateTime.Now.Year
            }), model.EmailAddress, model.Name, _config.SupportEmailAddress, _config.CompanyName);

            model.PromoCode.SentOn = DateTime.Now;
            _promoCodesRepository.Update(model.PromoCode);
        }
    }
}