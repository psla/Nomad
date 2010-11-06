using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Nomad.Modules;
using Nomad.Modules.Manifest;
using Nomad.Signing.SignatureAlgorithms;
using Nomad.Utils;
using NUnit.Framework;
using TestsShared;
using System.Linq;
using Version = Nomad.Utils.Version;

namespace Nomad.Tests.FunctionalTests.Signing
{
    [FunctionalTests]
    public class PKIModuleSigning
    {
        private string _moduleDirectory;
        private string _assemblyName;
        private string _issuerName;
        private string _assemblyPath;
        private string _manifestPath;
        private string _privateKeyFileName;
        private string _manifestSignature;


        [TestFixtureSetUp]
        public void setup()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US"); 
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            _privateKeyFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                        @"res\pki\SignedByCA.pfx");
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                         @"res\pki\NomadFakeCa.cer");
            _assemblyName = "sample_module.dll";
            _moduleDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                            @"FunctionalTests\Signing\Module");
            _assemblyPath = Path.Combine(_moduleDirectory, _assemblyName);
            _manifestPath = _assemblyPath + ModuleManifest.ManifestFileNameSuffix;
            _issuerName = "test-issuer";

            _manifestSignature = _manifestPath + ModuleManifest.ManifestSignatureFileNameSuffix;

            if(File.Exists(_manifestPath))
                File.Delete(_manifestPath);
            if(File.Exists(_manifestSignature))
                File.Delete(_manifestSignature);

            File.WriteAllText(_assemblyPath, "test assembly");

            ManifestCreator.ManifestCreatorProgram.Main(new[]
                                                            {
                                                                "pki", _privateKeyFileName, _moduleDirectory,
                                                                _assemblyName, _issuerName, "abc123"
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
            Assert.IsTrue(File.Exists(_manifestSignature), "Manifest signature does not exist: {0}",
                          _manifestSignature);
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
        public void verification_succeeds_when_key_in_store_or_in_chain()
        {
            var pkiSignatureAlgorithm = new PkiSignatureAlgorithm(new X509Certificate2Collection()); //none additional certificates
            
            Assert.IsTrue(pkiSignatureAlgorithm.Verify(File.ReadAllBytes(_manifestPath),
                                         File.ReadAllBytes(_manifestSignature)), "Verification of manifest should succeed, when CA in store");
        }

        [Test]
        public void verification_fails_when_key_not_in_store_or_chain()
        {
            var collection = new X509Certificate2Collection();
            var pkiSignatureAlgorithm = new PkiSignatureAlgorithm(collection); //none additional certificates
            Assert.IsFalse(pkiSignatureAlgorithm.Verify(File.ReadAllBytes(_manifestPath),
                                         File.ReadAllBytes(_manifestSignature)), "Verification of manifest should not succeed, when CA is not store");
        }
    }
}