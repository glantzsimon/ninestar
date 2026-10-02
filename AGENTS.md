# NineStar project conventions

Preserve existing structure, naming, formatting, services, repository patterns and dependency injection. Keep changes focused; do not refactor unrelated code. Use C# 7.3 and .NET Framework 4.7.2-compatible code.

## UI authoring

The owner requires UI controls to use existing K9 HTML helpers, rather than hand-written control markup or client-generated control HTML. Inspect comparable views and actual helper signatures before implementing UI.

- Use `Html.BootstrapEditorFor` for editable model properties and `Html.BootstrapDisplayFor` for display properties, with existing editor/display templates and `EditorOptions`.
- Use `Html.BeginBootstrapForm`, `Html.BootstrapButton` and `Html.BootstrapActionLinkButton` for forms and buttons. Reuse existing alert, panel, collapsible-panel and validation helpers.
- Model field labels through existing resource-backed metadata or `EditorOptions`; avoid separately hand-writing labels and inputs.
- For dynamic UI, clone helper-rendered controls instead of creating raw button/input HTML in JavaScript.
- Follow calculator, account and personal-cycle views for spacing, styling and help-icon patterns. Structural layout containers should match existing views.
- Expanded inline help uses the existing `.well` container for rounded corners, even padding and the theme border (white in dark mode). Place it inside the collapse wrapper so the collapsed state and height animation remain correct; reuse shared styles rather than duplicating them.

## Panel loading

Use the existing `$.fn.displaySpinner($container)` and `$.fn.hideSpinner($container)` helpers in `Views/Shared/Scripts/Default.cshtml`, as demonstrated in `Views/Predictions/_CyclesJs.cshtml`. They render the existing `partialSpinner` / optional `partialOverlay` styled by `Content/less/controls/pageSpinner.less`. Do not substitute a plain Loading label or introduce a separate spinner design. Keep the container height stable while loading, stop a pending spinner fade before restarting it, and clear loading on success, failure, timeout and rendering exceptions. Panel requests should own their loading/error handlers without triggering unrelated global AJAX handlers.

## Globalisation

All user-visible labels, help, tooltips and messages belong in Globalisation. Check existing resource keys and text first; reuse entries without duplicates.

Use strongly typed `Dictionary.ResourceName` properties for fixed UI text, including weekday headings. Keep generated accessors in sync. Do not use string-key helpers or `ResourceManager.GetString("FixedLabel", ...)` for fixed text. Calculated resource keys, such as combined year/month/day predictions, may use dynamic lookups.

## Branch and deployment workflow

Work on the existing feature branch and prepare a draft pull request for review. Do not merge or deploy without the user's instruction. Normal commits must not contain `#teamcity`; that marker deliberately triggers Integration deployment and should only be added when deployment is requested.

## git-crypt

The owner uses git-crypt locally: protected files are stored encrypted in Git and decrypted in the local working project. Before editing any file, check the applicable .gitattributes rules (or `git check-attr filter diff -- <path>`) for git-crypt protection. Leave protected files and their encryption attributes untouched. Do not replace ciphertext with plaintext, bypass the filters, or change keys. If a requested change needs a protected file, explain the need to the owner before proceeding. Work on ordinary, unprotected files normally.

Do not assume an unexpected modified-file status is caused by git-crypt. Inspect the diff and applicable attributes before recommending that local changes be discarded.

## LESS and CSS

Treat `webapp/WebApplication/Content/less/` as the styling source. Preserve the existing organisation: `sections/` for page/feature sections (such as predictions and navbar), `controls/` for reusable controls (such as panels and the personal calendar), `main/` for general and responsive styles, and `config/` for shared colours and paths. Edit the relevant existing LESS file rather than adding unrelated overrides.

Keep the corresponding CSS under `Content/` in sync. Check `webapp/WebApplication/compilerconfig.json` for the LESS-to-CSS mapping; register new stylesheets there and in the project as required. Compile with the project's tooling when available. If compilation is unavailable, update both files consistently and clearly state the verification limit.

Reuse existing theme variables instead of hardcoded colours. Panel backgrounds use `background: var(--well-color)` (a gradient in light mode and teal in dark mode). Calendar full-screen mode uses that same variable; embedded mode is transparent. Use the existing energy artwork under `ninestar/energies/` through `MediaService.BaseImagesPath`, matching the cycle views.

For hover and selected states, inspect comparable existing controls before styling. The predictions calendar in `Content/less/sections/predictions.less` provides the calendar visual hierarchy: 2px hover lift and shadow, blue selected background and stronger shadow. Preserve theme colours and account for the general important button shadow in `main/elements.less` when adapting these effects.

Inspect actual stacking contexts and existing z-index values before adding overlays. The calendar is moved to the document body while expanded and restored on close; its z-index must cover the floating navbar/account header. Preserve keyboard focus and Escape handling.

## Maintaining project continuity

As work proceeds, update this file for verified, lasting project conventions and owner preferences. Update relevant `docs/` files for feature behaviour, calculation contracts, decisions and validation limits. Keep entries concise and current; replace superseded guidance rather than accumulating contradictions. Do not record secrets, private subscription links, encryption keys or transient build status.

At the start of a new thread, fetch the current branch and read `AGENTS.md` and the relevant feature documentation before editing. Repository files are the authoritative technical record; project memory is supplementary. Do not rely on old chat summaries or scratch copies when current repository contents are available. For calendar work, read `docs/personal-calendar.md`; its calculation uses the Primary Cycles selected-date path, preserving local calendar date, birth time and their respective timezones.
