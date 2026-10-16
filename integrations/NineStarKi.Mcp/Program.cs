using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ModelContextProtocol.AspNetCore;
using ModelContextProtocol.Server;
using NineStarKi.Mcp;

var builder = WebApplication.CreateBuilder(args);

// Secrets are configured only on the IIS application pool; never in Git.
var upstreamApiKey = builder.Configuration["NineStarKi:ApiKey"];
var mcpBearerToken = builder.Configuration["Mcp:BearerToken"];
if (string.IsNullOrWhiteSpace(upstreamApiKey) || string.IsNullOrWhiteSpace(mcpBearerToken))
{
    throw new InvalidOperationException(
        "Missing NineStarKi__ApiKey or Mcp__BearerToken. Server is deliberately disabled.");
}

var baseUrl = builder.Configuration["NineStarKi:BaseUrl"] ?? "https://9starkiastrology.com/";
if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri) ||
    baseUri.Scheme != Uri.UriSchemeHttps)
{
    throw new InvalidOperationException("NineStarKi__BaseUrl must be an HTTPS URL.");
}

// OAuth is opt-in for the separate MCP server. When no authority is configured,
// the existing private Bearer-token flow keeps working unchanged.
// An established OAuth provider is the authorization server: this application
// ONLY verifies its signed access tokens. Never accept unverified JWT claims.
var oauthAuthority = builder.Configuration["Mcp:OAuthAuthority"];
var oauthAudience = builder.Configuration["Mcp:OAuthAudience"];
var oauthSubject = builder.Configuration["Mcp:OAuthSubject"];
var oauthScope = builder.Configuration["Mcp:OAuthScope"] ?? "read:9starki";
var publicBaseUrl = builder.Configuration["Mcp:PublicBaseUrl"] ?? "https://mcp.9starkiastrology.com";

if (!Uri.TryCreate(publicBaseUrl, UriKind.Absolute, out var publicBaseUri) ||
    publicBaseUri.Scheme != Uri.UriSchemeHttps ||
    publicBaseUri.AbsolutePath != "/" || !string.IsNullOrEmpty(publicBaseUri.Query) ||
    !string.IsNullOrEmpty(publicBaseUri.Fragment) || !string.IsNullOrEmpty(publicBaseUri.UserInfo))
{
    throw new InvalidOperationException("Mcp__PublicBaseUrl must be an HTTPS origin without a path.");
}

var resource = publicBaseUri.GetLeftPart(UriPartial.Authority) + "/mcp";
var resourceMetadata = publicBaseUri.GetLeftPart(UriPartial.Authority) +
    "/.well-known/oauth-protected-resource";

var oauthEnabled = !string.IsNullOrWhiteSpace(oauthAuthority);
if (oauthEnabled)
{
    if (!Uri.TryCreate(oauthAuthority, UriKind.Absolute, out var authorityUri) ||
        authorityUri.Scheme != Uri.UriSchemeHttps ||
        !string.IsNullOrEmpty(authorityUri.Query) ||
        !string.IsNullOrEmpty(authorityUri.Fragment) ||
        !string.IsNullOrEmpty(authorityUri.UserInfo))
    {
        throw new InvalidOperationException("Mcp__OAuthAuthority must be a trusted HTTPS issuer URL.");
    }

    if (string.IsNullOrWhiteSpace(oauthAudience) ||
        string.IsNullOrWhiteSpace(oauthScope) ||
        string.IsNullOrWhiteSpace(oauthSubject))
    {
        throw new InvalidOperationException(
            "Mcp__OAuthAudience, Mcp__OAuthScope and Mcp__OAuthSubject must be configured for OAuth.");
    }

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = oauthAuthority;
            options.Audience = oauthAudience;
            options.RequireHttpsMetadata = true;
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.FromSeconds(60)
            };
        });
}

builder.Services.AddHttpClient<AstrologyApiClient>(client =>
{
    client.BaseAddress = new Uri(baseUri.AbsoluteUri.TrimEnd('/') + "/");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("ApiKey", upstreamApiKey);
    client.Timeout = TimeSpan.FromSeconds(40);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false });

builder.Services.AddMcpServer()
    .WithHttpTransport(options => options.SessionMode = HttpServerSessionMode.Stateless)
    .WithTools<AstrologyTools>();

var app = builder.Build();

// Advertise the OAuth issuer using RFC 9728 protected resource metadata.
// The OAuth authorization-code/PKCE and refresh-token endpoints must be
// supplied by the configured identity provider, never improvised here.
if (oauthEnabled)
{
    object metadata = new
    {
        resource,
        authorization_servers = new[] { oauthAuthority!.TrimEnd('/') },
        bearer_methods_supported = new[] { "header" },
        scopes_supported = new[] { oauthScope }
    };
    app.MapGet("/.well-known/oauth-protected-resource", () => Results.Json(metadata));
    app.MapGet("/.well-known/oauth-protected-resource/mcp", () => Results.Json(metadata));
}

// Protect EVERY MCP request, including tools/list.
// Keep the original private operator token for diagnostics; JWT OAuth support
// becomes active only after the provider has been configured explicitly.
app.Use(async (context, next) =>
{
    if (!context.Request.Path.StartsWithSegments("/mcp"))
    {
        await next();
        return;
    }

    var authorization = context.Request.Headers.Authorization.ToString();
    const string prefix = "Bearer ";
    var supplied = authorization.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
        ? authorization[prefix.Length..].Trim()
        : string.Empty;

    var suppliedBytes = Encoding.UTF8.GetBytes(supplied);
    var expectedBytes = Encoding.UTF8.GetBytes(mcpBearerToken);
    if (suppliedBytes.Length > 0 &&
        CryptographicOperations.FixedTimeEquals(suppliedBytes, expectedBytes))
    {
        await next();
        return;
    }

    if (oauthEnabled && suppliedBytes.Length > 0)
    {
        var authentication = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
        if (authentication.Succeeded && authentication.Principal is not null &&
            HasRequiredScope(authentication.Principal, oauthScope) &&
            string.Equals(authentication.Principal.FindFirst("sub")?.Value, oauthSubject, StringComparison.Ordinal))
        {
            context.User = authentication.Principal;
            await next();
            return;
        }
    }

    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    context.Response.Headers.WWWAuthenticate = oauthEnabled
        ? $"Bearer resource_metadata=\"{resourceMetadata}\""
        : "Bearer";
});

// A readiness probe only: never return account details, tokens or API data.
app.MapGet("/healthz", () => Results.Ok(new { status = "ready" }));
app.MapMcp("/mcp");
app.Run();

static bool HasRequiredScope(ClaimsPrincipal user, string requiredScope) =>
    user.FindAll("scope").Concat(user.FindAll("scp"))
        .SelectMany(claim => claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        .Any(scope => string.Equals(scope, requiredScope, StringComparison.Ordinal));
