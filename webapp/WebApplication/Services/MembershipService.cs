using K9.Base.DataAccessLayer.Enums;
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
        private readonly StripeConfiguration _stripeConfig;
        private readonly IStripeService _stripeService;
        private readonly IContactService _contactService;
        private readonly IMailer _mailer;
        private readonly WebsiteConfiguration _config;
        private readonly UrlHelper _urlHelper;

        public MembershipService(ILogger logger, IAuthentication authentication, IRepository<MembershipOption> membershipOptionRepository, IRepository<UserMembership> userMembershipRepository, IRepository<UserProfileReading> userProfileReadingsRepository, IRepository<UserRelationshipCompatibilityReading> userRelationshipCompatibilityReadingsRepository, IOptions<StripeConfiguration> stripeConfig, IStripeService stripeService, IContactService contactService, IMailer mailer, IOptions<WebsiteConfiguration> config)
        {
            _logger = logger;
            _authentication = authentication;
            _membershipOptionRepository = membershipOptionRepository;
            _userMembershipRepository = userMembershipRepository;
            _userProfileReadingsRepository = userProfileReadingsRepository;
            _userRelationshipCompatibilityReadingsRepository = userRelationshipCompatibilityReadingsRepository;
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

                    var isUpgradable = (!isScheduledSwitch && !isSubscribed) && activeUserMembership.MembershipOption.CanUpgradeTo(membershipOption);

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
                    return userMembership;
                }).ToList()
                : new List<UserMembership>();
            return userMemberships;
        }

        /// <summary>
        /// Sometimes user memberships can overlap, when upgrading for example. This returns the Active membership.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserMembership GetActiveUserMembership(int? userId = null)
        {
            return GetActiveUserMemberships(userId).OrderByDescending(_ => _.MembershipOption.SubscriptionType)
                .FirstOrDefault();
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

        public bool GetProfileReading(int? userId, DateTime dateOfBirth, EGender gender, bool createIfNull = true)
        {
            var activeUserMembership = GetActiveUserMembership(userId);
            if (activeUserMembership?.ProfileReadings?.Any(e => e.DateOfBirth == dateOfBirth && e.Gender == gender) == true)
            {
                return true;
            }
            if (createIfNull && activeUserMembership?.NumberOfProfileReadingsLeft > 0)
            {
                CreateNewUserProfileReading(activeUserMembership.Id, dateOfBirth, gender);
                return true;
            }

            return false;
        }

        public bool GetRelationshipCompatibilityReading(int? userId, DateTime firstDateOfBirth,
            EGender firstGender, DateTime secondDateOfBirth, EGender secondGender, bool createIfNull = true)
        {
            var activeUserMembership = GetActiveUserMembership(userId);
            if (activeUserMembership?.RelationshipCompatibilityReadings?.Any(e =>
                    e.FirstDateOfBirth == firstDateOfBirth && e.FirstGender == firstGender &&
                    e.SecondDateOfBirth == secondDateOfBirth && e.SecondGender == secondGender) == true)
            {
                return true;
            }
            if (createIfNull && activeUserMembership?.NumberOfRelationshipCompatibilityReadingsLeft > 0)
            {
                CreateNewUserRelationshipCompatibilityReading(activeUserMembership.Id, firstDateOfBirth, firstGender, secondDateOfBirth, secondGender);
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
                LocalisedCurrencyThreeLetters = StripeModel.GetLocalisedCurrency()
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
                _userMembershipRepository.Create(new UserMembership
                {
                    UserId = _authentication.CurrentUserId,
                    MembershipOptionId = model.MembershipOptionId,
                    StartsOn = DateTime.Today,
                    EndsOn = membershipOption.IsAnnual ? DateTime.Today.AddYears(1) : DateTime.Today.AddMonths(1),
                    IsAutoRenew = true
                });
                TerminateExistingMemberships(model.MembershipOptionId);
                _contactService.CreateCustomer(result.StripeCustomer.Id, model.StripeBillingName, model.StripeEmail);
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

        private void CreateNewUserProfileReading(int userMembershipId, DateTime dateOfBirth, EGender gender)
        {
            _userProfileReadingsRepository.Create(new UserProfileReading
            {
                DateOfBirth = dateOfBirth,
                Gender = gender,
                UserId = _authentication.CurrentUserId,
                UserMembershipId = userMembershipId
            });
        }

        private void CreateNewUserRelationshipCompatibilityReading(int userMembershipId, DateTime firstDateOfBirth, EGender firstGender, DateTime secondDateOfBirth, EGender secondGender)
        {
            _userRelationshipCompatibilityReadingsRepository.Create(new UserRelationshipCompatibilityReading
            {
                FirstDateOfBirth = firstDateOfBirth,
                FirstGender = firstGender,
                SecondDateOfBirth = secondDateOfBirth,
                SecondGender = secondGender,
                UserId = _authentication.CurrentUserId,
                UserMembershipId = userMembershipId
            });
        }

        private void SendEmailToNineStar(Donation donation)
        {
            var template = Dictionary.DonationReceivedEmail;
            var title = "We have received a donation!";
            _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
            {
                Title = title,
                donation.Customer,
                donation.CustomerEmail,
                Amount = donation.DonationAmount,
                donation.Currency,
                LinkToSummary = _urlHelper.AbsoluteAction("Index", "Donations"),
                Company = _config.CompanyName,
                ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl)
            }), _config.SupportEmailAddress, _config.CompanyName, _config.SupportEmailAddress, _config.CompanyName);
        }

        private void SendEmailToCustomer(Donation donation)
        {
            var template = Dictionary.DonationThankYouEmail;
            var title = Dictionary.ThankyouForDonationEmailTitle;
            _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
            {
                Title = title,
                donation.CustomerName,
                donation.CustomerEmail,
                Amount = donation.DonationAmount,
                donation.Currency,
                Company = _config.CompanyName,
                ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl),
                DateTime.Now.Year
            }), donation.CustomerEmail, donation.Customer, _config.SupportEmailAddress, _config.CompanyName);
        }

        private double GetCostOfRemainingActiveSubscription()
        {
            var activeUserMembership = GetActiveUserMembership();
            return activeUserMembership?.CostOfRemainingActiveSubscription ?? 0;
        }

    }
}