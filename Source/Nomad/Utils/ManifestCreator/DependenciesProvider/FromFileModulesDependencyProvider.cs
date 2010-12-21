using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
    ///        Conf file is text file which consists of paths to valid .NET assemblies (relative or absolute).
    ///     </para>
    /// </remarks>
    public class FromFileModulesDependencyProvider : IModulesDependenciesProvider
    {
        private readonly string _depFileName;

        /// <summary>
        ///     The file name in which the dependencies are stored.
        /// </summary>
        public string DependencyFileName
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory,_depFileName); }
        }

        /// <summary>
        ///     Default FileName to be searched for dependencies for module.
        /// </summary>
        public static string DefaultFileName
        {
            get { return @"dep.conf"; }
        }

        #region Constructors

        /// <summary>
        ///     Initializes the instance of the <see cref="FromFileModulesDependencyProvider"/>  class with
        /// <see cref="DefaultFileName"/> as file name to be searched.
        /// </summary>
        public FromFileModulesDependencyProvider() : this(DefaultFileName)
        {
        }


        /// <summary>
        ///     Initializes the instance of the <see cref="FromFileModulesDependencyProvider"/> class.
        /// </summary>
        /// <param name="depFileName">File name to be searched for dependencies.</param>
        public FromFileModulesDependencyProvider(string depFileName)
        {
            if (string.IsNullOrEmpty(depFileName))
                throw new ArgumentNullException("depFileName");

            _depFileName = depFileName;
        }

        #endregion

        #region IModulesDependenciesProvider Members

        public IEnumerable<ModuleDependency> GetDependencyModules(string directory,
                                                                  string assemblyPath)
        {
            var dependencies = new List<ModuleDependency>();

            // check for existence
            if (!File.Exists(_depFileName))
            {
                //NOTE: if not find file then use empty list
                return dependencies;
            }

            // open file    
            using (StreamReader reader = File.OpenText(_depFileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, line);
                        AssemblyName asmName = AssemblyName.GetAssemblyName(path);
                        var dependency = new ModuleDependency
                                             {
                                                 HasLoadingOrderPriority = false,
                                                 ModuleName = asmName.Name,
                                                 MinimalVersion = new Version(asmName.Version),
                                                 ProcessorArchitecture =
                                                     asmName.ProcessorArchitecture
                                             };
                        dependencies.Add(dependency);
                    }
                    catch (Exception e)
                    {
                        throw new FileFormatException("File corrupted", e);
                    }
                }
            }

            return dependencies;
        }

        #endregion
    }
}