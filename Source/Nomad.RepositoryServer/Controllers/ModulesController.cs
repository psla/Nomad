using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nomad.RepositoryServer.Models;
using Nomad.Updater.ModuleRepositories.WebRepositories;
using Nomad.Utils;

namespace Nomad.RepositoryServer.Controllers
{
    /// <summary>
    ///     Manages access to files to be dowloaded. Returns no views at all only <see cref="FileResult"/> classes.
    /// </summary>
    public class ModulesController : Controller
    {
        private readonly RepositoryModel _repositoryModel;


        public ModulesController(RepositoryModel repositoryModel)
        {
            _repositoryModel = repositoryModel;
        }


        /// <summary>
        ///     Gets the XML file with avaliable modules.
        /// </summary>
        /// <remarks>
        ///     method bases on http://stackoverflow.com/questions/186062/can-an-asp-net-mvc-controller-return-an-image
        /// </remarks>
        /// <returns><see cref="FileResult"/> with XML file describing the avaliable modules</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetModules()
        {
            // TODO: add handling errors of not having something or anything else
            var repoList = new List<IModuleInfo>(_repositoryModel.ModuleInfosList);
            List<WebModulePackageInfo> packageList = repoList
                .Select(repositoryModuleInfo
                        =>
                        new WebModulePackageInfo(repositoryModuleInfo.Manifest,
                                                 // TODO: prepare proper version of this url
                                                 repositoryModuleInfo.Id))
                .ToList();
            var webPackagesCollection = new WebAvailablePackagesCollection(packageList);

            return File(XmlSerializerHelper.Serialize(webPackagesCollection),
                        "text/xml");
        }


        /// <summary>
        ///     Gets the binary data requested package.
        /// </summary>
        /// <param name="urlId"></param>
        /// <returns>Binary file</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetModulePackage(string urlId)
        {
            if (string.IsNullOrEmpty(urlId))
                return View("FileNotFound");

            byte[] data = _repositoryModel.ModuleInfosList
                .Where(x => x.Id.Equals(urlId))
                .Select(x => x.ModuleData)
                .DefaultIfEmpty(null)
                .SingleOrDefault();

            if (data == null)
                return View("FileNotFound");

            return File(data, "application/zip");
        }
    }
}