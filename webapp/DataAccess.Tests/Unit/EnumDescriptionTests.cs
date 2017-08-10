
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
			var language = ELanguage.English.GetLocalisedLanguageName();
			
			Assert.AreEqual("English", language);
		}

	}
}
