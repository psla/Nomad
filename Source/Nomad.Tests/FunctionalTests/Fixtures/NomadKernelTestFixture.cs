using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Nomad.Modules;
using Nomad.Modules.Discovery;
using Nomad.Modules.Filters;
using Nomad.Tests.FunctionalTests.Kernel.Messages;
using NUnit.Framework;

namespace Nomad.Tests.FunctionalTests.Fixtures
{
    public class NomadKernelTestFixture
    {
        protected Mock<IDependencyChecker> _dependencyCheckerMock;
        protected Mock<IModuleFilter> _moduleFilterMock;


        [TestFixtureSetUp]
        public void set_up_fixture()
        {
            _moduleFilterMock = new Mock<IModuleFilter>(MockBehavior.Loose);
            _moduleFilterMock.Setup(x => x.Matches(It.IsAny<ModuleInfo>())).Returns(true);

            _dependencyCheckerMock = new Mock<IDependencyChecker>(MockBehavior.Loose);
            _dependencyCheckerMock.Setup(x => x.SortModules(It.IsAny<IEnumerable<ModuleInfo>>())).
                Returns<IEnumerable<ModuleInfo>>(e => e);
        }


        protected static IModuleDiscovery SetUpDiscovery(params ModuleInfo[] expectedModules)
        {
            var mock = new Mock<IModuleDiscovery>(MockBehavior.Loose);

            mock.Setup(x => x.GetModules()).Returns(expectedModules.Select(module => module).ToList);

            return mock.Object;
        }


        protected class MessageCarrier : MarshalByRefObject
        {
            private readonly IList<string> _list;


            public MessageCarrier()
            {
                _list = EventHandledRegistry.GetRegisteredTypes()
                    .Select(type => type.Name).ToList();
            }


            public IEnumerable<string> List
            {
                get { return _list; }
            }
        }
    }
}