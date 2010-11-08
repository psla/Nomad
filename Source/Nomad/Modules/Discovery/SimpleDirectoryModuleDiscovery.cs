using System;
using System.Collections.Generic;
using System.IO;

namespace  Nomad.Modules.Discovery
{
    
    /// <summary>
    ///     Discovers modules by enumerating all module files in an assembly
    /// </summary>
    /// <remarks>
    ///     Treats all assemblies in the pinpointed folders as modules.
    /// </remarks>
    public class SimpleDirectoryModuleDiscovery : IModuleDiscovery
    {
        private readonly string _directoryPath;


        /// <summary>
        ///     Initializes new instance of the <see cref="SimpleDirectoryModuleDiscovery"/>.
        /// </summary>
        /// <param name="directoryPath">Full or relative path to the directory with modules</param>
        /// <exception cref="ArgumentNullException">When <paramref name="directoryPath"/> is <c>null</c> or empty.</exception>
        public SimpleDirectoryModuleDiscovery(string directoryPath)
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