using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace K9.SharedLibrary.Extensions
{
    public static class StringExtensions
    {

        public static string ToProperCase(this string text)
        {
            return CultureInfo.CurrentUICulture.TextInfo.ToTitleCase(text.ToLower());
        }

        public static string ToCamelCase(this string text)
        {
            return $"{text.Substring(0, 1).ToLower()}{CultureInfo.CurrentUICulture.TextInfo.ToTitleCase(text.Substring(1).ToLower())}";
        }

        public static string RemoveSpaces(this string value)
        {
            return Regex.Replace(value, @"\s+", "");
        }

        public static string SplitOnCapitalLetter(this string value)
        {
            var regex = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

            return regex.Replace(value, " ");
        }

        public static string ToDelimitedString(this IEnumerable<string> list, string delimiter = ",")
        {
            return list.Aggregate("", (a, b) => string.IsNullOrEmpty(a) ? b : $"{a}{delimiter} {b}");
        }

        public static string IsNull(this string value, string text)
        {
            return string.IsNullOrEmpty(value) ? text : value;
        }

        public static string JoinWithOr(this IEnumerable<string> items)
        {
            var list = items?.Where(x => !string.IsNullOrWhiteSpace(x)).ToList() ?? new List<string>();

            if (list.Count == 0) return string.Empty;
            if (list.Count == 1) return list[0];
            if (list.Count == 2) return $"{list[0]} or {list[1]}";

            var allButLast = list.Take(list.Count - 1);
            var last = list.Last();

            return $"{string.Join(", ", allButLast)} or {last}";
        }

    }
}
