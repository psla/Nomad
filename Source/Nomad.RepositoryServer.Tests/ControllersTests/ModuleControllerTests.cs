using System.Collections.Generic;
using System.Web.Mvc;
using Moq;
using Nomad.Modules.Manifest;
using Nomad.RepositoryServer.Controllers;
using Nomad.RepositoryServer.Models;
using NUnit.Framework;
using TestsShared;

namespace Nomad.RepositoryServer.Tests.ControllersTests
{
    [IntegrationTests]
    public class ModuleControllerTests
    {
        private ModulesController _controller;
        private Mock<IStorageProvider> _mockedStorageProvider;
        private RepositoryModel _repositoryModel;


        [SetUp]
        public void set_up()
        {
            _mockedStorageProvider = new Mock<IStorageProvider>();
            _repositoryModel = new RepositoryModel(_mockedStorageProvider.Object);
            _controller = new ModulesController(_repositoryModel);
        }


        [Test]
        public void get_modules_gives_always_valid_xml()
        {
            _mockedStorageProvider.Setup(x => x.GetAvaliableModules())
                .Returns(new List<IModuleInfo>())
                .Verifiable("Access to repository model should be done.");

            FileResult fileResult = null;

            // the result of action is FileResult class and has XML MIME type
            Assert.DoesNotThrow(() => fileResult = (FileResult) _controller.GetModules());
            Assert.AreEqual("text/xml", fileResult.ContentType);

            // assert the data resulted by action 
            _mockedStorageProvider.Verify();

            // TODO: provide the way to test the outcome of this code ? or not
        }


        [Test]
        public void get_existing_module_reutns_a_file()
        {
            // arrange
            const string stringId = "#TEST_ID";
            ArrangeStorage(stringId);

            // act 
            ActionResult result = _controller.GetModulePackage(stringId);
            var fileResult = result as FileResult;

            // assert
            Assert.NotNull(fileResult, "The result should be file");
            Assert.AreEqual("application/zip", fileResult.ContentType,
                            "File content should be set to zip");
        }

        [Test]
        public void getting_null_element_returns_file_not_found_view()
        {
            var viewResut = _controller.GetModulePackage(null) as ViewResult;
            Assert.NotNull(viewResut,"The view result should be returned");
            Assert.AreEqual("FileNotFound",viewResut.ViewName);
        }

        [Test]
        public void get_non_existing_module_returns_file_not_found_view()
        {
            // arrange the repository without this
            const string existingId = "#TEST_ID";
            const string nonExistingId = "TEST_ID_2";
            ArrangeStorage(existingId);

            // act
            var viewResult = _controller.GetModulePackage(nonExistingId) as ViewResult;

            // assert
            Assert.NotNull(viewResult, "The view resutl should be returned");
            Assert.AreEqual("FileNotFound", viewResult.ViewName);
        }

        #region Helper Methods

        private void ArrangeStorage(string stringId)
        {
            var moduleInfo = new ModuleInfo
                                 {
                                     Id = stringId,
                                     ModuleData = new byte[] {0xFF},
                                     Manifest = new ModuleManifest()
                                 };

            _mockedStorageProvider.Setup(x => x.GetAvaliableModules())
                .Returns(new List<IModuleInfo>
                             {
                                 moduleInfo
                             });
        }

        #endregion
    }
}