namespace Nomad.Signing
{
    /// <summary>
    /// Signature which always denies verification and returns empty signature
    /// </summary>
    public class NullSignatureAlgorithm : ISignatureAlgorithm
    {
        #region ISignatureAlgorithm Members

        /// <summary>
        /// Return empty signature
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Sign(byte[] data)
        {
            return new byte[0];
        }


        /// <summary>
        /// Deny verification
        /// </summary>
        /// <param name="data"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        public bool Verify(byte[] data, byte[] signature)
        {
            return false;
        }

        #endregion
    }
}