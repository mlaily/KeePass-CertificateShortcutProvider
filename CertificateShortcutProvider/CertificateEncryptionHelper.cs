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

namespace CertificateShortcutProvider
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

    public class CertificateEncryptionHelper
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

        public KeyFile EncryptSecret(X509Certificate2 certificate, ProtectedBinary secret)
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

            var result = new KeyFile(encryptedSecret, certificate);
            return result;
        }

        public ProtectedBinary DecryptSecret(KeyFile keyFile)
        {
            var fromSourceCertificate = keyFile.ReadCertificate();
            var fromStoreCertificate = LoadFromStore(fromSourceCertificate);

            ProtectedBinary decryptedSecret;

            RSA rsa;
            ECDsa ecdsa;
            if ((rsa = fromStoreCertificate.GetRSAPrivateKey()) != null)
            {
                decryptedSecret = new ProtectedBinary(true, rsa.Decrypt(keyFile.EncryptedSecret, RSAEncryptionPadding.OaepSHA256));
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
