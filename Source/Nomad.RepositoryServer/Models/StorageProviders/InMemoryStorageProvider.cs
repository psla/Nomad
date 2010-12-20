using System;
using System.Collections.Generic;

namespace Nomad.RepositoryServer.Models.StorageProviders
{
    /// <summary>
    ///     Simple storage provider used by me for testing far from database.
    /// </summary>
    public class InMemoryStorageProvider : IStorageProvider
    {
        private readonly List<IModuleInfo> _moduleInfos = new List<IModuleInfo>();
        private int _number = 0;


        public IEnumerable<IModuleInfo> GetAvaliableModules()
        {
            return _moduleInfos.AsReadOnly();
        }


        public void SaveModule(IModuleInfo moduleInfo)
        {
            // make the new id for newly saved thing
            var info = new ModuleInfo()
                                  {
                                      Id = GetNewId(),
                                      Manifest = moduleInfo.Manifest,
                                      ModuleData = moduleInfo.ModuleData,
                                  };
            _moduleInfos.Add(info);
        }


        private string GetNewId()
        {
            return string.Format("#{0}", ++_number);
        }


        public void RemoveModule(IModuleInfo moduleInfo)
        {
            _moduleInfos.Remove(moduleInfo);
        }
    }
}