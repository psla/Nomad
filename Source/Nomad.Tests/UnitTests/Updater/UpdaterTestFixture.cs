using System;
using System.IO;
using Moq;
using Nomad.Communication.EventAggregation;
using Nomad.Core;
using Nomad.Modules;
using Nomad.Modules.Discovery;
using Nomad.Modules.Manifest;
using Nomad.Updater;
using Nomad.Updater.ModulePackagers;
using Nomad.Updater.ModuleRepositories;
using NUnit.Framework;

namespace Nomad.Tests.UnitTests.Updater
{
    public class UpdaterTestFixture
    {
        protected Mock<IModuleDiscovery> ModuleDiscovery;
        
        protected Mock<IEventAggregator> EventAggregator;

        protected Mock<IModuleManifestFactory> ModuleManifestFactory;
        protected Mock<IModulesOperations> ModulesOperations;
        protected Mock<IModulesRepository> ModulesRepository;
        protected Mock<IModulePackager> ModulePackager;
        protected Mock<IDependencyChecker> DependencyChecker;
        protected string PluginsDir;

        /// <summary>
        ///     System under test
        /// </summary>
        protected Nomad.Updater.NomadUpdater NomadUpdater;


        [SetUp]
        public void Setup()
        {
            // initialize mock
            ModulesRepository = new Mock<IModulesRepository>();
            ModulesOperations = new Mock<IModulesOperations>();
            ModuleDiscovery = new Mock<IModuleDiscovery>();
            ModuleManifestFactory = new Mock<IModuleManifestFactory>();
            EventAggregator = new Mock<IEventAggregator>(MockBehavior.Loose);
            DependencyChecker = new Mock<IDependencyChecker>(MockBehavior.Loose);
            ModulePackager = new Mock<IModulePackager>();
            
            PluginsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpdateTests");

            // directory clean up
            if (Directory.Exists(PluginsDir))
                Directory.Delete(PluginsDir, true);
            Directory.CreateDirectory(PluginsDir);

            NomadUpdater = new Nomad.Updater.NomadUpdater(PluginsDir, ModulesRepository.Object,
                                                      ModulesOperations.Object,
                                                      ModuleManifestFactory.Object,
                                                      EventAggregator.Object, ModulePackager.Object,DependencyChecker.Object);
        }
    }
}