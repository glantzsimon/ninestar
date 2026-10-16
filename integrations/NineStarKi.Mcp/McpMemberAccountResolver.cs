using System.Security.Claims;

namespace NineStarKi.Mcp;

/// <summary>
/// In production, derive membership from an issuer-signed website identity.
/// A user-supplied account number NEVER overrides the verified membership.
/// During private staging only, the legacy account-number parameter is used.
/// </summary>
public sealed class McpMemberAccountResolver(
    IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
{
    public string Resolve(string? legacyAccountNumber)
    {
        if (!configuration.GetValue<bool>("Mcp:RequireUserOAuth"))
        {
            if (string.IsNullOrWhiteSpace(legacyAccountNumber))
            {
                throw new ArgumentException(
                    "An existing 9 Star Ki membership account number is required during private testing.");
            }

            return AstrologyApiClient.AccountNumber(legacyAccountNumber);
        }

        var principal = httpContextAccessor.HttpContext?.User;
        if (principal?.Identity?.IsAuthenticated != true ||
            string.IsNullOrWhiteSpace(principal.FindFirst("sub")?.Value))
        {
            throw new UnauthorizedAccessException("An authorised 9 Star Ki login is required.");
        }

        // This value is issued only by the website authorisation server and
        // validated as part of the signed OAuth access token at /mcp.
        var verifiedAccount = principal.FindFirst("membership_account")?.Value;
        if (string.IsNullOrWhiteSpace(verifiedAccount))
        {
            throw new UnauthorizedAccessException("No website membership is associated with this login.");
        }

        // Only an identity verified by the OAuth issuer determines access.
        return AstrologyApiClient.AccountNumber(verifiedAccount);
    }
}
