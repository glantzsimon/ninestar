# 9 Star Ki MCP connector (isolated development branch)

Separate .NET 10 Streamable HTTP MCP bridge to the existing 9starkiastrology.com API. This project does **not** change the website, its existing membership checks, or the original Custom GPT.

## Verified private prototype

- The MCP app runs at `https://mcp.9starkiastrology.com/mcp` (separate IIS site and app pool).
- `/healthz` returned `{"status":"ready"}`.
- Unauthenticated calls to `/mcp` returned 401.
- Authenticated `tools/list` exposed all five tools, using `snake_case` names:
  `get_personal_chart`, `get_predictions`, `get_compatibility`,
  `get_knowledge_base`, and `get_i_ching`.
- Authenticated `get_i_ching` call returned genuine data (`success:true`), confirming the entire MCP -> website API path.
- Built successfully on GitHub Actions for the original connector and after a provisional OAuth experiment. OAuth support has since been **removed from the development branch**, according to the owner's clarified architecture. The working IIS deployment is unchanged.

## Server-side configuration

Set the two secrets only in the MCP application's IIS pool environment variables:

- `NineStarKi__ApiKey` — upstream 9 Star Ki MVC website API key.
- `Mcp__BearerToken` — separate strong secret for private MCP diagnostic access.

Optional: `NineStarKi__BaseUrl`, default `https://9starkiastrology.com/`.

### Experimental OpenAI-managed mTLS support (not deployed)

The development branch now provides **opt-in** OpenAI-managed mTLS verification for the `/mcp` endpoint. It is **off by default**, preserving the working private Bearer-token test without additional settings.

- `Mcp__RequireOpenAiMtls=true` enforces an OpenAI client certificate instead of the private Bearer token.
- `Mcp__OpenAiRootCaPath`: absolute path to OpenAI's official public root CA PEM.
- `Mcp__OpenAiIntermediateCaPath`: absolute path to OpenAI's official Connectors mTLS intermediate CA PEM.

Download the published PEM files only from the links at https://developers.openai.com/plugins/build/auth#mutual-tls-mtls . The verifier checks leaf DNS SAN `mtls.prod.connectors.openai.com`, client-authentication EKU, chain validity, pinned root and exact Connectors intermediate, and online certificate revocation. Invalid or absent client certificates are rejected.

**IIS/Plesk prerequisite:** configure only the separate MCP HTTPS site to request/accept TLS client certificates, and check that `HttpContext.Connection.GetClientCertificateAsync` receives the real TLS peer certificate. Do not trust forwarded certificate headers supplied by arbitrary callers. Do not set `Mcp__RequireOpenAiMtls=true` until IIS is configured and an actual ChatGPT handshake can be tested. There is no impact on the existing production site unless the new artifact is deployed and the feature enabled.

mTLS authenticates ChatGPT as a **calling service**, not an individual customer. Do not expose paid or account-specific tools using only mTLS and a guessable nine-digit account number. OpenAI's public plugin guidelines prohibit collecting passwords, API keys, or OTP credentials in plugin tool arguments/chat. Customer authorisation needs a separate supported design.


Never commit secrets, disclose them in chat, or embed them in a plugin package.

## Product goal / public distribution

The owner's intended user flow is: users install 9 Star Ki Astrologer in ChatGPT, are prompted for their existing website **account number**, and the website checks membership entitlement. The owner would prefer **one service-level Bearer credential** between the ChatGPT plugin and the MCP server rather than OAuth or an external identity provider.

**Currently blocked by the ChatGPT public plugin portal:** although a portable MCP manifest may declare `auth.type: "api_key", headerScheme: "bearer"`, the public plugin submission/connection flow supports only no authentication, OAuth, or mixed noauth/OAuth. It has **no API-key or static Bearer-token input**. Public submission also rejects embedding nonempty secret request headers in `mcp.json`. See https://developers.openai.com/plugins/deploy/submission#authentication-limitations . Do **not** work around this by hardcoding a secret or misdeclaring the protected endpoint as unauthenticated.

A website account number is an **identifier**, not proof of account ownership. The legacy membership lookup must not be assumed sufficient to authorise access to birth details, compatibility information, or paid subscription features. Before public release choose a supported authentication design or obtain platform support for publisher-managed static credentials. If unauthenticated public MCP access is considered, safeguard personal/member resources with an independent ownership-verification mechanism and robust rate limits; do not remove the auth barrier without reviewing this risk. A public I Ching tool could be exposed separately as unauthenticated if its data access is genuinely public.

The developer may contact OpenAI to ask about submitting a publisher-managed API-key plugin. Otherwise, standard user OAuth can be implemented against the **existing 9 Star Ki website** without necessarily using Auth0.

## Build

The GitHub Actions workflow `.github/workflows/9-star-ki-mcp.yml` builds and publishes `nine-star-ki-mcp`. Keep this branch and draft PR #11 separate from `master`. Do not deploy changed authentication without a supported public solution and explicit testing.

## Upstream caveats

- The existing ASP.NET MVC API can return HTTP 200 with `success:false`, including logical status codes 401, 404, 422; callers must inspect the JSON body.
- Some endpoints require a nine-digit account number and birth time/location. Don't invent unknown values.
- The legacy API places some personal parameters in URL paths, which may be logged upstream.
- Do not carry over the old GPT's prompt-based administrative backdoor.
