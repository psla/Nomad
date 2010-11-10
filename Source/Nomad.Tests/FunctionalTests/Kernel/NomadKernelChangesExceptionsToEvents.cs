using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using Nomad.Communication.Messages;
using Nomad.Core;
using Nomad.Exceptions;
using Nomad.KeysGenerator;
using Nomad.Modules;
using Nomad.Modules.Discovery;
using Nomad.Modules.Filters;
using Nomad.Tests.FunctionalTests.Kernel.Messages;
using Nomad.Tests.FunctionalTests.Modules;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.Kernel
{
    [FunctionalTests]
    public class NomadKernelChangesExceptionsToEvents
    {
        private Mock<IDependencyChecker> _dependencyCheckerMock;
        private Mock<IModuleFilter> _moduleFilterMock;


        [TestFixtureSetUp]
        public void set_up_fixture()
        {
            _moduleFilterMock = new Mock<IModuleFilter>(MockBehavior.Loose);
            _moduleFilterMock.Setup(x => x.Matches(It.IsAny<ModuleInfo>())).Returns(true);

            _dependencyCheckerMock = new Mock<IDependencyChecker>(MockBehavior.Loose);
            _dependencyCheckerMock.Setup(x => x.SortModules(It.IsAny<IEnumerable<ModuleInfo>>())).
                Returns<IEnumerable<ModuleInfo>>(e => e);
        }


        private static IModuleDiscovery SetUpDiscovery(params ModuleInfo[] expectedModules)
        {
            var mock = new Mock<IModuleDiscovery>(MockBehavior.Loose);

            mock.Setup(x => x.GetModules()).Returns(expectedModules.Select(module => module).ToList);

            return mock.Object;
        }


        [Test]
        public void
            exception_during_module_loading_is_changed_into_event_visible_from_module_and_kernel()
        {
            // set up configuration of kernel with mocks
            NomadConfiguration configuration = NomadConfiguration.Default;
            configuration.ModuleFilter = _moduleFilterMock.Object;
            configuration.DependencyChecker = _dependencyCheckerMock.Object;

            var kernel = new NomadKernel(configuration);

            //  compile module for event aggregation
            const string dir = @"Modules\Kernel\Event\";
            const string fileName = "event.dll";
            const string keyFile = @"newkey.xml";

            var compiler = new ModuleCompiler();

            compiler.OutputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir);
            compiler.OutputName = Path.Combine(compiler.OutputDirectory, fileName);

            string modulePath = compiler.GenerateModuleFromCode(
                @"..\Source\Nomad.Tests\FunctionalTests\Data\Kernel\EventAwareModule.cs");

            KeysGeneratorProgram.Main(new[] {keyFile});
            compiler.GenerateManifestForModule(modulePath, keyFile);

            // set up module which subscribes for event
            IModuleDiscovery setUpDiscovery = SetUpDiscovery(new ModuleInfo(modulePath));
            kernel.LoadModules(setUpDiscovery);

            // set up the discovery mock
            IModuleDiscovery nonExistingDiscovery =
                SetUpDiscovery(new ModuleInfo("NonExistingModule.dll"));

            // perform test
            Assert.Throws<NomadCouldNotLoadModuleException>(() => kernel.LoadModules(nonExistingDiscovery),
                                "Exception should be thrown in kernel domain.");

            //verify the method being called in a module.
            var carrier = (MessageCarrier) kernel.ModuleAppDomain.CreateInstanceAndUnwrap(
                typeof (MessageCarrier).Assembly.FullName, typeof (MessageCarrier).FullName);

            Assert.AreEqual(new[]{"EventAwareModule"}, carrier.List.ToArray());

        }

        #region Nested type: MessageCarrier

        private class MessageCarrier : MarshalByRefObject
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

        #endregion
    }
}