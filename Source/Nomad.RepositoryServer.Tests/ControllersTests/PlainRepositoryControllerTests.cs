using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Ionic.Zip;
using Moq;
using Nomad.KeysGenerator;
using Nomad.Modules.Manifest;
using Nomad.RepositoryServer.Controllers;
using Nomad.Signing;
using Nomad.Signing.FileUtils;
using Nomad.Utils;
using Nomad.Utils.ManifestCreator;
using Nomad.Utils.ManifestCreator.DependenciesProvider;
using Nomad.Utils.ManifestCreator.FileSignerProviders;
using Nomad.Utils.ManifestCreator.VersionProviders;
using NUnit.Framework;
using TestsShared;
using File = System.IO.File;
using Version = Nomad.Utils.Version;

namespace Nomad.RepositoryServer.Tests.ControllersTests
{
    [IntegrationTests]
    public class PlainRepositoryControllerTests
    {
        private const string FolderPath = @"IntegrationTests\Server\ModuleController";
        private PlainController _plainController;

        [TestFixtureSetUp]
        public void set_up_fixture()
        {
            if(Directory.Exists(FolderPath))
                Directory.Delete(FolderPath,true);

            Directory.CreateDirectory(FolderPath);
        }

        [SetUp]
        public void set_up()
        {
            _plainController = new PlainController();
        }

        [Test]
        public void publishing_proper_zip_file_results_in_redirect_to_index()
        { 
            // some remarkable constancies, we are using sample module from psake build
            const string issuerName = @"TEST_ISSUER";
            const string issuerXmlPath = @"TEST_XML_KEY_FILE.xml";
            const string assemblyName = @"Modules\Simple\SimplestModulePossible1.dll";

            // get the key file
            KeysGeneratorProgram.Main(new []{Path.Combine(FolderPath,issuerXmlPath)});

            // get the assembly file into test folder
            File.Copy(assemblyName,Path.Combine(FolderPath,Path.GetFileName(assemblyName)));
            
            // NOTE: we are using here default builder configuration for simplicity of the test
            var manifestBuilder = new ManifestBuilder(issuerName,
                                                      Path.Combine(FolderPath, issuerXmlPath),
                                                      Path.GetFileName(assemblyName), FolderPath, KeyStorage.Nomad,
                                                      string.Empty, ManifestBuilderConfiguration.Default);

            manifestBuilder.Create();

            var memoryStream = new MemoryStream();

            // make zip out of this folder 
            var zipFilePath = Path.GetTempFileName();
            File.Delete(zipFilePath);
            using(var zipFile = new ZipFile(zipFilePath))
            {
                foreach(var f in Directory.GetFiles(FolderPath))
                {
                    zipFile.AddFile(f, ".");
                }

                zipFile.Save(memoryStream);
            }


            // set up http posted file context
            var file = new Mock<HttpPostedFileBase>(MockBehavior.Loose);
            file.Setup(x => x.FileName)
                .Returns("package.zip");
            file.Setup(x => x.InputStream)
                .Returns(memoryStream)
                .Verifiable("The data must be read from file");

            var testedController = new PlainController();

            ActionResult result = testedController.UploadPackage(file.Object);

            Assert.IsInstanceOf(typeof(ViewResult), result, "Returned no view");
            var resutlView = (ViewResult)result;
            Assert.AreEqual("Index", resutlView.ViewName);
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