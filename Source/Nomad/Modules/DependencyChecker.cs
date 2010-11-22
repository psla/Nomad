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
        private Dictionary<string, ModuleWrapper> _myNodesDict;
        private Dictionary<string, List<string>> _nextNodeList;
        private Stack<ModuleInfo> _stack;

        #region IDependencyChecker Members

        /// <summary>
        ///     Inherited.
        /// </summary>
        public IEnumerable<ModuleInfo> SortModules(IEnumerable<ModuleInfo> modules)
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
            // initialize empty list for non valid modules
            nonValidModules = new List<ModuleInfo>();

            // concatenate loaded and new modules into one list, with preference over newModules things. O(n^2)
            var concatentedList = new List<ModuleInfo>(newModules);

            foreach (ModuleInfo loadedModule in loadedModules)
            {
                if (!concatentedList.Contains(loadedModule,
                                             new InjectableEqualityComparer<ModuleInfo>(
                                                 (a, b) =>
                                                 a.Manifest.ModuleName.Equals(b.Manifest.ModuleName))))
                {
                    concatentedList.Add(loadedModule);
                }
            }


            // TODO: rewrite the whole class to use no exception but only TrySortModulesMethod
            // use try sort method to check if it is possible to sort that collections
            try
            {
                SortModules(concatentedList);
            }
            catch (ArgumentException e)
            {
                if (!string.IsNullOrEmpty(e.ParamName))
                {
                    // search for that module info
                    nonValidModules =
                        (IEnumerable<ModuleInfo>)
                        concatentedList.Select(x => x.Manifest.ModuleName.Equals(e.ParamName));
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        #endregion

        private void GoDFS(string myNode, string startingNode, int depth, ModuleWrapper myNodeInfo)
        {
            // get list of nodes to go
            List<string> nodesToGo;
            if (!_nextNodeList.TryGetValue(myNode, out nodesToGo))
                throw new ArgumentException(
                    string.Format("No such dependency in dictionary. The {0} could not be found.",
                                  myNode), "myNode");

            // check if graph has cycles
            if (myNode.Equals(startingNode) && depth > 0)
                throw new ArgumentException(
                    string.Format("Graph has cycles. Duplicated node is {0}.", startingNode),
                    "startingNode");

            depth++;

            // stop condition
            if (myNodeInfo.Visited)
                return;

            myNodeInfo.Visited = true;

            foreach (string nodeName in nodesToGo)
            {
                // check for the going into node about the versions
                ModuleWrapper nodeToGo;
                if (!_myNodesDict.TryGetValue(nodeName, out nodeToGo))
                    throw new ArgumentException(
                        string.Format(
                            "No such dependency in dictionary. The {0} could not be found.", myNode),
                        "myNode");

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
                    throw new ArgumentException(
                        string.Format(
                            "Dependency version ({0}) is higher than module's version {1}",
                            myNodeDependencyVersion, dependencyVersion));

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
    }
}