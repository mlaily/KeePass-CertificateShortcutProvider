using KeePassLib.Cryptography.KeyDerivation;
using KeePassLib.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AlternativeSourcesKeyProvider
{
    [XmlType("Key")]
    public class KeyFile
    {
        [XmlArray("Sources")]
        [XmlArrayItem("Source")]
        public List<SourceBase> Sources { get; set; } = new List<SourceBase>();

        public KeyFile() { }
        public KeyFile(IEnumerable<SourceBase> sources)
        {
            Sources = new List<SourceBase>(sources);
        }
    }

    [XmlInclude(typeof(PassphraseSource))]
    [XmlInclude(typeof(CertificateSource))]
    public abstract class SourceBase
    {
        public string Name { get; set; }
        public byte[] EncryptedSecret { get; set; }
        public byte[] IV { get; set; }

        public SourceBase() { }
        public SourceBase(string name, byte[] encryptedSecret, byte[] iv)
        {
            Name = name;
            EncryptedSecret = encryptedSecret;
            IV = iv;
        }
    }

    [XmlType("Passphrase")]
    public class PassphraseSource : SourceBase
    {
        public byte[] KdfParameters { get; set; }

        public PassphraseSource() { }
        public PassphraseSource(string name, byte[] encryptedSecret, byte[] iv, KdfParameters kdfParameters)
            : base(name, encryptedSecret, iv)
        {
            KdfParameters = KeePassLib.Cryptography.KeyDerivation.KdfParameters.SerializeExt(kdfParameters);
        }

        public KdfParameters DeserializeKdfParameters()
            => KeePassLib.Cryptography.KeyDerivation.KdfParameters.DeserializeExt(KdfParameters);
    }

    [XmlType("X509Certificate")]
    public class CertificateSource : SourceBase
    {
        public string CertificateThumbprint { get; set; }

        public CertificateSource() { }
        public CertificateSource(string name, byte[] encryptedSecret, byte[] iv, string certificateThumbprint)
            : base(name, encryptedSecret, iv)
        {
            CertificateThumbprint = certificateThumbprint;
        }
    }
}
