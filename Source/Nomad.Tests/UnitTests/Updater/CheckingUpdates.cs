using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Nomad.Messages.Updating;
using Nomad.Modules;
using Nomad.Modules.Manifest;
using Nomad.Updater;
using NUnit.Framework;
using TestsShared;
using Version = Nomad.Utils.Version;

namespace Nomad.Tests.UnitTests.Updater
{
    [UnitTests]
    public class CheckingUpdates : UpdaterTestFixture
    {
        [Test]
        public void publishes_error_message_when_checking_for_updates_failed()
        {
            ModulesRepository.Setup(x => x.GetAvailableModules())
                .Throws(new Exception("Can not have these modules"));

            EventAggregator.Setup(x => x.Publish(It.IsAny<NomadAvailableUpdatesMessage>()))
                .Callback<NomadAvailableUpdatesMessage>(
                    (msg) =>
                        {
                            Assert.IsTrue(msg.Error);
                            Assert.AreEqual(0, msg.AvailableUpdates.Count);
                        })
                    .Verifiable("Message was not published");

            Assert.DoesNotThrow(() => Updater.CheckUpdates());

            EventAggregator.Verify();
        }


        [Test]
        public void publishes_avaliable_modules_message_when_everything_is_ok()
        {
            const string assembly = "test_assembly.dll";
            var moduleInfo = new ModuleInfo(assembly);

            var moduleManifest = new ModuleManifest
                                     {
                                         ModuleName = assembly,
                                         ModuleVersion = new Version("0.0.0.0")
                                     };

            var updateManifest = new ModuleManifest
                                     {
                                         ModuleName = assembly,
                                         ModuleVersion = new Version("0.0.0.1")
                                     };

            ModuleDiscovery.Setup(x => x.GetModules()).Returns(new List<ModuleInfo>
                                                                   {
                                                                       moduleInfo
                                                                   });

            // FIXME: this should use internal manifest factory
            ModuleManifestFactory.Setup(x => x.GetManifest(It.Is<ModuleInfo>(y => y == moduleInfo)))
                .Returns(moduleManifest);


            EventAggregator.Setup(x => x.Publish(It.IsAny<NomadAvailableUpdatesMessage>()))
                .Callback<NomadAvailableUpdatesMessage>(msg =>
                                                         {
                                                             Assert.AreEqual(1,
                                                                             msg.AvailableUpdates.
                                                                                 Count);
                                                             Assert.AreEqual(updateManifest,
                                                                             msg.AvailableUpdates.
                                                                                 First());
                                                         })
                .Verifiable("Message was not published");

            ModulesRepository
                .Setup(x => x.GetAvailableModules())
                .Returns(new AvailableModules
                             {
                                 Manifests = new List<ModuleManifest> {updateManifest}
                             })
                .Verifiable("Get Avaliable modules has not been called");

            Updater.CheckUpdates();

            ModulesRepository.Verify();
            EventAggregator.Verify();
        }

    }
}