
using System.Globalization;

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

	}
}
