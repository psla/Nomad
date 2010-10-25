using System;
using Nomad.Core;
using Nomad.Modules.Discovery;

namespace EventAggregator_Host_Application
{

    /// <summary>
    ///     Nomad's thin client that starts the whole framework based application.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            // using default configuration
            var kernel = new NomadKernel();

            // loading module of listener using simplest possible discovery pattern
            var controllingModuleDiscovery =
                new SingleModuleDiscovery(@".\Modules\Controlling_Publisher_Module.dll");
            kernel.LoadModules(controllingModuleDiscovery);

            //loading module of publisher using simplest possible discovery pattern
            var simplePublisherModuleDiscovery =
                new SingleModuleDiscovery(@".\Modules\Simple_Publisher_Module.dll");
            kernel.LoadModules(simplePublisherModuleDiscovery);

            //wait for input
            Console.ReadLine();
        }
    }
}