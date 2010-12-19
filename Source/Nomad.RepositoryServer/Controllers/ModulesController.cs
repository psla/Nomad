using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nomad.RepositoryServer.Models;
using Nomad.Updater.ModuleRepositories.WebRepositories;
using Nomad.Utils;

namespace Nomad.RepositoryServer.Controllers
{
    /// <summary>
    ///     Manages access to files to be dowloaded. Returns no views at all only <see cref="FileResult"/> classes.
    /// </summary>
    /// <remarks>
    ///     If any error occurs output will be Error View.
    /// </remarks>
    [HandleError(View = "Error")]
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
        /// <returns><see cref="FileResult"/> with XML file describing the avaliable modules, compilant with <see cref="WebAvailablePackagesCollection"/></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public FileResult GetModules()
        {
            var splitedUrl = Request.RawUrl.Split('/');
            // select all but last element
            var urlBase= splitedUrl.Take(splitedUrl.Count() - 1);
            var addressToController = urlBase.Aggregate((a, b) => a + '/' + b);
            
            var repoList = new List<IModuleInfo>(_repositoryModel.ModuleInfosList);
            List<WebModulePackageInfo> packageList = repoList
                .Select(repositoryModuleInfo
                        =>
                        new WebModulePackageInfo(repositoryModuleInfo.Manifest,
                                                 addressToController + '/' + repositoryModuleInfo.Id))
                .ToList();
            var webPackagesCollection = new WebAvailablePackagesCollection(packageList);

            return File(
                    new MemoryStream(XmlSerializerHelper.Serialize(webPackagesCollection)),
                    "text/xml","updates.xml");
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

            
            return  File(new MemoryStream(data), "application/zip","module.zip");
        }
    }
}