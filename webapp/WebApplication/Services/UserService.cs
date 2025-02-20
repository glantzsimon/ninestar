using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Config;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using K9.WebApplication.Packages;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UserMembership = K9.DataAccessLayer.Models.UserMembership;

namespace K9.WebApplication.Services
{
    public class UserService : IUserService
    {
        public INineStarKiPackage Package { get; }

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

        public UserService(INineStarKiPackage package, IRepository<PromoCode> promoCodesRepository, IRepository<UserPromoCode> userPromoCodeRepository, IRepository<UserConsultation> userConsultationsRepository, IRepository<Consultation> consultationsRepository, IConsultationService consultationService,
            IRepository<UserMembership> userMembershipsRepository, IRepository<UserOTP> userOtpRepository)
        {
            Package = package;

            _userPromoCodeRepository = userPromoCodeRepository;
            _userConsultationsRepository = userConsultationsRepository;
            _consultationsRepository = consultationsRepository;
            _consultationService = consultationService;
            _userMembershipsRepository = userMembershipsRepository;
            _userOtpRepository = userOtpRepository;
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
        
        public User Find(int id)
        {
            return _usersRepository.Find(id);
        }

        public User Find(string username)
        {
            return _usersRepository.Find(e => e.Username == username).FirstOrDefault();
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
                    _userRolesRepository.Delete(userRole.Id);
                }

                var userConsultations = _userConsultationsRepository.Find(e => e.UserId == user.Id);
                foreach (var userConsultation in userConsultations)
                {
                    _userConsultationsRepository.Delete(userConsultation.Id);
                }

                var contactRecord = _contactsRepository.Find(e => e.EmailAddress == user.EmailAddress).FirstOrDefault();
                if (contactRecord != null)
                {
                    var consultations = _consultationsRepository.Find(e => e.ContactId == contactRecord.Id);
                    foreach (var consultation in consultations)
                    {
                        var itemToUpdate = _consultationsRepository.Find(consultation.Id);
                        itemToUpdate.ContactId = 2; // SYSTEM
                        _consultationsRepository.Update(itemToUpdate);
                    }

                    _contactsRepository.Delete(contactRecord.Id);
                }

                var userMemberships = _userMembershipsRepository.Find(e => e.UserId == user.Id);
                foreach (var userMembership in userMemberships)
                {
                    _userMembershipsRepository.Delete(userMembership.Id);
                }

                var userOTPs = _userOtpRepository.Find(e => e.UserId == user.Id);
                foreach (var userOTP in userOTPs)
                {
                    _userOtpRepository.Delete(userOTP.Id);
                }

                var userPromoCodes = _userPromoCodeRepository.Find(e => e.UserId == user.Id);
                foreach (var userPromoCode in userPromoCodes)
                {
                    _userPromoCodeRepository.Delete(userPromoCode.Id);
                }

                _usersRepository.GetQuery($"DELETE FROM [User] WHERE Id = {user.Id}");

            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, $"UserService => DeleteUser => Error: {e.GetFullErrorMessage()}");
                throw new Exception("Error deleting user account");
            }
        }

        public bool AreMarketingEmailsAllowedForUser(int id)
        {
            var user = _usersRepository.Find(id);
            if (user == null)
            {
                _logger.Log(LogLevel.Error, $"UserService => AreMarketingEmailsAllowedForUser => User with UserId: {id} not found");
                throw new Exception("User not found");
            }
            return !user.IsUnsubscribed;
        }

        public void EnableMarketingEmails(int id, bool value = true)
        {
            EnableMarketingEmailForUser(value, _usersRepository.Find(id));
        }

        public void EnableMarketingEmails(string externalId, bool value = true)
        {
            var user = _usersRepository.Find(e => e.Name == externalId).FirstOrDefault();
            if (user == null)
            {
                _logger.Log(LogLevel.Error, $"UserService => EnableMarketingEmails => User with External Id: {externalId} not found");
                throw new Exception("User not found");
            }

            EnableMarketingEmailForUser(value, user);

            var contact = _contactService.Find(user.EmailAddress);
            if (contact != null)
            {
                try
                {
                    contact.IsUnsubscribed = !value;
                    _contactsRepository.Update(contact);
                }
                catch (Exception e)
                {
                    _logger.Log(LogLevel.Error,
                        $"UserService => EnableMarketingEmails => Could not update contact => ContactId: {contact.Id} Error => {e.GetFullErrorMessage()}");
                    throw;
                }
            }
        }

        private void EnableMarketingEmailForUser(bool value, User user)
        {
            user.IsUnsubscribed = !value;
            try
            {
                _usersRepository.Update(user);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error,
                    $"UserService => EnableMarketingEmailForUser => Could not update user => UserId: {user.Id} => error: {e.GetFullErrorMessage()}");
                throw;
            }
        }
    }
}