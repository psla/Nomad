using System;
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
    /// Only <see cref="IModulesRepository"/> mocked, for not hitting the web.
    /// </summary>
    [FunctionalTests]
    public class UpdaterLifetimeTests : Fixtures.ModuleLoadingWithCompilerTestFixture
    {
        private Mock<IModulesRepository> _moduleRepository;
        private NomadConfiguration _configuration;
        private Random _random = new Random();


        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _moduleRepository = new Mock<IModulesRepository>(MockBehavior.Loose);

            // get the basic of configuration (common place)
            _configuration = NomadConfiguration.Default;
            _configuration.ModuleRepository = _moduleRepository.Object;
            _configuration.ModuleDirectoryPath = @"UpdaterLifetime\ModulesDir" + _random.Next();
            
        }

        /// <summary>
        ///     Sets the kernel up with the provided in <see cref="_configuration"/> configuration.
        /// </summary>
        private void SetUpKernel()
        {
            Kernel = new NomadKernel(_configuration);

            Domain = Kernel.KernelAppDomain;
        }


        [Test]
        public void resolve_updater_thorugh_service_locator_after_nomad_start_up_and_unload()
        {
            // invokde settuping kernel with current state
            SetUpKernel();

            Assert.DoesNotThrow(() => Kernel.ServiceLocator.Resolve<IUpdater>());

            Kernel.UnloadModules();
            Assert.DoesNotThrow(() => Kernel.ServiceLocator.Resolve<IUpdater>());

            var discovery = new CompositeModuleDiscovery();

            Kernel.LoadModules(discovery);
            Assert.DoesNotThrow(() => Kernel.ServiceLocator.Resolve<IUpdater>());

            Kernel.UnloadModules();
            Assert.DoesNotThrow(() => Kernel.ServiceLocator.Resolve<IUpdater>());
        }
        
        [Test]
        public void basic_usage_scenerio_with_newer_versions_avaliable_automatic_update()
        {
            //  override kernel configuration
            _configuration.UpdaterType = UpdaterType.Automatic;
            SetUpKernel();

            // prepare modules for discovery (compile, etc)
            string updaterDir = _configuration.ModuleDirectoryPath + @"\UpdateModule";
            string moduleA = _configuration.ModuleDirectoryPath + @"\ModuleA";
            string moduleB = _configuration.ModuleDirectoryPath + @"\ModuleB";

            // compile with specific version and so on.

            // test againts loading

            // test against updating
        }

        [Test]
        public void basic_usage_scenerio_with_newer_versions_avaliable_manual_update()
        {
            // override kernel configuration
            _configuration.UpdaterType = UpdaterType.Manual;
            SetUpKernel();

        }
        
        //TODO: wrtie more complicated scenarios with some failures about update and so on
    }
}