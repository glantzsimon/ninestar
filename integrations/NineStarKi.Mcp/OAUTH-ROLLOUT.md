# ChatGPT customer sign-in: implementation and safe rollout

**Status:** development only. Resource-server OAuth verification and OpenAI mTLS validation are staged in `feature/9-star-ki-mcp-connector`. Neither mode is enabled on the currently working MCP server. The website's OAuth implementation is now **partially staged behind a disabled compilation flag**. It is not deployed or enabled, the DataAccess migration and ChatGPT client record do not yet exist, and account linking has NOT been tested.

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

The OAuth authorisation server should be implemented using a security-reviewed OAuth/OIDC library, **not handwritten token/code exchange logic**. OpenIddict 7.7.2 supports ASP.NET MVC 5/OWIN on .NET Framework 4.7.2 and Entity Framework 6. Its EF6 entities must be mapped into the **existing DataAccess LocalDb context**, not a second context or project. EF6 6.5.2 (required by the recommended OpenIddict 7.7.2) and OWIN 4.2.3 must be tested as patch upgrades across the legacy solution before enabling the library. Existing login cookies/credentials and SQL Server remain authoritative. Official docs: https://documentation.openiddict.com/integrations/entity-framework and https://documentation.openiddict.com/integrations/aspnet-core ; MVC/OWIN sample: https://github.com/openiddict/openiddict-samples/tree/dev/samples/Mortis .

## Corrected database migration ownership

The first attempt put an OAuth EF6 migration into a **separate** `integrations/NineStarKi.OAuth.Persistence` project, which violated the established application structure. That project, its separate migration configuration, and its isolated-context migration/snapshot **have been removed from this feature branch**. Those files were never deployed and must not be copied or applied. Their snapshot cannot be treated as a valid continuation of DataAccess's existing EF6 migration history.

**Single authority:** `webapp/DataAccess/Database/Db.cs` (`LocalDb`) and `webapp/DataAccess/Migrations/` using the existing `DatabaseInitialiserLocal` configuration. The site already calls `DbMigrator(new DatabaseInitialiserLocal()).Update()` on startup. Consequently do not commit an unfinished OAuth migration into DataAccess: a future integration deployment would auto-apply it. Generate only after model integration and complete metadata verification; then test on integration with a database backup.

The corrected migration in DataAccess **has not yet been generated**, and no database schema has been changed. This is intentional to avoid unsafe, hand-edited EF6 snapshots. `LocalDb` is now declared `partial` and `webapp/DataAccess/Database/Db.OAuth.cs` contains the official `modelBuilder.UseOpenIddict()` model mapping behind the **disabled-by-default `NINESTAR_OAUTH` compilation symbol**. It does **nothing** in the current build. Once the NuGet dependencies have been safely integrated, enable this symbol and generate the first migration via the **existing** EF6 `Add-Migration` tooling (DataAccess as migration project, website as startup project). Do not run `Update-Database` against production.

`webapp/WebApplication/Services/OAuthAccountIdentityService.cs` is also committed and registered with Autofac: it resolves the current authenticated `WebSecurity.CurrentUserId` and its **existing** active membership via the side-effect-free `GetExistingActiveMembershipForOAuth`. It does not expose an endpoint or issue tokens; the login behaviour is unchanged.

**NuGet dependencies installed and pushed (17 October 2026):** The website's existing NuGet workflow updated `EntityFramework` to `6.5.2` across the solution, upgraded the OWIN packages to `4.2.3`, and installed `OpenIddict.EntityFramework`/`OpenIddict.Server.Owin` `7.7.2` in their respective existing projects. The owner confirmed a successful **local full solution build before the latest source additions**. Do not mistake that earlier build for verification of subsequent OAuth code. `packages.config`, Web.config and some other project configuration files are git-crypt protected; don't overwrite the encrypted GitHub blobs with plaintext.

**Token-claims foundation added (17 October 2026):** `webapp/WebApplication/Services/OAuthClaimsPrincipalFactory.cs` is compiled in `WebApplication.csproj` and has five new xUnit tests in `WebApplication.Tests/Unit/Services/OAuthClaimsPrincipalFactoryTests.cs`. It creates an OpenIddict principal from the authenticated-user-only `OAuthAccountIdentity`, restricts scope to `nine-star-ki.read`, sets the MCP resource to `https://mcp.9starkiastrology.com/mcp`, and marks `sub`/`membership_account` for the access token only. This does **not** expose OAuth routes, create signing keys or issue tokens. The new source/tests have not yet been verified by a local website build.

