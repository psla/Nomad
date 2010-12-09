using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nomad.Modules.Manifest;
using Nomad.RepositoryServer.Models;
using Nomad.RepositoryServer.Models.ModulesUploading;
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

            _repositoryModel.RemoveModule(item);

            return RedirectToAction("Index");
        }

        #region Adding Module

        /// <summary>
        ///     Initializes the whole process of adding module to repository.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddModule(HttpPostedFileBase file)
        {
            if (file == null)
                return View("Error");

            var moduleAdder = new VirtualModuleAdder(file.FileName);
            file.SaveAs(moduleAdder.AssemblyFilePath);

            try
            {
                moduleAdder.GenerateModuleInfo();
            }
            catch (Exception)
            {
                // save the day with 
                return View("Error");
            }

            // save the module adder in the session 
            Session["ModuleAdder"] = moduleAdder;

            return View("AddModule", moduleAdder);
        }


        /// <summary>
        ///     Adds files to virtual folder. Is a mid step in adding module to repository.
        /// </summary>
        /// <remarks>
        ///     TODO: Needs <see cref="FormCollection"/> or Request object to provide means for remembering the files to be included in package.
        /// </remarks>
        /// <param name="file"></param>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddFile(HttpPostedFileBase file)
        {
            // get module adder from session
            object moduleAdder = Session["ModuleAdder"];
            if (moduleAdder == null)
                return View("Error");

            var adder = (VirtualModuleAdder) moduleAdder;

            // use obtained session module adder to add something into virtual folder
            adder.SaveFileToVirtualFolder(file);

            //// manage the chekboxes from view
            //foreach (var fileWrapper in adder.InFolderFiles)
            //{
            //    // search this file wrapper in form collection
            //    var key = fileWrapper.FileName;
            //    var value = Request.Form[key];

            //    if(value != null && value[0].Equals("true"))
            //    {
            //        fileWrapper.ToPackage = true;
            //    }
            //}

            // pass the new VirtualModuleAdder into the View once again.
            return View("AddModule", adder);
        }


        /// <summary>
        ///     Publishes module into repository.
        /// </summary>
        /// <remarks>
        ///     Performs the following actions:
        /// 1. Manifest building with normal options through <see cref="ManifestBuilder"/>
        /// 2. Packs the files in virtual folder into ZIP file
        /// 3. Adds all necessary information to repository using <see cref="IStorageProvider"/>
        /// </remarks>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PublishModule()
        {
            // get module adder from session
            object moduleAdder = Session["ModuleAdder"];
            if (moduleAdder == null)
                return View("Error");

            var adder = (VirtualModuleAdder) moduleAdder;

            var listOfFilesInPackage = new List<string>();
            //TODO: get list of files to include into package (assembly,asc,manifest are required) from checkboxes from page

            // generate manifest with the manifest provider from sever
            ModuleManifest manifest = _manifestProvider.GenerateManifest(adder.AssemblyFilePath,
                                                                         adder.VirtualFolderPath);

            // use packager as service, with more options about packing
            var zipPackager = new ZipPackager();

            // use add ModuleInfo to module repository (storage layer under repository should make everything work)
            _repositoryModel.AddModule(new VirtualModuleInfo
                                           {
                                               // maybe this should be generated automatically
                                               Id = manifest.ModuleName + manifest.ModuleVersion,
                                               Manifest = manifest,
                                               ModuleData =
                                                   zipPackager.Package(adder.VirtualFolderPath),
                                           });

            // dispose of module adder to free virtual directory
            adder.Dispose();

            return RedirectToAction("Index");
        }

        #endregion
    }
}