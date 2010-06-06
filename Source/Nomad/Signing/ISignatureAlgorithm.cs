namespace Nomad.Signing
{
    /// <summary>
    /// Responsible for signing and verifying signature 
    /// </summary>
    public interface ISignatureAlgorithm
    {
        /// <summary>
        /// Signs provided data
        /// </summary>
        /// <param name="data">Data to sign</param>
        /// <returns>Signature of sign</returns>
        byte[] Sign(byte[] data);


        /// <summary>
        /// Verifies provided data agains provided signature
        /// </summary>
        /// <param name="data">data to be verified</param>
        /// <param name="signature">signature to verificate agains</param>
        /// <returns>true if verification was successful</returns>
        bool Verify(byte[] data, byte[] signature);
    }
}