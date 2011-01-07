using System;
using System.IO;
using Nomad.Core;
using Nomad.Modules.Discovery;

namespace EventAggregator_Host_Application
{

    /// <summary>
    ///     Nomad's thin client that starts the whole framework based application.
    /// </summary>
    internal class Program
    {
        private static void Main()
        {
            // signing the assemblies and creating the manifest using manifestBuilder api
            GenerateManifestUsingApi("Controlling_Publisher_Module.dll", @".\Modules\ControllingPublisher");
            GenerateManifestUsingApi("Simple_Publisher_Module.dll", @".\Modules\SimplePublisher");


            // using default configuration
            var kernel = new NomadKernel();

            // loading modules using single module discovery pattern
            var discovery = new CompositeModuleDiscovery(
                new DirectoryModuleDiscovery(@".\Modules\ControllingPublisher", SearchOption.TopDirectoryOnly),
                new DirectoryModuleDiscovery(@".\Modules\SimplePublisher", SearchOption.TopDirectoryOnly));
            kernel.LoadModules(discovery);

            //wait for input
            Console.ReadLine();
        }




        private static void GenerateManifestUsingApi(string assemblyName, string path)
        {
            var builder = new Nomad.Utils.ManifestCreator.ManifestBuilder(@"TUTORIAL_ISSUER",
                                                                          @"..\..\KEY_FILE.xml",
                                                                          assemblyName,
                                                                          path);
            builder.CreateAndPublish();
        }
    }
}