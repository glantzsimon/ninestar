# Quick deployment: Production and Integration

These changes prepare deployment tooling; they do not install server scripts, deploy either website, restart IIS, execute migrations or upload anything. Work remains on `feature/mobile-json-api`.

## Configuration and routing

The owner added these properties to the existing encrypted DefaultValuesConfiguration and shared appsettings.json. Keep the same entries in the **local and server** appsettings files:

```json
"VaultPath": "C:\\inetpub\\vhosts\\9starkiastrology.com\\vault",
"VaultIntPath": "C:\\inetpub\\vhosts\\9starkiastrology.com\\vault\\int",
"VaultDeployScript": "deploy-vault-to-app.ps1",
"SiteIntBaseUrl": "int.9starkiastrology.com"
```

The Python entry point normalises SiteIntBaseUrl to `https://int.9starkiastrology.com` and calls that site's deployment API with `environment=int`. Production keeps the existing configured upload host and sends `environment=prod`. The Integration host also rejects Production targets. Missing/unknown/mismatched configuration fails; there is no fallback from Integration to Production.

The deployment API remains in the plugin ApiController, with deployment-specific implementation moved to `Controllers/ApiController.Deployment.cs`. It preserves API-key authentication, adds HTTP error statuses/no-store and a read-only `GET /api/deploy/target?environment=int` preflight. Upload/maintenance successes include `{ success: true, environment: "int", protocolVersion: 2, error: null }`. The new client requires all three acknowledgement fields. A 200/HTML/false/mismatched acknowledgement is failure. Old servers fail preflight before the first upload.

Uploads explicitly specify a category, preserve subfolders (including JSON and Views config), reject path traversal and mismatched extensions, and save via a temporary file/rename. Legacy requests without category/environment still map to Production and retain the `views.config` alias. Shared VaultDeployScript is always resolved in VaultPath; only data staging selects VaultIntPath. The script must contain the matching protocol marker or preflight fails.

The common server script lives at `C:\inetpub\vhosts\9starkiastrology.com\vault\deploy-vault-to-app.ps1`:

| Environment | Source staging | Destination | Application pool |
| --- | --- | --- | --- |
| prod | `vault` | sibling `httpdocs` | `9starkiastrology.com(domain)(4.0)(pool)` |
| int | `vault\int` | sibling `int` | `int.9starkiastrology.com(domain)(4.0)(pool)` |

It accepts `-Environment prod|int` and `-SourceRoot`, defaults to the existing Production behaviour, and rejects a source root inconsistent with the environment. It verifies copies using SHA-256, checks the app pool before binary copying, and exits nonzero on copy/restart/cleanup errors. Source files are only consumed after all copies and any binary-triggered app-pool restart succeed. This is **not** an atomic deployment or rollback: a failure after some copies can leave a partially updated destination. Inspect it before retrying. A lost HTTP acknowledgement during a restart is treated as unconfirmed, not as success; verify server state before retrying.

## Installation order: one-time setup

1. Pull this branch in `D:\Workspace\ninestar`. Keep a single uploader: `deploy/quick-deploy.py`. Save the supplied replacement over this **locally decrypted** file, preserving its configuration/API key, then commit and push through the existing git-crypt setup. Do not upload plaintext to GitHub or change encryption attributes. The replacement is returned separately because this editing environment cannot encrypt the protected Git blob; it is not installed by this cleanup commit. Install that replacement before running the uploader tests. No wrapper or second uploader is required.
2. Build/test the website with VS2017. Run the added `DeploymentTargetHelperTests` and `DeploymentControllerTests` (no IIS/startup/PowerShell deployment required). Run `python -m unittest discover -s tools/tests -p test_quick_deploy.py`. Python uses the existing requests dependency; no package upgrade is required.
3. Back up the current server-side deployment script and replace it manually with the repository's updated `deploy/deploy-vault-to-app.ps1`, keeping it in the root vault. It remains compatible with the old Production call that passes no parameters. During installation, use only its `-WhatIf` preview, never a real deployment. The owner already created `vault\int`.
4. Deploy the new backend to **Integration through the existing TeamCity workflow**, only when the owner authorises that deployment. This bootstraps the new preflight/upload/maintenance protocol without relying on quick deploy. Production quick deploy with the new client remains unavailable until Production also receives this backend through its normal approved release. Do not deploy the feature branch to Production merely to enable the tooling.
5. Keep the existing Git Bash function pointing to `deploy/quick-deploy.py`, forwarding arguments. If it was changed to a wrapper, restore it as follows, retaining the original Python executable if different. Save it in the same shell profile and reload that profile:

