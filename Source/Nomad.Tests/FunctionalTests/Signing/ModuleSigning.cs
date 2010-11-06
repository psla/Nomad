using System;
using System.IO;
using System.Reflection;
using Nomad.Modules;
using Nomad.Modules.Manifest;
using Nomad.Utils;
using NUnit.Framework;
using TestsShared;
using System.Linq;
using Version = Nomad.Utils.Version;

namespace Nomad.Tests.FunctionalTests.Signing
{
    [FunctionalTests]
    public class ModuleSigning
    {
        private string _keyFileName;
        private string _moduleDirectory;
        private string _assemblyName;
        private string _issuerName;
        private string _assemblyPath;
        private string _manifestPath;


        [TestFixtureSetUp]
        public void setup()
        {
            _keyFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                        @"FunctionalTests\Signing\KeyDir\manifest-key.xml");
            _moduleDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                            @"FunctionalTests\Signing\Module");
            _assemblyName = "sample_module.dll";
            _assemblyPath = Path.Combine(_moduleDirectory, _assemblyName);
            _manifestPath = _assemblyPath + ModuleManifest.ManifestFileNameSuffix;
            _issuerName = "test-issuer";
            if (File.Exists(_keyFileName))
                File.Delete(_keyFileName);
            KeysGenerator.KeysGeneratorProgram.Main(new[] {_keyFileName});

            File.WriteAllText(_assemblyPath, "test assembly");

            ManifestCreator.ManifestCreatorProgram.Main(new[]
                                                            {
                                                                "rsa",
                                                                _keyFileName, _moduleDirectory,
                                                                _assemblyName, _issuerName
                                                            });
        }


        [Test]
        public void manifest_is_created()
        {
            Assert.IsTrue(File.Exists(_manifestPath), "Manifest does not exist: {0}", _manifestPath);
        }


        [Test]
        public void signature_for_manifest_is_created()
        {
            var manifestSignature = _manifestPath + ModuleManifest.ManifestSignatureFileNameSuffix;
            Assert.IsTrue(File.Exists(manifestSignature), "Manifest signature does not exist: {0}",
                          manifestSignature);
        }


        [Test]
        public void is_signature_correct_xml()
        {
            Assert.DoesNotThrow(
                () =>
                XmlSerializerHelper.Deserialize<ModuleManifest>(File.ReadAllBytes(_manifestPath)),
                "Incorrect signature file");
        }


        [Test]
        public void is_signature_module_path_relative()
        {
            var moduleManifest =
                XmlSerializerHelper.Deserialize<ModuleManifest>(File.ReadAllBytes(_manifestPath));
            Assert.AreEqual(_assemblyName, moduleManifest.SignedFiles.First().FilePath);
        }

        [Test]
        public void does_signature_contains_proper_module_name_and_version()
        {
            var moduleManifest =
                XmlSerializerHelper.Deserialize<ModuleManifest>(File.ReadAllBytes(_manifestPath));
            Assert.AreEqual(moduleManifest.ModuleName, "sample_module.dll");
            var expectedVersion = new Version("0.0.0.0");
            Assert.AreEqual(expectedVersion, moduleManifest.ModuleVersion);
        }
    }
}