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
    /// </remarks>
    [FunctionalTests]
    public class UpdaterLifetimeTests : ModuleLoadingWithCompilerTestFixture
    {
        private const string TestSpacePath = @"UpdaterLifetime\";

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

            _moduleRepository = new Mock<IModulesRepository>(MockBehavior.Loose);

            // get the basic of configuration (common place)
            _configuration = NomadConfiguration.Default;
            _configuration.ModuleRepository = _moduleRepository.Object;
            _configuration.ModuleDirectoryPath = Path.Combine(TestSpacePath, "ModulesDir_") +
                                                 _random.Next();
        }


        /// <summary>
        ///     Sets the kernel up with the provided in <see cref="_configuration"/> configuration.
        /// </summary>
        private void SetUpKernel()
        {
            Kernel = new NomadKernel(_configuration);

            Domain = Kernel.KernelAppDomain;
        }


        private IModuleDiscovery SetUpModulesWithVersion(string directory,
                                                         string versionString)
        {
            string moduleA = directory + @"\ModuleA";
            string moduleB = directory + @"\ModuleB";

            ManifestBuilderConfiguration builderConfiguration = ManifestBuilderConfiguration.Default;
            builderConfiguration.VersionProvider = GetVersionProviderForVersion(versionString);

            SetUpModuleWithManifest(moduleA, ModuleCompiler.DefaultSimpleModuleSource,
                                    builderConfiguration);
            SetUpModuleWithManifest(moduleB, ModuleCompiler.DefaultSimpleModuleSourceAlternative,
                                    builderConfiguration);

            return new CompositeModuleDiscovery(new DirectoryModuleDiscovery(moduleA, SearchOption.TopDirectoryOnly),
                                                new DirectoryModuleDiscovery(moduleB, SearchOption.TopDirectoryOnly));
        }

        #endregion

        private readonly Random _random = new Random();
        private NomadConfiguration _configuration;
        private Mock<IModulesRepository> _moduleRepository;


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
            string updaterDir = _configuration.ModuleDirectoryPath;
            SetUpModuleWithManifest(updaterDir,
                                    @"..\Source\Nomad.Tests\FunctionalTests\Data\Updater\UpdaterModule.cs"); 

            // version for modules A, B and C
            const string versionString = "1.0.0.0";
           
            // create modules A with version v0 (this version and dependencies are only in manifest - not in assembly)
            var moduelADir = Path.Combine(_configuration.ModuleDirectoryPath, @"ModuleADir");
            var moduleAConfiguration = ManifestBuilderConfiguration.Default;
            
            moduleAConfiguration.VersionProvider = GetVersionProviderForVersion(versionString);
            moduleAConfiguration.ModulesDependenciesProvider = GetModuleDependenciesOnSingleModule("SimplestModulePossible2",versionString);
            
            SetUpModuleWithManifest(moduelADir,ModuleCompiler.DefaultSimpleModuleSource,moduleAConfiguration);

            // create module B with the same setting as A (with version v0)
            var moduelBDir = Path.Combine(_configuration.ModuleDirectoryPath, @"ModuleBDir");
            moduleAConfiguration = ManifestBuilderConfiguration.Default;

            moduleAConfiguration.VersionProvider = GetVersionProviderForVersion(versionString);
            moduleAConfiguration.ModulesDependenciesProvider = GetModuleDependenciesOnSingleModule("SimplestModulePossible3", versionString);

            SetUpModuleWithManifest(moduelBDir, ModuleCompiler.DefaultSimpleModuleSourceAlternative, moduleAConfiguration);

            // create module C with no dependency on any other module with version v0
            var moduleCDir = Path.Combine(_configuration.ModuleDirectoryPath, @"ModuleCDir");
            moduleAConfiguration = ManifestBuilderConfiguration.Default;

            moduleAConfiguration.VersionProvider = GetVersionProviderForVersion(versionString);
            SetUpModuleWithManifest(moduleCDir, ModuleCompiler.DefaultSimpleModuleSourceLastAlternative, moduleAConfiguration);

            // create module B in version v1 which depends on module C in version v1
            var moduleBVersionUpperDir = Path.Combine(_configuration.ModuleDirectoryPath,@"ModuleBUpperDir");
            moduleAConfiguration = ManifestBuilderConfiguration.Default;

            moduleAConfiguration.VersionProvider = GetVersionProviderForVersion("2.0.0.0");
            moduleAConfiguration.ModulesDependenciesProvider = GetModuleDependenciesOnSingleModule("SimplestModulePossible3", "2.0.0.0");
            
            SetUpModuleWithManifest(moduleBVersionUpperDir, ModuleCompiler.DefaultSimpleModuleSourceAlternative, moduleAConfiguration);

            // put module B into repository
            var bRepoModuleInfo = new DirectoryModuleDiscovery(moduleBVersionUpperDir, SearchOption.TopDirectoryOnly)
                .GetModules()
                .Select(x => x)
                .Single();


            _moduleRepository.Setup(x => x.GetAvailableModules())
                .Returns(new AvailableModules(new List<ModuleManifest>(){bRepoModuleInfo.Manifest}));

            _moduleRepository
                .Setup(x => x.GetModule(It.IsAny<string>()))
                .Returns(new ModulePackage()
                             {
                                 ModuleManifest = bRepoModuleInfo.Manifest,
                                 ModuleZip = GetZippedData(new List<ModuleInfo>(){bRepoModuleInfo},bRepoModuleInfo.Manifest.ModuleName )
                             });

            // configure kernel
            _configuration.UpdaterType = UpdaterType.Automatic;
            SetUpKernel();
            
            // load modules A,B,C in version v0 into Nomad
            var discovery = new CompositeModuleDiscovery(new DirectoryModuleDiscovery(moduelADir, SearchOption.TopDirectoryOnly),
                                                         new DirectoryModuleDiscovery(moduelBDir, SearchOption.TopDirectoryOnly),
                                                         new DirectoryModuleDiscovery(moduleCDir, SearchOption.TopDirectoryOnly),
                                                         new DirectoryModuleDiscovery(updaterDir, SearchOption.TopDirectoryOnly));
            Kernel.LoadModules(discovery);

            // register for updates avaliable message
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


       


        [Test]
        public void basic_usage_scenerio_with_newer_versions_avaliable_automatic_update()
        {
            // create the updater module
            string updaterDir = _configuration.ModuleDirectoryPath;
            SetUpModuleWithManifest(updaterDir,
                                    @"..\Source\Nomad.Tests\FunctionalTests\Data\Updater\UpdaterModule.cs");

            // set up two simple modules
            IModuleDiscovery v0Discovery = SetUpTwoSimpleModulesGetTheirDiscovery();

            //  override kernel configuration and initialize kernel
            _configuration.UpdaterType = UpdaterType.Automatic;
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
            Kernel.EventAggregator.Subscribe<NomadAvailableUpdatesMessage>(
                message =>
                    {
                        if (message.Error == false)
                            avaliableUpdates = true;
                    });

            bool readyUpdates = false;
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
            Assert.IsTrue(avaliableUpdates, "Avaliable updates message was not published.");
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
           
            // create the updater module for maual testing
            string updaterDir = _configuration.ModuleDirectoryPath;
            SetUpModuleWithManifest(updaterDir,
                                    @"..\Source\Nomad.Tests\FunctionalTests\Data\Updater\UpdaterModuleManual.cs");

            // set up two simple modules
            IModuleDiscovery basicDiscovery = SetUpTwoSimpleModulesGetTheirDiscovery();

            // override kernel configuration
            _configuration.UpdaterType = UpdaterType.Manual;
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
            Assert.AreEqual(2, loadedModules.Count); // updater + simples module 2 ;)
            AssertVersion("2.0.0.0", loadedModules, "SimplestModulePossible2");
        }

        #region Helper methods

        private IModuleDiscovery SetUpTwoSimpleModulesGetTheirDiscovery()
        {
            string repositoryDir = Path.Combine(TestSpacePath, "Repository");

            // create the modules with specific version (mocking the version provider) and use the special manifest builder
            IModuleDiscovery v0Discovery =
                SetUpModulesWithVersion(_configuration.ModuleDirectoryPath, "1.0.0.0");

            // preapre module for this test with verions v1 (only of module A and module B) and put them into repository
            IModuleDiscovery v1Discovery = SetUpModulesWithVersion(repositoryDir, "2.0.0.0");

            // putting them into repo
            var updateModuleInfos = new List<ModuleInfo>(v1Discovery.GetModules());
            List<ModuleManifest> updateManifests = (from moduleInfo in updateModuleInfos
                                                    select moduleInfo.Manifest).ToList();

            _moduleRepository.Setup(x => x.GetAvailableModules())
                .Returns(new AvailableModules(updateManifests));

            _moduleRepository.Setup(
                x => x.GetModule(It.IsAny<string>()))
                .Returns<string>(name => new ModulePackage
                {
                    ModuleManifest = updateModuleInfos
                        .Where(x => x.Manifest.ModuleName.Equals(name))
                        .Select(x => x.Manifest)
                        .Single(),
                    ModuleZip = GetZippedData(updateModuleInfos,
                                              name)
                });
            return v0Discovery;
        }

        private static void AssertVersion(string version, IEnumerable<ModuleInfo> modules,
                                          string moduleName)
        {
            string loadedVersion = modules
                .Where(x => x.Manifest.ModuleName.Equals(moduleName))
                .Select(x => x.Manifest.ModuleVersion)
                .Single()
                .ToString();

            Assert.AreEqual(version, loadedVersion);
        }


        private static byte[] GetZippedData(List<ModuleInfo> updateModuleInfos,
                                            string name)
        {
            string tmpFile = Path.GetTempFileName();
            using (var zipFile = new ZipFile())
            {
                // get the directory in which we have this module
                string asmPath =
                    updateModuleInfos.Where(x => x.Manifest.ModuleName.Equals(name)).Select(
                        x => x.AssemblyPath).Single();

                DirectoryInfo directoryInfo = new DirectoryInfo(asmPath).Parent;

                // get all files from this directory into zip archive
                foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                {
                    zipFile.AddFile(fileInfo.FullName,".");
                }

                zipFile.Save(tmpFile);
            }

            return File.ReadAllBytes(tmpFile);
        }

        private static IModulesDependenciesProvider GetModuleDependenciesOnSingleModule(string s, string versionString)
        {
            var moduleAonBDependency = new ModuleDependency()
            {
                HasLoadingOrderPriority = false,
                MinimalVersion = new Version(versionString),
                ModuleName = s,
                ProcessorArchitecture = ProcessorArchitecture.MSIL
            };

            var moduleDependencyProviderMock = new Mock<IModulesDependenciesProvider>(MockBehavior.Loose);
            moduleDependencyProviderMock
                .Setup(x => x.GetDependencyModules(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<ModuleDependency>() { moduleAonBDependency });

            return moduleDependencyProviderMock.Object;
        }

        private static IVersionProvider GetVersionProviderForVersion(string versionString)
        {
            var mockedVersionProvider = new Mock<IVersionProvider>(MockBehavior.Loose);
            mockedVersionProvider
                .Setup(x => x.GetVersion(It.IsAny<string>()))
                .Returns(new Version(versionString));

            return mockedVersionProvider.Object;
        }

        #endregion
    }

    [Serializable]
    public class BeginUpdateMessage
    {
    }
}