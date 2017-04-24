using System.Globalization;
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
	}
}
