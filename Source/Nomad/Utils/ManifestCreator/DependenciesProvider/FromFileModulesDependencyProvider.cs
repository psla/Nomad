using System;
using System.Collections.Generic;
using System.IO;
using Nomad.Modules.Manifest;

namespace Nomad.Utils.ManifestCreator.DependenciesProvider
{
    /// <summary>
    ///     Searches the provided file with for dependencies if files exists then applies this dependency.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///     If not file provided as dependency file the list of dependencies of method <see cref="GetDependencyModules"/> will be empty.
    ///     </para>
    ///     <para>
    ///          Does not utilize the <see cref="ModuleDependency.HasLoadingOrderPriority"/> property.
    ///     </para>
    ///     <para>
    ///         TODO: write about this file
    ///     </para>
    /// </remarks>
    public class FromFileModulesDependencyProvider : IModulesDependenciesProvider
    {
        private readonly string _depFileName;

        #region Constructors

        /// <summary>
        ///     Initializes the instance of the <see cref="FromFileModulesDependencyProvider"/>  class with
        /// <see cref="DefaultFileName"/> as fiel name to be serached.
        /// </summary>
        public FromFileModulesDependencyProvider() : this(DefaultFileName)
        {
        }


        /// <summary>
        ///     Initializes the instance of the <see cref="FromFileModulesDependencyProvider"/> class.
        /// </summary>
        /// <param name="depFileName">File name to be searched for depenencies.</param>
        public FromFileModulesDependencyProvider(string depFileName)
        {
            if (string.IsNullOrEmpty(depFileName))
                throw new ArgumentNullException("depFileName");

            _depFileName = depFileName;
        }

        #endregion

        /// <summary>
        ///     Default FileName to be searched for dependencies for module.
        /// </summary>
        public static string DefaultFileName
        {
            get { return @"dep.conf"; }
        }

        #region IModulesDependenciesProvider Members

        public IEnumerable<ModuleDependency> GetDependencyModules(string directory,
                                                                  string assemblyPath)
        {
            var dependencies = new List<ModuleDependency>();

            // check for existance
            if (!File.Exists(_depFileName))
            {
                //NOTE: if not find file then use empty list
                return dependencies;
            }

            // open file  
            
            
            // foreach dependency in add to module depenency list

            
            return dependencies;
        }

        #endregion
    }
}