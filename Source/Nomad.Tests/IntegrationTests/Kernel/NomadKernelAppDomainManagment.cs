using Moq;
using Nomad.Core;
using Nomad.Modules;
using NUnit.Framework;
using TestsShared;


namespace Nomad.Tests.IntegrationTests.Kernel
{
    /// <summary>
    /// Integrates Module, ModuleInfo, Nomad.Kernel -> checks for proper implementation of appDomains.
    /// </summary>
    [IntegrationTests]
    public class NomadKernelAppDomainManagment
    {
        private Core.NomadKernel _nomadKernel;


        [SetUp]
        public void set_up()
        {
            _nomadKernel = new Core.NomadKernel();
        }


        [Test]
        public void creating_module_appdomain_upon_module_loading()
        {
            var expectedModuleInfos = new[]
                                          {
                                              new ModuleInfo("a"),
                                              new ModuleInfo("b")
                                          };

            var moduleDiscoveryMock = new Mock<IModuleDiscovery>(MockBehavior.Loose);
            moduleDiscoveryMock.Setup(x => x.GetModules())
                .Returns(expectedModuleInfos);

            //TODO: write nomad kernel loading... 
            //_nomadKernel.ModuleManager.LoadModules(moduleDiscoveryMock.Object);

        }


        [Test]
        public void unloading_module_appdomain_upon_unload_request()
        {
            //TODO: write tests
        }


        [Test]
        public void verifing_starting_appdomain_to_have_not_module_loading_implementation_loaded()
        {
            //TODO: write tests
        }
    }
}