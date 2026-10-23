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

            // Offline access permits renewal, not additional website access.
            // Old connections requesting only the read scope remain supported.
            var scopes = requestedScopes.ToArray();
            if (!IsSupportedScopes(scopes))
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

            identity.SetScopes(scopes);
            identity.SetResources(McpResource);

            // Claims are not included in OpenIddict access tokens by default.
            // Never put the membership account into an identity token.
            identity.SetDestinations(claim =>
                claim.Type == Claims.Subject || claim.Type == MembershipAccountClaim
                    ? new[] { Destinations.AccessToken }
                    : Array.Empty<string>());

            return new ClaimsPrincipal(identity);
        }

        public static bool IsSupportedScopes(IEnumerable<string> requestedScopes)
        {
            if (requestedScopes == null) return false;

            var scopes = requestedScopes.ToArray();
            return scopes.Contains(ReadScope, StringComparer.Ordinal) &&
                   scopes.Distinct(StringComparer.Ordinal).Count() == scopes.Length &&
                   scopes.All(scope => string.Equals(scope, ReadScope, StringComparison.Ordinal) ||
                       string.Equals(scope, Scopes.OfflineAccess, StringComparison.Ordinal));
        }

        public static ClaimsPrincipal Renew(OAuthAccountIdentity account,
            ClaimsPrincipal validatedPrincipal, IEnumerable<string> requestedScopes)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            if (validatedPrincipal == null) throw new ArgumentNullException(nameof(validatedPrincipal));

            var identity = validatedPrincipal.Identity as ClaimsIdentity;
            if (identity == null || !string.Equals(identity.FindFirst(Claims.Subject)?.Value,
                    account.UserId.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal) ||
                !IsSupportedScopes(validatedPrincipal.GetScopes()) ||
                !IsSupportedScopes(requestedScopes))
            {
                throw new ArgumentException("The validated OAuth grant is not supported.");
            }

            var scopes = requestedScopes.ToArray();
            if (scopes.Except(validatedPrincipal.GetScopes(), StringComparer.Ordinal).Any())
            {
                throw new ArgumentException("The OAuth grant cannot be broadened.", nameof(requestedScopes));
            }

            // Preserve OpenIddict's validated authorization ID, presenters and
            // token metadata so revocation and rotation remain library-managed.
            // Token validation uses its own authentication type. OWIN selects the
            // sign-in handler by this value, so explicitly target the server
            // while retaining the validated identity and all grant claims.
            var renewedIdentity = new ClaimsIdentity(identity, null,
                OpenIddictServerOwinDefaults.AuthenticationType,
                identity.NameClaimType, identity.RoleClaimType);
            renewedIdentity.SetScopes(scopes);
            renewedIdentity.SetClaim(MembershipAccountClaim, account.MembershipAccountNumber);
            renewedIdentity.SetDestinations(claim =>
                claim.Type == Claims.Subject || claim.Type == MembershipAccountClaim
                    ? new[] { Destinations.AccessToken }
                    : Array.Empty<string>());

            return new ClaimsPrincipal(renewedIdentity);
        }
    }

    // Public ChatGPT client-registration helpers; no startup-side effects.
    public static class OAuthChatGptClientRegistration
    {
        /// <summary>
        /// Builds one public ChatGPT client restricted to the exact callback
        /// from the ChatGPT MCP connection settings. No wildcard redirects,
        /// client secrets, implicit grants or password grants. Refresh is
        /// permitted only for a previously approved offline-access grant.
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
                    Permissions.GrantTypes.RefreshToken,
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
        /// Only the exact previous code-only registration may be upgraded by
        /// adding refresh permission. All other security differences fail closed.
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
                !new HashSet<string>(registeredRequirements, StringComparer.Ordinal)
                    .SetEquals(descriptor.Requirements))
            {
                throw new InvalidOperationException(
                    "The existing ChatGPT OAuth client does not match the approved registration.");
            }

            var permissions = new HashSet<string>(registeredPermissions, StringComparer.Ordinal);
            if (permissions.SetEquals(descriptor.Permissions)) return;

            var previousPermissions = new HashSet<string>(descriptor.Permissions, StringComparer.Ordinal);
            previousPermissions.Remove(Permissions.GrantTypes.RefreshToken);
            if (!permissions.SetEquals(previousPermissions))
            {
                throw new InvalidOperationException(
                    "The existing ChatGPT OAuth client does not match the approved registration.");
            }

            // Preserve stored display metadata and properties; the only change
            // to the exact legacy registration is the approved refresh grant.
            var updated = new OpenIddictApplicationDescriptor();
            await manager.PopulateAsync(updated, existing);
            updated.Permissions.Add(Permissions.GrantTypes.RefreshToken);
            await manager.UpdateAsync(existing, updated);
        }
    }
}

