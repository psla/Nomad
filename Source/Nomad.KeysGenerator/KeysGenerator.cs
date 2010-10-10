using System.IO;
using System.Security.Cryptography;

namespace Nomad.KeysGenerator
{
    internal class KeysGenerator
    {
        private readonly ArgumentsParser _arguments;


        public KeysGenerator(ArgumentsParser arguments)
        {
            _arguments = arguments;
        }


        public void GenerateSignature()
        {
            // Generate a signing key.
            var key = new RSACryptoServiceProvider();
            File.WriteAllText(_arguments.TargetFile, key.ToXmlString(true));
            if (_arguments.PublicFile != null)
                File.WriteAllText(_arguments.PublicFile, key.ToXmlString(false));
        }
    }
}