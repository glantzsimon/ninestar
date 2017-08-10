
using K9.DataAccess.Attributes;
using K9.DataAccess.Enums;
using K9.SharedLibrary.Extensions;

namespace K9.DataAccess.Extensions
{
	public static class EnumExtensions
	{

		public static string GetLocalisedLanguageName(this ELanguage language)
		{
			var attr = language.GetAttribute<EnumDescriptionAttribute>();
			return attr.GetDescription();
		}

		public static string GetLanguageCode(this ELanguage language)
		{
			var attr = language.GetAttribute<EnumDescriptionAttribute>();
			return attr.LanguageCode;
		}

	}
}
