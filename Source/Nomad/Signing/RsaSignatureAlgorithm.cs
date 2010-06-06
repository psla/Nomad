using System.Security.Cryptography;

namespace Nomad.Signing
{
    /// <summary>
    /// Signature algorithm implementation with RSA encryption
    /// </summary>
    public class RsaSignatureAlgorithm : ISignatureAlgorithm
    {
        private readonly RSACryptoServiceProvider _crypto;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keys">XML with keys (public for verification, public and private for signing) 
        /// <example>This example shows initialization of <see cref="RsaSignatureAlgorithm"/>
        /// <code>
        /// private string keys = "<RSAKeyValue><Modulus>wuOa3hTuiqDqpW0aFfYzDVknSlNxINjKHreuGf4QloGMoPi5rMeo7SZg6FLj98+dVsjPl3HHRYEjSRDXg2b9qfhOFh/ixVuFc27GB1bkgIgx8eGh3a+yQU6QYGLYtNjv4n5pSTbPvAI1Od1oTzX6JCwrRfKQjQB8bHonf8A71FE=</Modulus><Exponent>AQAB</Exponent><P>5UupQU4f4n3mvnOn/SEMWZ3U9Y0PkirRRpCGzWhgRZiXC60PX3E9KkjPPT+IW513OLUKw/lTOtKCu7wlsslY5w==</P><Q>2ZYhkOqIKHiTho4owk/ChttaqSB7KgP+5Q2ip/XqqFPNTR1BCiG4ga6R1LxNqaaWWe4CevKgLBCFYkY/b6eqBw==</Q><DP>gvXm0rTDQ0IzYv19qfaRLjIWKrUJeqtX4dy0EWeW7MkNJA8Q46syiw8QxmOeBu115X6JiorBfaw0HPOF+IpFaw==</DP><DQ>NNazggiiVgmp8bQWC9HosG0WYBnmLvbXcHJWPKmIeaYYJhDjmX1E3CEzgtDTn044FtCow4imAu1kJtBeWVQDAw==</DQ><InverseQ>GhwtJBaMc0lpRv2ChZ6jzaUKsMGfCUfdEyuFK/OsYjuDLKNbQOhlTEsYzpHS6mWrUZJf96J0mfCR8LVKKqjJIA==</InverseQ><D>MnMC0vxuJ5qyQzJz7z11R75NanMhNNSJtdvsiLP5DUzp332javOgagtmSJ20f6DR5PZErrp4UCi2vqJKd6yAezyqKPAo2j/RUvSZ41vfShzVugvk/VgmLXJTax6ccrCwjcYlWdifI+SNTAKUyOZwLHeACNWQBWb6iz1DKZmxN/U=</D></RSAKeyValue>";
        /// private string publicKey = "<RSAKeyValue><Modulus>wuOa3hTuiqDqpW0aFfYzDVknSlNxINjKHreuGf4QloGMoPi5rMeo7SZg6FLj98+dVsjPl3HHRYEjSRDXg2b9qfhOFh/ixVuFc27GB1bkgIgx8eGh3a+yQU6QYGLYtNjv4n5pSTbPvAI1Od1oTzX6JCwrRfKQjQB8bHonf8A71FE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        /// ISignatureAlgorithm algorithm = new RsaSignatureAlgorithm(keys); //initializes for signing and verifying
        /// ISignatureAlgorithm verificationAlgorithm = new RsaSignatureAlgorithm(publicKey); //initializes for verifying only
        /// </code>
        /// </example></param>
        public RsaSignatureAlgorithm(string keys)
        {
            _crypto = new RSACryptoServiceProvider();
            _crypto.FromXmlString(keys);
        }

        #region Implementation of ISignatureAlgorithm

        public byte[] Sign(byte[] data)
        {
            return _crypto.SignData(data, new SHA1CryptoServiceProvider());
        }


        public bool Verify(byte[] data, byte[] signature)
        {
            return _crypto.VerifyData(data, new SHA1CryptoServiceProvider(), signature);
        }

        #endregion
    }
}