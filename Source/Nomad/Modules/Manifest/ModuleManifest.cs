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
    /// <para>
    /// To verify correctness of manifest, there have to be file AssemblyName.dll.manifest.asc with signature of manifest file. 
    /// If such file is missing, module won't be loaded.
    /// </para>
    /// <para>
    /// Application defines trusted issuers and their public keys. Name of issuer provided by module manifest must be defined by application.
    /// </para>
    /// </remarks>
    [Serializable]
    public class ModuleManifest
    {
        ///<summary>
        /// Initializes empty module manifest.
        ///</summary>
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
        /// At least each dll and exe file has to be signed, otherwise Nomad will not accept such module. Manifest signature is in separate file.
        /// </remarks>
        public List<SignedFile> SignedFiles { get; private set; }
    }
}