using System;
using System.Linq;
using System.Web.Mvc;
using Nomad.RepositoryServer.Models;

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
        ///     Displays the view with information about module with provided <paramref name="itemId"/>
        /// </summary>
        /// <param name="itemId">The key to provided module</param>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Details(string itemId)
        {
            if(string.IsNullOrEmpty(itemId))
            {
                ViewData["Message"] = "No key provided";
                return View("Error");
            }

            IModuleInfo selectedModel = _repositoryModel.ModuleInfosList
                .Where(x => x.Id.Equals(itemId))
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
        ///     Removes the module with provided <paramref name="itemId"/>.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Remove(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                ViewData["Message"] = "No key provided";
                return View("Error");
            }

            IModuleInfo item = _repositoryModel.ModuleInfosList
                .Where(x => x.Id.Equals(itemId))
                .Select(x => x)
                .DefaultIfEmpty(null)
                .SingleOrDefault();

            if (item == null)
            {
                ViewData["Message"] = "Key could not be found in database";
                return View("Error");
            }

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