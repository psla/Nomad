using System;
using System.IO;
using System.Linq;
using Nomad.Modules.Manifest;
using Nomad.Updater;
using Nomad.Updater.ModuleFinders;
using Nomad.Utils;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.IntegrationTests.Updater
{
    [IntegrationTests]
    public class ModuleFinderTests
    {
        private ModuleFinder _moduleFinder;

        private readonly string _testDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                    @"ModuleFinderTests");
        [TestFixtureSetUp]
        public void set_up_fixture()
        {
            if(Directory.Exists(_testDirectory))
                Directory.Delete(_testDirectory,true);

            Directory.CreateDirectory(_testDirectory);
        }

        [TestFixtureTearDown]
        public void tear_down_fixture()
        {
            if (Directory.Exists(_testDirectory))
                Directory.Delete(_testDirectory, true);
        }

        [SetUp]
        public void set_up()
        {
            _moduleFinder = new ModuleFinder();
        }

        [TearDown]
        public void tear_down()
        {
            foreach( var file in Directory.GetFiles(_testDirectory))
            {
                File.Delete(file);
            }
        }


        [Test]
        public void passing_null_to_finder_raises_exception()
        {
            Assert.Throws<ArgumentException>(
                () => _moduleFinder.FindDirectoryForPackage("Some text", null));
            Assert.Throws<ArgumentException>(
                () => _moduleFinder.FindDirectoryForPackage(string.Empty, new ModulePackage()));
            Assert.Throws<ArgumentException>(
                () => _moduleFinder.FindDirectoryForPackage(null, new ModulePackage()));
        }


        [Test]
        public void finds_manifest_in_multi_level_tree()
        {
            // module name 
            const string moduleName = "moduleName";
            

            // create sub folder
            const string folderName = "folderName";
            string folderPathCreated = Path.Combine(_testDirectory, folderName);
            Directory.CreateDirectory(folderPathCreated);

            // create name for manifest
            string pathToManifest = Path.Combine(folderPathCreated, "SOME_ITCHY_NAME" + ModuleManifest.ManifestFileNameSuffix);

            // put manifest there
            var manifest = new ModuleManifest()
                                 {
                                     ModuleName = moduleName
                                 };
            var data = XmlSerializerHelper.Serialize(manifest);
            File.WriteAllBytes(pathToManifest, data);

            // create package with manifest of the same name
            var package = GetPackageWithName(moduleName);

            // run test
            var folderPathResolved = _moduleFinder.FindDirectoryForPackage(_testDirectory, package);

            // assert that path to sub folder has been returned
            Assert.AreEqual(folderPathResolved, folderPathCreated, "The path to folder was not found properly");
        }


        [Test]
        public void creates_directory_when_no_manifest_provided()
        {
            // prepare some package
            const string moduleString = "module1";
            ModulePackage modulePackage = GetPackageWithName(moduleString);

            var name = _moduleFinder.FindDirectoryForPackage(_testDirectory, modulePackage);

            Assert.IsTrue(Directory.Exists(name),"The directory does not exisits");
        }


        private static ModulePackage GetPackageWithName(string moduleString)
        {
            var package = new ModulePackage
                              {
                                  ModuleZip = new byte[] {0xFf},
                                  ModuleManifest = new ModuleManifest
                                                       {
                                                           ModuleName = moduleString,
                                                       }
                              };

            return package;
        }
    }
}