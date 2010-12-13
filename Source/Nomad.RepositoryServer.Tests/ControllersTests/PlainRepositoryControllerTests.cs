using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Moq;
using Nomad.Modules.Manifest;
using Nomad.RepositoryServer.Controllers;
using Nomad.Signing.FileUtils;
using Nomad.Utils;
using NUnit.Framework;
using TestsShared;
using File = System.IO.File;
using Version = Nomad.Utils.Version;

namespace Nomad.RepositoryServer.Tests.ControllersTests
{
    [IntegrationTests]
    public class PlainRepositoryControllerTests
    {
        private PlainController _plainController;

        [SetUp]
        public void set_up()
        {
            _plainController = new PlainController();
        }

        [Test]
        public void publishing_proper_zip_file_results_in_redirect_to_index()
        { 
            // TODO: should I implement this test, this is pretty solid one?

            //// set up simplest possible manifest
            //var manifest = new ModuleManifest
            //                   {
            //                       Issuer = @"TEST_ISSUER",
            //                       ModuleName = @"SimplesModulePossible1",
            //                       ModuleVersion = new Version("1.0.0.0"),
            //                       ModuleDependencies = new List<ModuleDependency>(),
            //                       SignedFiles = new List<SignedFile>()
            //                   };
            //var manifestData = XmlSerializerHelper.Serialize(manifest);
            //File.WriteAllBytes(@"manifestPath",manifestData);
    

            //// TODO: provide module package

            //byte[] fileData = null;

            //// set up http posted file context
            //var file = new Mock<HttpPostedFileBase>(MockBehavior.Loose);
            //file.Setup(x => x.FileName)
            //    .Returns("package.zip");
            //file.Setup(x => x.InputStream)
            //    .Returns(new MemoryStream(fileData))
            //    .Verifiable("The data must be read from file");

            //var testedController = new PlainController();

            //ActionResult result = testedController.UploadPackage(file.Object);

            //Assert.IsInstanceOf(typeof(ViewResult),result,"Reutrned no view");
            //var resutlView = (ViewResult) result;
            //Assert.AreEqual("Index",resutlView.ViewName);

        }

        [Test]
        public void publishing_module_thorugh_post_wth_no_file_results_in_redirection_to_index()
        {
            ActionResult result =_plainController.UploadPackage(null);
            
            var redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult,"Expected redirect result");
            Assert.AreEqual("Index",redirectResult.RouteValues["action"],"Redirect action should be");
        }

        [Test]
        public void publishing_corrupted_data_results_in_failure_page()
        {
            // corrupted file
            var fileData = new byte[6] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            // set up http posted file context
            var file = new Mock<HttpPostedFileBase>(MockBehavior.Loose);
            file.Setup(x => x.FileName)
                .Returns("corruptedFile.zip");
            file.Setup(x => x.InputStream)
                .Returns(new MemoryStream(fileData))
                .Verifiable("The data must be read from file");

            // act
            ActionResult result = _plainController.UploadPackage(file.Object);

            // assert
            file.Verify();
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult,"Expected view");
            Assert.AreEqual("Error", viewResult.ViewName);
            Assert.AreEqual("The file is corrupted",viewResult.ViewData["Message"]);

        }
    }
}