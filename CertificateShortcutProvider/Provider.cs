using KeePass.UI;
using KeePassLib.Keys;
using KeePassLib.Utility;

namespace CertificateShortcutProvider;

public sealed class CertificateShortcutKeyProvider : KeyProvider
{
    public const string DefaultKeyExtension = ".cspkey";

    public override string Name
    {
        get { return "Certificate Shortcut Provider"; }
    }

    public override bool SecureDesktopCompatible => true;

    public override bool GetKeyMightShowGui => true;

    public override byte[] GetKey(KeyProviderQueryContext ctx)
    {
        if (ctx == null) throw new ArgumentNullException(nameof(ctx));

        // The key file is expected to be next to the database by default:
        var keyFilePath = UrlUtil.StripExtension(ctx.DatabasePath) + DefaultKeyExtension;

        CertificateShortcutProviderKey cspKey;

        if (ctx.CreatingNewKey)
        {
            // Always return null, so that it's not possible to accidentally create a composite key with this provider.
            MessageBox.Show("Certificate Shortcut Provider uses the encrypted master key to access the database. There is no initialization needed other than the master key.\n\nUse 'Options > Initialize Certificate Shortcut Provider...' from the menu to create the encrypted master key.", Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return null;
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
