# Mobile artwork validation and app handoff

Current artwork refresh: `feature/ui-artwork-refresh` (draft PR #16). The original mobile transport is merged into master. Contract: `docs/mobile-api.md`, Public artwork catalogue. Existing web views, membership/auth, private API caching and image fields are unchanged. No migration or deployment is part of this change.

## Cache correction and validation without startup

The original action-only mocked cache tests missed the existing `Application_BeginRequest` default. `MvcApplication.SetCacheBehaviour` applied `NoCache` in DEBUG and `ServerAndPrivate` in Release before the action. ASP.NET only accepts a subsequent cacheability change if it is more restrictive, so the action could not restore Public.

`MobileArtworkCacheHelper.ApplyRequestDefault` now leaves only GET of the exact application-relative `~/api/mobile/v1/metadata/artwork` route (including casing/trailing slash accepted by MVC, with no PathInfo suffix) to the controller. Every other request keeps the existing default. It does not mark a request public itself. The action alone publishes successful 200/304 catalogue responses; unavailable media remains no-store and pipeline exceptions still use the mobile error handler's no-store policy. The controller's session-disabled behaviour is retained.

Run `MobileArtworkCacheHelperTests` and `MobileArtworkControllerTests` in the existing Windows test runner **without launching IIS or the website**. These construct cache policies/controller contexts in memory; they never call `Application_Start`, `DataConfig`, EF or migration code. Tests cover DEBUG/Release defaults, exact-path boundaries, private endpoints, 200/304, real `HttpCachePolicy.GetCacheability()`, max-age, ETag and no-store. They also reproduce the old restrictive-default behaviour. Do not use `HttpResponse.CacheControl` as evidence of the effective policy: its compatibility getter can return its default `private` string independently of `Response.Cache.SetCacheability`. Inspect `Response.Cache.GetCacheability()` in these tests and actual emitted HTTP headers for hosted validation.

Local IIS validation is a separate outstanding gate. `Application_Start` still calls `DataConfig.InitialiseDatabase()` and seeds users/roles/timezones. Nothing in this correction bypasses or changes that behaviour. Do not start/restart IIS or the normal website, or deploy, under the current no-schema-execution instruction. Perform build and in-memory tests first; hosted checks below remain deferred until startup is explicitly authorised for the intended database. Do not remove migration calls, enable automatic migrations or edit migration history to make this test run.

The app-side validation report at `ninestar-mobile/docs/artwork-backend-validation.md` remains owned by the mobile checkout. Bring its exact failing assertions into the local rerun; that Windows path is not available in the backend editing environment. This correction has source/project checks here, not a completed Windows or live-HTTP validation claim.

## Missing max-age follow-up

Integration subsequently returned public catalogue 200, valid ETag/empty 304, sample images with 30-day caching, and anonymous private 401/no-store responses. The owner relayed a successful VS2017 build and 609 passing tests. The remaining wire-level failure was a missing `max-age=300`; those results precede this follow-up.

The artwork action now explicitly calls `SetSlidingExpiration(true)` alongside `SetMaxAge(five minutes)` to request a constant per-response client lifetime, and `SetNoServerCaching()` to prevent origin/kernel caching from retaining an old media-base/ETag selection or affecting client expiration headers. Private endpoints, global route boundaries, ETags and image-host policies are unchanged. Regression assertions cover these settings on both fresh 200 and conditional 304 response policies. This is a source correction, not evidence that the missing Integration header is resolved.

Pull the follow-up on `feature/mobile-json-api`, build and rerun artwork tests without starting the website or migrations. After a separately authorised Integration deployment, require actual response headers on both 200 and 304 to contain `public` and `max-age=300`, with a matching ETag and empty 304 body. Recheck anonymous private endpoints remain 401/no-store. Retain the app's 15-second timeout. Do not change the catalogue lifetime to the image host's 30-day policy; they serve different purposes. Update the mobile validation report with the deployed revision and exact wire headers before clearing its gate. No deployment or schema execution is part of preparing this fix.

## Website Codex: validate locally first

1. In `D:\Workspace\ninestar`, confirm `git rev-parse --abbrev-ref HEAD` is `feature/mobile-json-api`, preserve local edits and `git pull --ff-only`.
2. Read AGENTS.md and the artwork contract. Restore existing packages and build the normal VS2017 solution in the owner's usual configuration. Do not upgrade dependencies.
3. Run the full existing website test suite, including `MobileArtworkRegistryTests`, `MobileArtworkCacheHelperTests` and `MobileArtworkControllerTests`. Report exact build/test totals and warnings. The registry tests include all nine energy symbol sets, all 202 moon illumination/direction combinations, numerology code IDs, canonical URL deduplication, isolated results and cache validators.
4. If Python/Pillow is available, run `python tools/update-mobile-artwork.py --check`. Do not regenerate the catalogue unnecessarily.
5. **Deferred until startup is explicitly authorised:** launch DEV and request `/api/mobile/v1/metadata/artwork` anonymously: confirm JSON 200, 424 IDs, 431 variants, absolute media URLs, public five-minute caching and ETag. Repeat with `If-None-Match`: expect 304 without a body. Test an old ETag: expect 200. Confirm logged-in and anonymous bodies/ETags match.
6. Request representative energy, trigram, element, organ, moon, numerology, dark-mode, flag and GIF URLs, including their `?v=` query. Confirm HTTP 200, image MIME and correct image. Check a shared-content pair (earth/stable) shares the exact URL. Verify each returned URL uses the API request origin and application path: Integration, Production and a local virtual directory must remain isolated. Website CDN health/fallback selection must not change catalogue URLs. A missing/invalid request-derived base must return uncacheable JSON 503.
7. Smoke-test the normal website chart/account/cycle pages, personal chart API, moon and numerology APIs. Existing image fields must remain; each non-null new ID must resolve in the catalogue. Confirm account/me and membership/entitlements still use no-store.
8. Report results before any Integration deployment. Do not run schema changes, add TeamCity markers or deploy without the owner's instruction.

## Mobile Codex: implement after backend validation/deployment

Read `D:\Workspace\ninestar\docs\mobile-api.md`, Public artwork catalogue, and this handoff. In `D:\Workspace\ninestar-mobile`, add the typed catalogue client, cache and shared image resolver according to that agreed contract. Keep app-specific notes in `ninestar-mobile/docs/`.

Use per-environment catalogue/ETag storage, handle 200/304 and retry once unconditionally if 304 arrives without a cached body. Use returned IDs/URLs, select actual dark/default variants, share identical image cache entries and load images on demand. Retain legacy rendering for older backends, missing IDs and failed refreshes. Keep the 15-second timeout and preserve authentication, entitlements and unsaved account edits. Do not implement purchasing or alter website pages.

Add meaningful tests for first load, 304 reuse, changed version/media host, startup without cache, failed refresh with/without prior cache, environment switching, theme fallback, missing IDs and identical-content URL sharing. Run tests, lint and typecheck. Validate on iPhone using Integration after the owner deploys: My Chart, cycles/planners, knowledge base, numerology and moon screens as available. Confirm no bulk download, no repeated catalogue fetch on every image and no account/subscription regression. Document any unimplemented screens separately.

## Same-origin catalogue correction

The catalogue now derives `/Images` from the incoming request origin and application path, independently of `MediaService.BaseImagesPath`. Run `CatalogueUsesItsOwnRequestOriginAndApplicationPath` in the Windows test suite to cover Integration, Production, virtual directories, ports and HTTP/HTTPS. After deployment, every variant URL returned by Integration must begin with `https://int.9starkiastrology.com/Images/`. Deploy the image files into the application Images folder; website CDN copies remain a separate concern. Keep the existing environment-separated cache and ETag handling; no client host rewrite is needed. This correction has syntax/source checks here, not a Windows execution or live deployment claim.
