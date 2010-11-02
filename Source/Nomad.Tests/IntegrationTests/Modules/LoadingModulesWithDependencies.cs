using System.Collections.Generic;
using Castle.Windsor;
using Moq;
using Nomad.Exceptions;
using Nomad.Modules;
using Nomad.Modules.Discovery;
using Nomad.Modules.Filters;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.IntegrationTests.Modules
{
    [IntegrationTests]
    public class LoadingModulesWithDependencies
    {
        private IWindsorContainer _container;
        private ModuleLoader _moduleLoader;
        private ModuleManager _moduleManager;
        private Mock<IModuleDiscovery> _moduleDiscoveryMock;


        private void SetUpModuleDiscovery(IEnumerable<ModuleInfo> moduleInfos)
        {
            _moduleDiscoveryMock = new Mock<IModuleDiscovery>(MockBehavior.Loose);
            _moduleDiscoveryMock.Setup(x => x.GetModules())
                .Returns(moduleInfos);
        }

        [SetUp]
        public void set_up()
        {
            _container = new WindsorContainer();

            _moduleLoader = new ModuleLoader(_container);
            _moduleManager = new ModuleManager(_moduleLoader,
                                               new CompositeModuleFilter(new IModuleFilter[] {}));
        }


        [Test]
        public void loading_module_with_no_dependency()
        {
            //prepare modules for tests

            //preapre IModuleDiscovery fur tests             
            var expectedModules = new ModuleInfo[]
                                      {
                                          //TODO: put modules infos here
                                      };
            
            SetUpModuleDiscovery(expectedModules);

            //TODO: implement this test 
            //Assert.Throws<NomadCouldNotLoadModuleException>(
            //    () => _moduleManager.LoadModules(_moduleDiscoveryMock.Object));
        }
    }
}