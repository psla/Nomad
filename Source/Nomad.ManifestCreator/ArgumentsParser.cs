using System;
using System.IO;

namespace Nomad.ManifestCreator
{
    /// <summary>
    ///     Parses arguments provided to <see cref="ManifestCreatorProgram"/> and initializes the instance of <see cref="Nomad.ManifestCreator"/> class.
    /// </summary>
    public class ArgumentsParser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <exception cref="ArgumentException">when provided argument is incorrect</exception>
        public ArgumentsParser(string[] args)
        {
            KeyStore = args[0];
            IssuerXml = args[1];
            Directory = args[2];
            AssemblyName = args[3];
            IssuerName = args[4];
            
            KeyPassword = null;
            if(args.Length == 6)
                KeyPassword = args[5];

            FormatDirectory();
            ValidateArguments();
        }



        /// <summary>
        /// Contains Algorithm Type
        /// </summary>
        /// <remarks>
        /// May be: RSA or PKI
        /// </remarks>
        public string KeyStore { get; private set; }


        private void FormatDirectory()
        {
            if (!Directory.EndsWith(@"\"))
                Directory += @"\";
        }


        public string IssuerName { get; private set; }


        public string IssuerXml { get; private set; }
        public string Directory { get; private set; }
        public string AssemblyName { get; private set; }

        public string KeyPassword { get; private set; }


        private void FormatDirectory()
        {
            if (!Directory.EndsWith(@"\"))
                Directory += @"\";
        }


        private void ValidateArguments()
        {
            if (!File.Exists(IssuerXml))
                throw new ArgumentException("Incorrect issuer xml path");
            if (!System.IO.Directory.Exists(Directory))
                throw new ArgumentException("Incorrect directory file path");
            if (!File.Exists(Path.Combine(Directory, AssemblyName)))
                throw new ArgumentException("There is no such assembly name");
        }


        public Utils.ManifestCreator GetManifestCreator()
        {
            return new Utils.ManifestCreator(IssuerName, IssuerXml, AssemblyName, Directory);
        }
    }
}