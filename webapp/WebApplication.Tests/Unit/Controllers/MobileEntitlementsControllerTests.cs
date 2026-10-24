using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Controllers;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Controllers
{
    public class MobileEntitlementsControllerTests
    {
        [Fact]
        public void BothEndpointsExposeIdenticalAllowlistedSnapshotAndPreserveAccountFields()
        {
            var fixture = new Fixture();
            var entitlement = fixture.Entitlements()["data"];
            var account = fixture.Account()["data"];
            Assert.True(JToken.DeepEquals(entitlement, account["entitlements"]));
            Assert.True((bool)entitlement["paid"]);
            Assert.Equal("AnnualPlatinum", (string)entitlement["sources"][0]["subscriptionType"]);
            Assert.Equal("Si", (string)account["firstName"]);
            Assert.True((bool)account["membership"]["paid"]);
            Assert.Equal(7, (int)account["membership"]["complementaryPersonalChartReadingCount"]);
            Assert.Null(entitlement["userId"]);
            Assert.Null(entitlement["emailAddress"]);
            Assert.Null(entitlement["sources"][0]["accountNumber"]);
            Assert.Null(entitlement["sources"][0]["membershipOption"]);
            Assert.Equal(new[] { "autoRenewEnabled", "changeEffectiveAtUtc", "endsOn", "expiresAtUtc", "nextProductId",
                "nextSubscriptionType", "productId", "provider", "state", "subscriptionType", "verifiedAtUtc" },
                ((JObject)entitlement["sources"][0]).Properties().Select(e => e.Name).OrderBy(e => e).ToArray());
            fixture.Memberships.Verify(e => e.GetActiveUserMembership(It.IsAny<int?>()), Times.Never);
            fixture.Memberships.Verify(e => e.CreateFreeMembership(It.IsAny<int>()), Times.Never);
            fixture.Cache.Verify(e => e.SetNoStore(), Times.AtLeastOnce);
        }

        [Fact]
        public void MissingMembershipReturnsEmptySourcesWithoutCreatingOne()
        {
            var fixture = new Fixture();
            fixture.Grants.Clear();
            Assert.Empty((JArray)fixture.Entitlements()["data"]["sources"]);
            var account = fixture.Account()["data"];
            Assert.False((bool)account["entitlements"]["paid"]);
            Assert.Equal(JTokenType.Null, account["membership"].Type);
            fixture.Memberships.Verify(e => e.GetActiveUserMembership(It.IsAny<int?>()), Times.Never);
            fixture.Memberships.Verify(e => e.CreateFreeMembership(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void IdentityChangeAndLogoutCannotReusePaidSnapshot()
        {
            var fixture = new Fixture();
            Assert.True((bool)fixture.Entitlements()["data"]["paid"]);
            fixture.UserId = 43;
            Assert.False((bool)fixture.Entitlements()["data"]["paid"]);
            Assert.False((bool)fixture.Account()["data"]["entitlements"]["paid"]);
            fixture.Authenticated = false;
            var calls = fixture.MembershipReads;
            Assert.Equal("authentication_required", (string)fixture.Entitlements()["error"]["code"]);
            Assert.Equal("authentication_required", (string)fixture.Account()["error"]["code"]);
            Assert.Equal(calls, fixture.MembershipReads);
            Assert.Equal(401, fixture.Response.Object.StatusCode);
        }

        [Fact]
        public void RevocationIsRecheckedAndAdminRoleDoesNotGrantSubscriptionBenefits()
        {
            var fixture = new Fixture();
            Assert.True((bool)fixture.Entitlements()["data"]["paid"]);
            fixture.Grants[42].IsDeactivated = true;
            fixture.Users.Setup(e => e.UserIsAdmin(42)).Returns(true);
            Assert.False((bool)fixture.Entitlements()["data"]["paid"]);
            Assert.False((bool)fixture.Account()["data"]["entitlements"]["paid"]);
        }

        [Fact]
        public void StaleUserSessionCannotReadMembershipForMissingAccount()
        {
            var fixture = new Fixture();
            fixture.UserId = 99;
            Assert.Equal("account_not_found", (string)fixture.Entitlements()["error"]["code"]);
            Assert.Equal(404, fixture.Response.Object.StatusCode);
            fixture.Memberships.Verify(e => e.GetExistingActiveMembership(99), Times.Never);
        }

        private sealed class MembershipController : MobileMembershipController
        {
            private readonly Func<int> _identity;
            public MembershipController(INineStarKiPackage package, Func<int> identity) : base(package, null) { _identity = identity; }
            protected override int ResolveCurrentUserId() => _identity();
            protected override DateTime GetUtcNow() => new DateTime(2026, 10, 23, 20, 0, 0, DateTimeKind.Utc);
        }

        private sealed class AccountController : MobileAccountController
        {
            private readonly Func<int> _identity;
            public AccountController(INineStarKiPackage package, Func<int> identity) : base(package, null) { _identity = identity; }
            protected override int ResolveCurrentUserId() => _identity();
            protected override DateTime GetUtcNow() => new DateTime(2026, 10, 23, 20, 0, 0, DateTimeKind.Utc);
        }

        private sealed class Fixture
        {
            public int UserId = 42;
            public bool Authenticated = true;
            public int MembershipReads;
            public readonly Mock<IMembershipService> Memberships = new Mock<IMembershipService>(MockBehavior.Strict);
            public readonly Mock<IUserService> Users = new Mock<IUserService>();
            public readonly Mock<HttpResponseBase> Response = new Mock<HttpResponseBase>();
            public readonly Mock<HttpCachePolicyBase> Cache = new Mock<HttpCachePolicyBase>();
            public readonly Dictionary<int, UserMembership> Grants = new Dictionary<int, UserMembership>();
            private readonly MembershipController _memberships;
            private readonly AccountController _account;

            public Fixture()
            {
                Grants[42] = Grant(42, MembershipOption.ESubscriptionType.AnnualPlatinum);
                Grants[43] = Grant(43, MembershipOption.ESubscriptionType.Free);
                Memberships.Setup(e => e.GetExistingActiveMembership(It.IsAny<int>()))
                    .Callback(() => MembershipReads++)
                    .Returns((int id) => Grants.ContainsKey(id) ? Grants[id] : null);
                Users.Setup(e => e.Find(It.IsAny<int>())).Returns((int id) => id == 42 || id == 43
                    ? new User { Id = id, FirstName = "Si", BirthDate = new DateTime(1979, 6, 16) } : null);
                var info = new Mock<IRepository<UserInfo>>();
                info.Setup(e => e.Find(It.IsAny<Expression<Func<UserInfo, bool>>>())).Returns(new List<UserInfo>());
                var package = new Mock<INineStarKiPackage>();
                package.SetupGet(e => e.UserService).Returns(Users.Object);
                package.SetupGet(e => e.MembershipService).Returns(Memberships.Object);
                package.SetupGet(e => e.UserInfosRepository).Returns(info.Object);
                var request = new Mock<HttpRequestBase>();
                request.SetupGet(e => e.IsAuthenticated).Returns(() => Authenticated);
                Response.SetupProperty(e => e.StatusCode);
                Response.SetupGet(e => e.Cache).Returns(Cache.Object);
                var context = new Mock<HttpContextBase>();
                context.SetupGet(e => e.Request).Returns(request.Object);
                context.SetupGet(e => e.Response).Returns(Response.Object);
                _memberships = new MembershipController(package.Object, () => UserId);
                _account = new AccountController(package.Object, () => UserId);
                _memberships.ControllerContext = new ControllerContext(context.Object, new RouteData(), _memberships);
                _account.ControllerContext = new ControllerContext(context.Object, new RouteData(), _account);
            }

            public JObject Entitlements() => JObject.Parse(Assert.IsType<ContentResult>(_memberships.Entitlements()).Content);
            public JObject Account() => JObject.Parse(Assert.IsType<ContentResult>(_account.Me()).Content);

            private static UserMembership Grant(int id, MembershipOption.ESubscriptionType type) => new UserMembership
            {
                UserId = id, StartsOn = DateTime.Today.AddDays(-1), EndsOn = DateTime.Today.AddDays(5),
                ComplementaryPersonalChartReadingCount = 7,
                MembershipOption = new MembershipOption { SubscriptionType = type }
            };
        }
    }
}
