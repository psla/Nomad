using System;
using System.IO;

namespace Nomad.ManifestCreator
{
    /// <summary>
    /// Parses arguments provided to <see cref="ManifestCreatorProgram"/>
    /// </summary>
    internal class ArgumentsParser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <exception cref="ArgumentException">when provided argument is incorrect</exception>
        public ArgumentsParser(string[] args)
        {
            IssuerXml = args[0];
            Directory = args[1];
            AssemblyName = args[2];
            IssuerName = args[3];

            ValidateArguments();
        }


        public string IssuerName { get; private set; }


        public string IssuerXml { get; private set; }
        public string Directory { get; private set; }
        public string AssemblyName { get; private set; }


        private void ValidateArguments()
        {
            if (!File.Exists(IssuerXml))
                throw new ArgumentException("Incorrect issuer xml path");
            if (!System.IO.Directory.Exists(Directory))
                throw new ArgumentException("Incorrect directory file path");
            if (!File.Exists(Path.Combine(Directory, AssemblyName)))
                throw new ArgumentException("There is no such assembly name");
        }
    }
}