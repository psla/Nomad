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

        private Core.NomadConfiguration _configuration;

        [TestFixtureSetUp]
        public void setup_fixture()
        {
            _moduleFilterMock = new Mock<IModuleFilter>(MockBehavior.Loose);
            _moduleFilterMock.Setup( x => x.Matches(It.IsAny<ModuleInfo>()))
                .Verifiable("Matches was not invoked during test");
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

            Assert.AreSame(_moduleFilterMock.Object, _configuration.ModuleFilter, "IModuleFilter was not set");

            _configuration.ModuleFilter = null;

            Assert.IsNull(_configuration.ModuleFilter);

        }

        [Test]
        public void freezing_does_not_change_already_set_services()
        {
            _configuration = new Core.NomadConfiguration();
            _configuration.ModuleFilter = _moduleFilterMock.Object;

            _configuration.Freeze();

            Assert.AreSame(_moduleFilterMock.Object,_configuration.ModuleFilter,"Freezing changed IModuleFilter service provider");
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