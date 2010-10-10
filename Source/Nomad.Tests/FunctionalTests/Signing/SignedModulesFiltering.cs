using System;
using System.IO;
using Moq;
using Nomad.KeysGenerator;
using Nomad.ManifestCreator;
using Nomad.Modules;
using Nomad.Signing;
using NUnit.Framework;
using File = System.IO.File;

namespace Nomad.Tests.FunctionalTests.Signing
{
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


        [SetUp]
        public void generate_key_manifest()
        {
            _keyFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                        @"Modules\signing-key.xml");
            _publicKeyFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                              @"Modules\public-key.xml");
            _moduleDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                            @"Modules\signing\");
            _assemblyName = "EmptyModule.dll";
            _issuerName = "test-issuer";
            _modulePath = Path.Combine(_moduleDirectory, _assemblyName);
            KeysGeneratorProgram.Main(new[] {_keyFileName, _publicKeyFileName});
            ManifestCreatorProgram.Main(new[]
                                            {
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
        }


        [TearDown]
        public void tear_down()
        {
            string manifestPath = _modulePath + ".manifest";
            string manifestSignature = manifestPath + ".asc";
            if (File.Exists(manifestPath))
                File.Delete(manifestPath);
            if (File.Exists(manifestSignature))
                File.Delete(manifestSignature);
        }


        [Test]
        public void filter_accepts_when_structure_and_signatures_are_ok()
        {
            Assert.IsTrue(_filter.Matches(new ModuleInfo(_modulePath)));
        }
    }
}