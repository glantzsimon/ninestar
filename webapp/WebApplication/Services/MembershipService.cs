using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Helpers;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Exceptions;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.Services
{
    public class MembershipService : BaseService, IMembershipService
    {
        private readonly IRepository<MembershipOption> _membershipOptionRepository;
        private readonly IRepository<UserMembership> _userMembershipRepository;
        private readonly IRepository<Promotion> _promotionsRepository;
        private readonly IRepository<Consultation> _consultationsRepository;
        private readonly IRepository<UserConsultation> _userConsultationsRepository;
        private readonly IConsultationService _consultationService;
        private readonly IPromotionService _promotionService;
        private readonly IContactService _contactService;
        private readonly IEmailTemplateService _emailTemplateService;

        public MembershipService(INineStarKiBasePackage my, IRepository<MembershipOption> membershipOptionRepository, IRepository<UserMembership> userMembershipRepository, IRepository<Promotion> promotionsRepository, IRepository<Consultation> consultationsRepository, IRepository<UserConsultation> userConsultationsRepository, IConsultationService consultationService, IPromotionService promotionService, IContactService contactService,
            IEmailTemplateService emailTemplateService)
            : base(my)
        {
            _membershipOptionRepository = membershipOptionRepository;
            _userMembershipRepository = userMembershipRepository;
            _promotionsRepository = promotionsRepository;
            _consultationsRepository = consultationsRepository;
            _userConsultationsRepository = userConsultationsRepository;
            _consultationService = consultationService;
            _promotionService = promotionService;
            _contactService = contactService;
            _emailTemplateService = emailTemplateService;
        }

        public UserMembership GetActiveUserMembership(string accountNumber)
        {
            var membershipIds =
                _userMembershipRepository.CustomQuery<string>($"SELECT [{nameof(DataAccessLayer.Models.UserMembership.Name)}] FROM {nameof(UserMembership)}");

            var matching = membershipIds.FirstOrDefault(e => e.ToSixDigitCode() == accountNumber);

            if (matching != null)
            {
                var userMembership = _userMembershipRepository.Find(e => e.Name == matching).FirstOrDefault();
                if (userMembership != null)
                {
                    return GetActiveUserMembership(userMembership.UserId);
                }
            }

            return null;
        }

        public UserMembershipViewModel GetMembershipViewModel(int? userId = null)
        {
            userId = userId ?? Current.UserId;
            var membershipOptions = _membershipOptionRepository.Find(e => !e.IsDeleted).ToList();
            var activeUserMembership = userId.HasValue ? GetActiveUserMembership(userId) : null;

            return new UserMembershipViewModel
            {
                MembershipModels = new List<MembershipModel>(membershipOptions
                    .Where(e => !e.IsDeleted)
                    .OrderBy(e => e.SubscriptionType).ToList()
                    .Select(membershipOption =>
                {
                    var isSubscribed = activeUserMembership != null && activeUserMembership.MembershipOptionId == membershipOption.Id;
                    var isUpgradable = activeUserMembership == null || activeUserMembership.MembershipOption.CanUpgradeTo(membershipOption);

                    return new MembershipModel(Current.UserId, membershipOption, activeUserMembership)
                    {
                        IsSelectable = !isSubscribed && isUpgradable,
                        IsSubscribed = isSubscribed
                    };
                }))
            };
        }

        public List<UserMembership> GetActiveUserMemberships(int userId, bool includeScheduled = false)
        {
            var membershipOptions = _membershipOptionRepository.List();
            var activeMemberships = _userMembershipRepository.Find(_ => _.UserId == userId).ToList()
                .Where(_ => _.IsActive || includeScheduled && _.EndsOn > DateTime.Today);
            var userMemberships = activeMemberships.Select(userMembership =>
                {
                    userMembership.MembershipOption =
                        membershipOptions.FirstOrDefault(m => m.Id == userMembership.MembershipOptionId);
                    return userMembership;
                }).ToList();

            return userMemberships;
        }

        /// <summary>
        /// Sometimes user memberships can overlap, when upgrading for example. This returns the Active membership.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserMembership GetActiveUserMembership(int? userId = null)
        {
            userId = userId ?? Current.UserId;
            var activeUserMembership = GetActiveUserMemberships(userId.Value).OrderByDescending(_ => _.MembershipOption?.SubscriptionType)
                .FirstOrDefault();

            if (activeUserMembership == null && userId.HasValue)
            {
                try
                {
                    if (My.UsersRepository.Find(userId.Value) != null)
                    {
                        CreateFreeMembership(userId.Value);
                    }
                    activeUserMembership = GetActiveUserMemberships(userId.Value).OrderByDescending(_ => _.MembershipOption.SubscriptionType)
                        .FirstOrDefault();
                }
                catch (Exception e)
                {
                    My.Logger.Error($"MembershipService => GetActiveUserMembership => {e.GetFullErrorMessage()}");
                }
            }
            else
            {
                activeUserMembership.User = My.UsersRepository.Find(activeUserMembership.UserId);
            }

            return activeUserMembership;
        }

        public MembershipModel GetSwitchMembershipModel(int membershipOptionId, int? userId = null)
        {
            userId = userId.HasValue ? userId : Current.UserId;

            var userMemberships = GetActiveUserMemberships(userId.Value);
            if (!userMemberships.Any())
            {
                throw new Exception(Dictionary.SwitchMembershipErrorNotSubscribed);
            }

            ValidateUpgrade(userId.Value, membershipOptionId);

            var activeUserMembership = GetActiveUserMembership(userId);
            var membershipOption = _membershipOptionRepository.Find(membershipOptionId);

            return new MembershipModel(userId.Value, membershipOption, activeUserMembership)
            {
                IsSelected = true
            };
        }

        private void ValidateUpgrade(int userId, int membershipOptionId)
        {
            var activeUserMembership = GetActiveUserMembership(userId);
            if (activeUserMembership.MembershipOptionId == membershipOptionId)
            {
                throw new UserAlreadySubscribedException();
            }

            var membershipOption = _membershipOptionRepository.Find(membershipOptionId);
            if (!activeUserMembership.MembershipOption.CanUpgradeTo(membershipOption))
            {
                throw new UpgradeNotPossibleException();
            }
        }

        public MembershipModel GetPurchaseMembershipModel(int membershipOptionId, string promoCode = "")
        {
            ValidateUpgrade(Current.UserId, membershipOptionId);
            Promotion promotionModel = null;

            if (!string.IsNullOrEmpty(promoCode))
            {
                try
                {
                    promotionModel = _promotionsRepository.Find(e => e.Code == promoCode).FirstOrDefault();
                    if (promotionModel == null)
                    {
                        var errorMessage =
                            $"MembershipService => GetPurchaseMembershipModel => Invalid Promo Code: {promoCode}";
                        My.Logger.Error(errorMessage);
                        throw new Exception(errorMessage);
                    }
                }
                catch (Exception e)
                {
                    var errorMessage =
                        $"MembershipService => GetPurchaseMembershipModel => Error: {e.GetFullErrorMessage()}";
                    My.Logger.Error(errorMessage);
                    throw;
                }
            }

            var membershipOption = _membershipOptionRepository.Find(membershipOptionId);
            if (promotionModel != null)
            {
                promotionModel.MembershipOption = membershipOption;
                membershipOption.Promotion = promotionModel;
            }

            return new MembershipModel(Current.UserId, membershipOption, null, promotionModel)
            {
                IsSelected = true,
            };
        }

        public bool CreateMembershipFromPromoCode(int userId, string code)
        {
            var promotion = _promotionsRepository.Find(e => e.Code == code).FirstOrDefault();
            if (promotion == null)
            {
                My.Logger.Error($"MembershipService => ProcessPurchaseWithPromoCode => Invalid Promo Code");
                throw new Exception("Invalid promo code");
            }

            var membershipOption = _membershipOptionRepository.Find(e => e.Id == promotion.MembershipOptionId).FirstOrDefault();
            if (membershipOption == null)
            {
                My.Logger.Error($"MembershipService => ProcessPurchaseWithPromoCode => No MembershipOption of type {promotion.SubscriptionTypeName} found");
                throw new Exception($"No Membership Option of type {promotion.SubscriptionTypeName} found");
            }

            var activeMembership = GetActiveUserMembership(userId);
            var user = My.UsersRepository.Find(userId);

            if (activeMembership.MembershipOption.SubscriptionType > MembershipOption.ESubscriptionType.Free)
            {
                try
                {
                    ValidateUpgrade(membershipOption.Id, userId);
                }
                catch (Exception e)
                {
                    My.Logger.Error($"MembershipService => CreateMembershipFromPromoCode => ValidateSwitch Failed => {e.GetFullErrorMessage()}");
                    throw;
                }
            }

            // If they have to pay something, return false, which redirects them to the payment page
            if (promotion.SpecialPrice > 0)
            {
                return false;
            }

            CreateMembership(membershipOption.Id, user.FullName, user.EmailAddress);
            _promotionService.UsePromotion(user.Id, code);

            return true;
        }

        public void ProcessPurchase(PurchaseModel purchaseModel)
        {
            CreateMembership(purchaseModel.ItemId, purchaseModel.CustomerName, purchaseModel.CustomerEmailAddress);
        }

        public void CreateMembership(int membershipOptionId, string customerName, string customerEmailAddress)
        {
            UserMembership userMembership = null;
            MembershipOption membershipOption = null;

            try
            {
                membershipOption = _membershipOptionRepository.Find(membershipOptionId);

                if (membershipOption == null)
                {
                    My.Logger.Error(
                        $"MembershipService => CreateMembership => No MembershipOption with id {membershipOptionId} was found.");
                    throw new IndexOutOfRangeException("Invalid MembershipOptionId");
                }

                userMembership = new UserMembership
                {
                    UserId = Current.UserId,
                    MembershipOptionId = membershipOptionId,
                    StartsOn = DateTime.Today,
                    IsAutoRenew = true
                };
                SetMembershipEndDate(userMembership);

                TerminateExistingMemberships(userMembership.UserId);

                _userMembershipRepository.Create(userMembership);
                userMembership.User = My.UsersRepository.Find(Current.UserId);

                if (membershipOption.SubscriptionType == MembershipOption.ESubscriptionType.LifeTimePlatinum)
                {
                    CreateComplementaryUserConsultation(Current.UserId);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"MembershipService => CreateMembership => Purchase failed: {ex.GetFullErrorMessage()}";
                My.Logger.Error(errorMessage);
                SendEmailToNineStarAboutSubscriptionFailure(customerName, customerEmailAddress, errorMessage);
                throw;
            }

            var user = userMembership.User;
            try
            {
                var contact = _contactService.Find(customerEmailAddress);
                if (contact == null)
                {
                    contact = _contactService.GetOrCreateContact("", customerName, customerEmailAddress, "",
                        user.Id);
                }
            }
            catch (Exception e)
            {
                var errorMessage = $"MembershipService => ProcessPurchase => Get contact record failed failed for user: {user.Id} {e.GetFullErrorMessage()}";
                My.Logger.Error(errorMessage);
                SendEmailToNineStarAboutSubscriptionFailure(customerName, customerEmailAddress, errorMessage);
            }

            try
            {
                userMembership.MembershipOption = membershipOption;
                SendEmailToNineStar(customerName, customerEmailAddress, userMembership);
                SendEmailToUser(userMembership);
            }
            catch (Exception e)
            {
                var errorMessage = $"MembershipService => ProcessPurchase => Send Emails failed: {e.GetFullErrorMessage()}";
                My.Logger.Error(errorMessage);

                SendEmailToNineStarAboutSubscriptionFailure(customerName, customerEmailAddress, errorMessage);
            }
        }

        private void SetMembershipEndDate(UserMembership membership)
        {
            var membershipOption = _membershipOptionRepository.Find(e => e.Id == membership.MembershipOptionId)
                    .FirstOrDefault();

            if (membershipOption.IsWeekly)
            {
                membership.EndsOn = membership.StartsOn.AddDays(7);
            }
            else if (membershipOption.IsMonthly)
            {
                membership.EndsOn = membership.StartsOn.AddMonths(1);
            }
            else if (membershipOption.IsAnnual)
            {
                membership.EndsOn = membership.StartsOn.AddYears(1);
            }
            else if (membershipOption.IsForever)
            {
                membership.EndsOn = DateTime.MaxValue;
            }
        }

        public void AssignMembershipToUser(int membershipOptionId, int userId, Promotion promotion = null)
        {
            try
            {
                var membershipOption = _membershipOptionRepository.Find(membershipOptionId);
                if (membershipOption == null)
                {
                    My.Logger.Error($"MembershipService => AssignMembershipToUser => No MembershipOption with id {membershipOptionId} was found.");
                    throw new IndexOutOfRangeException("Invalid MembershipOptionId");
                }

                var userMembership = new UserMembership
                {
                    UserId = userId,
                    MembershipOptionId = membershipOptionId,
                    StartsOn = DateTime.Today,
                    IsAutoRenew = true
                };
                SetMembershipEndDate(userMembership);

                TerminateExistingMemberships(userId);

                _userMembershipRepository.Create(userMembership);
                userMembership.User = My.UsersRepository.Find(userId);
            }
            catch (Exception ex)
            {
                My.Logger.Error($"MembershipService => AssignMembershipToUser => Assign Membership failed: {ex.GetFullErrorMessage()}");
                throw;
            }
        }

        public void CreateFreeMembership(int userId)
        {
            try
            {
                var membershipOption = _membershipOptionRepository.Find(e => e.SubscriptionType == MembershipOption.ESubscriptionType.Free).FirstOrDefault();

                if (membershipOption == null)
                {
                    My.Logger.Error($"MembershipService => CreateFreeMembership => MembershipOption with Subscription Type {MembershipOption.ESubscriptionType.Free} was not found.");
                    return;
                }

                if (My.UsersRepository.Find(userId) == null)
                {
                    My.Logger.Error($"MembershipService => CreateFreeMembership => UserId {userId} was not found.");
                    return;
                }

                _userMembershipRepository.Create(new UserMembership
                {
                    UserId = userId,
                    MembershipOptionId = membershipOption.Id,
                    StartsOn = DateTime.Today,
                    EndsOn = DateTime.MaxValue,
                    IsAutoRenew = true
                });
            }
            catch (Exception ex)
            {
                var errorMessage = $"MembershipService => CreateFreeMembership => Error: {ex.GetFullErrorMessage()}";
                My.Logger.Error(errorMessage);

                var user = My.UsersRepository.Find(userId);
                SendEmailToNineStarAboutFailure(errorMessage, userId);

                throw;
            }
        }

        public void CreateComplementaryUserConsultation(int userId, EConsultationDuration duration = EConsultationDuration.OneHour)
        {
            var user = My.UsersRepository.Find(userId);
            var contact = _contactService.Find(user.EmailAddress);

            if (contact == null)
            {
                contact = _contactService.GetOrCreateContact("", user.FullName, user.EmailAddress, user.PhoneNumber,
                    user.Id);
            }

            var consultation = new Consultation
            {
                ContactId = contact.Id,
                ConsultationDuration = duration,
                ContactName = contact.FullName
            };

            try
            {
                _consultationService.CreateConsultation(consultation, contact, userId, true);
            }
            catch (Exception e)
            {
                var errorMessage = $"MembershipService => CreateComplementaryUserConsultation => Error creating consultation => {e.GetFullErrorMessage()}";
                My.Logger.Error(errorMessage);
                SendEmailToNineStarAboutFailure(errorMessage, userId);
            }
        }

        public void ScheduleRemindersForUser(int userId)
        {
            if (My.SystemSettings.IsSendMembershipUpgradeReminders)
            {
                _promotionService.SendFirstMembershipReminderToUser(userId);
                _promotionService.SendSecondMembershipReminderToUser(userId);
                _promotionService.SendThirdMembershipReminderToUser(userId);
            }
        }

        private void TerminateExistingMemberships(int userId)
        {
            var userMemberships = GetActiveUserMemberships(userId);
            var activeUserMemberships = userMemberships.Where(e => e.IsActive).ToList();
            if (!activeUserMemberships.Any())
            {
                My.Logger.Error($"MembershipService => TerminateExistingMemberships => No active memberships");
                return;
            }
            foreach (var userMembership in activeUserMemberships)
            {
                userMembership.EndsOn = DateTime.Now;
                userMembership.IsDeactivated = true;
                _userMembershipRepository.Update(userMembership);
            }
        }

        private void SendEmailToNineStar(string customerName, string customerEmailAddress, UserMembership userMembership)
        {
            var user = My.UsersRepository.Find(userMembership.UserId);
            var title = "We have received a new subscription!";
            var body = _emailTemplateService.ParseForUser(
                title,
                Dictionary.NewSubscriptionEmail,
                user,
                new
                {
                    Customer = customerName,
                    CustomerEmail = customerEmailAddress,
                    SubscriptionType = userMembership.MembershipOption.SubscriptionTypeNameLocal,
                    TotalPrice = userMembership.MembershipOption.FormattedPrice,
                    LinkToSummary = My.UrlHelper.AbsoluteAction("Index", "UserMemberships"),
                });

            try
            {
                My.Mailer.SendEmail(
                    title,
                    body,
                    My.WebsiteConfiguration.SupportEmailAddress,
                    My.WebsiteConfiguration.CompanyName);
            }
            catch (Exception ex)
            {
                My.Logger.Error(ex.GetFullErrorMessage());
            }
        }

        private void SendEmailToUser(UserMembership userMembership)
        {
            var user = My.UsersRepository.Find(userMembership.UserId);
            var userPromotions = _promotionService.ListForUser(userMembership.UserId)
                .Where(e => !e.UsedOn.HasValue)
                .Select(e => e.PromotionId)
                .ToList();
            var promotion = _promotionsRepository.Find(e =>
                    userPromotions.Contains(e.Id) && e.MembershipOptionId == userMembership.MembershipOptionId)
                .FirstOrDefault();
            if (promotion != null)
            {
                promotion.MembershipOption = userMembership.MembershipOption;
            }

            var title = TemplateParser.Parse(Dictionary.ThankyouForSubscriptionEmailTitle, new
            {
                SubscriptionType = userMembership.MembershipOption.SubscriptionTypeNameLocal
            });
            var body = _emailTemplateService.ParseForUser(
                title,
                Dictionary.NewSubscriptionThankYouEmail,
                user,
                new
                {
                    CustomerName = user.FirstName,
                    SubscriptionType = userMembership.MembershipOption.SubscriptionTypeNameLocal,
                    TotalPrice = promotion != null ? promotion.FormattedSpecialPrice : userMembership.MembershipOption.FormattedPrice,
                    EndsOn = userMembership.EndsOn.ToLongDateString(),
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

        private void SendEmailToNineStarAboutSubscriptionFailure(string customerName, string customerEmailAddress, string errorMessage)
        {
            var contact = My.ContactsRepository.Find(e => e.EmailAddress == customerEmailAddress).FirstOrDefault();
            var title = "A new subscription was created, but something went wrong.";
            var body = _emailTemplateService.ParseForContact(
                title,
                Dictionary.NewSubscriptionErrorEmail,
                contact,
                new
                {
                    Customer = customerName,
                    CustomerEmail = customerEmailAddress,
                    ErrorMessage = errorMessage
                });

            try
            {
                My.Mailer.SendEmail(
                    title,
                    body,
                    My.WebsiteConfiguration.SupportEmailAddress,
                    My.WebsiteConfiguration.CompanyName);
            }
            catch (Exception ex)
            {
                My.Logger.Error(ex.GetFullErrorMessage());
            }
        }

    }
}