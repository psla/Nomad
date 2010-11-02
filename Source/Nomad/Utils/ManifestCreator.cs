using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Nomad.Modules.Manifest;
using Nomad.Signing.FileUtils;
using Nomad.Signing.SignatureAlgorithms;
using File = System.IO.File;

namespace Nomad.Utils
{
    /// <summary>
    ///     Tool for creating Nomad compliant manifests for existing assemblies.
    /// </summary>
    public class ManifestCreator
    {
        private readonly string _assemblyName;
        private readonly string _directory;
        private readonly string _issuerName;
        private readonly string _issuerXmlPath;


        private RSACryptoServiceProvider _key;
        private ISignatureAlgorithm _signatureAlgorithm;


        /// <summary>
        ///     Initializes the new instance of <see cref="ManifestCreator"/> class.
        /// </summary>
        /// <param name="issuerName">Name of the issuer of the signing.</param>
        /// <param name="issuerXmlPath">Path to the file with issuer.</param>
        /// <param name="assemblyName">Name of the assembly for which manifest is going to be created.</param>
        /// <param name="directory">Directory within this assembly.</param>
        public ManifestCreator(string issuerName, string issuerXmlPath, string assemblyName,
                               string directory)
        {
            _issuerName = issuerName;
            _issuerXmlPath = issuerXmlPath;
            _assemblyName = assemblyName;
            _directory = directory;

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
            return Path.Combine(_directory, _assemblyName);
        }

        /// <summary>
        ///     Generates the manifest based on passed parameters.
        /// </summary>
        public void Create()
        {
            string[] files = Directory.GetFiles(_directory, "*",
                                                SearchOption.AllDirectories);

            IEnumerable<SignedFile> signedFiles = from file in files
                                                  select
                                                      new SignedFile
                                                          {
                                                              FilePath =
                                                                  file.Substring(
                                                                      _directory.
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
            catch (Exception e)
            {
                Console.WriteLine("Cannot infer assembly version from assembly. {0}", e.Message);
            }
            if (version == null)
                version = new Version("0.0.0.0");
            var manifest = new ModuleManifest
                               {
                                   Issuer = _issuerName,
                                   ModuleName = _assemblyName,
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