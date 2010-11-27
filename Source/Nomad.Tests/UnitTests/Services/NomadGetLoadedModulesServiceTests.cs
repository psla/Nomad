using System.Collections.Generic;
using Moq;
using Nomad.Modules;
using Nomad.Services;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.UnitTests.Services
{
    [UnitTests]
    public class NomadGetLoadedModulesServiceTests
    {
        [Test]
        public void empty_domain_returns_empty_list()
        {
            var moduleLoaderMock = new Mock<IModuleLoader>(MockBehavior.Loose);
            moduleLoaderMock.Setup(x => x.GetLoadedModules()).Returns(
                new List<ModuleInfo>());

            var loadedModulesService = new LoadedModulesService(moduleLoaderMock.Object);

            Assert.AreEqual(0,loadedModulesService.GetLoadedModules().Count);
        }


        [Test]
        public void loaded_modules_appear_on_list()
        {
            var moduleLoaderMock = new Mock<IModuleLoader>(MockBehavior.Loose);

            var expectedList = new List<ModuleInfo>
                                   {
                                       new ModuleInfo("Module1.dll"),
                                       new ModuleInfo("Module2.dll")
                                   };
            moduleLoaderMock.Setup(x => x.GetLoadedModules()).Returns(expectedList);

            var loadedModulesService = new LoadedModulesService(moduleLoaderMock.Object);

            Assert.AreEqual(expectedList, loadedModulesService.GetLoadedModules());
        }
    }
}