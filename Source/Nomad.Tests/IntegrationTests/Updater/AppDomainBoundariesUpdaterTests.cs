using System;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using System.Threading;
using Moq;
using Nomad.Communication.EventAggregation;
using Nomad.Core;
using Nomad.Modules.Discovery;
using Nomad.Modules.Manifest;
using Nomad.Updater;
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
        private Nomad.Updater.Updater _updater;


        [SetUp]
        public void SetupFixture()
        {
            _appdomain = AppDomain.CreateDomain("Updater AppDomain",
                                         new Evidence(AppDomain.CurrentDomain.Evidence),
                                         AppDomain.CurrentDomain.BaseDirectory,
                                         ".",
                                         true);
            
            //_appdomain.Load(("Nomad.dll"));
        }


        [Test]
        public void resolve_updater_cross_domain()
        {
            //Assert.DoesNotThrow(() => _appdomain.DoCallBack(Operations.CreateUpdater), "Updater should support cross domain working");
            var asmName = typeof (Operations).Assembly.FullName;
            var typeName = typeof (Operations).FullName;
            Assert.DoesNotThrow(() => { var instance = Activator.CreateInstance(_appdomain, asmName, typeName);
                                          _updater = (instance.Unwrap() as Operations).Updater;
            },
                                "Updater should support cross domain working");
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
            var modulesOperations = new Mock<IModulesOperations>();
            var moduleDiscovery = new Mock<IModuleDiscovery>();
            var moduleManifestFactory = new Mock<IModuleManifestFactory>();
            var eventAggregator = new Mock<IEventAggregator>();
            Updater = new Nomad.Updater.Updater(targetDirectory, modulesRepository.Object,
                                                 modulesOperations.Object, moduleDiscovery.Object,
                                                 moduleManifestFactory.Object, eventAggregator.Object);
        }


        public Nomad.Updater.Updater Updater { get; set; }
    }
}