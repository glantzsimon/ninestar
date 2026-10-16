# ChatGPT customer sign-in: implementation and safe rollout

**Status:** development only. Resource-server OAuth verification and OpenAI mTLS validation are staged in `feature/9-star-ki-mcp-connector`. Neither mode is enabled on the currently working MCP server. The **authorisation server does not yet exist** and ChatGPT account linking has NOT been tested.

## Confirmed existing capabilities

- Existing website login: `webapp/WebApplication/Controllers/AccountController.cs`, `AccountController.Login`, `WebSecurity.IsAuthenticated`, `My.AccountService.Login(...)`. Users can log in with username or email. Existing website authentication remains unchanged.
- Member entitlement: `ApiController.Validate` checks the upstream `Authorization: ApiKey <key>`, looks up a membership from its account number, and checks `IsActive` / `MembershipOption.IsUnlimited` or administrator privilege. It **does not check ownership of the account number**.
- Account number: `UserMembership.AccountNumber` is derived from `Name.ToSixDigitCode()`; despite that method name, it consists of the first **nine** digits of the stored membership name. It is **not** a login secret.
- MCP tool discovery and `get_i_ching` were authenticated and tested successfully using the private operator Bearer token on the running server.
- ChatGPT presents an OpenAI-managed TLS client certificate for mTLS, but this authenticates ChatGPT as the service, not the human user.
- ChatGPT supports OAuth 2.1 authorisation-code + PKCE, protected-resource metadata, predefined clients, DCR and CIMD. It does **not** accept usernames/passwords as plugin arguments.

## Target architecture

```text
Customer in ChatGPT
    |
    | OpenAI-managed client mTLS certificate
    v
MCP site mcp.9starkiastrology.com
    |   * Check real OpenAI certificate when mTLS is enabled
    |   * Check website-issued OAuth access token
    |   * Read verified membership_account claim; ignore any user-supplied account number
    |   * Forward legacy upstream calls with server-only API key
    v
9starkiastrology.com (existing MVC / SQL Server)
    |   * OAuth authorisation server backed by existing website login
    |   * Resolve membership from authenticated website user ID
    |   * Issue short-lived signed access tokens
    |   * Enforce membership status on every existing legacy API request
    v
Existing chart / prediction / compatibility / I Ching functions
```

The OAuth authorisation server should be implemented using a security-reviewed OAuth/OIDC library, **not handwritten token/code exchange logic**. OpenIddict 7.7.2 (released 10 October 2026) is a viable candidate: it supports ASP.NET MVC 5/OWIN on .NET Framework 4.7.2 and Entity Framework 6 stores, allowing us to reuse the site's current SQL Server and login cookie without a separate Auth0 account or an authentication bridge. Official docs: https://documentation.openiddict.com/integrations/entity-framework and https://documentation.openiddict.com/integrations/aspnet-core ; MVC/OWIN sample: https://github.com/openiddict/openiddict-samples/tree/dev/samples/Mortis .

## Authorisation server requirements (to implement on website)

