using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Exceptions;
using K9.WebApplication.Packages;
using K9.WebApplication.ViewModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.Services
{
    public class PromotionService : BaseService, IPromotionService
    {
        private readonly IRepository<Promotion> _promotionsRepository;
        private readonly IRepository<UserPromotion> _userPromotionsRepository;
        private readonly IRepository<UserMembership> _userMembershipsRepository;
        private readonly IRepository<MembershipOption> _membershipOptionsRepository;
        private readonly IContactService _contactService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailQueueService _emailQueueService;

        public PromotionService(INineStarKiBasePackage my, IRepository<Promotion> promotionsRepository, IRepository<UserPromotion> userPromotionsRepository, IRepository<UserMembership> userMembershipsRepository, IRepository<UserOTP> userOtpRepository, IRepository<MembershipOption> membershipOptionsRepository, IContactService contactService, IEmailTemplateService emailTemplateService, IEmailQueueService emailQueueService)
            : base(my)
        {
            _promotionsRepository = promotionsRepository;
            _userPromotionsRepository = userPromotionsRepository;
            _userMembershipsRepository = userMembershipsRepository;
            _membershipOptionsRepository = membershipOptionsRepository;
            _contactService = contactService;
            _emailTemplateService = emailTemplateService;
            _emailQueueService = emailQueueService;
        }

        public Promotion Find(int id)
        {
            return _promotionsRepository.Find(id);
        }

        public Promotion FindByMembershipOption(int membershipOptionId)
        {
            return _promotionsRepository.Find(e => e.MembershipOptionId == membershipOptionId).FirstOrDefault();
        }

        public Promotion Find(string code)
        {
            var promotion = _promotionsRepository.Find(e => e.Code == code).FirstOrDefault();
            if (promotion != null && promotion.MembershipOptionId > 0)
            {
                promotion.MembershipOption = _membershipOptionsRepository.Find(promotion.MembershipOptionId);
            }
            return promotion;
        }

        public UserPromotion FindForUser(string code, int userId)
        {
            var promotion = Find(code);
            return _userPromotionsRepository.Find(e => e.PromotionId == promotion.Id && e.UserId == userId).FirstOrDefault();
        }

        public List<UserPromotion> ListForUser(int userId)
        {
            return _userPromotionsRepository.Find(e => e.UserId == userId).ToList();
        }

        public bool IsPromotionAlreadyUsed(string code, int userId)
        {
            var promotion = Find(code);
            if (promotion == null)
            {
                throw new Exception("Invalid promo code");
            }

            var userPromoCode = _userPromotionsRepository.Find(e => e.PromotionId == promotion.Id && e.UserId == userId).FirstOrDefault();
            if (userPromoCode != null)
            {
                return userPromoCode.UsedOn.HasValue;
            }

            return false;
        }

        public void UsePromotion(int userId, string code)
        {
            var promotion = Find(code);
            var userPromotion = FindForUser(code, userId);

            if (promotion == null || userPromotion == null)
            {
                if (!promotion.IsReusable)
                {
                    throw new Exception(Dictionary.InvalidPromoCode);
                }

                if (userPromotion != null)
                {
                    if (userPromotion.UsedOn.HasValue)
                        throw new Exception("Promo code has already been used");

                    userPromotion.UsedOn = DateTime.Now;
                    _userPromotionsRepository.Update(userPromotion);
                }
                else
                {
                    // Promotion is reusable - we need to create the UserPromotion record as it doesn't exist yet
                    userPromotion = new UserPromotion
                    {
                        PromotionId = promotion.Id,
                        UserId = userId,
                        SentOn = DateTime.Now,
                        Name = Guid.NewGuid().ToString()
                    };
                    _userPromotionsRepository.Create(userPromotion);
                }
            }
        }

        public void SendRegistrationPromotion(EmailPromoCodeViewModel model)
        {
            // Check if user already exists with email address
            var user = My.UsersRepository.Find(e => e.EmailAddress == model.EmailAddress).FirstOrDefault();
            var code = model.Promotion.Code;
            var promotion = Find(code);
            
            if (promotion == null)
            {
                throw new Exception($"PromoCodeService => SendRegistrationPromotion => Promotion {code} was not found");
            }

            var userPromotion = FindForUser(code, user?.Id ?? 0);
            if (userPromotion != null)
            {
                if (userPromotion.UsedOn.HasValue)
                {
                    throw new Exception($"PromoCodeService => SendRegistrationPromotion => Promotion {code} was already used on {userPromotion.UsedOn.Value}");
                }
                else
                {
                    if (userPromotion.SentOn >= DateTime.Today.Subtract(TimeSpan.FromDays(90)))
                    {
                        throw new Exception($"PromoCodeService => SendRegistrationPromotion => Promotion {promotion.Name} ({code}) has already been sent to user {user.FullName} (UserId {user.Id}) in the last 90 days");
                    }
                }
            }
            else
            {
                if (user != null)
                {
                    userPromotion = new UserPromotion
                    {
                        PromotionId = promotion.Id,
                        UserId = user.Id,
                        SentOn = DateTime.Now,
                        Name = Guid.NewGuid().ToString()
                    };
                    _userPromotionsRepository.Create(userPromotion);
                }
                else if (!promotion.IsReusable)
                {
                    throw new Exception($"PromoCodeService => SendRegistrationPromotion => Promotion {promotion.Name} ({code}) cannot be sent to email {model.EmailAddress} as the user does not exist on the system yet and the code is not reusable.");
                }
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
                    PromoDetails = promotion.MembershipOption.SubscriptionTypeNameLocal,
                    promotion.PriceDescription,
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
                throw;
            }
        }

        public void SendMembershipPromotion(EmailPromoCodeViewModel model)
        {
            var code = model.Promotion.Code;
            var promotion = Find(code);
            if (promotion == null)
            {
                throw new Exception($"PromoCodeService => SendMembershipPromoCode => PromoCode {code} was not found");
            }

            // Check if user already exists with email address
            var user = My.UsersRepository.Find(model.UserId.Value);
            if (user == null)
            {
                My.Logger.Log(LogLevel.Error, $"PromoCodeService => SendMembershipPromoCode => User {model.UserId.Value} not found");
                throw new Exception($"Cannot use this promo code. The user {model.UserId.Value} was not found");
            }

            var userPromotion = FindForUser(code, user.Id);
            if (userPromotion != null)
            {
                if (userPromotion.UsedOn.HasValue)
                {
                    throw new Exception($"PromoCodeService => SendMembershipPromotion => Promotion {promotion.Name} ({code}) was already used on {userPromotion.UsedOn.Value}");
                }
                else
                {
                    if (userPromotion.SentOn >= DateTime.Today.Subtract(TimeSpan.FromDays(90)))
                    {
                        throw new Exception($"PromoCodeService => SendMembershipPromotion => Promotion {promotion.Name} ({code}) has already been sent to user {user.FullName} (UserID: {user.Id}) in the last 90 days");
                    }
                }
            }
            else
            {
                userPromotion = new UserPromotion
                {
                    PromotionId = promotion.Id,
                    UserId = user.Id,
                    SentOn = DateTime.Now,
                    Name = Guid.NewGuid().ToString()
                };
                if (userPromotion.Id == 0)
                {
                    _userPromotionsRepository.Create(userPromotion);
                }
            }

            // Check membership option
            var membershipOption = _membershipOptionsRepository.Find(model.Promotion.MembershipOptionId);
            if (membershipOption == null)
            {
                My.Logger.Log(LogLevel.Error, $"PromoCodeService => SendMembershipPromoCode => Membership Option {promotion.MembershipOptionId} not found");
                throw new Exception($"Cannot use this promo code. The Membership Option {promotion.MembershipOptionId} was not found");
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
                    PromoLink = My.UrlHelper.AbsoluteAction("PurchaseStart", "Membership", new { membershipOptionId = model.Promotion.MembershipOptionId, promoCode = promotion.Code }),
                    PromoDetails = model.Promotion.Description,
                    promotion.PriceDescription,
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
        }

        public void SendFirstMembershipReminderToUser(int userId)
        {
            var template = _emailTemplateService.FindSystemTemplate(ESystemEmailTemplate.FirstMembershipReminder);
            var promotion = CreateOrUpdatePromotionForMembership(EDiscount.FirstDiscount, template.Name, true);
#if DEBUG
            SchedulePromotionFromTemplateToUser(userId, template, promotion, TimeSpan.FromMinutes(3));
#else
            SchedulePromotionFromTemplateToUser(userId, template, promotion, TimeSpan.FromDays(7));
#endif
        }

        public void SendSecondMembershipReminderToUser(int userId)
        {
            var template = _emailTemplateService.FindSystemTemplate(ESystemEmailTemplate.SecondMembershipReminder);
            var promotion = CreateOrUpdatePromotionForMembership(EDiscount.SecondDiscount, template.Name, true);
#if DEBUG
            SchedulePromotionFromTemplateToUser(userId, template, promotion, TimeSpan.FromMinutes(7));
#else
            SchedulePromotionFromTemplateToUser(userId, template, promotion, TimeSpan.FromDays(11));
#endif
        }

        public void SendThirdMembershipReminderToUser(int userId)
        {
            var template = _emailTemplateService.FindSystemTemplate(ESystemEmailTemplate.ThirdMembershipReminder);
            var promotion = CreateOrUpdatePromotionForMembership(EDiscount.ThirdDiscount, template.Name, true);
#if DEBUG
            SchedulePromotionFromTemplateToUser(userId, template, promotion, TimeSpan.FromMinutes(12));
#else
            SchedulePromotionFromTemplateToUser(userId, template, promotion, TimeSpan.FromDays(22));
#endif
        }

        public void SendPromotionFromTemplateToUser(int userId, EmailTemplate emailTemplate, Promotion promotion, bool isScheduled = false, TimeSpan? scheduledOn = null, bool isTest = false)
        {
            if (isScheduled && scheduledOn == null)
            {
                throw new Exception("scheduledOn must be set when scheduling an email.");
            }

            if (promotion == null)
            {
                throw new Exception($"PromoCodeService => SendPromotionFromTemplateToUser => PromoCode {promotion.Code} was not found");
            }

            promotion.MembershipOption = _membershipOptionsRepository
                                             .Find(e => e.Id == promotion.MembershipOptionId).FirstOrDefault() ?? throw new Exception($"Membership {promotion.MembershipOptionId} not found on the system");

            // Check if user exists 
            var user = My.UsersRepository.Find(userId);
            if (user == null)
            {
                My.Logger.Log(LogLevel.Error, $"PromoCodeService => SendMembershipReminderToUser => User {userId} not found");
                throw new Exception($"The user {userId} was not found");
            }

            // Check membership option - if the user is signed up, don't send
            var activeUserMembershipIds = _userMembershipsRepository.Find(e => e.UserId == userId)
                .Where(e => e.IsActive)
                .Select(e => e.MembershipOptionId)
                .ToList();
            var userMembershipOptions =
                _membershipOptionsRepository.Find(e => activeUserMembershipIds.Contains(e.Id)).ToList();

            if (!isTest && userMembershipOptions.Any(e => e.SubscriptionType > MembershipOption.ESubscriptionType.Free))
            {
                // User is already signed up
                var errorMessage = $"PromoCodeService => SendMembershipReminderToUser => User is already signed up";
                My.Logger.Log(LogLevel.Error, errorMessage);
                throw new UserAlreadySubscribedException();
            }

            var userPromotion = FindForUser(promotion.Code, userId);
            if (userPromotion != null)
            {
                if (!isTest && userPromotion.UsedOn.HasValue)
                {
                    throw new Exception($"PromoCodeService => SendPromotionFromTemplateToUser => Promotion {promotion.Code} was already used on {userPromotion.UsedOn.Value}");
                }
                else
                {
                    if (!isTest && userPromotion.SentOn >= DateTime.Today.Subtract(TimeSpan.FromDays(90)))
                    {
                        throw new Exception($"PromoCodeService => SendPromotionFromTemplateToUser => Promotion {promotion.Name} ({promotion.Code}) has already been sent to user {user.FullName} (UserId: {userId}) in the last 90 days");
                    }
                }
            }
            else
            {
                userPromotion = new UserPromotion
                {
                    PromotionId = promotion.Id,
                    UserId = userId,
                    SentOn = DateTime.Now,
                    Name = Guid.NewGuid().ToString()
                };
            }

            promotion.PromoLink = My.UrlHelper.AbsoluteAction("PurchaseStart", "Membership",
                new { membershipOptionId = promotion.MembershipOptionId, promoCode = promotion.Code });

            var data = new
            {
                user.FirstName,
                promotion.DiscountPercent,
                promotion.DiscountPercentText,
                promotion.DiscountPercentHtmlText,
                promotion.FormattedFullPrice,
                promotion.FormattedSpecialPrice,
                promotion.MembershipName,
                promotion.PromoLink
            };
            var subject = TemplateParser.Parse(emailTemplate.Subject, data);
            var body = _emailTemplateService.ParseForUser(
                emailTemplate,
                user,
                data);

            try
            {
                if (isScheduled)
                {
                    _emailQueueService.AddEmailToQueueForUser(emailTemplate.Id, user.Id, subject, body, EEmailType.MembershipPromotion, scheduledOn);
                }
                else
                {
                    My.Mailer.SendEmail(
                        subject,
                        body,
                        user.EmailAddress,
                        user.FullName);
                }

                if (userPromotion.Id == 0)
                {
                    _userPromotionsRepository.Create(userPromotion);
                }
            }
            catch (Exception ex)
            {
                My.Logger.Error(ex.GetFullErrorMessage());
                throw new Exception($"PromoCodeService => SendPromotionFromTemplateToUser failed => {ex.GetFullErrorMessage()}");
            }
        }

        private Promotion CreateOrUpdatePromotionForMembership(EDiscount discount, string name, bool updateIfExists = false)
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

            var promotion = _promotionsRepository
                .Find(e => e.MembershipOptionId == yearlyMembershipOption.Id &&
                           e.Discount == discount &&
                           e.Name == name).FirstOrDefault();

            if (promotion == null)
            {
                promotion = new Promotion
                {
                    MembershipOptionId = yearlyMembershipOption.Id,
                    Discount = discount,
                    Name = name
                };
                promotion.SpecialPrice = promotion.GetSpecialPrice(yearlyMembershipOption.Price);

                _promotionsRepository.Create(promotion);
            }
            else if (updateIfExists)
            {
                promotion.Name = name;
                promotion.Discount = discount;
                promotion.SpecialPrice = Math.Ceiling(promotion.GetSpecialPrice(yearlyMembershipOption.Price));

                _promotionsRepository.Update(promotion);
            };

            return promotion;
        }

        private void SchedulePromotionFromTemplateToUser(int userId, EmailTemplate emailTemplate, Promotion promotion,
            TimeSpan scheduledOn)
        {
            try
            {
                SendPromotionFromTemplateToUser(userId, emailTemplate, promotion, true, scheduledOn);
            }
            catch (UserAlreadySubscribedException e)
            {
                // Do nothing
            }
            catch (Exception e)
            {
                My.Logger.Error($"PromotionService => SchedulePromotionFromTemplateToUser => {e.GetFullErrorMessage()}");
                throw;
            }
        }
    }
}