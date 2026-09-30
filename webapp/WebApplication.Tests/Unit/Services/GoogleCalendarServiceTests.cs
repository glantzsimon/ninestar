using K9.WebApplication.Models;
using K9.DataAccessLayer.Enums;
using K9.Base.DataAccessLayer.Enums;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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
            var assembly = typeof(K9.Globalisation.Dictionary).Assembly;
            for (var year = 1; year <= 9; year++)
                for (var month = 1; month <= 9; month++)
                    for (var day = 1; day <= 9; day++)
                    {
                        var name = string.Format(CultureInfo.InvariantCulture,
                            "K9.Globalisation.Predictions.Combined.{0}-{1}-{2}.htm", year, month, day);
                        using (var stream = assembly.GetManifestResourceStream(name))
                            Assert.NotNull(stream);
                    }
        }
    }
}
