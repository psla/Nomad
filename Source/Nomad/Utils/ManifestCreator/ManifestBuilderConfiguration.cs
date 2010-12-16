using Nomad.Utils.ManifestCreator.DependenciesProvider;
using Nomad.Utils.ManifestCreator.FileSignerProviders;
using Nomad.Utils.ManifestCreator.VersionProviders;

namespace Nomad.Utils.ManifestCreator
{
    /// <summary>
    ///     Represents configuration of the manifest builder class. 
    /// </summary>
    /// <remarks>
    ///     Provides default engines for constructing the manifest.
    /// </remarks>
    public class ManifestBuilderConfiguration
    {
        public IVersionProvider VersionProvider { get; set; }
        public ISignedFilesProvider SignedFilesProvider { get; set; }
        public IModulesDependenciesProvider ModulesDependenciesProvider { get; set; }

        public static ManifestBuilderConfiguration Default
        {
            get
            {
                return new ManifestBuilderConfiguration
                           {
                               VersionProvider = new SimpleVersionProvider(),
                               SignedFilesProvider = new WholeDirectorySignedFilesProvider(),
                               ModulesDependenciesProvider = new FromFileModulesDependencyProvider(),
                           };
            }
        }
    }
}