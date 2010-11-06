using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Nomad.Modules.Manifest;
using Nomad.Signing.FileUtils;
using Nomad.Signing.SignatureAlgorithms;
using Nomad.Utils;
using File = System.IO.File;
using Version = Nomad.Utils.Version;

namespace Nomad.ManifestCreator
{
    public class ManifestCreator
    {
        private readonly ArgumentsParser _argumentsParser;
        private ISignatureAlgorithm _signatureAlgorithm;


        public ManifestCreator(ArgumentsParser argumentsParser)
        {
            _argumentsParser = argumentsParser;
            LoadKey();
        }


        private void LoadKey()
        {
            if (_argumentsParser.KeyStore.ToLower() == "rsa")
                _signatureAlgorithm =
                    new RsaSignatureAlgorithm(File.ReadAllText(_argumentsParser.IssuerXml));
            else
                _signatureAlgorithm =
                    new PkiSignatureAlgorithm(File.ReadAllBytes(_argumentsParser.IssuerXml), _argumentsParser.KeyPassword);
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
                                                              FilePath =
                                                                  file.Substring(
                                                                      _argumentsParser.Directory.
                                                                          Length),
                                                              Signature =
                                                                  _signatureAlgorithm.Sign(
                                                                      File.ReadAllBytes(file))
                                                          };
            Version version = null;
            try
            {
                version = new Version(AssemblyName.GetAssemblyName(GetAssemblyPath()).Version);
            }
            catch(Exception e)
            {
                Console.WriteLine("Cannot infer assembly version from assembly. {0}", e.Message);
            }
            if(version==null)
                version = new Version("0.0.0.0");
            var manifest = new ModuleManifest
                               {
                                   Issuer = _argumentsParser.IssuerName,
                                   ModuleName = _argumentsParser.AssemblyName,
                                   ModuleVersion = version,
                                   SignedFiles = signedFiles.ToList()
                               };
            byte[] manifestSerialized = XmlSerializerHelper.Serialize(manifest);

            string manifestPath = string.Format("{0}{1}", GetAssemblyPath(),
                                                ModuleManifest.ManifestFileNameSuffix);

            File.WriteAllBytes(manifestPath, manifestSerialized);
            File.WriteAllBytes(manifestPath + ModuleManifest.ManifestSignatureFileNameSuffix,
                               _signatureAlgorithm.Sign(File.ReadAllBytes(manifestPath)));
        }
    }
}