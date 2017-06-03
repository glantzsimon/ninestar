
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace K9.SharedLibrary.Extensions
{
	public static class Extensions
	{

		public static string GetLocaleName(this CultureInfo cultureInfo)
		{
			var regionInfo = new RegionInfo(cultureInfo.LCID);
			var regionName = regionInfo.TwoLetterISORegionName;
			var languageName = cultureInfo.TwoLetterISOLanguageName;

			return regionName == languageName ? languageName : string.Format("{0}-{1}", languageName, regionName);
		}

		public static string GetFullErrorMessage(this Exception ex)
		{
			var sb = new StringBuilder();
			Exception innerException = ex.InnerException;

			sb.AppendLine(ex.Message);

			while (innerException != null)
			{
				sb.AppendLine(innerException.Message);
				innerException = innerException.InnerException;
			}

			return sb.ToString();
		}

		public static bool IsDuplicateIndexError(this Exception ex)
		{
			return ex.GetFullErrorMessage().Contains("Cannot insert duplicate key row in object");
		}

		public static string GetDuplicateIndexErrorPropertyName(this Exception ex)
		{
			if (IsDuplicateIndexError(ex))
			{
				var indexRegex = new Regex(@"(?:IX_)([A-Z]\w+)");
				var fullErrorMessage = ex.GetFullErrorMessage();
				return indexRegex.IsMatch(fullErrorMessage) ? indexRegex.Match(fullErrorMessage).Groups[1].Value : "";
			}
			return string.Empty;
		}

		public static string GetValueFromResource(this Type resourceType, string key)
		{
			var propInfo = resourceType.GetProperties().FirstOrDefault(p => p.Name == key);
				if (propInfo != null)
				{
					return propInfo.GetValue(null, null).ToString();
				}
			throw new KeyNotFoundException();
		}

	}
}
