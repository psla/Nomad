using System;

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

        //TODO: We may want to require minimal version of another module (or other requirements)
    }
}