using System;
using System.Collections.Generic;
using System.Linq;
using Nomad.Modules.Manifest;

namespace Nomad.Modules
{
    /// <summary>
    ///     Nomad default implementation of <see cref="IDependencyChecker"/> class. 
    /// </summary>
    /// <remarks>
    ///     Checks for dependencies using <see cref="ModuleManifest"/> using topology sorting algorithm. 
    ///     O(n^2) where the <c>n</c> is number of passed modules.
    /// </remarks>
    public class DependencyChecker : IDependencyChecker
    {
        private Dictionary<string, List<string>> _nextNodeList;
        private Dictionary<string, ModuleWrapper> _myNodesDict;
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
                _nextNodeList[moduleInfo.Manifest.ModuleName] =
                    new List<string>(
                        moduleInfo.Manifest.ModuleDependencies.Select(m => m.ModuleName));


                _myNodesDict[moduleInfo.Manifest.ModuleName] = new ModuleWrapper()
                                                                   {
                                                                       Module = moduleInfo,
                                                                       Visited = false
                                                                   };
            }

            // TODO: switch arrows -> O(n^2)

            // run DFS through graph O(n)
            foreach (var moduleInfo in modules)
            {
                GoDFS(moduleInfo.Manifest.ModuleName,moduleInfo.Manifest.ModuleName,0);
            }
                
            
            // read stack backwardly O(n)
            return _stack.Reverse().ToList();
        }


        private void GoDFS(string myNode,string startingNode,int depth)
        {
            List<string> nodesToGo;
            ModuleWrapper myNodeInfo;
            if(!_nextNodeList.TryGetValue(myNode, out nodesToGo) || !_myNodesDict.TryGetValue(myNode,out myNodeInfo))
                throw new ArgumentException(string.Format("No such dependency in dictionary. The {0} could not be found.",myNode));
            

            if(myNode.Equals(startingNode) && depth > 0)
                throw new ArgumentException(string.Format("Graph has cycles. Duplicated node is {0}.",startingNode));

            depth++;

            if(myNodeInfo.Visited)
                return;

            myNodeInfo.Visited = true;

            foreach (var node in nodesToGo)
            {
                GoDFS(node,startingNode,depth);
            }

            _stack.Push(myNodeInfo.Module);
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

        #region Nested type: ModuleWrapper

        private class ModuleWrapper
        {
            public ModuleInfo Module { get; set; }
            public bool Visited { get; set; }
        }

        #endregion
    }
}