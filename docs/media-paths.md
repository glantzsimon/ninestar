# Shared media paths

The existing `Config/appsettings.json` → `ConfigHelper.GetConfiguration<DefaultValuesConfiguration>` registration remains the single source of media settings. There is no additional settings file, configuration model or registration.

Add this property to the existing `DefaultValuesConfiguration` JSON section:

```json
"UseHostedMedia": false
```

The owner has added `public bool UseHostedMedia { get; set; }` to the encrypted `DefaultValuesConfiguration.cs`. Both owner-side changes must accompany this commit. Encrypted files were not edited from this checkout. Because ConfigHelper maps every property, add the explicit JSON entry before rebuilding/restarting; do not rely on a missing property default.

## Environment selection

| Build | Existing setting | Default supplied by owner |
| --- | --- | --- |
| Integration (`Integration`, even with `DEBUG`) | `SiteIntBaseUrl` | `int.9starkiastrology.com` |
| Local (`DEBUG`, without `Integration` or `PRODUCTION`) | `LocalMediaBasePath` | `https://localhost/ninestar` |
| Release / Production | `LiveMediaBasePath` | `https://9starkiastrology.com` |

Scheme-less site URLs receive `https://`. Explicit HTTP(S), ports and application paths are preserved; trailing slashes are removed. Invalid URLs, credentials, queries and fragments are rejected. Change the existing local setting if the local IIS application uses another port or virtual directory. `Request.IsLocal` and incoming Host headers do not select environment, so background jobs use the same stable paths as web requests.

`MediaService.InitialiseMediaPaths` runs during configuration registration, before requests or background services can use the shared paths. `/Images` and `/Videos` are appended to the selected application base. The static MediaService paths, DefaultValuesConfiguration paths and email template media paths are kept consistent. Existing views, legacy API image fields and embedded help HTML inherit these paths without per-view changes. User-supplied or third-party URLs are not rewritten.

## Hosted media

When `UseHostedMedia` is false, HTTP health checks return immediately. Startup removes the existing `CheckStorjHealth` recurring job, and already queued calls also skip the probe. All shared image/video paths use the selected installation.

To restore hosted media later, set `UseHostedMedia` to true and restart. The original `HostedImagesBasePath` and `HostedVideosBasePath` remain available. Startup and the existing periodic check select hosted media after a successful probe; failure falls back to the selected installation. Upload tooling is unchanged.

The mobile artwork catalogue separately remains request-origin based, preserving its virtual application paths, environment-specific URLs, public caching and ETags. It is not switched to Storj by this setting. No access rules or API response fields change.

## Validation and deployment

Rebuild the intended environment configuration after applying the owner-side class/JSON edits. Deploy binaries and the existing updated appsettings file, plus the Images/Videos content required on that installation. No new runtime files or schema changes are required.

`MediaServiceTests` covers selected-build mapping, scheme-less URLs, ports/virtual directories, disabled probes with absent hosted settings and no HTTP context, invalid configuration, and agreement among static/configuration/email paths. Run in Debug, Integration and Production website builds to cover all conditional branches. Full .NET Framework build, xUnit execution and IIS checks require the Windows development environment and were not executed here.

After restart, inspect a chart image, stars video and legacy API image in the browser network panel. Expected bases are localhost/ninestar, int.9starkiastrology.com and 9starkiastrology.com respectively. Confirm the Storj recurring job is absent while disabled. Check `/api/mobile/v1/metadata/artwork` independently for request-origin URLs.
