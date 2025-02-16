﻿using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Config;
using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.ViewModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using K9.DataAccessLayer.Helpers;

namespace K9.WebApplication.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly ILogger _logger;
        private readonly IAuthentication _authentication;
        private readonly IRepository<MembershipOption> _membershipOptionRepository;
        private readonly IRepository<DataAccessLayer.Models.UserMembership> _userMembershipRepository;
        private readonly IRepository<User> _usersRepository;
        private readonly IContactService _contactService;
        private readonly IMailer _mailer;
        private readonly IRepository<PromoCode> _promoCodesRepository;
        private readonly IUserService _userService;
        private readonly IRepository<Consultation> _consultationsRepository;
        private readonly IRepository<UserConsultation> _userConsultationsRepository;
        private readonly IConsultationService _consultationService;
        private readonly WebsiteConfiguration _config;
        private readonly UrlHelper _urlHelper;

        public MembershipService(ILogger logger, IAuthentication authentication, IRepository<MembershipOption> membershipOptionRepository, IRepository<DataAccessLayer.Models.UserMembership> userMembershipRepository, IRepository<User> usersRepository, IContactService contactService, IMailer mailer, IOptions<WebsiteConfiguration> config, IRepository<PromoCode> promoCodesRepository, IUserService userService, IRepository<Consultation> consultationsRepository, IRepository<UserConsultation> userConsultationsRepository, IConsultationService consultationService)
        {
            _logger = logger;
            _authentication = authentication;
            _membershipOptionRepository = membershipOptionRepository;
            _userMembershipRepository = userMembershipRepository;
            _usersRepository = usersRepository;
            _contactService = contactService;
            _mailer = mailer;
            _promoCodesRepository = promoCodesRepository;
            _userService = userService;
            _consultationsRepository = consultationsRepository;
            _userConsultationsRepository = userConsultationsRepository;
            _consultationService = consultationService;
            _config = config.Value;
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        public DataAccessLayer.Models.UserMembership GetActiveUserMembership(string accountNumber)
        {
            var membershipIds =
                _userMembershipRepository.CustomQuery<string>($"SELECT [{nameof(DataAccessLayer.Models.UserMembership.Name)}] FROM {nameof(DataAccessLayer.Models.UserMembership)}");

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

        public List<DataAccessLayer.Models.UserMembership> GetActiveUserMemberships(int userId, bool includeScheduled = false)
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
        public DataAccessLayer.Models.UserMembership GetActiveUserMembership(int? userId = null)
        {
            userId = userId ?? Current.UserId;
            var activeUserMembership = GetActiveUserMemberships(userId.Value).OrderByDescending(_ => _.MembershipOption?.SubscriptionType)
                .FirstOrDefault();

            if (activeUserMembership == null && userId.HasValue)
            {
                try
                {
                    if (_userService.Find(userId.Value) != null)
                    {
                        CreateFreeMembership(userId.Value);
                    }
                    activeUserMembership = GetActiveUserMemberships(userId.Value).OrderByDescending(_ => _.MembershipOption.SubscriptionType)
                        .FirstOrDefault();
                }
                catch (Exception e)
                {
                    _logger.Error($"MembershipService => GetActiveUserMembership => {e.GetFullErrorMessage()}");
                }
            }
            else
            {
                activeUserMembership.User = _userService.Find(activeUserMembership.UserId);
            }

            return activeUserMembership;
        }

        public MembershipModel GetSwitchMembershipModel(int membershipOptionId)
        {
            var userMemberships = GetActiveUserMemberships(Current.UserId);
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

            return new MembershipModel(Current.UserId, membershipOption, activeUserMembership)
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
            var userMemberships = GetActiveUserMemberships(Current.UserId);
            if (userMemberships.Any(e => e.MembershipOption.Id != membershipOption.Id && !e.MembershipOption.IsFree))
            {
                throw new Exception(Dictionary.PurchaseMembershipErrorAlreadySubscribedToAnother);
            }

            return new MembershipModel(Current.UserId, membershipOption)
            {
                IsSelected = true
            };
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
            var contact = _contactService.GetOrCreateContact("", user.FullName, user.EmailAddress, user.PhoneNumber, user.Id);

            ProcessPurchase(new PurchaseModel
            {
                ItemId = subscription.Id,
                ContactId = contact.Id
            });

            _userService.UsePromoCode(user.Id, code);
        }

        public void ProcessPurchase(PurchaseModel purchaseModel)
        {
            UserMembership userMembership = null;
            MembershipOption membershipOption = null;

            try
            {
                var membershipOptionId = purchaseModel.ItemId;
                membershipOption = _membershipOptionRepository.Find(membershipOptionId);

                if (membershipOption == null)
                {
                    _logger.Error(
                        $"MembershipService => ProcessPurchase => No MembershipOption with id {membershipOptionId} was found.");
                    throw new IndexOutOfRangeException("Invalid MembershipOptionId");
                }

                userMembership = new DataAccessLayer.Models.UserMembership
                {
                    UserId = Current.UserId,
                    MembershipOptionId = membershipOptionId,
                    StartsOn = DateTime.Today,
                    IsAutoRenew = true
                };
                SetMembershipEndDate(userMembership);

                TerminateExistingMemberships(userMembership.UserId);

                _userMembershipRepository.Create(userMembership);
                userMembership.User = _usersRepository.Find(Current.UserId);

                if (membershipOption.SubscriptionType >= MembershipOption.ESubscriptionType.AnnualPlatinum)
                {
                    CreateComplementaryUserConsultation(Current.UserId);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"MembershipService => ProcessPurchase => Purchase failed: {ex.GetFullErrorMessage()}");
                SendEmailToNineStarAboutFailure(purchaseModel, ex.GetFullErrorMessage());
                throw ex;
            }

            var user = userMembership.User;
            try
            {
                var contact = _contactService.Find(purchaseModel.CustomerEmailAddress);
                if (contact == null)
                {
                    contact = _contactService.GetOrCreateContact("", purchaseModel.CustomerName, purchaseModel.CustomerEmailAddress, "",
                        user.Id);
                }
            }
            catch (Exception e)
            {
                _logger.Error($"MembershipService => ProcessPurchase => Get contact record failed failed for user: {user.Id} {e.GetFullErrorMessage()}");
                SendEmailToNineStarAboutFailure(purchaseModel, e.GetFullErrorMessage());
            }

            try
            {
                userMembership.MembershipOption = membershipOption;
                SendEmailToNineStar(purchaseModel, userMembership);
                SendEmailToUser(purchaseModel, userMembership);
            }
            catch (Exception e)
            {
                _logger.Error($"MembershipService => ProcessPurchase => Send Emails failed: {e.GetFullErrorMessage()}");
            }
        }

        private void SetMembershipEndDate(DataAccessLayer.Models.UserMembership membership)
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

        public void AssignMembershipToUser(int membershipOptionId, int userId, PromoCode promoCode = null)
        {
            try
            {
                var membershipOption = _membershipOptionRepository.Find(membershipOptionId);
                if (membershipOption == null)
                {
                    _logger.Error($"MembershipService => AssignMembershipToUser => No MembershipOption with id {membershipOptionId} was found.");
                    throw new IndexOutOfRangeException("Invalid MembershipOptionId");
                }

                var userMembership = new DataAccessLayer.Models.UserMembership
                {
                    UserId = userId,
                    MembershipOptionId = membershipOptionId,
                    StartsOn = DateTime.Today,
                    IsAutoRenew = true
                };
                SetMembershipEndDate(userMembership);

                TerminateExistingMemberships(userId);

                _userMembershipRepository.Create(userMembership);
                userMembership.User = _usersRepository.Find(userId);
            }
            catch (Exception ex)
            {
                _logger.Error($"MembershipService => AssignMembershipToUser => Assign Membership failed: {ex.GetFullErrorMessage()}");
                throw ex;
            }
        }

        public void ProcessSwitch(int membershipOptionId)
        {
            PurchaseModel purchaseModel = null;

            try
            {
                var membershipOption = _membershipOptionRepository.Find(membershipOptionId);
                if (membershipOption == null)
                {
                    _logger.Error($"MembershipService => ProcessSwitch => No MembershipOption with id {membershipOptionId} was found.");
                    throw new IndexOutOfRangeException("Invalid MembershipOptionId");
                }

                var userMembership = new DataAccessLayer.Models.UserMembership
                {
                    UserId = Current.UserId,
                    MembershipOptionId = membershipOptionId,
                    StartsOn = DateTime.Today,
                    IsAutoRenew = true
                };
                SetMembershipEndDate(userMembership);

                TerminateExistingMemberships(Current.UserId);

                _userMembershipRepository.Create(userMembership);
                var user = _userService.Find(Current.UserId);
                userMembership.User = user;

                var contact = _contactService.Find(user.EmailAddress);
                if (contact == null)
                {
                    contact = _contactService.GetOrCreateContact("", user.FullName, user.EmailAddress, user.PhoneNumber,
                        user.Id);
                }

                purchaseModel = new PurchaseModel
                {
                    ItemId = membershipOptionId,
                    ContactId = contact.Id,
                    Quantity = 1,
                    Amount = 1,
                    Currency = "USD",
                    CustomerEmailAddress = contact.EmailAddress,
                    CustomerName = contact.FullName
                };

                SendEmailToNineStar(purchaseModel, userMembership);
                SendEmailToUser(purchaseModel, userMembership);
            }
            catch (Exception ex)
            {
                _logger.Error($"MembershipService => ProcessSwitch => Switch failed: {ex.GetFullErrorMessage()}");

                SendEmailToNineStarAboutFailure(purchaseModel, ex.GetFullErrorMessage());
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

                if (_userService.Find(userId) == null)
                {
                    _logger.Error($"MembershipService => CreateFreeMembership => UserId {userId} was not found.");
                    return;
                }

                _userMembershipRepository.Create(new DataAccessLayer.Models.UserMembership
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

        public void CreateComplementaryUserConsultation(int userId, EConsultationDuration duration = EConsultationDuration.OneHour)
        {
            var user = _usersRepository.Find(userId);
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
                _consultationService.CreateConsultation(consultation, contact, userId);
            }
            catch (Exception e)
            {
                _logger.Error($"MembershipService => CreateComplementaryUserConsultation => Error creating consultation => {e.GetFullErrorMessage()}");
            }
        }

        private void TerminateExistingMemberships(int userId)
        {
            var userMemberships = GetActiveUserMemberships(userId);
            var activeUserMemberships = userMemberships.Where(e => e.IsActive).ToList();
            if (!activeUserMemberships.Any())
            {
                _logger.Error($"MembershipService => TerminateExistingMemberships => No active memberships");
                return;
            }
            foreach (var userMembership in activeUserMemberships)
            {
                userMembership.EndsOn = DateTime.Now;
                userMembership.IsDeactivated = true;
                _userMembershipRepository.Update(userMembership);
            }
        }

        private void SendEmailToNineStar(PurchaseModel purchaseModel, DataAccessLayer.Models.UserMembership userMembership)
        {
            var template = Dictionary.MembershipCreatedEmail;
            var title = "We have received a new subscription!";
            _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
            {
                Title = title,
                Customer = purchaseModel.CustomerName,
                CustomerEmail = purchaseModel.CustomerEmailAddress,
                SubscriptionType = userMembership.MembershipOption.SubscriptionTypeNameLocal,
                TotalPrice = userMembership.MembershipOption.FormattedPrice,
                LinkToSummary = _urlHelper.AbsoluteAction("Index", "UserMemberships"),
                Company = _config.CompanyName,
                ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl),
                FailedText = ""
            }), _config.SupportEmailAddress, _config.CompanyName, _config.SupportEmailAddress, _config.CompanyName);
        }

        private void SendEmailToUser(PurchaseModel purchaseModel, UserMembership userMembership)
        {
            var user = userMembership.User;
            var template = Dictionary.NewMembershipThankYouEmail;
            var title = TemplateProcessor.PopulateTemplate(Dictionary.ThankyouForSubscriptionEmailTitle, new
            {
                SubscriptionType = userMembership.MembershipOption.SubscriptionTypeNameLocal
            });

            _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
            {
                Title = title,
                CustomerName = user.FirstName,
                SubscriptionType = userMembership.MembershipOption.SubscriptionTypeNameLocal,
                TotalPrice = userMembership.MembershipOption.FormattedPrice,
                EndsOn = userMembership.EndsOn.ToLongDateString(),
                userMembership.MembershipOption.NumberOfProfileReadings,
                userMembership.MembershipOption.NumberOfCompatibilityReadings,
                ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl),
                PrivacyPolicyLink = _urlHelper.AbsoluteAction("PrivacyPolicy", "Home"),
                TermsOfServiceLink = _urlHelper.AbsoluteAction("TermsOfService", "Home"),
                UnsubscribeLink = _urlHelper.AbsoluteAction("UnsubscribeUser", "Account", new { externalId = user.Name }),
                DateTime.Now.Year
            }), user.EmailAddress, user.FirstName, _config.SupportEmailAddress,
                _config.CompanyName);
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