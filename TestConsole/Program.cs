using AlternativeSourcesKeyProvider;
using KeePassLib.Cryptography.KeyDerivation;
using KeePassLib.Keys;
using KeePassLib.Security;
using KeePassLib.Utility;
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
            var sources = new List<SourceBase>()
            {
            new PassphraseSource("testPass", new byte[32]),
             new CertificateSource("testCert", new byte[32], "thumbprint"),
            };

            var keyFile = new KeyFile(sources);

            var ms = new MemoryStream();

            XmlUtilEx.Serialize(ms, keyFile);

            var outxml = Encoding.UTF8.GetString(ms.ToArray());

            ms.Position = 0;

            var deserializer = XmlUtilEx.Deserialize<KeyFile>(ms);





            var parameters = CreateKdfParameters(1000000);
            var derivedPassphrase = GetDerivedKey("prout", parameters);

            var serializedParameters = KdfParameters.SerializeExt(parameters);
            var ms2 = new MemoryStream();
            XmlUtilEx.Serialize(ms2, serializedParameters);
            var xmlParameters = Encoding.UTF8.GetString(ms2.ToArray());


            var parametersFromXml = KdfParameters.DeserializeExt(serializedParameters);
            var derivedPassphraseFromXmlParameters = GetDerivedKey("prout", parametersFromXml);

            Debug.Assert(derivedPassphrase.Equals(derivedPassphraseFromXmlParameters));
        }

        private static KdfParameters GetBestKdfParameters(uint ms = 1000)
        {
            var kdf = new AesKdf();
            var kdfp = kdf.GetBestParameters(ms);
            kdf.Randomize(kdfp);
            return kdfp;
        }

        private static KdfParameters CreateKdfParameters(ulong? rounds = null)
        {
            var kdf = new AesKdf();
            var p = kdf.GetDefaultParameters();
            kdf.Randomize(p);
            if (rounds != null)
            {
                p.SetUInt64(AesKdf.ParamRounds, rounds.Value);
            }
            return p;
        }

        private static ProtectedBinary GetDerivedKey(string passphrase, KdfParameters kdfParameters)
        {
            var compositeKey = new CompositeKey();
            compositeKey.AddUserKey(new KcpPassword(passphrase));
            var derived = compositeKey.GenerateKey32(kdfParameters);
            return derived;
        }
    }
}
