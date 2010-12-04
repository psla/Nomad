using System;
using System.Collections.Generic;

namespace Nomad.Modules.Discovery
{
    /// <summary>
    ///     Discovers single assembly with the path provided.
    /// </summary>
    /// <remarks>
    ///     Does not check for manifest file neither completeness of the resources or other files. 
    ///     Discovery simply provides the mean for loading pinpointed file.
    /// </remarks>
    [Serializable]
    public class SingleModuleDiscovery : IModuleDiscovery
    {
        private readonly ModuleInfo _moduleInfo;


        /// <summary>
        ///     Initializes the instance of  <see cref="SingleModuleDiscovery"/> class.
        /// </summary>
        /// <param name="path">Path to the assembly to be loaded.</param>
        public SingleModuleDiscovery(string path)
        {
            _moduleInfo = new ModuleInfo(path);
        }

        #region IModuleDiscovery Members

        /// <summary>
        ///     Inherited from <see cref="IModuleDiscovery"/>
        /// </summary>
        public IEnumerable<ModuleInfo> GetModules()
        {
            return new List<ModuleInfo> { _moduleInfo }.AsReadOnly();
        }

        #endregion
    }
}