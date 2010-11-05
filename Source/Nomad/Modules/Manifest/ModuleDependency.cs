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

        /// <summary>
        ///     Stores information about relationship which is held with this dependency.
        /// </summary>
        /// <remarks>
        ///     This information is required to prepare a proper sorted list of modules to be loaded.
        ///     It provides the module loader with service-based dependency information.
        ///     
        ///     Using the ServiceLocator pattern with communication interface, service provider and service utilising modules 
        ///     each in a separate assembly requires them to be loaded in a proper manner: first the interface assembly following by
        ///     the implementation provider class (marked as HasLoadingOrderPriority) and finally the service utilizer. 
        ///     Any different order from given above would result in a ServiceNotFoundException being raised.
        /// 
        ///     EventAggregator service also requires for every subscribing module to be loaded prior to publishers,
        ///     this loading order protects the subscribers from missing any published event raised before they are loaded.
        ///     Therefore subscribing modules are also indirectly (not reference based) dependent and need to be marked as HasLoadingOrderPriority.
        ///  </remarks>
        public bool HasLoadingOrderPriority { get; set; }
    }
}