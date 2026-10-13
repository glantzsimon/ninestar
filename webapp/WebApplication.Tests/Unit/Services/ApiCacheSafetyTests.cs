using K9.Base.WebApplication.Config;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Controllers;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using Moq;
using NLog;
using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Services
{
    public class ApiCacheSafetyTests
    {
        [Theory]
        [InlineData("ApiKey wrong", false, false, 401)]
        [InlineData("ApiKey test-key", false, false, 404)]
        [InlineData("ApiKey test-key", true, false, 422)]
        [InlineData("ApiKey test-key", true, true, 0)]
        public void ValidationPreservesAccessResponsesAndReusesMembership(
            string header, bool found, bool paid, int status)
        {
            var previous = HttpContext.Current;
            HttpContext.Current = Helpers.Helpers.CreateHttpContextWithSession();
            HttpContext.Current.Request.RequestContext = new RequestContext(
                new HttpContextWrapper(HttpContext.Current), new RouteData());
            try
            {
                var membership = found ? new UserMembership
                {
                    UserId = 8, StartsOn = DateTime.Today.AddDays(-1), EndsOn = DateTime.Today.AddDays(1),
                    MembershipOption = new MembershipOption
                    {
                        SubscriptionType = paid ? MembershipOption.ESubscriptionType.WeeklyPlatinum
                            : MembershipOption.ESubscriptionType.Free
                    }
                } : null;
                var memberships = new Mock<IMembershipService>(MockBehavior.Strict);
                memberships.Setup(e => e.GetActiveUserMembership("123456")).Returns(membership);
                var users = new Mock<IUserService>(MockBehavior.Strict);
                users.Setup(e => e.GetOrCreateUserInfo(8)).Returns(new UserInfo { UserId = 8 });
                var package = new Mock<INineStarKiPackage>();
                package.SetupGet(e => e.Logger).Returns(new Mock<ILogger>().Object);
                package.SetupGet(e => e.DataSetsHelper).Returns(new Mock<IDataSetsHelper>().Object);
                package.SetupGet(e => e.Roles).Returns(new Mock<IRoles>().Object);
                package.SetupGet(e => e.Authentication).Returns(new Mock<IAuthentication>().Object);
                package.SetupGet(e => e.FileSourceHelper).Returns(new Mock<IFileSourceHelper>().Object);
                package.SetupGet(e => e.ApiConfiguration).Returns(new ApiConfiguration { ApiKey = "test-key" });
                package.SetupGet(e => e.MembershipService).Returns(memberships.Object);
                package.SetupGet(e => e.UserService).Returns(users.Object);
                var controller = new ApiController(package.Object, new Mock<INineStarKiService>().Object,
                    new Mock<IIChingService>().Object);
                var request = new Mock<HttpRequestBase>();
                request.SetupGet(e => e.Headers).Returns(new NameValueCollection { { "Authorization", header } });
                var context = new Mock<HttpContextBase>();
                context.SetupGet(e => e.Request).Returns(request.Object);
                controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
                var invoked = false;
                Func<UserMembership, JsonResult> callback = validated =>
                {
                    invoked = true;
                    Assert.Same(membership, validated);
                    var getInfo = typeof(ApiController).GetMethod("GetUserInfo", BindingFlags.Instance | BindingFlags.NonPublic);
                    var info = (UserInfo)getInfo.Invoke(controller, new object[] { validated });
                    Assert.Equal(8, info.UserId);
                    return new JsonResult { Data = "ok" };
                };
                var validate = typeof(ApiController).GetMethod("Validate", BindingFlags.Instance | BindingFlags.NonPublic,
                    null, new[] { typeof(string), typeof(Func<UserMembership, JsonResult>) }, null);
                var result = (JsonResult)validate.Invoke(controller, new object[] { "123456", callback });

                Assert.Equal(status == 0, invoked);
                if (status != 0)
                    Assert.Equal(status, (int)result.Data.GetType().GetProperty("statusCode").GetValue(result.Data));
                memberships.Verify(e => e.GetActiveUserMembership("123456"),
                    status == 401 ? Times.Never() : Times.Once());
                users.Verify(e => e.GetOrCreateUserInfo(8), status == 0 ? Times.Once() : Times.Never());
            }
            finally
            {
                HttpContext.Current = previous;
            }
        }

        [Fact]
        public void SensitivePagesCannotServeStoredOutputWithoutRunningTheirActions()
        {
            foreach (var method in new[]
            {
                typeof(ApiController).GetMethod("GetKnowledgeBase"),
                typeof(ReportsController).GetMethod("YearlyReport")
            })
            {
                var attribute = method.GetCustomAttribute<OutputCacheAttribute>();
                Assert.True(attribute.NoStore);
                Assert.Equal(0, attribute.Duration);
                Assert.Equal(System.Web.UI.OutputCacheLocation.None, attribute.Location);
            }
        }
    }
}
