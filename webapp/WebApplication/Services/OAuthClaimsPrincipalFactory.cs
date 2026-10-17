using OpenIddict.Abstractions;
using OpenIddict.Server.Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace K9.WebApplication.Services
{
    /// <summary>
    /// Constructs the delegated access-token principal from a membership
    /// resolved by the EXISTING website login, not from client-supplied data.
    /// The OpenIddict authorisation endpoint will call this only after
    /// authenticating the customer and obtaining their consent.
    /// </summary>
    public static class OAuthClaimsPrincipalFactory
    {
        public const string ReadScope = "nine-star-ki.read";
        public const string McpResource = "https://mcp.9starkiastrology.com/mcp";
        public const string MembershipAccountClaim = "membership_account";

        public static ClaimsPrincipal Create(
            OAuthAccountIdentity account,
            IEnumerable<string> requestedScopes)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            if (requestedScopes == null)
            {
                throw new ArgumentNullException(nameof(requestedScopes));
            }

            // For the initial ChatGPT connection we support precisely one
            // delegated scope. Reject unknown scopes instead of silently
            // broadening the permissions granted to the client.
            var scopes = requestedScopes.ToArray();
            if (scopes.Length != 1 ||
                !string.Equals(scopes[0], ReadScope, StringComparison.Ordinal))
            {
                throw new ArgumentException(
                    "The requested OAuth scopes are not supported.", nameof(requestedScopes));
            }

            var identity = new ClaimsIdentity(
                authenticationType: OpenIddictServerOwinDefaults.AuthenticationType,
                nameType: Claims.Name,
                roleType: Claims.Role);

            // The user ID was read from WebSecurity.CurrentUserId; the account
            // number was looked up for that same verified user's membership.
            identity.SetClaim(Claims.Subject,
                account.UserId.ToString(CultureInfo.InvariantCulture));
            identity.SetClaim(MembershipAccountClaim, account.MembershipAccountNumber);

            identity.SetScopes(ReadScope);
            identity.SetResources(McpResource);

            // Claims are not included in OpenIddict access tokens by default.
            // Never put the membership account into an identity token.
            identity.SetDestinations(claim =>
                claim.Type == Claims.Subject || claim.Type == MembershipAccountClaim
                    ? new[] { Destinations.AccessToken }
                    : Array.Empty<string>());

            return new ClaimsPrincipal(identity);
        }
    }

    // Public ChatGPT client-registration helpers; no startup-side effects.
    public static class OAuthChatGptClientRegistration
    {
        /// <summary>
        /// Builds one public ChatGPT client restricted to the exact callback
        /// from the ChatGPT MCP connection settings. No wildcard redirects,
        /// client secrets, implicit grants, password grants or refresh grants.
        /// </summary>
        public static OpenIddictApplicationDescriptor CreateDescriptor(
            string configuredClientId, string configuredRedirectUri)
        {
            if (string.IsNullOrWhiteSpace(configuredClientId))
            {
                throw new ArgumentException("The OAuth client ID is required.",
                    nameof(configuredClientId));
            }

            if (!Uri.TryCreate(configuredRedirectUri, UriKind.Absolute, out var redirect) ||
                redirect.Scheme != Uri.UriSchemeHttps ||
                !redirect.IsDefaultPort ||
                !string.Equals(redirect.Host, "chatgpt.com", StringComparison.OrdinalIgnoreCase) ||
                !string.IsNullOrWhiteSpace(redirect.UserInfo) ||
                !string.IsNullOrEmpty(redirect.Query) ||
                !string.IsNullOrEmpty(redirect.Fragment) ||
                !(string.Equals(redirect.AbsolutePath, "/connector_platform_oauth_redirect",
                      StringComparison.Ordinal) ||
                  (redirect.AbsolutePath.StartsWith("/connector/oauth/", StringComparison.Ordinal) &&
                   redirect.AbsolutePath.Length > "/connector/oauth/".Length &&
                   !redirect.AbsolutePath.Substring("/connector/oauth/".Length).Contains("/"))))
            {
                throw new ArgumentException(
                    "The callback must be the exact HTTPS ChatGPT redirect URI.",
                    nameof(configuredRedirectUri));
            }

            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = configuredClientId,
                ClientType = ClientTypes.Public,
                ConsentType = ConsentTypes.Explicit,
                DisplayName = "ChatGPT",
                RedirectUris = { redirect },
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.ResponseTypes.Code,
                    Permissions.Prefixes.Scope + OAuthClaimsPrincipalFactory.ReadScope
                },
                Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange
                }
            };

            return descriptor;
        }

        /// <summary>
        /// Idempotently registers the configured public client.
        /// Existing clients with different security settings are NOT updated
        /// or trusted silently. They require explicit administrator review.
        /// </summary>
        public static async Task RegisterAsync(
            IOpenIddictApplicationManager manager,
            string configuredClientId, string configuredRedirectUri)
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));

            var descriptor = CreateDescriptor(configuredClientId, configuredRedirectUri);
            var existing = await manager.FindByClientIdAsync(descriptor.ClientId);
            if (existing == null)
            {
                await manager.CreateAsync(descriptor);
                return;
            }

            var registeredRedirects = await manager.GetRedirectUrisAsync(existing);
            var registeredPermissions = await manager.GetPermissionsAsync(existing);
            var registeredRequirements = await manager.GetRequirementsAsync(existing);

            if (!string.Equals(await manager.GetClientTypeAsync(existing), descriptor.ClientType,
                    StringComparison.Ordinal) ||
                !string.Equals(await manager.GetConsentTypeAsync(existing), descriptor.ConsentType,
                    StringComparison.Ordinal) ||
                registeredRedirects.Length != 1 ||
                !string.Equals(registeredRedirects[0], descriptor.RedirectUris.Single().AbsoluteUri,
                    StringComparison.Ordinal) ||
                !new HashSet<string>(registeredPermissions, StringComparer.Ordinal)
                    .SetEquals(descriptor.Permissions) ||
                !new HashSet<string>(registeredRequirements, StringComparer.Ordinal)
                    .SetEquals(descriptor.Requirements))
            {
                throw new InvalidOperationException(
                    "The existing ChatGPT OAuth client does not match the approved registration.");
            }
        }
    }
}
