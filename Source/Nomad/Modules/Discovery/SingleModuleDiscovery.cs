using System.Collections.Generic;

namespace Nomad.Modules.Discovery
{
    public class SingleModuleDiscovery : IModuleDiscovery
    {
        private readonly ModuleInfo _moduleInfo;


        public SingleModuleDiscovery(string path)
        {
            _moduleInfo = new ModuleInfo(path);
        }

        #region IModuleDiscovery Members

        public IEnumerable<ModuleInfo> GetModules()
        {
            return new List<ModuleInfo> {_moduleInfo}.AsReadOnly();
        }

        #endregion
    }
}