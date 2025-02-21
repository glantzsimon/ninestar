using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.ViewModels;
using NLog;
using System;
using System.Linq;
using K9.DataAccessLaye.Attributes;
using K9.DataAccessLayer.Enums;
using K9.WebApplication.EmailTemplates;

namespace K9.WebApplication.Services
{
    public class PromotionService : BaseService, IPromotionService
    {
        private readonly IRepository<PromoCode> _promoCodesRepository;
        private readonly IRepository<UserPromoCode> _userPromoCodeRepository;
        private readonly IRepository<UserMembership> _userMembershipsRepository;
        private readonly IRepository<MembershipOption> _membershipOptionsRepository;
        private readonly IContactService _contactService;
        private readonly IEmailTemplateService _emailTemplateService;

        public PromotionService(INineStarKiBasePackage my, IRepository<PromoCode> promoCodesRepository, IRepository<UserPromoCode> userPromoCodeRepository, IRepository<UserMembership> userMembershipsRepository, IRepository<UserOTP> userOtpRepository, IRepository<MembershipOption> membershipOptionsRepository, IContactService contactService, IEmailTemplateService emailTemplateService)
            : base(my)
        {
            _promoCodesRepository = promoCodesRepository;
            _userPromoCodeRepository = userPromoCodeRepository;
            _userMembershipsRepository = userMembershipsRepository;
            _membershipOptionsRepository = membershipOptionsRepository;
            _contactService = contactService;
            _emailTemplateService = emailTemplateService;
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
            // Check if user already exists with email address
            var user = My.UsersRepository.Find(e => e.EmailAddress == model.EmailAddress).FirstOrDefault();
            if (user != null)
            {
                var errorMessage = $"PromoCodeService => SendRegistrationPromoCode => User {user.Id} is already registered";
                My.Logger.Log(LogLevel.Error, errorMessage);
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
            var title = Dictionary.PromoCodeEmailTitle;
            var body = _emailTemplateService.ParseForContact(
                title,
                Dictionary.PromoCodeOfferedEmail,
                contact,
                new
                {
                    model.FirstName,
                    model.EmailAddress,
                    PromoLink = My.UrlHelper.AbsoluteAction("Register", "Account", new { promoCode = code }),
                    PromoDetails = model.PromoCode.Details,
                    promoCode.PriceDescription,
                });

            try
            {
                My.Mailer.SendEmail(
                    title,
                    body,
                    model.EmailAddress,
                    model.Name);
            }
            catch (Exception ex)
            {
                My.Logger.Error(ex.GetFullErrorMessage());
            }

            promoCode.SentOn = DateTime.Now;
            _promoCodesRepository.Update(promoCode);
        }

        public void SendMembershipPromoCode(EmailPromoCodeViewModel model)
        {
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

            var title = Dictionary.PromoCodeEmailTitle;
            var body = _emailTemplateService.ParseForUser(
                title,
                Dictionary.PromoCodeOfferedEmail,
                user,
                new
                {
                    user.FirstName,
                    user.EmailAddress,
                    PromoLink = My.UrlHelper.AbsoluteAction("PurchaseStart", "Membership", new { membershipOptionId = model.PromoCode.MembershipOptionId, promoCode = promoCode.Code }),
                    PromoDetails = model.PromoCode.Details,
                    promoCode.PriceDescription,
                });

            try
            {
                My.Mailer.SendEmail(
                    title,
                    body,
                    user.EmailAddress,
                    user.FullName);
            }
            catch (Exception ex)
            {
                My.Logger.Error(ex.GetFullErrorMessage());
            }

            promoCode.SentOn = DateTime.Now;
            _promoCodesRepository.Update(promoCode);
        }

        public void SendFirstMembershipReminderToUser(int userId)
        {
            var promoCode = CreatePromoCodeForMembership(EDiscount.FirstDiscount);
            SendPromotionFromTemplateToUser(userId, new FirstMembershipReminderEmailTemplate(), promoCode);
        }

        public void SendSecondMembershipReminderToUser(int userId)
        {
            var promoCode = CreatePromoCodeForMembership(EDiscount.SecondDiscount);
            SendPromotionFromTemplateToUser(userId, new FirstMembershipReminderEmailTemplate(), promoCode);
        }

        public void SendThirdMembershipReminderToUser(int userId)
        {
            var promoCode = CreatePromoCodeForMembership(EDiscount.ThirdDiscount);
            SendPromotionFromTemplateToUser(userId, new FirstMembershipReminderEmailTemplate(), promoCode);
        }

        private PromoCode CreatePromoCodeForMembership(EDiscount discount)
        {
            var yearlyMembershipOption = _membershipOptionsRepository
                .Find(e => e.SubscriptionType == MembershipOption.ESubscriptionType.AnnualPlatinum).FirstOrDefault();

            if (yearlyMembershipOption == null)
            {
                throw new Exception("Yearly Membership not found on the system");
            }
            
            if (discount == EDiscount.None)
            {
                throw new Exception($"Discount cannot be zero");
            }

            var promoCode = new PromoCode
            {
                MembershipOptionId = yearlyMembershipOption.Id,
                Discount = discount
            };

            var discountAmount = discount.GetAttribute<DiscountAttribute>().DiscountPercent / 100;
            promoCode.TotalPrice = yearlyMembershipOption.Price - (yearlyMembershipOption.Price * discountAmount);
            _promoCodesRepository.Create(promoCode);

            return promoCode;
        }

        private void SendPromotionFromTemplateToUser(int userId, EmailTemplate emailTemplate, PromoCode promoCode)
        {
            promoCode = Find(promoCode.Code);
            if (promoCode == null)
            {
                throw new Exception($"PromoCodeService => SendMembershipReminderToUser => PromoCode {promoCode.Code} was not found");
            }
            if (promoCode.UsedOn.HasValue)
            {
                throw new Exception($"PromoCodeService => SendMembershipReminderToUser => PromoCode {promoCode.Code} was already used on {promoCode.UsedOn.Value}");
            }
            if (promoCode.SentOn.HasValue)
            {
                throw new Exception($"PromoCodeService => SendMembershipReminderToUser => PromoCode {promoCode.Code} was already sent on {promoCode.SentOn.Value}");
            }

            // Check if user already exists with email address
            var user = My.UsersRepository.Find(userId);
            if (user == null)
            {
                My.Logger.Log(LogLevel.Error, $"PromoCodeService => SendMembershipReminderToUser => User {userId} not found");
                throw new Exception($"The user {userId} was not found");
            }

            // Check membership option - if the user is signed up, don't send
            var activeUserMembershipIds = _userMembershipsRepository.Find(e => e.IsActive && e.UserId == userId).Select(e => e.MembershipOptionId).ToList();
            var userMembershipOptions =
                _membershipOptionsRepository.Find(e => activeUserMembershipIds.Contains(e.Id)).ToList();
            
            if (userMembershipOptions.Any(e => e.SubscriptionType >= MembershipOption.ESubscriptionType.Free))
            {
                // User is already signed up
                var errorMessage = $"PromoCodeService => SendMembershipReminderToUser => User is already signed up";
                My.Logger.Log(LogLevel.Error, errorMessage);
                return;
            }

            var body = _emailTemplateService.ParseForUser(
                emailTemplate.Subject,
                emailTemplate.HtmlBody,
                user,
                new
                {
                    user.FirstName,
                    PromoLink = My.UrlHelper.AbsoluteAction("PurchaseStart", "Membership", new { membershipOptionId = promoCode.MembershipOptionId, promoCode = promoCode.Code })
                });

            try
            {
                My.Mailer.SendEmail(
                    emailTemplate.Subject,
                    body,
                    user.EmailAddress,
                    user.FullName);
            }
            catch (Exception ex)
            {
                My.Logger.Error(ex.GetFullErrorMessage());
            }

            promoCode.SentOn = DateTime.Now;
            _promoCodesRepository.Update(promoCode);
        }

    }
}