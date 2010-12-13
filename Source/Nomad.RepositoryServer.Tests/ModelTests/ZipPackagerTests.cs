using System;
using System.IO;
using Ionic.Zip;
using Nomad.RepositoryServer.Models;
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
        private ZipPackager _zipPackager;


        [SetUp]
        public void set_up()
        {
            _zipPackager = new ZipPackager();
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
            Assert.Throws<ArgumentException>(() => moduleInfo = _zipPackager.UnPack(data));
        }


        [Test]
        public void proper_zip_goes_through_test()
        {
            var zipFilePath = Path.GetTempFileName();
            File.Delete(zipFilePath);
            using (var zipFile = new ZipFile(zipFilePath))
            {
                string fileName = Path.GetTempFileName();
                File.WriteAllText(fileName,"TEST_TEXT");
                zipFile.AddFile(fileName, ".");

                zipFile.Save();
            }

            // act
            var data = File.ReadAllBytes(zipFilePath);

            // assert
            IModuleInfo moduleInfo = null;
            Assert.DoesNotThrow( () => moduleInfo = _zipPackager.UnPack(data),"Unpacking data should not throw any exceptions");
            Assert.NotNull(moduleInfo,"Returned moduleInfoImplementation shoud be used");
            
        }

        public void package_is_missing_assembly()
        {
        }


        public void packge_is_missing_manifest()
        {
        }


        public void package_is_missing_key_file()
        {
        }

        #endregion
    }
}