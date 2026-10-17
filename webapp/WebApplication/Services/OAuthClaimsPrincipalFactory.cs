using OpenIddict.Abstractions;
using OpenIddict.Server.Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
}
