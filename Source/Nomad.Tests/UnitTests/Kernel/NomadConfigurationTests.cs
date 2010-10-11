using System;
using Moq;
using Nomad.Modules;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.UnitTests.Kernel
{
    [UnitTests]
    public class NomadConfigurationTests
    {
        private Mock<IModuleFilter> _moduleFilterMock;
        private Mock<IModuleLoader> _moduleLoaderMock;

        private Core.NomadConfiguration _configuration;

        [TestFixtureSetUp]
        public void setup_fixture()
        {
            _moduleFilterMock = new Mock<IModuleFilter>(MockBehavior.Loose);
            _moduleFilterMock.Setup( x => x.Matches(It.IsAny<ModuleInfo>()))
                .Verifiable("Matches was not invoked during test");

            _moduleLoaderMock = new Mock<IModuleLoader>(MockBehavior.Loose);
            _moduleLoaderMock.Setup(x => x.LoadModule(It.IsAny<ModuleInfo>()))
               .Verifiable("LoadModule was not invoked during test");
        }

        [Test]
        public void default_implementation_is_not_frozen()
        {
            //TODO: this is the question how the default implementation should behave..
            _configuration = new Core.NomadConfiguration();

            Assert.IsFalse(_configuration.IsFrozen);
        }

        [Test]
        public void unfrozen_configuration_is_fully_configurable()
        {
            _configuration = new Core.NomadConfiguration();
            _configuration.ModuleFilter = _moduleFilterMock.Object;
            _configuration.ModuleLoader = _moduleLoaderMock.Object;

            Assert.AreSame(_moduleFilterMock.Object, _configuration.ModuleFilter, "IModuleFilter was not set");
            Assert.AreSame(_moduleLoaderMock.Object, _configuration.ModuleLoader, "IModuleLoader was not set");

            _configuration.ModuleFilter = null;
            _configuration.ModuleLoader = null;

            Assert.IsNull(_configuration.ModuleFilter);
            Assert.IsNull(_configuration.ModuleLoader);

        }

        [Test]
        public void freezing_does_not_change_already_set_services()
        {
            _configuration = new Core.NomadConfiguration();
            _configuration.ModuleFilter = _moduleFilterMock.Object;
            _configuration.ModuleLoader = _moduleLoaderMock.Object;

            _configuration.Freeze();

            Assert.AreSame(_moduleFilterMock.Object,_configuration.ModuleFilter,"Freezing changed IModuleFilter service provider");
            Assert.AreSame(_moduleLoaderMock.Object, _configuration.ModuleLoader, "Freezing changed IModuleLoader service provider");
        }

        [Test]
        public void freezing_prevents_from_using_setter()
        {
            _configuration = new Core.NomadConfiguration();

            _configuration.ModuleFilter = _moduleFilterMock.Object;
            _configuration.Freeze();

            Assert.Throws<InvalidOperationException>(() => _configuration.ModuleFilter = null);

            Assert.AreSame(_moduleFilterMock.Object,_configuration.ModuleFilter,"IModuleFilter was changed despite being frozen");
            Assert.NotNull(_configuration.ModuleFilter,"Freezing does not stop setter");
        }
    }
}