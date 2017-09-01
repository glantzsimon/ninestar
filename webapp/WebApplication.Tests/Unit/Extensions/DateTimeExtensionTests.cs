using System;
using System.Globalization;
using System.Threading;
using K9.WebApplication.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace K9.WebApplication.Tests.Unit.Extensions
{
	[TestClass]
	public class DateTimeExtensionTests
	{

		private DateTime _justNow = DateTime.Now.Subtract(TimeSpan.FromSeconds(59));
		private DateTime _minuteAgo = DateTime.Now.Subtract(TimeSpan.FromSeconds(74));
		private DateTime _2MinutesAgo = DateTime.Now.Subtract(TimeSpan.FromSeconds(120));
		private DateTime _59MinutesAgo = DateTime.Now.Subtract(TimeSpan.FromMinutes(59).Add(TimeSpan.FromSeconds(50)));
		private DateTime _hourAgo = DateTime.Now.Subtract(TimeSpan.FromHours(1));
		private DateTime _hourAgo59 = DateTime.Now.Subtract(TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(59)));
		private DateTime _twoHoursAgo = DateTime.Now.Subtract(TimeSpan.FromHours(2));
		private DateTime _twoHoursAgo59 = DateTime.Now.Subtract(TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(59)));
		private DateTime _23HoursAgo = DateTime.Now.Subtract(TimeSpan.FromHours(23).Add(TimeSpan.FromMinutes(59)));
		private DateTime _yesterday = DateTime.Now.Subtract(TimeSpan.FromHours(24));
		private DateTime _yesterday23 = DateTime.Now.Subtract(TimeSpan.FromHours(24).Subtract(TimeSpan.FromHours(23)));
		private DateTime _2DaysAgo = DateTime.Now.Subtract(TimeSpan.FromHours(24));
		private DateTime _2DaysAgo23 = DateTime.Now.Subtract(TimeSpan.FromHours(24).Subtract(TimeSpan.FromHours(23)));
		private DateTime _6DaysAgo23 = DateTime.Now.Subtract(TimeSpan.FromDays(6).Subtract(TimeSpan.FromHours(23)).Subtract(TimeSpan.FromMinutes(59)));
		private DateTime _today = DateTime.Today;
		private DateTime _tomorrow = DateTime.Today.AddDays(1);
		private DateTime _days2 = DateTime.Today.AddDays(2);
		private DateTime _days6 = DateTime.Today.AddDays(6);
		private DateTime _week = DateTime.Today.AddDays(7);
		private DateTime _week1Day = DateTime.Today.AddDays(8);
		private DateTime _week6Days = DateTime.Today.AddDays(13);
		private DateTime _2Weeks = DateTime.Today.AddDays(14);
		private DateTime _1Month = DateTime.Today.AddMonths(1);

		[TestMethod]
		public void ToHumanReadableString_ShouldDisplayCorrectValue()
		{
			Assert.AreEqual("Just now", _justNow.ToHumanReadableString());
			Assert.AreEqual("1 minute ago", _minuteAgo.ToHumanReadableString());
			Assert.AreEqual("2 minutes ago", _2MinutesAgo.ToHumanReadableString());
			Assert.AreEqual("59 minutes ago", _59MinutesAgo.ToHumanReadableString());
			Assert.AreEqual("1 hour ago", _hourAgo.ToHumanReadableString());
			Assert.AreEqual("1 hour ago", _hourAgo59.ToHumanReadableString());
			Assert.AreEqual("2 hours ago", _twoHoursAgo.ToHumanReadableString());
			Assert.AreEqual("2 hours ago", _twoHoursAgo59.ToHumanReadableString());
			Assert.AreEqual("23 hours ago", _23HoursAgo.ToHumanReadableString());
			Assert.AreEqual("Yesterday", _yesterday.ToHumanReadableString());
			Assert.AreEqual("Yesterday", _yesterday23.ToHumanReadableString());
			Assert.AreEqual("2 days ago", _2DaysAgo.ToHumanReadableString());
			Assert.AreEqual("2 days ago", _2DaysAgo23.ToHumanReadableString());
			Assert.AreEqual("6 days ago", _6DaysAgo23.ToHumanReadableString());
			//Assert.AreEqual("Today", _today.ToHumanReadableString());
			//Assert.AreEqual("Tomorrow", _tomorrow.ToHumanReadableString());
			//Assert.AreEqual("In 2 days", _days2.ToHumanReadableString());
			//Assert.AreEqual("In 6 days", _days6.ToHumanReadableString());
			//Assert.AreEqual("In 1 week", _week.ToHumanReadableString());
			//Assert.AreEqual("In 1 week and 1 day", _week1Day.ToHumanReadableString());
			//Assert.AreEqual("In 1 week and 6 days", _week6Days.ToHumanReadableString());
			//Assert.AreEqual("In 2 weeks", _2Weeks.ToHumanReadableString());
		}

		[TestMethod]
		public void ToHumanReadableString_ShouldDisplayCorrectValueInFrench()
		{
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr");

			Assert.AreEqual("A l'instant", _justNow.ToHumanReadableString());
			Assert.AreEqual("Il y a 1 minute", _minuteAgo.ToHumanReadableString());
			Assert.AreEqual("Il y a 2 minutes", _2MinutesAgo.ToHumanReadableString());
			Assert.AreEqual("Il y a 59 minutes", _59MinutesAgo.ToHumanReadableString());
			Assert.AreEqual("Il y a 1 heure", _hourAgo.ToHumanReadableString());
			Assert.AreEqual("Il y a 1 heure", _hourAgo59.ToHumanReadableString());
			Assert.AreEqual("Il y a 2 heures", _twoHoursAgo.ToHumanReadableString());
			Assert.AreEqual("Il y a 2 heures", _twoHoursAgo59.ToHumanReadableString());
			Assert.AreEqual("Il y a 23 heures", _23HoursAgo.ToHumanReadableString());
			Assert.AreEqual("Hier", _yesterday.ToHumanReadableString());
			Assert.AreEqual("Hier", _yesterday23.ToHumanReadableString());
			Assert.AreEqual("Il y a 2 jours", _2DaysAgo.ToHumanReadableString());
			Assert.AreEqual("Il y a 2 jours", _2DaysAgo23.ToHumanReadableString());
			Assert.AreEqual("Il y a 6 jours", _6DaysAgo23.ToHumanReadableString());
			//Assert.AreEqual("Aujourd'hui", _today.ToHumanReadableString());
			//Assert.AreEqual("Demain", _tomorrow.ToHumanReadableString());
			//Assert.AreEqual("Dans 2 jours", _tomorrow.ToHumanReadableString());
			//Assert.AreEqual("Dans 6 jours", _tomorrow.ToHumanReadableString());
			//Assert.AreEqual("Dans 1 semaine", _week.ToHumanReadableString());
			//Assert.AreEqual("Dans 1 semaine et un jour", _week1Day.ToHumanReadableString());
			//Assert.AreEqual("Dans 1 semaine et 6 jours", _week6Days.ToHumanReadableString());
			//Assert.AreEqual("Dans 2 semaines", _2Weeks.ToHumanReadableString());
		}

	}
}
