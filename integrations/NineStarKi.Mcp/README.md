# 9 Star Ki MCP connector (isolated development branch)

ASP.NET Core 10 Streamable HTTP MCP bridge for 9starkiastrology.com. The five tools call the existing website API. **This project does not change the website, membership rules, or original Custom GPT.**

## Verified baseline (16 October 2026)

- Windows IIS + Plesk HTTPS deployment: `https://mcp.9starkiastrology.com/mcp`.
- GET `/healthz` returns `{"status":"ready"}`.
- Unauthenticated MCP requests return HTTP 401.
- Authenticated `tools/list` exposed five tools: `get_knowledge_base`, `get_personal_chart`, `get_predictions`, `get_compatibility`, `get_i_ching`.
- Authenticated `tools/call` with `get_i_ching` returned `success:true` and real hexagram data from the upstream API.
- The original deployed build does not yet implement OAuth. The OAuth changes in this branch are **not deployed or tested yet**.

## Configuration on the MCP application's IIS app pool

All credentials are set as per-application-pool environment variables, not in source control or chat.

| Variable | Purpose |
| --- | --- |
| `NineStarKi__ApiKey` | Existing upstream MVC API credential (required). |
| `Mcp__BearerToken` | Independent strong operator-only Bearer token for diagnostics (required). |
| `NineStarKi__BaseUrl` | Optional; defaults to `https://9starkiastrology.com/`. |
| `Mcp__PublicBaseUrl` | Optional HTTPS origin, defaults to `https://mcp.9starkiastrology.com`. |
| `Mcp__OAuthAuthority` | OAuth issuer / OIDC discovery authority from a trusted identity provider; **optional until configured**. |
| `Mcp__OAuthAudience` | Audience of JWT access tokens issued to the MCP API. Required when OAuth authority is set. Recommended value: `https://mcp.9starkiastrology.com/mcp`. |
| `Mcp__OAuthScope` | Required delegated access scope, defaults to `read:9starki`. |
| `Mcp__OAuthSubject` | Stable OAuth user `sub` for the **single allowed operator**. Required when OAuth is enabled. |

**Leave `Mcp__OAuthAuthority` unset until the identity provider is ready.** Without it, this version behaves like the already-tested private connector. Never put provider client secrets or user passwords in this repository or ChatGPT messages.

## OAuth migration plan

Use a standards-compliant third-party OAuth 2.1 provider such as Auth0, not a homemade authorization server. Configure an authorization-code + PKCE flow, an authorized ChatGPT CIMD client (or supported DCR), and an RS256 JWT for the MCP API audience. Enable a scope such as `read:9starki` only for the allowed operator account.

1. Create an OAuth API at your identity provider with identifier `https://mcp.9starkiastrology.com/mcp`.
2. Give the private operator identity permission to obtain `read:9starki`; limit which users can request access to the API.
3. Register ChatGPT via CIMD (preferred) with `https://chatgpt.com/oauth/client.json`, subject to provider support. Its callback is currently `https://chatgpt.com/connector_platform_oauth_redirect`. Check the actual link dialog for the connection's callback before finalising provider settings.
4. Configure `Mcp__OAuthAuthority`, `Mcp__OAuthAudience`, and `Mcp__OAuthSubject` (plus the optional scope override) on the MCP IIS app pool. Recycle **only the MCP pool**.
5. Verify GET `/.well-known/oauth-protected-resource` advertises the expected provider and resource, and unauthorized `/mcp` returns `WWW-Authenticate: Bearer resource_metadata="..."`.
6. Test an actual OAuth authorization/code exchange and a legitimate ChatGPT tool invocation before switching the private plugin manifest to `auth.type: "oauth"` and `client.mode: "cimd"`.
7. Keep upstream website access and membership checks separate. This proof-of-concept uses a **single operator** only; multi-member support needs binding OAuth identities to individual website accounts and enforcing entitlements on every tool call.

**Implementation in this branch:** OAuth-enabled mode uses Microsoft ASP.NET Core JWT Bearer token validation against the configured provider's signed JWKS and OIDC issuer metadata. It validates issuer, signature, audience, expiration, scope **and operator subject**. Existing `Mcp__BearerToken` access is preserved only for the operator's PowerShell diagnostics. There is no endpoint that creates or issues OAuth tokens.

## Local / hosted build

The separate GitHub Actions workflow `.github/workflows/9-star-ki-mcp.yml` builds and publishes the .NET 10 project to artifact `nine-star-ki-mcp`, without requiring an SDK on the Windows desktop. Do not merge this development branch into `master` or deploy the OAuth build until CI and the identity-provider setup are verified.

## Constraints and privacy

- The upstream MVC API may return HTTP 200 even when the response body says `success:false,statusCode:401/404/422`; callers must check JSON contents.
- The legacy website embeds account numbers and birth details in route paths; IIS upstream logs may record these. Do not expand exposure until logging/privacy are assessed.
- Birth time and location currently have required path parameters. Never fabricate missing values.
- The original GPT's old prompt-based administrative bypass must never be ported.
- Static Bearer-token access is staging/operator-only and is **not** suitable for public deployment.
- Always keep `healthz` free of account details or credentials.

## MCP PowerShell diagnostic notes

With `Mcp__BearerToken` already in your session's clipboard, authenticated MCP `tools/list` and `tools/call` can be invoked over Streamable HTTP. This SDK exposes lower-case `snake_case` tool names, e.g. `get_i_ching`, not `GetIChing`. For protocol version `2026-07-28`, include metadata `params._meta["io.modelcontextprotocol/protocolVersion"]` in requests, plus the `Mcp-Method` and `Mcp-Name` HTTP headers for tool calls.
