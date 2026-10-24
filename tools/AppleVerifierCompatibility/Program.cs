// Compatibility probe only. Synthetic keys/certificates are never Apple evidence.
// This is NOT an Apple verifier and must not be used to grant membership access.
using System;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

internal static class Program
{
    private const string LeafPurpose = "1.2.840.113635.100.6.11.1";
    private const string IntermediatePurpose = "1.2.840.113635.100.6.2.1";
    private static int _passed;

    private static int Main()
    {
        try
        {
            var now = DateTime.UtcNow;
            var rootKey = Key();
            var intermediateKey = Key();
            var leafKey = Key();
            var otherKey = Key();
            var root = Certificate("probe-root", "probe-root", rootKey.Public, rootKey.Private, 1, true, null, now);
            var intermediate = Certificate("probe-intermediate", "probe-root", intermediateKey.Public, rootKey.Private, 2, true, IntermediatePurpose, now);
            var leaf = Certificate("probe-leaf", "probe-intermediate", leafKey.Public, intermediateKey.Private, 3, false, LeafPurpose, now);
            var parsed = new X509CertificateParser().ReadCertificate(leaf.GetEncoded());
            Check(parsed.SerialNumber.Equals(leaf.SerialNumber), "X.509 DER certificate parsing");

            var p8 = PrivateKeyInfoFactory.CreatePrivateKeyInfo(leafKey.Private).GetDerEncoded();
            var imported = PrivateKeyFactory.CreateKey(p8);
            Check(imported is ECPrivateKeyParameters, "PKCS#8 EC private-key import (Apple .p8 building block)");

            // JOSE uses P1363 r||s, NOT the DER sequence used by X.509 signatures.
            var signingInput = Encoding.ASCII.GetBytes("synthetic-header.synthetic-payload");
            var signature = Sign(signingInput, imported);
            Check(signature.Length == 64, "ES256-compatible P-256 signature is 64-byte r||s");
            Check(Verify(signingInput, signature, parsed.GetPublicKey()), "ECDSA signature acceptance");
            var tampered = (byte[])signingInput.Clone();
            tampered[0] ^= 1;
            Check(!Verify(tampered, signature, parsed.GetPublicKey()), "Changed payload rejected");
            var badSignature = (byte[])signature.Clone();
            badSignature[0] ^= 1;
            Check(!Verify(signingInput, badSignature, parsed.GetPublicKey()), "Changed signature rejected");
            Check(!Verify(signingInput, signature, otherKey.Public), "Unrelated signing key rejected");

            // Supplying trusted roots is application policy; x5c is not a trust store.
            root.Verify(rootKey.Public);
            intermediate.Verify(root.GetPublicKey());
            leaf.Verify(intermediate.GetPublicKey());
            Check(true, "Root/intermediate/leaf certificate signatures validate");
            Reject(() => intermediate.Verify(otherKey.Public), "Unrelated trust-root key rejected");
            Check(leaf.GetExtensionValue(new DerObjectIdentifier(LeafPurpose)) != null &&
                intermediate.GetExtensionValue(new DerObjectIdentifier(IntermediatePurpose)) != null &&
                intermediate.GetBasicConstraints() >= 0,
                "Certificate purpose OIDs and CA constraint are readable");
            Check(root.GetExtensionValue(new DerObjectIdentifier(LeafPurpose)) == null,
                "Missing leaf purpose can be distinguished");
            leaf.CheckValidity(now);
            Reject(() => leaf.CheckValidity(now.AddDays(10)), "Expired certificate rejected");

            var id = new CertificateID(new AlgorithmIdentifier(NistObjectIdentifiers.IdSha256, DerNull.Instance),
                intermediate, leaf.SerialNumber);
            var request = new OcspReqGenerator();
            request.AddRequest(id);
            var encodedRequest = request.Generate().GetEncoded();
            Check(new OcspReq(encodedRequest).GetRequestList().Length == 1, "OCSP request generation and parsing");
            var response = new BasicOcspRespGenerator(intermediateKey.Public);
            response.AddResponse(id, CertificateStatus.Good, now, now.AddMinutes(5), null);
            var signedResponse = response.Generate("SHA256WITHECDSA", intermediateKey.Private, new[] { intermediate }, now);
            Check(signedResponse.Verify(intermediateKey.Public) && signedResponse.Responses.Length == 1,
                "Signed OCSP response generation and signature verification");
            Check(!signedResponse.Verify(otherKey.Public), "OCSP response with unrelated key rejected");

            Console.WriteLine("PASS: " + _passed + " primitive compatibility checks.");
            Console.WriteLine("Not tested: Apple trust policy, live OCSP/network, Windows/IIS, real Apple transactions, membership integration.");
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine("FAIL: " + exception.GetType().Name + ": " + exception.Message);
            return 1;
        }
    }

    private static AsymmetricCipherKeyPair Key()
    {
        var generator = new ECKeyPairGenerator();
        generator.Init(new ECKeyGenerationParameters(SecObjectIdentifiers.SecP256r1, new SecureRandom()));
        return generator.GenerateKeyPair();
    }

    private static X509Certificate Certificate(string subject, string issuer, AsymmetricKeyParameter publicKey,
        AsymmetricKeyParameter issuerPrivateKey, long serial, bool ca, string purpose, DateTime now)
    {
        var generator = new X509V3CertificateGenerator();
        generator.SetSerialNumber(BigInteger.ValueOf(serial));
        generator.SetIssuerDN(new X509Name("CN=" + issuer));
        generator.SetSubjectDN(new X509Name("CN=" + subject));
        generator.SetNotBefore(now.AddDays(-1));
        generator.SetNotAfter(now.AddDays(1));
        generator.SetPublicKey(publicKey);
        generator.AddExtension(X509Extensions.BasicConstraints, true, new BasicConstraints(ca));
        if (purpose != null)
            generator.AddExtension(new DerObjectIdentifier(purpose), false, DerNull.Instance);
        return generator.Generate(new Asn1SignatureFactory("SHA256WITHECDSA", issuerPrivateKey));
    }

    private static byte[] Sign(byte[] input, AsymmetricKeyParameter privateKey)
    {
        var signer = SignerUtilities.GetSigner("SHA-256withPLAIN-ECDSA");
        signer.Init(true, privateKey);
        signer.BlockUpdate(input, 0, input.Length);
        return signer.GenerateSignature();
    }

    private static bool Verify(byte[] input, byte[] signature, AsymmetricKeyParameter publicKey)
    {
        var signer = SignerUtilities.GetSigner("SHA-256withPLAIN-ECDSA");
        signer.Init(false, publicKey);
        signer.BlockUpdate(input, 0, input.Length);
        return signer.VerifySignature(signature);
    }

    private static void Reject(Action operation, string name)
    {
        var rejected = false;
        try { operation(); }
        catch (Org.BouncyCastle.Security.GeneralSecurityException) { rejected = true; }
        Check(rejected, name);
    }

    private static void Check(bool condition, string name)
    {
        if (!condition) throw new InvalidOperationException(name);
        _passed++;
        Console.WriteLine("PASS " + name);
    }
}
