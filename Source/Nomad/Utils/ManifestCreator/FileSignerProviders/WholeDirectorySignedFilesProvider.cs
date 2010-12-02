using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nomad.Signing.FileUtils;
using Nomad.Signing.SignatureAlgorithms;
using File = System.IO.File;

namespace Nomad.Utils.ManifestCreator.FileSignerProviders
{
    ///<summary>
    ///     Signs all files in the provided directory.
    ///</summary>
    public class WholeDirectorySignedFilesProvider : ISignedFilesProvider
    {
        #region ISignedFilesProvider Members

        public IEnumerable<SignedFile> GetSignedFiles(string directory,
                                                      ISignatureAlgorithm signatureAlgorithm)
        {
            string[] files = Directory.GetFiles(directory, "*",
                                                SearchOption.AllDirectories);

            IEnumerable<SignedFile> signedFiles = from file in files
                                                  select
                                                      new SignedFile
                                                          {
                                                              FilePath =
                                                                  file.Substring(
                                                                      directory.
                                                                          Length),
                                                              Signature =
                                                                  signatureAlgorithm.Sign(
                                                                      File.ReadAllBytes(file))
                                                          };
            return signedFiles;
        }

        #endregion
    }
}