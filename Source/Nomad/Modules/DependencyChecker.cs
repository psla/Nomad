using System;
using System.Collections.Generic;
using Nomad.Modules.Manifest;

namespace Nomad.Modules
{
    /// <summary>
    ///     Nomad defualt implementation of <see cref="IDependencyChecker"/> class. 
    /// </summary>
    /// <remarks>
    ///     Checks for dependencies using <see cref="ModuleManifest"/> using topology sorting algorithm.
    /// </remarks>
    public class DependencyChecker : IDependencyChecker
    {
        #region IDependencyChecker Members

        /// <summary>
        ///     Inherited.
        /// </summary>
        public IEnumerable<ModuleInfo> SortModules(IEnumerable<ModuleInfo> modules)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        ///   Inherited
        /// </summary>
        public bool CheckModules(IEnumerable<ModuleInfo> loadedModules,
                                 IEnumerable<ModuleInfo> newModules,
                                 out IEnumerable<ModuleInfo> nonValidModules)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}