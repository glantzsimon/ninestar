using K9.Base.DataAccessLayer.Enums;
using K9.Base.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using System;
using System.Linq;

namespace K9.WebApplication.Helpers
{
    public static class SessionHelper
    {
        public static string GetStringValue(string key)
        {
            var value = Base.WebApplication.Helpers.SessionHelper.GetValue(key);
            var stringValue = value?.ToString() ?? string.Empty;
            return stringValue;
        }

        public static int GetIntValue(string key)
        {
            var value = Base.WebApplication.Helpers.SessionHelper.GetValue(key);
            var stringValue = value?.ToString() ?? string.Empty;
            int.TryParse(stringValue, out var intValue);
            return intValue;
        }

        public static DateTime? GetDateTimeValue(string key)
        {
            var value = Base.WebApplication.Helpers.SessionHelper.GetValue(key);
            var stringValue = value?.ToString() ?? string.Empty;
            if (DateTime.TryParse(stringValue, out var dateTimeValue))
            {
                return dateTimeValue;
            }
            return null;
        }

        public static bool GetBooleanValue(string key)
        {
            var value = Base.WebApplication.Helpers.SessionHelper.GetValue(key);
            var stringValue = value?.ToString() ?? string.Empty;
            if (bool.TryParse(stringValue, out var boolValue))
            {
                return boolValue;
            }
            return false;
        }

        public static void SetCurrentUserTimeZone(string value)
        {
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.UserTimeZone, value);
        }

        public static string GetCurrentUserTimeZone()
        {
            var value = Base.WebApplication.Helpers.SessionHelper.GetValue(Constants.SessionConstants.UserTimeZone);
            var stringValue = value?.ToString() ?? string.Empty;
            return stringValue;
        }

        public static int GetCurrentUserId()
        {
            return GetIntValue(Constants.SessionConstants.CurrentUserId);
        }

