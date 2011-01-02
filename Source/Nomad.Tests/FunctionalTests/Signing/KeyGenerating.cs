using System;
using System.IO;
using System.Security.Cryptography;
using Nomad.KeysGenerator;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.Signing
{
    [FunctionalTests]
    public class KeyGenerating
    {
        private string _keyFileName;
        private string _publicKeyFile;


        [SetUp]
        public void setup()
        {
            _keyFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                        @"FunctionalTests\Signing\KeyDir\key.xml");
            _publicKeyFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                          @"FunctionalTests\Signing\KeyDir\public_key.xml");
            if (File.Exists(_keyFileName))
                File.Delete(_keyFileName);
            if (File.Exists(_publicKeyFile))
                File.Delete(_publicKeyFile);
        }


        [Test]
        public void generates_non_empty_key_file()
        {
            KeysGeneratorProgram.Main(new[]
                                          {
                                              _keyFileName
                                          }
                );
            File.Exists(_keyFileName);
        }


        [Test]
        public void generates_public_key_file_if_asked()
        {
            KeysGeneratorProgram.Main(new[]
                                          {
                                              _keyFileName,
                                              _publicKeyFile
                                          }
                );
            File.Exists(_keyFileName);
            File.Exists(_publicKeyFile);
        }

        [Test]
        public void generates_keys_of_specified_length()
        {
            KeysGeneratorProgram.Main(new[] { _keyFileName, _publicKeyFile, "2048"});
            var cryptoProvider = new RSACryptoServiceProvider();
            cryptoProvider.FromXmlString(File.ReadAllText(_keyFileName));
        }
    }
}