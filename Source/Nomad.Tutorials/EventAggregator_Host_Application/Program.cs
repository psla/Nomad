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

            // copy the modules to modules catalogue
           // CopyFilesIntoModulesPlaces(@".\Modules\ControllingPublisher");
           // CopyFilesIntoModulesPlaces(@".\Modules\SimplePublisher");


            // using default configuration
            var kernel = new NomadKernel();

            // loading module of listener using simplest possible discovery pattern
            var controllingModuleDiscovery =
                new SingleModuleDiscovery(@".\Modules\ControllingPublisher\Controlling_Publisher_Module.dll");
            kernel.LoadModules(controllingModuleDiscovery);

            //loading module of publisher using simplest possible discovery pattern
            var simplePublisherModuleDiscovery =
                new SingleModuleDiscovery(@".\Modules\SimplePublisher\Simple_Publisher_Module.dll");
            kernel.LoadModules(simplePublisherModuleDiscovery);

            //wait for input
            Console.ReadLine();
        }


        private static void CopyFilesIntoModulesPlaces(string s)
        {
            foreach (var file in Directory.GetFiles(s)) 
            {
                File.Copy(file,Path.Combine(@".",Path.GetFileName(file)),true);
            }
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