using KeePass.UI;
using KeePassLib.Keys;
using KeePassLib.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CertificateShortcutProvider
{
    public sealed class CertificateShortcutProvider : KeyProvider
    {
        public const string DefaultKeyExtension = ".cspkey";

        public override string Name
        {
            get { return "Certificate Shortcut Provider"; }
        }

        public override bool SecureDesktopCompatible
        {
            get
            {
                return true;
            }
        }

        public override bool GetKeyMightShowGui
        {
            get
            {
                return true;
            }
        }

        public override byte[] GetKey(KeyProviderQueryContext ctx)
        {
            // The key file is expected to be next to the database by default:
            var keyFilePath = UrlUtil.StripExtension(ctx.DatabasePath) + DefaultKeyExtension;

            CertificateShortcutProviderKey cspKey;

            if (ctx.CreatingNewKey)
            {
                // Show the key file creation form and always return null,
                // so that it's not possible to accidentally create a composite key with this provider.

                using (var form = new KeyCreationForm(keyFilePath))
                {
                    form.ShowDialog();
                    return null;
                }
            }
            else
            {
                // We need to load an existing key file...

                while (!File.Exists(keyFilePath))
                {
                    var ofd = new OpenFileDialogEx("Select your Certificate Shortcut Provider Key file.");
                    ofd.Filter = $"Certificate Shortcut Provider Key files (*{DefaultKeyExtension})|*{DefaultKeyExtension}|All files (*.*)|*.*";
                    if (ofd.ShowDialog() != DialogResult.OK)
                    {
                        return null;
                    }
                    else
                    {
                        keyFilePath = ofd.FileName;
                    }
                }

                using (var fs = new FileStream(keyFilePath, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete))
                {
                    cspKey = XmlUtilEx.Deserialize<CertificateShortcutProviderKey>(fs);
                }

                var secretKey = CryptoHelpers.DecryptPassphrase(cspKey);

                return secretKey.ReadData();
            }
        }
    }
}
