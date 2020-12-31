using K9.Base.DataAccessLayer.Enums;
using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Config;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Models;
using K9.WebApplication.Services.Stripe;
using K9.WebApplication.ViewModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace K9.WebApplication.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly ILogger _logger;
        private readonly IAuthentication _authentication;
        private readonly IRepository<MembershipOption> _membershipOptionRepository;
        private readonly IRepository<UserMembership> _userMembershipRepository;
        private readonly IRepository<UserProfileReading> _userProfileReadingsRepository;
        private readonly IRepository<UserRelationshipCompatibilityReading> _userRelationshipCompatibilityReadingsRepository;
        private readonly IRepository<UserCreditPack> _userCreditPacksRepository;
        private readonly IRepository<User> _usersRepository;
        private readonly StripeConfiguration _stripeConfig;
        private readonly IStripeService _stripeService;
        private readonly IContactService _contactService;
        private readonly IMailer _mailer;
        private readonly WebsiteConfiguration _config;
        private readonly UrlHelper _urlHelper;

        public MembershipService(ILogger logger, IAuthentication authentication, IRepository<MembershipOption> membershipOptionRepository, IRepository<UserMembership> userMembershipRepository, IRepository<UserProfileReading> userProfileReadingsRepository, IRepository<UserRelationshipCompatibilityReading> userRelationshipCompatibilityReadingsRepository, IRepository<UserCreditPack> userCreditPacksRepository, IRepository<User> usersRepository, IOptions<StripeConfiguration> stripeConfig, IStripeService stripeService, IContactService contactService, IMailer mailer, IOptions<WebsiteConfiguration> config)
        {
            _logger = logger;
            _authentication = authentication;
            _membershipOptionRepository = membershipOptionRepository;
            _userMembershipRepository = userMembershipRepository;
            _userProfileReadingsRepository = userProfileReadingsRepository;
            _userRelationshipCompatibilityReadingsRepository = userRelationshipCompatibilityReadingsRepository;
            _userCreditPacksRepository = userCreditPacksRepository;
            _usersRepository = usersRepository;
            _stripeConfig = stripeConfig.Value;
            _stripeService = stripeService;
            _contactService = contactService;
            _mailer = mailer;
            _config = config.Value;
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        public MembershipViewModel GetMembershipViewModel(int? userId = null)
        {
            userId = userId ?? _authentication.CurrentUserId;
            var membershipOptions = _membershipOptionRepository.List();
            var activeUserMembership = GetActiveUserMembership(userId);
            var scheduledMembership = GetScheduledSwitchUserMembership(userId);

            return new MembershipViewModel
            {
                MembershipModels = new List<MembershipModel>(membershipOptions.Select(membershipOption =>
                {
                    var isSubscribed = activeUserMembership != null && activeUserMembership.MembershipOptionId == membershipOption.Id;
                    var isScheduledSwitch = scheduledMembership != null && membershipOption.SubscriptionType == scheduledMembership.MembershipOption.SubscriptionType;

                    var isUpgradable = activeUserMembership == null || activeUserMembership.MembershipOption.CanUpgradeTo(membershipOption);

                    return new MembershipModel(_authentication.CurrentUserId, membershipOption, activeUserMembership)
                    {
                        IsSelectable = !isScheduledSwitch && !isSubscribed && isUpgradable,
                        IsSubscribed = isSubscribed
                    };
                }))
            };
        }

        public List<UserMembership> GetActiveUserMemberships(int? userId = null, bool includeScheduled = false)
        {
            userId = userId ?? _authentication.CurrentUserId;
            var membershipOptions = _membershipOptionRepository.List();
            var userMemberships = _authentication.IsAuthenticated
                ? _userMembershipRepository.Find(_ => _.UserId == userId).ToList().Where(_ => _.IsActive || includeScheduled && _.EndsOn > DateTime.Today).Select(userMembership =>
                {
                    userMembership.MembershipOption = membershipOptions.FirstOrDefault(m => m.Id == userMembership.MembershipOptionId);
                    userMembership.ProfileReadings = _userProfileReadingsRepository.Find(e => e.UserId == userId).ToList();
                    userMembership.RelationshipCompatibilityReadings = _userRelationshipCompatibilityReadingsRepository
                        .Find(e => e.UserId == userId).ToList();
                    userMembership.NumberOfCreditsLeft = GetNumberOfCreditsLeft(userMembership);

                    return userMembership;
                }).ToList()
                : new List<UserMembership>();
            return userMemberships;
        }

        private int GetNumberOfCreditsLeft(UserMembership userMembership)
        {
            var creditPacks = _userCreditPacksRepository.Find(e => e.UserId == userMembership.UserId);
            var creditPackIds = creditPacks.Select(c => c.Id);
            var numberOfUsedCredits =
                _userProfileReadingsRepository.Find(e => creditPackIds.Contains(e.UserCreditPackId ?? 0))?.Count() +
                _userRelationshipCompatibilityReadingsRepository.Find(e => creditPackIds.Contains(e.UserCreditPackId ?? 0))?.Count();
            var totalCredits = creditPacks.Any() ? creditPacks.Sum(e => e.NumberOfCredits) : 0;
            return totalCredits - numberOfUsedCredits ?? 0;
        }

        /// <summary>
        /// Sometimes user memberships can overlap, when upgrading for example. This returns the Active membership.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserMembership GetActiveUserMembership(int? userId = null)
        {
            var activeUserMembership = GetActiveUserMemberships(userId).OrderByDescending(_ => _.MembershipOption.SubscriptionType)
                .FirstOrDefault();

            if (activeUserMembership == null && userId.HasValue)
            {
                CreateFreeMembership(userId.Value);
                activeUserMembership = GetActiveUserMemberships(userId).OrderByDescending(_ => _.MembershipOption.SubscriptionType)
                    .FirstOrDefault();
            }

            return activeUserMembership;
        }

        /// <summary>
        /// A user can opt to downgrade at the end of the current subscription. This returns the membership option that will auto renew when the active membership expires
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserMembership GetScheduledSwitchUserMembership(int? userId = null)
        {
            var activeUserMembership = GetActiveUserMembership(userId);
            return GetActiveUserMemberships(userId, true).FirstOrDefault(_ => _.StartsOn > activeUserMembership.EndsOn && _.IsAutoRenew);
        }

        public bool IsCompleteProfileReading(int? userId, DateTime dateOfBirth, EGender gender)
        {
            var activeUserMembership = GetActiveUserMembership(userId);
            if (activeUserMembership?.ProfileReadings?.Any(e => e.DateOfBirth == dateOfBirth && e.Gender == gender) == true)
            {
                return true;
            }
            if (activeUserMembership?.NumberOfProfileReadingsLeft > 0 || activeUserMembership?.NumberOfCreditsLeft > 0)
            {
                CreateNewUserProfileReading(activeUserMembership, dateOfBirth, gender);
                return true;
            }

            return false;
        }

        public bool IsCompleteRelationshipCompatibilityReading(int? userId, DateTime firstDateOfBirth,
            EGender firstGender, DateTime secondDateOfBirth, EGender secondGender)
        {
            var activeUserMembership = GetActiveUserMembership(userId);
            if (activeUserMembership?.RelationshipCompatibilityReadings?.Any(e =>
                    e.FirstDateOfBirth == firstDateOfBirth && e.FirstGender == firstGender &&
                    e.SecondDateOfBirth == secondDateOfBirth && e.SecondGender == secondGender) == true)
            {
                return true;
            }
            if (activeUserMembership?.NumberOfRelationshipCompatibilityReadingsLeft > 0 || activeUserMembership?.NumberOfCreditsLeft > 0)
            {
                CreateNewUserRelationshipCompatibilityReading(activeUserMembership, firstDateOfBirth, firstGender, secondDateOfBirth, secondGender);
                return true;
            }

            return false;
        }

        public MembershipModel GetSwitchMembershipModel(int membershipOptionId)
        {
            var userMemberships = GetActiveUserMemberships();
            if (!userMemberships.Any())
            {
                throw new Exception(Dictionary.SwitchMembershipErrorNotSubscribed);
            }

            var activeUserMembership = GetActiveUserMembership();
            if (activeUserMembership.MembershipOptionId == membershipOptionId)
            {
                throw new Exception(Dictionary.SwitchMembershipErrorAlreadySubscribed);
            }

            var scheduledUserMembership = GetScheduledSwitchUserMembership();
            if (scheduledUserMembership?.MembershipOptionId == membershipOptionId)
            {
                throw new Exception(Dictionary.SwitchMembershipErrorAlreadySubscribed);
            }

            var membershipOption = _membershipOptionRepository.Find(membershipOptionId);

            return new MembershipModel(_authentication.CurrentUserId, membershipOption, activeUserMembership)
            {
                IsSelected = true
            };
        }

        public MembershipModel GetPurchaseMembershipModel(int membershipOptionId)
        {
            var activeUserMembership = GetActiveUserMembership();
            if (activeUserMembership?.MembershipOptionId == membershipOptionId)
            {
                throw new Exception(Dictionary.PurchaseMembershipErrorAlreadySubscribed);
            }

            var userMemberships = GetActiveUserMemberships();
            if (userMemberships.Any())
            {
                throw new Exception(Dictionary.PurchaseMembershipErrorAlreadySubscribedToAnother);
            }

            var membershipOption = _membershipOptionRepository.Find(membershipOptionId);
            return new MembershipModel(_authentication.CurrentUserId, membershipOption)
            {
                IsSelected = true
            };
        }

        public MembershipModel GetSwitchMembershipModelBySubscriptionType(MembershipOption.ESubscriptionType subscriptionType)
        {
            var membershipOption = _membershipOptionRepository.Find(e => e.SubscriptionType == subscriptionType).FirstOrDefault();
            return GetSwitchMembershipModel(membershipOption?.Id ?? 0);
        }

        public MembershipModel GetPurchaseMembershipModelBySubscriptionType(MembershipOption.ESubscriptionType subscriptionType)
        {
            var membershipOption = _membershipOptionRepository.Find(e => e.SubscriptionType == subscriptionType).FirstOrDefault();
            return GetPurchaseMembershipModel(membershipOption?.Id ?? 0);
        }

        public StripeModel GetPurchaseStripeModel(int membershipOptionId)
        {
            var membershipOption = _membershipOptionRepository.Find(membershipOptionId);
            if (membershipOption == null)
            {
                _logger.Error($"MembershipService => GetPurchaseStripeModel => No MembershipOption with id {membershipOptionId} was found.");
                throw new IndexOutOfRangeException();
            }

            return new StripeModel
            {
                SubscriptionAmount = membershipOption.Price,
                SubscriptionDiscount = GetCostOfRemainingActiveSubscription(),
                Description = membershipOption.SubscriptionTypeNameLocal,
                MembershipOptionId = membershipOptionId,
                PublishableKey = _stripeConfig.PublishableKey,
                LocalisedCurrencyThreeLetters = StripeModel.GetSystemCurrencyCode()
            };
        }

        public StripeModel GetPurchaseCreditsStripeModel(PurchaseCreditsViewModel model)
        {
            if (model.NumberOfCredits == 0)
            {
                _logger.Error($"MembershipService => GetPurchaseCreditsStripeModel => No credits were selected.");
                throw new IndexOutOfRangeException("Number of credits to purchase number be greater than zero.");
            }

            return new StripeModel
            {
                TotalNumberOfCreditsToPurchase = model.NumberOfCredits,
                CreditsPurchaseAmount = model.TotalPrice,
                Description = Dictionary.CreditsPurchaseDescription,
                PublishableKey = _stripeConfig.PublishableKey,
                LocalisedCurrencyThreeLetters = StripeModel.GetSystemCurrencyCode()
            };
        }

        public void ProcessPurchase(StripeModel model)
        {
            try
            {
                var membershipOption = _membershipOptionRepository.Find(model.MembershipOptionId);
                if (membershipOption == null)
                {
                    _logger.Error($"MembershipService => ProcessPurchase => No MembershipOption with id {model.MembershipOptionId} was found.");
                    throw new IndexOutOfRangeException("Invalid MembershipOptionId");
                }

                var result = _stripeService.Charge(model);
                var userMembership = new UserMembership
                {
                    UserId = _authentication.CurrentUserId,
                    MembershipOptionId = model.MembershipOptionId,
                    StartsOn = DateTime.Today,
                    EndsOn = membershipOption.IsAnnual ? DateTime.Today.AddYears(1) : DateTime.Today.AddMonths(1),
                    IsAutoRenew = true,
                    User = _usersRepository.Find(_authentication.CurrentUserId)
                };
                _userMembershipRepository.Create(userMembership);
                TerminateExistingMemberships(model.MembershipOptionId);
                var contact = _contactService.GetOrCreateContact(result.StripeCustomer.Id, model.StripeBillingName, model.StripeEmail);
                SendEmailToNineStar(userMembership);
                SendEmailToCustomer(contact, userMembership);
            }
            catch (Exception ex)
            {
                _logger.Error($"MembershipService => ProcessPurchase => Purchase failed: {ex.Message}");
                throw ex;
            }
        }

        public void ProcessCreditsPurchase(StripeModel model)
        {
            try
            {
                var result = _stripeService.Charge(model);
                var userCreditPack = new UserCreditPack
                {
                    UserId = _authentication.CurrentUserId,
                    NumberOfCredits = model.TotalNumberOfCreditsToPurchase,
                    TotalPrice = model.CreditsPurchaseAmount,
                    User = _usersRepository.Find(_authentication.CurrentUserId)
                };
                _userCreditPacksRepository.Create(userCreditPack);
                _contactService.GetOrCreateContact(result.StripeCustomer.Id, model.StripeBillingName, model.StripeEmail);
                SendEmailToNineStar(userCreditPack);
                SendEmailToCustomer(userCreditPack);
            }
            catch (Exception ex)
            {
                _logger.Error($"MembershipService => ProcessPurchase => Purchase failed: {ex.Message}");
                throw ex;
            }
        }

        public void ProcessSwitch(int membershipOptionId)
        {
            try
            {
                var membershipOption = _membershipOptionRepository.Find(membershipOptionId);
                if (membershipOption == null)
                {
                    _logger.Error($"MembershipService => ProcessSwitch => No MembershipOption with id {membershipOptionId} was found.");
                    throw new IndexOutOfRangeException("Invalid MembershipOptionId");
                }

                _userMembershipRepository.Create(new UserMembership
                {
                    UserId = _authentication.CurrentUserId,
                    MembershipOptionId = membershipOptionId,
                    StartsOn = DateTime.Today,
                    EndsOn = membershipOption.IsAnnual ? DateTime.Today.AddYears(1) : DateTime.Today.AddMonths(1),
                    IsAutoRenew = true
                });
                TerminateExistingMemberships(membershipOptionId);
            }
            catch (Exception ex)
            {
                _logger.Error($"MembershipService => ProcessSwitch => Switch failed: {ex.Message}");
                throw ex;
            }
        }

        public void CreateFreeMembership(int userId)
        {
            try
            {
                var membershipOption = _membershipOptionRepository.Find(e => e.SubscriptionType == MembershipOption.ESubscriptionType.Free).FirstOrDefault();

                if (membershipOption == null)
                {
                    _logger.Error($"MembershipService => CreateFreeMembership => MembershipOption with Subscription Type {MembershipOption.ESubscriptionType.Free} was not found.");
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
                _logger.Error($"MembershipService => CreateFreeMembership => failed: {ex.Message}");
                throw ex;
            }
        }

        private void TerminateExistingMemberships(int activeUserMembershipId)
        {
            var userMemberships = GetActiveUserMemberships();
            var activeUserMembership =
                userMemberships.FirstOrDefault(_ => _.MembershipOptionId == activeUserMembershipId);
            if (activeUserMembership == null)
            {
                _logger.Error($"MembershipService => TerminateExistingMemberships => ActiveMembership cannot be determined or does not exist");
                throw new Exception("Active membership not found");
            }
            foreach (var userMembership in userMemberships.Where(_ => _.MembershipOptionId != activeUserMembershipId))
            {
                userMembership.EndsOn = activeUserMembership.StartsOn;
                userMembership.IsDeactivated = true;
                _userMembershipRepository.Update(userMembership);
            }
        }

        private void CreateNewUserProfileReading(UserMembership userMembership, DateTime dateOfBirth, EGender gender)
        {
            var userProfileReading = new UserProfileReading
            {
                DateOfBirth = dateOfBirth,
                Gender = gender,
                UserId = userMembership.UserId,
                UserMembershipId = userMembership.Id
            };

            if (userMembership.NumberOfProfileReadingsLeft == 0)
            {
                var userCredit = _userCreditPacksRepository.Find(e => e.UserId == userMembership.UserId).FirstOrDefault();
                if (userMembership.NumberOfCreditsLeft == 0 || userCredit == null)
                {
                    _logger.Error($"MembershipService => CreateNewUserProfileReading => Not enough Credits remaining for User {userMembership.UserId}.");
                    throw new Exception("Not enough credits remaining");
                }

                userProfileReading.UserCreditPackId = userCredit.Id;
            }

            _userProfileReadingsRepository.Create(userProfileReading);
        }

        private void CreateNewUserRelationshipCompatibilityReading(UserMembership userMembership, DateTime firstDateOfBirth, EGender firstGender, DateTime secondDateOfBirth, EGender secondGender)
        {
            var userRelationshipCompatibilityReading = new UserRelationshipCompatibilityReading
            {
                FirstDateOfBirth = firstDateOfBirth,
                FirstGender = firstGender,
                SecondDateOfBirth = secondDateOfBirth,
                SecondGender = secondGender,
                UserId = _authentication.CurrentUserId,
                UserMembershipId = userMembership.Id
            };

            if (userMembership.NumberOfProfileReadingsLeft <= 0)
            {
                var userCredit = _userCreditPacksRepository.Find(e => e.UserId == userMembership.UserId).FirstOrDefault();
                if (userMembership.NumberOfCreditsLeft == 0 || userCredit == null)
                {
                    _logger.Error($"MembershipService => CreateNewUserProfileReading => No User Credits were found for User {userMembership.UserId}.");
                    throw new Exception("No User Credits were found");
                }

                userRelationshipCompatibilityReading.UserCreditPackId = userCredit.Id;
            }

            _userRelationshipCompatibilityReadingsRepository.Create(userRelationshipCompatibilityReading);
        }

        private void SendEmailToNineStar(UserMembership userMembership)
        {
            var template = Dictionary.MembershipCreatedEmail;
            var title = "We have received a new subscription!";
            _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
            {
                Title = title,
                Customer = userMembership.User.FullName,
                CustomerEmail = userMembership.User.EmailAddress,
                SubscriptionType = userMembership.MembershipOption.SubscriptionTypeNameLocal,
                TotalPrice = userMembership.MembershipOption.FormattedPrice,
                LinkToSummary = _urlHelper.AbsoluteAction("Index", "UserMemberships"),
                Company = _config.CompanyName,
                ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl)
            }), _config.SupportEmailAddress, _config.CompanyName, _config.SupportEmailAddress, _config.CompanyName);
        }

        private void SendEmailToCustomer(UserMembership userMembership, Contact contact)
        {
            var template = Dictionary.NewMembershipThankYouEmail;
            var title = TemplateProcessor.PopulateTemplate(Dictionary.ThankyouForSubscriptionEmailTitle, new
            {
                SubscriptionType = userMembership.MembershipOption.SubscriptionTypeNameLocal
            });
            if (contact != null && !contact.IsUnsubscribed)
            {
                _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
                {
                    Title = title,
                    CustomerName = userMembership.User.FullName,
                    SubscriptionType = userMembership.MembershipOption.SubscriptionTypeNameLocal,
                    TotalPrice = userMembership.MembershipOption.FormattedPrice,
                    EndsOn = userMembership.EndsOn.ToLongDateString(),
                    NumberOfProfileReadings = userMembership.MembershipOption.MaxNumberOfProfileReadings,
                    NumberOfCompatibilityReadings =
                        userMembership.MembershipOption.MaxNumberOfCompatibilityReadings,
                    ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl),
                    PrivacyPolicyLink = _urlHelper.AbsoluteAction("PrivacyPolicy", "Home"),
                    UnsubscribeLink = _urlHelper.AbsoluteAction("Unsubscribe", "Contacts", new { id = contact.Id }),
                    DateTime.Now.Year
                }), userMembership.User.EmailAddress, userMembership.User.FirstName, _config.SupportEmailAddress,
                    _config.CompanyName);
            }
        }

        private void SendEmailToNineStar(UserCreditPack userCreditPack)
        {
            var template = Dictionary.CreditPackPurchased;
            var title = "We have received a new credit pack purchase!";
            _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
            {
                Title = title,
                Customer = userCreditPack.User.FullName,
                CustomerEmail = userCreditPack.User.EmailAddress,
                userCreditPack.NumberOfCredits,
                TotalPrice = userCreditPack.FormattedPrice,
                LinkToCreditPacks = _urlHelper.AbsoluteAction("Index", "UserCreditPacks"),
                Company = _config.CompanyName,
                ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl)
            }), _config.SupportEmailAddress, _config.CompanyName, _config.SupportEmailAddress, _config.CompanyName);
        }

        private void SendEmailToCustomer(UserCreditPack userCreditPack, Contact contact)
        {
            var template = Dictionary.NewCreditPackThankYouEmail;
            var title = Dictionary.ThankyouForCreditPackPurchaseEmailTitle;
            if (contact != null && !contact.IsUnsubscribed) P
            {
                _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
                {
                    Title = title,
                    CustomerName = userCreditPack.User.FullName,
                    NumberOfCreditsPurchased = userCreditPack.NumberOfCredits,
                    TotalPrice = userCreditPack.FormattedPrice,
                    ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl),
                    PrivacyPolicyLink = _urlHelper.AbsoluteAction("PrivacyPolicy", "Home"),
                    UnsubscribeLink = _urlHelper.AbsoluteAction("Unsubscribe", "Contacts", new { id = contact.Id }),
                    DateTime.Now.Year
                }), userCreditPack.User.EmailAddress, userCreditPack.User.FirstName, _config.SupportEmailAddress,
                    _config.CompanyName);
            }
        }

        private double GetCostOfRemainingActiveSubscription()
        {
            var activeUserMembership = GetActiveUserMembership();
            return activeUserMembership?.CostOfRemainingActiveSubscription ?? 0;
        }

    }
}