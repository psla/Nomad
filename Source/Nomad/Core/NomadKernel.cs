namespace Nomad.Core
{
    /// <summary>
    /// Nomad's entry point. Presents Nomad's features to the developer.
    /// </summary>
    public class NomadKernel
    {
        /// <summary>
        /// Initializes new instance of the <see cref="NomadKernel"/> class.
        /// </summary>
        /// <param name="nomadConfiguration">
        /// Configuration used to initialize kernel modules.
        /// If this parameter is NULL, then Nomad uses default configuration available as <see cref="NomadConfiguration.Default"/> property.
        /// </param>
        public NomadKernel(NomadConfiguration nomadConfiguration)
        {
            NomadConfiguration tempKernelConfiguration;
            if (nomadConfiguration == null)
            {
                tempKernelConfiguration = NomadConfiguration.Default;
            }
            else
            {
                tempKernelConfiguration = nomadConfiguration;
            }
        }
    }
}