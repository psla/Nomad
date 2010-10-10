using System;
using System.IO;

namespace Nomad.KeysGenerator
{
    internal class ArgumentsParser
    {
        public ArgumentsParser(string[] args)
        {
            TargetFile = args[0];
            VerifyValues();
        }


        private void VerifyValues()
        {
            if(File.Exists(TargetFile))
                throw new ArgumentException("Specified file already exists");
        }


        public string TargetFile { get; private set; }
    }
}