using System;
using Nomad.Core;
using Nomad.Modules.Discovery;

namespace Communicating_via_ServiceLocator_Program
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // using default configuration
            var kernel = new NomadKernel();

            // loading modules using single module discovery pattern

            var registeringModuleDiscovery =
                new SingleModuleDiscovery(@".\Modules\Registering_within_ServiceLocator_Module.dll");
            kernel.LoadModules(registeringModuleDiscovery);

            var serviceDependentModuleDiscovery =
                new SingleModuleDiscovery(@".\Modules\ServiceLocator_dependent_Module.dll");
            kernel.LoadModules(serviceDependentModuleDiscovery);

            Console.ReadLine();
        }
    }
}