```bash
deployninestar() {
    python /d/Workspace/ninestar/deploy/quick-deploy.py "$@"
}
```

6. First run `deployninestar full int --dry-run`. It performs no network calls, builds, staging writes or deployment. Review the target, configuration and file selection. A first Integration run without its own timestamp selects the complete asset baseline.
7. Only when explicitly ready to deploy, run `deployninestar full int`. It preflights the server, builds Integration, transforms its web.config, stages/uploads, invokes maintenance, and requires a successful acknowledgement before advancing the checkpoint. Confirm Integration and the mobile API, and verify Production was untouched.

Server-side preview, when desired, is `powershell.exe -NoProfile -File C:\inetpub\vhosts\9starkiastrology.com\vault\deploy-vault-to-app.ps1 -Environment int -WhatIf`. This only lists a plan; a real deployment can restart the app and run its existing startup migrations. No schema/startup bypass was added.

## Commands and failure semantics

| Command | Behaviour |
| --- | --- |
| `deployninestar` | Changed Production views/assets |
| `deployninestar full` | Above plus Production2 build, transformed web.config, all top-level bin DLL/PDBs and changed JSON config |
| `deployninestar int` | Changed Integration views/assets |
| `deployninestar full int` | Above plus Integration build, transformed web.config, all top-level bin DLL/PDBs and changed JSON config |
| `deployninestar full int --dry-run` | Local preview only |
| `deployninestar full int --all-files` | Deliberately include all eligible assets/config, ignoring the timestamp |

`full` still does not delete obsolete destination files or reproduce every Web Deploy publish action. Use the normal full publish when those are needed. It now **builds** first and includes dependencies, instead of copying potentially stale project DLLs. Production2 and Integration both output to the existing bin directory; a local cross-environment lock prevents concurrent uploader runs. Do not build the same checkout concurrently in Visual Studio. The Videos source is corrected from the previous Images path to the actual Videos directory.

Local staging is separate under `vault\prod` and `vault\int`. Checkpoints are `deploy/last_run.prod.txt` and `deploy/last_run.int.txt`. Production initially honours the old last_run.txt if present; Integration never inherits Production's timestamp. A successful run records its **start** time so files edited during the deployment cannot be skipped by a later run. Uploads and maintenance must all acknowledge success before advancing it. A failure preserves staging/checkpoints and exits nonzero; no automatic retries or opposite-environment fallback occur. A retained full deployment must be retried with full before an assets-only run. Keep runs serial; this is not a multi-operator deployment transaction.

HTTPS verification and finite request timeouts are enabled. Credentials remain in the same protected local script and are never printed or returned by preflight. The supplied replacement preserves the existing key; it must be committed through local git-crypt. No actual credentials or encrypted file contents are embedded in tests/docs.

## Validation limits

All 17 Python tests passed here, using fake responses/temp directories and exercise 200/success:false, malformed JSON, HTTP errors, environment/protocol mismatch, old-server preflight, upload/maintenance failure, retained staging, independent checkpoints, retry, dry-run, build failure, dependency/config selection, locking and Videos source. They make no network requests. C#/Python syntax, PowerShell grammar, project registrations and source encryption boundaries also passed here. VS2017/full xUnit, native PowerShell runtime, IIS permissions, real uploads and restarts still require Windows/Integration validation. A script or controller source change is not deployment approval.

## Encrypted-file editing convention

Keep a single uploader, editing the existing protected file rather than introducing a second entry point or loader. Tests load `deploy/quick-deploy.py` directly. In an environment holding only the encrypted Git blob, the test runner can set `NINESTAR_QUICK_DEPLOY_TEST_SOURCE` to the supplied plaintext replacement for offline tests; this is a test input, not another deployment implementation. Normal Windows runs need no override. The cleanup leaves the protected Git blob unchanged pending the owner's local replacement/commit, so do not mistake a Git pull alone for installing the updated uploader.
