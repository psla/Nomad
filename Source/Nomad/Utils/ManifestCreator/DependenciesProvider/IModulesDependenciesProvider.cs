using System.Collections.Generic;
using Nomad.Modules.Manifest;

namespace Nomad.Utils.ManifestCreator.DependenciesProvider
{
    ///<summary>    
    ///     Gets the dependencies of the assembly in the folder.
    ///</summary>
    public interface IModulesDependenciesProvider
    {
        ///<summary>
        ///     Gets the enumerable collection of <see cref="ModuleDependency"/> objects. Basing on <paramref name="directory"/> and <paramref name="assemblyPath"/>.
        ///</summary>
        ///<param name="directory">The directory to be used to generate dependencies.</param>
        ///<param name="assemblyPath">The assembly for which we are generating dependencies.</param>
        ///<returns></returns>
        IEnumerable<ModuleDependency> GetDependencyModules(string directory, string assemblyPath);
    }
}