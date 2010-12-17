using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Linq;

namespace Nomad.Signing.SignatureAlgorithms
{
    /// <summary>
    /// Signature algorithm using PKI infrastructure
    /// </summary>
    public class PkiSignatureAlgorithm : ISignatureAlgorithm
    {
        private readonly X509Certificate2Collection _certificate2Collection;
        private readonly X509Certificate2 _cert;
        private MD5 _md5;


        /// <summary>
        /// Create instance with private key or public cert.
        /// </summary>
        /// <remarks>
        /// Supports only RSA algorithm
        /// </remarks>
        /// <param name="rawData">X509 certificate (with or without public key)</param>
        /// <param name="password">passphrase to private key. Leave empty if none</param>
        public PkiSignatureAlgorithm(byte[] rawData, string password) : this(new X509Store(StoreLocation.CurrentUser).Certificates)
        {
            if(string.IsNullOrEmpty(password))
                _cert = new X509Certificate2(rawData);
            else
                _cert = new X509Certificate2(rawData, password);
        }

        /// <summary>
        /// Creates instance of pki signature algorithm which allows verification against certificate collection
        /// </summary>
        /// <param name="certificate2Collection">Collection of additional (to system one) certificates. Remember, that providing a CA certificate, you should provide certificate revocation list!</param>
        public PkiSignatureAlgorithm(X509Certificate2Collection certificate2Collection)
        {
            _certificate2Collection = certificate2Collection;
            _md5 = MD5.Create();
        }

        /// <summary>
        /// Creates instance of pki signature which allows verification against system certificates collection
        /// </summary>
        public PkiSignatureAlgorithm() : this (new X509Certificate2Collection())
        {
            
        }


        public byte[] Sign(byte[] data)
        {
            
            ContentInfo contentInfo = new ContentInfo(_md5.ComputeHash(data));
            SignedCms signedCms = new SignedCms(contentInfo);
            CmsSigner cmsSigner = new CmsSigner(_cert);
            cmsSigner.IncludeOption = X509IncludeOption.WholeChain;
            signedCms.ComputeSignature(cmsSigner);
            return signedCms.Encode();
        }


        public bool Verify(byte[] data, byte[] signature)
        {
            var signedCms = new SignedCms();
            signedCms.Decode(signature);
            try
            {
                signedCms.CheckSignature(_certificate2Collection, false);
            }
            catch(Exception e)
            {
                return false;
            }
            return signedCms.ContentInfo.Content.SequenceEqual(_md5.ComputeHash(data));
        }
    }
}