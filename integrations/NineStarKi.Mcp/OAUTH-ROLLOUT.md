# ChatGPT customer sign-in: implementation and safe rollout

## Renewable connections (23 October 2026)

The code on master now supports authorization-code and refresh-token grants. See
[OAuth refresh tokens](../../docs/oauth-refresh-tokens.md) for the current scope,
lifetime, registration-upgrade and validation contract. This supersedes the
code-only/no-refresh descriptions in the historical implementation notes below.
Access tokens remain valid for 15 minutes. A connection requesting
`nine-star-ki.read offline_access` can receive rotating refresh tokens with a
90-day lifetime renewed on use. Existing read-only connections remain compatible
but need a new authorization with `offline_access` before they can renew.
No schema migration or package upgrade is required. Deployment and a real ChatGPT
renewal test are still required; committing source does not update a connection's
App-owned OAuth settings or issue refresh tokens to an existing connection.

**Current local baseline (21 October 2026):** The owner confirmed that OpenIddict 6.4.0 with all OAuth source restored builds, website tests pass, and the local website starts. Controlled tests isolated the Visual Studio 2017 editor crash to OpenIddict.Abstractions 7.7.2; keep the package family aligned at 6.4.0. The existing OAuth migration has already been applied locally; the explicit 150-character token Type mapping preserves that migration's model. With local OAuth enabled, the owner verified discovery at `https://localhost/ninestar/.well-known/openid-configuration` and JWKS at `https://localhost/ninestar/.well-known/jwks`. This verifies metadata/public-key publication, not private-key signing, consent or token exchange. Integration is the agreed environment for end-to-end testing. Older dated notes below are historical and superseded by this baseline.

## Integration preparation (21 October 2026)

**Integration deployment requested:** The owner confirmed the latest branch is current locally, tests pass, the integration database backup is available, and Web.Integration.config still overrides NineStarKiOAuthEnabled to false. Deploy this feature branch through TeamCity Integration with OAuth disabled. Deployment success and integration certificate provisioning remain to be verified.

Work on `feature/9-star-ki-mcp-connector`; integration can deploy this branch without merging master. Normal preparation commits do not trigger TeamCity. Add its deployment marker only when the integration database backup and configuration are ready.

The client-registration helper is now wired through `OAuthServerConfiguration.RegisterMiddleware`. When OAuth is enabled and either client setting is supplied, both settings are validated. On the first request, after ASP.NET Application_Start and its existing DataAccess migrator, a single shared asynchronous task uses a disposable Autofac scope to register the client through OpenIddict. Existing mismatched registrations fail closed rather than being rewritten. Registration failures remain cached until the application is recycled. Leaving both settings absent/empty permits discovery testing without creating a client; OAuth disabled performs no client registration. No new migration or schema change is introduced by this wiring.

The settings to add in locally decrypted Web.config, then override through Web.Integration.config when the actual values are known, are:

```xml
<add key="NineStarKiOAuthChatGptClientId" value="" />
<add key="NineStarKiOAuthChatGptRedirectUri" value="" />
```

For the planned predefined public client, use an agreed public identifier (for example `ninestar-chatgpt-integration`) and the exact callback supplied by the OAuth-configured ChatGPT connection. These are identifiers, not secrets. Do not populate a guessed callback. This does not implement CIMD or dynamic client registration. The installed private plugin version 0.3.1 currently selects api_key/Bearer authentication, so its OAuth connection configuration remains a separate step. Do not switch the working plugin before integration endpoints and registration are ready.

Discovery no longer advertises the default `openid` scope: the implemented controller accepts exactly `nine-star-ki.read` and does not implement OIDC identity-token issuance. Verify the updated scope list after building.

### Deployment sequence

