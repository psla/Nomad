using System;
using Nomad.Modules;

namespace Nomad.Core
{
    /// <summary>
    /// Nomad's entry point. Presents Nomad's features to the developer.
    /// </summary>
    public class NomadKernel
    {
        #region AppDomain Managment

        public AppDomain ModuleAppDomain
        { 
            get; set;
        }

        public AppDomain KernelAppDomain
        {
            get; set;
        }


        #endregion

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

            ModuleManager = new ModuleManager(KernelConfiguration.ModuleLoader,
                                              KernelConfiguration.ModuleFilter);
        }


        /// <summary>
        /// Initializes new instance of the <see cref="NomadKernel"/> class.
        /// Uses frozen <see cref="NomadConfiguration.Default"/> as configuration data.
        /// </summary>
        public NomadKernel() : this(NomadConfiguration.Default)
        {
        }


        /// <summary>
        /// Provides readonly access to initialized Kernel configuration.
        /// </summary>
        public NomadConfiguration KernelConfiguration { get; private set; }

        /// <summary>
        /// Provides readonly access to already initialized ModuleManager
        /// </summary>
        public ModuleManager ModuleManager { get; private set; }
    }
}