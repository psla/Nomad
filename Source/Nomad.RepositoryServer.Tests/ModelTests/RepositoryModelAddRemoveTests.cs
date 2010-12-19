using System;
using System.Collections.Generic;
using Moq;
using Nomad.Modules.Manifest;
using Nomad.RepositoryServer.Models;
using NUnit.Framework;
using TestsShared;

namespace Nomad.RepositoryServer.Tests.ModelTests
{
    /// <summary>
    ///     Testing fail safe operations of add and delete from model.
    /// </summary>
    /// <remarks>
    ///     NOTE: that this tests does not try to answer about thread - safety of the <see cref="RepositoryModel"/> class.
    /// </remarks>
    [IntegrationTests]
    public class RepositoryModelAddRemoveTests
    {
        private RepositoryModel _repositoryModel;
        private Mock<IStorageProvider> _storageMock;


        [SetUp]
        public void set_up()
        {
            _storageMock = new Mock<IStorageProvider>(MockBehavior.Loose);
            _repositoryModel = new RepositoryModel(_storageMock.Object);
        }


        private static Mock<IModuleInfo> GetDuplicateInfoMock()
        {
            var duplicateInfoMock = new Mock<IModuleInfo>();
            duplicateInfoMock.Setup(x => x.Id).Returns("1");
            duplicateInfoMock.Setup(x => x.Manifest).Returns(new ModuleManifest());
            duplicateInfoMock.Setup(x => x.ModuleData).Returns(new byte[] {0x00});
            return duplicateInfoMock;
        }


        private static Mock<IModuleInfo> GetModuleInfoMock()
        {
            var moduleInfoMock = new Mock<IModuleInfo>();
            moduleInfoMock.Setup(x => x.Id).Returns("1");
            moduleInfoMock.Setup(x => x.Manifest).Returns(new ModuleManifest());
            moduleInfoMock.Setup(x => x.ModuleData).Returns(new byte[] {0xFF});
            return moduleInfoMock;
        }

        #region Adding

        [Test]
        public void adding_duplicate_module_raises_exception()
        {
            // set up infos, equal only by id
            Mock<IModuleInfo> moduleInfoMock = GetModuleInfoMock();

            Mock<IModuleInfo> duplicateInfoMock = GetDuplicateInfoMock();

            // put one info into storage
            _storageMock.Setup(x => x.GetAvaliableModules()).Returns(new List<IModuleInfo>
                                                                         {
                                                                             moduleInfoMock.Object
                                                                         }).Verifiable(
                                                                             "List of modules should be accesed");

            // act and assert
            Assert.Throws<ArgumentException>(
                () => _repositoryModel.AddModule(duplicateInfoMock.Object));
        }


        [Test]
        public void adding_incomplete_module_raises_exception()
        {
            var moduleInfoMock = new Mock<IModuleInfo>(MockBehavior.Loose);
            moduleInfoMock
                .Setup(x => x.ModuleData)
                .Returns(new byte[] {0xFF});
            moduleInfoMock.
                Setup(x => x.Manifest)
                .Returns(() => null)
                .Verifiable("This method should be called");

            Assert.Throws<ArgumentException>(
                () => _repositoryModel.AddModule(moduleInfoMock.Object),
                "Argument exception should be thrown");
            moduleInfoMock.Verify();
        }


        [Test]
        public void adding_incomplete_module_no_data_raises_exception()
        {
            var moduleInfoMock = new Mock<IModuleInfo>(MockBehavior.Loose);
            moduleInfoMock
                .Setup(x => x.Manifest)
                .Returns(new ModuleManifest());
            moduleInfoMock
                .Setup(x => x.ModuleData)
                .Returns(() => null)
                .Verifiable("This method should be called");

            Assert.Throws<ArgumentException>(
                () => _repositoryModel.AddModule(moduleInfoMock.Object),
                "Argument exception should be thrown");
            moduleInfoMock.Verify();
        }


        [Test]
        public void adding_null_raises_exception()
        {
            Assert.Throws<ArgumentNullException>(() => _repositoryModel.AddModule(null),
                                                 "Adding null should rise an exception");
        }


        [Test]
        public void adding_module_add_module_to_storage()
        {
            Mock<IModuleInfo> moduleInfoMock = GetModuleInfoMock();

            _storageMock.Setup(x => x.SaveModule(moduleInfoMock.Object))
                .Verifiable("Add should be performed on module info mock");

            _repositoryModel.AddModule(moduleInfoMock.Object);

            _storageMock.Verify();
        }

        #endregion

        #region Removing 

        [Test]
        public void passign_null_to_remove_raises_exception()
        {
            Assert.Throws<ArgumentNullException>(() => _repositoryModel.RemoveModule(null));
        }


        [Test]
        public void removing_non_existing_item_raises_exception()
        {
            _storageMock.Setup(x => x.GetAvaliableModules())
                .Returns(new List<IModuleInfo>())
                .Verifiable("Get modules should be done");

            Mock<IModuleInfo> moduleInfoMock = GetModuleInfoMock();

            Assert.Throws<ArgumentException>(
                () => _repositoryModel.RemoveModule(moduleInfoMock.Object));
            _storageMock.Verify();
        }


        [Test]
        public void removing_existing_item_invokes_removal_on_storage()
        {
            Mock<IModuleInfo> moduleMockInfo = GetModuleInfoMock();
            _storageMock.Setup(x => x.RemoveModule(moduleMockInfo.Object))
                .Verifiable("Remove should be placed upon mocked object");
            _storageMock.Setup(x => x.GetAvaliableModules())
                .Returns(new List<IModuleInfo>
                             {
                                 moduleMockInfo.Object
                             });

            _repositoryModel.RemoveModule(moduleMockInfo.Object);
        }

        #endregion
    }
}