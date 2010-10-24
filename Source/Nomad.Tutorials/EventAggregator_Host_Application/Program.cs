using System;
using Nomad.Core;
using Nomad.Modules.Discovery;

namespace EventAggregator_Host_Application
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // using default configuration
            var kernel = new NomadKernel();

            // loading modules using single module discovery pattern

            var controllingModuleDiscovery =
                new SingleModuleDiscovery(@".\Modules\Controlling_Publisher_Module.dll");
            kernel.LoadModules(controllingModuleDiscovery);

            var simplePublisherModuleDiscovery =
                new SingleModuleDiscovery(@".\Modules\Simple_Publisher_Module.dll");
            kernel.LoadModules(simplePublisherModuleDiscovery);

            Console.ReadLine();
        }
    }
}