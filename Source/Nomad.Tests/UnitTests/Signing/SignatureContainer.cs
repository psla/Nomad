using Moq;
using Nomad.Signing;
using Nomad.Signing.SignatureAlgorithms;
using Nomad.Signing.SignatureProviders;
using NUnit.Framework;

namespace Nomad.Tests.UnitTests.Signing
{
    public class SignatureContainer
    {
        private ISignatureProvider _signatureProvider;


        [SetUp]
        public void setup()
        {
            _signatureProvider = new SignatureProvider();
        }


        [Test]
        public void returns_correct_issuer_when_one_registered()
        {
            string issuerName = "testIssuer";
            var issuer = new IssuerInformation(issuerName, new NullSignatureAlgorithm());
            _signatureProvider.Register(issuer);
            Assert.AreSame(issuer, _signatureProvider.GetIssuer(issuerName),
                           "Incorrect issuer returned");
        }


        [Test]
        public void returns_correct_issuer_when_two_registered()
        {
            string issuerName = "testIssuer";
            string issuerName2 = "testIssuer2";
            var issuer = new IssuerInformation(issuerName, new NullSignatureAlgorithm());
            var issuer2 = new IssuerInformation(issuerName2, new NullSignatureAlgorithm());
            _signatureProvider.Register(issuer);
            _signatureProvider.Register(issuer2);
            Assert.AreSame(issuer, _signatureProvider.GetIssuer(issuerName),
                           "Incorrect issuer returned");
            Assert.AreSame(issuer2, _signatureProvider.GetIssuer(issuerName2),
                           "Incorrect issuer returned");
        }


        [Test]
        public void returns_issuer_with_null_signature_algorithm_if_issuer_not_found()
        {
            var issuerName = "notExistingIssuer";
            var issuer = _signatureProvider.GetIssuer(issuerName);
            Assert.AreEqual(issuerName, issuer.IssuerName, "Incorrect issuer name in returned class");
            Assert.IsInstanceOf(typeof (NullSignatureAlgorithm), issuer.IssuerAlgorithm,
                                "Incorrect issuer algorithm");
        }

        [Test]
        public void returns_default_provider_when_issuer_provider_not_specified()
        {
            var defaultAlgorithm = new Mock<ISignatureAlgorithm>();
            var signatureProvider = new SignatureProvider(defaultAlgorithm.Object);
            
            var provider = _signatureProvider.GetIssuer("someone");

            Assert.AreSame(defaultAlgorithm.Object, provider, "When no vendor algorithm provided, default should be returned");
        }
    }
}