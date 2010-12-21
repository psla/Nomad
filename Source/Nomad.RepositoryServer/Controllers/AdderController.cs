using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Nomad.Modules.Manifest;
using Nomad.RepositoryServer.Models;
using Nomad.RepositoryServer.Models.ServerSigner;
using Nomad.Utils.ManifestCreator;

namespace Nomad.RepositoryServer.Controllers
{
    /// <summary>
    ///     Class representing old way of working of server.
    /// </summary>
    [Obsolete("Signing packages by server supported no more")]
    public class AdderController : Controller
    {
        private readonly IManifestProvider _manifestProvider;
        private readonly RepositoryModel _repositoryModel;


        public AdderController(IManifestProvider manifestProvider, RepositoryModel repositoryModel)
        {
            _manifestProvider = manifestProvider;
            _repositoryModel = repositoryModel;
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

            var adder = (VirtualModuleAdder)moduleAdder;

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

            var adder = (VirtualModuleAdder)moduleAdder;

            var listOfFilesInPackage = new List<string>();
            //TODO: get list of files to include into package (assembly,asc,manifest are required) from checkboxes from page

            // generate manifest with the manifest provider from sever
            ModuleManifest manifest = _manifestProvider.GenerateManifest(adder.AssemblyFilePath,
                                                                         adder.VirtualFolderPath);

            // use packager as service, with more options about packing
            var zipPackager = new ZipPackager();

            // use add ModuleInfo to module repository (storage layer under repository should make everything work)
            _repositoryModel.AddModule(new ModuleInfo
            {
                // maybe this should be generated automatically
                Id = manifest.ModuleName + manifest.ModuleVersion,
                Manifest = manifest,
                ModuleData =
                    zipPackager.Package(adder.VirtualFolderPath),
            });

            // dispose of module adder to free virtual directory
            adder.Dispose();

            return RedirectToAction("Index","Home");
        }

        #endregion
    }
}