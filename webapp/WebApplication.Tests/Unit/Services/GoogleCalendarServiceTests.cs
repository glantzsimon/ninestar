using K9.WebApplication.Models;
using K9.DataAccessLayer.Enums;
using K9.Base.DataAccessLayer.Enums;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.Controllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Services
{
    public class GoogleCalendarServiceTests
    {
        private static GoogleCalendarService CreateService()
        {
            return new GoogleCalendarService(new Mock<INineStarKiBasePackage>().Object,
                new Mock<INineStarKiService>().Object, new Mock<IUserService>().Object);
        }

        [Fact]
        public void SubscriptionTokenIsStablePrivateRevocableAndBoundToItsUser()
        {
            var userService = new Mock<IUserService>();
            var preferences = new Dictionary<int, string>();
            userService.Setup(e => e.Find(It.IsAny<int>()))
                .Returns((int id) => new K9.Base.DataAccessLayer.Models.User { Id = id });
            userService.Setup(e => e.GetUserPreference<string>(It.IsAny<int>(), It.IsAny<string>(), null))
                .Returns((int id, string key, string fallback) => preferences.TryGetValue(id, out var value) ? value : fallback);
            userService.Setup(e => e.UpdateUserPreference(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<object>()))
                .Callback((int id, string key, object value) => preferences[id] = (string)value);
            var service = new GoogleCalendarService(new Mock<INineStarKiBasePackage>().Object,
                new Mock<INineStarKiService>().Object, userService.Object);

            var token = service.GetOrCreateSubscriptionToken(42);
            Assert.Equal(token, service.GetOrCreateSubscriptionToken(42));
            Assert.Equal((int?)42, service.GetUserIdFromSubscriptionToken(token));
            Assert.NotEqual(token, service.GetOrCreateSubscriptionToken(43));
            var changed = (token[0] == 'A' ? "B" : "A") + token.Substring(1);
            Assert.Null(service.GetUserIdFromSubscriptionToken(changed));
            preferences[42] = preferences[43];
            Assert.Null(service.GetUserIdFromSubscriptionToken(token));
            preferences[42] = token;
            service.RevokeSubscription(42);
            Assert.Null(service.GetUserIdFromSubscriptionToken(token));
            Assert.NotEqual(token, service.GetOrCreateSubscriptionToken(42));
        }

        [Fact]
        public void SubscriptionRejectsMissingAndOversizedTokensBeforeReadingUsers()
        {
            var userService = new Mock<IUserService>(MockBehavior.Strict);
            var service = new GoogleCalendarService(new Mock<INineStarKiBasePackage>().Object,
                new Mock<INineStarKiService>().Object, userService.Object);
            Assert.Null(service.GetUserIdFromSubscriptionToken(null));
            Assert.Null(service.GetUserIdFromSubscriptionToken(" "));
            Assert.Null(service.GetUserIdFromSubscriptionToken(new string('A', 1025)));
        }

        [Fact]
        public void FeedRejectsInvalidTokensAndMembersWithoutPredictionAccess()
        {
            var calendar = new Mock<IGoogleCalendarService>();
            var users = new Mock<IUserService>();
            var memberships = new Mock<IMembershipService>();
            var controller = new PersonalCalendarController(calendar.Object, users.Object, memberships.Object);
            Assert.IsType<HttpNotFoundResult>(controller.Feed("invalid"));
            calendar.Setup(e => e.GetUserIdFromSubscriptionToken("valid")).Returns(42);
            Assert.IsType<HttpNotFoundResult>(controller.Feed("valid"));
            calendar.Verify(e => e.GenerateCalendar(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public void FeedUsesMemberLocalTodayAndARollingWindow()
        {
            var calendar = new Mock<IGoogleCalendarService>();
            var users = new Mock<IUserService>();
            var memberships = new Mock<IMembershipService>();
            calendar.Setup(e => e.GetUserIdFromSubscriptionToken("valid")).Returns(42);
            calendar.Setup(e => e.GetCalendarToday(42)).Returns(new DateTime(2026, 9, 30));
            users.Setup(e => e.UserIsAdmin(42)).Returns(true);
            users.Setup(e => e.GetUserPreference(42, "PersonalCalendarCulture", "en-GB")).Returns("en-GB");
            calendar.Setup(e => e.GenerateCalendar(42, new DateTime(2026, 8, 30), new DateTime(2027, 10, 1)))
                .Returns("BEGIN:VCALENDAR\r\nEND:VCALENDAR\r\n");
            var controller = new PersonalCalendarController(calendar.Object, users.Object, memberships.Object);
            var culture = CultureInfo.CurrentCulture;
            var uiCulture = CultureInfo.CurrentUICulture;
            var result = Assert.IsType<ContentResult>(controller.Feed("valid"));
            Assert.Equal("text/calendar", result.ContentType);
            Assert.Equal(Encoding.UTF8, result.ContentEncoding);
            Assert.Contains("BEGIN:VCALENDAR", result.Content);
            Assert.Equal(culture, CultureInfo.CurrentCulture);
            Assert.Equal(uiCulture, CultureInfo.CurrentUICulture);
            calendar.Verify(e => e.GenerateCalendar(42, new DateTime(2026, 8, 30), new DateTime(2027, 10, 1)), Times.Once);
        }

        [Fact]
        public void CalendarUsesAllDayDatesAndStablePersonalUids()
        {
            var service = CreateService();
            var entries = new List<CalendarEntry>
            {
                new CalendarEntry { Date = new DateTime(2028, 2, 29), Summary = "9Star · 1.9.3" },
                new CalendarEntry { Date = new DateTime(2028, 2, 28), Summary = "9Star · 1.9.2" }
            };
            var calendar = service.ConvertToICalendar(42, entries);
            Assert.Contains("DTSTART;VALUE=DATE:20280229\r\nDTEND;VALUE=DATE:20280301\r\n", calendar);
            Assert.Contains("UID:42-20280229@ninestarki.app\r\n", calendar);
            Assert.Contains("TRANSP:TRANSPARENT\r\n", calendar);
            Assert.True(calendar.IndexOf("20280228@", StringComparison.Ordinal) < calendar.IndexOf("20280229@", StringComparison.Ordinal));
            Assert.Contains("UID:42-20280229@ninestarki.app", service.ConvertToICalendar(42, entries));
            Assert.Contains("UID:43-20280229@ninestarki.app", service.ConvertToICalendar(43, entries));
            Assert.EndsWith("END:VCALENDAR\r\n", calendar);
        }

        [Fact]
        public void CalendarEscapesTextAndFoldsUtf8WithoutSplittingCharacters()
        {
            var text = "A\\B;C,D\r\n" + string.Concat(Enumerable.Repeat("水🔥é", 80));
            var calendar = CreateService().ConvertToICalendar(1, new List<CalendarEntry>
            {
                new CalendarEntry { Date = new DateTime(2026, 12, 31), Summary = text, Description = text }
            });
            foreach (var line in calendar.Split(new[] { "\r\n" }, StringSplitOptions.None))
            {
                Assert.InRange(Encoding.UTF8.GetByteCount(line), 0, 75);
                // Strict encoding rejects a surrogate split across physical lines.
                new UTF8Encoding(false, true).GetBytes(line);
            }
            var unfolded = calendar.Replace("\r\n ", string.Empty);
            var escaped = "A\\\\B\\;C\\,D\\n" + string.Concat(Enumerable.Repeat("水🔥é", 80));
            Assert.Contains("SUMMARY:" + escaped + "\r\n", unfolded);
            Assert.Contains("DESCRIPTION:" + escaped + "\r\n", unfolded);
            Assert.DoesNotContain("\n", calendar.Replace("\r\n", string.Empty));
        }

        [Fact]
        public void CalendarRejectsDuplicateDatesAndInvalidInputs()
        {
            var service = CreateService();
            Assert.Throws<ArgumentNullException>(() => service.ConvertToICalendar(1, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => service.ConvertToICalendar(0, new List<CalendarEntry>()));
            Assert.Throws<ArgumentException>(() => service.ConvertToICalendar(1, new List<CalendarEntry>
            {
                new CalendarEntry { Date = new DateTime(2026, 1, 1) },
                new CalendarEntry { Date = new DateTime(2026, 1, 1, 12, 0, 0) }
            }));
            Assert.Throws<ArgumentException>(() => service.ConvertToICalendar(1, new List<CalendarEntry>
            {
                new CalendarEntry { Date = DateTime.MaxValue }
            }));
            Assert.Throws<ArgumentException>(() => service.GetCalendarEntries(1, new DateTime(2026, 2, 1), new DateTime(2026, 1, 1)));
        }

        [Fact]
        public void SelectedInstantRetainsCalendarDateAcrossTimezoneOffsets()
        {
            var date = new DateTime(2026, 3, 29, 0, 0, 0, DateTimeKind.Unspecified);
            foreach (var timeZoneId in new[] { "Europe/London", "America/New_York", "Pacific/Auckland" })
            {
                var instant = DateTimeHelper.ConvertToUT(date, timeZoneId);
                Assert.Equal(DateTimeKind.Utc, instant.Kind);
                Assert.Equal(date, DateTimeHelper.ConvertToLocaleDateTime(instant, timeZoneId));
            }
        }

        [Fact]
        public void ExplicitCalculatorTypeIsUsedBeforeCalculatingPersonalHouses()
        {
            var person = new PersonModel { DateOfBirth = new DateTime(1980, 1, 1), Gender = EGender.Male };
            var model = new NineStarKiModel(person, 5, 5, 3, 1, 1, 5, 5,
                5, 5, 1, 5, 5, new (int DailyKi, int? InvertedDailyKi)[] { (3, null), (9, null) }, 5,
                selectedDate: new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                calculationMethod: ECalculationMethod.Traditional, userTimeZoneId: "Europe/London",
                calculatorType: ECalculatorType.Advanced);
            Assert.Equal(ECalculatorType.Advanced, model.CalculatorType);
            Assert.Equal(ECalculationMethod.Traditional, model.CalculationMethod);
            Assert.Equal(7, model.PersonalHousesOccupiedEnergies.Year.EnergyNumber);
            Assert.Equal(3, model.PersonalHousesOccupiedEnergies.Month.EnergyNumber);
            Assert.Equal(5, model.PersonalHousesOccupiedEnergies.Day.EnergyNumber);
            Assert.Equal(8, model.PersonalHousesOccupiedEnergies.Day2.EnergyNumber);
        }

        [Fact]
        public void All729CombinedDescriptionsArePackaged()
        {
            for (var year = 1; year <= 9; year++)
                for (var month = 1; month <= 9; month++)
                    for (var day = 1; day <= 9; day++)
                    {
                        var name = string.Format(CultureInfo.InvariantCulture,
                            "_{0}_{1}_{2}", year, month, day);
                        var html = K9.Globalisation.Dictionary.ResourceManager.GetString(name, K9.Globalisation.Dictionary.Culture);
                        Assert.False(string.IsNullOrWhiteSpace(html));
                        Assert.Contains("<p>", html);
                    }
        }
    }
}
