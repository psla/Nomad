using System;
using System.IO;

namespace Nomad.KeysGenerator
{
    internal class ArgumentsParser
    {
        public ArgumentsParser(string[] args)
        {
            KeySize = 1024;

            TargetFile = args[0];
            if (args.Length > 1)
                PublicFile = args[1];
            if (args.Length > 2)
                KeySize = int.Parse(args[2]);
            VerifyValues();
        }


        /// <summary>
        /// Length of the generated key in bits. Default 1024
        /// </summary>
        public int KeySize { get; private set; }


        /// <summary>
        /// Location of public file
        /// </summary>
        public string PublicFile { get; set; }

        /// <summary>
        /// Location of target private & public file
        /// </summary>
        public string TargetFile { get; private set; }


        private void VerifyValues()
        {
            if (File.Exists(TargetFile))
                throw new ArgumentException("Specified file already exists");
            if (PublicFile != null && File.Exists(PublicFile))
                throw new ArgumentException("Specified file already exists");
        }
    }
}