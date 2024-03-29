using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;

namespace CertificateShortcutProvider;

[XmlType("CertificateShortcutProviderKey")]
public class XmlCertificateShortcutProviderKey
{
    public const int CurrentVersion = 2;

    [XmlAttribute]
    public int Version { get; set; }

    public byte[] Certificate { get; set; }

    /// <summary>
    /// Random key encrypted with the certificate.
    /// </summary>
    public byte[] EncryptedKey { get; set; }
    public byte[] IV { get; set; }
    /// <summary>
    /// User passphrase encrypted with the random key.
    /// </summary>
    public byte[] EncryptedPassphrase { get; set; }

    public string RSAEncryptionPaddingName { get; set; }

    public XmlCertificateShortcutProviderKey() { }
    public XmlCertificateShortcutProviderKey(X509Certificate2 certificate, byte[] encryptedKey, byte[] iv, byte[] encryptedPassphrase, string rsaEncryptionPaddingName)
    {
        if (certificate == null) throw new ArgumentNullException(nameof(certificate));

        Version = CurrentVersion;

        // public part only
        Certificate = certificate.Export(X509ContentType.Cert);

        EncryptedKey = encryptedKey;
        IV = iv;
        EncryptedPassphrase = encryptedPassphrase;

        RSAEncryptionPaddingName = rsaEncryptionPaddingName;
    }

    public X509Certificate2 ReadCertificate() => new(Certificate);
    public AllowedRSAEncryptionPadding RSAEncryptionPadding => AllowedRSAEncryptionPadding.GetFromNameOrDefault(RSAEncryptionPaddingName);
}
