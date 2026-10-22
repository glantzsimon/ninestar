using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NineStarKi.Mcp;
using Xunit;

namespace NineStarKi.Mcp.Tests;

public sealed class McpMemberAccountResolverTests
{
    [Theory]
    [InlineData(nameof(AstrologyTools.GetPersonalChart))]
    [InlineData(nameof(AstrologyTools.GetPredictions))]
    [InlineData(nameof(AstrologyTools.GetCompatibility))]
    [InlineData(nameof(AstrologyTools.GetKnowledgeBase))]
    public void PublicToolsDoNotExposeAccountNumber(string toolName)
    {
        var method = typeof(AstrologyTools).GetMethod(toolName);
        Assert.NotNull(method);
        Assert.DoesNotContain(method!.GetParameters(),
            parameter => parameter.Name == "accountNumber");
    }

    [Fact]
    public void AuthenticatedMemberCannotOverrideAccountNumber()
    {
        var resolver = CreateResolver(oauthEnabled: true, memberAccount: "123456789");
        Assert.Equal("123456789", resolver.Resolve("987654321"));
    }

    [Fact]
    public void AuthenticatedMemberDoesNotNeedToSupplyAccountNumber()
    {
        var resolver = CreateResolver(oauthEnabled: true, memberAccount: "123456789");
        Assert.Equal("123456789", resolver.Resolve(null));
    }

    [Fact]
    public void MissingMembershipClaimIsRejectedInOAuthMode()
    {
        var resolver = CreateResolver(oauthEnabled: true, memberAccount: null);
        Assert.Throws<UnauthorizedAccessException>(() => resolver.Resolve("987654321"));
    }

    [Fact]
    public void AnonymousUserCannotUseAnAccountNumberInOAuthMode()
    {
        var resolver = CreateResolver(oauthEnabled: true, memberAccount: "123456789", authenticated: false);
        Assert.Throws<UnauthorizedAccessException>(() => resolver.Resolve("987654321"));
    }

    [Fact]
    public void StagingModeUsesExistingAccountNumber()
    {
        var resolver = CreateResolver(oauthEnabled: false, memberAccount: null);
        Assert.Equal("987654321", resolver.Resolve("987654321"));
    }

    [Fact]
    public void StagingModeStillRequiresNineDigitNumber()
    {
        var resolver = CreateResolver(oauthEnabled: false, memberAccount: null);
        Assert.Throws<ArgumentException>(() => resolver.Resolve("123"));
    }

    private static McpMemberAccountResolver CreateResolver(
        bool oauthEnabled, string? memberAccount, bool authenticated = true)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Mcp:RequireUserOAuth"] = oauthEnabled.ToString()
            })
            .Build();

        var claims = new List<Claim>
        {
            new("sub", "website-user-42")
        };
        if (memberAccount is not null)
        {
            claims.Add(new Claim("membership_account", memberAccount));
        }

        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(claims, authenticated ? "verified-oauth" : null));

        return new McpMemberAccountResolver(configuration,
            new HttpContextAccessor
            {
                HttpContext = new DefaultHttpContext { User = principal }
            });
    }
}