        public static void SetCurrentUserId(int value)
        {
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.CurrentUserId, value);
        }

        public static void CleaCurrentUserId()
        {
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.CurrentUserId, 0);
        }

        public static string GetCurrentUserName()
        {
            return GetStringValue(Constants.SessionConstants.CurrentUserName);
        }

        public static void SetCurrentUserName(string value)
        {
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.CurrentUserName, value);
        }

        public static void ClearCurrentUserName()
        {
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.CurrentUserName, "");
        }

        public static void SetLastProfile(NineStarKiModel model)
        {
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastProfileDateOfBirth, model.PersonModel.DateOfBirth.ToString(Constants.FormatConstants.SessionDateTimeFormat));
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastProfileGender, model.PersonModel.Gender);
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastProfileName, model.PersonModel.Name);
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.IsRetrieveProfile, true);
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.ProfileStoredOn, DateTime.Now);
        }

        public static void ClearLastProfile()
        {
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.IsRetrieveProfile, false);
        }

        public static RetrieveLastModel GetLastProfile(bool todayOnly = false, bool remove = true)
        {
            var storedOn = GetDateTimeValue(Constants.SessionConstants.ProfileStoredOn);

            if (Base.WebApplication.Helpers.SessionHelper.GetBoolValue(Constants.SessionConstants.IsRetrieveProfile) && (!todayOnly || storedOn?.Date == DateTime.Today))
            {
                DateTime.TryParse(Base.WebApplication.Helpers.SessionHelper.GetStringValue(Constants.SessionConstants.LastProfileDateOfBirth), out var dateOfBirth);
                Enum.TryParse<EGender>(Base.WebApplication.Helpers.SessionHelper.GetStringValue(Constants.SessionConstants.LastProfileGender), out var gender);

                if (remove)
                    ClearLastProfile();

                var model = new PersonModel
                {
                    DateOfBirth = dateOfBirth,
                    Gender = gender,
                    Name = Base.WebApplication.Helpers.SessionHelper.GetStringValue(Constants.SessionConstants.LastProfileName)
                };

                return new RetrieveLastModel
                {
                    Section = ESection.Profile,
                    StoredOn = storedOn,
                    PersonModel = model
                };
            }
            return null;
        }

        public static void SetLastPrediction(NineStarKiModel model)
        {
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastPredictionDateOfBirth, model.PersonModel.DateOfBirth.ToString(Constants.FormatConstants.SessionDateTimeFormat));
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastPredictionGender, model.PersonModel.Gender);
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastPredictionName, model.PersonModel.Name);
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.IsRetrievePrediction, true);
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.PredictionStoredOn, DateTime.Now);
        }

        public static void ClearLastPrediction()
        {
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.IsRetrievePrediction, false);
        }

        public static RetrieveLastModel GetLastPrediction(bool todayOnly = false, bool remove = true)
        {
            var storedOn = GetDateTimeValue(Constants.SessionConstants.PredictionStoredOn);

            if (Base.WebApplication.Helpers.SessionHelper.GetBoolValue(Constants.SessionConstants.IsRetrievePrediction) && (!todayOnly || storedOn?.Date == DateTime.Today))
            {
                DateTime.TryParse(Base.WebApplication.Helpers.SessionHelper.GetStringValue(Constants.SessionConstants.LastPredictionDateOfBirth), out var dateOfBirth);
                Enum.TryParse<EGender>(Base.WebApplication.Helpers.SessionHelper.GetStringValue(Constants.SessionConstants.LastPredictionGender), out var gender);

                if (remove)
                    ClearLastPrediction();

                var model = new PersonModel
                {
                    DateOfBirth = dateOfBirth,
                    Gender = gender,
                    Name = Base.WebApplication.Helpers.SessionHelper.GetStringValue(Constants.SessionConstants.LastPredictionName)
                };

                return new RetrieveLastModel
                {
                    Section = ESection.Predictions,
                    StoredOn = storedOn,
                    PersonModel = model
                };
            }
            return null;
        }

        public static void SetLastBiorhythm(NineStarKiModel model)
        {
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastBiorhythmDateOfBirth, model.PersonModel.DateOfBirth.ToString(Constants.FormatConstants.SessionDateTimeFormat));
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastBiorhythmGender, model.PersonModel.Gender);
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastBiorhythmName, model.PersonModel.Name);
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.IsRetrieveBiorhythm, true);
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.BiorhythmStoredOn, DateTime.Now);
        }

        public static void ClearLastBiorhythm()
        {
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.IsRetrieveBiorhythm, false);
        }

        public static RetrieveLastModel GetLastBiorhythm(bool todayOnly = false, bool remove = true)
        {
            var storedOn = GetDateTimeValue(Constants.SessionConstants.BiorhythmStoredOn);

            if (Base.WebApplication.Helpers.SessionHelper.GetBoolValue(Constants.SessionConstants.IsRetrieveBiorhythm) && (!todayOnly || storedOn?.Date == DateTime.Today))
            {
                DateTime.TryParse(Base.WebApplication.Helpers.SessionHelper.GetStringValue(Constants.SessionConstants.LastBiorhythmDateOfBirth), out var dateOfBirth);
                Enum.TryParse<EGender>(Base.WebApplication.Helpers.SessionHelper.GetStringValue(Constants.SessionConstants.LastBiorhythmGender), out var gender);

                if (remove)
                    ClearLastBiorhythm();

                var model = new PersonModel
                {
                    DateOfBirth = dateOfBirth,
                    Gender = gender,
                    Name = Base.WebApplication.Helpers.SessionHelper.GetStringValue(Constants.SessionConstants.LastBiorhythmName)
                };

                return new RetrieveLastModel
                {
                    Section = ESection.Biorhythms,
                    StoredOn = storedOn,
                    PersonModel = model
                };
            }
            return null;
        }

        public static void SetLastKnowledgeBase(string value)
        {
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastKnowledgeBase, value);
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.IsRetrieveKnowledgeBase, true);
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.KnowledgeBaseStoredOn, DateTime.Now);
        }

        public static void ClearLastKnowledgeBase()
        {
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.IsRetrieveKnowledgeBase, false);
        }

        public static RetrieveLastModel GetLastKnowledgeBase(bool todayOnly = false, bool remove = true)
        {
            var storedOn = GetDateTimeValue(Constants.SessionConstants.KnowledgeBaseStoredOn);

            if (Base.WebApplication.Helpers.SessionHelper.GetBoolValue(Constants.SessionConstants.IsRetrieveKnowledgeBase) && (!todayOnly || storedOn?.Date == DateTime.Today))
            {
                if (remove)
                    ClearLastKnowledgeBase();

                return new RetrieveLastModel
                {
                    Section = ESection.KnowledgeBase,
                    StoredOn = storedOn,
                    Value = Base.WebApplication.Helpers.SessionHelper.GetStringValue(
                        Constants.SessionConstants.LastKnowledgeBase)
                };
            }
            return null;
        }

        public static void SetLastCompatibility(CompatibilityModel model)
        {
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastCompatibilityProfileDateOfBirth1, model.NineStarKiModel1.PersonModel.DateOfBirth.ToString(Constants.FormatConstants.SessionDateTimeFormat));
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastCompatibilityProfileDateOfBirth2, model.NineStarKiModel2.PersonModel.DateOfBirth.ToString(Constants.FormatConstants.SessionDateTimeFormat));
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastCompatibilityProfileGender1, model.NineStarKiModel1.PersonModel.Gender);
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastCompatibilityProfileGender2, model.NineStarKiModel2.PersonModel.Gender);
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastCompatibilityProfileName1, model.NineStarKiModel1.PersonModel.Name);
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastCompatibilityProfileName2, model.NineStarKiModel2.PersonModel.Name);
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastCompatibilityHideSexuality, model.IsHideSexualChemistry);

            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.IsRetrieveCompatibility, true);
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.CompatibilityStoredOn, DateTime.Now);
        }

        public static void ClearLastCompatibility()
        {
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.IsRetrieveCompatibility, false);
        }

        public static RetrieveLastModel GetLastCompatibility(bool todayOnly = false, bool remove = true)
        {
            var storedOn = GetDateTimeValue(Constants.SessionConstants.CompatibilityStoredOn);

            if (Base.WebApplication.Helpers.SessionHelper.GetBoolValue(Constants.SessionConstants.IsRetrieveCompatibility) && (!todayOnly || storedOn?.Date == DateTime.Today))
            {
                DateTime.TryParse(Base.WebApplication.Helpers.SessionHelper.GetStringValue(Constants.SessionConstants.LastCompatibilityProfileDateOfBirth1), out var dateOfBirth1);
                DateTime.TryParse(Base.WebApplication.Helpers.SessionHelper.GetStringValue(Constants.SessionConstants.LastCompatibilityProfileDateOfBirth2), out var dateOfBirth2);
                Enum.TryParse<EGender>(Base.WebApplication.Helpers.SessionHelper.GetStringValue(Constants.SessionConstants.LastCompatibilityProfileGender1), out var gender1);
                Enum.TryParse<EGender>(Base.WebApplication.Helpers.SessionHelper.GetStringValue(Constants.SessionConstants.LastCompatibilityProfileGender2), out var gender2);
                var hideSexuality = Base.WebApplication.Helpers.SessionHelper.GetBoolValue(Constants.SessionConstants.LastCompatibilityHideSexuality);

                if (remove)
                    ClearLastCompatibility();

                var model = new CompatibilityModel(
                    new NineStarKiModel(new PersonModel
                    {
                        DateOfBirth = dateOfBirth1,
                        Gender = gender1,
                        Name = Base.WebApplication.Helpers.SessionHelper.GetStringValue(Constants.SessionConstants.LastCompatibilityProfileName1)
                    }),
                    new NineStarKiModel(new PersonModel
                    {
                        DateOfBirth = dateOfBirth2,
                        Gender = gender2,
                        Name = Base.WebApplication.Helpers.SessionHelper.GetStringValue(Constants.SessionConstants.LastCompatibilityProfileName2)
                    }))
                {
                    IsHideSexualChemistry = hideSexuality
                };

                return new RetrieveLastModel
                {
                    Section = ESection.Compatibility,
                    StoredOn = storedOn,
                    CompatibilityModel = model
                };
            }
            return null;
        }

        public static void SetCurrentUserRoles(IRepository<Role> rolesRepository, IRepository<UserRole> userRolesRepository, int userId)
        {
            var adminRole = rolesRepository.Find(e => e.Name == Constants.Constants.Administrator).First();
            var powerUserRole = rolesRepository.Find(e => e.Name == Constants.Constants.ClientUser).First();
            var clientRole = rolesRepository.Find(e => e.Name == Constants.Constants.ClientUser).First();

            var isAdmin = userRolesRepository.Exists(e => e.UserId == userId && e.RoleId == adminRole.Id);
            var isPower = userRolesRepository.Exists(e => e.UserId == userId && e.RoleId == powerUserRole.Id);
            var isClient = userRolesRepository.Exists(e => e.UserId == userId && e.RoleId == clientRole.Id);

            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.Constants.Administrator, isAdmin);
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.Constants.PowerUser, isPower);
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.Constants.ClientUser, isClient);
        }

        public static bool CurrentUserIsAdmin() => GetBooleanValue(Constants.Constants.Administrator);
        public static bool CurrentUserIsPowertUser() => GetBooleanValue(Constants.Constants.PowerUser);
        public static bool CurrentUserIsClientUser() => GetBooleanValue(Constants.Constants.ClientUser);
    }
}