1. Build the latest branch locally and run the existing website tests. The registration wiring and discovery-scope change still need this Windows/.NET Framework validation.
2. Confirm the integration connection string targets the integration database. Take a database backup and retain the previously deployed website/configuration for rollback. Startup can apply the already committed AddOpenIddictOAuth migration even while OAuth is disabled; do not scaffold it again or enable automatic migrations.
3. In the encrypted integration transform, set `NineStarKiOAuthIssuer=https://int.9starkiastrology.com/` and `NineStarKiOAuthEnabled=false`. Ensure local `true` is overridden. Deploy the feature branch through the existing TeamCity Integration build.
4. Provision separate integration signing and encryption certificates in LocalMachine/My on the integration server. Grant the actual IIS application-pool identity Read access to their private keys. Override both certificate thumbprints in the integration transform; do not reuse local development thumbprints or certificates.
5. Enable integration OAuth and recycle the application. Verify `https://int.9starkiastrology.com/.well-known/openid-configuration` and `https://int.9starkiastrology.com/.well-known/jwks`.
6. Configure the OAuth test connection, obtain its exact callback, supply both client settings and recycle. First request creates/checks the registration. Verify login, explicit consent, S256 PKCE, rejection cases and token redemption. Configure a separate MCP test environment for the integration issuer/upstream/database; do not point integration tokens at the existing production MCP settings.
7. Merge and production rollout remain separate steps after integration validation. If rollback is needed after migration, restore the verified integration database backup and matching previous application/configuration together.

Configuration files are git-crypt protected and must be edited by the owner in the decrypted local checkout. Never overwrite encrypted repository blobs with plaintext.

## Local issuer setup

The website reads `NineStarKiOAuthIssuer` from its existing environment-specific Web.config appSettings when OAuth is enabled. It must be an absolute HTTPS URL ending in a slash, with no user information, query or fragment. The confirmed local application URL is `https://localhost/ninestar/`; production must use its own issuer setting. Authorization and token endpoint paths are relative, yielding `https://localhost/ninestar/oauth/authorize` and `https://localhost/ninestar/oauth/token` for local hosting.

Add the issuer to the locally decrypted Web.config and keep OAuth disabled while provisioning credentials:

```xml
<add key="NineStarKiOAuthIssuer" value="https://localhost/ninestar/" />
<add key="NineStarKiOAuthEnabled" value="false" />
```

Update an existing key rather than adding a duplicate. Web.config is git-crypt protected; edit it locally using the owner's normal workflow. The signing and encryption certificate thumbprint settings remain required before enabling OAuth. Keys must be available to the actual IIS/IIS Express process identity. Local discovery and JWKS have been verified by the owner. Private-key signing, configured client registration and browser authorization still require validation.

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

The corrected migration `webapp/DataAccess/Migrations/202610170149068_AddOpenIddictOAuth` was generated in the existing DataAccess migration history and committed with its `.Designer.cs` and `.resx`. Its `Up()` creates only `OpenIddictApplications`, `OpenIddictAuthorizations`, `OpenIddictTokens`, and `OpenIddictScopes`; `Down()` reverses those changes. We decoded the EF6 `Target` snapshots: the prior migration has 36 existing entity types, and the new snapshot retains all 36 plus four OpenIddict entity types. No database schema has yet been changed. `LocalDb` remains the only EF6 context; `Db.OAuth.cs` always includes `modelBuilder.UseOpenIddict()` to keep the model consistent with migration history in both Debug and Release. No separate OAuth migration history is permitted.

`webapp/WebApplication/Services/OAuthAccountIdentityService.cs` is also committed and registered with Autofac: it resolves the current authenticated `WebSecurity.CurrentUserId` and its **existing** active membership via the side-effect-free `GetExistingActiveMembershipForOAuth`. It does not expose an endpoint or issue tokens; the login behaviour is unchanged.

