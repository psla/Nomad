namespace Nomad.Updater.ModuleFinders
{
    /// <summary>
    ///     Engine for <see cref="NomadUpdater"/> class. Connects <see cref="ModulePackage"/> with its place in file system.
    /// </summary>
    /// <remarks>
    ///     This interface is created solely for the need of <see cref="NomadUpdater"/> class. 
    /// </remarks>
    public interface IModuleFinder
    {
        /// <summary>
        ///     Get the absolute path to directory where the package should be unpacked.
        /// </summary>
        /// <param name="modulesDirectory">Some path to be taken into consideration during finding.</param>
        /// <param name="modulePackage">Package for which is path being searched.</param>
        /// <returns>Absolute path to directory where package should be placed.</returns>
        string FindDirectoryForPackage(string modulesDirectory, ModulePackage modulePackage);
    }
}