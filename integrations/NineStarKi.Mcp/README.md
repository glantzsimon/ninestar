# 9 Star Ki MCP connector (development branch only)

A separate ASP.NET Core 10 Streamable HTTP MCP bridge to the existing 9starkiastrology.com endpoints. **Does not alter the site's existing controllers, business logic, credentials, memberships, or Custom GPT.**

## Status

Source scaffold only. Not deployed and not yet connected to the private 9 Star Ki ChatGPT plugin. The GitHub workflow builds a downloadable Windows-ready publish artifact on demand. Verify the build before attempting deployment.

## Requirements

- .NET 10 ASP.NET Core Hosting Bundle on IIS for live deployment, or a separate reverse-proxy arrangement.
- HTTPS endpoint under a **separate IIS application or subdomain**. Do not use or replace the existing site's application pool.
- Two separate secrets supplied as process environment variables / secure server settings:
  - `NineStarKi__ApiKey`: existing MVC API key, used only on the server to call the source API.
  - `Mcp__BearerToken`: new long random secret for private MCP access.
- Optional `NineStarKi__BaseUrl` (defaults to `https://9starkiastrology.com/`).
- All secrets must remain outside source control and outside chat messages.

## Behaviour

The five MCP tools map to the existing site actions: chart, predictions, compatibility, knowledge base and I Ching. Each request has independent inputs; account numbers are NOT stored. HTTP redirects are disabled; client timeout 40 seconds. Existing MVC API may return `success:false` plus a logical `statusCode` despite HTTP 200, so errors are not inferred from HTTP status alone.

## Important limitations

- **Private, single-operator staging only.** A static bearer token is not a suitable publicly available member authentication system. For distribution, integrate OAuth/user binding and enforce the account membership on the website.
- Legacy API places account numbers and birth details in URL paths, potentially appearing in upstream IIS logs. Review privacy and logging before public deployment.
- The website presently expects birth time and location in the URL. Do not invent unknown values. Decide separately how optional details will be represented before enabling those flows.
- Do not publish, deploy, attach the plugin, or add real keys until a reviewed build and host isolation check.
- Do not send credentials through GitHub Actions inputs, workflow logs, or a chat.
