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
using Nomad.RepositoryServer.Models;
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
        private Mock<IStorageProvider> _storageMock;
        private RepositoryModel _repositoryModel;
        private ZipPackager _zipPackager;
        

        [SetUp]
        public void set_up()
        {
            if (Directory.Exists(FolderPath))
                Directory.Delete(FolderPath, true);

            Directory.CreateDirectory(FolderPath);

            _storageMock = new Mock<IStorageProvider>();
            _repositoryModel = new RepositoryModel(_storageMock.Object);
            _zipPackager = new ZipPackager();
            _plainController = new PlainController(_repositoryModel,_zipPackager);
            
        }

        [TearDown]
        public void tear_down()
        {
            Directory.Delete(FolderPath,true);
        }

        private static void BuildModule()
        {
            // some remarkable constancies, we are using sample module from psake build
            const string issuerName = @"TEST_ISSUER";
            const string issuerXmlPath = @"TEST_XML_KEY_FILE.xml";
            const string assemblyName = @"Modules\Simple\SimplestModulePossible1.dll";

            // get the key file
            KeysGeneratorProgram.Main(new[] { Path.Combine(FolderPath, issuerXmlPath) });

            // get the assembly file into test folder
            File.Copy(assemblyName, Path.Combine(FolderPath, Path.GetFileName(assemblyName)),true);

            // NOTE: we are using here default builder configuration for simplicity of the test
            var manifestBuilder = new ManifestBuilder(issuerName,
                                                      Path.Combine(FolderPath, issuerXmlPath),
                                                      Path.GetFileName(assemblyName), FolderPath, KeyStorage.Nomad,
                                                      string.Empty, ManifestBuilderConfiguration.Default);

            manifestBuilder.Create();
        }

        [Test]
        public void publishing_proper_zip_file_results_in_redirect_to_index()
        { 
            // prepare files for zipping
            BuildModule();

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

            // set up storage context in the plain controller
            _storageMock
                .Setup(x => x.GetAvaliableModules())
                .Returns(new List<IModuleInfo>())
                .Verifiable("This method should be called, by repository model!");

            _storageMock
                .Setup(x => x.SaveModule(It.IsAny<IModuleInfo>()))
                .Verifiable("This method should be called upon publishing file");

            // set up http posted file context
            var file = new Mock<HttpPostedFileBase>(MockBehavior.Loose);
            file.Setup(x => x.FileName)
                .Returns("package.zip");
            file.Setup(x => x.InputStream)
                .Returns(memoryStream)
                .Verifiable("The data must be read from file");

            // act
            var result = _plainController.UploadPackage(file.Object) as RedirectToRouteResult;

            // assert the gotten redirect action
            Assert.NotNull(result,"The redirect action should be returned");
            Assert.AreEqual("Index", result.RouteValues["action"], "Redirect action should be");
            
            // assert the changes in storage - checking if add was called
            _storageMock.Verify();
        }

        [Test]
        public void publishing_zip_file_with_missing_data_results_in_failure_page()
        {
            // prepare files for zipping
            BuildModule();

            var memoryStream = new MemoryStream();

            // build wrong zip (missing manifest)
            var zipFilePath = Path.GetTempFileName();
            File.Delete(zipFilePath);
            using (var zipFile = new ZipFile(zipFilePath))
            {
                foreach (var f in Directory.GetFiles(FolderPath))
                {
                    // dont add manifest for the package - number one error
                    if(f.EndsWith(ModuleManifest.ManifestFileNameSuffix))
                        continue;

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

            // act
            var result = _plainController.UploadPackage(file.Object) as ViewResult;

            // assert the gotten redirect action
            Assert.NotNull(result, "Expected view");
            Assert.AreEqual("Error", result.ViewName);
            Assert.AreEqual("No manifest file in package", result.ViewData["Message"]);

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