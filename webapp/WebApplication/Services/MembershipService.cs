using K9.Base.DataAccessLayer.Enums;
using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Config;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Models;
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
        private readonly IContactService _contactService;
        private readonly IMailer _mailer;
        private readonly IRepository<PromoCode> _promoCodesRepository;
        private readonly WebsiteConfiguration _config;
        private readonly UrlHelper _urlHelper;

        public MembershipService(ILogger logger, IAuthentication authentication, IRepository<MembershipOption> membershipOptionRepository, IRepository<UserMembership> userMembershipRepository, IRepository<UserProfileReading> userProfileReadingsRepository, IRepository<UserRelationshipCompatibilityReading> userRelationshipCompatibilityReadingsRepository, IRepository<UserCreditPack> userCreditPacksRepository, IRepository<User> usersRepository, IContactService contactService, IMailer mailer, IOptions<WebsiteConfiguration> config, IRepository<PromoCode> promoCodesRepository)
        {
            _logger = logger;
            _authentication = authentication;
            _membershipOptionRepository = membershipOptionRepository;
            _userMembershipRepository = userMembershipRepository;
            _userProfileReadingsRepository = userProfileReadingsRepository;
            _userRelationshipCompatibilityReadingsRepository = userRelationshipCompatibilityReadingsRepository;
            _userCreditPacksRepository = userCreditPacksRepository;
            _usersRepository = usersRepository;
            _contactService = contactService;
            _mailer = mailer;
            _promoCodesRepository = promoCodesRepository;
            _config = config.Value;
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        public MembershipViewModel GetMembershipViewModel(int? userId = null)
        {
            userId = userId ?? _authentication.CurrentUserId;
            var membershipOptions = _membershipOptionRepository.List();
            var activeUserMembership = GetActiveUserMembership(userId);

            return new MembershipViewModel
            {
                MembershipModels = new List<MembershipModel>(membershipOptions.Select(membershipOption =>
                {
                    var isSubscribed = activeUserMembership != null && activeUserMembership.MembershipOptionId == membershipOption.Id;
                    var isUpgradable = activeUserMembership == null || activeUserMembership.MembershipOption.CanUpgradeTo(membershipOption);

                    return new MembershipModel(_authentication.CurrentUserId, membershipOption, activeUserMembership)
                    {
                        IsSelectable = !isSubscribed && isUpgradable,
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

        public bool IsCompleteProfileReading(int? userId, PersonModel personModel)
        {
            var activeUserMembership = GetActiveUserMembership(userId);
            if (activeUserMembership?.ProfileReadings?.Any(e => e.DateOfBirth == personModel.DateOfBirth && e.Gender == personModel.Gender && e.FullName == personModel.Name) == true)
            {
                return true;
            }
            if (activeUserMembership?.NumberOfProfileReadingsLeft > 0 || activeUserMembership?.NumberOfCreditsLeft > 0)
            {
                CreateNewUserProfileReading(activeUserMembership, personModel.Name, personModel.DateOfBirth, personModel.Gender);
                return true;
            }

            return false;
        }

        public bool IsCompleteRelationshipCompatibilityReading(int? userId, PersonModel personModel1, PersonModel personModel2)
        {
            var activeUserMembership = GetActiveUserMembership(userId);
            if (activeUserMembership?.RelationshipCompatibilityReadings?.Any(e =>
                    e.FirstName == personModel1.Name && e.FirstDateOfBirth == personModel1.DateOfBirth && e.FirstGender == personModel1.Gender &&
                    e.SecondName == personModel2.Name && e.SecondDateOfBirth == personModel2.DateOfBirth && e.SecondGender == personModel2.Gender) == true)
            {
                return true;
            }
            if (activeUserMembership?.NumberOfRelationshipCompatibilityReadingsLeft > 0 || activeUserMembership?.NumberOfCreditsLeft > 0)
            {
                CreateNewUserRelationshipCompatibilityReading(activeUserMembership, personModel1, personModel2);
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

            var membershipOption = _membershipOptionRepository.Find(membershipOptionId);
            if (!activeUserMembership.MembershipOption.CanUpgradeTo(membershipOption))
            {
                throw new Exception(Dictionary.CannotSwitchMembershipError);
            }

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

            var membershipOption = _membershipOptionRepository.Find(membershipOptionId);
            var userMemberships = GetActiveUserMemberships();
            if (userMemberships.Any(e => e.MembershipOption.Id != membershipOption.Id))
            {
                throw new Exception(Dictionary.PurchaseMembershipErrorAlreadySubscribedToAnother);
            }

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

        public void ProcessPurchaseWithPromoCode(int userId, string code)
        {
            var promoCode = _promoCodesRepository.Find(e => e.Code == code).FirstOrDefault();

            if (promoCode == null)
            {
                _logger.Error($"MembershipService => ProcessPurchaseWithPromoCode => Invalid Promo Code");
                throw new Exception("Invalid promo code");
            }

            var subscription = _membershipOptionRepository.Find(e => e.SubscriptionType == promoCode.SubscriptionType).FirstOrDefault();

            if (subscription == null)
            {
                _logger.Error($"MembershipService => ProcessPurchaseWithPromoCode => No subscription of type {promoCode.SubscriptionTypeName} found");
                throw new Exception($"No subscription of type {promoCode.SubscriptionTypeName} found");
            }

            var credits = promoCode.Credits;
            var user = _usersRepository.Find(userId);
            var contact = _contactService.GetOrCreateContact("", user.FullName, user.EmailAddress, user.PhoneNumber);

            ProcessPurchase(new PurchaseModel
            {
                ItemId = subscription.Id,
                ContactId = contact.Id
            }, userId, promoCode);

            ProcessCreditsPurchase(new PurchaseModel
            {
                ItemId = subscription.Id,
                ContactId = contact.Id,
                Quantity = credits
            }, userId, promoCode);
        }

        public void ProcessPurchase(PurchaseModel purchaseModel, int? userId = null, PromoCode promoCode = null)
        {
            try
            {
                var membershipOptionId = purchaseModel.ItemId;
                var membershipOption = _membershipOptionRepository.Find(membershipOptionId);
                if (membershipOption == null)
                {
                    _logger.Error($"MembershipService => ProcessPurchase => No MembershipOption with id {membershipOptionId} was found.");
                    throw new IndexOutOfRangeException("Invalid MembershipOptionId");
                }

                var userMembership = new UserMembership
                {
                    UserId = userId ?? _authentication.CurrentUserId,
                    MembershipOptionId = membershipOptionId,
                    StartsOn = DateTime.Today,
                    EndsOn = membershipOption.IsAnnual ? DateTime.Today.AddYears(1) : DateTime.Today.AddMonths(1),
                    IsAutoRenew = true
                };

                _userMembershipRepository.Create(userMembership);
                userMembership.User = _usersRepository.Find(userId ?? _authentication.CurrentUserId);
                TerminateExistingMemberships(membershipOptionId);

                var contact = _contactService.Find(purchaseModel.ContactId);

                SendEmailToNineStar(userMembership, promoCode);
                SendEmailToCustomer(userMembership, contact, promoCode);
            }
            catch (Exception ex)
            {
                _logger.Error($"MembershipService => ProcessPurchase => Purchase failed: {ex.GetFullErrorMessage()}");
                SendEmailToNineStarAboutFailure(purchaseModel, ex.GetFullErrorMessage());
                throw ex;
            }
        }

        public void ProcessCreditsPurchase(PurchaseModel purchaseModel, int? userId = null, PromoCode promoCode = null)
        {
            try
            {
                var numberOfCredits = purchaseModel.Quantity;
                var creditsModel = new PurchaseCreditsViewModel
                {
                    NumberOfCredits = numberOfCredits
                };

                var userCreditPack = new UserCreditPack
                {
                    UserId = userId ?? _authentication.CurrentUserId,
                    NumberOfCredits = numberOfCredits,
                    TotalPrice = promoCode?.TotalPrice ?? creditsModel.TotalPrice
                };

                _userCreditPacksRepository.Create(userCreditPack);
                userCreditPack.User = _usersRepository.Find(_authentication.CurrentUserId);

                var contact = _contactService.Find(purchaseModel.ContactId);

                SendEmailToNineStar(userCreditPack, promoCode);
                SendEmailToCustomer(userCreditPack, contact, promoCode);
            }
            catch (Exception ex)
            {
                _logger.Error($"MembershipService => ProcessPurchase => Purchase failed: {ex.GetFullErrorMessage()}");
                SendEmailToNineStarAboutFailure(purchaseModel, ex.GetFullErrorMessage());
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
                _logger.Error($"MembershipService => ProcessSwitch => Switch failed: {ex.GetFullErrorMessage()}");
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
                _logger.Error($"MembershipService => CreateFreeMembership => failed: {ex.GetFullErrorMessage()}");
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
                return;
            }
            foreach (var userMembership in userMemberships.Where(_ => _.MembershipOptionId != activeUserMembershipId))
            {
                userMembership.EndsOn = activeUserMembership.StartsOn;
                userMembership.IsDeactivated = true;
                _userMembershipRepository.Update(userMembership);
            }
        }

        private void CreateNewUserProfileReading(UserMembership userMembership, string name, DateTime dateOfBirth, EGender gender)
        {
            var userProfileReading = new UserProfileReading
            {
                FullName = name,
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

        private void CreateNewUserRelationshipCompatibilityReading(UserMembership userMembership, PersonModel personModel1, PersonModel personModel2)
        {
            var userRelationshipCompatibilityReading = new UserRelationshipCompatibilityReading
            {
                FirstName = personModel1.Name,
                FirstDateOfBirth = personModel1.DateOfBirth,
                FirstGender = personModel1.Gender,
                SecondName = personModel2.Name,
                SecondDateOfBirth = personModel2.DateOfBirth,
                SecondGender = personModel2.Gender,
                UserId = _authentication.CurrentUserId,
                UserMembershipId = userMembership.Id
            };

            if (userMembership.NumberOfRelationshipCompatibilityReadingsLeft <= 0)
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

        private void SendEmailToNineStar(UserMembership userMembership, PromoCode promoCode)
        {
            var template = Dictionary.MembershipCreatedEmail;
            var title = "We have received a new subscription!";
            _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
            {
                Title = title,
                Customer = userMembership.User.FullName,
                CustomerEmail = userMembership.User.EmailAddress,
                SubscriptionType = userMembership.MembershipOption.SubscriptionTypeNameLocal,
                TotalPrice = promoCode?.FormattedPrice ?? userMembership.MembershipOption.FormattedPrice,
                LinkToSummary = _urlHelper.AbsoluteAction("Index", "UserMemberships"),
                Company = _config.CompanyName,
                ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl)
            }), _config.SupportEmailAddress, _config.CompanyName, _config.SupportEmailAddress, _config.CompanyName);
        }

        private void SendEmailToCustomer(UserMembership userMembership, Contact contact, PromoCode promoCode = null)
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
                    CustomerName = userMembership.User.FirstName,
                    SubscriptionType = userMembership.MembershipOption.SubscriptionTypeNameLocal,
                    TotalPrice = promoCode.FormattedPrice ?? userMembership.MembershipOption.FormattedPrice,
                    EndsOn = userMembership.EndsOn.ToLongDateString(),
                    NumberOfProfileReadings = userMembership.MembershipOption.MaxNumberOfProfileReadings,
                    NumberOfCompatibilityReadings =
                        userMembership.MembershipOption.MaxNumberOfCompatibilityReadings,
                    ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl),
                    PrivacyPolicyLink = _urlHelper.AbsoluteAction("PrivacyPolicy", "Home"),
                    UnsubscribeLink = _urlHelper.AbsoluteAction("Unsubscribe", "Account", new { id = contact.Id }),
                    DateTime.Now.Year
                }), userMembership.User.EmailAddress, userMembership.User.FirstName, _config.SupportEmailAddress,
                    _config.CompanyName);
            }
        }

        private void SendEmailToNineStar(UserCreditPack userCreditPack, PromoCode promoCode)
        {
            var template = Dictionary.CreditPackPurchased;
            var title = "We have received a new credit pack purchase!";
            _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
            {
                Title = title,
                Customer = userCreditPack.User.FullName,
                CustomerEmail = userCreditPack.User.EmailAddress,
                userCreditPack.NumberOfCredits,
                TotalPrice = promoCode?.FormattedPrice ?? userCreditPack.FormattedPrice,
                LinkToCreditPacks = _urlHelper.AbsoluteAction("Index", "UserCreditPacks"),
                Company = _config.CompanyName,
                ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl)
            }), _config.SupportEmailAddress, _config.CompanyName, _config.SupportEmailAddress, _config.CompanyName);
        }

        private void SendEmailToCustomer(UserCreditPack userCreditPack, Contact contact, PromoCode promoCode = null)
        {
            var template = Dictionary.NewCreditPackThankYouEmail;
            var title = Dictionary.ThankyouForCreditPackPurchaseEmailTitle;
            if (contact != null && !contact.IsUnsubscribed)
            {
                _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
                {
                    Title = title,
                    CustomerName = userCreditPack.User.FirstName,
                    NumberOfCreditsPurchased = userCreditPack.NumberOfCredits,
                    TotalPrice = promoCode?.FormattedPrice ?? userCreditPack.FormattedPrice,
                    ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl),
                    PrivacyPolicyLink = _urlHelper.AbsoluteAction("PrivacyPolicy", "Home"),
                    UnsubscribeLink = _urlHelper.AbsoluteAction("Unsubscribe", "Account", new { code = contact.Name }),
                    DateTime.Now.Year
                }), userCreditPack.User.EmailAddress, userCreditPack.User.FirstName, _config.SupportEmailAddress,
                    _config.CompanyName);
            }
        }

        private void SendEmailToNineStarAboutFailure(PurchaseModel purchaseModel, string errorMessage)
        {
            var template = Dictionary.PaymentError;
            var title = "A customer made a successful payment, but an error occurred.";
            _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
            {
                Title = title,
                Customer = purchaseModel.CustomerName,
                CustomerEmail = purchaseModel.CustomerEmailAddress,
                ErrorMessage = errorMessage,
                Company = _config.CompanyName,
                ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl)
            }), _config.SupportEmailAddress, _config.CompanyName, _config.SupportEmailAddress, _config.CompanyName);
        }

    }
}