**NuGet dependencies installed and pushed (17 October 2026):** The website's existing NuGet workflow updated `EntityFramework` to `6.5.2` across the solution, upgraded the OWIN packages to `4.2.3`, and installed `OpenIddict.EntityFramework`/`OpenIddict.Server.Owin` `7.7.2` in their respective existing projects. The owner confirmed a successful **local full solution build before the latest source additions**. Do not mistake that earlier build for verification of subsequent OAuth code. `packages.config`, Web.config and some other project configuration files are git-crypt protected; don't overwrite the encrypted GitHub blobs with plaintext.

**Token-claims foundation added (17 October 2026):** `webapp/WebApplication/Services/OAuthClaimsPrincipalFactory.cs` is compiled in `WebApplication.csproj` and has five new xUnit tests in `WebApplication.Tests/Unit/Services/OAuthClaimsPrincipalFactoryTests.cs`. It creates an OpenIddict principal from the authenticated-user-only `OAuthAccountIdentity`, restricts scope to `nine-star-ki.read`, sets the MCP resource to `https://mcp.9starkiastrology.com/mcp`, and marks `sub`/`membership_account` for the access token only. This does **not** expose OAuth routes, create signing keys or issue tokens. The new source/tests have not yet been verified by a local website build.

**Gated OWIN server groundwork (17 October 2026):** After the NuGet push, `webapp/WebApplication/App_Start/OAuthServerConfiguration.cs` was added using the official OpenIddict EF6/OWIN and existing Autofac-container pattern. It configures an issuer, code+PKCE-only flow, exact `nine-star-ki.read` scope, short-lived signed and non-encrypted MCP JWT access tokens, and requires two persistent X.509 certificates from the local-machine store (appSettings `NineStarKiOAuthSigningCertificateThumbprint` and `NineStarKiOAuthEncryptionCertificateThumbprint`). `Startup.cs` compiles this setup in normal Debug builds but only invokes it when `NineStarKiOAuthEnabled=true`; `LocalDb` is also registered `AsSelf()` so the OpenIddict EF6 stores can use the same app DbContext. The running OAuth server remains disabled by default; no signing credentials or client registration have been configured. Do not turn on the symbol until ALL security/migration requirements are implemented and tested.

**Package verification completed (17 October 2026):** The owner installed and pushed `Autofac.Owin 7.1.0`, and the website now references `Autofac 6.5.0`, `Autofac.Extensions.DependencyInjection 8.0.0`, OpenIddict `7.7.2`, EF6 `6.5.2`, and OWIN `4.2.3`. Both MVC and DataAccess reference Autofac 6.5.0. The normal website solution build was confirmed prior to the changes below; **the later OAuth sources and their gated code have not been locally compiled or executed**.

**Gated authorisation / token endpoints (17 October 2026):** `WebApplication/Controllers/OAuthController.cs` adds `/oauth/authorize` GET/POST and `/oauth/token` POST under `#if NINESTAR_OAUTH`. Consent is explicit with anti-forgery protection in `Views/OAuth/Consent.cshtml`, using existing Globalisation Dictionary labels. Login continues through `AccountController.Login` with a same-site return URL. Both authorisation and token redemption verify the user's **existing** active membership; token redemption resolves the user from OpenIddict's validated authorisation-code ticket rather than trusting a client-supplied membership number. The flow requires S256 PKCE and exactly `nine-star-ki.read`; only code grants are accepted. Token claims come from the existing `OAuthClaimsPrincipalFactory`. The view model is intentionally available in normal builds so Razor compilation does not fail while OAuth is disabled. The endpoints have NOT been tested in a flag-enabled build.

**ChatGPT client registration (17 October 2026):** `WebApplication/Services/OAuthClaimsPrincipalFactory.cs` also contains `OAuthChatGptClientRegistration` (compiled by the existing legacy .csproj entry). It creates a public, explicit-consent, PKCE-only OpenIddict application with exactly one HTTPS ChatGPT redirect URI: either `https://chatgpt.com/connector_platform_oauth_redirect` or `https://chatgpt.com/connector/oauth/{callback-id}`, obtained from the actual ChatGPT MCP connection settings. There is **no guessed redirect URL, wildcard, client secret, anonymous DCR, refresh token, or password grant**. Existing client records are checked against exact security settings and are not silently rewritten. Four groups of xUnit tests are in `WebApplication.Tests/Unit/Services/OAuthChatGptClientRegistrationTests.cs`; the test project now directly references the already-installed OpenIddict.Abstractions assembly. No registration is run automatically or persisted before migration. The client identifier and exact callback still need to be supplied from the user's actual connector setup.

