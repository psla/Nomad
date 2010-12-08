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
        private const string InviteMessage = "Nomad Modules Center";

        private readonly RepositoryModel _repositoryModel;

        public HomeController(RepositoryModel repositoryModel)
        {
            _repositoryModel = repositoryModel;
        }

        /// <summary>
        ///     Displays main page with the list of available modules.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index()
        {
            ViewData["Message"] = InviteMessage;

            return View("Index", _repositoryModel);
        }

        /// <summary>
        ///     Displays view with information about authors.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult About()
        {
            return View("About");
        }


        /// <summary>
        ///     Handles file not found action
        /// </summary>
        /// <returns></returns>
        public ActionResult FileNotFound()
        {
            return View("FileNotFound");
        }
    }
}