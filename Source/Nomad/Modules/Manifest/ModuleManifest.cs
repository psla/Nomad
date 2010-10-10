using System;
using System.Collections.Generic;
using Nomad.Modules.Manifest;
using Version = Nomad.Modules.Manifest.Version;

namespace Nomad.Modules
{
    /// <summary>
    /// Manifest contains additional information about module, especially:
    ///     files signature
    ///     dependencies information
    ///     repository url
    /// </summary>
    /// <remarks>
    ///     <para>Module Manifest has to be saved as AssemblyName.dll.manifest. 
    /// When module manifest is missing, then module won't be loaded. 
    /// If some signatures for dll, exe or manifest will be missing, module won't be loaded.
    /// </para>
    /// </remarks>
    [Serializable]
    public class ModuleManifest
    {
        public ModuleManifest()
        {
            ModuleDependencies = new List<ModuleDependency>();
            SignedFiles = new List<SignedFile>();
        }


        ///<summary>
        /// Unique issuer name. Application contains defined key for such issuer and validates all signatures against this key.
        /// Issuer name must be unique.
        ///</summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Unique for repository module name
        /// </summary>
        public string ModuleName { get; private set; }

        /// <summary>
        /// Version of module
        /// </summary>
        public Version ModuleVersion { get; private set; }

        ///<summary>
        /// List of modules this one is dependent on
        ///</summary>
        public List<ModuleDependency> ModuleDependencies { get; private set; }

        ///<summary>
        /// List of files, which have to be signed.
        ///</summary>
        /// <remarks>
        /// At least each dll, exe file has to be signed, otherwise Nomad will not accept such module. 
        /// TODO: Signing manifest (separate file with manifest signature?)
        /// </remarks>
        public List<SignedFile> SignedFiles { get; private set; }
    }
}