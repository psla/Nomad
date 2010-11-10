using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Nomad.Modules.Manifest;
using Nomad.Signing;
using Nomad.Signing.FileUtils;
using Nomad.Signing.SignatureAlgorithms;
using File = System.IO.File;

namespace Nomad.Utils
{
    /// <summary>
    ///     Tool for creating Nomad compliant manifests for existing assemblies.
    /// </summary>
    public class ManifestBuilder
    {
        private readonly string _assemblyName;
        private readonly string _directory;
        private readonly string _issuerName;
        private readonly string _issuerXmlPath;

        private readonly string _keyPassword;
        private readonly KeyStorage _keyStore;

        private RSACryptoServiceProvider _key;
        private ISignatureAlgorithm _signatureAlgorithm;


        /// <summary>
        ///     Initializes the new instance of <see cref="ManifestBuilder"/> class.
        /// </summary>
        /// <param name="issuerName">Name of the issuer of the signing.</param>
        /// <param name="issuerXmlPath">Path to the file with issuer.</param>
        /// <param name="assemblyName">Name of the assembly for which manifest is going to be created.</param>
        /// <param name="directory">Directory within this assembly.</param>
        /// <param name="keyStore">Flag which describes whether to use information from PKI or from Nomad key mechanism.</param>
        /// <param name="keyPassword">Password for PKI certificate.</param>
        public ManifestBuilder(string issuerName, string issuerXmlPath, string assemblyName,
                               string directory, KeyStorage keyStore, string keyPassword)
        {
            _issuerName = issuerName;
            _issuerXmlPath = issuerXmlPath;
            _assemblyName = assemblyName;
            _directory = directory;
            _keyStore = keyStore;
            _keyPassword = keyPassword;

            LoadKey();
        }


        /// <summary>
        ///     Initializes the new instance of <see cref="ManifestBuilder"/> class.
        /// </summary>
        /// <remarks>
        ///     Uses Nomad RSA key algorithm to provide security - <see cref="KeyStorage.Nomad"/>.
        /// </remarks>
        /// <param name="issuerName">Name of the issuer of the signing.</param>
        /// <param name="issuerXmlPath">Path to the file with issuer.</param>
        /// <param name="assemblyName">Name of the assembly for which manifest is going to be created.</param>
        /// <param name="directory">Directory within this assembly.</param>
        public ManifestBuilder(string issuerName, string issuerXmlPath, string assemblyName,
                               string directory)
            : this(issuerName, issuerXmlPath, assemblyName, directory, KeyStorage.Nomad, string.Empty)
        {
        }


        private void LoadKey()
        {
            if (_keyStore == KeyStorage.Nomad)
                _signatureAlgorithm =
                    new RsaSignatureAlgorithm(File.ReadAllText(_issuerXmlPath));
            else if ( _keyStore == KeyStorage.PKI )
                _signatureAlgorithm =
                    new PkiSignatureAlgorithm(File.ReadAllBytes(_issuerXmlPath), _keyPassword);
        }


        private string GetAssemblyPath()
        {
            return Path.Combine(_directory, _assemblyName);
        }


        private string GetAssemblyName()
        {
            return AssemblyName.GetAssemblyName(GetAssemblyPath()).Name;
        }


        /// <summary>
        ///     Generates the manifest based on passed parameters.
        /// </summary>
        /// <returns>The newly created manifest.</returns>
        public ModuleManifest Create()
        {
            // get the content of manifest
            Version version = GetVersion();
            IEnumerable<SignedFile> signedFiles = GetSignedFiles();
            IEnumerable<ModuleDependency> dependencyModules = GetDependencyModules2();

            //  create manifest
            var manifest = new ModuleManifest
                               {
                                   Issuer = _issuerName,
                                   ModuleName = GetAssemblyName(),
                                   ModuleVersion = version,
                                   SignedFiles = signedFiles.ToList(),
                                   ModuleDependencies = dependencyModules.ToList()
                               };

            byte[] manifestSerialized = XmlSerializerHelper.Serialize(manifest);

            //  sign the manifest
            string manifestPath = string.Format("{0}{1}", GetAssemblyPath(),
                                                ModuleManifest.ManifestFileNameSuffix);

            File.WriteAllBytes(manifestPath, manifestSerialized);
            File.WriteAllBytes(manifestPath + ModuleManifest.ManifestSignatureFileNameSuffix,
                               _signatureAlgorithm.Sign(File.ReadAllBytes(manifestPath)));

            return manifest;
        }


        private IEnumerable<ModuleDependency> GetDependencyModules2()
        {
            var dependencies = new List<ModuleDependency>();

            // read the directory 
            IEnumerable<string> files = Directory.GetFiles(_directory, "*.dll",
                                                           SearchOption.AllDirectories).Concat(
                                                               Directory.GetFiles(_directory,
                                                                                  "*.exe",
                                                                                  SearchOption.
                                                                                      AllDirectories));
            // mine module
            string myAsm = GetAssemblyName();

            // for each assembly in this collection, try resolving its, name version and so on.)
            foreach (string file in files)
            {
                AssemblyName asmName = null;
                try
                {
                    asmName = AssemblyName.GetAssemblyName(file);
                }
                catch (BadImageFormatException)
                {
                    // nothing happens
                    continue;
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException("Access to file is somewhat corrupted", e);
                }

                // do not add itself
                if (asmName.Name.Equals(myAsm))
                    continue;

                dependencies.Add(new ModuleDependency
                                     {
                                         ModuleName = asmName.Name,
                                         MinimalVersion = new Version(asmName.Version),
                                         ProcessorArchitecture = asmName.ProcessorArchitecture,
                                         //TODO: implement recognition of isServiceProvider ability
                                         HasLoadingOrderPriority = false,
                                     });
            }

            return dependencies;
        }


        private IEnumerable<SignedFile> GetSignedFiles()
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
            return signedFiles;
        }


        private Version GetVersion()
        {
            Version version = null;

            try
            {
                version = new Version(AssemblyName.GetAssemblyName(GetAssemblyPath()).Version);
            }
            catch (Exception)
            {
                //TODO: this cannot be done ! this way. Implement the proper way of logging this exception
                version = new Version("0.0.0.0");
            }

            return version;
        }
    }
}