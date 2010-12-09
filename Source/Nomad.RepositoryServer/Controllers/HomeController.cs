using System;
using System.Linq;
using System.Web.Mvc;
using Nomad.RepositoryServer.Models;

namespace Nomad.RepositoryServer.Controllers
{
    /// <summary>
    ///     Handles the display of the actual state of the repository and so on.
    /// </summary>
    [HandleError]
    public class HomeController : Controller
    {

        private readonly RepositoryModel _repositoryModel;

        public HomeController(RepositoryModel repositoryModel)
        {
            _repositoryModel = repositoryModel;
        }

        /// <summary>
        ///     Displays main page with the list of available modules and some CRUD options.
        /// </summary>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index()
        {
            return View("Index", _repositoryModel);
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
        ///     Handles file not found action
        /// </summary>
        /// <remarks>
        ///     TODO: change to use HandleException attrubute for more automation
        /// </remarks>
        /// <returns></returns>
        public ActionResult FileNotFound()
        {
            return View("FileNotFound");
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
            
            // check validty of the moduleInfo
            if(selectedModel == null)
                return View("FileNotFound");

            if(selectedModel.Manifest == null || selectedModel.ModuleData == null)
                return View("FileNotFound");

            // if evrythings ok go to details
            return View("Details", selectedModel);
        }

        /// <summary>
        ///     Removes the module with provided <paramref name="id"/>.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Remove(string id)
        {
            // TODO: implement removing item from repository if error show special view

            return RedirectToAction("Index");
        }
    }
}