1. Add isolated OpenIddict OWIN services and an EF6 context + schema for applications, codes, authorisations and tokens. Keep migration scoped, backed up and reversible. Do not replace existing Forms/WebSecurity login.
2. Publish OAuth authorisation server metadata under the public HTTPS issuer, typically `https://9starkiastrology.com/.well-known/oauth-authorization-server`, with real `authorization_endpoint`, `token_endpoint`, PKCE `S256`, scopes, supported client authentication, and supported client registration mechanism.
3. Use current website login in the OAuth authorisation action. Require user consent and anti-forgery protection. Never accept a free-form external return URL; exact allowlisting of ChatGPT's registered callback is mandatory. Check existing `AccountController.Login` return-url handling before using it in an OAuth redirect sequence.
4. Register ChatGPT using a **documented** supported mode (CIMD with validated client metadata, or a predefined public client with a pinned redirect URI and PKCE; avoid unauthenticated open DCR). Verify RFC 9207 issuer identification before using ChatGPT's stable callback URL. Do not claim CIMD support in discovery until actually implemented.
5. Emit an access token signed by a persistent, securely stored signing key (JWKS published for verification) with `iss=https://9starkiastrology.com`, `aud=https://mcp.9starkiastrology.com/mcp`, `sub=<stable internal website user ID>`, `scope=nine-star-ki.read`, and `membership_account=<account number server-resolved from this signed-in user's active membership>`. Never take `membership_account` from browser query, chat or caller input.
6. Enforce short token expiry, PKCE code redemption, replay protection, refresh token rotation/revocation, privacy-safe logging and account disconnect. If the member has no eligible subscription, return a controlled membership response rather than falling back to a number they type.
7. Test two independent member accounts: one must **never** be able to borrow the other's membership number or modify the verified claim. Test invalid/expired/foreign JWTs, unsupported scopes, wrong audience, revoked subscriptions, login CSRF, arbitrary redirects, and code/refresh-token replay.
8. Add an authenticated read-only MCP profile tool after connection is working to identify accounts cleanly in the ChatGPT linking UI.

## MCP resource server implementation (development branch)

- `Mcp__RequireOpenAiMtls` defaults to false; when true, requests must present a valid OpenAI-managed client certificate. IIS must be configured to request client certificates and expose the true TLS peer certificate (not a forged header).
- `Mcp__RequireUserOAuth` defaults to false; when true, requires `Mcp__OAuthAuthority` (verified HTTPS issuer), validates JWT issuer, signature/JWKS, audience, expiry and `nine-star-ki.read` scope, plus presence of `sub` and `membership_account`.
- The two gates are independent and both can be enabled. **mTLS does not bypass OAuth**.
- `McpMemberAccountResolver` binds chart/prediction/compatibility/knowledge-base membership access to the validated OAuth `membership_account` claim and ignores chat-supplied numbers. The upstream API independently re-checks membership on each call.
- In legacy private staging (OAuth disabled), operator Bearer token plus explicit account number still works. Do **not** switch on OAuth before the issuer is implemented and verified.
- `GET /.well-known/oauth-protected-resource` and its `/mcp` variant are served only when OAuth mode is enabled. Failed OAuth authentication emits `WWW-Authenticate: Bearer resource_metadata="..."`.
- `get_i_ching` remains a public-membership tool, but the staged OAuth implementation currently requires an authorised connection to access the *server*. A future per-tool `noauth`/OAuth `securitySchemes` implementation can allow I Ching before linking; don't incorrectly claim that feature is already implemented.

## Completed development checks (16 October 2026)

- MCP build and publish completed successfully in GitHub Actions run #19.
- New account-identity tests **passed all six cases**, with zero failures.
- The website login action has received a **development-branch-only** redirect hardening change, replacing blind `Redirect(returnUrl)` with a local-return-URL check (`Url.IsLocalUrl`). **This website change has not been deployed or independently built**.
- The signed-token issuer and browser-based account connection **remain to be implemented**. Do not treat a passing MCP build as a completed login integration.

## Test and deployment gates

1. GitHub Actions must build both the MCP host and the account-isolation tests on the feature branch.
2. Implement and build a website OAuth issuer on the same **development branch**, with no production deployment until code review.
3. Run a staged browser-based sign-in with real website credentials on the website only; use PKCE test client and inspect issuer discovery/JWKS/claims. Never paste credentials/tokens into chat.
4. Configure the standalone MCP IIS site for mTLS and OAuth, verify with ChatGPT, and only then switch plugin `mcp.json` to OAuth authentication. Stage the site settings independently to avoid breaking production.
5. Test customer A/B entitlement isolation and real I Ching tool results end to end; confirm errors are reported accurately (upstream can return `success:false` with HTTP 200).
6. Prepare public Plugin submission only once signed-in member workflow, privacy policy, and authorisation are actually verified.

No user action is needed to write the isolated development code. Server/Plesk environment changes, certificates, DB schema deployment and enabling production OAuth will require the owner at a later checkpoint.
