using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Ionic.Zip;
using Nomad.KeysGenerator;
using Nomad.Modules.Manifest;
using Nomad.RepositoryServer.Models;
using Nomad.Signing;
using Nomad.Utils.ManifestCreator;
using Version = Nomad.Utils.Version;

namespace Nomad.RepositoryServer.Controllers
{
    /// <summary>
    ///     Handles the display of the actual state of the repository. Manages the CRUD operations on repository.
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

        #region Adding Module
        /// <summary>
        ///     Initializes the whole process of adding module to repository.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddModule(HttpPostedFileBase file)
        {
            // FIXME: issue with loosing track of virtual modules (various uploads then cancels) (session sope?)
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


            return View("AddModule",moduleAdder);
        }

        /// <summary>
        ///     Adds files to virtual folder. Is a mid step in adding module to repository.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
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

            // pass the new VirtualModuleAdder into the View once again.
            return View("AddModule",adder);
        }

        /// <summary>
        ///     Publishes module into repository
        /// </summary>
        /// <remarks>
        ///     Performs the following actions:
        /// 1. Manifest building with normal options through <see cref="ManifestBuilder"/>
        /// 2. Packs the files in virtual folder into ZIP file
        /// 3. Adds all necessary information to repository using <see cref="IStorageProvider"/>
        /// </remarks>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PublishModule(FormCollection form)
        {
            // get module adder from session
            object moduleAdder = Session["ModuleAdder"];
            if (moduleAdder == null)
                return View("Error");

            var adder = (VirtualModuleAdder)moduleAdder;

            var listOfFilesInPackage = new List<string>();
            // get list of files to include into package (assembly,asc,manifest are required) from checkboxes from page
            foreach (var item in form)
            {
                 // TODO: implement getting this work properly  
            }

            // build manifest for virtual folder
            var configuration = ManifestBuilderConfiguration.Default;
            var issuerPathXml = @"ISSUER_PATH";
            KeysGeneratorProgram.Main(new[]{issuerPathXml} );
            var manifestBuilder = new ManifestBuilder("ISSUER_NAME_HERE", issuerPathXml,
                                                      adder.AssemblyFilePath,
                                                      adder.VirtualFolderPath, KeyStorage.Nomad,
                                                      string.Empty, configuration);
            var manifest = manifestBuilder.Create();

            // pack the things from the form into zip file TODO: encapsulate this
            string tmpZipFile = Path.GetTempFileName();
            using (var zipFile = new ZipFile())
            {

                var directoryInfo = new DirectoryInfo(adder.VirtualFolderPath);

                // get all files from this directory into zip archive
                foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                {
                    zipFile.AddFile(fileInfo.FullName, ".");
                }

                zipFile.Save(tmpZipFile);
            }

            // use add ModuleInfo to module repository (storage layer under repository should make everything good)
            _repositoryModel.AddModule(new VirtualModuleInfo()
                                           {
                                               // maybe this should be generated automatically
                                               Id = manifest.ModuleName + manifest.ModuleVersion, 
                                               Manifest =  manifest,
                                               ModuleData = System.IO.File.ReadAllBytes(tmpZipFile),
                                           });

            // dispose of adder and zip file
            System.IO.File.Delete(tmpZipFile);
            adder.Dispose();

            return RedirectToAction("Index");
        }

        #endregion

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
            if(selectedModel == null)
                return View("FileNotFound");

            if(selectedModel.Manifest == null || selectedModel.ModuleData == null)
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
            // TODO: implement removing item from repository if error show special view

            return RedirectToAction("Index");
        }

        
    }
}