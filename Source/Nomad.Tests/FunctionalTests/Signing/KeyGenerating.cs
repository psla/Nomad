using System;
using System.IO;
using Nomad.KeysGenerator;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.Signing
{
    [FunctionalTests]
    public class KeyGenerating
    {
        private string _keyFileName;


        [SetUp]
        public void setup()
        {
            _keyFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                           @"FunctionalTests\Signing\KeyDir\key.xml");
            if(File.Exists(_keyFileName))
                File.Delete(_keyFileName);
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
    }
}