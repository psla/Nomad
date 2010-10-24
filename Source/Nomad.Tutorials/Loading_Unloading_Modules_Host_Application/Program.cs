using System;
using Nomad.Core;
using Nomad.Modules.Discovery;

namespace Loading_Unloading_Modules_Host_Application
{
    
    internal class Program
    {
        private static void Main(string[] args)
        {
            // using default configuration
            var kernel = new NomadKernel();

            // loading modules using directory module discovery pattern
            var discovery = new DirectoryModuleDiscovery(@".\Modules\");

            // subscribing for load/unload events
            kernel.ModuleAppDomain.AssemblyLoad +=
                (sender, moduleArgs) =>
                Console.WriteLine(String.Format("Module Loaded: {0}",
                                                moduleArgs.LoadedAssembly.FullName));

            kernel.ModuleAppDomain.DomainUnload +=
                (sender, domainArgs) => Console.WriteLine("Domain Unloaded");

            // loading items
            kernel.LoadModules(discovery);

            Console.ReadLine();

            // unloading events
            kernel.UnloadModules();

            Console.ReadLine();
        }
    }
}