using System;
using System.Collections.Generic;
using System.IO;

namespace  Nomad.Modules.Discovery
{
    /* TODO: look for manifests instead of assemblies
     * 
     * Right now, DirectoryModuleDiscovery looks for all assemblies
     * in the specified directory. However, since many assemblies
     * there will not be modules, but modules' dependencies, this 
     * is not the best idea.
     * 
     * When manifests are added, we should look for them instead.
     * */

    /// <summary>
    ///     Discovers modules by enumerating all module files in an assembly
    /// </summary>
    public class DirectoryModuleDiscovery : IModuleDiscovery
    {
        private readonly string _directoryPath;


        /// <summary>
        ///     Initializes new instance of the <see cref="DirectoryModuleDiscovery"/>.
        /// </summary>
        /// <param name="directoryPath">Full or relative path to the directory with modules</param>
        /// <exception cref="ArgumentNullException">When <paramref name="directoryPath"/> is <c>null</c> or empty.</exception>
        public DirectoryModuleDiscovery(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
                throw new ArgumentException("directoryPath is required", "directoryPath");
            _directoryPath = directoryPath;
        }


        /// <summary>Inherited.</summary>
        public IEnumerable<ModuleInfo> GetModules()
        {
            var dllsInDirectory = Directory.GetFiles(_directoryPath, "*.dll");

            foreach (var dll in dllsInDirectory)
                yield return new ModuleInfo(dll);
        }
    }
}