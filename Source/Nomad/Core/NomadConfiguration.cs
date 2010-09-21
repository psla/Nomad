using System;

namespace Nomad.Core
{
    /// <summary>
    /// Contains all informations concerning <see cref="NomadKernel"/> configuration.
    /// This class acts as freezable. Also provides default configuration.
    /// </summary>
    public class NomadConfiguration
    {
        /// <summary>
        /// Determines the state of configuration object.
        /// </summary>
        public bool IsFrozen { get; private set; }

        /// <summary>
        /// Provides default and user-modifiable configuration for Nomad framework.
        /// </summary>
        public static NomadConfiguration Default
        {
            get
            {
                return new NomadConfiguration()
                           {
                           };
            }
        }


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
            AssertNotFrozen();
            IsFrozen = true;
        }
    }
}