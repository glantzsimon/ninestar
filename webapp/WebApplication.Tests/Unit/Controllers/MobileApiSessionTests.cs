using K9.DataAccessLayer.Models;
using K9.WebApplication.Controllers;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using Moq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Controllers
{
    public class MobileApiSessionTests
    {
        [Fact]
        public void LastInputsAreIsolatedByEffectiveUserAndCannotBeReadAnonymously()
        {
            var fixture = new Fixture();
            fixture.Controller.Remember("private name");
            Assert.Equal("private name", fixture.Controller.Recall());
            fixture.Controller.UserId = 43;
            Assert.Null(fixture.Controller.Recall());
            fixture.Controller.UserId = 42;
            fixture.Request.SetupGet(e => e.IsAuthenticated).Returns(false);
            Assert.Null(fixture.Controller.Recall());
        }

        [Fact]
        public void RevokedMembershipPreventsReopeningStoredInput()
        {
            var fixture = new Fixture();
            fixture.Controller.Remember("private name");
            fixture.Memberships.Setup(e => e.GetActiveUserMembership(42)).Returns(new UserMembership
            {
                MembershipOption = new MembershipOption { SubscriptionType = MembershipOption.ESubscriptionType.Free }
            });
            var result = Assert.IsType<ContentResult>(fixture.Controller.Reopen());
            Assert.Equal(403, fixture.Response.Object.StatusCode);
            var payload = JObject.Parse(result.Content);
            Assert.Equal("membership_required", (string)payload["error"]["code"]);
            Assert.Equal(JTokenType.Null, payload["data"].Type);
        }

        [Fact]
        public void OversizedRequestsStopBeforeActionExecution()
        {
            var fixture = new Fixture();
            fixture.Request.SetupGet(e => e.ContentLength).Returns(65537);
            var context = fixture.Controller.Filter();
            Assert.IsType<ContentResult>(context.Result);
            Assert.Equal(413, fixture.Response.Object.StatusCode);
        }

        private sealed class Probe : MobileApiControllerBase
        {
            public int UserId = 42;
            public Probe(INineStarKiPackage package) : base(package) { }
            protected override int ResolveCurrentUserId() => UserId;
            public void Remember(string name) => SaveLastRequest("test", new Input { Name = name });
            public string Recall() => ReadLastRequest<Input>("test")?.Name;
            public ActionResult Reopen()
            {
                var error = RequireUser(out var id, paid: true);
                return error ?? ApiSuccess(Recall());
            }
            public ActionExecutingContext Filter()
            {
                var context = new ActionExecutingContext { Controller = this, HttpContext = HttpContext };
                OnActionExecuting(context);
                return context;
            }
        }

        private sealed class Input { public string Name { get; set; } }
        private sealed class SessionState : HttpSessionStateBase
        {
            private readonly Dictionary<string, object> _items = new Dictionary<string, object>();
            public override object this[string name]
            {
                get => _items.TryGetValue(name, out var value) ? value : null;
                set => _items[name] = value;
            }
        }

        private sealed class Fixture
        {
            public readonly Mock<HttpRequestBase> Request = new Mock<HttpRequestBase>();
            public readonly Mock<HttpResponseBase> Response = new Mock<HttpResponseBase>();
            public readonly Mock<IMembershipService> Memberships = new Mock<IMembershipService>();
            public readonly Probe Controller;
            public Fixture()
            {
                var package = new Mock<INineStarKiPackage>();
                package.SetupGet(e => e.UserService).Returns(new Mock<IUserService>().Object);
                package.SetupGet(e => e.MembershipService).Returns(Memberships.Object);
                Request.SetupGet(e => e.IsAuthenticated).Returns(true);
                Request.SetupGet(e => e.HttpMethod).Returns("GET");
                Response.SetupProperty(e => e.StatusCode);
                Response.SetupGet(e => e.Cache).Returns(new Mock<HttpCachePolicyBase>().Object);
                var context = new Mock<HttpContextBase>();
                context.SetupGet(e => e.Request).Returns(Request.Object);
                context.SetupGet(e => e.Response).Returns(Response.Object);
                context.SetupGet(e => e.Session).Returns(new SessionState());
                Controller = new Probe(package.Object);
                Controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), Controller);
            }
        }
    }
}
