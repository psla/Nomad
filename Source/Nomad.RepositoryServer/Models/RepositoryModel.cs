using System.Collections.Generic;

namespace Nomad.RepositoryServer.Models
{
    /// <summary>
    ///     Describes the state of the Nomad modules repository.
    /// </summary>
    /// <remarks>
    ///     TODO: implement better support injection of storageProvider mechanism
    /// </remarks>
    public class RepositoryModel
    {
        private IStorageProvider _storageProvider;

        public RepositoryModel(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;

            // initialize the list of modules using the storageProvider provider
            _moduleInfosList = _storageProvider.GetAvaliableModules();
        }


        private List<IModuleInfo> _moduleInfosList;

        public IList<IModuleInfo> ModuleInfosList
        {
            get { return _moduleInfosList.AsReadOnly(); }
        }

        public void AddModule(IModuleInfo moduleInfo)
        {
            // TODO: implement checking
            _moduleInfosList.Add(moduleInfo);
        }

        public void RemoveModule(IModuleInfo moduleInfo)
        {
            // TODO: implement checking
            _moduleInfosList.Remove(moduleInfo);
        }

    }
}