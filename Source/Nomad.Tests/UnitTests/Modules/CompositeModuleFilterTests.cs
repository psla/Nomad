using Moq;
using Nomad.Modules;
using Nomad.Modules.Filters;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.UnitTests.Modules
{
    [UnitTests]
    public class CompositeModuleFilterTests
    {
        private readonly ModuleInfo _moduleInfo = new ModuleInfo("test");
        private MockFactory _mockFactory;


        [SetUp]
        public void set_up()
        {
            _mockFactory = new MockFactory(MockBehavior.Loose);
        }


        private IModuleFilter CreateModuleFilter(bool value)
        {
            var mock = _mockFactory.Create<IModuleFilter>();
            mock.Setup(filter => filter.Matches(It.IsAny<ModuleInfo>()))
                .Returns(value);
            return mock.Object;
        }


        [Test]
        public void matches_when_all_subfilters_match()
        {
            var moduleFilters = new[]
                                    {
                                        CreateModuleFilter(true),
                                        CreateModuleFilter(true),
                                        CreateModuleFilter(true)
                                    };

            var compositeModuleFilter = new CompositeModuleFilter(moduleFilters);

            Assert.IsTrue(compositeModuleFilter.Matches(_moduleInfo),
                          "Should have matched the module");
        }


        [Test]
        public void doesnt_match_when_any_subfilter_doesnt_match()
        {
            var moduleFilters = new[]
                                    {
                                        CreateModuleFilter(true),
                                        CreateModuleFilter(false),
                                        CreateModuleFilter(true)
                                    };

            var compositeModuleFilter = new CompositeModuleFilter(moduleFilters);

            Assert.IsFalse(compositeModuleFilter.Matches(_moduleInfo),
                           "Shouldn't have matched the module");
        }


        [Test]
        public void empty_composite_filters_matches_any_module()
        {
            var compositeModuleFilter = new CompositeModuleFilter();
            Assert.IsTrue(compositeModuleFilter.Matches(_moduleInfo),
                          "Should have matched any module");
        }
    }
}