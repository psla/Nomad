using System;
using System.Collections.Generic;
using System.Linq;
using Nomad.Modules.Manifest;
using Version = Nomad.Utils.Version;

namespace Nomad.Modules
{
    /// <summary>
    ///     Nomad default implementation of <see cref="IDependencyChecker"/> class. 
    /// </summary>
    /// <remarks>
    ///     Checks for dependencies using <see cref="ModuleManifest"/> using topology sorting algorithm. 
    ///     O(n^2) / Theta(n+m) where the <c>n</c> is number of passed modules and <c>m</c> is average number of dependencies per module.
    /// </remarks>
    public class DependencyChecker : IDependencyChecker
    {
        private bool _canBeSorted;
        private Dictionary<string, ModuleWrapper> _myNodesDict;
        private Dictionary<string, List<string>> _nextNodeList;

        private IList<ModuleInfo> _nonValidModules;
        private SortingMode _sortingMode;
        private Stack<ModuleInfo> _stack;

        #region IDependencyChecker Members

        /// <summary>
        ///     Inherited.
        /// </summary>
        public IEnumerable<ModuleInfo> SortModules(IEnumerable<ModuleInfo> modules)
        {
            _sortingMode = SortingMode.Exceptions;
            InitializeGraph(modules);

            // run DFS through graph O(n)
            foreach (ModuleInfo moduleInfo in modules)
            {
                ModuleWrapper wrapper;
                if (!_myNodesDict.TryGetValue(moduleInfo.Manifest.ModuleName, out wrapper))
                    throw new ArgumentException(
                        string.Format(
                            "No such dependency in dictionary. The {0} could not be found.",
                            moduleInfo.Manifest.ModuleName));

                GoDFS(moduleInfo.Manifest.ModuleName, moduleInfo.Manifest.ModuleName, 0, wrapper);
            }

            // read stack backwardly O(n)
            return _stack.Reverse().ToList();
        }


        /// <summary>
        ///   Inherited.
        /// </summary>
        public bool CheckModules(IEnumerable<ModuleInfo> loadedModules,
                                 IEnumerable<ModuleInfo> newModules,
                                 out IEnumerable<ModuleInfo> nonValidModules)
        {
            // concatenate loaded and new modules into one list, with preference over newModules things - use newList as default
            var concatentedList = new List<ModuleInfo>(newModules);

            // control variables
            var downgradedModules = new List<ModuleInfo>();
            bool hasDowngradedModule = false;

            // concatenate the lists, watching out for downgrades O(n^2)
            foreach (ModuleInfo loadedModule in loadedModules)
            {
                ModuleInfo inLoopModule = loadedModule;
                ModuleInfo uploadModule =
                    concatentedList.Where((module) => module.Manifest.ModuleName.Equals(inLoopModule.Manifest.ModuleName))
                    .Select(x => x)
                    .DefaultIfEmpty(null)
                    .SingleOrDefault();

                if (uploadModule == null)
                {
                    // if not contains add to update list
                    concatentedList.Add(loadedModule);
                }
                else
                {
                    // if contains the same module check if the update module is not a downgrade
                    if (uploadModule.Manifest.ModuleVersion.GetSystemVersion() <
                        loadedModule.Manifest.ModuleVersion.GetSystemVersion())
                    {
                        hasDowngradedModule = true;
                        downgradedModules.Add(uploadModule);
                    }
                }
            }

            // check if there were downgrades if true then stop
            if (hasDowngradedModule)
            {
                nonValidModules = downgradedModules;
                return false;
            }

            // use try sort method to check if it is possible to sort that collections
            IEnumerable<ModuleInfo> sortedModules;

            return TrySortModules(concatentedList, out sortedModules, out nonValidModules);
        }

        #endregion

        private void InitializeGraph(IEnumerable<ModuleInfo> modules)
        {
            _stack = new Stack<ModuleInfo>();
            _nextNodeList = new Dictionary<string, List<string>>();
            _myNodesDict = new Dictionary<string, ModuleWrapper>();

            // populate the graph representing the modules O(n^2), Theta (n+m)
            foreach (ModuleInfo moduleInfo in modules)
            {
                // populate next - list
                _nextNodeList[moduleInfo.Manifest.ModuleName] =
                    new List<string>(
                        moduleInfo.Manifest.ModuleDependencies.Select(m => m.ModuleName));

                // populate the visited list
                _myNodesDict[moduleInfo.Manifest.ModuleName] = new ModuleWrapper
                                                                   {
                                                                       Module = moduleInfo,
                                                                       Visited = false
                                                                   };
            }
        }


