using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AssemblyRelocator.Models
{
    class ILSpyProxy
    {
        public static Process Open(params string[] assemblies)
        {
            var config = XElement.Load("Config.xml");
            var ilspyPath = config.Element("ILSpyPath")?.Value;
            if (!File.Exists(ilspyPath))
            {
                return null;
            }

            var args = string.Join(" ", assemblies.Select(x => $"\"{x}\"").ToArray());
            return Process.Start(ilspyPath, args);
        }
    }
}
