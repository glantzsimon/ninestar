using K9.WebApplication.Models;
using K9.WebApplication.ViewModels;
using K9.DataAccessLayer.Enums;
using K9.Base.DataAccessLayer.Enums;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.DataAccessLayer.Models;
using K9.WebApplication.Constants;
using K9.WebApplication.Enums;
using K9.WebApplication.Helpers;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Services
{
    public class GoogleCalendarServiceTests
    {
        private static GoogleCalendarService CreateService()
        {
            return new GoogleCalendarService(new Mock<INineStarKiBasePackage>().Object,
                new Mock<INineStarKiService>().Object, new Mock<IUserService>().Object, new Mock<IAstrologyService>().Object);
        }

        [Fact]
        public void CancelledMonthDoesNotReadUsersOrCalculateProfiles()
        {
            var users = new Mock<IUserService>(MockBehavior.Strict);
            var profiles = new Mock<INineStarKiService>(MockBehavior.Strict);
            var service = new GoogleCalendarService(new Mock<INineStarKiBasePackage>().Object,
                profiles.Object, users.Object, new Mock<IAstrologyService>().Object);
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
                new Mock<INineStarKiService>().Object, userService.Object, new Mock<IAstrologyService>().Object);

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
                new Mock<INineStarKiService>().Object, userService.Object, new Mock<IAstrologyService>().Object);
            Assert.Null(service.GetUserIdFromSubscriptionToken(null));
            Assert.Null(service.GetUserIdFromSubscriptionToken(" "));
            Assert.Null(service.GetUserIdFromSubscriptionToken(new string('A', 1025)));
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
        [InlineData("Europe/London", 10, 1, 1, false)]
        [InlineData("America/New_York", 10, 1, 1, false)]
        [InlineData("Pacific/Auckland", 10, 1, 1, false)]
        [InlineData("Europe/London", 3, 29, 1, false)]
        [InlineData("Europe/London", 10, 25, 1, false)]
        [InlineData("Europe/London", 2, 1, 60, false)]
        [InlineData("Europe/London", 2, 4, 1, true)]
        [InlineData("Pacific/Auckland", 2, 4, 60, true)]
        [InlineData("Europe/London", 6, 10, 1, true)]
        public void CalendarReusesPlannerBatchesWithLocalDatesBirthTimeAndSavedOptions(string timeZoneId, int month, int day, int numberOfDays, bool startsBeforeSolarTransition)
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
            // Synthetic years contain two months, keeping cross-year batching small.
            for (var yearOffset = 0; yearOffset < numberOfDays; yearOffset += 40)
            {
                var yearStart = date.AddDays(yearOffset);
                var model = new NineStarKiModel(new PersonModel { DateOfBirth = birthDate, Gender = EGender.Male },
                    5, 5, 3, 1, 1, 5, 5, 5, 5, 1 + yearOffset / 40, 1, 1,
                    new (int DailyKi, int? InvertedDailyKi)[] { (3, null), (9, null) }, 5,
                    selectedDate: yearStart, userTimeZoneId: timeZoneId, calculatorType: ECalculatorType.Advanced);
                var houses = model.PersonalHousesOccupiedEnergies;
                var months = new List<PlannerViewModelItem>();
                for (var offset = yearOffset; offset < yearOffset + 40; offset += 20)
                {
                    var batchStart = date.AddDays(offset);
                    var batchEnd = batchStart.AddDays(20);
                    var monthEnergy = model.GetPersonalCycleEnergy(1 + offset / 20, ENineStarKiEnergyCycleType.MonthlyCycleEnergy);
                    months.Add(new PlannerViewModelItem
                    {
                        EnergyStartsOn = batchStart.AddDays(-3), EnergyEndsOn = batchEnd.AddDays(-1),
                        Energy = monthEnergy
                    });
                    if (offset >= numberOfDays)
                        continue;
                    // Include out-of-range and unsorted rows to verify trimming.
                    var rows = Enumerable.Range(-1, 21).Select(index => new PlannerViewModelItem
                    {
                        EnergyStartsOn = batchStart.AddDays(index),
                        Energy = houses.Day, SecondEnergy = houses.Day2
                    }).Reverse().ToList();
                    profiles.Setup(e => e.GetPlannerData(
                            It.Is<DateTime>(d => d == birthDate && d.Kind == DateTimeKind.Unspecified),
                            info.BirthTimeZoneId, birthTime, EGender.Male,
                            It.Is<DateTime>(d => d == batchStart && d.Kind == DateTimeKind.Unspecified),
                            timeZoneId, ECalculationMethod.Traditional, ECalculatorType.Advanced,
                            EDisplayDataForPeriod.SelectedDate, EHousesDisplay.SolarHouse, true, true,
                            EPlannerView.Month, EScopeDisplay.PersonalKi, EPlannerNavigationDirection.None, model, false))
                        .Returns(new PlannerViewModel
                        {
                            NineStarKiModel = model, Energy = houses.Month, Energies = rows
                        });
                    for (var expectedDate = batchStart; expectedDate < batchEnd && expectedDate < date.AddDays(numberOfDays); expectedDate = expectedDate.AddDays(1))
                        expected.Add(new CalendarEntry
                        {
                            Date = expectedDate, YearHouse = houses.Year.EnergyNumber,
                            MonthHouse = monthEnergy.EnergyNumber, DayHouse = houses.Day.EnergyNumber,
                            AfternoonDayHouse = houses.Day2.EnergyNumber == houses.Day.EnergyNumber ? (int?)null : houses.Day2.EnergyNumber
                        });
                }
                var yearSelection = yearStart;
                if (startsBeforeSolarTransition && yearOffset == 0)
                {
                    // Midnight is still before the exact solar transition. Return
                    // the expired year's rows, as the real planner does.
                    profiles.Setup(e => e.GetPlannerData(
                            It.Is<DateTime>(d => d == birthDate && d.Kind == DateTimeKind.Unspecified),
                            info.BirthTimeZoneId, birthTime, EGender.Male,
                            It.Is<DateTime>(d => d == yearStart && d.Kind == DateTimeKind.Unspecified),
                            timeZoneId, ECalculationMethod.Traditional, ECalculatorType.Advanced,
                            EDisplayDataForPeriod.SelectedDate, EHousesDisplay.SolarHouse, true, true,
                            EPlannerView.Year, EScopeDisplay.PersonalKi, EPlannerNavigationDirection.None, null, false))
                        .Returns(new PlannerViewModel
                        {
                            PeriodStartsOn = yearStart.AddYears(-1),
                            PeriodEndsOn = yearStart.AddDays(-1),
                            Energies = new List<PlannerViewModelItem>
                            {
                                new PlannerViewModelItem
                                {
                                    EnergyStartsOn = yearStart.AddDays(-30),
                                    EnergyEndsOn = yearStart.AddDays(-1)
                                }
                            }
                        });
                    yearSelection = yearStart.AddDays(2);
                }
                profiles.Setup(e => e.GetPlannerData(
                        It.Is<DateTime>(d => d == birthDate && d.Kind == DateTimeKind.Unspecified),
                        info.BirthTimeZoneId, birthTime, EGender.Male,
                        It.Is<DateTime>(d => d == yearSelection && d.Kind == DateTimeKind.Unspecified),
                        timeZoneId, ECalculationMethod.Traditional, ECalculatorType.Advanced,
                        EDisplayDataForPeriod.SelectedDate, EHousesDisplay.SolarHouse, true, true,
                        EPlannerView.Year, EScopeDisplay.PersonalKi, EPlannerNavigationDirection.None, null, false))
                    .Returns(new PlannerViewModel
                    {
                        NineStarKiModel = model, Energy = houses.Year, Energies = months,
                        PeriodStartsOn = yearStart, PeriodEndsOn = yearStart.AddDays(39)
                    });
            }
            var astrology = new Mock<IAstrologyService>(MockBehavior.Strict);
            var calendar = new GoogleCalendarService(package.Object, profiles.Object, users.Object, astrology.Object);

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
                EPlannerView.Month, EScopeDisplay.PersonalKi, EPlannerNavigationDirection.None, It.IsAny<NineStarKiModel>(), false),
                Times.Exactly((numberOfDays + 19) / 20));
            profiles.Verify(e => e.GetPlannerData(
                It.IsAny<DateTime>(), info.BirthTimeZoneId, birthTime, EGender.Male,
                It.IsAny<DateTime>(), timeZoneId, ECalculationMethod.Traditional, ECalculatorType.Advanced,
                EDisplayDataForPeriod.SelectedDate, EHousesDisplay.SolarHouse, true, true,
                EPlannerView.Year, EScopeDisplay.PersonalKi, EPlannerNavigationDirection.None, null, false),
                Times.Exactly((numberOfDays + 39) / 40 + (startsBeforeSolarTransition ? 1 : 0)));
            profiles.VerifyAll();

            Assert.All(entries, e => Assert.Null(e.MoonPhase));
            astrology.Verify(e => e.GetMoonPhase(It.IsAny<DateTime>(), It.IsAny<string>(),
                It.IsAny<bool>(), It.IsAny<NineStarKiEnergy>()), Times.Never);
            var phase = new MoonPhase(88.6, false) { LunarDay = 19, LunarDayDescription = "<p>Lunar reading</p>" };
            astrology.Setup(e => e.GetMoonPhase(
                It.Is<DateTime>(d => d.Kind == DateTimeKind.Unspecified && d >= date && d < date.AddDays(numberOfDays)),
                timeZoneId, false, It.IsAny<NineStarKiEnergy>())).Returns(phase);
            var expanded = calendar.GetCalendarEntries(42, date, date.AddDays(numberOfDays), CancellationToken.None, true);
            Assert.Equal(entries.Select(e => e.Date), expanded.Select(e => e.Date));
            Assert.All(expanded, e => Assert.Same(phase, e.MoonPhase));
            astrology.Setup(e => e.GetMoonPhase(
                It.Is<DateTime>(d => d == date && d.Kind == DateTimeKind.Unspecified),
                timeZoneId, true, It.IsAny<NineStarKiEnergy>())).Returns(phase);
            var selectedLunar = calendar.GetLunarCalendarEntry(42, date, CancellationToken.None);
            Assert.Equal(date, selectedLunar.Date);
            Assert.Same(phase, selectedLunar.MoonPhase);

            // The same planner batches supply the website and the subscription;
            // only the description resource family changes for the feed.
            var rawFeed = calendar.GenerateCalendar(42, date, date.AddDays(numberOfDays));
            var feed = rawFeed.Replace("\r\n ", string.Empty);
            var first = entries[0];
            var concise = CombinedPredictionHelper.GetCalendarText(first.YearHouse, first.MonthHouse, first.DayHouse);
            Assert.DoesNotContain(K9.Globalisation.Dictionary.CalendarIdealForHeading, first.Description);
            Assert.Contains("\n\n", first.Description);
            Assert.True(concise.Length < first.Description.Length);
            if (first.AfternoonDayHouse.HasValue)
                concise += $"\n\n{K9.Globalisation.Dictionary.AfternoonEnergyLabel} ({first.YearHouse}.{first.MonthHouse}.{first.AfternoonDayHouse.Value}):\n\n" +
                    CombinedPredictionHelper.GetCalendarText(first.YearHouse, first.MonthHouse, first.AfternoonDayHouse.Value);
            var expectedFeed = calendar.ConvertToICalendar(42, new List<CalendarEntry>
            {
                new CalendarEntry { Date = first.Date, Description = concise }
            }).Replace("\r\n ", string.Empty);
            var expectedDescription = expectedFeed.Split(new[] { "\r\n" }, StringSplitOptions.None)
                .Single(e => e.StartsWith("DESCRIPTION:", StringComparison.Ordinal));
            Assert.Contains(expectedDescription + "\r\n", feed);
            astrology.Verify(e => e.GetMoonPhase(It.IsAny<DateTime>(), timeZoneId, false,
                It.IsAny<NineStarKiEnergy>()), Times.Exactly(numberOfDays));
            astrology.Verify(e => e.GetMoonPhase(It.IsAny<DateTime>(), timeZoneId, true,
                It.IsAny<NineStarKiEnergy>()), Times.Once);
            foreach (var line in rawFeed.Split(new[] { "\r\n" }, StringSplitOptions.None))
                Assert.InRange(Encoding.UTF8.GetByteCount(line), 0, 75);
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
                        var firstParagraph = html.Substring(html.IndexOf("<p>", StringComparison.Ordinal) + 3);
                        Assert.StartsWith("Today’s ", firstParagraph);
                    }
        }

        [Fact]
        public void All729CalendarReadingsHaveTwoListsAndOneParagraph()
        {
            for (var year = 1; year <= 9; year++)
                for (var month = 1; month <= 9; month++)
                    for (var day = 1; day <= 9; day++)
                    {
                        var text = CombinedPredictionHelper.GetCalendarText(year, month, day).Replace("\r\n", "\n");
                        var blocks = text.Split(new[] { "\n\n" }, StringSplitOptions.None);
                        Assert.Equal(3, blocks.Length);
                        var ideal = blocks[0].Split('\n');
                        var mindful = blocks[1].Split('\n');
                        Assert.Equal(4, ideal.Length);
                        Assert.Equal(4, mindful.Length);
                        Assert.Equal(K9.Globalisation.Dictionary.CalendarIdealForHeading, ideal[0]);
                        Assert.Equal(K9.Globalisation.Dictionary.CalendarBeMindfulOfHeading, mindful[0]);
                        Assert.All(ideal.Skip(1).Concat(mindful.Skip(1)), item => Assert.StartsWith("- ", item));
                        Assert.DoesNotContain("\n", blocks[2]);
                        Assert.DoesNotContain("<p>", text);
                        Assert.DoesNotContain("{0}", text);
                        Assert.DoesNotContain("{1}", text);
                        Assert.False(string.IsNullOrWhiteSpace(blocks[2]));
                        Assert.StartsWith("Today’s ", blocks[2]);
                    }
            Assert.Throws<ArgumentOutOfRangeException>(() => CombinedPredictionHelper.GetCalendarText(0, 1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => CombinedPredictionHelper.GetCalendarText(1, 1, 10));
        }
    }
}