**Gated OWIN server groundwork (17 October 2026):** After the NuGet push, `webapp/WebApplication/App_Start/OAuthServerConfiguration.cs` was added using the official OpenIddict EF6/OWIN and existing Autofac-container pattern. It configures an issuer, code+PKCE-only flow, exact `nine-star-ki.read` scope, short-lived signed and non-encrypted MCP JWT access tokens, and requires two persistent X.509 certificates from the local-machine store (appSettings `NineStarKiOAuthSigningCertificateThumbprint` and `NineStarKiOAuthEncryptionCertificateThumbprint`). `Startup.cs` invokes the setup only under the **disabled-by-default `NINESTAR_OAUTH` compilation symbol**; `LocalDb` is also registered `AsSelf()` so the OpenIddict EF6 stores can use the same app DbContext. It DOES NOT compile the gated code, enable routes or apply any migration. Do not turn on the symbol until ALL security/migration requirements are implemented and tested.

**Package verification completed (17 October 2026):** The owner installed and pushed `Autofac.Owin 7.1.0`, and the website now references `Autofac 6.5.0`, `Autofac.Extensions.DependencyInjection 8.0.0`, OpenIddict `7.7.2`, EF6 `6.5.2`, and OWIN `4.2.3`. Both MVC and DataAccess reference Autofac 6.5.0. The normal website solution build was confirmed prior to the changes below; **the later OAuth sources and their gated code have not been locally compiled or executed**.

**Gated authorisation / token endpoints (17 October 2026):** `WebApplication/Controllers/OAuthController.cs` adds `/oauth/authorize` GET/POST and `/oauth/token` POST under `#if NINESTAR_OAUTH`. Consent is explicit with anti-forgery protection in `Views/OAuth/Consent.cshtml`, using existing Globalisation Dictionary labels. Login continues through `AccountController.Login` with a same-site return URL. Both authorisation and token redemption verify the user's **existing** active membership; token redemption resolves the user from OpenIddict's validated authorisation-code ticket rather than trusting a client-supplied membership number. The flow requires S256 PKCE and exactly `nine-star-ki.read`; only code grants are accepted. Token claims come from the existing `OAuthClaimsPrincipalFactory`. The view model is intentionally available in normal builds so Razor compilation does not fail while OAuth is disabled. The endpoints have NOT been tested in a flag-enabled build.

**ChatGPT client registration (17 October 2026):** `WebApplication/Services/OAuthClaimsPrincipalFactory.cs` also contains `OAuthChatGptClientRegistration` (compiled by the existing legacy .csproj entry). It creates a public, explicit-consent, PKCE-only OpenIddict application with exactly one HTTPS ChatGPT redirect URI: either `https://chatgpt.com/connector_platform_oauth_redirect` or `https://chatgpt.com/connector/oauth/{callback-id}`, obtained from the actual ChatGPT MCP connection settings. There is **no guessed redirect URL, wildcard, client secret, anonymous DCR, refresh token, or password grant**. Existing client records are checked against exact security settings and are not silently rewritten. Five groups of xUnit tests are in `WebApplication.Tests/Unit/Services/OAuthChatGptClientRegistrationTests.cs`; the test project now directly references the already-installed OpenIddict.Abstractions assembly. No registration is run automatically or persisted before migration. The client identifier and exact callback still need to be supplied from the user's actual connector setup.

**Still outstanding:** Build and test the latest ordinary solution (with the feature flag OFF), compile the opt-in OAuth paths in a controlled integration environment, review the OpenIddict request handling and the consent UI, obtain the exact callback/client ID and persist the approved ChatGPT registration **after** migration, provision persistent X.509 signing/encryption certificates, and generate/review the **complete** DataAccess Code First OAuth migration and metadata using the existing EF6 `DatabaseInitialiserLocal`. Test all code and database changes with a backed-up integration database, verify issuer discovery/JWKS, login, PKCE, consent, token redemption and separate accounts before any deployment. Never use development signing certificates or deploy incomplete auth endpoints.

**Database migration safety:** `App_Start/DataConfig.cs` executes DataAccess migrations at startup. Do not enable the OAuth mapping on a deployed site until the reviewed DataAccess migration is committed and an integration rollout plan exists; do not generate EF6 snapshots against an isolated context. No OAuth schema change has been applied.

## Authorisation server requirements (to implement on website)

1. Add OpenIddict OWIN services to the existing MVC website while preserving Forms/WebSecurity login. **All OAuth EF6 entities and Code First schema changes belong in `webapp/DataAccess`, using the existing `LocalDb` model and `DatabaseInitialiserLocal` migration history.** Update the existing model's `OnModelCreating` with OpenIddict EF6 mappings; generate a NEW migration with the existing DataAccess migration configuration so its metadata `Target` represents the complete current site schema PLUS the OAuth entities. Review the generated `.cs`, `.Designer.cs`, and `.resx`, and commit them in `webapp/DataAccess/Migrations`. Do not reuse the model snapshot from a different EF6 context. Back up and test the migration on integration before deployment.
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
