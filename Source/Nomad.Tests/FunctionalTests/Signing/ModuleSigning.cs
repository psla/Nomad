using System;
using System.IO;
using NUnit.Framework;
using TestsShared;

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


        [TestFixtureSetUp]
        public void setup()
        {
            _keyFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                               @"FunctionalTests\Signing\KeyDir\manifest-key.xml");
            _moduleDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                               @"FunctionalTests\Signing\Module");
            _assemblyName = "sample_module.dll";
            _assemblyPath = Path.Combine(_moduleDirectory, _assemblyName);
            _issuerName = "test-issuer";
            if (File.Exists(_keyFileName))
                File.Delete(_keyFileName);
            KeysGenerator.KeysGeneratorProgram.Main(new[] {_keyFileName});

            File.WriteAllText(_assemblyPath, "test assembly");

            ManifestCreator.ManifestCreatorProgram.Main(new[]
                                                            {
                                                                _keyFileName, _moduleDirectory,
                                                                _assemblyName, _issuerName
                                                            });
        }

        [Test]
        public void manifest_is_created()
        {
            var manifestPath = _assemblyPath + ".manifest";
            Assert.IsTrue(File.Exists(manifestPath), "Manifest does not exist: {0}", manifestPath);
        }

        [Test]
        public void signature_for_manifest_is_created()
        {
            var manifestPath = _assemblyPath + ".manifest";
            var manifestSignature = manifestPath + ".asc";
            Assert.IsTrue(File.Exists(manifestSignature), "Manifest signature does not exist: {0}", manifestSignature);
        }
    }
}