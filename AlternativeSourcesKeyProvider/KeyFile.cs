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
        public byte[] EncryptedKey { get; set; }

        public SourceBase() { }
        public SourceBase(string name, byte[] encryptedKey)
        {
            Name = name;
            EncryptedKey = encryptedKey;
        }
    }

    [XmlType("Passphrase")]
    public class PassphraseSource : SourceBase
    {
        public PassphraseSource() { }
        public PassphraseSource(string name, byte[] encryptedKey)
            : base(name, encryptedKey) { }
    }

    [XmlType("X509Certificate")]
    public class CertificateSource : SourceBase
    {
        public string CertificateThumbprint { get; set; }

        public CertificateSource() { }
        public CertificateSource(string name, byte[] encryptedKey, string certificateThumbprint)
            : base(name, encryptedKey)
        {
            CertificateThumbprint = certificateThumbprint;
        }
    }
}
