using KeePass.Plugins;
using KeePassLib.Keys;
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
            get { return "Alternative Sources Key Provider"; }
        }

        public override byte[] GetKey(KeyProviderQueryContext ctx)
        {
            if (ctx.CreatingNewKey)
            {
             

            }




      
            return null;
        }
    }
}
