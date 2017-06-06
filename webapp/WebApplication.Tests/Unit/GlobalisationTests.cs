using System.Globalization;
using K9.SharedLibrary.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace K9.WebApplication.Tests.Unit
{
	[TestClass]
	public class GlobalisationTests
	{

		[TestMethod]
		public void GetLocaleLanguage_ReturnsCorrectLanguageName()
		{
			var cultureInfo = new CultureInfo("fr-fr");

			Assert.AreEqual("French", cultureInfo.GetLocaleLanguage());
		}

		
	}

}
