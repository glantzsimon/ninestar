using System;
using System.IO;
using K9.DataAccess.Config;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using Xunit;

namespace K9.SharedLibrary.Tests.Unit
{
	public class ConfigHelperTests
	{
		[Fact]
		public void ConfigHelper_ShouldCreateConfiguration_FromJson()
		{
			var json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../appsettings.json"));

			var smtpConfig = ConfigHelper.GetConfiguration<SmtpConfiguration>(json).Value;
			var dbConfig = ConfigHelper.GetConfiguration<DatabaseConfiguration>(json).Value;
			
			Assert.Equal("mail.vibranthealthnow.co.uk", smtpConfig.SmtpServer);
			Assert.Equal("info@vibranthealthnow.co.uk", smtpConfig.SmtpUserId);
			Assert.Equal("12345", smtpConfig.SmtpPassword);
			Assert.Equal("info@vibranthealthnow.co.uk", smtpConfig.SmtpFromEmailAddress);
			Assert.Equal("Vibrant Health", smtpConfig.SmtpFromDisplayName);

			Assert.True(dbConfig.AutomaticMigrationDataLossAllowed);
		}

	}
}
