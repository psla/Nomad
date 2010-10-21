using System;
using System.Collections.Generic;
using Nomad.Signing.FileUtils;
using Nomad.Utils;
using Version = Nomad.Utils.Version;

namespace Nomad.Modules.Manifest
{
    /// <summary>
    /// Manifest contains additional information about module, especially:
    ///     files signature
    ///     dependencies information
    ///     repository url
    /// </summary>
    /// <remarks>
    /// <para>
    /// Application defines trusted issuers and their public keys. Name of issuer provided by module manifest must be defined by application.
    /// </para>
    /// </remarks>
    [Serializable]
    public class ModuleManifest
    {
        ///<summary>
        /// Suffix added to AssemblyName to get manifest file
        ///</summary>
        public const string ManifestFileNameSuffix = ".manifest";

        ///<summary>
        /// Suffix added to Manifest File Name to get manifest signature file
        ///</summary>
        public const string ManifestSignatureFileNameSuffix = ".asc";


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
        public string ModuleName { get; set; }

        /// <summary>
        /// Version of module
        /// </summary>
        public Version ModuleVersion { get; set; }

        ///<summary>
        /// List of modules this one is dependent on
        ///</summary>
        public List<ModuleDependency> ModuleDependencies { get; set; }

        ///<summary>
        /// List of files, which have to be signed.
        ///</summary>
        /// <remarks>
        /// At least each dll and exe file has to be signed, otherwise Nomad will not accept such module. Manifest signature is in separate file.
        /// </remarks>
        public List<SignedFile> SignedFiles { get; set; }
    }
}