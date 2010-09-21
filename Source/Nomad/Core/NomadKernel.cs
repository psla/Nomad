using System;

namespace Nomad.Core
{
    /// <summary>
    /// Nomad's entry point. Presents Nomad's features to the developer.
    /// </summary>
    public class NomadKernel
    {
        /// <summary>
        /// Provides readonly access to initialized Kernel configuration.
        /// </summary>
        public NomadConfiguration KernelConfiguration { get; private set; }


        /// <summary>
        /// Initializes new instance of the <see cref="NomadKernel"/> class.
        /// </summary>
        /// <param name="nomadConfiguration">
        /// <see cref="NomadConfiguration"/> used to initialize kernel modules.
        /// </param>
        public NomadKernel(NomadConfiguration nomadConfiguration)
        {
            if (nomadConfiguration == null)
            {
                throw new ArgumentNullException("nomadConfiguration",
                                                "Configuration must be provided.");
            }
            nomadConfiguration.Freeze();
            KernelConfiguration = nomadConfiguration;
        }


        /// <summary>
        /// Initializes new instance of the <see cref="NomadKernel"/> class.
        /// Uses frozen <see cref="NomadConfiguration.Default"/> as configuration data.
        /// </summary>
        public NomadKernel()
        {
            KernelConfiguration = NomadConfiguration.Default;
            KernelConfiguration.Freeze();
        }
    }
}