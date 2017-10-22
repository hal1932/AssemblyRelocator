using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyRelocator.Models
{
    public class AssemblyProxy : MarshalByRefObject
    {
        struct _Item
        {
            public Assembly Assembly;
            public int Depth;
        }

        public AssemblyReference[] ResolveReferences(string path)
        {
            var result = new List<AssemblyReference>();

            var items = new Queue<_Item>();
            items.Enqueue(new _Item() { Assembly = Assembly.LoadFile(path), Depth = 0 });

            while (items.Any())
            {
                var item = items.Dequeue();
                foreach (var referenceName in item.Assembly.GetReferencedAssemblies())
                {
                    Assembly referencedAssembly = null;
                    Exception ex = null;

                    try
                    {
                        referencedAssembly = Assembly.Load(referenceName);
                    }
                    catch (Exception e)
                    {
                        ex = e;
                    }

                    var reference = new AssemblyReference(referenceName, referencedAssembly?.Location, ex);
                    var found = result.FirstOrDefault(x => x.Equals(reference));
                    if (found != null)
                    {
                        found.AddSource(item.Assembly.GetName());
                    }
                    else
                    {
                        reference.AddSource(item.Assembly.GetName());
                        reference.ReferenceDepth = item.Depth + 1;

                        result.Add(reference);

                        if (referencedAssembly != null)
                        {
                            items.Enqueue(new _Item() { Assembly = referencedAssembly, Depth = item.Depth + 1 });
                        }
                    }
                }
            }

            return result.ToArray();
        }
    }
}
