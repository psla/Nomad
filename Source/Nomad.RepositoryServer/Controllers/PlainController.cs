using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Nomad.RepositoryServer.Controllers
{
    public class PlainController : Controller 
    {
        public ActionResult UploadPackage(HttpPostedFileBase httpPostedFileBase)
        {
            if (httpPostedFileBase == null)
                return RedirectToAction("Index", "Home");

            // save file into memory stream
            using (var memoryStream = new MemoryStream())
            {
                while (httpPostedFileBase.InputStream.Position < httpPostedFileBase.InputStream.Length)
                {
                    var dataSinge = (byte) httpPostedFileBase.InputStream.ReadByte();
                    memoryStream.WriteByte(dataSinge);
                }

                // TODO: additional logic here

            }
            
            ViewData["Message"] = "The file is corrupted";
            return View("Error");
        }
    }
}