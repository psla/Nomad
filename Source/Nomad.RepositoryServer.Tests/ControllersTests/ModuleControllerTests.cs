using System.Collections.Generic;
using System.Web.Mvc;
using Moq;
using Nomad.RepositoryServer.Controllers;
using Nomad.RepositoryServer.Models;
using NUnit.Framework;
using TestsShared;

namespace Nomad.RepositoryServer.Tests.ControllersTests
{
    [IntegrationTests]
    public class ModuleControllerTests
    {
        [Test]
        public void get_modules_gives_always_valid_xml()
        {
            var mockedStorageProvider = new Mock<IStorageProvider>();

            mockedStorageProvider.Setup(x => x.GetAvaliableModules())
                .Returns(new List<IModuleInfo>())
                .Verifiable("Access to repository model should be done.");

            var repositoryModel = new RepositoryModel(mockedStorageProvider.Object);

            var controller = new ModulesController(repositoryModel);

            FileResult fileResult = null;

            // the result of action is FileResult class and has XML MIME type
            Assert.DoesNotThrow(() => fileResult = (FileResult) controller.GetModules());
            Assert.AreEqual("text/xml", fileResult.ContentType);

            // assert the data resulted by action 
            mockedStorageProvider.Verify();

            // TODO: provide the way to test the outcome of this code ?
        }
    }
}