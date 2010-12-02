using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Nomad.Modules.Manifest;

namespace Nomad.Utils.ManifestCreator.DependenciesProvider
{
    ///<summary>
    ///     Gets the dependencies using all assemblies from directory as references.
    ///</summary>
    /// <remarks>
    ///     Does not utilize the <see cref="ModuleDependency.HasLoadingOrderPriority"/> property.
    /// </remarks>
    public class WholeDirectoryModulesDependenciesProvider : IModulesDependenciesProvider
    {
        #region IModulesDependenciesProvider Members

        public IEnumerable<ModuleDependency> GetDependencyModules(string directory,
                                                                   string assemblyPath)
        {
            var dependencies = new List<ModuleDependency>();

            // read the directory 
            IEnumerable<string> files = Directory.GetFiles(directory, "*.dll",
                                                           SearchOption.AllDirectories).Concat(
                                                               Directory.GetFiles(directory,
                                                                                  "*.exe",
                                                                                  SearchOption.
                                                                                      AllDirectories));
            // mine module
            string myAsm = AssemblyName.GetAssemblyName(assemblyPath).Name;

            // for each assembly in this collection, try resolving its, name version and so on.)
            foreach (string file in files)
            {
                AssemblyName asmName = null;
                try
                {
                    asmName = AssemblyName.GetAssemblyName(file);
                }
                catch (BadImageFormatException)
                {
                    // nothing happens
                    continue;
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException("Access to file is somewhat corrupted", e);
                }

                // do not add itself
                if (asmName.Name.Equals(myAsm))
                    continue;

                dependencies.Add(new ModuleDependency
                                     {
                                         ModuleName = asmName.Name,
                                         MinimalVersion = new Version(asmName.Version),
                                         ProcessorArchitecture = asmName.ProcessorArchitecture,
                                         HasLoadingOrderPriority = false,
                                     });
            }

            return dependencies;
        }

        #endregion
    }
}