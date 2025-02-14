using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Config;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using K9.WebApplication.ViewModels;
using NLog;
using System;
using System.Collections.Generic;
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
        private readonly IRepository<UserConsultation> _userConsultationsRepository;
        private readonly IRepository<Consultation> _consultationsRepository;
        private readonly ILogger _logger;
        private readonly IConsultationService _consultationService;
        private readonly IRepository<UserRole> _userRolesRepository;
        private readonly IRepository<Contact> _contactsRepository;
        private readonly IRepository<UserMembership> _userMembershipsRepository;
        private readonly IRepository<UserOTP> _userOtpRepository;
        private readonly WebsiteConfiguration _config;
        private readonly UrlHelper _urlHelper;

        public UserService(IRepository<User> usersRepository, IRepository<PromoCode> promoCodesRepository, IRepository<UserPromoCode> userPromoCodeRepository, IAuthentication authentication, IMailer mailer, IOptions<WebsiteConfiguration> config, IContactService contactService, IRepository<UserConsultation> userConsultationsRepository, IRepository<Consultation> consultationsRepository, ILogger logger, IConsultationService consultationService,
            IRepository<UserRole> userRolesRepository, IRepository<Contact> contactsRepository, IRepository<UserMembership> userMembershipsRepository,
            IRepository<UserOTP> userOtpRepository)
        {
            _usersRepository = usersRepository;
            _promoCodesRepository = promoCodesRepository;
            _userPromoCodeRepository = userPromoCodeRepository;
            _authentication = authentication;
            _mailer = mailer;
            _contactService = contactService;
            _userConsultationsRepository = userConsultationsRepository;
            _consultationsRepository = consultationsRepository;
            _logger = logger;
            _consultationService = consultationService;
            _userRolesRepository = userRolesRepository;
            _contactsRepository = contactsRepository;
            _userMembershipsRepository = userMembershipsRepository;
            _userOtpRepository = userOtpRepository;
            _config = config.Value;
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        public void UpdateActiveUserEmailAddressIfFromFacebook(Contact contact)
        {
            if (_authentication.IsAuthenticated)
            {
                var activeUser = _usersRepository.Find(Current.UserId);
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

        public bool CheckIfPromoCodeIsUsed(string code)
        {
            var promoCode = _promoCodesRepository.Find(e => e.Code == code).FirstOrDefault();
            if (promoCode == null)
            {
                throw new Exception("Invalid promo code");
            }

            var userPromoCode = _userPromoCodeRepository.Find(e => e.PromoCodeId == promoCode.Id)
                .FirstOrDefault();
            if (userPromoCode != null)
            {
                return true;
            }

            return false;
        }

        public void UsePromoCode(int userId, string code)
        {
            var promoCode = _promoCodesRepository.Find(e => e.Code == code).FirstOrDefault();
            if (promoCode == null)
            {
                throw new Exception("Invalid promo code");
            }

            var userPromoCode = _userPromoCodeRepository.Find(e => e.PromoCodeId == promoCode.Id)
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
            var template = Dictionary.PromoCodeEmail;
            var title = Dictionary.PromoCodeEmailTitle;
            var contact = _contactService.GetOrCreateContact("", model.Name, model.EmailAddress);

            _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
            {
                Title = title,
                model.FirstName,
                model.EmailAddress,
                ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl),
                PrivacyPolicyLink = _urlHelper.AbsoluteAction("PrivacyPolicy", "Home"),
                TermsOfServiceLink = _urlHelper.AbsoluteAction("TermsOfService", "Home"),
                UnsubscribeLink = _urlHelper.AbsoluteAction("Unsubscribe", "Account", new { code = contact.Name }),
                PromoLink = _urlHelper.AbsoluteAction("Register", "Account", new { promoCode = model.PromoCode.Code }),
                PromoDetails = model.PromoCode.Details,
                DateTime.Now.Year
            }), model.EmailAddress, model.Name, _config.SupportEmailAddress, _config.CompanyName);

            model.PromoCode.SentOn = DateTime.Now;
            _promoCodesRepository.Update(model.PromoCode);
        }

        public User Find(int id)
        {
            return _usersRepository.Find(id);
        }

        public User Find(string username)
        {
            return _usersRepository.Find(username).FirstOrDefault();
        }

        public List<UserConsultation> GetPendingConsultations(int? userId = null)
        {
            var user = _usersRepository.Find(userId ?? Current.UserId);
            var userConsultations = new List<UserConsultation>();

            if (user != null)
            {
                try
                {
                    if (SessionHelper.CurrentUserIsAdmin())
                    {
                        userConsultations = _userConsultationsRepository.List();
                    }
                    else
                    {
                        userConsultations = _userConsultationsRepository.Find(e => e.UserId == user.Id);
                    }

                    foreach (var userConsultation in userConsultations)
                    {
                        userConsultation.Consultation = _consultationService.Find(userConsultation.ConsultationId);
                        userConsultation.User = _usersRepository.Find(userConsultation.UserId);
                    }
                }
                catch (Exception e)
                {
                    _logger.Error($"UserService => GetConsultations => {e.GetFullErrorMessage()}");
                }
            }

            return userConsultations.Where(e => !e.Consultation.ScheduledOn.HasValue || e.Consultation.ScheduledOn >= DateTime.Today).ToList();
        }

        public void DeleteUser(int id)
        {
            var user = Find(id);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            try
            {
                var userRoles = _userRolesRepository.Find(e => e.UserId == user.Id);
                foreach (var userRole in userRoles)
                {
                    _userRolesRepository.Delete(userRole);
                }

                var userConsultations = _userConsultationsRepository.Find(e => e.UserId == user.Id);
                foreach (var userConsultation in userConsultations)
                {
                    _userConsultationsRepository.Delete(userConsultation);
                }

                var contactRecord = _contactService.Find(user.EmailAddress);
                if (contactRecord != null)
                {
                    var consultations = _consultationsRepository.Find(e => e.ContactId == contactRecord.Id);
                    foreach (var consultation in consultations)
                    {
                        consultation.ContactId = 2; // SYSTEM
                        _consultationsRepository.Update(consultation);
                    }

                    _contactsRepository.Delete(contactRecord);
                }

                var userMemberships = _userMembershipsRepository.Find(e => e.UserId == user.Id);
                foreach (var userMembership in userMemberships)
                {
                    _userMembershipsRepository.Delete(userMembership);
                }

                var userOTPs = _userOtpRepository.Find(e => e.UserId == user.Id);
                foreach (var userOTP in userOTPs)
                {
                    _userOtpRepository.Delete(userOTP);
                }

                var userPromoCodes = _userPromoCodeRepository.Find(e => e.UserId == user.Id);
                foreach (var userPromoCode in userPromoCodes)
                {
                    _userPromoCodeRepository.Delete(userPromoCode);
                }

            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, $"UserService => DeleteUser => Error: {e.GetFullErrorMessage()}");
                throw new Exception("Error deleting user account");
            }
        }
    }
}