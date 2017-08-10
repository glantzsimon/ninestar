
using System.Globalization;
using System.Threading;
using K9.DataAccess.Enums;
using K9.DataAccess.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace K9.DataAccess.Tests.Unit
{
	[TestClass]
	public class EnumDescriptionTests
	{

		[TestMethod]
		public void ELanguage_GetLanguageDescription_ShouldReturnCorrectLanguage()
		{
			var english = ELanguage.English.GetLocalisedLanguageName();
			var french = ELanguage.French.GetLocalisedLanguageName();

			Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr");
			var anglais = ELanguage.English.GetLocalisedLanguageName();
			var francais = ELanguage.French.GetLocalisedLanguageName();
			
			Assert.AreEqual("English", english);
			Assert.AreEqual("Anglais", anglais);
			Assert.AreEqual("French", french);
			Assert.AreEqual("Français", francais);
		}

		[TestMethod]
		public void ELanguage_GetLanguageCode_ShouldReturnCorrectLanguageCode()
		{
			var languageCode = ELanguage.English.GetLanguageCode();
			var languageCodeFr = ELanguage.French.GetLanguageCode();
			
			Assert.AreEqual("en", languageCode);
			Assert.AreEqual("fr", languageCodeFr);
		}

	}
}
