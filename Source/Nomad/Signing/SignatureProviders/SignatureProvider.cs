using System;
using System.Collections.Generic;
using Nomad.Signing.SignatureAlgorithms;

namespace Nomad.Signing.SignatureProviders
{
    ///<summary>
    /// Container for trusted signatures
    ///</summary>
    public class SignatureProvider : ISignatureProvider
    {
        private readonly ISignatureAlgorithm _defaultSignatureAlgorithm;

        private readonly IDictionary<string, IssuerInformation> _issuers =
            new Dictionary<string, IssuerInformation>();

        /// <summary>
        /// Initializes <see cref="SignatureProvider"/> with <see cref="NullSignatureAlgorithm"/>
        /// </summary>
        /// <remarks>
        /// Such initialization assumes, that if no vendor-specific algorithm provided, signature check will not pass
        /// </remarks>
        public SignatureProvider() : this(new NullSignatureAlgorithm())
        {
            
        }
        /// <summary>
        /// Initializes <see cref="SignatureProvider"/> with default algorithm
        /// </summary>
        /// <param name="defaultSignatureAlgorithm">When no algorithm for issuer specified, check with provided one</param>
        public SignatureProvider(ISignatureAlgorithm defaultSignatureAlgorithm)
        {
            _defaultSignatureAlgorithm = defaultSignatureAlgorithm;
        }

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
            return new IssuerInformation(issuerName, _defaultSignatureAlgorithm);
        }

        #endregion
    }
}