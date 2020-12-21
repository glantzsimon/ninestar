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
            var stringValue = value == null ? string.Empty : value.ToString();
            int.TryParse(stringValue, out var intValue);
            return intValue;
        }

        public static void SetLastProfile(PersonModel personModel)
        {
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastProfileDateOfBirth, personModel.DateOfBirth.ToString(CultureInfo.InvariantCulture));
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.LastProfileGender, personModel.Gender);
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.IsRetrieveProfile, true);
        }

        public static void ClearLastProfile()
        {
            Base.WebApplication.Helpers.SessionHelper.SetValue(Constants.SessionConstants.IsRetrieveProfile, false);
        }

        public static PersonModel GetLastProfile()
        {
            if (Base.WebApplication.Helpers.SessionHelper.GetBoolValue(Constants.SessionConstants.IsRetrieveProfile))
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

    }
}