using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Nomad.Modules;
using Nomad.Modules.Manifest;
using Nomad.Signing;
using File = System.IO.File;

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
        /// third argument contains module assembly name
        /// fourth - issuer name
        /// </param>
        public static void Main(string[] args)
        {
            try
            {
                var argumentsParser = new ArgumentsParser(args);
                var manifestCreator = new ManifestCreator(argumentsParser);
                manifestCreator.Create();
            }
            catch (Exception e)
            {
                if (args.Length != 3)
                {
                    Console.WriteLine(
                        "manifestCreator.exe path_to_issuer_xml path_to_directory assembly_name.dll issuer_name");
                    Console.WriteLine(e.Message);
                    return;
                }
            }
        }
    }

    internal class ManifestCreator
    {
        private readonly ArgumentsParser _argumentsParser;
        private RSACryptoServiceProvider _key;
        private ISignatureAlgorithm _signatureAlgorithm;


        public ManifestCreator(ArgumentsParser argumentsParser)
        {
            _argumentsParser = argumentsParser;
            LoadKey();
        }


        private void LoadKey()
        {
            _signatureAlgorithm =
                new RsaSignatureAlgorithm(File.ReadAllText(_argumentsParser.IssuerXml));
        }


        private string GetAssemblyPath()
        {
            return Path.Combine(_argumentsParser.Directory, _argumentsParser.AssemblyName);
        }


        public void Create()
        {
            string[] files = Directory.GetFiles(_argumentsParser.Directory, "*",
                                                SearchOption.AllDirectories);

            IEnumerable<SignedFile> signedFiles = from file in files
                                                  select
                                                      new SignedFile
                                                          {
                                                              FilePath = file.Substring(_argumentsParser.Directory.Length),
                                                              Signature =
                                                                  _signatureAlgorithm.Sign(
                                                                      File.ReadAllBytes(file))
                                                          };
            var manifest = new ModuleManifest
                               {
                                   Issuer = _argumentsParser.IssuerName,
                                   ModuleName = _argumentsParser.AssemblyName,
                                   SignedFiles = signedFiles.ToList()
                               };
            byte[] manifestSerialized = XmlSerializerHelper.Serialize(manifest);

            string manifestPath = string.Format("{0}.manifest", GetAssemblyPath());

            File.WriteAllBytes(manifestPath, manifestSerialized);
            File.WriteAllBytes(manifestPath + ".asc",
                               _signatureAlgorithm.Sign(File.ReadAllBytes(manifestPath)));
        }
    }
}