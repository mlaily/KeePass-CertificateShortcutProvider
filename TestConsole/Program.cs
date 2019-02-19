using KeePassLib.Cryptography;
using KeePassLib.Cryptography.KeyDerivation;
using KeePassLib.Keys;
using KeePassLib.Security;
using KeePassLib.Utility;
using OptionalCertificateOverMasterKeyProvider;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TestConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var certificateHelper = new CertificateEncryptionHelper();

            var secret = new ProtectedBinary(true, StrUtil.Utf8.GetBytes("master password"));
            var keyFile = certificateHelper.EncryptSecret(certificateHelper.PickCertificate(), secret);

            var keyFileXml = XmlHelper.Serialize(keyFile);

            var keyFileRoundTrip = XmlHelper.Deserialize<KeyFile>(keyFileXml);

            var certificateSecretRoundTrip = certificateHelper.DecryptSecret(keyFileRoundTrip);

            Debug.Assert(secret.ReadData().SequenceEqual(certificateSecretRoundTrip.ReadData()));
        }
    }
}
