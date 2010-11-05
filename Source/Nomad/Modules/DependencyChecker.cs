using System;
using System.Collections.Generic;
using Nomad.Modules.Manifest;

namespace Nomad.Modules
{
    /// <summary>
    ///     Nomad default implementation of <see cref="IDependencyChecker"/> class. 
    /// </summary>
    /// <remarks>
    ///     Checks for dependencies using <see cref="ModuleManifest"/> using topology sorting algorithm. 
    ///     O(3n) where the <c>n</c> is number of passed modules.
    /// </remarks>
    public class DependencyChecker : IDependencyChecker
    {
        private Stack<ModuleInfo> _stack;

        #region IDependencyChecker Members

        /// <summary>
        ///     Inherited.
        /// </summary>
        public IEnumerable<ModuleInfo> SortModules(IEnumerable<ModuleInfo> modules)
        {
            var sortedModules = new List<ModuleInfo>(modules);
            _stack = new Stack<ModuleInfo>();

            // populate the graph representing the modules O(n), switch the arrows

            // run DFS through graph O(n)

            // read stack backwardly O(n)

            return sortedModules;
        }


        /// <summary>
        ///   Inherited.
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