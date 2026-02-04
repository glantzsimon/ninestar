using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using K9.WebApplication.Packages;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using UserMembership = K9.DataAccessLayer.Models.UserMembership;

namespace K9.WebApplication.Services
{
    public class UserService : BaseService, IUserService
    {
        private readonly IRepository<UserPromotion> _userPromoCodeRepository;
        private readonly IRepository<UserConsultation> _userConsultationsRepository;
        private readonly IRepository<Consultation> _consultationsRepository;
        private readonly IConsultationService _consultationService;
        private readonly IRepository<UserMembership> _userMembershipsRepository;
        private readonly IRepository<UserOTP> _userOtpRepository;
        private readonly IRepository<UserInfo> _userInfosRepository;
        private readonly IRepository<UserPreference> _userPreferencesRepository;

        public UserService(INineStarKiBasePackage my, IRepository<Promotion> promoCodesRepository, IRepository<UserPromotion> userPromoCodeRepository, IRepository<UserConsultation> userConsultationsRepository, IRepository<Consultation> consultationsRepository, IConsultationService consultationService,
            IRepository<UserMembership> userMembershipsRepository, IRepository<UserOTP> userOtpRepository, IRepository<UserInfo> userInfosRepository,
            IRepository<UserPreference> userPreferencesRepository)
            : base(my)
        {
            _userPromoCodeRepository = userPromoCodeRepository;
            _userConsultationsRepository = userConsultationsRepository;
            _consultationsRepository = consultationsRepository;
            _consultationService = consultationService;
            _userMembershipsRepository = userMembershipsRepository;
            _userOtpRepository = userOtpRepository;
            _userInfosRepository = userInfosRepository;
            _userPreferencesRepository = userPreferencesRepository;
        }

        public void UpdateActiveUserEmailAddressIfFromFacebook(Contact contact)
        {
            if (My.Authentication.IsAuthenticated)
            {
                var activeUser = My.UsersRepository.Find(Current.UserId);
                var defaultFacebookAddress = $"{activeUser.FirstName}.{activeUser.LastName}@facebook.com";
                if (activeUser.IsOAuth && activeUser.EmailAddress == defaultFacebookAddress && activeUser.EmailAddress != contact.EmailAddress)
                {
                    if (!My.UsersRepository.Find(e => e.EmailAddress == contact.EmailAddress).Any())
                    {
                        activeUser.EmailAddress = contact.EmailAddress;
                        My.UsersRepository.Update(activeUser);
                    }
                }
            }
        }

        public User Find(int id)
        {
            return My.UsersRepository.Find(id);
        }

        public User Find(string username)
        {
            return My.UsersRepository.Find(e => e.Username == username).FirstOrDefault();
        }

        public User Find(Guid id)
        {
            var info = _userInfosRepository.Find(e => e.UniqueIdentifier == id).FirstOrDefault();
            return My.UsersRepository.Find(info?.UserId ?? 0);
        }

        public bool UserIsAdmin(int userId)
        {
            var adminRole = My.RolesRepository.Find(e => e.Name == Constants.Constants.Administrator).First();
            return My.UserRolesRepository.Exists(e => e.UserId == userId && e.RoleId == adminRole.Id);
        }

        public UserInfo GetOrCreateUserInfo(int userId)
        {
            var userInfo = _userInfosRepository.Find(e => e.UserId == userId).FirstOrDefault();
            if (userInfo != null)
            {
                return userInfo;
            }

            var newUserInfo = new UserInfo
            {
                UserId = userId,
                UniqueIdentifier = Guid.NewGuid()
            };

            _userInfosRepository.Create(newUserInfo);
            return newUserInfo;
        }

