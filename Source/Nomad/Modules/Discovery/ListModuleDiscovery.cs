using System;
using System.Collections.Generic;

namespace Nomad.Modules.Discovery
{
    /// <summary>
    ///     List discovery - provides the discovery of <see cref="ModuleInfo"/> provided in list.
    /// </summary>
    [Serializable]
    public class ListModuleDiscovery : IModuleDiscovery
    {
        private readonly ModuleInfo[] _moduleInfos;


        /// <summary>
        ///     Initializes the instance of <see cref="ListModuleDiscovery"/> class.
        /// </summary>
        /// <param name="moduleInfos">List of modules to be returned as discovery.</param>
        public ListModuleDiscovery(ModuleInfo[] moduleInfos)
        {
            _moduleInfos = moduleInfos;
        }

        #region IModuleDiscovery Members

        public IEnumerable<ModuleInfo> GetModules()
        {
            return _moduleInfos;
        }

        #endregion
    }
}