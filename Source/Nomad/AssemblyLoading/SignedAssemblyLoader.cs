using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Authentication;
using Nomad.Signing;
using File = Nomad.Signing.File;

namespace Nomad.AssemblyLoading
{
    /// <summary>
    /// Loader of assemblies which verifies, that they are signed by trusted authority
    /// </summary>
    public class SignedAssemblyLoader : IAssemblyLoader
    {
        private readonly IFileSignatureVerificator _fileVerificator;
        private readonly IAssemblyLoader _properLoader;

        /// <summary>
        /// Creates loader which verifies 
        /// </summary>
        /// <param name="properLoader"><see cref="IAssemblyLoader"/> which will do the real loading of assemblies</param>
        /// <param name="fileVerificator"><see cref="IFileSignatureVerificator"/> which is responsible for veryfying of correctness of signature</param>
        public SignedAssemblyLoader(IAssemblyLoader properLoader,
                                    IFileSignatureVerificator fileVerificator)
        {
            _properLoader = properLoader;
            _fileVerificator = fileVerificator;
        }

        #region Implementation of IAssemblyLoader

        /// <summary>
        /// Verifies if the assembly has proper signature and loads
        /// </summary>
        /// <param name="name">Name of assembly to load</param>
        /// <exception cref="InvalidCredentialException">Thrown when load of assembly fail due to incorrect signature</exception>
        public void LoadAssembly(string name)
        {
            string signatureName = string.Format("{0}.sig", name);
            FileSignature fileSignature = null;
            
            if (!System.IO.File.Exists(name))
                throw new InvalidCredentialException(string.Format("Missing assembly file for {0}",
                                                                   name));
            if (!System.IO.File.Exists(signatureName))
                throw new InvalidCredentialException(string.Format(
                    "Missing signature file for {0}", name));

            var file = new File(name);

            try
            {
                using (var fs = new FileStream(name, FileMode.Open))
                {
                    var bf = new BinaryFormatter();
                    fileSignature = (FileSignature) bf.Deserialize(fs);
                }
            }
            catch (Exception e)
            {
                //TODO: do some logging here ?
                var exception = new InvalidCredentialException("Invalid signature file", e);
            }

            if(_fileVerificator.VerifyFile(file, fileSignature))
                _properLoader.LoadAssembly(name);
            else
            {
                throw new InvalidCredentialException(string.Format("Invalid signature for assembly {0}", name));
            }
        }

        #endregion
    }
}