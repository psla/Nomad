using System;
using System.Collections.Generic;
using Nomad.Modules;

namespace Nomad.Services
{
    ///<summary>
    /// Implementation of <see cref="ILoadedModulesService"/>
    ///</summary>
    public class LoadedModulesService : MarshalByRefObject, ILoadedModulesService
    {
        private readonly IModuleLoader _moduleLoader;


        /// <summary>
        /// Initializes the instance of <see cref="LoadedModulesService"/>
        /// </summary>
        /// <param name="moduleLoader"><see cref="IModuleLoader"/> containing information of loaded modules.</param>
        public LoadedModulesService(IModuleLoader moduleLoader)
        {
            _moduleLoader = moduleLoader;
        }

        #region ILoadedModulesService Members

        ///<summary>
        /// Provides information about currently loaded modules.
        ///</summary>
        ///<returns><see cref="IList{ModuleInfo}"/> containing moduleInfos of all loaded modules.</returns>
        public IList<ModuleInfo> GetLoadedModules()
        {
            return _moduleLoader.GetLoadedModules().AsReadOnly();
        }

        #endregion
    }
}