using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ModelContextProtocol.AspNetCore;
using ModelContextProtocol.Server;
using NineStarKi.Mcp;

var builder = WebApplication.CreateBuilder(args);

// Two separate secrets, scoped to this IIS application pool.
// Neither is committed to Git nor exposed to ChatGPT users.
var upstreamApiKey = builder.Configuration["NineStarKi:ApiKey"];
var mcpBearerToken = builder.Configuration["Mcp:BearerToken"];
if (string.IsNullOrWhiteSpace(upstreamApiKey) || string.IsNullOrWhiteSpace(mcpBearerToken))
{
    throw new InvalidOperationException("Missing NineStarKi__ApiKey or Mcp__BearerToken.");
}

var baseUrl = builder.Configuration["NineStarKi:BaseUrl"] ?? "https://9starkiastrology.com/";
if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri) ||
    baseUri.Scheme != Uri.UriSchemeHttps)
{
    throw new InvalidOperationException("NineStarKi__BaseUrl must be an HTTPS URL.");
}

var requireOpenAiMtls = builder.Configuration.GetValue<bool>("Mcp:RequireOpenAiMtls");
var requireUserOAuth = builder.Configuration.GetValue<bool>("Mcp:RequireUserOAuth");

// The OAuth issuer will be the EXISTING website's authorisation server, not
// a new external user directory. Don't enable this until that issuer is live.
var issuer = builder.Configuration["Mcp:OAuthAuthority"];
const string resource = "https://mcp.9starkiastrology.com/mcp";
const string metadataUrl = "https://mcp.9starkiastrology.com/.well-known/oauth-protected-resource";
const string requiredScope = "nine-star-ki.read";

if (requireUserOAuth)
{
    if (!Uri.TryCreate(issuer, UriKind.Absolute, out var authorityUri) ||
        authorityUri.Scheme != Uri.UriSchemeHttps ||
        !string.IsNullOrEmpty(authorityUri.UserInfo) ||
        !string.IsNullOrEmpty(authorityUri.Query) ||
        !string.IsNullOrEmpty(authorityUri.Fragment))
    {
        throw new InvalidOperationException("Mcp__OAuthAuthority must be a trusted HTTPS OAuth issuer.");
    }

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = issuer;
            options.Audience = resource;
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

// Required to resolve the validated website membership identity in each tool.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<McpMemberAccountResolver>();

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

// mTLS is additional ChatGPT-client authentication. It is never a replacement
// for a customer's OAuth identity or membership entitlements.
OpenAiMtlsCertificateValidator? openAiMtlsValidator = null;
if (requireOpenAiMtls)
{
    var rootCaPath = builder.Configuration["Mcp:OpenAiRootCaPath"];
    var intermediateCaPath = builder.Configuration["Mcp:OpenAiIntermediateCaPath"];
    if (string.IsNullOrWhiteSpace(rootCaPath) ||
        string.IsNullOrWhiteSpace(intermediateCaPath))
    {
        throw new InvalidOperationException("Missing OpenAI mTLS CA paths.");
    }

    openAiMtlsValidator = new OpenAiMtlsCertificateValidator(rootCaPath, intermediateCaPath);
}

var app = builder.Build();
if (openAiMtlsValidator is not null)
{
    app.Lifetime.ApplicationStopped.Register(openAiMtlsValidator.Dispose);
}

// RFC 9728 resource-server discovery for OAuth-capable ChatGPT connections.
// Deliberately unavailable until the website authorisation server is ready.
if (requireUserOAuth)
{
    var oauthMetadata = new
    {
        resource,
        authorization_servers = new[] { issuer! },
        scopes_supported = new[] { requiredScope },
        bearer_methods_supported = new[] { "header" }
    };
    app.MapGet("/.well-known/oauth-protected-resource", () => Results.Json(oauthMetadata));
    app.MapGet("/.well-known/oauth-protected-resource/mcp", () => Results.Json(oauthMetadata));
}

app.Use(async (context, next) =>
{
    if (!context.Request.Path.StartsWithSegments("/mcp"))
    {
        await next();
        return;
    }

    // Verify the actual TLS peer certificate, not an untrusted HTTP header.
    if (requireOpenAiMtls)
    {
        var clientCertificate = await context.Connection.GetClientCertificateAsync(context.RequestAborted);
        if (!openAiMtlsValidator!.IsValid(clientCertificate))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }
    }

    if (requireUserOAuth)
    {
        var result = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
        var principal = result.Principal;
        if (!result.Succeeded || principal is null ||
            principal.FindFirst("sub") is null ||
            string.IsNullOrWhiteSpace(principal.FindFirst("membership_account")?.Value) ||
            !principal.FindAll("scope").Concat(principal.FindAll("scp"))
                .SelectMany(claim => claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .Contains(requiredScope, StringComparer.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.Headers.WWWAuthenticate =
                $"Bearer resource_metadata=\"{metadataUrl}\", scope=\"{requiredScope}\"";
            return;
        }

        context.User = principal;
        await next();
        return;
    }

    // Existing, tested single-operator access for staging ONLY.
    var authorization = context.Request.Headers.Authorization.ToString();
    const string prefix = "Bearer ";
    var supplied = authorization.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
        ? authorization[prefix.Length..].Trim()
        : string.Empty;

    var suppliedBytes = Encoding.UTF8.GetBytes(supplied);
    var expectedBytes = Encoding.UTF8.GetBytes(mcpBearerToken);
    if (suppliedBytes.Length == 0 ||
        !CryptographicOperations.FixedTimeEquals(suppliedBytes, expectedBytes))
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.Headers.WWWAuthenticate = "Bearer";
        return;
    }

    await next();
});

app.MapGet("/healthz", () => Results.Ok(new { status = "ready" }));
app.MapMcp("/mcp");
app.Run();
