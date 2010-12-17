using System;

namespace Nomad.Signing.SignatureAlgorithms
{
    /// <summary>
    /// This signature algorithm does not make any use of signature - simply always accepts it.
    /// </summary>
    /// <remarks>
    /// Remember, that no signature test is being performed.
    /// </remarks>
    public class AcceptSignatureAlgorithm : ISignatureAlgorithm
    {
        /// <summary>
        /// You cannot sign anything using this kind of signature algorithm
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Sign(byte[] data)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// This method always return true.
        /// </summary>
        /// <param name="data">whatever</param>
        /// <param name="signature">whatever</param>
        /// <returns>true</returns>
        public bool Verify(byte[] data, byte[] signature)
        {
            return true;
        }
    }
}