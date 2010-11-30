using Moq;
using Nomad.Core;
using Nomad.Modules.Discovery;
using Nomad.Updater;
using Nomad.Updater.ModuleRepositories;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.Updater
{
    /// <summary>
    ///     Tests functionally (with <see cref="NomadKernel"/>) class as an entry point. Changing only configuration.
    /// Only <see cref="IModulesRepository"/> mocked.
    /// </summary>
    [FunctionalTests]
    public class UpdaterLifetimeTests : Fixtures.ModuleLoadingWithCompilerTestFixture
    {
        private Mock<IModulesRepository> _moduleRepository;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _moduleRepository = new Mock<IModulesRepository>(MockBehavior.Loose);

            var configuration = NomadConfiguration.Default;
            configuration.ModuleRepository = _moduleRepository.Object;
            configuration.ModuleDirectoryPath = @"UpdaterLifetime\ModulesDir";
            
            Kernel = new NomadKernel(configuration);

            Domain = Kernel.KernelAppDomain;
        }

        [Test]
        public void resolve_updater_thorugh_service_locator_after_nomad_start_up_and_unload()
        {
            IUpdater startingUpdater = null;
            IUpdater tmpUpdater = null;

            Assert.DoesNotThrow(() => startingUpdater = Kernel.ServiceLocator.Resolve<IUpdater>());

            Kernel.UnloadModules();
            Assert.DoesNotThrow(() => tmpUpdater = Kernel.ServiceLocator.Resolve<IUpdater>());

            // TODO: provide some additional information
            var discovery = new CompositeModuleDiscovery();

            Kernel.LoadModules(discovery);
            Assert.DoesNotThrow(() => tmpUpdater =Kernel.ServiceLocator.Resolve<IUpdater>());

            Kernel.UnloadModules();
            Assert.DoesNotThrow(() => tmpUpdater = Kernel.ServiceLocator.Resolve<IUpdater>());
        }
        
        [Test]
        public void basic_usage_scenerio_with_newer_versions_avaliable_automatic_update()
        {
            
        }

        [Test]
        public void basic_usage_scenerio_with_newer_versions_avaliable_manual_update()
        {
            
        }
        
        //TODO: wrtie more complicated scenarios with some failures about update and so on
    }
}