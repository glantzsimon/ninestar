# NineStar project conventions

Preserve existing structure, naming, formatting, services, repository patterns and dependency injection. Keep changes focused; do not refactor unrelated code. Use C# 7.3 and .NET Framework 4.7.2-compatible code.

## UI authoring

The owner requires UI controls to use existing K9 HTML helpers, rather than hand-written control markup or client-generated control HTML. Inspect comparable views and actual helper signatures before implementing UI.

- Use `Html.BootstrapEditorFor` for editable model properties and `Html.BootstrapDisplayFor` for display properties, with existing editor/display templates and `EditorOptions`.
- Use `Html.BeginBootstrapForm`, `Html.BootstrapButton` and `Html.BootstrapActionLinkButton` for forms and buttons. Reuse existing alert, panel, collapsible-panel and validation helpers.
- Model field labels through existing resource-backed metadata or `EditorOptions`; avoid separately hand-writing labels and inputs.
- For dynamic UI, clone helper-rendered controls instead of creating raw button/input HTML in JavaScript.
- Follow calculator, account and personal-cycle views for spacing, styling and help-icon patterns. Structural layout containers should match existing views.

## Globalisation

All user-visible labels, help, tooltips and messages belong in Globalisation. Check existing resource keys and text first; reuse entries without duplicates.

Use strongly typed `Dictionary.ResourceName` properties for fixed UI text, including weekday headings. Keep generated accessors in sync. Do not use string-key helpers or `ResourceManager.GetString("FixedLabel", ...)` for fixed text. Calculated resource keys, such as combined year/month/day predictions, may use dynamic lookups.

## Branch and deployment workflow

Work on the existing feature branch and prepare a draft pull request for review. Do not merge or deploy without the user's instruction. Normal commits must not contain `#teamcity`; that marker deliberately triggers Integration deployment and should only be added when deployment is requested.

## git-crypt

The owner uses git-crypt locally: protected files are stored encrypted in Git and decrypted in the local working project. Before editing any file, check the applicable .gitattributes rules (or `git check-attr filter diff -- <path>`) for git-crypt protection. Leave protected files and their encryption attributes untouched. Do not replace ciphertext with plaintext, bypass the filters, or change keys. If a requested change needs a protected file, explain the need to the owner before proceeding. Work on ordinary, unprotected files normally.

Do not assume an unexpected modified-file status is caused by git-crypt. Inspect the diff and applicable attributes before recommending that local changes be discarded.
