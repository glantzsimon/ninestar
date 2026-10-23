using K9.WebApplication.Services;
using OpenIddict.Abstractions;
using Moq;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            Assert.Contains(Permissions.GrantTypes.RefreshToken,
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

        [Fact]
        public async Task ExactLegacyRegistrationReceivesOnlyRefreshPermission()
        {
            var application = new object();
            var descriptor = OAuthChatGptClientRegistration.CreateDescriptor(Client, StableCallback);
            descriptor.Permissions.Remove(Permissions.GrantTypes.RefreshToken);
            var manager = ExistingRegistration(application, descriptor);
            manager.Setup(e => e.PopulateAsync(It.IsAny<OpenIddictApplicationDescriptor>(), application,
                    It.IsAny<CancellationToken>()))
                .Callback<OpenIddictApplicationDescriptor, object, CancellationToken>((updated, _, __) =>
                {
                    updated.ClientId = descriptor.ClientId;
                    updated.DisplayName = "Preserved display name";
                    foreach (var permission in descriptor.Permissions) updated.Permissions.Add(permission);
                }).Returns(default(ValueTask));
            manager.Setup(e => e.UpdateAsync(application, It.IsAny<OpenIddictApplicationDescriptor>(),
                    It.IsAny<CancellationToken>())).Returns(default(ValueTask));

            await OAuthChatGptClientRegistration.RegisterAsync(manager.Object, Client, StableCallback);

            manager.Verify(e => e.UpdateAsync(application,
                It.Is<OpenIddictApplicationDescriptor>(updated =>
                    updated.DisplayName == "Preserved display name" &&
                    updated.Permissions.Count == descriptor.Permissions.Count + 1 &&
                    updated.Permissions.Contains(Permissions.GrantTypes.RefreshToken)),
                It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task CurrentRegistrationNeedsNoWrite()
        {
            var manager = ExistingRegistration(new object(),
                OAuthChatGptClientRegistration.CreateDescriptor(Client, StableCallback));

            // Strict mock: any attempt to update this registration fails.
            await OAuthChatGptClientRegistration.RegisterAsync(manager.Object, Client, StableCallback);
        }

        [Theory]
        [InlineData("redirect")]
        [InlineData("pkce")]
        [InlineData("permission")]
        [InlineData("client-type")]
        [InlineData("consent")]
        public async Task LegacyUpgradeRejectsUnrelatedSecurityDifferences(string difference)
        {
            var descriptor = OAuthChatGptClientRegistration.CreateDescriptor(Client, StableCallback);
            descriptor.Permissions.Remove(Permissions.GrantTypes.RefreshToken);
            if (difference == "redirect") descriptor.RedirectUris.Add(new Uri("https://evil.example/callback"));
            if (difference == "pkce") descriptor.Requirements.Clear();
            if (difference == "permission") descriptor.Permissions.Add(Permissions.GrantTypes.Password);
            if (difference == "client-type") descriptor.ClientType = ClientTypes.Confidential;
            if (difference == "consent") descriptor.ConsentType = ConsentTypes.Implicit;
            var manager = ExistingRegistration(new object(), descriptor);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                OAuthChatGptClientRegistration.RegisterAsync(manager.Object, Client, StableCallback));
        }

        private static Mock<IOpenIddictApplicationManager> ExistingRegistration(
            object application, OpenIddictApplicationDescriptor descriptor)
        {
            var manager = new Mock<IOpenIddictApplicationManager>(MockBehavior.Strict);
            manager.Setup(e => e.FindByClientIdAsync(Client, It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<object>(application));
            manager.Setup(e => e.GetRedirectUrisAsync(application, It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<ImmutableArray<string>>(
                    descriptor.RedirectUris.Select(uri => uri.AbsoluteUri).ToImmutableArray()));
            manager.Setup(e => e.GetPermissionsAsync(application, It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<ImmutableArray<string>>(descriptor.Permissions.ToImmutableArray()));
            manager.Setup(e => e.GetRequirementsAsync(application, It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<ImmutableArray<string>>(descriptor.Requirements.ToImmutableArray()));
            manager.Setup(e => e.GetClientTypeAsync(application, It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<string>(descriptor.ClientType));
            manager.Setup(e => e.GetConsentTypeAsync(application, It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<string>(descriptor.ConsentType));
            return manager;
        }
    }
}

