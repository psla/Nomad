using System;
using Nomad.Core;
using Nomad.Modules.Discovery;

namespace ServiceLocator_Host_Application
{
    /// <summary>
    ///     Nomad's thin client that starts the whole framework based application.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            // signing the assemblies and creating the manifest using manifestBuilder api
            GenerateManifestUsingApi("Registering_within_ServiceLocator_Module.dll", @".\Modules\RegisteringModule");
            GenerateManifestUsingApi("ServiceLocator_dependent_Module.dll", @".\Modules\DependantModule");


            // using default configuration
            var kernel = new NomadKernel();

            // loading modules using single module discovery pattern
            var registeringModuleDiscovery =
                new SingleModuleDiscovery(@".\Modules\Registering_within_ServiceLocator_Module.dll");
            kernel.LoadModules(registeringModuleDiscovery);

            var serviceDependentModuleDiscovery =
                new SingleModuleDiscovery(@".\Modules\ServiceLocator_dependent_Module.dll");
            kernel.LoadModules(serviceDependentModuleDiscovery);

            //wait
            Console.ReadLine();
        }

        private static void GenerateManifestUsingApi(string assemblyName, string path)
        {
            var builder = new Nomad.Utils.ManifestCreator.ManifestBuilder(@"TUTORIAL_ISSUER",
                                                                          @"..\..\KEY_FILE.xml",
                                                                          assemblyName,
                                                                          path);
            builder.Create();
        }
    }
}