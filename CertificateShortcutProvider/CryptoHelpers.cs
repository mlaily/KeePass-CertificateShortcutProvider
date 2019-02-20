using KeePassLib.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CertificateShortcutProvider
{
    public static class CryptoHelpers
    {
        public static CertificateShortcutProviderKey EncryptSecret(X509Certificate2 certificate, ProtectedString secret)
        {
            byte[] encryptedSecret;

            RSA rsa;
            ECDsa ecdsa;
            if ((rsa = certificate.GetRSAPublicKey()) != null)
            {
                //var csp = new RSACryptoServiceProvider();
                //csp.ImportParameters(rsa.ExportParameters(false));

                //encryptedSecret = csp.Encrypt(secret.ReadData(), RSAEncryptionPadding.OaepSHA256);

                encryptedSecret = rsa.Encrypt(secret.ReadUtf8(), RSAEncryptionPadding.OaepSHA256);
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

            var result = new CertificateShortcutProviderKey(encryptedSecret, certificate);
            return result;
        }

        public static ProtectedBinary DecryptSecret(CertificateShortcutProviderKey keyFile)
        {
            var fromSourceCertificate = keyFile.ReadCertificate();
            var fromStoreCertificate = LoadFromStore(fromSourceCertificate);

            if (fromStoreCertificate == null)
            {
                throw new InvalidOperationException("The specified certificate could not be found in the store.");
            }

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

        public static X509Certificate2 LoadFromStore(X509Certificate2 certificate)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            using (store)
            {
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                var result = store.Certificates.Find(X509FindType.FindByThumbprint, certificate.Thumbprint, false);
                return result.Cast<X509Certificate2>().SingleOrDefault();
            }
        }
    }
}
