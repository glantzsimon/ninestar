using K9.WebApplication.Enums;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace K9.WebApplication.Extensions
{
    public static class Extensions
    {
        public static bool IsYin(this EGender gender)
        {
            return new List<EGender>
            {
                EGender.Female,
                EGender.TransFemale,
                EGender.Other,
            }.Contains(gender);
        }

        public static string ToSeoFriendlyString(this string value)
        {
            var regex = new Regex("[^a-zA-Z0-9 -]");
            var alphaNumericString = regex.Replace(value, "");

            return string.Join("-", alphaNumericString.ToLower().Split(' '));
        }

        public static string ToPreviewText(this string value, int length = 100)
        {
            var valueLength = value.Length;
            var canBeAbbreviated = valueLength > length;
            var substring = value.Substring(0, canBeAbbreviated ? length : valueLength);
            var abbrevationSuffix = canBeAbbreviated ? "..." : string.Empty;
            return $"{substring}{abbrevationSuffix}";
        }
    }
}
