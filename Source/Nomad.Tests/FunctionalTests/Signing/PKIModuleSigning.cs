using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Nomad.ManifestCreator;
using Nomad.Modules.Manifest;
using Nomad.Signing.SignatureAlgorithms;
using Nomad.Utils;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.Signing
{
    [FunctionalTests]
    public class PKIModuleSigning
    {
        private string _assemblyName;
        private string _assemblyPath;
        private string _issuerName;
        private string _manifestPath;
        private string _manifestSignature;
        private string _moduleDirectory;
        private string _privateKeyFileName;


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

            File.WriteAllText(_assemblyPath, "test assembly");

            CreateSignature(_privateKeyFileName, "abc123");
        }


        private void CreateSignature(string key, string password)
        {
            if (File.Exists(_manifestPath))
                File.Delete(_manifestPath);
            if (File.Exists(_manifestSignature))
                File.Delete(_manifestSignature);

            ManifestCreatorProgram.Main(new[]
                                            {
                                                "pki", key, _moduleDirectory,
                                                _assemblyName, _issuerName, password
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
            var pkiSignatureAlgorithm = new PkiSignatureAlgorithm(new X509Certificate2Collection());
                //none additional certificates

            Assert.IsTrue(pkiSignatureAlgorithm.Verify(File.ReadAllBytes(_manifestPath),
                                                       File.ReadAllBytes(_manifestSignature)),
                          "Verification of manifest should succeed, when CA in store");
        }


        [Test]
        public void verification_fails_when_key_not_in_store_or_chain()
        {
            string invalidManifest = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                  @"res\pki\signed_with_untrusted_cert\sample_module.dll.manifest");
            string invalidManifestSignature = invalidManifest + ".asc";
            var collection = new X509Certificate2Collection();
            var pkiSignatureAlgorithm = new PkiSignatureAlgorithm(collection);
                //none additional certificates
            Assert.IsFalse(pkiSignatureAlgorithm.Verify(File.ReadAllBytes(invalidManifest),
                                                        File.ReadAllBytes(invalidManifestSignature)),
                           "Verification of manifest should not succeed, when CA is not in store");
        }


        [Test]
        public void verification_succeds_when_ca_in_chain()
        {
            string invalidManifest = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                  @"res\pki\signed_with_untrusted_cert\sample_module.dll.manifest");
            string cert = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                       @"res\pki\NomadInvalidCa.");
            string invalidManifestSignature = invalidManifest + ".asc";
            var certificate = new X509Certificate2(File.ReadAllBytes(cert + "cer"));
            //var revocationList = new X509Certificate2(File.ReadAllBytes(cert + "crl"));
            var collection = new X509Certificate2Collection
                                 {
                                     certificate
                                 };
            var pkiSignatureAlgorithm = new PkiSignatureAlgorithm(collection);
                //none additional certificates
            Assert.IsFalse(pkiSignatureAlgorithm.Verify(File.ReadAllBytes(invalidManifest),
                                                        File.ReadAllBytes(invalidManifestSignature)),
                           "Verification of manifest should not succeed, when CA is not store");
        }
    }
}