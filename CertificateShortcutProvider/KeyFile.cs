using KeePassLib.Cryptography.KeyDerivation;
using KeePassLib.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CertificateShortcutProvider
{
    [XmlType("Key")]
    public class KeyFile
    {
        public byte[] EncryptedSecret { get; set; }
        public byte[] Certificate { get; set; }

        public KeyFile() { }
        public KeyFile(byte[] encryptedSecret, X509Certificate2 certificate)
        {
            EncryptedSecret = encryptedSecret;
            // public part only
            Certificate = certificate.Export(X509ContentType.Cert);
        }

        public X509Certificate2 ReadCertificate() => new X509Certificate2(Certificate);
    }
}
