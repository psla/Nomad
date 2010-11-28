using System;
using System.Collections.Generic;
using System.IO;
using Nomad.Modules.Manifest;
using Nomad.Updater;
using Nomad.Updater.ModulePackagers;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.IntegrationTests.Updater
{
    [IntegrationTests]
    public class ZipPackager
    {
        [Test]
        public void zip_packager_unzips_the_file_properly()
        {
            
            string zipFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"res\test.zip");

            var modulePackage = new ModulePackage
                {
                    ModuleManifest =
                        new ModuleManifest(),
                    ModuleZip =
                        File.ReadAllBytes(zipFile)
                };
                                                      

            // get the zip packager
            IModulePackager packager = new ModulePackager();

            // set up directory
            string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,@"Updater\Zip\");
            if(Directory.Exists(targetDirectory))
                Directory.Delete(targetDirectory,true);

            Directory.CreateDirectory(targetDirectory);

            // perform test
            packager.PerformUpdates(targetDirectory,modulePackage);

            // assert test
            Assert.IsTrue(File.Exists(Path.Combine(targetDirectory, "readme")));
        }
    }
}