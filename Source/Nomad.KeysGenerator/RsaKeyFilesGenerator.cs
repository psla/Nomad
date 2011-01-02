using System.IO;
using System.Security.Cryptography;

namespace Nomad.KeysGenerator
{
    public class RsaKeyFilesGenerator
    {
        /// <summary>
        /// Location of public file
        /// </summary>
        private readonly string _publicFile;

        /// <summary>
        /// Location of target private & public file
        /// </summary>
        private readonly string _targetFile;


        /// <summary>
        /// Initializes instance of<see cref="RsaKeyFilesGenerator"/> class.
        /// </summary>
        /// <param name="publicFile">File containing the assembly to be signed.</param>
        /// <param name="targetFile">File containing the secure signature.</param>
        /// <param name="keySize">length of the key in bits</param>
        public RsaKeyFilesGenerator(string publicFile, string targetFile, int keySize)
        {
            _publicFile = publicFile;
            _targetFile = targetFile;
        }


        /// <summary>
        /// Generates secure Signature.
        /// </summary>
        public void GenerateSignature()
        {
            // Generate a signing key.
            var rsaCryptoServiceProvider = new RSACryptoServiceProvider();
            File.WriteAllText(_targetFile, rsaCryptoServiceProvider.ToXmlString(true));
            if (_publicFile != null)
                File.WriteAllText(_publicFile, rsaCryptoServiceProvider.ToXmlString(false));
        }
    }
}