using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Nomad.RepositoryServer.Models;

namespace Nomad.RepositoryServer.Controllers
{
    public class PlainController : Controller 
    {
        private readonly RepositoryModel _repositoryModel;
        private readonly ZipPackager _zipPackager;


        public PlainController(RepositoryModel repositoryModel, ZipPackager zipPackager)
        {
            _repositoryModel = repositoryModel;
            _zipPackager = zipPackager;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadPackage(HttpPostedFileBase file)
        {
            if (file == null)
                return RedirectToAction("Index", "Home");

            // save file into memory stream
            using (var memoryStream = new MemoryStream())
            {
                file.InputStream.Position = 0;
                while (file.InputStream.Position < file.InputStream.Length)
                {
                    var dataSinge = (byte) file.InputStream.ReadByte();
                    memoryStream.WriteByte(dataSinge);
                }

                // get the module info from stream
                try
                {
                    IModuleInfo moduleInfo = _zipPackager.UnPack(memoryStream.ToArray());
                    _repositoryModel.AddModule(moduleInfo);
                }
                catch (Exception e)
                {
                    ViewData["Message"] = e.Message;
                    return View("Error");
                }
            }
            
            return RedirectToAction("Index","Home");
        }
    }
}