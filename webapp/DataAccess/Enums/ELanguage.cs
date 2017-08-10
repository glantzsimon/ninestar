
using K9.DataAccess.Attributes;
using K9.Globalisation;

namespace K9.DataAccess.Enums
{
	public enum ELanguage
	{
		[EnumDescription(ResourceType = typeof(Dictionary), LanguageCode = Strings.LanguageCodes.En, Name = Strings.Names.English)]
		English,
		[EnumDescription(ResourceType = typeof(Dictionary), LanguageCode = Strings.LanguageCodes.Fr, Name = Strings.Names.French)]
		French
	}
}
