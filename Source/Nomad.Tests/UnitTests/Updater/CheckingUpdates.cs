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
            ModuleDiscovery.Setup(x => x.GetModules())
                .Throws(new Exception("Can not have these modules"));

            EventAggregator.Setup(x => x.Publish(It.IsAny<NomadAvailableUpdatesMessage>()))
                .Callback<NomadAvailableUpdatesMessage>(
                    (msg) =>
                        {
                            Assert.IsTrue(msg.Error);
                            Assert.AreEqual(0, msg.AvailableUpdates.Count);
                        })
                    .Verifiable("Message was not published");

            Assert.DoesNotThrow(() => Updater.CheckUpdates(ModuleDiscovery.Object));

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

            Updater.CheckUpdates(ModuleDiscovery.Object);

            ModulesRepository.Verify();
            EventAggregator.Verify();
        }

        [Test]
        public void invokes_publish_message_if_passed_null_as_discovery()
        {
            EventAggregator.Setup(x => x.Publish(It.IsAny<NomadAvailableUpdatesMessage>()))
               .Callback<NomadAvailableUpdatesMessage>(
                   (msg) =>
                   {
                       Assert.IsTrue(msg.Error);
                       Assert.AreEqual(0, msg.AvailableUpdates.Count);
                   })
                   .Verifiable("Message was not published");

            Updater.CheckUpdates(null);
        }

        #region Obsolete

        //[Test]
        //public void does_not_inform_about_modules_which_have_same_version()
        //{
        //    AvailableUpdatesEventArgs payload = null;
        //    string assembly = "test_assembly.dll";
        //    var moduleInfo = new ModuleInfo(assembly);
        //    var moduleManifest = new ModuleManifest
        //    {
        //        ModuleName = assembly,
        //        ModuleVersion = new Version("0.0.0.1")
        //    };
        //    var updateManifest = new ModuleManifest
        //    {
        //        ModuleName = assembly,
        //        ModuleVersion = new Version("0.0.0.1")
        //    };
        //    ModuleDiscovery.Setup(x => x.GetModules()).Returns(new List<ModuleInfo>
        //                                                            {
        //                                                                moduleInfo
        //                                                            });
        //    ModuleManifestFactory.Setup(x => x.GetManifest(It.Is<ModuleInfo>(y => y == moduleInfo)))
        //        .Returns(moduleManifest);

        //    ModulesRepository.
        //        Setup(x => x.GetAvailableModules()).
        //        Returns(new AvailableModules
        //        {
        //            Manifests = new List<ModuleManifest> { updateManifest }
        //        });

        //    EventAggregator.Subscribe<AvailableUpdatesEventArgs>(x => payload = x);
        //    Updater.CheckUpdates();

        //    Assert.AreEqual(0, payload.AvailableUpdates.Count, "There should be no updates");
        //}


        //[Test]
        //public void adds_dependencies_which_are_not_installed_during_update()
        //{
        //    UpdatesReadyEventArgs payload = null;
        //    string moduleName = "test_module.dll";
        //    string dependencyModuleName = "other_module.dll";

        //    var updateManifest = new ModuleManifest
        //    {
        //        ModuleName = moduleName,
        //        ModuleVersion = new Version("0.0.0.1"),
        //        ModuleDependencies = new List<ModuleDependency>
        //                                                          {
        //                                                              new ModuleDependency
        //                                                                  {
        //                                                                      MinimalVersion =
        //                                                                          new Version(
        //                                                                          "0.0.0.0"),
        //                                                                      ModuleName =
        //                                                                          dependencyModuleName
        //                                                                  }
        //                                                          }
        //    };
        //    var dependencyModuleManifest = new ModuleManifest { ModuleName = dependencyModuleName };
        //    var availableUpdates =
        //        new AvailableUpdatesEventArgs(new List<ModuleManifest> { updateManifest });

        //    ModulesRepository.Setup(x => x.GetModule(It.Is<string>(y => y == moduleName))).Returns(
        //        new ModulePackage { ModuleManifest = updateManifest });

        //    ModulesRepository.Setup(x => x.GetModule(It.Is<string>(y => y == dependencyModuleName)))
        //        .Returns(
        //            new ModulePackage { ModuleManifest = dependencyModuleManifest });

        //    EventAggregator.Subscribe<UpdatesReadyEventArgs>(x => payload = x);
        //    Updater.PrepareUpdate(availableUpdates);

        //    Assert.AreEqual(2, payload.ModulePackages.Count,
        //                    "There should be two modules to update. Module, and module dependency");
        //    Assert.Contains(updateManifest,
        //                    payload.ModulePackages.Select(x => x.ModuleManifest).ToList());
        //    Assert.Contains(dependencyModuleManifest,
        //                    payload.ModulePackages.Select(x => x.ModuleManifest).ToList());
        //}

        #endregion
    }
}