        /// <summary>
        ///     Second implementation of topology sorting, without exceptions.
        /// </summary>
        /// <param name="inputList">Enumerable collection of modules to be sorted.</param>
        /// <param name="outputList">Enumerable collection of sorted modules.</param>
        /// <param name="nonValidModules">Enumerable collection of modules which made sorting impossible.</param>
        /// <returns></returns>
        public bool TrySortModules(IEnumerable<ModuleInfo> inputList,
                                   out IEnumerable<ModuleInfo> outputList,
                                   out IEnumerable<ModuleInfo> nonValidModules)
        {
            _sortingMode = SortingMode.Silent;
            _canBeSorted = true;

            InitializeGraph(inputList);

            _nonValidModules = new List<ModuleInfo>();

            // run DFS through graph O(n)
            foreach (ModuleInfo moduleInfo in inputList)
            {
                ModuleWrapper wrapper;
                if (!_myNodesDict.TryGetValue(moduleInfo.Manifest.ModuleName, out wrapper))
                {
                    _canBeSorted = false;
                    _nonValidModules.Add(moduleInfo);
                }

                GoDFS(moduleInfo.Manifest.ModuleName, moduleInfo.Manifest.ModuleName, 0, wrapper);
            }

            // read stack backwardly O(n) or return empty list if sorting was not possible
            if (_canBeSorted)
                outputList = _stack.Reverse().ToList();
            else
                outputList = new List<ModuleInfo>();

            // return non valid modules
            nonValidModules = _nonValidModules;

            // return state value of 'this'
            return _canBeSorted;
        }


        private void GoDFS(string myNode, string startingNode, int depth, ModuleWrapper myNodeInfo)
        {
            // get list of nodes to go
            List<string> nodesToGo;
            if (!_nextNodeList.TryGetValue(myNode, out nodesToGo))
                if (_sortingMode == SortingMode.Exceptions)
                {
                    throw new ArgumentException(
                        string.Format(
                            "No such dependency in dictionary. The {0} could not be found.",
                            myNode), "myNode");
                }
                else if (_sortingMode == SortingMode.Silent)
                {
                    _canBeSorted = false;
                    return; // nodes to go wil be empty -> no place to go
                }

            // check if graph has cycles
            if (myNode.Equals(startingNode) && depth > 0)
                if (_sortingMode == SortingMode.Exceptions)
                {
                    throw new ArgumentException(
                        string.Format("DependencyGraph has cycles. Duplicated node is {0}.",
                                      startingNode),
                        "startingNode");
                }
                else if (_sortingMode == SortingMode.Silent)
                {
                    _canBeSorted = false;
                    return; // we have to jump out because of the infinite loop
                }

            depth++;

            // stop condition
            if (myNodeInfo.Visited)
                return;

            myNodeInfo.Visited = true;

            foreach (string nodeName in nodesToGo)
            {
                // check if the node exists
                ModuleWrapper nodeToGo;
                if (!_myNodesDict.TryGetValue(nodeName, out nodeToGo))
                    if (_sortingMode == SortingMode.Exceptions)
                    {
                        throw new ArgumentException(
                            string.Format(
                                "No such dependency in dictionary. The {0} could not be found.",
                                myNode),
                            "myNode");
                    }
                    else if (_sortingMode == SortingMode.Silent)
                    {
                        _canBeSorted = false;

                        // add the starting node to the list of non valid nodes (it has some empty reference)
                        if (!_nonValidModules.Contains(myNodeInfo.Module))
                            _nonValidModules.Add(myNodeInfo.Module);

                        // go to another node
                        continue;
                    }

                // get version 
                string name = nodeName;
                IEnumerable<Version> myNodeDependencyVersions =
                    myNodeInfo.Module.Manifest.ModuleDependencies.Where(
                        x => x.ModuleName.Equals(name)).Select(x => x.MinimalVersion);

                Version myNodeDependencyVersion =
                    myNodeDependencyVersions.SingleOrDefault();

                Version dependencyVersion = nodeToGo.Module.Manifest.ModuleVersion;

                // compare values of the versions, using system feature
                if (myNodeDependencyVersion.GetSystemVersion() >
                    dependencyVersion.GetSystemVersion())
                    if (_sortingMode == SortingMode.Exceptions)
                    {
                        throw new ArgumentException(
                            string.Format(
                                "Dependency version ({0}) is higher than module's version {1}",
                                myNodeDependencyVersion, dependencyVersion));
                    }
                    else if (_sortingMode == SortingMode.Silent)
                    {
                        _canBeSorted = false;

                        // wrong version means that our node has wrong reference
                        if (!_nonValidModules.Contains(myNodeInfo.Module))
                            _nonValidModules.Add(myNodeInfo.Module);

                        // let go this way
                    }

                // if everything ok, go DFS
                GoDFS(nodeName, startingNode, depth, nodeToGo);
            }
            _stack.Push(myNodeInfo.Module);
        }

        #region Nested type: InjectableEqualityComparer

        private class InjectableEqualityComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _comparer;


            public InjectableEqualityComparer(Func<T, T, bool> comparer)
            {
                _comparer = comparer;
            }

            #region IEqualityComparer<T> Members

            public bool Equals(T a, T b)
            {
                return _comparer(a, b);
            }


            public int GetHashCode(T a)
            {
                return a.GetHashCode();
            }

            #endregion
        }

        #endregion

        #region Nested type: ModuleWrapper

        private class ModuleWrapper
        {
            public ModuleInfo Module { get; set; }
            public bool Visited { get; set; }
        }

        #endregion

        #region Nested type: SortingMode

        private enum SortingMode
        {
            Exceptions,
            Silent
        }

        #endregion
    }
}