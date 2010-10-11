using System;
using Nomad.Modules;

namespace Nomad.Core
{
    /// <summary>
    /// Contains all informations concerning <see cref="NomadKernel"/> configuration.
    /// This class acts as freezable. Also provides default configuration.
    /// </summary>
    public class NomadConfiguration
    {
        #region Configuration

        private IModuleLoader _moduleLoader;
        private IModuleFilter _moduleFilter;

        /// <summary>
        /// Implementation of <see cref="IModuleLoader"/> which will be used by Kernel.
        /// </summary>
        /// <exception cref="InvalidOperationException">Raised when accessing frozen configuration.</exception>
        public IModuleLoader ModuleLoader
        {
            get { return _moduleLoader; }
            set
            {
                AssertNotFrozen();
                _moduleLoader = value;
            }
        }

        /// <summary>
        /// Implementation of <see cref="IModuleFilter"/> which will be used by Kernel.
        /// </summary>
        /// <exception cref="InvalidOperationException">Raised when accessing frozen configuration.</exception>
        public IModuleFilter ModuleFilter
        {
            get { return _moduleFilter; }
            set
            {
                AssertNotFrozen();
                _moduleFilter = value;
            }
        }

        #endregion

        /// <summary>
        /// Provides default and user-modifiable configuration for Nomad framework.
        /// </summary>
        public static NomadConfiguration Default
        {
            get
            {
                return new NomadConfiguration
                           {
                               //ModuleFilter = new CompositeModuleFilter(),
                               //ModuleLoader = new ModuleLoader()
                           };
            }
        }

        #region Freeze Implementation

        /// <summary>
        /// Determines the state of configuration object.
        /// </summary>
        public bool IsFrozen { get; private set; }


        /// <summary>
        /// Checks wheter current instance is already frozen.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If object is already frozen.
        /// </exception>
        private void AssertNotFrozen()
        {
            if (IsFrozen)
            {
                throw new InvalidOperationException("This configuration object is frozen.");
            }
        }


        /// <summary>
        /// Freezes the configuration.
        /// </summary>
        public void Freeze()
        {
            IsFrozen = true;
        }

        #endregion
    }
}