using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.ViewModels;
using NLog;
using System;
using System.Linq;

namespace K9.WebApplication.Services
{
    public class PromoCodeService : BaseService, IPromoCodeService
    {
        private readonly IRepository<PromoCode> _promoCodesRepository;
        private readonly IRepository<UserPromoCode> _userPromoCodeRepository;
        private readonly IContactService _contactService;
        private readonly IRepository<UserMembership> _userMembershipsRepository;
        private readonly IRepository<MembershipOption> _membershipOptionsRepository;

        public PromoCodeService(INineStarKiBasePackage my, IRepository<PromoCode> promoCodesRepository, IRepository<UserPromoCode> userPromoCodeRepository, IRepository<UserMembership> userMembershipsRepository, IRepository<UserOTP> userOtpRepository, IRepository<MembershipOption> membershipOptionsRepository)
            : base(my)
        {
            _promoCodesRepository = promoCodesRepository;
            _userPromoCodeRepository = userPromoCodeRepository;
            _userMembershipsRepository = userMembershipsRepository;
            _membershipOptionsRepository = membershipOptionsRepository;
        }

        public PromoCode Find(string code)
        {
            return _promoCodesRepository.Find(e => e.Code == code).FirstOrDefault();
        }

        public bool IsPromoCodeAlreadyUsed(string code)
        {
            var promoCode = Find(code);
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
            var promoCode = Find(code);
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

        public void SendRegistrationPromoCode(EmailPromoCodeViewModel model)
        {
            var template = Dictionary.PromoCodeEmail;
            var title = Dictionary.PromoCodeEmailTitle;

            // Check if user already exists with email address
            var user = My.UsersRepository.Find(e => e.EmailAddress == model.EmailAddress).FirstOrDefault();
            if (user != null)
            {
                My.Logger.Log(LogLevel.Error, $"PromoCodeService => SendRegistrationPromoCode => User {user.Id} is already registered");
                throw new Exception("Cannot use this promo code. The user is already registered on the system");
            }

            var code = model.PromoCode.Code;
            var promoCode = Find(code);
            if (promoCode == null)
            {
                throw new Exception($"PromoCodeService => SendRegistrationPromoCode => PromoCode {code} was not found");
            }
            if (promoCode.UsedOn.HasValue)
            {
                throw new Exception($"PromoCodeService => SendRegistrationPromoCode => PromoCode {code} was already used on {promoCode.UsedOn.Value}");
            }
            if (promoCode.SentOn.HasValue)
            {
                throw new Exception($"PromoCodeService => SendRegistrationPromoCode => PromoCode {code} was already sent on {promoCode.SentOn.Value}");
            }

            var contact = _contactService.GetOrCreateContact("", model.Name, model.EmailAddress);

            My.Mailer.SendEmail(title, TemplateParser.Parse(template, new
            {
                Title = title,
                model.FirstName,
                model.EmailAddress,
                ImageUrl = My.UrlHelper.AbsoluteContent(My.WebsiteConfiguration.CompanyLogoUrl),
                PrivacyPolicyLink = My.UrlHelper.AbsoluteAction("PrivacyPolicy", "Home"),
                TermsOfServiceLink = My.UrlHelper.AbsoluteAction("TermsOfService", "Home"),
                UnsubscribeLink = My.UrlHelper.AbsoluteAction("UnsubscribeContact", "Account", new { externalId = contact.Name }),
                PromoLink = My.UrlHelper.AbsoluteAction("Register", "Account", new { promoCode = code }),
                PromoDetails = model.PromoCode.Details,
                promoCode.PriceDescription,
                DateTime.Now.Year
            }), model.EmailAddress, model.Name, My.WebsiteConfiguration.SupportEmailAddress, My.WebsiteConfiguration.CompanyName);


            promoCode.SentOn = DateTime.Now;
            _promoCodesRepository.Update(promoCode);
        }

        public void SendMembershipPromoCode(EmailPromoCodeViewModel model)
        {
            var template = Dictionary.PromoCodeEmail;
            var title = Dictionary.PromoCodeEmailTitle;

            var code = model.PromoCode.Code;
            var promoCode = Find(code);
            if (promoCode == null)
            {
                throw new Exception($"PromoCodeService => SendMembershipPromoCode => PromoCode {code} was not found");
            }
            if (promoCode.UsedOn.HasValue)
            {
                throw new Exception($"PromoCodeService => SendMembershipPromoCode => PromoCode {code} was already used on {promoCode.UsedOn.Value}");
            }
            if (promoCode.SentOn.HasValue)
            {
                throw new Exception($"PromoCodeService => SendMembershipPromoCode => PromoCode {code} was already sent on {promoCode.SentOn.Value}");
            }

            // Check if user already exists with email address
            var user = My.UsersRepository.Find(model.UserId.Value);
            if (user == null)
            {
                My.Logger.Log(LogLevel.Error, $"PromoCodeService => SendMembershipPromoCode => User {model.UserId.Value} not found");
                throw new Exception($"Cannot use this promo code. The user {model.UserId.Value} was not found");
            }

            // Check membership option
            var membershipOption = _membershipOptionsRepository.Find(model.PromoCode.MembershipOptionId);
            if (membershipOption == null)
            {
                My.Logger.Log(LogLevel.Error, $"PromoCodeService => SendMembershipPromoCode => Membership Option {promoCode.MembershipOptionId} not found");
                throw new Exception($"Cannot use this promo code. The Membership Option {promoCode.MembershipOptionId} was not found");
            }

            My.Mailer.SendEmail(title, TemplateParser.Parse(template, new
            {
                Title = title,
                user.FirstName,
                user.EmailAddress,
                ImageUrl = My.UrlHelper.AbsoluteContent(My.WebsiteConfiguration.CompanyLogoUrl),
                PrivacyPolicyLink = My.UrlHelper.AbsoluteAction("PrivacyPolicy", "Home"),
                TermsOfServiceLink = My.UrlHelper.AbsoluteAction("TermsOfService", "Home"),
                UnsubscribeLink = My.UrlHelper.AbsoluteAction("UnsubscribeUser", "Account", new { externalId = user.Name }),
                PromoLink = My.UrlHelper.AbsoluteAction("PurchaseStart", "Membership", new { membershipOptionId = model.PromoCode.MembershipOptionId, promoCode = promoCode.Code }),
                PromoDetails = model.PromoCode.Details,
                promoCode.PriceDescription,
                DateTime.Now.Year
            }), user.EmailAddress, user.Name, My.WebsiteConfiguration.SupportEmailAddress, My.WebsiteConfiguration.CompanyName);

            promoCode.SentOn = DateTime.Now;
            _promoCodesRepository.Update(promoCode);
        }

    }
}