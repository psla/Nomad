using System;
using System.Threading;
using Nomad.Core;
using Nomad.Modules.Discovery;
using Nomad.Utils.ManifestCreator;

namespace ThinWpfHost
{
    /// <summary>
    ///     Sample WpfApplication loader 
    /// </summary>
    /// <remarks>
    ///     This tutorial shows how to load & unload & load again wpf application.
    ///     Second part of this tutorial shows how to use updater module.
    /// </remarks>
    internal class Program
    {
        [STAThread]
        private static void Main()
        {
            // signing the assemblies and creating the manifest using manifestBuilder api
            GenerateManifestUsingApi("WpfApplicationModule.exe", @".\Modules\WpfApplication");
            GenerateManifestUsingApi("WpfButtonModule.dll", @".\Modules\WpfButton");
            GenerateManifestUsingApi("WpfUpdaterModule.dll", @".\Modules\WpfUpdater");

            // using default configuration
            var kernel = new NomadKernel();

            // loading modules using single module discovery pattern
            var discovery = new CompositeModuleDiscovery(
                new SingleModuleDiscovery(@".\Modules\WpfApplication\WpfApplicationModule.exe"),
                new SingleModuleDiscovery(@".\Modules\WpfButton\WpfButtonModule.dll"),
                new SingleModuleDiscovery(@".\Modules\WpfUpdater\WpfUpdaterModule.dll")
                );

            kernel.LoadModules(discovery);

            // FIXME: what about ending the thread here ?
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