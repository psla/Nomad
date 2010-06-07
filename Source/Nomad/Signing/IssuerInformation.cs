namespace Nomad.Signing
{
    /// <summary>
    /// Information structure about issuer
    /// </summary>
    public class IssuerInformation
    {
        /// <summary>
        /// Creates instance of IssuerInformation using specified <see cref="ISignatureAlgorithm"/> and Issuername
        /// </summary>
        /// <param name="issuerName">Name of issuer. Suggested FQDN.</param>
        /// <param name="issuerAlgorithm">Algorithm issuer use to sign files</param>
        public IssuerInformation(string issuerName, ISignatureAlgorithm issuerAlgorithm)
        {
            IssuerName = issuerName;
            IssuerAlgorithm = issuerAlgorithm;
        }


        /// <summary>
        /// Unique Name of issuer. Suggested FQDN.
        /// </summary>
        public string IssuerName { get; private set; }

        /// <summary>
        /// Algorithm used by issuer to sign files
        /// </summary>
        public ISignatureAlgorithm IssuerAlgorithm { get; private set; }
    }
}