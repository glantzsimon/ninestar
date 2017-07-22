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
			return list.Aggregate("", (a, b) => string.IsNullOrEmpty(a) ? b : string.Format("{0}{1} {2}", a, delimiter, b));
		}
	}
}
