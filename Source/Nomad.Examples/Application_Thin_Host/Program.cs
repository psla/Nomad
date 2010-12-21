using Nomad.Core;
using Nomad.Modules.Discovery;

namespace Application_Thin_Host
{
    /// <summary>
    ///     Bootstrapper code for big application
    /// </summary>
    internal class Program
    {
        private static void Main()
        {
            // use default configuration for kernel
            var kernel = new NomadKernel();

            // load the Application_WPF_Shell
            kernel.LoadModules(new SingleModuleDiscovery(@".\Application_WPF_Shell\Application_WPF_Shell.exe"));

            kernel.LoadModules(new CompositeModuleDiscovery(
                new SingleModuleDiscovery(@".\FileLoaderModule\FileLoaderModule.dll"),
                new SingleModuleDiscovery(@".\GraphicFilesHandlerModule\GraphicFilesHandlerModule.dll"),
                new SingleModuleDiscovery(@".\TextFileHandlerModule\TextFileHandlerModule.dll")
                ));
        }
    }
}