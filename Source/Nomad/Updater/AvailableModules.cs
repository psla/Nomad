using System.Collections.Generic;
using System.Runtime.Serialization;
using Nomad.Modules.Manifest;

namespace Nomad.Updater
{
    /// <summary>
    /// Complex class that represents available modules list. 
    /// </summary>
    /// <remarks>
    ///     Class is defined this way because of the <see cref="DataContract"/> constraints
    /// </remarks>
    [DataContract]
    public class AvailableModules
    {
        /// <summary>
        /// 
        /// </summary>
        public AvailableModules()
        {
        }


        /// <summary>
        ///     Initializes the instance of <see cref="AvailableModules"/> class.
        /// </summary>
        /// <param name="manifests">List of <see cref="ModuleManifest"/> </param>
        public AvailableModules(List<ModuleManifest> manifests)
        {
            Manifests = manifests;
        }


        /// <summary>
        /// List of available modules within repository.
        /// </summary>
        [DataMember]
        public List<ModuleManifest> Manifests { get; set; }
    }
}