using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Nomad.Modules.Manifest;
using Nomad.Signing;
using Nomad.Signing.FileUtils;
using Nomad.Signing.SignatureAlgorithms;
using Nomad.Utils;
using File = System.IO.File;

namespace Nomad.ManifestCreator
{
    public class ManifestCreator
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
                                                              FilePath =
                                                                  file.Substring(
                                                                      _argumentsParser.Directory.
                                                                          Length),
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

            string manifestPath = string.Format("{0}{1}", GetAssemblyPath(),
                                                ModuleManifest.ManifestFileNameSuffix);

            File.WriteAllBytes(manifestPath, manifestSerialized);
            File.WriteAllBytes(manifestPath + ModuleManifest.ManifestSignatureFileNameSuffix,
                               _signatureAlgorithm.Sign(File.ReadAllBytes(manifestPath)));
        }
    }
}