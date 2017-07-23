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

			Assert.AreEqual("mail.vibranthealthnow.co.uk", config.Value.SmtpServer);
			Assert.AreEqual("info@vibranthealthnow.co.uk", config.Value.SmtpUserId);
			Assert.AreEqual("12345", config.Value.SmtpPassword);
			Assert.AreEqual("info@vibranthealthnow.co.uk", config.Value.SmtpFromEmailAddress);
			Assert.AreEqual("Vibrant Health", config.Value.SmtpFromDisplayName);
		}

	}
}
