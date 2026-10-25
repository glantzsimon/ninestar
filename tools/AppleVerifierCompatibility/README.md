# Apple verifier primitive compatibility probe

This standalone console probe targets .NET Framework 4.7.2 and C# 7.3. It is not included in NineStar.sln or the website project, has no website/database dependency, makes no network requests and uses only synthetic keys/certificates created in memory. It reuses BouncyCastle.Cryptography 2.6.2 from the existing website package directory. No package upgrade or new NuGet dependency is required.

**This is not an Apple transaction verifier. Never use it to grant access.** It checks cryptographic API availability/behaviour, not complete JWS parsing, Apple trust policy, OCSP freshness/delegation policy, app/environment/ownership validation or subscription reconciliation.

## Run in the normal Windows development checkout

After the existing website packages have been restored, use the VS2017 Developer Command Prompt from the repository root:

```bat
msbuild tools\AppleVerifierCompatibility\AppleVerifierCompatibility.csproj /t:Build /p:Configuration=Debug
tools\AppleVerifierCompatibility\bin\Debug\AppleVerifierCompatibility.exe
```

Expected exit code: 0, with `PASS: 15 primitive compatibility checks.` A missing DLL means first checking the existing package restore/path, not changing website dependency versions. The project requires the existing .NET Framework 4.7.2 targeting pack and a compiler supporting the repository's C# 7.3 setting.

## Verified in the cloud spike, 23 October 2026

- Compiled Program.cs with Roslyn 4.8.0, `-langversion:7.3`, `-nostdlib+`, warnings as errors, official .NET Framework 4.7.2 reference assemblies (Microsoft.NETFramework.ReferenceAssemblies.net472 1.0.3) and the **net461** Bouncy Castle 2.6.2 DLL used by the website.
- Compilation passed without warnings/errors.
- Executed that assembly with an isolated Linux .NET 8.0.22 runtime: all 15 checks passed.
- **Not executed under Windows .NET Framework or IIS.** Cross-target compilation and Linux execution are useful evidence, not a substitute for that runtime check or a passing VS2017 build.
- The temporary compiler/runtime/packages stayed outside the repository. No binary artifacts, private keys, database changes, website reference changes or deployment are part of this probe.

The checks cover X.509 DER parsing, PKCS#8 EC private-key import, P-256/SHA-256 signatures in JOSE-compatible 64-byte r||s representation, signature acceptance and tampering/wrong-key rejection, synthetic chain signatures, purpose OID/CA readability, expired certificate rejection, OCSP request parsing and signed response acceptance/wrong-key rejection. They do not establish that a synthetic test certificate is an Apple certificate.

Read [the compatibility assessment](../../docs/apple-verifier-compatibility.md) for the implementation recommendation, alternatives and remaining gates.
