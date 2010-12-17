using System;
using System.Collections.Generic;
using System.Linq;

namespace Nomad.RepositoryServer.Models
{
    public class InMemoryStorageProvider : IStorageProvider

    {
        private readonly IList<IModuleInfo> _modulesInfos = new List<IModuleInfo>();

        #region IStorageProvider Members

        public IEnumerable<IModuleInfo> GetAvaliableModules()
        {
            return _modulesInfos;
        }


        public void SaveModule(IModuleInfo moduleInfo)
        {
            IModuleInfo existingItem =
                _modulesInfos.Where(x => x.Id == moduleInfo.Id).FirstOrDefault();
            if (existingItem != null)
                _modulesInfos.Remove(existingItem);
            _modulesInfos.Add(moduleInfo);
        }


        public void RemoveModule(IModuleInfo moduleInfo)
        {
            IModuleInfo existingItem =
                _modulesInfos.Where(x => x.Id == moduleInfo.Id).FirstOrDefault();

            if (existingItem != null)
                _modulesInfos.Remove(existingItem);
            else
                throw new InvalidOperationException("Specified item does not exist!");
        }

        #endregion
    }
}