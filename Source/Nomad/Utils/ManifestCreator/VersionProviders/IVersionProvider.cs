namespace Nomad.Utils.ManifestCreator.VersionProviders
{
    ///<summary>
    ///     Provides the means to get the proper version from the assembly.
    ///</summary>
    public interface IVersionProvider
    {
        /// <summary>
        ///     Gets the version of the specified assembly.
        /// </summary>
        /// <param name="assemblyPath">The absolute path to assembly.</param>
        /// <returns></returns>
        Version GetVersion(string assemblyPath);
    }
}