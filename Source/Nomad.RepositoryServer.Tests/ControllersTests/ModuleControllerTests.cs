using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using Nomad.Modules.Manifest;
using Nomad.RepositoryServer.Controllers;
using Nomad.RepositoryServer.Models;
using Nomad.Updater.ModuleRepositories.WebRepositories;
using Nomad.Utils;
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
        private Mock<HttpRequestBase> _requestBaseMock;


        [SetUp]
        public void set_up()
        {
            _mockedStorageProvider = new Mock<IStorageProvider>();
            _repositoryModel = new RepositoryModel(_mockedStorageProvider.Object);
            _controller = new ModulesController(_repositoryModel);
            _requestBaseMock = new Mock<HttpRequestBase>();
        }


        [Test]
        public void get_all_avaliable_updates_then_get_one_of_the_avliable_updates()
        {
            // mock requestUrl
            MocRequestUrl();

            // set up two avaliable modules
            var dataTable1 = new byte[] {0x00};
            const string moduleName1 = "MODULE_NAME_1";
            var moduleInfo = new ModuleInfo
                                 {
                                     Id = "No1",
                                     Manifest = new ModuleManifest
                                                    {
                                                        ModuleName = moduleName1
                                                    },
                                     ModuleData = dataTable1
                                 };

            var dataTable2 = new byte[] {0xFF};
            const string moduleName2 = "MODULE_NAME_2";
            var moduleInfo2 = new ModuleInfo
                                  {
                                      Id = "No2",
                                      Manifest = new ModuleManifest
                                                     {
                                                         ModuleName = moduleName2
                                                     },
                                      ModuleData = dataTable2
                                  };

            // set up storage for module             
            _mockedStorageProvider
                .Setup(x => x.GetAvaliableModules())
                .Returns(new List<IModuleInfo>
                             {
                                 moduleInfo,
                                 moduleInfo2
                             });

            // get the xml
            var fileStreamResult = _controller.GetModules() as FileStreamResult;

            // read this xml using the repository client class
            var output =
                XmlSerializerHelper.Deserialize<WebAvailablePackagesCollection>(
                    GetDataFromStream(fileStreamResult));

            // get the one module (module No2) out of the xml file
            WebModulePackageInfo selectedModule = output.AvailablePackages
                .Where(x => x.Manifest.ModuleName.Equals(moduleName2))
                .Select(x => x)
                .Single();

            // get from url: 'xxx/yyy/zzzz./id' the id thing, assert the url has this format
            string[] splitedUrl = selectedModule.Url.Split('/');
            string selectedId = splitedUrl.Last();

            // perform so called "download" using the url - id provided
            var fileStreamModule =
                _controller.GetModulePackage(selectedId) as FileStreamResult;

            // assert recived file
            Assert.NotNull(fileStreamModule, "The resulted outcome should be file stream result");
            Assert.AreEqual(dataTable2, GetDataFromStream(fileStreamModule),
                            "The data saved in database and returned should be the same");
        }


        [Test]
        public void get_modules_gives_always_valid_xml()
        {
            // mock request thing
            MocRequestUrl();

            _mockedStorageProvider.Setup(x => x.GetAvaliableModules())
                .Returns(new List<IModuleInfo>())
                .Verifiable("Access to repository model should be done.");

            // the result of action is FileStreamResult class and has XML MIME type
            var streamResult = _controller.GetModules() as FileStreamResult;
           
            var output = XmlSerializerHelper.Deserialize<WebAvailablePackagesCollection>(GetDataFromStream(streamResult));

            // assert the data resulted by action 
            _mockedStorageProvider.Verify();

            Assert.AreEqual("text/xml", streamResult.ContentType);
            Assert.NotNull(streamResult, "Should be file stream returned");
            Assert.NotNull(output, "The deserialization of xml should be possible");
        }


        private void MocRequestUrl()
        {
            _requestBaseMock
                .Setup(x => x.RawUrl)
                .Returns("/xxx/yyy/controller/GetModules");
            var httpContextMock = new Mock<HttpContextBase>();
            httpContextMock.Setup(x => x.Request)
                .Returns(_requestBaseMock.Object);

            _controller.ControllerContext = new ControllerContext(httpContextMock.Object,
                                                                  new RouteData(), _controller);
        }


        private static byte[] GetDataFromStream(FileStreamResult streamResult)
        {
            streamResult.FileStream.Position = 0;
            using (var memoryStream = new MemoryStream())
            {
                while (streamResult.FileStream.Position < streamResult.FileStream.Length)
                {
                    memoryStream.WriteByte((byte) streamResult.FileStream.ReadByte());
                }
                return memoryStream.ToArray();
            }
        }


        [Test]
        public void get_existing_module_reutns_a_file()
        {
            // arrange
            const string stringId = "#TEST_ID";
            ArrangeStorage(stringId);

            // act 
            ActionResult result = _controller.GetModulePackage(stringId);
            var fileResult = result as FileStreamResult;

            // assert
            Assert.NotNull(fileResult, "The result should be file stream");
            Assert.AreEqual("application/zip", fileResult.ContentType,
                            "File content should be set to zip");
        }


        [Test]
        public void getting_null_element_returns_file_not_found_view()
        {
            var viewResut = _controller.GetModulePackage(null) as ViewResult;
            Assert.NotNull(viewResut, "The view result should be returned");
            Assert.AreEqual("FileNotFound", viewResut.ViewName);
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
                                     Manifest = new ModuleManifest(),
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