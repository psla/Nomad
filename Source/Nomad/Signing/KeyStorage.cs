namespace Nomad.Signing
{
    /// <summary>
    ///     Describes the way the manifest for modules should be signed. 
    /// </summary>
    public enum KeyStorage
    {
        /// <summary>
        ///     Use Nomad provided set of RSA-based keys to ensure security.
        /// </summary>
        Nomad,

        /// <summary>
        ///     Use certificate Public Key Infrastructure to sign the assemblies. 
        /// </summary>
        PKI
    }
}