# Mobile artwork validation and app handoff

Backend branch: `feature/mobile-json-api`. Contract: `docs/mobile-api.md`, Public artwork catalogue. Existing web views, membership/auth, private API caching and image fields are unchanged. No migration or deployment is part of this change.

## Website Codex: validate locally first

1. In `D:\Workspace\ninestar`, confirm `git rev-parse --abbrev-ref HEAD` is `feature/mobile-json-api`, preserve local edits and `git pull --ff-only`.
2. Read AGENTS.md and the artwork contract. Restore existing packages and build the normal VS2017 solution in the owner's usual configuration. Do not upgrade dependencies.
3. Run the full existing website test suite, including `MobileArtworkRegistryTests` and `MobileArtworkControllerTests`. Report exact build/test totals and warnings. The registry tests include all nine energy symbol sets, all 202 moon illumination/direction combinations, numerology code IDs, canonical URL deduplication, isolated results and cache validators.
4. If Python/Pillow is available, run `python tools/update-mobile-artwork.py --check`. Do not regenerate the catalogue unnecessarily.
5. Launch DEV and request `/api/mobile/v1/metadata/artwork` anonymously: confirm JSON 200, 424 IDs, 431 variants, absolute media URLs, public five-minute caching and ETag. Repeat with `If-None-Match`: expect 304 without a body. Test an old ETag: expect 200. Confirm logged-in and anonymous bodies/ETags match.
6. Request representative energy, trigram, element, organ, moon, numerology, dark-mode, flag and GIF URLs, including their `?v=` query. Confirm HTTP 200, image MIME and correct image. Check a shared-content pair (earth/stable) shares the exact URL. Validate configured hosted and fallback bases separately without disrupting the live service. Initial startup with an unavailable base must return uncacheable JSON 503.
7. Smoke-test the normal website chart/account/cycle pages, personal chart API, moon and numerology APIs. Existing image fields must remain; each non-null new ID must resolve in the catalogue. Confirm account/me and membership/entitlements still use no-store.
8. Report results before any Integration deployment. Do not run schema changes, add TeamCity markers or deploy without the owner's instruction.

## Mobile Codex: implement after backend validation/deployment

Read `D:\Workspace\ninestar\docs\mobile-api.md`, Public artwork catalogue, and this handoff. In `D:\Workspace\ninestar-mobile`, add the typed catalogue client, cache and shared image resolver according to that agreed contract. Keep app-specific notes in `ninestar-mobile/docs/`.

Use per-environment catalogue/ETag storage, handle 200/304 and retry once unconditionally if 304 arrives without a cached body. Use returned IDs/URLs, select actual dark/default variants, share identical image cache entries and load images on demand. Retain legacy rendering for older backends, missing IDs and failed refreshes. Keep the 15-second timeout and preserve authentication, entitlements and unsaved account edits. Do not implement purchasing or alter website pages.

Add meaningful tests for first load, 304 reuse, changed version/media host, startup without cache, failed refresh with/without prior cache, environment switching, theme fallback, missing IDs and identical-content URL sharing. Run tests, lint and typecheck. Validate on iPhone using Integration after the owner deploys: My Chart, cycles/planners, knowledge base, numerology and moon screens as available. Confirm no bulk download, no repeated catalogue fetch on every image and no account/subscription regression. Document any unimplemented screens separately.
