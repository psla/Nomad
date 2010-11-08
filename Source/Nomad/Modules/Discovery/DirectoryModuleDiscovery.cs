using System;
using System.Collections.Generic;
using System.IO;
using Nomad.Modules.Manifest;

namespace Nomad.Modules.Discovery
{
    /// <summary>
    ///      Discovers modules by enumerating all module files in an assembly
    /// </summary>
    /// <remarks>
    ///     Discovers only the modules that are described with proper  <see cref="ModuleManifest"/> file 
    /// inferred from the default <see cref="IModuleManifestFactory"/>. 
    /// </remarks>
    public class DirectoryModuleDiscovery : IModuleDiscovery
    {
        private readonly string _directoryPath;


        /// <summary>
        ///     Initializes new instance of the <see cref="SimpleDirectoryModuleDiscovery"/>.
        /// </summary>
        /// <param name="directoryPath">Full or relative path to the directory with modules</param>
        /// <exception cref="ArgumentNullException">When <paramref name="directoryPath"/> is <c>null</c> or empty.</exception>
        public DirectoryModuleDiscovery(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
                throw new ArgumentException("directoryPath is required", "directoryPath");

            _directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, directoryPath);
        }

        #region Implementation of IModuleDiscovery

        /// <summary>
        ///     Inherited.
        /// </summary>
        public IEnumerable<ModuleInfo> GetModules()
        {
            string[] dllsInDirectory = Directory.GetFiles(_directoryPath, "*.dll");

            foreach (string dll in dllsInDirectory)
            {
                var moduleInfo = new ModuleInfo(dll, ModuleInfo.DefaultFactory);

                ModuleManifest manifest;
                try
                {
                    manifest = moduleInfo.Manifest;
                }
                catch (Exception)
                {
                    // modules with exception upon getting the manifest are disqualified
                    continue;
                }

                // modules with null manifest are also disqualified
                if (manifest != null)
                    yield return moduleInfo;
            }
        }

        #endregion
    }
}