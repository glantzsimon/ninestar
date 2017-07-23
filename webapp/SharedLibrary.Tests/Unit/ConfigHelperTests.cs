using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace K9.SharedLibrary.Tests.Unit
{
	[TestClass]
	public class ConfigHelperTests
	{
		[TestMethod]
		public void ConfigHelper_ShouldCreateConfiguration_FromJson()
		{
			var json = "{\"SmtpConfiguration\": {" +
			           "\"SmtpServer\": \"mail.vibranthealthnow.co.uk\"," +
			           "\"SmtpUserId\": \"info@vibranthealthnow.co.uk\"," +
			           "\"SmtpPassword\": \"12345\"," +
			           "\"SmtpFromEmailAddress\": \"info@vibranthealthnow.co.uk\"," +
			           "\"SmtpFromDisplayName\": \"Vibrant Health\"" +
			           "}}";

			var config = ConfigHelper.GetConfiguration<SmtpConfiguration>(json);

			Assert.AreEqual("mail.vibranthealthnow.co.uk", config.SmtpServer);
			Assert.AreEqual("info@vibranthealthnow.co.uk", config.SmtpUserId);
			Assert.AreEqual("12345", config.SmtpPassword);
			Assert.AreEqual("info@vibranthealthnow.co.uk", config.SmtpFromEmailAddress);
			Assert.AreEqual("Vibrant Health", config.SmtpFromDisplayName);
		}

	}
}
