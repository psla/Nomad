using System;
using Nomad.Core;
using Nomad.Modules.Discovery;

namespace Loading_Unloading_Modules_Host_Application
{
    /// <summary>
    ///     Nomad's thin client that starts the whole framework based application.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            // signing the assembly and creating the manifest using manifestBuilder api
            GenerateManifestUsingApi();

            // using default configuration
            var kernel = new NomadKernel();

            // loading modules using directory module discovery pattern
            var discovery = new SimpleDirectoryModuleDiscovery(@".\Modules\");

            // loading discovered modules
            kernel.LoadModules(discovery);

            //wait for input
            Console.ReadLine();

            // unloading all modules
            kernel.UnloadModules();

            //wait for input
            Console.ReadLine();
        }


        private static void GenerateManifestUsingApi()
        {
            var builder = new Nomad.Utils.ManifestCreator.ManifestBuilder(@"TUTORIAL_ISSUER",
                                                                          @"..\..\KEY_FILE.xml",
                                                                          @"Tutorial_01_Example_Module.dll",
                                                                          @".\Modules\");
            builder.CreateAndPublish();
        }
    }
}