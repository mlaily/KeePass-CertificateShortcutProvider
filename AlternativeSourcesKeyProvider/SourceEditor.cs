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
using System.Security.Cryptography;
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

    public abstract class SourceFactoryBase
    {
        protected byte[] EncryptSecret(ProtectedBinary secret, ProtectedBinary key, out byte[] iv)
        {
            iv = CryptoRandom.Instance.GetRandomBytes(16); // AES 256 uses 128bits blocks
            using (var ms = new MemoryStream())
            {
                using (var encryptionStream = new StandardAesEngine().EncryptStream(ms, key.ReadData(), iv))
                {
                    MemUtil.Write(encryptionStream, secret.ReadData());
                }
                return ms.ToArray();
            }
        }

        protected ProtectedBinary DecryptSecret(byte[] encryptedSecret, ProtectedBinary key, byte[] iv)
        {
            using (var ms = new MemoryStream(encryptedSecret))
            using (var decryptionStream = new StandardAesEngine().DecryptStream(ms, key.ReadData(), iv))
            {
                return new ProtectedBinary(true, ReadToEnd(decryptionStream));
            }
        }

        protected byte[] ReadToEnd(Stream stream)
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

    public class PassphraseSourceFactory : SourceFactoryBase
    {
        public KdfParameters GetBestKdfParameters(uint miliseconds = 1000)
        {
            var kdf = new AesKdf();
            var p = kdf.GetBestParameters(miliseconds);
            kdf.Randomize(p);
            return p;
        }

        public KdfParameters CreateKdfParameters(ulong? rounds = null)
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

        public PassphraseSource GeneratePassphraseSource(
            string friendlyName,
            ProtectedString passphrase,
            KdfParameters kdfParameters,
            ProtectedBinary secret)
        {
            // Derive the passphrase
            var derivedKey = DerivePassphrase(passphrase, kdfParameters);

            // Encrypt the secret with the derived key
            var encryptedSecretBytes = EncryptSecret(secret, derivedKey, out var iv);

            var result = new PassphraseSource(friendlyName, encryptedSecretBytes, iv, kdfParameters);
            return result;
        }

        public ProtectedBinary DecryptSecret(PassphraseSource passphraseSource, ProtectedString passphrase)
        {
            // Read parameters
            var kdfParameters = passphraseSource.DeserializeKdfParameters();

            // Derive the passphrase
            var derivedKey = DerivePassphrase(passphrase, kdfParameters);

            // Deccrypt the secret with the derived key
            var decryptedSecret = DecryptSecret(passphraseSource.EncryptedSecret, derivedKey, passphraseSource.IV);

            return decryptedSecret;
        }
    }

    public class CertificateSourceFactory
    {
        public X509Certificate2 PickCertificate()
        {
            MessageService.ShowInfoEx("Get ready", "If you want to use a certificate from a hardware token, please plug it now...");

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

                return selection;
            }
        }

        public X509Certificate2 LoadFromStore(X509Certificate2 certificate)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            using (store)
            {
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                var result = store.Certificates.Find(X509FindType.FindByThumbprint, certificate.Thumbprint, false);
                return result.Cast<X509Certificate2>().SingleOrDefault();
            }
        }

        public CertificateSource GenerateCertificateSource(
            string friendlyName,
            X509Certificate2 certificate,
            ProtectedBinary secret)
        {
            byte[] encryptedSecret;

            RSA rsa;
            ECDsa ecdsa;
            if ((rsa = certificate.GetRSAPublicKey()) != null)
            {
                //var csp = new RSACryptoServiceProvider();
                //csp.ImportParameters(rsa.ExportParameters(false));

                //encryptedSecret = csp.Encrypt(secret.ReadData(), RSAEncryptionPadding.OaepSHA256);

                encryptedSecret = rsa.Encrypt(secret.ReadData(), RSAEncryptionPadding.OaepSHA256);
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

            var result = new CertificateSource(friendlyName, encryptedSecret, null, certificate);
            return result;
        }

        public ProtectedBinary DecryptSecret(CertificateSource certificateSource)
        {
            var fromSourceCertificate = certificateSource.ReadCertificate();
            var fromStoreCertificate = LoadFromStore(fromSourceCertificate);

            ProtectedBinary decryptedSecret;

            RSA rsa;
            ECDsa ecdsa;
            if ((rsa = fromStoreCertificate.GetRSAPrivateKey()) != null)
            {
                decryptedSecret = new ProtectedBinary(true, rsa.Decrypt(certificateSource.EncryptedSecret, RSAEncryptionPadding.OaepSHA256));
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

            return decryptedSecret;
        }
    }
}
