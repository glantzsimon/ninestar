using K9.WebApplication.Models;
using K9.WebApplication.ViewModels;
using K9.DataAccessLayer.Enums;
using K9.Base.DataAccessLayer.Enums;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.DataAccessLayer.Models;
using K9.WebApplication.Constants;
using K9.WebApplication.Enums;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.Controllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
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
        public void CancelledMonthDoesNotReadUsersOrCalculateProfiles()
        {
            var users = new Mock<IUserService>(MockBehavior.Strict);
            var profiles = new Mock<INineStarKiService>(MockBehavior.Strict);
            var service = new GoogleCalendarService(new Mock<INineStarKiBasePackage>().Object,
                profiles.Object, users.Object);
            using (var cancellation = new CancellationTokenSource())
            {
                cancellation.Cancel();
                Assert.Throws<OperationCanceledException>(() => service.GetCalendarEntries(42,
                    new DateTime(2027, 3, 1), new DateTime(2027, 4, 1), cancellation.Token));
            }
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

        [Theory]
        [InlineData("Europe/London", 10, 1, 1)]
        [InlineData("America/New_York", 10, 1, 1)]
        [InlineData("Pacific/Auckland", 10, 1, 1)]
        [InlineData("Europe/London", 3, 29, 1)]
        [InlineData("Europe/London", 10, 25, 1)]
        [InlineData("Europe/London", 2, 1, 60)]
        public void CalendarReusesPlannerBatchesWithLocalDatesBirthTimeAndSavedOptions(string timeZoneId, int month, int day, int numberOfDays)
        {
            var date = new DateTime(2026, month, day);
            var birthDate = new DateTime(1979, 6, 16);
            var birthTime = new TimeSpan(8, 15, 0);
            var info = new UserInfo
            {
                UserId = 42, TimeOfBirth = birthTime, BirthTimeZoneId = "Europe/London",
                CalculationMethod = ECalculationMethod.Traditional,
                CalculatorType = ECalculatorType.Advanced, HousesDisplay = EHousesDisplay.SolarHouse
            };
            var repository = new Mock<IRepository<UserInfo>>();
            repository.Setup(e => e.Find(It.IsAny<Expression<Func<UserInfo, bool>>>()))
                .Returns(new List<UserInfo> { info });
            var package = new Mock<INineStarKiBasePackage>();
            package.SetupGet(e => e.UserInfosRepository).Returns(repository.Object);
            var users = new Mock<IUserService>();
            users.Setup(e => e.Find(42)).Returns(new K9.Base.DataAccessLayer.Models.User
            {
                Id = 42, BirthDate = birthDate, Gender = EGender.Male
            });
            users.Setup(e => e.GetUserPreference(42, SessionConstants.UserTimeZone, info.BirthTimeZoneId)).Returns(timeZoneId);
            users.Setup(e => e.GetUserPreference(42, SessionConstants.UserCalculationMethod, info.CalculationMethod)).Returns(info.CalculationMethod);
            users.Setup(e => e.GetUserPreference(42, SessionConstants.DefaultCalculatorType, info.CalculatorType)).Returns(info.CalculatorType);
            users.Setup(e => e.GetUserPreference(42, SessionConstants.UserHousesDisplay, info.HousesDisplay)).Returns(info.HousesDisplay);
            users.Setup(e => e.GetUserPreference(42, SessionConstants.InvertDailyAndHourlyKiForSouthernHemisphere, false)).Returns(true);
            users.Setup(e => e.GetUserPreference(42, SessionConstants.InvertDailyAndHourlyCycleKiForSouthernHemisphere, false)).Returns(true);
            var profiles = new Mock<INineStarKiService>(MockBehavior.Strict);
            var expected = new List<CalendarEntry>();
            // Deliberately return rows outside the requested range and out of order.
            // Each batch has its own year/month houses and a split daily energy.
            for (var offset = 0; offset < numberOfDays; offset += 20)
            {
                var batchStart = date.AddDays(offset);
                var batchEnd = batchStart.AddDays(20);
                var model = new NineStarKiModel(new PersonModel { DateOfBirth = birthDate, Gender = EGender.Male },
                    5, 5, 3, 1, 1, 5, 5, 5, 5, 1 + offset / 20, 1, 1,
                    new (int DailyKi, int? InvertedDailyKi)[] { (3, null), (9, null) }, 5,
                    selectedDate: batchStart, userTimeZoneId: timeZoneId, calculatorType: ECalculatorType.Advanced);
                var houses = model.PersonalHousesOccupiedEnergies;
                var rows = Enumerable.Range(-1, 21).Select(index => new PlannerViewModelItem
                {
                    EnergyStartsOn = batchStart.AddDays(index),
                    Energy = houses.Day,
                    SecondEnergy = houses.Day2
                }).Reverse().ToList();
                profiles.Setup(e => e.GetPlannerData(
                        It.Is<DateTime>(d => d == birthDate && d.Kind == DateTimeKind.Unspecified),
                        info.BirthTimeZoneId, birthTime, EGender.Male,
                        It.Is<DateTime>(d => d == batchStart && d.Kind == DateTimeKind.Unspecified),
                        timeZoneId, ECalculationMethod.Traditional, ECalculatorType.Advanced,
                        EDisplayDataForPeriod.SelectedDate, EHousesDisplay.SolarHouse, true, true,
                        EPlannerView.Month, EScopeDisplay.PersonalKi, EPlannerNavigationDirection.None, null))
                    .Returns(new PlannerViewModel
                    {
                        NineStarKiModel = model, Energy = houses.Month, Energies = rows
                    });
                for (var expectedDate = batchStart; expectedDate < batchEnd && expectedDate < date.AddDays(numberOfDays); expectedDate = expectedDate.AddDays(1))
                    expected.Add(new CalendarEntry
                    {
                        Date = expectedDate, YearHouse = houses.Year.EnergyNumber,
                        MonthHouse = houses.Month.EnergyNumber, DayHouse = houses.Day.EnergyNumber,
                        AfternoonDayHouse = houses.Day2.EnergyNumber == houses.Day.EnergyNumber ? (int?)null : houses.Day2.EnergyNumber
                    });
            }
            var calendar = new GoogleCalendarService(package.Object, profiles.Object, users.Object);

            var entries = calendar.GetCalendarEntries(42, date, date.AddDays(numberOfDays));

            Assert.Equal(expected.Count, entries.Count);
            for (var index = 0; index < entries.Count; index++)
            {
                Assert.Equal(expected[index].Date, entries[index].Date);
                Assert.Equal(expected[index].YearHouse, entries[index].YearHouse);
                Assert.Equal(expected[index].MonthHouse, entries[index].MonthHouse);
                Assert.Equal(expected[index].DayHouse, entries[index].DayHouse);
                Assert.Equal(expected[index].AfternoonDayHouse, entries[index].AfternoonDayHouse);
            }
            // Strict mocks reject any per-date CalculateNineStarKiProfile calls.
            profiles.Verify(e => e.GetPlannerData(
                It.IsAny<DateTime>(), info.BirthTimeZoneId, birthTime, EGender.Male,
                It.IsAny<DateTime>(), timeZoneId, ECalculationMethod.Traditional, ECalculatorType.Advanced,
                EDisplayDataForPeriod.SelectedDate, EHousesDisplay.SolarHouse, true, true,
                EPlannerView.Month, EScopeDisplay.PersonalKi, EPlannerNavigationDirection.None, null),
                Times.Exactly((numberOfDays + 19) / 20));
            profiles.VerifyAll();
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
