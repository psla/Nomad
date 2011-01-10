using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Ionic.Zip;
using Moq;
using Nomad.Core;
using Nomad.Messages.Updating;
using Nomad.Modules;
using Nomad.Modules.Discovery;
using Nomad.Modules.Manifest;
using Nomad.Services;
using Nomad.Tests.FunctionalTests.Fixtures;
using Nomad.Updater;
using Nomad.Updater.ModuleRepositories;
using Nomad.Utils.ManifestCreator;
using Nomad.Utils.ManifestCreator.DependenciesProvider;
using Nomad.Utils.ManifestCreator.VersionProviders;
using NUnit.Framework;
using TestsShared;
using Version = Nomad.Utils.Version;

namespace Nomad.Tests.FunctionalTests.Updater
{
    /// <summary>
    ///     Tests functionally (with <see cref="NomadKernel"/>) class as an entry point. Changing only configuration.
    /// Only <see cref="IModulesRepository"/> mocked, for not hitting the web.
    /// </summary>
    /// <remarks>
    ///     Some minor classes might be mocked like <see cref="IVersionProvider"/> from Manifest building.
    /// 
    ///     TODO: refactor this class into more managable one
    /// </remarks>
    [FunctionalTests]
    public class UpdaterLifetimeTests : UpdaterLifetimeTestsBase
    {
        #region SetUp Methods

        [TestFixtureSetUp]
        public override void SetUpFixture()
        {
            base.SetUpFixture();

            if (Directory.Exists(TestSpacePath))
            {
                foreach (var file in Directory.GetFiles(TestSpacePath))
                {
                    File.Delete(file);
                }
                Directory.Delete(TestSpacePath, true);
            }
                
            Directory.CreateDirectory(TestSpacePath);
        }


        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            ModulesRepository = new Mock<IModulesRepository>(MockBehavior.Loose);

            // get the basic of configuration (common place)
            NomadConfigurationSettings = NomadConfiguration.Default;
            NomadConfigurationSettings.ModuleRepository = ModulesRepository.Object;
            NomadConfigurationSettings.ModuleDirectoryPath = Path.Combine(TestSpacePath, "ModulesDir_") +
                                                 _random.Next();
        }

        #endregion

        private readonly Random _random = new Random();


