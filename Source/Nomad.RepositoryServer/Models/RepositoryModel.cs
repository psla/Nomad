using System;
using System.Collections.Generic;
using Nomad.KeysGenerator;
using Nomad.Modules.Manifest;
using Nomad.Signing;
using Nomad.Utils.ManifestCreator;

namespace Nomad.RepositoryServer.Models
{
    /// <summary>
    ///     Describes the state of the Nomad modules repository.
    /// </summary>
    /// <remarks>
    ///    Uses <see cref="IStorageProvider"/> mechanism to access the data.
    /// </remarks>
    public class RepositoryModel
    {

        private readonly List<IModuleInfo> _moduleInfosList;
        private readonly IStorageProvider _storageProvider;


        public RepositoryModel(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;

            // initialize the list of modules using the storageProvider provider
            _moduleInfosList = _storageProvider.GetAvaliableModules();

        }


        public IEnumerable<IModuleInfo> ModuleInfosList
        {
            get { return _moduleInfosList.AsReadOnly(); }
        }


        public void AddModule(IModuleInfo moduleInfo)
        {
            // TODO: implement checking
            // check for completeness of the provided thing

            // check for duplications

            _moduleInfosList.Add(moduleInfo);

            // save into storage
        }


        public void RemoveModule(IModuleInfo moduleInfo)
        {
            // TODO: implement checking

            // check for existence
            _moduleInfosList.Remove(moduleInfo);

            // remove from storage
        }

    }
}