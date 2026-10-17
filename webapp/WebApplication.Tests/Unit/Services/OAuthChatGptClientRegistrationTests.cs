using K9.WebApplication.Services;
using OpenIddict.Abstractions;
using System;
using System.Linq;
using Xunit;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace K9.WebApplication.Tests.Unit.Services
{
    public class OAuthChatGptClientRegistrationTests
    {
        private const string Client = "https://chatgpt.com/oauth/client.json";
        private const string StableCallback =
            "https://chatgpt.com/connector_platform_oauth_redirect";

        [Fact]
        public void StableCallbackIsRegisteredAsAnExplicitConsentPublicClient()
        {
            var registration = OAuthChatGptClientRegistration.CreateDescriptor(
                Client, StableCallback);

            Assert.Equal(Client, registration.ClientId);
            Assert.Equal(ClientTypes.Public, registration.ClientType);
            Assert.Equal(ConsentTypes.Explicit, registration.ConsentType);
            Assert.Single(registration.RedirectUris);
            Assert.Equal(StableCallback,
                registration.RedirectUris.Single().AbsoluteUri);
            Assert.Contains(Permissions.GrantTypes.AuthorizationCode,
                registration.Permissions);
            Assert.Contains(Permissions.Prefixes.Scope +
                OAuthClaimsPrincipalFactory.ReadScope, registration.Permissions);
            Assert.Contains(Requirements.Features.ProofKeyForCodeExchange,
                registration.Requirements);
            Assert.DoesNotContain(Permissions.GrantTypes.RefreshToken,
                registration.Permissions);
            Assert.DoesNotContain(Permissions.GrantTypes.Password,
                registration.Permissions);
        }

        [Fact]
        public void ConnectionSpecificChatGptCallbackIsAccepted()
        {
            var registration = OAuthChatGptClientRegistration.CreateDescriptor(
                Client, "https://chatgpt.com/connector/oauth/12345");

            Assert.Equal("https://chatgpt.com/connector/oauth/12345",
                registration.RedirectUris.Single().AbsoluteUri);
        }

        [Theory]
        [InlineData("http://chatgpt.com/connector_platform_oauth_redirect")]
        [InlineData("https://evil.example/connector_platform_oauth_redirect")]
        [InlineData("https://chatgpt.com.evil.example/connector_platform_oauth_redirect")]
        [InlineData("https://chatgpt.com/other/path")]
        [InlineData("https://chatgpt.com/connector/oauth/")]
        [InlineData("https://chatgpt.com/connector/oauth/abc/def")]
        [InlineData("https://chatgpt.com/connector_platform_oauth_redirect?next=evil")]
        [InlineData("https://chatgpt.com/connector_platform_oauth_redirect#fragment")]
        [InlineData("https://chatgpt.com:8443/connector_platform_oauth_redirect")]
        public void OtherCallbacksAreRejected(string callback)
        {
            Assert.Throws<ArgumentException>(() =>
                OAuthChatGptClientRegistration.CreateDescriptor(Client, callback));
        }

        [Fact]
        public void ClientIdIsRequired()
        {
            Assert.Throws<ArgumentException>(() =>
                OAuthChatGptClientRegistration.CreateDescriptor("", StableCallback));
        }
    }
}
