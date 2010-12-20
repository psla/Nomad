using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nomad.Modules.Manifest;
using Nomad.RepositoryServer.Models;
using Nomad.RepositoryServer.Models.ServerSigner;
using Nomad.Utils.ManifestCreator;

namespace Nomad.RepositoryServer.Controllers
{
    /// <summary>
    ///     Handles the display of the actual state of the repository. Manages the CRUD operations on repository.
    /// </summary>
    [HandleError]
    public class HomeController : Controller
    {
        private readonly IManifestProvider _manifestProvider;
        private readonly RepositoryModel _repositoryModel;


        public HomeController(RepositoryModel repositoryModel, IManifestProvider manifestProvider)
        {
            _repositoryModel = repositoryModel;
            _manifestProvider = manifestProvider;
        }


        /// <summary>
        ///     Displays main page with the list of available modules and some CRUD options.
        /// </summary>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index()
        {
            return View("Index", _repositoryModel.ModuleInfosList);
        }


        /// <summary>
        ///     Displays view with information about authors.
        /// </summary>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult About()
        {
            return View("About");
        }


        /// <summary>
        ///     Displays the view with information about module with provided <paramref name="id"/>
        /// </summary>
        /// <param name="id">The key to provided module</param>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Details(string id)
        {
            IModuleInfo selectedModel = _repositoryModel.ModuleInfosList
                .Where(x => x.Id.Equals(id))
                .Select(x => x)
                .DefaultIfEmpty(null)
                .SingleOrDefault();

            // check validity of the moduleInfo
            if (selectedModel == null)
                return View("FileNotFound");

            if (selectedModel.Manifest == null || selectedModel.ModuleData == null)
                return View("FileNotFound");

            // if everything's ok go to details
            return View("Details", selectedModel);
        }


        /// <summary>
        ///     Removes the module with provided <paramref name="id"/>.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Remove(string id)
        {
            if (string.IsNullOrEmpty(id))
                return View("FileNotFound");

            var item = _repositoryModel.ModuleInfosList
                .Where(x => x.Id.Equals(id))
                .Select(x => x)
                .DefaultIfEmpty(null)
                .SingleOrDefault();

            if (item == null)
                return View("FileNotFound");
            try
            {
                _repositoryModel.RemoveModule(item);
            }
            catch (Exception e)
            {
                ViewData["Message"] = e.Message;
                return View("Error");
            }

            return RedirectToAction("Index");
        }

        
    }
}