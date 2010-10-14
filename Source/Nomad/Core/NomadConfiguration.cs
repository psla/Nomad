using System;
using Castle.Windsor;
using Nomad.Modules;

namespace Nomad.Core
{
    /// <summary>
    /// Contains all information concerning <see cref="NomadKernel"/> configuration.
    /// This class acts as freezable. Also provides default configuration.
    /// </summary>
    public class NomadConfiguration
    {
        #region Windsor Container

        private IWindsorContainer _windsorContainer;

        /// <summary>
        ///     Container to be used within Nomad. Compliant with <see cref="IWindsorContainer"/> interface.
        /// </summary>
        /// <remarks>
        ///     This is the highest place with Nomad Framework for IoC Container to be defined. 
        ///     Other parts of the framework would eventually use container defined in this place.
        /// </remarks>
        public IWindsorContainer WindsorContainer
        {
            get { return _windsorContainer; }
            set
            {
                AssertNotFrozen();
                _windsorContainer = value;
            }
        }

        #endregion

        #region Configuration

        private IModuleFilter _moduleFilter;
        private IModuleLoader _moduleLoader;


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
                var container = new WindsorContainer();
                return new NomadConfiguration
                           {
                               WindsorContainer = container,
                               ModuleFilter = new CompositeModuleFilter(new IModuleFilter[] {}),
                               ModuleLoader = new ModuleLoader(container)
                           };
            }
        }

        #region Freeze Implementation

        /// <summary>
        /// Determines the state of configuration object.
        /// </summary>
        public bool IsFrozen { get; private set; }


        /// <summary>
        /// Checks whether current instance is already frozen.
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