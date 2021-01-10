using K9.Base.DataAccessLayer.Enums;
using K9.WebApplication.Models;
using System;
using System.Globalization;

namespace K9.WebApplication.Helpers
{
    public static class SessionHelper
    {

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

        public static void SetLastProfile(NineStarKiModel model)
        {
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastProfileDateOfBirth, model.PersonModel.DateOfBirth.ToString(CultureInfo.InvariantCulture));
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastProfileGender, model.PersonModel.Gender);
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.IsRetrieveProfile, true);
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.ProfileStoredOn, DateTime.Today);
        }

        public static void ClearLastProfile()
        {
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.IsRetrieveProfile, false);
        }

        public static PersonModel GetLastProfile(bool todayOnly = false)
        {
            if (Base.WebApplication.Helpers.SessionHelper.GetBoolValue(Constants.SessionConstants.IsRetrieveProfile) && (!todayOnly || GetDateTimeValue(Constants.SessionConstants.ProfileStoredOn) == DateTime.Today))
            {
                DateTime.TryParse(Base.WebApplication.Helpers.SessionHelper.GetStringValue(Constants.SessionConstants.LastProfileDateOfBirth), out var dateOfBirth);
                Enum.TryParse<EGender>(Base.WebApplication.Helpers.SessionHelper.GetStringValue(Constants.SessionConstants.LastProfileGender), out var gender);

                ClearLastProfile();

                return new PersonModel
                {
                    DateOfBirth = dateOfBirth,
                    Gender = gender
                };
            }
            return null;
        }

        public static void SetLastCompatibility(CompatibilityModel model)
        {
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastCompatibilityProfileDateOfBirth1, model.NineStarKiModel1.PersonModel.DateOfBirth.ToString(CultureInfo.InvariantCulture));
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastCompatibilityProfileDateOfBirth2, model.NineStarKiModel2.PersonModel.DateOfBirth.ToString(CultureInfo.InvariantCulture));
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastCompatibilityProfileGender1, model.NineStarKiModel1.PersonModel.Gender);
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastCompatibilityProfileGender2, model.NineStarKiModel2.PersonModel.Gender);

            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.IsRetrieveCompatibility, true);
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.CompatibilityStoredOn, DateTime.Today);
        }

        public static void ClearLastCompatibility()
        {
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.IsRetrieveCompatibility, false);
        }

        public static CompatibilityModel GetLastCompatibility(bool todayOnly = false)
        {
            if (Base.WebApplication.Helpers.SessionHelper.GetBoolValue(Constants.SessionConstants.IsRetrieveCompatibility) && (!todayOnly || GetDateTimeValue(Constants.SessionConstants.CompatibilityStoredOn) == DateTime.Today))
            {
                DateTime.TryParse(Base.WebApplication.Helpers.SessionHelper.GetStringValue(Constants.SessionConstants.LastCompatibilityProfileDateOfBirth1), out var dateOfBirth1);
                DateTime.TryParse(Base.WebApplication.Helpers.SessionHelper.GetStringValue(Constants.SessionConstants.LastCompatibilityProfileDateOfBirth2), out var dateOfBirth2);
                Enum.TryParse<EGender>(Base.WebApplication.Helpers.SessionHelper.GetStringValue(Constants.SessionConstants.LastCompatibilityProfileGender1), out var gender1);
                Enum.TryParse<EGender>(Base.WebApplication.Helpers.SessionHelper.GetStringValue(Constants.SessionConstants.LastCompatibilityProfileGender2), out var gender2);

                ClearLastCompatibility();

                return new CompatibilityModel(
                    new NineStarKiModel(new PersonModel
                    {
                        DateOfBirth = dateOfBirth1,
                        Gender = gender1
                    }),
                    new NineStarKiModel(new PersonModel
                    {
                        DateOfBirth = dateOfBirth2,
                        Gender = gender2
                    }));
            }
            return null;
        }

    }
}