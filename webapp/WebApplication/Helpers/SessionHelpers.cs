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

    }
}