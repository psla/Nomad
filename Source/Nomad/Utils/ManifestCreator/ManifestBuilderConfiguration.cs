using Nomad.Utils.ManifestCreator.DependenciesProvider;
using Nomad.Utils.ManifestCreator.FileSignerProviders;
using Nomad.Utils.ManifestCreator.VersionProviders;

namespace Nomad.Utils.ManifestCreator
{
    /// <summary>
    ///     Represents configuration of the manifest builder class. 
    /// </summary>
    /// <remarks>
    ///     Provides default configuratino for constructing the manifest. 
    /// </remarks>
    public class ManifestBuilderConfiguration
    {
        /// <summary>
        ///     Provides the version.
        /// </summary>
        public IVersionProvider VersionProvider { get; set; }

        /// <summary>
        ///     Provides the file signer.
        /// </summary>
        public ISignedFilesProvider SignedFilesProvider { get; set; }

        /// <summary>
        ///     Provides the module dependency.
        /// </summary>
        public IModulesDependenciesProvider ModulesDependenciesProvider { get; set; }

        /// <summary>
        ///     Default implementation of configuration.
        /// </summary>
        /// <remarks>
        ///     Uses following engines:
        ///      <see cref="SimpleVersionProvider"/>,
        ///      <see cref="WholeDirectorySignedFilesProvider"/> and 
        ///      <see cref="FromFileModulesDependencyProvider"/>
        /// </remarks>
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