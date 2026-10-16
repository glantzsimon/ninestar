using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using ModelContextProtocol.AspNetCore;
using ModelContextProtocol.Server;
using NineStarKi.Mcp;

var builder = WebApplication.CreateBuilder(args);

// Configuration comes from server environment variables, NEVER source control.
// NineStarKi__ApiKey: API key used by the existing MVC application's endpoints.
// Mcp__BearerToken: separate strong secret authorising access to this MCP server.
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

var requireOpenAiMtls = builder.Configuration.GetValue<bool>("Mcp:RequireOpenAiMtls");
OpenAiMtlsCertificateValidator? openAiMtlsValidator = null;
if (requireOpenAiMtls)
{
    var rootCaPath = builder.Configuration["Mcp:OpenAiRootCaPath"];
    var intermediateCaPath = builder.Configuration["Mcp:OpenAiIntermediateCaPath"];
    if (string.IsNullOrWhiteSpace(rootCaPath) ||
        string.IsNullOrWhiteSpace(intermediateCaPath))
    {
        throw new InvalidOperationException(
            "Mcp__RequireOpenAiMtls requires both published OpenAI CA certificate file paths.");
    }

    openAiMtlsValidator = new OpenAiMtlsCertificateValidator(rootCaPath, intermediateCaPath);
}

var app = builder.Build();
if (openAiMtlsValidator is not null)
{
    app.Lifetime.ApplicationStopped.Register(openAiMtlsValidator.Dispose);
}

// Private single-operator testing is protected with a server-side bearer secret.
// For public publication, ChatGPT's current plugin portal does not support
// uploading a shared publisher-managed static token. Do not embed credentials
// into a plugin manifest; choose a supported secure public-auth design first.
app.Use(async (context, next) =>
{
    if (!context.Request.Path.StartsWithSegments("/mcp"))
    {
        await next();
        return;
    }

    if (requireOpenAiMtls)
    {
        // IIS must request/accept TLS client certificates for this hostname.
        // mTLS verifies the ChatGPT CLIENT, not the individual site account.
        var clientCertificate = await context.Connection.GetClientCertificateAsync(context.RequestAborted);
        if (!openAiMtlsValidator!.IsValid(clientCertificate))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

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
    if (suppliedBytes.Length == 0 ||
        !CryptographicOperations.FixedTimeEquals(suppliedBytes, expectedBytes))
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.Headers.WWWAuthenticate = "Bearer";
        return;
    }

    await next();
});

// No account details, API response, credentials or membership state exposed.
app.MapGet("/healthz", () => Results.Ok(new { status = "ready" }));
app.MapMcp("/mcp");
app.Run();
