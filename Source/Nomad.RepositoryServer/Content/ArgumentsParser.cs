using System;
using System.IO;

namespace Nomad.KeysGenerator
{
    internal class ArgumentsParser
    {
        public ArgumentsParser(string[] args)
        {
            TargetFile = args[0];
            if (args.Length > 1)
                PublicFile = args[1];
            VerifyValues();
        }


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