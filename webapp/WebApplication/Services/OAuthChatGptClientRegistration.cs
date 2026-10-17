// Client records must only be created AFTER the OAuth tables have been
// migrated in the existing website DataAccess context. Not wired to startup.
#if NINESTAR_OAUTH
using OpenIddict.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace K9.WebApplication.Services
{
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
                !registeredPermissions.ToHashSet(StringComparer.Ordinal)
                    .SetEquals(descriptor.Permissions) ||
                !registeredRequirements.ToHashSet(StringComparer.Ordinal)
                    .SetEquals(descriptor.Requirements))
            {
                throw new InvalidOperationException(
                    "The existing ChatGPT OAuth client does not match the approved registration.");
            }
        }
    }
}
#endif
