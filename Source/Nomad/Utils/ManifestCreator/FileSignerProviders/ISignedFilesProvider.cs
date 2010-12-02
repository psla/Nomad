using System.Collections.Generic;
using Nomad.Signing.FileUtils;
using Nomad.Signing.SignatureAlgorithms;

namespace Nomad.Utils.ManifestCreator.FileSignerProviders
{
    /// <summary>
    ///     Responsible for generating the collection of <see cref="SignedFile"/>.
    /// </summary>
    public interface ISignedFilesProvider
    {
        ///<summary>
        ///     Gets the enumerable of singed files.
        ///</summary>
        ///<param name="directory">Directory in which the files to be signed are.</param>
        ///<param name="signatureAlgorithm">The way to sign the files.</param>
        ///<returns></returns>
        IEnumerable<SignedFile> GetSignedFiles(string directory,
                                               ISignatureAlgorithm signatureAlgorithm);
    }
}