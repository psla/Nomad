using System.Collections.Generic;

namespace Nomad.RepositoryServer.Models
{
    /// <summary>
    ///     Defines the way of accessing data repository, storage.
    /// </summary>
    public interface IStorageProvider
    {
        IEnumerable<IModuleInfo> GetAvaliableModules();

        void SaveModule(IModuleInfo moduleInfo);

        void RemoveModule(IModuleInfo moduleInfo);
    }
}