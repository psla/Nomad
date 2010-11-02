using System;
using System.IO;
using Moq;
using Nomad.KeysGenerator;
using Nomad.ManifestCreator;
using Nomad.Modules;
using Nomad.Modules.Filters;
using Nomad.Modules.Manifest;
using Nomad.Signing;
using Nomad.Signing.SignatureAlgorithms;
using Nomad.Signing.SignatureProviders;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.Modules.Filters
{
    [FunctionalTests]
    public class SignedModulesFiltering
    {
        private string _assemblyName;
        private string _issuerName;
        private string _keyFileName;
        private string _moduleDirectory;
        private string _modulePath;
        private string _publicKeyFileName;
        private Mock<ISignatureProvider> _signatureProvider;
        private SignatureModuleFilter _filter;
        private string _manifestPath;
        private string _manifestSignature;
        private byte[] _moduleContent;


        [TestFixtureSetUp]
        public void generate_keys()
        {
            _keyFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                        @"Modules\signing-key.xml");
            _publicKeyFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                              @"Modules\public-key.xml");
            _issuerName = "test-issuer";
            KeysGeneratorProgram.Main(new[] {_keyFileName, _publicKeyFileName});
        }


        [SetUp]
        public void generate_key_manifest()
        {
            _moduleDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                            @"Modules\signing\");
            _assemblyName = "EmptyModule.dll";
            _modulePath = Path.Combine(_moduleDirectory, _assemblyName);
            Program.Main(new[]
                                            {
                                                "rsa",
                                                _keyFileName
                                                , _moduleDirectory,
                                                _assemblyName, _issuerName
                                            });
            var mockFactory = new MockFactory(MockBehavior.Strict);
            _signatureProvider = mockFactory.Create<ISignatureProvider>();

            _signatureProvider.Setup(x => x.GetIssuer(_issuerName)).Returns(
                new IssuerInformation(_issuerName,
                                      new RsaSignatureAlgorithm(File.ReadAllText(_publicKeyFileName))));

            _filter = new SignatureModuleFilter(_signatureProvider.Object);

            _manifestPath = _modulePath + ModuleManifest.ManifestFileNameSuffix;
            _manifestSignature = _manifestPath + ModuleManifest.ManifestSignatureFileNameSuffix;

            _moduleContent = File.ReadAllBytes(_modulePath);
        }


        [TearDown]
        public void tear_down()
        {
            if (File.Exists(_manifestPath))
                File.Delete(_manifestPath);
            if (File.Exists(_manifestSignature))
                File.Delete(_manifestSignature);
            File.WriteAllBytes(_modulePath, _moduleContent);
        }


        [Test]
        public void filter_accepts_when_structure_and_signatures_are_ok()
        {
            Assert.IsTrue(_filter.Matches(new ModuleInfo(_modulePath)),
                          "Expecting to succeed, when all requirements are ok");
        }


        [Test]
        public void filter_fails_when_asc_file_missing()
        {
            File.Delete(_manifestSignature);
            Assert.IsFalse(_filter.Matches(new ModuleInfo(_modulePath)),
                           "Expecting to fail if no manifest signature file");
        }


        [Test]
        public void filter_fails_when_no_manifest()
        {
            File.Delete(_manifestPath);
            Assert.IsFalse(_filter.Matches(new ModuleInfo(_modulePath)),
                           "Expecting to fail if no manifest signature file");
        }


        [Test]
        public void filter_fails_when_file_content_changed()
        {
            var dll = File.ReadAllBytes(_modulePath);
            dll[0] = (byte) ((dll[0] + 15) % 256);
            File.WriteAllBytes(_modulePath, dll);
            Assert.IsFalse(_filter.Matches(new ModuleInfo(_modulePath)),
                           "Expecting to fail when assembly dll has changed");
        }
    }
}