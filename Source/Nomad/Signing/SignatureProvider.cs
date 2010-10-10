using System;
using System.Collections.Generic;

namespace Nomad.Signing
{
    ///<summary>
    /// Container for trusted signatures
    ///</summary>
    public class SignatureProvider : ISignatureProvider
    {
        private readonly IDictionary<string, IssuerInformation> _issuers =
            new Dictionary<string, IssuerInformation>();

        #region ISignatureProvider Members

        /// <summary>
        /// Registers provided issuer
        /// </summary>
        /// <param name="issuerInformation"></param>
        /// <exception cref="NullReferenceException">when <paramref name="issuerInformation"/> is <c>null</c></exception>
        public void Register(IssuerInformation issuerInformation)
        {
            if (issuerInformation == null)
                throw new NullReferenceException("issuerInformation");
            _issuers[issuerInformation.IssuerName] = issuerInformation;
        }


        /// <summary>
        /// Verifies if provider exists and returns its information, all returns provider with NullSignatureAlgorithm, when provider does not exist.
        /// </summary>
        /// <param name="issuerName">issuer name to look for</param>
        /// <returns></returns>
        public IssuerInformation GetIssuer(string issuerName)
        {
            IssuerInformation issuerInformation;
            if (_issuers.TryGetValue(issuerName, out issuerInformation))
                return issuerInformation;
            return new IssuerInformation(issuerName, new NullSignatureAlgorithm());
        }

        #endregion
    }
}