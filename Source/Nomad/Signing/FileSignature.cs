using System;

namespace Nomad.Signing
{
    /// <summary>
    /// File signature structure to store information about assembly
    /// </summary>
    [Serializable]
    public class FileSignature
    {
        /// <summary>
        /// Name of Issuer
        /// </summary>
        public string IssuerName { get; set; }

        /// <summary>
        /// Signature of file
        /// </summary>
        public byte[] Signature { get; set; }

        /// <summary>
        /// Filename which was signed
        /// </summary>
        public string FileName { get; set; }
    }
}