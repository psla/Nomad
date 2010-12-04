using System;
using System.Runtime.Serialization;
using Nomad.Modules.Manifest;

namespace Nomad.Updater
{
    /// <summary>
    ///     Information about package in repository.
    /// </summary>
    [DataContract]
    public class ModulePackage
    {
        /// <summary>
        ///     Contains package with all files of module. In zip there should also be contained manifest.
        /// </summary>
        [DataMember]
        public byte[] ModuleZip { get; set; } //TODO: This might eat a lot of memory when big big huge modules

        /// <summary>
        ///     Contains module manifest, accessible without decompressing zip
        /// </summary>
        [DataMember]
        public ModuleManifest ModuleManifest {get; set;}
    }
}