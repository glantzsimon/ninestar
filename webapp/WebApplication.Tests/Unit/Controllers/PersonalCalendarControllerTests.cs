using K9.WebApplication.Controllers;
using K9.WebApplication.Services;
using Moq;
using System;
using System.Globalization;
using System.Text;
using System.Web.Mvc;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Controllers
{
    public class PersonalCalendarControllerTests
    {
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

    }
}
