using KeePass.Plugins;
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
    public sealed class CertificateShortcutProviderExt : Plugin
    {
        private IPluginHost _host = null;
        private CertificateShortcutProvider _provider = new CertificateShortcutProvider();

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

        public override ToolStripMenuItem GetMenuItem(PluginMenuType t)
        {
            if (t == PluginMenuType.Main)
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem();
                tsmi.Text = "Initialize Certificate Shortcut Provider...";
                tsmi.Click += this.onOptionsClicked;
                return tsmi;
            }
            return null;
        }

        private void onOptionsClicked(object sender, EventArgs e)
        {
            var databasePath = UrlUtil.StripExtension(_host.Database.IOConnectionInfo.Path);

            if (string.IsNullOrWhiteSpace(databasePath))
            {
                databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Database");
            }

            var keyFilePath = databasePath + CertificateShortcutProvider.DefaultKeyExtension;

            using (var form = new KeyCreationForm(keyFilePath))
            {
                form.ShowDialog();
            }
        }
    }
}
