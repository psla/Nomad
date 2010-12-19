using System;
using System.Threading;
using Nomad.Core;
using Nomad.Modules.Discovery;
using Nomad.Utils.ManifestCreator;

namespace ThinWpfHost
{
    internal class Program
    {
        [STAThread]
        private static void Main()
        {
            // signing the assemblies and creating the manifest using manifestBuilder api
            GenerateManifestUsingApi("WpfApplicationModule.exe", @".\Modules\WpfApplication");

            // using default configuration
            var kernel = new NomadKernel();

            // loading modules using single module discovery pattern
            var discovery = new CompositeModuleDiscovery(
                new SingleModuleDiscovery(@".\Modules\WpfApplication\WpfApplicationModule.exe")
                );

            var thread = new Thread(() => kernel.LoadModules(discovery));
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = false;
            thread.Start();

            //wait for input
            //Console.ReadLine();

            //simulate reloading :]
            Thread.Sleep(15000);

            kernel.UnloadModules();

            //TODO: Here we might to try to substitute dll with new version to see if it was really unloaded

            Thread.Sleep(2000);

            thread = new Thread((ThreadStart)delegate { kernel.LoadModules(discovery); });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            Thread.Sleep(5000);

            //TODO: Here we should wait for event "onclose", because we are going to loose nomad ;)
        }


        private static void GenerateManifestUsingApi(string assemblyName, string path)
        {
            var builder = new ManifestBuilder(@"TUTORIAL_ISSUER",
                                              @"..\..\KEY_FILE.xml",
                                              assemblyName,
                                              path);
            builder.CreateAndPublish();
        }
    }
}