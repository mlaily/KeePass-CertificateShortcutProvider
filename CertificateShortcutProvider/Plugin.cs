using KeePass.Plugins;
using KeePassLib.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// The namespace must be named like the DLL file without extension.
namespace CertificateShortcutProvider
{
    // The main plugin class (which KeePass will instantiate when it loads your plugin) must be called exactly the same as the namespace plus "Ext". 
    public sealed class CertificateShortcutProviderExt : Plugin
    {
        private IPluginHost _host = null;
        private readonly CertificateShortcutKeyProvider _provider = new CertificateShortcutKeyProvider();

        /// <summary>
        /// The <c>Initialize</c> method is called by KeePass when
        /// you should initialize your plugin.
        /// </summary>
        /// <param name="host">Plugin host interface. Through this
        /// interface you can access the KeePass main window, the
        /// currently opened database, etc.</param>
        /// <returns>You must return <c>true</c> in order to signal
        /// successful initialization. If you return <c>false</c>,
        /// KeePass unloads your plugin (without calling the
        /// <c>Terminate</c> method of your plugin).</returns>
        public override bool Initialize(IPluginHost host)
        {
            if (host == null) return false; // Fail; we need the host

            _host = host;

            _host.KeyProviderPool.Add(_provider);

            return true;
        }

        /// <summary>
        /// The <c>Terminate</c> method is called by KeePass when
        /// you should free all resources, close files/streams,
        /// remove event handlers, etc.
        /// </summary>
        public override void Terminate()
        {
            _host.KeyProviderPool.Remove(_provider);
        }

        /// <summary>
        /// Get a menu item of the plugin. See
        /// https://keepass.info/help/v2_dev/plg_index.html#co_menuitem
        /// </summary>
        /// <param name="t">Type of the menu that the plugin should
        /// return an item for.</param>
        public override ToolStripMenuItem GetMenuItem(PluginMenuType t)
        {
            if (t == PluginMenuType.Main)
            {
                var tsmi = new ToolStripMenuItem();
                tsmi.Text = "Initialize Certificate Shortcut Provider...";
                tsmi.Click += OnOptionsClicked;
                return tsmi;
            }
            return null;
        }

        private void OnOptionsClicked(object sender, EventArgs e)
        {
            var databasePath = UrlUtil.StripExtension(_host.Database.IOConnectionInfo.Path);

            if (string.IsNullOrWhiteSpace(databasePath))
            {
                databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Database");
            }

            var keyFilePath = databasePath + CertificateShortcutKeyProvider.DefaultKeyExtension;

            using (var form = new KeyCreationForm(keyFilePath))
            {
                form.ShowDialog();
            }
        }
    }
}
