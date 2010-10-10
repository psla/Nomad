namespace Nomad.Signing
{
    /// <summary>
    /// Manages issuers signatures
    /// </summary>
    public interface ISignatureProvider
    {
        /// <summary>
        /// Register issuer as trusted
        /// </summary>
        /// <param name="issuerInformation">issuer signature</param>
        void Register(IssuerInformation issuerInformation);


        /// <summary>
        /// Looks for specified issuer and returns its information
        /// </summary>
        /// <param name="issuerName">name of issuer to search for</param>
        IssuerInformation GetIssuer(string issuerName);
    }
}