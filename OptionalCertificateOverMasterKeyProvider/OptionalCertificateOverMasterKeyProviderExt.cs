using KeePass.Plugins;
using KeePass.UI;
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
using System.Windows.Forms;
using System.Xml.Linq;

namespace OptionalCertificateOverMasterKeyProvider
{
    public sealed class OptionalCertificateOverMasterKeyProviderExt : Plugin
    {
        private IPluginHost _host = null;
        private OptionalCertificateOverMasterKeyProvider _provider = new OptionalCertificateOverMasterKeyProvider();

        public override bool Initialize(IPluginHost host)
        {
            _host = host;

            _host.KeyProviderPool.Add(_provider);
            return true;
        }

        public override void Terminate()
        {
            _host.KeyProviderPool.Remove(_provider);
        }
    }

    public sealed class OptionalCertificateOverMasterKeyProvider : KeyProvider
    {

        public override string Name
        {
            get { return "Alternative Sources Key Provider"; }
        }

        public override byte[] GetKey(KeyProviderQueryContext ctx)
        {
            StrUtil.Utf8.GetBytes("test");

            var keyFilePath = UrlUtil.StripExtension(ctx.DatabasePath) + ".key";

            while (!File.Exists(keyFilePath))
            {
                var ofd = new OpenFileDialogEx("Select your key-file.");
                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    return null;
                }
                else
                {
                    keyFilePath = ofd.FileName;
                }
            }

            var rawKeyFileContent = File.ReadAllText(keyFilePath, Encoding.UTF8);
            var keyFileContent = XmlHelper.Deserialize<KeyFile>(rawKeyFileContent);

            var secretKey = GetSecretKey(keyFileContent);

            return secretKey?.ReadData();
        }

        private ProtectedBinary GetSecretKey(KeyFile keyFile)
        {
            ProtectedBinary secret = null;

            // try to get the secret with the more convenient method first:

            var certificateSource = keyFile.Sources.OfType<CertificateSource>().FirstOrDefault();
            if (certificateSource != null)
            {
                var certificateSourceFactory = new CertificateSourceFactory();
                try
                {
                    secret = certificateSourceFactory.DecryptSecret(certificateSource);
                    return secret;
                }
                catch
                {
                    secret = null;
                    // TODO?
                }
            }

            var passphraseSource = keyFile.Sources.OfType<PassphraseSource>().FirstOrDefault();
            if (passphraseSource != null)
            {
                var passphrase = PassphrasePromptForm.ShowDialogPrompt();

                try
                {
                    var passphraseSourceFactory = new PassphraseSourceFactory();
                    secret = passphraseSourceFactory.DecryptSecret(passphraseSource, passphrase);
                    return secret;
                }
                catch
                {
                    secret = null;
                    // TODO?
                }
            }

            return null;
        }
    }
}
