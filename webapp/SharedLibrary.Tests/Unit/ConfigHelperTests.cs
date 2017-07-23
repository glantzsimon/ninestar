using System;
using System.IO;
using K9.DataAccess.Config;
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
			var json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../appsettings.json"));

			var smtpConfig = ConfigHelper.GetConfiguration<SmtpConfiguration>(json);
			var dbConfig = ConfigHelper.GetConfiguration<DatabaseConfiguration>(json);
			
			Assert.AreEqual("mail.vibranthealthnow.co.uk", smtpConfig.Value.SmtpServer);
			Assert.AreEqual("info@vibranthealthnow.co.uk", smtpConfig.Value.SmtpUserId);
			Assert.AreEqual("12345", smtpConfig.Value.SmtpPassword);
			Assert.AreEqual("info@vibranthealthnow.co.uk", smtpConfig.Value.SmtpFromEmailAddress);
			Assert.AreEqual("Vibrant Health", smtpConfig.Value.SmtpFromDisplayName);

			Assert.IsTrue(dbConfig.Value.AutomaticMigrationDataLossAllowed);
		}

	}
}
