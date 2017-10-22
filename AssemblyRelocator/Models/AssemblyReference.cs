using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyRelocator.Models
{
    [Serializable]
    [DebuggerDisplay("Name = {Info.Name}, Location = {Location}")]
    public class AssemblyReference : IEquatable<AssemblyReference>
    {
        public AssemblyName Info { get; }
        public Exception Exception { get; }
        public int ReferenceDepth { get; set; }

        public IReadOnlyCollection<AssemblyName> Sources { get; }
        public IEnumerable<string> SourceNames => Sources.Select(x => x.Name);

        public string FileName => Path.GetFileName(_location) ?? Info.Name;
        public string FullName => (Exception == null) ? Info.FullName : Exception.ToString();
        public string Location => _location ?? Exception.Message;
        public string LocationDetails => _location ?? Exception.ToString();

        public AssemblyReference(AssemblyName name, string location, Exception e)
        {
            Info = name;
            _location = location;
            Exception = e;
            Sources = _sources.AsReadOnly();
        }

        public void AddSource(AssemblyName name)
        {
            _sources.Add(name);
        }

        public bool Equals(AssemblyReference other)
        {
            if (_location != null && other._location != null)
            {
                return _location == other._location;
            }
            return false;
        }

        private string _location;
        private List<AssemblyName> _sources = new List<AssemblyName>();
    }
}