        [Test]
        public void resolve_updater_thorugh_service_locator_after_nomad_start_up_and_unload()
        {
            // invoke setting up kernel with current state
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

       

        /// <summary>
        ///     Module A depends on B which depends on C. Only B is in repository with newer version.
        /// </summary>
        /// <remarks>
        ///     Using dependency checker way of naming things:
        ///     Local: A(v0) v0-> B(v0) v0-> C(v0)
        ///     Remote: B(v1) v1-> C (which is missing)
        /// </remarks>
        [Test]
        public void failing_update_beacause_of_the_missing_dependencies()
        {
            // create the updater module
            string updaterDir = NomadConfigurationSettings.ModuleDirectoryPath;
            SetUpModuleWithManifest(updaterDir,
                                    @"..\Source\Nomad.Tests\FunctionalTests\Data\Updater\UpdaterModule.cs"); 

            // version for modules A, B and C
            const string versionString = "1.0.0.0";
           
            // create modules A with version v0 (this version and dependencies are only in manifest - not in assembly)
            var moduelADir = Path.Combine(NomadConfigurationSettings.ModuleDirectoryPath, @"ModuleADir");
            var moduleAConfiguration = ManifestBuilderConfiguration.Default;
            
            moduleAConfiguration.VersionProvider = GetVersionProviderForVersion(versionString);
            moduleAConfiguration.ModulesDependenciesProvider = GetModuleDependenciesOnSingleModule("SimplestModulePossible2",versionString);
            
            SetUpModuleWithManifest(moduelADir,ModuleCompiler.DefaultSimpleModuleSource,moduleAConfiguration);

            // create module B with the same setting as A (with version v0)
            var moduelBDir = Path.Combine(NomadConfigurationSettings.ModuleDirectoryPath, @"ModuleBDir");
            moduleAConfiguration = ManifestBuilderConfiguration.Default;

            moduleAConfiguration.VersionProvider = GetVersionProviderForVersion(versionString);
            moduleAConfiguration.ModulesDependenciesProvider = GetModuleDependenciesOnSingleModule("SimplestModulePossible3", versionString);

            SetUpModuleWithManifest(moduelBDir, ModuleCompiler.DefaultSimpleModuleSourceAlternative, moduleAConfiguration);

            // create module C with no dependency on any other module with version v0
            var moduleCDir = Path.Combine(NomadConfigurationSettings.ModuleDirectoryPath, @"ModuleCDir");
            moduleAConfiguration = ManifestBuilderConfiguration.Default;

            moduleAConfiguration.VersionProvider = GetVersionProviderForVersion(versionString);
            SetUpModuleWithManifest(moduleCDir, ModuleCompiler.DefaultSimpleModuleSourceLastAlternative, moduleAConfiguration);

            // create module B in version v1 which depends on module C in version v1
            var moduleBVersionUpperDir = Path.Combine(NomadConfigurationSettings.ModuleDirectoryPath,@"ModuleBUpperDir");
            moduleAConfiguration = ManifestBuilderConfiguration.Default;

            moduleAConfiguration.VersionProvider = GetVersionProviderForVersion("2.0.0.0");
            moduleAConfiguration.ModulesDependenciesProvider = GetModuleDependenciesOnSingleModule("SimplestModulePossible3", "2.0.0.0");
            
            SetUpModuleWithManifest(moduleBVersionUpperDir, ModuleCompiler.DefaultSimpleModuleSourceAlternative, moduleAConfiguration);

            // put module B into repository
            var bRepoModuleInfo = new DirectoryModuleDiscovery(moduleBVersionUpperDir, SearchOption.TopDirectoryOnly)
                .GetModules()
                .Select(x => x)
                .Single();


            ModulesRepository.Setup(x => x.GetAvailableModules())
                .Returns(new AvailableModules(new List<ModuleManifest>(){bRepoModuleInfo.Manifest}));

            ModulesRepository
                .Setup(x => x.GetModule(It.IsAny<string>()))
                .Returns(new ModulePackage()
                             {
                                 ModuleManifest = bRepoModuleInfo.Manifest,
                                 ModuleZip = GetZippedData(new List<ModuleInfo>(){bRepoModuleInfo},bRepoModuleInfo.Manifest.ModuleName )
                             });

            // configure kernel
            NomadConfigurationSettings.UpdaterType = UpdaterType.Automatic;
            SetUpKernel();
            
            // load modules A,B,C in version v0 into Nomad
            var discovery = new CompositeModuleDiscovery(new DirectoryModuleDiscovery(moduelADir, SearchOption.TopDirectoryOnly),
                                                         new DirectoryModuleDiscovery(moduelBDir, SearchOption.TopDirectoryOnly),
                                                         new DirectoryModuleDiscovery(moduleCDir, SearchOption.TopDirectoryOnly),
                                                         new DirectoryModuleDiscovery(updaterDir, SearchOption.TopDirectoryOnly));
            Kernel.LoadModules(discovery);

            // register for updates available message
            var hasBeenInvoked = false;
            Kernel.EventAggregator.Subscribe<NomadUpdatesReadyMessage>(message =>
                                                                           {
                                                                               hasBeenInvoked = true;
                                                                               Assert.IsTrue(message.Error,"The error should be reported");
                                                                               
                                                                               // list of non valid modules is accurate
                                                                               Assert.AreEqual("SimplestModulePossible2", message.ModuleManifests[0].ModuleName);
                                                                           });

            // initialize check updates (automatic mode)
            Kernel.EventAggregator.Publish(new BeginUpdateMessage());

            // verify that update can not be done
            Assert.IsTrue(hasBeenInvoked,"The updates ready message was not invoked");
            Assert.AreEqual(UpdaterStatus.Invalid,Kernel.ServiceLocator.Resolve<IUpdater>().Status);
        }



        /// <summary>
        ///     Testing the process of the installation new, completely independent module.
        /// </summary>
        /// <remarks>
        ///     Modules used in this tests are simple module possible guys.
        /// </remarks>
        [Test]
        public void basic_update_scenario_when_installing_new_module()
        {
            // create the updater module
            string updaterDir = NomadConfigurationSettings.ModuleDirectoryPath;
            SetUpModuleWithManifest(updaterDir,
                                    @"..\Source\Nomad.Tests\FunctionalTests\Data\Updater\UpdaterModule.cs");

            // set up two simple modules -- to be loaded into kernel
            string modulaADir = Path.Combine(NomadConfigurationSettings.ModuleDirectoryPath,"moduleA");
            string modulaBDir = Path.Combine(NomadConfigurationSettings.ModuleDirectoryPath,"moduleB");
            SetUpModuleWithManifest(modulaADir,ModuleCompiler.DefaultSimpleModuleSource);
            SetUpModuleWithManifest(modulaBDir, ModuleCompiler.DefaultSimpleModuleSourceAlternative);
            var twoSimpleModules = new CompositeModuleDiscovery(
                new DirectoryModuleDiscovery(modulaADir, SearchOption.TopDirectoryOnly),
                new DirectoryModuleDiscovery(modulaBDir, SearchOption.TopDirectoryOnly)
                    );

            // set up third simple module -- completely independent to be placed in remote repository
            string moduleCDir = Path.Combine(NomadConfigurationSettings.ModuleDirectoryPath, "moduleC");
            SetUpModuleWithManifest(moduleCDir,ModuleCompiler.DefaultSimpleModuleSourceLastAlternative);
            

            // put the third one in repository
            var listOfModuleInRepositoryInfos = twoSimpleModules.GetModules();
            var listOfModuleInRepository = new List<ModuleManifest>(twoSimpleModules.GetModules().Select(x => x.Manifest));
          
            SetUpModulesRepository(listOfModuleInRepository,listOfModuleInRepositoryInfos);

            // initialize kernel 
            NomadConfigurationSettings.UpdaterType = UpdaterType.Automatic;
            SetUpKernel();

            // set up the subscribers for stages of update
            bool updatesChecked = false;
            bool updatesReady = false;
            Kernel.EventAggregator.Subscribe<NomadAvailableUpdatesMessage>((m) =>
                {
                    Assert.IsFalse(m.Error, "There should no error in checking");
                    updatesChecked = true;
                });

            Kernel.EventAggregator.Subscribe<NomadUpdatesReadyMessage>((m) =>
                {
                    Assert.IsFalse(m.Error, "There should no error in preparing");
                    updatesReady = true;
                });

            // load those two modules into kernel
            var discovery = new CompositeModuleDiscovery(twoSimpleModules,
                                                         new DirectoryModuleDiscovery(updaterDir,SearchOption.TopDirectoryOnly));
            Kernel.LoadModules(discovery);

            // invoke automatic update process
            var updater = Kernel.ServiceLocator.Resolve<IUpdater>();
            Kernel.EventAggregator.Publish(new BeginUpdateMessage());

            // check stages of update
            Assert.IsTrue(updatesChecked,"Updates checked message has never been invoked");
            Assert.IsTrue(updatesReady,"Updates ready message has never been invoked ");

            // wait for updater to finish and being in a good state
            Assert.NotNull(updater.UpdateFinished,"Update finshed object is null, meaning that no one has started perform update");
            updater.UpdateFinished.WaitOne();
            Assert.AreEqual(UpdaterStatus.Idle,Kernel.ServiceLocator.Resolve<IUpdater>().Status, "Problem with the state of the updater after reload");

            // assert that there are 4 modules installed and running - 3 simples and one updater
            var loadedModules = Kernel.ServiceLocator.Resolve<ILoadedModulesService>().GetLoadedModules();
            Assert.AreEqual(4, loadedModules.Count);
            
        }
        #region Manual / Automatic

        [Test]
        public void basic_usage_scenerio_with_newer_versions_avaliable_automatic_update()
        {
            // create the updater module
            string updaterDir = NomadConfigurationSettings.ModuleDirectoryPath;
            SetUpModuleWithManifest(updaterDir,
                                    @"..\Source\Nomad.Tests\FunctionalTests\Data\Updater\UpdaterModule.cs");

            // set up two simple modules
            IModuleDiscovery v0Discovery = SetUpTwoSimpleModulesGetTheirDiscovery();

            //  override kernel configuration and initialize kernel
            NomadConfigurationSettings.UpdaterType = UpdaterType.Automatic;
            SetUpKernel();

            // test against loading
            var discovery = new CompositeModuleDiscovery(new DirectoryModuleDiscovery(updaterDir, SearchOption.TopDirectoryOnly),
                                                         v0Discovery);

            Kernel.LoadModules(discovery);

            // verify the versions of the loaded modules are proper
            IList<ModuleInfo> loadedModules =
                Kernel.ServiceLocator.Resolve<ILoadedModulesService>().GetLoadedModules();
            Assert.AreEqual(3, loadedModules.Count);
            AssertVersion("1.0.0.0", loadedModules, "SimplestModulePossible1");
            AssertVersion("1.0.0.0", loadedModules, "SimplestModulePossible2");

            // check if all stages of update were done
            bool avaliableUpdates = false;
            bool readyUpdates = false;

            Kernel.EventAggregator.Subscribe<NomadAvailableUpdatesMessage>(
                message =>
                    {
                        if (message.Error == false)
                            avaliableUpdates = true;
                    });

            
            Kernel.EventAggregator.Subscribe<NomadUpdatesReadyMessage>(message =>
                                                                           {
                                                                               if (message.Error ==
                                                                                   false)
                                                                                   readyUpdates =
                                                                                       true;
                                                                           });

            var updater = Kernel.ServiceLocator.Resolve<IUpdater>();

            // initialize the updating through updater module using event aggregator and publish
            Kernel.EventAggregator.Publish(new BeginUpdateMessage());

            // verify stages
            Assert.IsTrue(avaliableUpdates, "Available updates message was not published.");
            Assert.IsTrue(readyUpdates, "Updates ready message was not published.");

            // verify the outcome of the updater after finishing (this wait is for test purposes)
            updater.UpdateFinished.WaitOne();
            Assert.AreEqual(UpdaterStatus.Idle, Kernel.ServiceLocator.Resolve<IUpdater>().Status, "Problem with the state of the updater");

            // verify the versions of the newest modules are loaded
            loadedModules = Kernel.ServiceLocator.Resolve<ILoadedModulesService>().GetLoadedModules();
            Assert.AreEqual(3, loadedModules.Count);
            AssertVersion("2.0.0.0", loadedModules, "SimplestModulePossible1");
            AssertVersion("2.0.0.0", loadedModules, "SimplestModulePossible2");
        }
       

        [Test]
        public void selective_manual_update_with_updater_wokring_on_threads()
        {
           
            // create the updater module for manual testing
            string updaterDir = NomadConfigurationSettings.ModuleDirectoryPath;
            SetUpModuleWithManifest(updaterDir,
                                    @"..\Source\Nomad.Tests\FunctionalTests\Data\Updater\UpdaterModuleManual.cs");

            // set up two simple modules
            IModuleDiscovery basicDiscovery = SetUpTwoSimpleModulesGetTheirDiscovery();

            // override kernel configuration
            NomadConfigurationSettings.UpdaterType = UpdaterType.Manual;
            SetUpKernel();

            // skip verification about loaded modules, just load them
            Kernel.LoadModules(new CompositeModuleDiscovery(basicDiscovery, new DirectoryModuleDiscovery(updaterDir, SearchOption.TopDirectoryOnly)));

            // get updater reference, for synchronization
            var updater = Kernel.ServiceLocator.Resolve<IUpdater>();

            // initialize the updating through updater module using event aggregator and publish
            Kernel.EventAggregator.Publish(new BeginUpdateMessage());
            
            // no need to verify the messages - just final verification of loaded modules wait for finish
            updater.UpdateFinished.WaitOne();

            var loadedModules = Kernel.ServiceLocator.Resolve<ILoadedModulesService>().GetLoadedModules();
            Assert.AreEqual(3, loadedModules.Count); // updater + simples ;)
            AssertVersion("1.0.0.0", loadedModules, "SimplestModulePossible1");
            AssertVersion("2.0.0.0", loadedModules, "SimplestModulePossible2");
        }

        #endregion        
    }

    [Serializable]
    public class BeginUpdateMessage
    {
    }
}