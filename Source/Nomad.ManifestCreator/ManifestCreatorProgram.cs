using System;
using System.IO;

namespace Nomad.ManifestCreator
{
    /// <summary>
    ///Application responsible for creating manifest for all files in provided directory
    /// </summary>
    /// 
    public class ManifestCreatorProgram
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        /// first argument contains path to issuer
        /// second argument contains path to directory
        /// thir argument contains module assembly name
        /// </param>
        private static void Main(string[] args)
        {
            try
            {
                var argumentsParser = new ArgumentsParser(args);
                var manifestCreator = new ManifestCreator(argumentsParser);
            }
            catch (Exception e)
            {
                if (args.Length != 3)
                {
                    Console.WriteLine(
                        "manifestCreator.exe path_to_issuer_xml path_to_directory assembly_name.dll");
                    Console.WriteLine(e.Message);
                    return;
                }
            }
        }
    }

    internal class ManifestCreator
    {
        private readonly ArgumentsParser _argumentsParser;


        public ManifestCreator(ArgumentsParser argumentsParser)
        {
            _argumentsParser = argumentsParser;
        }
    }

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

            ValidateArguments();
        }


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