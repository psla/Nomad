using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nomad.KeysGenerator;
using Nomad.Modules.Manifest;
using Nomad.Signing;
using Nomad.Utils.ManifestCreator;

namespace Nomad.RepositoryServer.Models
{
    /// <summary>
    ///     Logic for getting all things working.
    /// </summary>
    /// <remarks>
    /// <para>
    ///    Uses <see cref="IStorageProvider"/> mechanism to access the data.
    ///    Uses zip pacakges form <see cref="Ionic"/>  namespace.
    ///     
    /// </para>    
    /// </remarks>
    public class RepositoryModel
    {
        private readonly IStorageProvider _storageProvider;


        public RepositoryModel(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;

            // initialize the list of modules using the storageProvider provider
        }

        public IEnumerable<IModuleInfo> ModuleInfosList
        {
            get { return _storageProvider.GetAvaliableModules(); }
        }


        public void AddModule(IModuleInfo moduleInfo)
        {
            if(moduleInfo == null)
                throw new ArgumentNullException("moduleInfo");

            // check for completeness of the provided thing
            if(string.IsNullOrEmpty(moduleInfo.Id) || moduleInfo.Manifest == null || moduleInfo.ModuleData == null)
                throw new ArgumentException("Module Info incomplete","moduleInfo");

            // check for duplications
            var avaliableModules = _storageProvider.GetAvaliableModules();
            if (avaliableModules.Any(module => module.Id.Equals(moduleInfo.Id)))
                throw new ArgumentException("Duplicate module","moduleInfo");
            

            // save into storage
            _storageProvider.SaveModule(moduleInfo);
        }


        public void RemoveModule(IModuleInfo moduleInfo)
        {
            if(moduleInfo == null)
                throw new ArgumentNullException("moduleInfo");

            // check for existence
            var modules = _storageProvider.GetAvaliableModules();
            if (!modules.Any(module => module.Id.Equals(moduleInfo.Id)))
                throw new ArgumentException("No such module info in collection", "moduleInfo");

            // remove from storage
            _storageProvider.RemoveModule(moduleInfo);
        }

    }
}