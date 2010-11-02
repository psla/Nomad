using System;
using System.Reflection;
using Version = Nomad.Utils.Version;

namespace Nomad.Modules.Manifest
{
    ///<summary>
    /// Contains information about dependency on separate module. Required by repository and updater
    ///</summary>
    [Serializable]
    public class ModuleDependency
    {
        /// <summary>
        /// ModuleName must be unique for application. Repository will be able to deliver proper module manifest depending on provided ModuleName.
        /// </summary>
        public string ModuleName { get; set; }

        //TODO: We may want to require not only minimal version of another module (or other requirements), but different validators. Talk about it.

        ///<summary>
        /// Minimal version of dependent module to cooperate with.
        ///</summary>
        public Version MinimalVersion { get; set; }

        /// <summary>
        ///     Target processor architecture of the module.
        /// </summary>
        public ProcessorArchitecture ProcessorArchitecture { get; set; }
    }
}