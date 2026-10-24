using K9.WebApplication.Attributes;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Helpers
{
    public class CalculatorTrialTests
    {
        [Fact]
        public void FirstAnonymousSubmissionRunsAndOnlySuccessfulResultConsumesTrial()
        {
            var f = new Fixture();
            var executing = f.Executing();
            f.Filter.OnActionExecuting(executing);
            Assert.Null(executing.Result);
            f.Filter.OnActionExecuted(f.Executed(new NineStarKiModel { IsProcessed = false }));
            Assert.False(CalculatorTrialHelper.HasUsedTrial(f.Http.Object));
            f.Filter.OnActionExecuted(f.Executed(new NineStarKiModel { IsProcessed = true }));
            Assert.True(CalculatorTrialHelper.HasUsedTrial(f.Http.Object));
            Assert.True(f.ResponseCookies[CalculatorTrialHelper.CookieName].HttpOnly);
            Assert.True(f.ResponseCookies[CalculatorTrialHelper.CookieName].Secure);
            Assert.Equal("/ninestar/", f.ResponseCookies[CalculatorTrialHelper.CookieName].Path);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("0")]
        [InlineData("")]
        public void ExistingCookieRequiresLoginRegardlessOfItsValue(string value)
        {
            var f = new Fixture();
            f.RequestCookies.Add(new HttpCookie(CalculatorTrialHelper.CookieName, value));
            var action = f.Executing();
            f.Filter.OnActionExecuting(action);
            var redirect = Assert.IsType<RedirectToRouteResult>(action.Result);
            Assert.Equal("Account", redirect.RouteValues["controller"]);
            Assert.Equal("Login", redirect.RouteValues["action"]);
            Assert.True((bool)f.Controller.TempData["CalculatorSignInRequired"]);
        }

        [Fact]
        public void SessionMarkerBlocksASecondCalculatorEvenBeforeCookieRoundTrip()
        {
            var f = new Fixture();
            CalculatorTrialHelper.RecordResult(f.Http.Object);
            Assert.Null(f.RequestCookies[CalculatorTrialHelper.CookieName]);
            var action = f.Executing();
            f.Filter.OnActionExecuting(action);
            Assert.IsType<RedirectToRouteResult>(action.Result);
        }

        [Fact]
        public void SignedInUsersAreNotLimitedOrGivenTrialCookies()
        {
            var f = new Fixture(authenticated: true);
            f.RequestCookies.Add(new HttpCookie(CalculatorTrialHelper.CookieName, "1"));
            var action = f.Executing();
            f.Filter.OnActionExecuting(action);
            Assert.Null(action.Result);
            f.Filter.OnActionExecuted(f.Executed(new NineStarKiModel { IsProcessed = true }));
            Assert.Null(f.ResponseCookies[CalculatorTrialHelper.CookieName]);
        }

        [Fact]
        public void InvalidSubmissionOrExceptionDoesNotConsumeTrial()
        {
            var f = new Fixture();
            f.Controller.ModelState.AddModelError("DateOfBirth", "invalid");
            f.Filter.OnActionExecuted(f.Executed(new NineStarKiModel { IsProcessed = true }));
            Assert.False(CalculatorTrialHelper.HasUsedTrial(f.Http.Object));
            f.Controller.ModelState.Clear();
            var failed = f.Executed(new NineStarKiModel { IsProcessed = true });
            failed.Exception = new InvalidOperationException();
            f.Filter.OnActionExecuted(failed);
            Assert.False(CalculatorTrialHelper.HasUsedTrial(f.Http.Object));
        }

        private sealed class TestController : Controller
        {
            public ActionResult Index() => new EmptyResult();
        }

        private sealed class Fixture
        {
            public readonly Mock<HttpContextBase> Http = new Mock<HttpContextBase>();
            public readonly HttpCookieCollection RequestCookies = new HttpCookieCollection();
            public readonly HttpCookieCollection ResponseCookies = new HttpCookieCollection();
            public readonly TestController Controller = new TestController();
            public readonly CalculatorTrialAttribute Filter = new CalculatorTrialAttribute();
            private readonly ActionDescriptor _action;

            public Fixture(bool authenticated = false)
            {
                var request = new Mock<HttpRequestBase>();
                request.SetupGet(e => e.Cookies).Returns(RequestCookies);
                request.SetupGet(e => e.ApplicationPath).Returns("/ninestar");
                request.SetupGet(e => e.IsSecureConnection).Returns(true);
                request.SetupGet(e => e.AppRelativeCurrentExecutionFilePath).Returns("~/calculator");
                var response = new Mock<HttpResponseBase>();
                response.SetupGet(e => e.Cookies).Returns(ResponseCookies);
                response.SetupGet(e => e.Cache).Returns(new Mock<HttpCachePolicyBase>().Object);
                var values = new Dictionary<string, object>();
                var session = new Mock<HttpSessionStateBase>();
                session.Setup(e => e[It.IsAny<string>()]).Returns((string key) => values.ContainsKey(key) ? values[key] : null);
                session.SetupSet(e => e[It.IsAny<string>()] = It.IsAny<object>())
                    .Callback<string, object>((key, value) => values[key] = value);
                Http.SetupGet(e => e.Request).Returns(request.Object);
                Http.SetupGet(e => e.Response).Returns(response.Object);
                Http.SetupGet(e => e.Session).Returns(session.Object);
                Http.SetupGet(e => e.User).Returns(new GenericPrincipal(new GenericIdentity(authenticated ? "member" : ""), new string[0]));
                Controller.ControllerContext = new ControllerContext(Http.Object, new RouteData(), Controller);
                _action = new ReflectedActionDescriptor(typeof(TestController).GetMethod("Index"), "Index",
                    new ReflectedControllerDescriptor(typeof(TestController)));
            }

            public ActionExecutingContext Executing() =>
                new ActionExecutingContext(Controller.ControllerContext, _action, new Dictionary<string, object>());

            public ActionExecutedContext Executed(object model) => new ActionExecutedContext(Controller.ControllerContext, _action, false, null)
            {
                Result = new ViewResult { ViewData = new ViewDataDictionary(model) }
            };
        }
    }
}
