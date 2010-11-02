using System.Collections.Generic;

namespace Nomad.Modules.Discovery
{
    /// <summary>
    ///     Provides composite pattern for <see cref="IModuleDiscovery"/>.
    /// </summary>
    public class CompositeModuleDiscovery : IModuleDiscovery
    {
        private readonly IModuleDiscovery[] _moduleDiscoveries;

        /// <summary>
        ///     Initializes the instance of <see cref="CompositeModuleDiscovery"/> class.
        /// </summary>
        /// <param name="moduleDiscoveries">Array of <see cref="IModuleDiscovery"/> objects to configure the discovery.</param>
        public CompositeModuleDiscovery(params IModuleDiscovery[] moduleDiscoveries)
        {
            _moduleDiscoveries = moduleDiscoveries;
        }

        #region Implementation of IModuleDiscovery

        /// <summary>
        ///     Inherited.
        /// </summary>
        public IEnumerable<ModuleInfo> GetModules()
        {
            var result = new List<ModuleInfo>();

            foreach (IModuleDiscovery moduleDiscovery in _moduleDiscoveries)
            {
                foreach (ModuleInfo moduleInfo in moduleDiscovery.GetModules())
                {
                    if (!result.Contains(moduleInfo))
                        result.Add(moduleInfo);
                }
            }

            return result;
        }

        #endregion
    }
}