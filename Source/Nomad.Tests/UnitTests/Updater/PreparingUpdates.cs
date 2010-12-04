using System;
using System.Collections.Generic;
using Moq;
using Nomad.Messages;
using Nomad.Messages.Updating;
using Nomad.Modules.Manifest;
using Nomad.Updater;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.UnitTests.Updater
{
    [UnitTests]
    public class PreparingUpdates : UpdaterTestFixture
    {
        private ModuleManifest _moduleManifest;
        private string _moduleName;


        [SetUp]
        public void set_up()
        {
            _moduleName = "test_module.dll";
            var moduleVersion = new Nomad.Utils.Version("0.1.1.0");

            _moduleManifest = new ModuleManifest { ModuleName = _moduleName, ModuleVersion = moduleVersion };
        }

        [Test]
        public void packages_gotten_from_repository_have_nulls()
        {
            EventAggregator.Setup(x => x.Publish(It.IsAny<NomadUpdatesReadyMessage>()))
                 .Callback<NomadUpdatesReadyMessage>(msg => Assert.IsTrue(msg.Error))
                 .Verifiable("The message should be published");

            var modulePackages = new List<ModuleManifest>
                                     {
                                         new ModuleManifest() { ModuleName = "Test" },
                                     };

            // provide wrong packages in repository
            ModulesRepository.Setup(x => x.GetModule(It.IsAny<string>())).Returns(
                new ModulePackage());

            Updater.PrepareUpdate(modulePackages);

            EventAggregator.Verify();
            Assert.AreEqual(UpdaterStatus.Invalid, Updater.Status);
        }

        [Test]
        public void preparing_updates_invoked_with_null_raises_message()
        {
            EventAggregator.Setup(x => x.Publish(It.IsAny<NomadUpdatesReadyMessage>()))
                .Callback<NomadUpdatesReadyMessage>( msg => Assert.IsTrue(msg.Error))
                .Verifiable("The message should be published");

            Updater.PrepareUpdate(null);

            EventAggregator.Verify();
            Assert.AreEqual(UpdaterStatus.Invalid, Updater.Status);
        }

        [Test]
        public void module_repository_throws_exception_while_preparing_updates_changed_to_message()
        {
            ModulesRepository.Setup(x => x.GetModule(It.IsAny<string>()))
                .Throws(new Exception("Module Cannot be selected"));

            EventAggregator.Setup(x => x.Publish(It.IsAny<NomadUpdatesReadyMessage>()))
                .Callback<NomadUpdatesReadyMessage>(msg => Assert.IsTrue(msg.Error))
                .Verifiable("The message should be published");

            Updater.PrepareUpdate(new List<ModuleManifest>()
                                                                       {
                                                                           _moduleManifest
                                                                       });

            EventAggregator.Verify();
            Assert.AreEqual(UpdaterStatus.Invalid,Updater.Status);
        }
        
        [Test]
        public void preparing_updates_calls_proper_subsystems_publishes_msg_at_the_end()
        {            

            // return the same - might stop working if some of the code is gonna be put to verify download };
            var modulePacakge = new ModulePackage {ModuleManifest = _moduleManifest};

            ModulesRepository.Setup(x => x.GetModule(It.Is<string>(y => y == _moduleName)))
                .Returns(modulePacakge)
                .Verifiable("This package should be downloaded");

            EventAggregator.Setup(x => x.Publish(It.IsAny<NomadUpdatesReadyMessage>()))
                .Callback<NomadUpdatesReadyMessage>(msg =>
                                                        {
                                                            Assert.IsFalse(msg.Error);
                                                            Assert.AreEqual(1,msg.ModulePackages.Count);
                                                            Assert.AreSame(modulePacakge.ModuleManifest,
                                                                           msg.ModulePackages[0]);
                                                        })
                .Verifiable("This message should be published upon exit");

            Updater.PrepareUpdate(new List<ModuleManifest>()
                                                                       {
                                                                           _moduleManifest
                                                                       });

            EventAggregator.Verify();
            ModulesRepository.Verify();
            Assert.AreEqual(UpdaterStatus.Prepared, Updater.Status);
        }
        

    }
}