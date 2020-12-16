using K9.DataAccessLayer.Models;
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

namespace K9.WebApplication.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly ILogger _logger;
        private readonly IAuthentication _authentication;
        private readonly IRepository<MembershipOption> _membershipOptionRepository;
        private readonly IRepository<UserMembership> _userMembershipRepository;
        private readonly StripeConfiguration _stripeConfig;
        private readonly IStripeService _stripeService;
        private readonly IContactService _contactService;
        private readonly IMailer _mailer;

        public MembershipService(ILogger logger, IAuthentication authentication, IRepository<MembershipOption> membershipOptionRepository, IRepository<UserMembership> userMembershipRepository, IOptions<StripeConfiguration> stripeConfig, IStripeService stripeService, IContactService contactService, IMailer mailer)
        {
            _logger = logger;
            _authentication = authentication;
            _membershipOptionRepository = membershipOptionRepository;
            _userMembershipRepository = userMembershipRepository;
            _stripeConfig = stripeConfig.Value;
            _stripeService = stripeService;
            _contactService = contactService;
            _mailer = mailer;
        }

        public MembershipViewModel GetMembershipViewModel(int? userId = null)
        {
            userId = userId ?? _authentication.CurrentUserId;
            var membershipOptions = _membershipOptionRepository.List();
            var activeUserMemberships = GetActiveUserMemberships(userId, true);
            var activeUserMembership = GetActiveUserMembership(userId);
            var scheduledMembership = GetScheduledSwitchUserMembership(userId);

            return new MembershipViewModel
            {
                Memberships = new List<MembershipModel>(membershipOptions.Select(membershipOption =>
                {
                    var isSubscribed = activeUserMemberships.FirstOrDefault(_ =>
                                          _.UserId == userId & _.MembershipOptionId == membershipOption.Id) != null;
                    var isScheduledSwitch = scheduledMembership != null && membershipOption.SubscriptionType == scheduledMembership.MembershipOption.SubscriptionType;
                    return new MembershipModel(_authentication.CurrentUserId, membershipOption, activeUserMembership)
                    {
                        IsSelectable = !isScheduledSwitch && !isSubscribed,
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

        public MembershipModel GetSwitchMembershipModel(int id)
        {
            var userMemberships = GetActiveUserMemberships();
            if (!userMemberships.Any())
            {
                throw new Exception(Globalisation.Dictionary.SwitchMembershipErrorNotSubscribed);
            }

            var activeUserMembership = GetActiveUserMembership();
            if (activeUserMembership.MembershipOptionId == id)
            {
                throw new Exception(Globalisation.Dictionary.SwitchMembershipErrorAlreadySubscribed);
            }

            var scheduledUserMembership = GetScheduledSwitchUserMembership();
            if (scheduledUserMembership?.MembershipOptionId == id)
            {
                throw new Exception(Globalisation.Dictionary.SwitchMembershipErrorAlreadySubscribed);
            }

            var membershipOption = _membershipOptionRepository.Find(id);

            return new MembershipModel(_authentication.CurrentUserId, membershipOption, activeUserMembership)
            {
                IsSelected = true
            };
        }

        public MembershipModel GetPurchaseMembershipModel(int id)
        {
            var activeUserMembership = GetActiveUserMembership();
            if (activeUserMembership?.MembershipOptionId == id)
            {
                throw new Exception(Globalisation.Dictionary.PurchaseMembershipErrorAlreadySubscribed);
            }

            var userMemberships = GetActiveUserMemberships();
            if (userMemberships.Any())
            {
                throw new Exception(Globalisation.Dictionary.PurchaseMembershipErrorAlreadySubscribedToAnother);
            }

            var membershipOption = _membershipOptionRepository.Find(id);
            return new MembershipModel(_authentication.CurrentUserId, membershipOption)
            {
                IsSelected = true
            };
        }

        public StripeModel GetPurchaseStripeModel(int id)
        {
            var membershipOption = _membershipOptionRepository.Find(id);
            if (membershipOption == null)
            {
                _logger.Error($"MembershipService => GetPurchaseStripeModel => No MembershipOption with id {id} was found.");
                throw new IndexOutOfRangeException();
            }

            return new StripeModel
            {
                PublishableKey = _stripeConfig.PublishableKey,
                SubscriptionAmount = membershipOption.Price,
                SubscriptionDiscount = GetCostOfRemainingActiveSubscription(),
                Description = membershipOption.SubscriptionTypeNameLocal,
                MembershipOptionId = id,
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

        public void ProcessSwitch(int id)
        {
            try
            {
                var membershipOption = _membershipOptionRepository.Find(id);
                if (membershipOption == null)
                {
                    _logger.Error($"MembershipService => ProcessSwitch => No MembershipOption with id {id} was found.");
                    throw new IndexOutOfRangeException("Invalid MembershipOptionId");
                }

                _userMembershipRepository.Create(new UserMembership
                {
                    UserId = _authentication.CurrentUserId,
                    MembershipOptionId = id,
                    StartsOn = DateTime.Today,
                    EndsOn = membershipOption.IsAnnual ? DateTime.Today.AddYears(1) : DateTime.Today.AddMonths(1),
                    IsAutoRenew = true
                });
                TerminateExistingMemberships(id);
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
                _userMembershipRepository.Update(userMembership);
            }
        }



        private double GetCostOfRemainingActiveSubscription()
        {
            var activeUserMembership = GetActiveUserMembership();
            return activeUserMembership?.CostOfRemainingActiveSubscription ?? 0;
        }

    }
}