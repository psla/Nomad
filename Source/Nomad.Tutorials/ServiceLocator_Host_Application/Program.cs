using System;
using Nomad.Core;
using Nomad.Modules.Discovery;
using Nomad.Utils.ManifestCreator;

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
            // FIXME(i dont like this) this step should be omitted in real life scenarios
            GenerateManifestUsingApi("Registering_within_ServiceLocator_Module.dll",
                                     @".\Modules\RegisteringModule");
            GenerateManifestUsingApi("ServiceLocator_dependent_Module.dll",
                                     @".\Modules\DependantModule");

            // using default configuration
            var kernel = new NomadKernel();

            // loading modules using single module discovery pattern
            var discovery = new CompositeModuleDiscovery(
                new DirectoryModuleDiscovery(@".\Modules\RegisteringModule"),
                new DirectoryModuleDiscovery(@".\Modules\DependantModule"));
            kernel.LoadModules(discovery);

            //wait
            Console.ReadLine();
        }


        private static void GenerateManifestUsingApi(string assemblyName, string path)
        {
            var builder = new ManifestBuilder(@"TUTORIAL_ISSUER",
                                              @"..\..\KEY_FILE.xml",
                                              assemblyName,
                                              path);
            builder.Create();
        }
    }
}