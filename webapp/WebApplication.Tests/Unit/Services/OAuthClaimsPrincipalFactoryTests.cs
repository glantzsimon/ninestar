using K9.WebApplication.Services;
using OpenIddict.Abstractions;
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

        [Fact]
        public void OfflineAccessIsOptionalAndDoesNotAddOtherPermissions()
        {
            var principal = OAuthClaimsPrincipalFactory.Create(
                new OAuthAccountIdentity(27, "123456789"),
                new[] { OAuthClaimsPrincipalFactory.ReadScope, OpenIddictConstants.Scopes.OfflineAccess });

            Assert.True(principal.HasScope(OAuthClaimsPrincipalFactory.ReadScope));
            Assert.True(principal.HasScope(OpenIddictConstants.Scopes.OfflineAccess));
            Assert.False(principal.HasScope(OpenIddictConstants.Scopes.OpenId));
            Assert.Single(principal.GetResources());
            Assert.Equal(OAuthClaimsPrincipalFactory.McpResource, principal.GetResources().Single());
        }

        [Theory]
        [InlineData("offline_access")]
        [InlineData("openid")]
        [InlineData("nine-star-ki.admin")]
        public void RenewalPermissionAloneCannotGrantWebsiteAccess(string scope)
        {
            Assert.False(OAuthClaimsPrincipalFactory.IsSupportedScopes(new[] { scope }));
        }

        [Fact]
        public void RenewalPreservesValidatedGrantMetadataAndRefreshesMembership()
        {
            var scopes = new[] { OAuthClaimsPrincipalFactory.ReadScope, OpenIddictConstants.Scopes.OfflineAccess };
            var original = OAuthClaimsPrincipalFactory.Create(new OAuthAccountIdentity(27, "123456789"), scopes);
            original.SetAuthorizationId("approved-grant");
            original.SetPresenters("registered-client");

            var renewed = OAuthClaimsPrincipalFactory.Renew(
                new OAuthAccountIdentity(27, "987654321"), original, scopes);

            Assert.Equal("27", renewed.FindFirst("sub").Value);
            Assert.Equal("987654321", renewed.FindFirst(OAuthClaimsPrincipalFactory.MembershipAccountClaim).Value);
            Assert.Equal("123456789", original.FindFirst(OAuthClaimsPrincipalFactory.MembershipAccountClaim).Value);
            Assert.Equal("approved-grant", renewed.GetAuthorizationId());
            Assert.Equal("registered-client", renewed.GetPresenters().Single());
            Assert.True(renewed.HasScope(OpenIddictConstants.Scopes.OfflineAccess));
            Assert.Contains(OpenIddictConstants.Destinations.AccessToken,
                renewed.FindFirst(OAuthClaimsPrincipalFactory.MembershipAccountClaim).GetDestinations());
        }

        [Fact]
        public void RenewalCannotChangeUserOrExpandTheOriginalScopes()
        {
            var scopes = new[] { OAuthClaimsPrincipalFactory.ReadScope };
            var principal = OAuthClaimsPrincipalFactory.Create(new OAuthAccountIdentity(27, "123456789"), scopes);

            Assert.Throws<ArgumentException>(() => OAuthClaimsPrincipalFactory.Renew(
                new OAuthAccountIdentity(28, "987654321"), principal, scopes));
            Assert.Throws<ArgumentException>(() => OAuthClaimsPrincipalFactory.Renew(
                new OAuthAccountIdentity(27, "123456789"), principal,
                new[] { OAuthClaimsPrincipalFactory.ReadScope, OpenIddictConstants.Scopes.OfflineAccess }));
        }

        [Fact]
        public void RenewalCanDropOfflineAccess()
        {
            var principal = OAuthClaimsPrincipalFactory.Create(new OAuthAccountIdentity(27, "123456789"),
                new[] { OAuthClaimsPrincipalFactory.ReadScope, OpenIddictConstants.Scopes.OfflineAccess });

            var renewed = OAuthClaimsPrincipalFactory.Renew(new OAuthAccountIdentity(27, "123456789"),
                principal, new[] { OAuthClaimsPrincipalFactory.ReadScope });

            Assert.False(renewed.HasScope(OpenIddictConstants.Scopes.OfflineAccess));
            Assert.True(renewed.HasScope(OAuthClaimsPrincipalFactory.ReadScope));
        }
    }
}

