# Apple purchase verifier compatibility assessment

Assessment date: 23 October 2026. Contract: [mobile-api.md](mobile-api.md), shared website/app Apple subscription proposal, revision 2.

## Finding

**No fundamental .NET Framework 4.7.2 cryptography blocker was found.** An in-process verifier is technically plausible using dependencies already referenced by the website. A complete, security-reviewed Apple verifier and actual Windows/IIS/Sandbox execution remain outstanding. Do not treat this primitive spike as completion of the verifier gate.

## Evidence checked

Inspected WebApplication.csproj and downloaded the actual NuGet archives, rather than relying only on calculated compatibility labels:

| Package | Website/reference evidence | Actual archive assets checked | Conclusion |
| --- | --- | --- | --- |
| BouncyCastle.Cryptography 2.6.2 | Existing reference points to lib/net461 | net461, net6.0, netstandard2.0 | Reusable managed EC/X.509/PKCS#8/OCSP primitives; net461 asset supports the target framework. |
| Microsoft.IdentityModel.Tokens 8.19.2 | Existing reference points to lib/net472 | net462, net472, netstandard2.0 and modern .NET assets | A framework-targeted signing/JWS building block is already referenced; its complete use with Apple payloads was not exercised by this probe. |
| Microsoft.IdentityModel.JsonWebTokens 8.19.2 | Existing reference points to lib/net472 | net462, net472, netstandard2.0 and modern .NET assets | Reuse is a candidate; avoid assuming Apple signed payloads have ordinary access-token issuer/audience/lifetime semantics. |
| Enjna.AppStoreServerLibrary 2.3.0 | Not installed; community library, not Apple-owned | net8.0 only | Cannot directly install this version into the legacy website. This is a package limitation, not proof Apple verification is impossible on Framework. |

The isolated [primitive probe](../tools/AppleVerifierCompatibility/README.md) compiled successfully against official .NET Framework 4.7.2 reference assemblies with C# 7.3 and warnings as errors. It reuses the net461 Bouncy Castle DLL. All **15 primitive checks** passed when the compiled assembly was executed on an isolated Linux .NET 8 runtime. No Windows Framework/MSBuild/VS2017 runtime execution was available here. The probe deliberately avoids database, account and payment code.

## What the full verifier still requires

Signature verification is only one layer. The implementation must apply Apple's trust roots, certificate purpose/constraints/date/revocation policy, verify signed transaction/renewal/notification data and enforce the contract's app/environment/product/owner checks. API authentication also requires server-side signing-key handling. Test malformed and tampered inputs, unrelated roots, certificate/revocation failures and evidence for the wrong app/environment/account. Use maintained cryptographic implementations; never invent the signature algorithms or trust a decoded token/embedded root automatically.

The probe proves basic APIs work in the stated test environment, not that this policy is implemented correctly. It does not make a live Apple request, validate a real Apple transaction, check live OCSP or validate the website's package loading/binding redirects under IIS. Certificate-related calls must fit the bounded request budget, fail closed on unverifiable new purchases and use retry/reconciliation rather than disabling validation.

## Preferred route and fallback

1. **First candidate: a narrow verifier adapter inside the existing application**, reusing the installed cryptographic packages and existing HTTP/dependency-injection conventions. No whole-site framework migration or new authentication system is required by the evidence found so far. Before selecting it, run the probe on Windows and implement a policy-level test spike against Apple's maintained verifier behaviour and signed test fixtures. Validate the complete app build/editor/package-loading behaviour as well as the primitive console program.
2. **Fallback if policy implementation/maintenance or legacy hosting proves impractical: a small private verifier service**, using Apple's maintained server library on a supported Node runtime. It can expose a narrowly authenticated private operation for retrieving/verifying Apple evidence. NineStar calls it server-to-server; the app cannot grant access by calling it directly. Validate the request/response and app/environment configuration on both sides, use fixed provider endpoints and protect signing keys/service credentials. It must own no NineStar database, user system or membership schema: all binding/ownership/entitlement persistence stays in the existing DataAccess/application service. This adds hosting/monitoring and network failure handling, so it is an alternative for review, not a deployment already chosen or performed.

A separate modern .NET verifier host is another possibility if the hosting environment favours it, but the inspected Enjna package is community-maintained and still needs review. Upgrading only .NET Framework 4.7.2 to 4.8 would not make a .NET 8-only package directly compatible. Migrating the whole website to ASP.NET Core is unnecessary for either route.

## Why the authenticated app cannot be the payment authority

The existing login cookie establishes the NineStar account making a request. It does not establish a purchase, ownership of an Apple subscription or current payment validity. A caller can bypass the official app and submit a fabricated purchase assertion to the same endpoint.

Server-side **verification** is mandatory for the proposed security boundary; an Apple network lookup on every website/API request is not. Apple-signed transaction information can be cryptographically verified locally on the server, subject to trust/revocation policy. The proposed App Store Server API lookup establishes current subscription state; verified notifications and periodic reconciliation keep stored state current when the app is closed. Normal content requests consult the stored verified grant and current time, rather than calling Apple for every chart/page. No network-free local signature check alone proves there was no subsequent refund.

## Next concrete checks

- Run the standalone probe on the owner's Windows .NET Framework runtime; no schema or deployment is involved.
- Verify full Apple policy and signing-key import with reviewed synthetic/official test fixtures, then real Sandbox transactions/notifications and current-status API responses once bundle/product/account credentials are configured.
- Exercise certificate rotation, online revocation/network failures, request deadlines and negative account/environment cases; confirm unchanged site build, editor behaviour and authentication.
- Select the verifier adapter implementation before calling the contract's verifier gate passed. Entitlement projection for existing website memberships can proceed independently after contract agreement.

No provider credentials, subscriptions, application configuration, NuGet versions, schema, migration or deployment was changed during this assessment. The new probe is not added to the website solution and cannot run at application startup.

## Primary references

- [Bouncy Castle package 2.6.2](https://www.nuget.org/packages/BouncyCastle.Cryptography/2.6.2)
- [Microsoft IdentityModel Tokens 8.19.2](https://www.nuget.org/packages/Microsoft.IdentityModel.Tokens/8.19.2)
- [Microsoft IdentityModel JsonWebTokens 8.19.2](https://www.nuget.org/packages/Microsoft.IdentityModel.JsonWebTokens/8.19.2)
- [Enjna package requirements](https://www.nuget.org/packages/Enjna.AppStoreServerLibrary/2.3.0)
- [Apple maintained server library](https://github.com/apple/app-store-server-library-node)
- [Apple signed-data verification implementation](https://github.com/apple/app-store-server-library-node/blob/main/jws_verification.ts)
- [Apple current subscription status API](https://developer.apple.com/documentation/appstoreserverapi/get-all-subscription-statuses)
