using System;
using System.Collections;
using System.Reflection;
using Nomad.Signing;
using NUnit.Framework;
using TestsShared;
using System.Linq;

namespace Nomad.Tests.UnitTests.Signing
{
    [TestFixture]
    [UnitTests]
    public class AssemblyVerification
    {
        /// <summary>
        /// Verifies if first five bytes of array are equal
        /// </summary>
        private class NullSignatureAlgorithm : ISignatureAlgorithm
        {
            #region Implementation of ISignatureAlgorithm

            public byte[] Sign(byte[] data)
            {
                return data.Take(5).ToArray();
            }


            public bool Verify(byte[] data, byte[] signature)
            {
                return data.Take(5).ToArray().SequenceEqual(signature);
            }

            #endregion
        }

        private FileSignatureVerificator _fileSignatureVerificator;


        [SetUp]
        public void SetUp()
        {
            _fileSignatureVerificator = new FileSignatureVerificator();
            var issuerInformation = new IssuerInformation("Nomad",
                                                          new NullSignatureAlgorithm());
            _fileSignatureVerificator.AddTrustedIssuer(issuerInformation);
        }


        [Test]
        public void verification_against_trusted_issuer()
        {
            var signature = new FileSignature()
                                {
                                    FileName = "Nomad.dll",
                                    IssuerName = "Nomad",
                                    Signature = new byte[] {65, 64, 63, 62, 61}
                                };
            var file = new File()
                           {FileName = "Nomad.dll", Data = new byte[] {65, 64, 63, 62, 61, 60}};
            Assert.IsTrue(_fileSignatureVerificator.VerifyFile(file, signature));
        }


        [Test]
        public void verification_against_untrusted_issuer()
        {
            var signature = new FileSignature()
                                {
                                    FileName = "Nomad.dll",
                                    IssuerName = "Nomad2",
                                    Signature = new byte[] {65, 64, 63, 62, 61}
                                };
            var file = new File()
                           {FileName = "Nomad.dll", Data = new byte[] {65, 64, 63, 62, 61, 60}};

            Assert.IsFalse(_fileSignatureVerificator.VerifyFile(file, signature));
        }


        [Test]
        public void verification_against_trusted_issuer_with_incorrect_file()
        {
            var signature = new FileSignature()
                                {
                                    FileName = "Nomad.dll",
                                    IssuerName = "Nomad",
                                    Signature = new byte[] {65, 64, 63, 62, 61}
                                };
            var file = new File()
                           {FileName = "Nomad.dll", Data = new byte[] {65, 64, 63, 62, 60, 60}};
            Assert.IsFalse(_fileSignatureVerificator.VerifyFile(file, signature));
        }


        [Test]
        public void verification_against_trusted_issuer_with_incorrect_filename()
        {
            var signature = new FileSignature()
                                {
                                    FileName = "Nomad2.dll",
                                    IssuerName = "Nomad",
                                    Signature = new byte[] {65, 64, 63, 62, 61}
                                };
            var file = new File()
                           {FileName = "Nomad.dll", Data = new byte[] {65, 64, 63, 62, 61, 60}};
            Assert.IsFalse(_fileSignatureVerificator.VerifyFile(file, signature));
        }
    }
}