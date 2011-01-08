using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Nomad.Modules.Manifest;
using Nomad.Updater.ModuleRepositories.WebRepositories;
using Nomad.Utils;

namespace Nomad.Updater.ModuleRepositories
{
    /// <summary>
    ///     Default implementation of <see cref="IModulesRepository"/> for presentation purposes.
    /// </summary>
    /// <remarks>
    ///     Uses the XML mediated communication to 
    /// </remarks>
    public class WebModulesRepository : IModulesRepository
    {
        private readonly string _updateUrl;
        private readonly WebClient _webClient;
        private IList<WebModulePackageInfo> _packagesCollection;
        private string _repositoryUrl;


        /// <summary>
        ///     Creates the instance of the <see cref="WebModulesRepository"/>.
        /// </summary>
        /// <param name="updateUrl">
        ///     Url to update address of the server. On this adress should be placed XML file
        /// describing the avaliable updates.
        /// </param>
        public WebModulesRepository(string updateUrl)
        {
            if (String.IsNullOrEmpty(updateUrl))
            {
                throw new ArgumentNullException("updateUrl");
            }

            _updateUrl = updateUrl;
            _repositoryUrl = DecodeUrl(_updateUrl);
            _webClient = new WebClient();
        }


        private static string DecodeUrl(string updateUrl)
        {
            var urlRegex =
                new Regex(@"^.*//[^/]*/");                   

            var match = urlRegex.Match(updateUrl);
            if(match.Success)
            {
                return match.Value;
            }
            throw new ArgumentException("Passed url is not url", "updateUrl");
        }

        #region IModulesRepository Members

        public AvailableModules GetAvailableModules()
        {
            byte[] data = _webClient.DownloadData(_updateUrl);
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
    }
}