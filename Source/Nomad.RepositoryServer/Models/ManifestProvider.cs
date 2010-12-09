using Nomad.KeysGenerator;
using Nomad.Modules.Manifest;
using Nomad.Signing;
using Nomad.Utils.ManifestCreator;

namespace Nomad.RepositoryServer.Models
{
    /// <summary>
    ///     Describes the settings of the manifests in server.
    /// </summary>
    /// <remarks>
    ///     Default configuration of manifest genrating in this server.
    /// </remarks>
    public class ManifestProvider : IManifestProvider
    {
        public string IssuerName { get { return "SERVER_ISSUER"; }}

        public string IssuerPath { get { return @"ISSUER_PATH"; } }

        public ManifestBuilderConfiguration Configuration { get { return ManifestBuilderConfiguration.Default; } }

        public ModuleManifest GenerateManifest(string assemblyFilePath, string folderPath)
        {
            // build manifest with this settings
            KeysGeneratorProgram.Main(new[] { IssuerPath });

            var manifestBuilder = new ManifestBuilder(IssuerName, IssuerPath,
                                                      assemblyFilePath,
                                                      folderPath, KeyStorage.Nomad,
                                                      string.Empty, Configuration);

            return manifestBuilder.Create();
        }
    }
}