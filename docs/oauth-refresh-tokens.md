# Renewable ChatGPT OAuth connections

The website owns token issuance. The MCP host continues to validate the website's signed access tokens and forward requests through the existing membership-checked API.

## Lifetime and permissions

- Access tokens expire after 15 minutes.
- Refresh tokens expire 90 days after issuance. OpenIddict 6.4.0's default rolling refresh tokens and sliding expiration remain enabled: each successful renewal returns a replacement with a fresh 90-day lifetime. Its default 30-second reuse leeway accommodates concurrent requests; replay after that window is rejected by the library.
- Authorisation requires `nine-star-ki.read`, with optional `offline_access`. No `openid`, profile, password, implicit or extra API permissions are granted. Old read-scope-only connections still work, but receive no refresh token.
- Every code redemption and refresh resolves the token's validated website user ID and checks their existing active membership again. Renewal replaces the membership account claim with the current server-resolved value. It never accepts an account number or user ID from request arguments.
- Renewal retains the validated authorization ID, presenters and library token metadata. OpenIddict remains responsible for expiry, client binding, token storage, rotation and replay checks. Requests can retain or narrow the approved scopes, never expand them.
- The MCP host validates JWTs locally; an already issued access token can remain valid until expiry. Upstream API membership checks remain in place. This change does not add a website revocation UI or a public revocation endpoint.

## Existing client registration

The first-request registration check accepts the current exact registration unchanged. It upgrades the exact previous code-only registration by adding only the refresh-token grant permission. Client type, explicit consent, exact callback, PKCE requirement and all other permissions must already match. Unrelated mismatches still fail closed. The upgrade populates the existing descriptor before changing permissions, preserving stored names, properties and settings. No schema migration, package change or manual SQL is needed.

## ChatGPT connection configuration

The App owns the installed plugin's OAuth connection settings; editing its bundled instructions does not change those settings. After deploying the website to integration, configure the development connection to request both scopes:

```text
nine-star-ki.read offline_access
```

Where source configuration exposes `baseScopes` and `defaultScopes`, include both scopes in each array. Keep the existing public client ID, exact callback, PKCE, token authentication method `none`, MCP URL and integration authority unchanged. `oidcEnabled` remains false: offline access does not require an identity token here. The submitted package's current OAuth declarations also need these scope additions in a future package update; do not overwrite an in-review submission or change its authority as part of a server deployment.

Reconnect once after the scope change. A previously issued 15-minute access token cannot be retroactively given a refresh token. Confirm the outgoing authorization request includes `offline_access`; merely seeing it in discovery does not prove ChatGPT requested it.

## Validation and rollout

Source-level tests cover optional offline access, rejection of unsupported scopes, grant identity/metadata preservation, membership claim replacement, scope narrowing and rejection of expansion, the exact legacy client upgrade, no write for the current registration, and rejection of unrelated registration changes.

Run the existing Windows/.NET Framework solution build and tests before deployment. In integration, connect with both scopes, use a reading, and repeat after the original access token's 15-minute lifetime has passed without reconnecting. Verify a refresh occurred successfully without logging tokens. A provider-level test must also confirm expired/revoked tokens, wrong client, replay outside the library's reuse window and inactive membership cannot renew. Unit tests alone do not verify the hosted exchange or ChatGPT's credential storage.

Existing certificate/private-key access and OAuth enablement settings remain required. Do not change the MCP host's integration authority during the submitted plugin review. Production deployment and production end-to-end validation are separate from committing this source.
