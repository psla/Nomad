using System;
using System.IO;
using Ionic.Zip;
using Nomad.KeysGenerator;
using Nomad.Modules.Manifest;
using Nomad.RepositoryServer.Models;
using Nomad.Utils.ManifestCreator;
using NUnit.Framework;
using TestsShared;

namespace Nomad.RepositoryServer.Tests.ModelTests
{
    /// <summary>
    ///     Zip packager test. 
    /// </summary>
    /// <remarks>
    ///     For now only the unpackaging test are working.
    /// </remarks>
    [IntegrationTests]
    public class ZipPackagerTests
    {
        private const string IssuerXmlPath = "KEY.xml";
        private const string IssuerName = "KEY_issuer";
        private static ModuleManifest _manifest;
        private ZipPackager _zipPackager;


        [SetUp]
        public void set_up()
        {
            _zipPackager = new ZipPackager();
            KeysGeneratorProgram.Main(new[]
                                          {
                                              IssuerXmlPath,
                                          });
        }


        private static byte[] GetZipFilePath(params string[] filesToZip)
        {
            string zipFilePath = Path.GetTempFileName();
            File.Delete(zipFilePath);
            using (var zipFile = new ZipFile(zipFilePath))
            {
                string fileName = Path.GetTempFileName();
                File.WriteAllText(fileName, "TEST_TEXT_README_LIKE");

                zipFile.AddFile(fileName, ".");

                foreach (string file in filesToZip)
                {
                    zipFile.AddFile(file, ".");
                }

                zipFile.Save();
            }
            return File.ReadAllBytes(zipFilePath);
        }


        private static string AddKeyFile()
        {
            return IssuerXmlPath;
        }


        private static string AddModuleFile()
        {
            // use SimplestModulePossible file from psake build (it's valid assembly)
            return @"Modules\Simple\SimplestModulePossible1.dll";
        }


        private static string AddManifestFile()
        {
            // generate manifest for SimplestModulePossible file
            var builder = new ManifestBuilder(IssuerName, IssuerXmlPath,
                                              @"SimplestModulePossible1.dll",
                                              @"Modules\Simple");
            _manifest = builder.Create();
            return @"Modules\Simple\SimplestModulePossible1.dll" +
                   ModuleManifest.ManifestFileNameSuffix;
        }

        #region Unpacking

        [Test]
        public void unpacking_zip_with_null_results_in_argument_null_exception()
        {
            Assert.Throws<ArgumentNullException>(() => _zipPackager.UnPack(null));
        }


        [Test]
        public void unpacking_corrupted_file_results_in_exception()
        {
            // corrupted data
            var data = new byte[] {0xFF, 0xFF, 0xFF};

            IModuleInfo moduleInfo;
            Assert.Throws<InvalidDataException>(() => moduleInfo = _zipPackager.UnPack(data));
        }


        [Test]
        public void proper_zip_goes_through_test()
        {
            // arragne
            byte[] data = GetZipFilePath(AddKeyFile(), AddManifestFile(), AddModuleFile());

            // act
            IModuleInfo moduleInfo = null;
            Assert.DoesNotThrow(() => moduleInfo = _zipPackager.UnPack(data),
                                "Unpacking data should not throw any exceptions");

            // assert
            Assert.NotNull(moduleInfo, "Returned moduleInfoImplementation shoud be used");
            Assert.AreEqual(data, moduleInfo.ModuleData, "Zip File data shoudl identical");
            Assert.AreEqual(_manifest.ModuleName, moduleInfo.Manifest.ModuleName,
                            "The manifest should be the with the same name");
            Assert.AreEqual(_manifest.ModuleVersion, moduleInfo.Manifest.ModuleVersion,
                            "The manifest should be same up tp version");
        }


        [Test]
        public void package_is_missing_assembly()
        {
            byte[] data = GetZipFilePath(AddKeyFile(), AddManifestFile());

            // assert
            Assert.Throws<InvalidDataException>(() => _zipPackager.UnPack(data));
        }


        [Test]
        public void packge_is_missing_manifest()
        {
            byte[] data = GetZipFilePath(AddKeyFile(), AddModuleFile());

            // assert
            Assert.Throws<InvalidDataException>(() => _zipPackager.UnPack(data));
        }

        #endregion
    }
}