        public List<UserConsultation> GetPendingConsultations(int? userId = null)
        {
            var user = My.UsersRepository.Find(userId ?? Current.UserId);
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
                        userConsultation.User = My.UsersRepository.Find(userConsultation.UserId);
                    }
                }
                catch (Exception e)
                {
                    My.Logger.Error($"UserService => GetConsultations => {e.GetFullErrorMessage()}");
                }
            }

            return userConsultations.Where(e => !e.Consultation.ScheduledOn.HasValue || e.Consultation.ScheduledOn >= DateTime.Today).ToList();
        }

        public void UpdateUserInfo(UserInfo userInfo)
        {
            _userInfosRepository.GetQuery($"UPDATE {nameof(UserInfo)} SET " +
                                          $"{nameof(UserInfo.TimeOfBirth)} = '{userInfo.TimeOfBirth.ToString()}', " +
                                          $"{nameof(UserInfo.AvatarImageUrl)} = '{userInfo.AvatarImageUrl}', " +
                                          $"{nameof(UserInfo.BirthTimeZoneId)} = '{userInfo.BirthTimeZoneId.ToString()}' " +
                                          $"WHERE Id = {userInfo.Id}");
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
                var userRoles = My.UserRolesRepository.Find(e => e.UserId == user.Id);
                foreach (var userRole in userRoles)
                {
                    My.UserRolesRepository.Delete(userRole.Id);
                }

                var userConsultations = _userConsultationsRepository.Find(e => e.UserId == user.Id);
                foreach (var userConsultation in userConsultations)
                {
                    _userConsultationsRepository.Delete(userConsultation.Id);
                }

                var contactRecord = My.ContactsRepository.Find(e => e.EmailAddress == user.EmailAddress).FirstOrDefault();
                if (contactRecord != null)
                {
                    var consultations = _consultationsRepository.Find(e => e.ContactId == contactRecord.Id);
                    foreach (var consultation in consultations)
                    {
                        var itemToUpdate = _consultationsRepository.Find(consultation.Id);
                        itemToUpdate.ContactId = 2; // SYSTEM
                        _consultationsRepository.Update(itemToUpdate);
                    }

                    My.ContactsRepository.Delete(contactRecord.Id);
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

                My.UsersRepository.GetQuery($"DELETE FROM [User] WHERE Id = {user.Id}");

            }
            catch (Exception e)
            {
                My.Logger.Log(LogLevel.Error, $"UserService => DeleteUser => Error: {e.GetFullErrorMessage()}");
                throw new Exception("Error deleting user account");
            }
        }

        public bool AreMarketingEmailsAllowedForUser(int id)
        {
            var user = My.UsersRepository.Find(id);
            if (user == null)
            {
                My.Logger.Log(LogLevel.Error, $"UserService => AreMarketingEmailsAllowedForUser => User with UserId: {id} not found");
                throw new Exception("User not found");
            }
            return !user.IsUnsubscribed;
        }

        public void EnableMarketingEmails(int id, bool value = true)
        {
            EnableMarketingEmailForUser(value, My.UsersRepository.Find(id));
        }

        public void EnableMarketingEmails(string externalId, bool value = true)
        {
            var user = My.UsersRepository.Find(e => e.Name == externalId).FirstOrDefault();
            if (user == null)
            {
                My.Logger.Log(LogLevel.Error, $"UserService => EnableMarketingEmails => User with External Id: {externalId} not found");
                throw new Exception("User not found");
            }

            EnableMarketingEmailForUser(value, user);

            var contact = My.ContactsRepository.Find(e => e.EmailAddress == user.EmailAddress).FirstOrDefault();
            if (contact != null)
            {
                try
                {
                    contact.IsUnsubscribed = !value;
                    My.ContactsRepository.Update(contact);
                }
                catch (Exception e)
                {
                    My.Logger.Log(LogLevel.Error,
                        $"UserService => EnableMarketingEmails => Could not update contact => ContactId: {contact.Id} Error => {e.GetFullErrorMessage()}");
                    throw;
                }
            }
        }

        public void UpdateUserPreference(int userId, string key, object value)
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be greater than 0.", nameof(userId));

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Preference key cannot be null or whitespace.", nameof(key));

            var valueType = value.GetType().FullName;
            string valueAsString = Convert.ToString(value, CultureInfo.InvariantCulture); // general format

            var existing = _userPreferencesRepository.Find(e => e.UserId == userId && e.Key == key).FirstOrDefault();
            if (existing != null)
            {
                existing.ValueType = valueType;
                existing.Value = valueAsString;
                _userPreferencesRepository.Update(existing);
            }
            else
            {
                _userPreferencesRepository.Create(new UserPreference
                {
                    UserId = userId,
                    Key = key,
                    Value = valueAsString,
                    ValueType = valueType
                });
            }

            SessionHelper.SetValue(key, value);

            InitUserPreferences(userId);
        }

        public T GetUserPreference<T>(int userId, string key, T defaultValue = default)
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be greater than 0.", nameof(userId));

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Preference key cannot be null or whitespace.", nameof(key));

            var pref = _userPreferencesRepository
                .Find(e => e.UserId == userId && e.Key == key)
                .FirstOrDefault();

            if (pref == null || string.IsNullOrEmpty(pref.Value))
                return defaultValue;

            try
            {
                var targetType = typeof(T);

                if (targetType == typeof(string))
                    return (T)(object)pref.Value;

                if (targetType.IsEnum)
                    return (T)Enum.Parse(targetType, pref.Value, ignoreCase: true);

                return (T)Convert.ChangeType(pref.Value, targetType, CultureInfo.InvariantCulture);
            }
            catch
            {
                return defaultValue;
            }
        }

        public void InitUserPreferences(string username)
        {
            var user = Find(username);
            if (user == null)
            {
                throw new ArgumentException($"User with username '{username}' not found.", nameof(username));
            }

            InitUserPreferences(user.Id);
        }

        public void InitUserPreferences(int userId)
        {
            var user = Find(userId);
            if (user == null)
            {
                throw new ArgumentException($"User with ID '{userId}' not found.", nameof(userId));
            }

            SessionHelper.UserPreferences = _userPreferencesRepository
                .Find(e => e.UserId == user.Id)
                .ToList();
        }

        public void ClearUserPreferences()
        {
            SessionHelper.UserPreferences = new List<UserPreference>();
        }

        private void EnableMarketingEmailForUser(bool value, User user)
        {
            user.IsUnsubscribed = !value;
            try
            {
                My.UsersRepository.Update(user);
            }
            catch (Exception e)
            {
                My.Logger.Log(LogLevel.Error,
                    $"UserService => EnableMarketingEmailForUser => Could not update user => UserId: {user.Id} => error: {e.GetFullErrorMessage()}");
                throw;
            }
        }
    }
}