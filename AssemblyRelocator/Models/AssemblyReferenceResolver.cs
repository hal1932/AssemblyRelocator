using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyRelocator.Models
{
    class AssemblyReferenceResolver
    {
        public async Task<AssemblyReference[]> ResolveReferences(string path)
        {
            return await Task.Run(() =>
            {
                AssemblyReference[] result;

                var domainSetup = new AppDomainSetup() { ApplicationBase = Path.GetDirectoryName(path) };
                var domain = AppDomain.CreateDomain(Guid.NewGuid().ToString(), null, domainSetup);
                {
                    var proxy = (AssemblyProxy)domain.CreateInstanceFromAndUnwrap(
                        Assembly.GetExecutingAssembly().Location,
                        typeof(AssemblyProxy).FullName);

                    result = proxy.ResolveReferences(path);
                }
                AppDomain.Unload(domain);

                return result;
            })
            .ConfigureAwait(false);
        }
    }
}
