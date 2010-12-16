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

namespace Nomad.Utils.ManifestCreator
{
    /// <summary>
    ///     Tool for creating Nomad compliant manifests for existing assemblies.
    /// </summary>
    /// <remarks>
    ///     This tool is simplest possible version of ManifestBuilder concept - it has few limitations though. Nomad default values are used from 
    /// <see cref="ManifestBuilderConfiguration.Default"/> and are the only authors proposition for creating manifests.
    /// <para>
    ///     It can be used with two types of cryptographic security.
    /// </para>
    ///     <c>To change any of this settings provide the <see cref="ManifestBuilderConfiguration"/> object into <see cref="ManifestBuilder"/>.</c>
    /// </remarks>
    public class ManifestBuilder
    {
        private readonly string _assemblyName;
        private readonly ManifestBuilderConfiguration _configuration;
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
        /// <param name="configuration">Configuration on manifest builder class.</param>
        public ManifestBuilder(string issuerName, string issuerXmlPath, string assemblyName,
                               string directory, KeyStorage keyStore, string keyPassword,
                               ManifestBuilderConfiguration configuration)
        {
            _issuerName = issuerName;
            _issuerXmlPath = issuerXmlPath;
            _assemblyName = assemblyName;
            _directory = directory;
            _keyStore = keyStore;
            _keyPassword = keyPassword;

            if (configuration == null)
                throw new ArgumentNullException("configuration");

            _configuration = configuration;

            LoadKey();
        }


        /// <summary>
        ///     Initializes the new instance of the <see cref="ManifestBuilder"/> class.
        /// </summary>
        /// <param name="issuerName"></param>
        /// <param name="issuerXmlPath"></param>
        /// <param name="assemblyName"></param>
        /// <param name="directory"></param>
        /// <param name="keyStore"></param>
        /// <param name="keyPassword"></param>
        public ManifestBuilder(string issuerName, string issuerXmlPath, string assemblyName,
                               string directory, KeyStorage keyStore, string keyPassword)
            : this(
                issuerName, issuerXmlPath, assemblyName, directory, keyStore, keyPassword,
                ManifestBuilderConfiguration.Default)
        {
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
            : this(
                issuerName, issuerXmlPath, assemblyName, directory, KeyStorage.Nomad, string.Empty,
                ManifestBuilderConfiguration.Default)
        {
        }


        private void LoadKey()
        {
            if (_keyStore == KeyStorage.Nomad)
                _signatureAlgorithm =
                    new RsaSignatureAlgorithm(File.ReadAllText(_issuerXmlPath));
            else if (_keyStore == KeyStorage.PKI)
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
        ///     Generates the manifest based on passed parameters and <c>saves manifest to file system</c>
        /// </summary>
        /// <remarks>
        ///     Performs the signing of the manifest file.
        /// </remarks>
        /// <returns>The newly created manifest.</returns>
        public ModuleManifest CreateAndPublish()
        {
            // perform creation
            ModuleManifest manifest = Create();

            byte[] manifestSerialized = XmlSerializerHelper.Serialize(manifest);

            //  sign the manifest
            string manifestPath = string.Format("{0}{1}", GetAssemblyPath(),
                                                ModuleManifest.ManifestFileNameSuffix);

            File.WriteAllBytes(manifestPath, manifestSerialized);
            File.WriteAllBytes(manifestPath + ModuleManifest.ManifestSignatureFileNameSuffix,
                               _signatureAlgorithm.Sign(File.ReadAllBytes(manifestPath)));

            return manifest;
        }


        /// <summary>
        ///     Generates manifest in memory.
        /// </summary>
        /// <remarks>
        ///     Does <c>not</c> sign the manifest, cause there is no file to sign.
        /// </remarks>
        /// <returns>The newly created manifest</returns>
        public ModuleManifest Create()
        {
            // get the content of manifest from various engines.
            Version version = _configuration.VersionProvider.GetVersion(GetAssemblyPath());

            IEnumerable<SignedFile> signedFiles =
                _configuration.SignedFilesProvider.GetSignedFiles(_directory, _signatureAlgorithm);

            IEnumerable<ModuleDependency> dependencyModules =
                _configuration.ModulesDependenciesProvider.GetDependencyModules(_directory,
                                                                                GetAssemblyPath());

            //  create manifest
            var manifest = new ModuleManifest
                               {
                                   Issuer = _issuerName,
                                   ModuleName = GetAssemblyName(),
                                   ModuleVersion = version,
                                   SignedFiles = signedFiles.ToList(),
                                   ModuleDependencies = dependencyModules.ToList()
                               };

            return manifest;
        }
    }
}