**Unit-test runtime repair and verified ordinary build (17 October 2026):** After aligning `WebApplication.Tests`'s `System.Text.Json 8.0.5` and compatible runtime references, the owner confirmed that **the full normal MVC solution builds and all tests pass locally**. This verifies the feature-flag-OFF build and tests, **not the compilation or execution of `#if NINESTAR_OAUTH` code**.

**Still outstanding:** Rebuild and test the latest Debug changes; apply the reviewed EF6 migration to a verified local/test database and validate its results; review the OpenIddict request handling and consent UI; obtain the exact callback/client ID and persist the approved ChatGPT registration **after** migration; provision persistent X.509 signing/encryption certificates and issuer metadata/JWKS. Test end-to-end sign-in, PKCE, consent, token redemption, membership changes and separate accounts on a backed-up integration database before any deployment.

**Simplified local Debug workflow (17 October 2026):** The normal `Debug|AnyCPU` configurations of both `webapp/DataAccess/DataAccessLayer.csproj` and `webapp/WebApplication/WebApplication.csproj` define `NINESTAR_OAUTH` (no special MSBuild command or compile-check flag). Non-Debug website code does not start the OAuth server, but `LocalDb` now includes the four OAuth entity mappings in every build configuration to match the generated migration. `Startup.cs` compiles the authorization server but activates it only when the existing site's environment-specific `appSettings["NineStarKiOAuthEnabled"]` is `true`; leaving that setting absent/false keeps MVC login, Autofac and Hangfire operating without requiring unprovisioned OAuth signing keys. This is a simple temporary runtime setup switch, not a permanent compilation switch. `DataConfig.InitialiseDatabase()` has its original EF6 startup migration code again. Don't enable the server until the schema, credentials and registration are ready.

**Debug compilation fixes (17 October 2026):** The first flag-on build revealed four C# extension-method errors. The OAuth MVC controller now imports `Owin`, the namespace where OpenIddict 7.7.2 defines `IOwinContext.GetOpenIddictServerRequest()`. The website's existing old-style `WebApplication.csproj` now directly references the already-downloaded `OpenIddict.Core.7.7.2` assembly for `.AddCore(...)` and `OpenIddict.EntityFramework.7.7.2` for `.UseEntityFramework(...)`. Both extension methods are declared in `Microsoft.Extensions.DependencyInjection`. Because the WebApplication `packages.config` is git-crypt protected, the owner should run `Install-Package OpenIddict.EntityFramework -Version 7.7.2 -ProjectName WebApplication` locally in Visual Studio so NuGet records the direct dependency and its OpenIddict.Core dependency in the website's own packages.config. The user should then rebuild Debug and push NuGet metadata using their `push` alias. No migration scaffold or DB update before the compile is clean.

**Next local action:** Pull the configurable-issuer change, add the local issuer appSetting with OAuth still disabled, and rebuild. Provision separate local signing/encryption certificates with private-key access for the website process, then register the approved test client and validate issuer discovery, consent and PKCE locally before integration rollout. Do not scaffold the existing OAuth migration again.

**Database migration safety:** `App_Start/DataConfig.cs` executes DataAccess migrations automatically at startup. The reviewed migration is committed on the feature branch and has been applied locally; integration and production rollout remain separate approvals. Once this branch is deployed, startup can apply that migration even while the OAuth server is disabled. Test on a verified local/test database first, then use an integration backup and deployment plan before merging/deploying. Do not deploy this branch to production yet.

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

