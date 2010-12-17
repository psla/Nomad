using System;
using System.Reflection;
using Moq;
using Nomad.Core;
using Nomad.Modules;
using Nomad.Modules.Filters;
using NUnit.Framework;
using TestsShared;
using System.Linq;

namespace Nomad.Tests.UnitTests.Kernel
{
    [UnitTests]
    public class NomadConfigurationTests
    {
        private NomadConfiguration _configuration;
        private Mock<IModuleFilter> _moduleFilterMock;


        [TestFixtureSetUp]
        public void setup_fixture()
        {
            _moduleFilterMock = new Mock<IModuleFilter>(MockBehavior.Loose);
            _moduleFilterMock.Setup(x => x.Matches(It.IsAny<ModuleInfo>()))
                .Verifiable("Matches was not invoked during test");
        }


        [Test]
        public void default_implementation_is_not_frozen()
        {
            _configuration = new NomadConfiguration();

            Assert.IsFalse(_configuration.IsFrozen);
        }


        [Test]
        public void unfrozen_configuration_is_fully_configurable()
        {
            _configuration = new NomadConfiguration();
            _configuration.ModuleFilter = _moduleFilterMock.Object;

            Assert.AreSame(_moduleFilterMock.Object, _configuration.ModuleFilter,
                           "IModuleFilter was not set");

            _configuration.ModuleFilter = null;

            Assert.IsNull(_configuration.ModuleFilter);
        }


        [Test]
        public void freezing_does_not_change_already_set_services()
        {
            _configuration = new NomadConfiguration();
            _configuration.ModuleFilter = _moduleFilterMock.Object;

            _configuration.Freeze();

            Assert.AreSame(_moduleFilterMock.Object, _configuration.ModuleFilter,
                           "Freezing changed IModuleFilter service provider");
        }


        [Test]
        public void freezing_prevents_from_using_setter()
        {
            _configuration = new NomadConfiguration();

            _configuration.ModuleFilter = _moduleFilterMock.Object;
            _configuration.Freeze();

            Assert.Throws<InvalidOperationException>(() => _configuration.ModuleFilter = null);

            Assert.AreSame(_moduleFilterMock.Object, _configuration.ModuleFilter,
                           "IModuleFilter was changed despite being frozen");
            Assert.NotNull(_configuration.ModuleFilter, "Freezing does not stop setter");
        }


        [Test]
        public void freezing_locks_all_properties()
        {
            _configuration = NomadConfiguration.Default;
            _configuration.Freeze();

            PropertyInfo[] setters = _configuration.GetType().GetProperties(BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.Public);
            foreach (PropertyInfo propertyInfo in setters.Where(x=>x.CanWrite))
            {
                Assert.Throws<InvalidOperationException>(
                    () =>
                    ThrowInnerException(() => propertyInfo.SetValue(_configuration, null, null))
                    , "Each setter should throw InvalidOperationException when configuration is frozen");
            }
        }


        private void ThrowInnerException(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    throw e.InnerException;
                else throw;
            }
        }
    }
}