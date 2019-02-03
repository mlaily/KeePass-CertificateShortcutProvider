using AlternativeSourcesKeyProvider;
using KeePassLib.Cryptography;
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
            var passphraseSourceFactory = new PassphraseSourceFactory();
            var certificateSourceFactory = new CertificateSourceFactory();

            // about 4s on my machine
            // var kdfParameters = PassphraseSourceFactory.CreateKdfParameters(10_000_000);
            var kdfParameters = passphraseSourceFactory.GetBestKdfParameters(500);
            var secret = new ProtectedBinary(true, CryptoRandom.Instance.GetRandomBytes(32));

            var sources = new List<SourceBase>()
            {
                passphraseSourceFactory.GeneratePassphraseSource(
                    "testPassphrase",
                    new ProtectedString(true, "riendutout"),
                    kdfParameters,
                    secret),
                certificateSourceFactory.GenerateCertificateSource(
                    "testCert",
                    certificateSourceFactory.PickCertificate(),
                    secret),
            };

            var keyFile = new KeyFile(sources);

            var keyFileXml = XmlHelper.Serialize(keyFile);

            var keyFileRoundTrip = XmlHelper.Deserialize<KeyFile>(keyFileXml);

            var passphraseSecretRoundTrip = passphraseSourceFactory.DecryptSecret(
                (PassphraseSource)keyFileRoundTrip.Sources[0],
                new ProtectedString(true, "riendutout"));

            Debug.Assert(secret.ReadData().SequenceEqual(passphraseSecretRoundTrip.ReadData()));

            var certificateSecretRoundTrip = certificateSourceFactory.DecryptSecret((CertificateSource)keyFileRoundTrip.Sources[1]);

            Debug.Assert(secret.ReadData().SequenceEqual(certificateSecretRoundTrip.ReadData()));

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
