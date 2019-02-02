using KeePass.Plugins;
using KeePassLib.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlternativeSourcesKeyProvider
{
    public sealed class AlternativeSourcesKeyProviderExt : Plugin
    {
        private IPluginHost _host = null;
        private SampleKeyProvider _prov = new SampleKeyProvider();

        public override bool Initialize(IPluginHost host)
        {
            _host = host;

            _host.KeyProviderPool.Add(_prov);
            return true;
        }

        public override void Terminate()
        {
            _host.KeyProviderPool.Remove(_prov);
        }
    }

    public sealed class SampleKeyProvider : KeyProvider
    {
        public override string Name
        {
            get { return "Sample Key Provider"; }
        }

        public override byte[] GetKey(KeyProviderQueryContext ctx)
        {
            // Return a sample key. In a real key provider plugin, the key
            // would be retrieved from smart card, USB device, ...
            return new byte[] { 2, 3, 5, 7, 11, 13 };
        }
    }
}
