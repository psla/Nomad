using System;
using System.Collections.Generic;

namespace Nomad.RepositoryServer.Models.StorageProviders
{
    public class InMemoryStorageProvider : IStorageProvider
    {
        private readonly List<IModuleInfo> _moduleInfos = new List<IModuleInfo>();

        public IEnumerable<IModuleInfo> GetAvaliableModules()
        {
            return _moduleInfos.AsReadOnly();
        }


        public void SaveModule(IModuleInfo moduleInfo)
        {
            _moduleInfos.Add(moduleInfo);
        }


        public void RemoveModule(IModuleInfo moduleInfo)
        {
            _moduleInfos.Remove(moduleInfo);
        }
    }
}