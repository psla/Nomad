using System.Collections.Generic;
using Nomad.Signing.FileUtils;

namespace Nomad.Signing
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFileSignatureVerificator
    {
        /// <summary>
        /// Trust specified issuer
        /// </summary>
        /// <param name="issuerInformation"><see cref="IssuerInformation"/> to trust</param>
        void AddTrustedIssuer(IssuerInformation issuerInformation);


        /// <summary>
        /// Check if specified file is trusted
        /// </summary>
        /// <param name="file"><see cref="File"/> to verify</param>
        /// <param name="signature"><see cref="FileSignature"/> of file we are verifying provided</param>
        /// <returns>true when file comes from trusted source</returns>
        bool VerifyFile(File file, FileSignature signature);
    }

    /// <summary>
    /// Responsible for verification provided assembly against trusted sources
    /// </summary>
    public class FileSignatureVerificator : IFileSignatureVerificator
    {
        private readonly IDictionary<string, IssuerInformation> _issuers =
            new Dictionary<string, IssuerInformation>();


        /// <summary>
        /// Trust specified issuer
        /// </summary>
        /// <param name="issuerInformation"><see cref="IssuerInformation"/> to trust</param>
        public void AddTrustedIssuer(IssuerInformation issuerInformation)
        {
            _issuers.Add(issuerInformation.IssuerName, issuerInformation);
        }


        /// <summary>
        /// Check if specified file is trusted
        /// </summary>
        /// <param name="file"><see cref="File"/> to verify</param>
        /// <param name="signature"><see cref="FileSignature"/> of file we are verifying provided</param>
        /// <returns>true when file comes from trusted source</returns>
        public bool VerifyFile(File file, FileSignature signature)
        {
            IssuerInformation information = null;
            if (_issuers.TryGetValue(signature.IssuerName, out information))
            {
                return information.IssuerAlgorithm.Verify(file.Data, signature.Signature)
                       && file.FileName == signature.FileName;
            }
            else
            {
                return false;
            }
        }
    }
}