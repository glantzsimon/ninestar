# Website icons

`Views/Shared/_SiteIcons.cshtml` owns the favicon, Apple touch icon and manifest declarations. Main, print and retained original layouts render it. Links use `Url.Content` so applications hosted below a virtual directory resolve correctly. Assets and manifest are served from the website origin, independently of MediaService/CDN health or CORS. Existing company logos and their email/layout consumers are retained.

## Files

- `Images/icons/favicon.ico`: real 16, 32 and 48 pixel frames, transparent exterior.
- `Images/icons/apple-touch-icon.png`: exact 180 square PNG.
- `Images/icons/icon-192.png` and `icon-512.png`: ordinary manifest icons.
- `Images/icons/icon-maskable-512.png`: separate icon with more padding to keep the emblem inside the central 80%-diameter safe circle.
- `Images/icons/manifest.json`: existing JSON extension retained; image URLs are relative to its own directory. No new IIS MIME mapping or protected configuration edit is needed. This is icon metadata, not an implementation of offline/installable PWA behaviour.

All generated PNGs preserve the supplied artwork's alpha transparency. Do not add a background colour or flatten transparency unless the owner explicitly requests it. The OS controls its own home-screen rendering. The detailed approved logo is retained at all sizes, rather than introducing a new favicon design. It naturally loses detail at 16 pixels.

## Regeneration and cleanup

Run `python tools/generate-site-icons.py` with Pillow to regenerate **only** the favicon and maskable icon from the current owner-supplied `Images/icons/icon-512.png`. It preserves alpha and does not rewrite the Apple icon, standard 192/512 icons or manifest. The former master under `tools/assets/` is historical artwork; it must not overwrite the owner's updated tighter-cropped icons.

Superseded Apple size variants, Android 192 variant, individual PNG favicon variants, Safari pinned-tab asset and unused social/web/duplicate logo filenames were removed from the website icons folder and project. The original layout's legacy declarations and nonexistent Windows tile link were replaced by the shared partial. No corresponding MobileApplication files or company logos were removed. Mobile artwork metadata was regenerated to reflect the owner's newly replaced company assets; the public IDs and explicit allowlist remain unchanged.

References in database-stored historical email or external bookmarks cannot be ruled out by a repository audit. Removed filenames remain recoverable in Git; deployed media-host copies are not deleted by this change. Review historical links separately if required.

## Validation

Checked exact image dimensions, transparent PNG backgrounds, all three ICO frames, manifest-relative target existence, XML project registrations, consistent shared layout usage and absence of removed icon references in website source. Mobile artwork generator `--check` passes. No schema execution, website startup, deployment or production media deletion was performed. Windows MVC/Razor build and actual browser/home-screen rendering remain to be checked after the normal build/deployment workflow. Favicons and saved shortcuts can retain cached artwork; recreate shortcuts when checking an update.

## Page-load logo

The main and retained original layouts now use `Images/shared/ninestar-spinner.png`, copied without pixel changes from the owner's updated icon-512.png. It is displayed at the previous GIF's 114-square size. The existing `pageFadeInOverlay`, centring styles and fadePageIn.js timings remain intact. The image loads eagerly rather than lazily inside a blocking overlay.

The shared pageSpinner LESS/CSS applies a gentle 1.8-second opacity pulse (1 to 0.55) to `.page-loading-image`, with no rotation or scaling. This avoids wobble from asymmetric artwork. Reduced-motion users get a static image. Existing pageSpinner/partialSpinner border animations are unchanged. The old GIF remains for the original mobile artwork catalogue and external consumers; its main website page-load references were replaced, not its public asset removed.

Validation: exact transparent maskable output and ICO frames checked; source Apple/192/512 icons and manifest unchanged; spinner PNG matches icon-512 bytes; project includes PNG; layout paths and CSS/LESS pulse agree. Full LESS compiler, Windows build and browser verification are unavailable in this environment; compiled CSS was synchronised manually. No deployment or schema operations.
