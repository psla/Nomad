using Nomad.Modules.Manifest;
using Nomad.Utils.ManifestCreator;

namespace Nomad.RepositoryServer.Models
{
    public interface IManifestProvider
    {
        string IssuerName { get; }
        string IssuerPath { get; }
        ManifestBuilderConfiguration Configuration { get; }
        ModuleManifest GenerateManifest(string assemblyFilePath, string folderPath);
    }
}