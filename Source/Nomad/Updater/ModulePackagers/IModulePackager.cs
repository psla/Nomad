using System.Collections.Generic;

namespace Nomad.Updater.ModulePackagers
{
    /// <summary>
    ///     Resposibile for unpackaging packages and accessing file system to perform update.
    /// </summary>
    public interface IModulePackager
    {
        /// <summary>
        ///     Performs setting package (<paramref name="modulePackages"/>) to new <paramref name="targetDirectory"/>
        /// </summary>
        /// <param name="targetDirectory">Path to directory on file system to place tha package.</param>
        /// <param name="modulePackages">Package to be placed.</param>
        void PerformUpdates(string targetDirectory, ModulePackage modulePackages);
    }
}