# 9 Star Ki MCP connector

Separate .NET 10 Streamable HTTP MCP bridge to the existing website at `9starkiastrology.com`. The connector has five tools; the original website and legacy Custom GPT are not modified by this connector.

## Verified private deployment (16 October 2026)

- The **existing deployed build** at `https://mcp.9starkiastrology.com/mcp` responds with `{"status":"ready"}` at `/healthz`.
- Without authorisation, `/mcp` returns HTTP 401.
- An authenticated `tools/list` call exposed `get_personal_chart`, `get_predictions`, `get_compatibility`, `get_knowledge_base` and `get_i_ching`.
- An authenticated `get_i_ching` call successfully fetched a real hexagram from the upstream API.
- GitHub Actions builds the MCP host; new OAuth and mTLS code is **staged in this development branch** but **not deployed**.

## Architecture and configuration

The intended **public** user journey is: client installs plugin in ChatGPT, clicks Connect, signs into the **existing** 9 Star Ki website in a secure browser window, and ChatGPT stores a delegated OAuth connection. No Auth0, second account, password in chat or copied membership number is required.

The two independent MCP security gates are feature flags:

| IIS app-pool environment variable | Effect |
|---|---|
| `NineStarKi__ApiKey` | Required upstream MVC API secret (never exposed to plugin users) |
| `Mcp__BearerToken` | Required private diagnostic Bearer token for the current staging mode (never embedded into plugin) |
| `Mcp__RequireOpenAiMtls` | Optional boolean, defaults false. If true, verifies OpenAI's real TLS client certificate |
| `Mcp__OpenAiRootCaPath` | Required when mTLS enabled; PEM downloaded from official OpenAI docs |
| `Mcp__OpenAiIntermediateCaPath` | Required when mTLS enabled; Connectors CA PEM downloaded from official OpenAI docs |
| `Mcp__RequireUserOAuth` | Optional boolean, defaults false. If true, requires a valid website-issued OAuth token for **every MCP request** |
| `Mcp__OAuthAuthority` | Required when OAuth enabled; HTTPS issuer URL for the website's OAuth server, to be built |
| `NineStarKi__BaseUrl` | Optional; default `https://9starkiastrology.com/` |

**Do not enable either flag yet.** The legacy private Bearer-token path remains functional when both are false. With both flags true, mTLS checks the calling ChatGPT service AND OAuth checks the individual website member; **mTLS never bypasses OAuth**. A real website OAuth issuer and its signing/JWKS endpoints are required before setting `Mcp__RequireUserOAuth=true`.

`McpMemberAccountResolver` derives the upstream membership account number from the validated issuer-signed `membership_account` token claim in OAuth mode. Any user-supplied account number is ignored, preventing a user from borrowing another member's privileges. In private staging only, the explicit account number parameter is still permitted. The original website API independently validates current subscription status on each paid tool call.

`get_i_ching` requires **no website membership**, although this first OAuth-resource implementation currently requires an OAuth connection to call the MCP server at all. Per-tool anonymous I Ching access is a separate future enhancement, not implemented yet.

## Implementing the existing-website OAuth issuer

Read [OAUTH-ROLLOUT.md](OAUTH-ROLLOUT.md) for the implementation plan, integration contracts and required security tests.

OpenIddict 7.7.2 supports the existing ASP.NET MVC 5/OWIN and EF6 stack, so the issuer can reuse the website's existing login. **All Code First migrations must be generated in `webapp/DataAccess/Migrations` using the existing `LocalDb` model and migration history.** An earlier isolated OAuth persistence project was removed because it violated this rule; its metadata must not be reused. The corrected migration has **not yet been generated**. The website OAuth issuer (PKCE, registered redirect URIs, signed tokens and refresh/consent storage) is also not implemented.

**Do not deploy the new MCP artifact or switch the plugin to OAuth** until the issuer, IIS mTLS settings, and browser login flow have been implemented and tested. The plugin configuration still targets the private prototype, so connection through ChatGPT itself is not yet working.

## Build and tests

Workflow `.github/workflows/9-star-ki-mcp.yml` restores/builds the .NET 10 MCP project, runs the test project `integrations/NineStarKi.Mcp.Tests` and produces an isolated Windows-ready `nine-star-ki-mcp` artifact.

Tests specifically verify that a verified member's token-selected account number cannot be overridden by an account number supplied through chat, and that missing/anonymous membership claims are rejected. Build and test conclusions must be inspected before deployment; a committed test is not proof that it passed.

## Important boundaries

- The upstream legacy API sometimes returns `success:false` JSON with HTTP 200 and internal statusCode 401, 404, or 422.
- Some existing website routes include personal details in path segments; review logging before broad deployment.
- Birth time and location remain required by current tool signatures; never fabricate unknown values.
- Review website redirect handling for OAuth callbacks. Never trust arbitrary `returnUrl` values.
- Do not port the legacy Custom GPT's prompt-based administrator shortcut.
- Secrets belong in server configuration, not in the plugin manifest or repository.
- Public rollout requires validated website OAuth, certificate handling, member isolation tests and OpenAI review.
