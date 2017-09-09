
using System.Globalization;
using System.Threading;
using K9.DataAccess.Enums;
using K9.DataAccess.Extensions;
using Xunit;

namespace K9.DataAccess.Tests.Unit
{
	public class EnumDescriptionTests
	{

		[Fact]
		public void ELanguage_GetLanguageDescription_ShouldReturnCorrectLanguage()
		{
		    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

            var english = ELanguage.English.GetLocalisedLanguageName();
			var french = ELanguage.French.GetLocalisedLanguageName();

			Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr");
			var anglais = ELanguage.English.GetLocalisedLanguageName();
			var francais = ELanguage.French.GetLocalisedLanguageName();
			
			Assert.Equal("English", english);
			Assert.Equal("Anglais", anglais);
			Assert.Equal("French", french);
			Assert.Equal("Français", francais);
		}

		public void ELanguage_GetLanguageCode_ShouldReturnCorrectLanguageCode()
		{
			var languageCode = ELanguage.English.GetLanguageCode();
			var languageCodeFr = ELanguage.French.GetLanguageCode();
			
			Assert.Equal("en", languageCode);
			Assert.Equal("fr", languageCodeFr);
		}

	}
}
