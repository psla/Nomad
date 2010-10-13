using System;
using System.IO;
using Nomad.Modules.Manifest;
using Nomad.Signing;
using Nomad.Utils;
using File = System.IO.File;

namespace Nomad.Modules
{
    ///<summary>
    /// Performs signature filtering
    ///</summary>
    /// <remarks>
    /// <para>Module Manifest has to be saved as AssemblyName.dll.manifest. 
    /// When module manifest is missing, then module won't be loaded. 
    /// If some signatures for dll, exe or manifest will be missing, module won't be loaded.
    /// </para>
    /// <para>
    /// To verify correctness of manifest, there have to be file AssemblyName.dll.manifest.asc with signature of manifest file. 
    /// If such file is missing, module won't be loaded.
    /// </para>
    /// </remarks>
    public class SignatureModuleFilter : IModuleFilter
    {
        private readonly ISignatureProvider _signatureProvider;


        ///<summary>
        /// Signature module filter requires container with trusted signatures to check module signature against.
        ///</summary>
        ///<param name="signatureProvider">container with trusted signatures</param>
        public SignatureModuleFilter(ISignatureProvider signatureProvider)
        {
            _signatureProvider = signatureProvider;
        }

        #region IModuleFilter Members

        /// <summary>
        /// Checks, if provided module has all necessary data and that it signature is trusted
        /// </summary>
        /// <remarks>
        /// <para>
        /// Firstly, it verifies that module has manifest, and that manifest has its signature.
        /// Secondly, it verifies that manifest matches it signatures.
        /// Than it verifies, that each file mentioned in manifest is present and matches its signature.
        /// </para>
        /// </remarks>
        /// <param name="moduleInfo">module to verify</param>
        /// <returns>true if all conditions are fullfilled, otherwise false - preventing module from loading</returns>
        public bool Matches(ModuleInfo moduleInfo)
        {
            string modulePath = moduleInfo.AssemblyPath;
            string basePath = Path.GetDirectoryName(modulePath);
            string manifestPath = modulePath + ModuleManifest.ManifestFileNameSuffix;
            string manifestSig = manifestPath + ModuleManifest.ManifestSignatureFileNameSuffix;

            try
            {
                //firstly - deserialize manifest
                var manifest =
                    XmlSerializerHelper.Deserialize<ModuleManifest>(File.ReadAllBytes(manifestPath));

                //get current issuer
                IssuerInformation issuer = _signatureProvider.GetIssuer(manifest.Issuer);

                //verify signature of manifest
                //TODO: Would we like to inform user, while module loading failed?
                if (!File.Exists(manifestSig))
                    return false;
                if (!issuer.IssuerAlgorithm.Verify(File.ReadAllBytes(manifestPath),
                                                   File.ReadAllBytes(manifestSig)))
                    return false;

                //verify all file signatures
                foreach (SignedFile signedFile in manifest.SignedFiles)
                {
                    string filePath = Path.Combine(basePath, signedFile.FilePath);
                    if (
                        !issuer.IssuerAlgorithm.Verify(File.ReadAllBytes(filePath),
                                                       signedFile.Signature))
                        return false;
                }
            }
            catch (Exception e)
            {
                //TODO: any exception should be logged!
                //if exception occurs, it means that something went bad and module is not correct
                return false;
            }

            //if we are here, then none of tests failed.
            return true;
        }

        #endregion
    }
}