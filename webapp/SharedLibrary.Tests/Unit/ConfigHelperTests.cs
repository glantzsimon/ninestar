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

			var smtpConfig = ConfigHelper.GetConfiguration<SmtpConfiguration>(json).Value;
			var dbConfig = ConfigHelper.GetConfiguration<DatabaseConfiguration>(json).Value;
			
			Assert.AreEqual("mail.vibranthealthnow.co.uk", smtpConfig.SmtpServer);
			Assert.AreEqual("info@vibranthealthnow.co.uk", smtpConfig.SmtpUserId);
			Assert.AreEqual("12345", smtpConfig.SmtpPassword);
			Assert.AreEqual("info@vibranthealthnow.co.uk", smtpConfig.SmtpFromEmailAddress);
			Assert.AreEqual("Vibrant Health", smtpConfig.SmtpFromDisplayName);

			Assert.IsTrue(dbConfig.AutomaticMigrationDataLossAllowed);
		}

	}
}
