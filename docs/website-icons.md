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

The main and retained original layouts use the owner's committed `Images/shared/ninestar-spinner.webp` for page loading. Its rotation, blur and opacity changes are encoded in the image; no additional CSS pulse is applied. A picture source selects WebP when reduced motion is not requested; the existing PNG is the fallback for unsupported WebP or reduced motion. The supplied WebP and PNG are unchanged.

The existing application-relative Url.Content paths, 114-square size, eager image loading, centring styles, pageFadeInOverlay and fadePageIn.js timings are preserved. Existing pageSpinner and partialSpinner controls and old GIF assets remain untouched. The WebP is explicitly registered as Content for publishing.

Validation: source asset exists in the branch; project XML, references, unchanged image hashes and removal of CSS pulse checked. Full Windows MVC/Razor build and hosted browser validation remain outstanding. IIS must serve .webp as image/webp; protected hosting configuration has not been inspected or changed. No deployment or schema operations.
