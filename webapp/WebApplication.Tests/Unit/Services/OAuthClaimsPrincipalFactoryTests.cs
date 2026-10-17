using K9.WebApplication.Services;
using System;
using System.Linq;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Services
{
    public class OAuthClaimsPrincipalFactoryTests
    {
        [Fact]
        public void VerifiedMembershipIsBoundToItsWebsiteUser()
        {
            var account = new OAuthAccountIdentity(27, "123456789");

            var principal = OAuthClaimsPrincipalFactory.Create(
                account, new[] { OAuthClaimsPrincipalFactory.ReadScope });

            Assert.Equal("27", principal.FindFirst("sub")?.Value);
            Assert.Equal("123456789",
                principal.FindFirst(OAuthClaimsPrincipalFactory.MembershipAccountClaim)?.Value);
            Assert.NotNull(principal.Identity);
            Assert.True(principal.Identity.IsAuthenticated);
        }

        [Fact]
        public void UnknownScopesAreRejectedRatherThanGranted()
        {
            var account = new OAuthAccountIdentity(27, "123456789");

            Assert.Throws<ArgumentException>(() => OAuthClaimsPrincipalFactory.Create(
                account, new[] { "nine-star-ki.admin" }));
        }

        [Fact]
        public void MultipleScopesAreRejectedRatherThanGranted()
        {
            var account = new OAuthAccountIdentity(27, "123456789");

            Assert.Throws<ArgumentException>(() => OAuthClaimsPrincipalFactory.Create(
                account, new[] { OAuthClaimsPrincipalFactory.ReadScope, "profile" }));
        }

        [Fact]
        public void MissingMembershipIdentityCannotProduceClaims()
        {
            Assert.Throws<ArgumentNullException>(() => OAuthClaimsPrincipalFactory.Create(
                null, new[] { OAuthClaimsPrincipalFactory.ReadScope }));

            Assert.Throws<ArgumentException>(() => new OAuthAccountIdentity(12, ""));
            Assert.Throws<ArgumentOutOfRangeException>(() => new OAuthAccountIdentity(0, "123456789"));
        }

        [Fact]
        public void EmptyScopeRequestIsRejected()
        {
            var account = new OAuthAccountIdentity(27, "123456789");

            Assert.Throws<ArgumentException>(() => OAuthClaimsPrincipalFactory.Create(
                account, Enumerable.Empty<string>()));
        }
    }
}
