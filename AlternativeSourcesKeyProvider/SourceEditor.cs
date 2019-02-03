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
}
