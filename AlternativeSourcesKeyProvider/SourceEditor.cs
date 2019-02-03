using KeePassLib.Cryptography;
using KeePassLib.Cryptography.Cipher;
using KeePassLib.Cryptography.KeyDerivation;
using KeePassLib.Keys;
using KeePassLib.Security;
using KeePassLib.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AlternativeSourcesKeyProvider
{
    public class SourceEditor
    {
        private const string KeyFileExtension = ".key";

        public byte[] bla(string databasePath)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            using (store)
            {
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                var selection = X509Certificate2UI.SelectFromCollection(
                    store.Certificates,
                    "Select a certificate",
                    "Select the certificate you want to use as your authentication method.",
                    X509SelectionFlag.SingleSelection)
                    .Cast<X509Certificate2>().SingleOrDefault();

                if (selection == null) return null;

                var keyFilePath = UrlUtil.StripExtension(databasePath) + KeyFileExtension;

                KeyFile keyFile = new KeyFile();

                if (File.Exists(keyFilePath))
                {
                    keyFile = XmlUtilEx.Deserialize<KeyFile>(File.OpenRead(keyFilePath));
                }

                //keyFile.Sources.Add(new CertificateSource(selection.FriendlyName,))


                //store.Certificates.Find(X509FindType.FindByThumbprint,)
                //selection.Thumbprint;

                KeePassLib.Cryptography.CryptoRandom.Instance.GetRandomBytes(128);

                return null;

            }
        }
    }

    public static class XmlHelper
    {
        public static string Serialize<T>(T t)
        {
            using (var ms = new MemoryStream())
            {
                XmlUtilEx.Serialize(ms, t);
                // XmlUtilEx uses UTF8
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public static T Deserialize<T>(string xml)
        { // XmlUtilEx uses UTF8
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                return XmlUtilEx.Deserialize<T>(ms);
            }
        }
    }

    public class PassphraseSourceFactory
    {
        public static KdfParameters GetBestKdfParameters(uint miliseconds = 1000)
        {
            var kdf = new AesKdf();
            var p = kdf.GetBestParameters(miliseconds);
            kdf.Randomize(p);
            return p;
        }

        public static KdfParameters CreateKdfParameters(ulong? rounds = null)
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

        private static ProtectedBinary DerivePassphrase(ProtectedString passphrase, KdfParameters kdfParameters)
        {
            var compositeKey = new CompositeKey();
            compositeKey.AddUserKey(new KcpPassword(passphrase.ReadString()));
            var derivedKey = compositeKey.GenerateKey32(kdfParameters);
            return derivedKey;
        }

        public static PassphraseSource GeneratePassphraseSource(
            string friendlyName,
            ProtectedString passphrase,
            KdfParameters kdfParameters,
            ProtectedBinary secret)
        {
            // Derive the passphrase
            var derivedKey = DerivePassphrase(passphrase, kdfParameters);

            // Encrypt the secret with the derived key
            byte[] encryptedSecretBytes;
            var iv = CryptoRandom.Instance.GetRandomBytes(16); // AES 256 uses 128bits blocks
            using (var ms = new MemoryStream())
            {
                using (var encryptionStream = new StandardAesEngine().EncryptStream(ms, derivedKey.ReadData(), iv))
                {
                    MemUtil.Write(encryptionStream, secret.ReadData());
                }
                encryptedSecretBytes = ms.ToArray();
            }

            var result = new PassphraseSource(friendlyName, encryptedSecretBytes, iv, kdfParameters);
            return result;
        }

        public static ProtectedBinary DecryptSecret(PassphraseSource passphraseSource, ProtectedString passphrase)
        {
            // Read parameters
            var kdfParameters = passphraseSource.DeserializeKdfParameters();

            // Derive the passphrase
            var derivedKey = DerivePassphrase(passphrase, kdfParameters);

            // Deccrypt the secret with the derived key
            ProtectedBinary decryptedSecret;
            using (var ms = new MemoryStream(passphraseSource.EncryptedSecret))
            using (var decryptionStream = new StandardAesEngine().DecryptStream(ms, derivedKey.ReadData(), passphraseSource.IV))
            {
                decryptedSecret = new ProtectedBinary(true, ReadToEnd(decryptionStream));
            }

            return decryptedSecret;
        }

        private static byte[] ReadToEnd(Stream stream)
        {
            var buffer = new byte[32768];
            using (var ms = new MemoryStream())
            {
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
    }

    public class CertificateSourceFactory
    {

    }
}
