using Nomad.Signing;
using Nomad.Signing.SignatureAlgorithms;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.UnitTests.Signing
{
    [TestFixture]
    [UnitTests]
    public class SignatureTests
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _signer = new RsaSignatureAlgorithm(_keys);
            _verifier = new RsaSignatureAlgorithm(_publicKey);
        }

        #endregion

        private string _keys =
            "<RSAKeyValue><Modulus>wuOa3hTuiqDqpW0aFfYzDVknSlNxINjKHreuGf4QloGMoPi5rMeo7SZg6FLj98+dVsjPl3HHRYEjSRDXg2b9qfhOFh/ixVuFc27GB1bkgIgx8eGh3a+yQU6QYGLYtNjv4n5pSTbPvAI1Od1oTzX6JCwrRfKQjQB8bHonf8A71FE=</Modulus><Exponent>AQAB</Exponent><P>5UupQU4f4n3mvnOn/SEMWZ3U9Y0PkirRRpCGzWhgRZiXC60PX3E9KkjPPT+IW513OLUKw/lTOtKCu7wlsslY5w==</P><Q>2ZYhkOqIKHiTho4owk/ChttaqSB7KgP+5Q2ip/XqqFPNTR1BCiG4ga6R1LxNqaaWWe4CevKgLBCFYkY/b6eqBw==</Q><DP>gvXm0rTDQ0IzYv19qfaRLjIWKrUJeqtX4dy0EWeW7MkNJA8Q46syiw8QxmOeBu115X6JiorBfaw0HPOF+IpFaw==</DP><DQ>NNazggiiVgmp8bQWC9HosG0WYBnmLvbXcHJWPKmIeaYYJhDjmX1E3CEzgtDTn044FtCow4imAu1kJtBeWVQDAw==</DQ><InverseQ>GhwtJBaMc0lpRv2ChZ6jzaUKsMGfCUfdEyuFK/OsYjuDLKNbQOhlTEsYzpHS6mWrUZJf96J0mfCR8LVKKqjJIA==</InverseQ><D>MnMC0vxuJ5qyQzJz7z11R75NanMhNNSJtdvsiLP5DUzp332javOgagtmSJ20f6DR5PZErrp4UCi2vqJKd6yAezyqKPAo2j/RUvSZ41vfShzVugvk/VgmLXJTax6ccrCwjcYlWdifI+SNTAKUyOZwLHeACNWQBWb6iz1DKZmxN/U=</D></RSAKeyValue>";

        private string _publicKey =
            "<RSAKeyValue><Modulus>wuOa3hTuiqDqpW0aFfYzDVknSlNxINjKHreuGf4QloGMoPi5rMeo7SZg6FLj98+dVsjPl3HHRYEjSRDXg2b9qfhOFh/ixVuFc27GB1bkgIgx8eGh3a+yQU6QYGLYtNjv4n5pSTbPvAI1Od1oTzX6JCwrRfKQjQB8bHonf8A71FE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        private readonly byte[] _data = new byte[] {23, 123, 34, 35, 75, 28, 246, 92, 84};
        private ISignatureAlgorithm _signer;
        private ISignatureAlgorithm _verifier;


        [Test]
        public void can_decrypt_using_verifier()
        {
            byte[] signature = _signer.Sign(_data);
            Assert.AreEqual(true, _verifier.Verify(_data, signature));
        }


        [Test]
        public void can_encrypt_and_decrypt()
        {
            byte[] signature = _signer.Sign(_data);
            Assert.AreNotEqual(_data, signature);
            bool decrypted = _signer.Verify(_data, signature);
            Assert.AreEqual(true, decrypted);
        }
    }
}