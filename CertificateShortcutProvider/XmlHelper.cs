using KeePassLib.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertificateShortcutProvider
{
    public static class XmlHelper
    {
        public static string Serialize<T>(T t)
        {
            using (var ms = new MemoryStream())
            {
                XmlUtilEx.Serialize(ms, t);
                // XmlUtilEx uses UTF8
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public static T Deserialize<T>(string xml)
        { // XmlUtilEx uses UTF8
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                return XmlUtilEx.Deserialize<T>(ms);
            }
        }
    }
}
