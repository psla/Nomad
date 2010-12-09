using System.Collections.Generic;

namespace Nomad.RepositoryServer.Models
{
    public interface IStorageProvider
    {
        List<IModuleInfo> GetAvaliableModules();

        bool SaveModule(IModuleInfo moduleInfo);

        bool RemoveModule(IModuleInfo moduleInfo);
    }
}