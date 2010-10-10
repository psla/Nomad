using System;

namespace Nomad.Modules.Manifest
{
    /// <summary>
    /// Contains signature for manifest with matching filename.
    /// </summary>
    /// <example>
    /// There is assembly.dll.manifest file, which signature is placed in assembly.dll.manifest.asc file
    /// </example>
    [Serializable]
    public class ManifestSignature
    {
        ///<summary>
        /// Signature of manifest. Issuer have to be the same as <see cref="ModuleManifest.Issuer"/>
        ///</summary>
        public string Signature { get; set; }
    }
}