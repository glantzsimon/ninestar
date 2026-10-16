using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace NineStarKi.Mcp;

/// <summary>
/// Validates ChatGPT's OpenAI-managed TLS client certificate against the
/// official published OpenAI connectors CA chain and expected DNS SAN.
/// Does not authenticate an individual ChatGPT user or membership.
/// </summary>
internal sealed class OpenAiMtlsCertificateValidator : IDisposable
{
    private const string ExpectedDnsName = "mtls.prod.connectors.openai.com";
    private const string ClientAuthenticationOid = "1.3.6.1.5.5.7.3.2";

    private readonly X509Certificate2 _root;
    private readonly X509Certificate2 _intermediate;

    public OpenAiMtlsCertificateValidator(string rootPemPath, string intermediatePemPath)
    {
        // CA certificates are public material downloaded from OpenAI's official
        // plugin documentation. Never commit custom CA bundles or secrets.
        _root = X509Certificate2.CreateFromPemFile(rootPemPath);
        _intermediate = X509Certificate2.CreateFromPemFile(intermediatePemPath);
    }

    public bool IsValid(X509Certificate2? leaf)
    {
        if (leaf is null)
        {
            return false;
        }

        try
        {
            // Verify a DNS SAN specifically, never use the CN fallback.
            var hasExpectedDnsName = leaf.Extensions
                .OfType<X509SubjectAlternativeNameExtension>()
                .SelectMany(extension => extension.EnumerateDnsNames())
                .Any(name => string.Equals(name, ExpectedDnsName, StringComparison.OrdinalIgnoreCase));

            if (!hasExpectedDnsName)
            {
                return false;
            }

            using var chain = new X509Chain();
            chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
            chain.ChainPolicy.CustomTrustStore.Add(_root);
            chain.ChainPolicy.ExtraStore.Add(_intermediate);
            chain.ChainPolicy.ApplicationPolicy.Add(new System.Security.Cryptography.Oid(ClientAuthenticationOid));
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
            chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            chain.ChainPolicy.UrlRetrievalTimeout = TimeSpan.FromSeconds(5);

            if (!chain.Build(leaf))
            {
                return false;
            }

            // In addition to the pinned root, the path must include the exact
            // official OpenAI Connectors mTLS intermediate CA.
            return chain.ChainElements.Cast<X509ChainElement>()
                .Any(element => CryptographicOperations.FixedTimeEquals(
                    element.Certificate.RawData, _intermediate.RawData));
        }
        catch (CryptographicException)
        {
            return false;
        }
    }

    public void Dispose()
    {
        _root.Dispose();
        _intermediate.Dispose();
    }
}
