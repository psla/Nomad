using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using System.Threading;
using Moq;
using Nomad.Communication.EventAggregation;
using Nomad.Core;
using Nomad.Modules;
using Nomad.Modules.Discovery;
using Nomad.Modules.Manifest;
using Nomad.Updater;
using Nomad.Updater.ModuleFinders;
using Nomad.Updater.ModulePackagers;
using Nomad.Updater.ModuleRepositories;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.IntegrationTests.Updater
{
    [IntegrationTests]
    public class AppDomainBoundariesUpdaterTests 
    {
        private AppDomain _appdomain;
        private string _targetDirectory;
        private Mock<IModulesRepository> _modulesRepository;
        private Mock<IModulesOperations> _modulesOperations;
        private Mock<IModuleDiscovery> _moduleDiscovery;
        private Mock<IModuleManifestFactory> _moduleManifestFactory;
        private Mock<IEventAggregator> _eventAggregator;
        private Nomad.Updater.NomadUpdater _nomadUpdater;


        [SetUp]
        public void SetupFixture()
        {
            _appdomain = AppDomain.CreateDomain("Updater AppDomain",
                                         new Evidence(AppDomain.CurrentDomain.Evidence),
                                         AppDomain.CurrentDomain.BaseDirectory,
                                         ".",
                                         true);
            
        }

        private void SetupUpdater()
        {
            var asmName = typeof(Operations).Assembly.FullName;
            var typeName = typeof(Operations).FullName; 
            var instance = Activator.CreateInstance(_appdomain, asmName, typeName);
            _nomadUpdater = (instance.Unwrap() as Operations).NomadUpdater;
        }

        [Test]
        public void resolve_updater_cross_domain()
        {
            Assert.DoesNotThrow(SetupUpdater, "Updater should support cross domain working");
        }

        [Test]
        public void update_event_is_cross_domain()
        {
            SetupUpdater();
            _nomadUpdater.CheckUpdates();
        }

        [TearDown]
        public void TearDown()
        {
            AppDomain.Unload(_appdomain);
            _appdomain = null;
        }
    }

    public class Operations : MarshalByRefObject
    {
        public Operations()
        {
            var targetDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var modulesRepository = new Mock<IModulesRepository>();
            modulesRepository.Setup(x => x.GetAvailableModules()).Returns(
                new AvailableModules(new List<ModuleManifest>()));
            var modulesOperations = new Mock<IModulesOperations>();
            var modulesFinder = new Mock<IModuleFinder>();
            var eventAggregator = new Mock<IEventAggregator>();
            var dependencyChecker = new Mock<IDependencyChecker>();
            var packager = new Mock<IModulePackager>();
            NomadUpdater = new NomadUpdater(targetDirectory, modulesRepository.Object,
                                                 modulesOperations.Object, eventAggregator.Object,
                                                 packager.Object,
                                                 dependencyChecker.Object,modulesFinder.Object);
        }

        public NomadUpdater NomadUpdater { get; set; }
    }
}