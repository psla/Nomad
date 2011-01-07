using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Nomad.Modules.Manifest;
using Nomad.Updater.ModuleRepositories.WebRepositories;
using Nomad.Utils;

namespace Nomad.Updater.ModuleRepositories
{
    public class WebModulesRepository : IModulesRepository
    {
        private readonly string _repositoryUrl;
        private readonly WebClient _webClient;
        private IList<WebModulePackageInfo> _packagesCollection;


        public WebModulesRepository(string repositoryUrl)
        {
            if (String.IsNullOrEmpty(repositoryUrl))
            {
                throw new ArgumentNullException("repositoryUrl");
            }
            _repositoryUrl = repositoryUrl;
            _webClient = new WebClient();
        }

        #region IModulesRepository Members

        public AvailableModules GetAvailableModules()
        {
            byte[] data = _webClient.DownloadData(_repositoryUrl);
            var modulesCollection =
                XmlSerializerHelper.Deserialize<WebAvailablePackagesCollection>(data);
            _packagesCollection = modulesCollection.AvailablePackages;
            List<ModuleManifest> manifests =
                modulesCollection.AvailablePackages.Select(x => x.Manifest).ToList();
            return new AvailableModules(manifests);
        }


        public ModulePackage GetModule(string moduleUniqueName)
        {
            IEnumerable<WebModulePackageInfo> packageInfo =
                _packagesCollection.Where(x => x.Manifest.ModuleName.Equals(moduleUniqueName));

            string url = packageInfo.Select(x => x.Url).Single();

            ModuleManifest manifest = packageInfo.Select(x => x.Manifest).Single();
            byte[] data = _webClient.DownloadData(url);

            return new ModulePackage {ModuleManifest = manifest, ModuleZip = data};
        }

        #endregion

        public bool AddModule(ModulePackage module)
        {
            throw new NotImplementedException("Client can not add module to a repository");
        }
    }
}