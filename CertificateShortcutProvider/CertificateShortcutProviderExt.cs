using KeePass.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
