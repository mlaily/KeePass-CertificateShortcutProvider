using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using KeePassLib.Cryptography;
using KeePassLib.Cryptography.Cipher;
using KeePassLib.Security;
using KeePassLib.Utility;

namespace CertificateShortcutProvider;

public record AllowedRSAEncryptionPadding(RSAEncryptionPadding Value, string Name, string DisplayName)
{
    public static AllowedRSAEncryptionPadding[] List { get; } =
        new[]
        {
            new AllowedRSAEncryptionPadding(RSAEncryptionPadding.OaepSHA256, "OAEPSHA256", "OAEP SHA256 (Recommended)"),
            new AllowedRSAEncryptionPadding(RSAEncryptionPadding.Pkcs1, "PKCS1", "PKCS #1 (Legacy)")
        };
    public static AllowedRSAEncryptionPadding Default => List[0];

    public static AllowedRSAEncryptionPadding GetFromNameOrDefault(string name) => List.SingleOrDefault(x => x.Name == name) ?? Default;
}

public static class CryptoHelpers
{
    public static XmlCertificateShortcutProviderKey EncryptPassphrase(X509Certificate2 certificate, ProtectedString passphrase, AllowedRSAEncryptionPadding rsaEncryptionPadding = null)
    {
        // Instead of directly encrypting the passphrase with the certificate,
        // we use an intermediate random symmetric key.
        // The passphrase is encrypted with the symmetric key, and the symmetric key is encrypted with the certificate.
        // (asymmetric encryption is not suited to encrypt a lot of data)

        // symmetric encryption:
        var randomKey = new ProtectedBinary(true, CryptoRandom.Instance.GetRandomBytes(32));
        var passphraseBinary = new ProtectedBinary(true, passphrase.ReadUtf8());
        var encryptedPassphrase = EncryptSecret(passphraseBinary, randomKey, out var iv);

        // now we asymmetrically encrypt the random key.
        byte[] encryptedRandomKey;
        RSA rsa;
        var rsaPadding = rsaEncryptionPadding ?? AllowedRSAEncryptionPadding.Default;
        ECDsa ecdsa;
        if ((rsa = certificate.GetRSAPublicKey()) != null)
        {
            encryptedRandomKey = rsa.Encrypt(randomKey.ReadData(), rsaPadding.Value);
        }
        else if ((ecdsa = certificate.GetECDsaPublicKey()) != null)
        {
            // TODO:
            // https://stackoverflow.com/questions/47116611/how-can-i-encrypt-data-using-a-public-key-from-ecc-x509-certificate-in-net-fram
            // var ecdh = ECDiffieHellman.Create(ecdsa.ExportParameters(false));
            throw new NotSupportedException("Certificate's key type not supported.");
        }
        else
        {
            throw new NotSupportedException("Certificate's key type not supported.");
        }

        var result = new XmlCertificateShortcutProviderKey(certificate, encryptedRandomKey, iv, encryptedPassphrase, rsaPadding.Name);
        return result;
    }

    public static ProtectedBinary DecryptPassphrase(XmlCertificateShortcutProviderKey keyFile)
    {
        using var fromSourceCertificate = keyFile.ReadCertificate();

        var fromStoreCertificate = LoadFromStore(fromSourceCertificate);

        if (fromStoreCertificate == null)
        {
            throw new InvalidOperationException("The specified certificate could not be found in the store.");
        }

        // First we decrypt the symmetric key:
        ProtectedBinary decryptedKey;
        RSA rsa;
        ECDsa ecdsa;
        if ((rsa = fromStoreCertificate.GetRSAPrivateKey()) != null)
        {
            decryptedKey = new ProtectedBinary(true, rsa.Decrypt(keyFile.EncryptedKey, keyFile.RSAEncryptionPadding.Value));
        }
        else if ((ecdsa = fromStoreCertificate.GetECDsaPrivateKey()) != null)
        {
            // TODO:
            // https://stackoverflow.com/questions/47116611/how-can-i-encrypt-data-using-a-public-key-from-ecc-x509-certificate-in-net-fram
            // var ecdh = ECDiffieHellman.Create(ecdsa.ExportParameters(false));
            throw new NotSupportedException("Certificate's key type not supported.");
        }
        else
        {
            throw new NotSupportedException("Certificate's key type not supported.");
        }

        // Then we use the symmetric key to decrypt the passphrase:
        var decryptedPassphrase = DecryptSecret(keyFile.EncryptedPassphrase, decryptedKey, keyFile.IV);

        return decryptedPassphrase;
    }

    public static X509Certificate2 LoadFromStore(X509Certificate2 certificate)
    {
        var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        using (store)
        {
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            var result = store.Certificates.Find(X509FindType.FindByThumbprint, certificate.Thumbprint, false);
            return result.Cast<X509Certificate2>().SingleOrDefault();
        }
    }

    private static byte[] EncryptSecret(ProtectedBinary secret, ProtectedBinary key, out byte[] iv)
    {
        iv = CryptoRandom.Instance.GetRandomBytes(16); // AES 256 uses 128bits blocks
        using var ms = new MemoryStream();
        using (var encryptionStream = new StandardAesEngine().EncryptStream(ms, key.ReadData(), iv))
        {
            MemUtil.Write(encryptionStream, secret.ReadData());
        }
        return ms.ToArray();
    }

    private static ProtectedBinary DecryptSecret(byte[] encryptedSecret, ProtectedBinary key, byte[] iv)
    {
        using var ms = new MemoryStream(encryptedSecret);
        using var decryptionStream = new StandardAesEngine().DecryptStream(ms, key.ReadData(), iv);
        return new ProtectedBinary(true, ReadToEnd(decryptionStream));
    }

    private static byte[] ReadToEnd(Stream stream)
    {
        var buffer = new byte[32768];
        using var ms = new MemoryStream();
        while (true)
        {
            int read = stream.Read(buffer, 0, buffer.Length);
            if (read <= 0)
                return ms.ToArray();
            else
                ms.Write(buffer, 0, read);
        }
    }
}
