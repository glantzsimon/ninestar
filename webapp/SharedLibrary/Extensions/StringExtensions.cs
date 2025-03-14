﻿using System.Collections.Generic;
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
	}
}
