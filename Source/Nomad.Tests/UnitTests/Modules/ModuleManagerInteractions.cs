using System.Collections.Generic;
using System.Linq;
using Moq;
using Nomad.Modules;
using Nomad.Modules.Discovery;
using Nomad.Modules.Filters;
using Nomad.Modules.Manifest;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.UnitTests.Modules
{
    [UnitTests]
    public class ModuleManagerInteractions
    {
        private Mock<IDependencyChecker> _dependencyMock;
        private Mock<IModuleFilter> _moduleFilterMock;
        private Mock<IModuleManifestFactory> _moduleManifestMock;

        private ModuleManifest _simpleManifest;


        [SetUp]
        public void set_up()
        {
            _simpleManifest = new ModuleManifest();
            _moduleManifestMock = new Mock<IModuleManifestFactory>(MockBehavior.Loose);
            _moduleManifestMock.Setup(x => x.GetManifest(It.IsAny<ModuleInfo>())).Returns(
                _simpleManifest);

            _dependencyMock = new Mock<IDependencyChecker>(MockBehavior.Loose);
            _dependencyMock.Setup(x => x.SortModules(It.IsAny<IEnumerable<ModuleInfo>>()))
                .Returns<IEnumerable<ModuleInfo>>(e => e);

            _moduleFilterMock = new Mock<IModuleFilter>(MockBehavior.Loose);
            _moduleFilterMock.Setup(x => x.Matches(It.IsAny<ModuleInfo>())).Returns(true);
        }


        [Test]
        public void manager_delegates_module_loading_to_module_loader()
        {
            const string moduleAssemblyName = "module assembly name";
            var expectedModuleInfo = new ModuleInfo(moduleAssemblyName, _moduleManifestMock.Object);
            var loaderMock = new Mock<IModuleLoader>(MockBehavior.Strict);
            loaderMock.Setup(x => x.GetLoadedModules()).Returns(new List<ModuleInfo>());
            loaderMock.Setup(loader => loader.LoadModule(expectedModuleInfo))
                .Verifiable("Loader was not told to load requested module");

            var manager = new ModuleManager(loaderMock.Object, _moduleFilterMock.Object,
                                            _dependencyMock.Object);
            manager.LoadSingleModule(new ModuleInfo(moduleAssemblyName, _moduleManifestMock.Object));

            loaderMock.Verify();
        }


        [Test]
        public void manager_delegates_module_dependency_resolving_with_currently_loaded_modules()
        {
            const string moduleAssemblyName = "module assembly name";
            var expectedModuleInfo = new ModuleInfo(moduleAssemblyName, _moduleManifestMock.Object);
            var loaderMock = new Mock<IModuleLoader>(MockBehavior.Strict);
            loaderMock.Setup(x => x.GetLoadedModules()).Returns(new List<ModuleInfo>
                                                                    {
                                                                        new ModuleInfo("bzdura",
                                                                                       new ModuleManifest
                                                                                           {
                                                                                               ModuleName
                                                                                                   =
                                                                                                   "bzdura"
                                                                                           },
                                                                                       new ModuleManifestFactory
                                                                                           ())
                                                                    });
            loaderMock.Setup(
                x => x.LoadModule(It.Is<ModuleInfo>(y => y.AssemblyPath == moduleAssemblyName)));

            var dependencyCheckerMock = new Mock<IDependencyChecker>(MockBehavior.Loose);

            dependencyCheckerMock.Setup(x => x.SortModules(
                It.Is<IEnumerable<ModuleInfo>>((y) => y.Count(z => z.AssemblyPath == "bzdura") == 1)))
                .Returns<IEnumerable<ModuleInfo>>(y => y);

            var manager = new ModuleManager(loaderMock.Object, _moduleFilterMock.Object,
                                            dependencyCheckerMock.Object);

            var discoveryMock = new Mock<IModuleDiscovery>(MockBehavior.Loose);
            discoveryMock.Setup(x => x.GetModules()).Returns(new List<ModuleInfo>
                                                                 {
                                                                     new ModuleInfo(
                                                                         moduleAssemblyName,
                                                                         _moduleManifestMock.Object)
                                                                 });
            manager.LoadModules(discoveryMock.Object);

            dependencyCheckerMock.Verify(x => x.SortModules(
                It.Is<IEnumerable<ModuleInfo>>((y) => y.Count(z => z.AssemblyPath == "bzdura") == 1)),
                                         Times.Exactly(1));
        }


        [Test]
        public void manager_delegates_module_dependency_resolving_to_dependency_checker()
        {
            const string moduleAssemblyName = "module assembly name";
            var expectedModuleInfo = new ModuleInfo(moduleAssemblyName, _moduleManifestMock.Object);
            var loaderMock = new Mock<IModuleLoader>(MockBehavior.Loose);
            loaderMock.Setup(x => x.GetLoadedModules()).Returns(new List<ModuleInfo>());
            loaderMock.Setup(loader => loader.LoadModule(expectedModuleInfo));

            var dependencyCheckerMock = new Mock<IDependencyChecker>(MockBehavior.Strict);
            dependencyCheckerMock.Setup(x => x.SortModules(It.IsAny<IEnumerable<ModuleInfo>>()))
                .Returns<IEnumerable<ModuleInfo>>(e => e)
                .Verifiable("No sorting has been called upon loading");

            var manager = new ModuleManager(loaderMock.Object, _moduleFilterMock.Object,
                                            dependencyCheckerMock.Object);

            var discoveryMock = new Mock<IModuleDiscovery>(MockBehavior.Loose);
            discoveryMock.Setup(x => x.GetModules()).Returns(new List<ModuleInfo>
                                                                 {
                                                                     new ModuleInfo(
                                                                         moduleAssemblyName,
                                                                         _moduleManifestMock.Object)
                                                                 });
            manager.LoadModules(discoveryMock.Object);

            dependencyCheckerMock.Verify();
        }


        [Test]
        public void load_modules_loads_all_modules_provided_by_module_discovery()
        {
            var expectedModuleInfos = new[]
                                          {
                                              new ModuleInfo("a", _moduleManifestMock.Object),
                                              new ModuleInfo("b", _moduleManifestMock.Object)
                                          };

            var discoveryMock = new Mock<IModuleDiscovery>(MockBehavior.Loose);
            discoveryMock.Setup(discovery => discovery.GetModules())
                .Returns(expectedModuleInfos);

            var loaderMock = new Mock<IModuleLoader>(MockBehavior.Strict);
            loaderMock.Setup(x => x.GetLoadedModules()).Returns(new List<ModuleInfo>());

            loaderMock.Setup(loader => loader.LoadModule(expectedModuleInfos[0]))
                .Verifiable("First module was never loaded");

            loaderMock.Setup(loader => loader.LoadModule(expectedModuleInfos[1]))
                .Verifiable("Second module was never loaded");

            var moduleManager = new ModuleManager(loaderMock.Object, _moduleFilterMock.Object,
                                                  _dependencyMock.Object);

            moduleManager.LoadModules(discoveryMock.Object);

            loaderMock.Verify();
        }


        [Test]
        public void load_modules_only_loads_modules_that_pass_filter()
        {
            var a = new ModuleInfo("a", _moduleManifestMock.Object);
            var b = new ModuleInfo("b", _moduleManifestMock.Object);

            var expectedModuleInfos = new[]
                                          {
                                              a, b
                                          };

            var discoveryMock = new Mock<IModuleDiscovery>(MockBehavior.Loose);
            discoveryMock.Setup(discovery => discovery.GetModules())
                .Returns(expectedModuleInfos);

            var filterMock = new Mock<IModuleFilter>(MockBehavior.Loose);
            filterMock.Setup(filter => filter.Matches(a))
                .Returns(true);
            filterMock.Setup(filter => filter.Matches(b))
                .Returns(false);

            var loaderMock = new Mock<IModuleLoader>(MockBehavior.Strict);
            loaderMock.Setup(x => x.GetLoadedModules()).Returns(new List<ModuleInfo>());
            loaderMock.Setup(loader => loader.LoadModule(a));

            var moduleManager = new ModuleManager(loaderMock.Object, filterMock.Object,
                                                  _dependencyMock.Object);

            moduleManager.LoadModules(discoveryMock.Object);

            loaderMock.Verify(x => x.LoadModule(a));
            loaderMock.Verify(x => x.LoadModule(It.IsAny<ModuleInfo>()), Times.Exactly(1));
        }
    }
}