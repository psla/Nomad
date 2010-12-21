using System;
using Nomad.Modules;
using Nomad.Modules.Filters;
using Nomad.Signing.SignatureAlgorithms;
using Nomad.Signing.SignatureProviders;
using Nomad.Updater;
using Nomad.Updater.ModulePackagers;
using Nomad.Updater.ModuleRepositories;

namespace Nomad.Core
{
    /// <summary>
    /// Contains all information concerning <see cref="NomadKernel"/> configuration.
    /// This class acts as freezable. Also provides default configuration.
    /// </summary>
    public class NomadConfiguration 
    {
        internal NomadConfiguration()
        {
            
        }

        #region Configuration

        private IDependencyChecker _dependencyChecker;
        private string _moduleDirectoryPath;
        private IModuleFilter _moduleFilter;
        private IModulePackager _modulePackager;
        private IModulesRepository _moduleRepository;
        private UpdaterType _updaterType;

        /// <summary>
        ///     Implementation of <see cref="IModuleFilter"/> which will be used by Kernel.
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

        /// <summary>
        ///     Place where all modules are stored.
        /// </summary>
        public string ModuleDirectoryPath
        {
            get { return _moduleDirectoryPath; }
            set
            {
                AssertNotFrozen();
                _moduleDirectoryPath = value;
            }
        }

        /// <summary>
        ///     Engine reposnsible for decoding packages from module repository.
        /// </summary>
        public IModulePackager ModulePackager
        {
            get { return _modulePackager; }
            set
            {
                AssertNotFrozen();
                _modulePackager = value;
            }
        }

        /// <summary>
        ///     Module repository responsible for connecting to update center.
        /// </summary>
        public IModulesRepository ModuleRepository
        {
            get { return _moduleRepository; }
            set
            {
                AssertNotFrozen();
                _moduleRepository = value;
            }
        }

        /// <summary>
        ///     Type of the updater to be used for the application.
        /// </summary>
        public UpdaterType UpdaterType
        {
            get { return _updaterType; }
            set
            {
                AssertNotFrozen();
                _updaterType = value;
            }
        }

        /// <summary>
        ///     Implementation of <see cref="IDependencyChecker"/> which will be used by Kernel.
        /// </summary>
        /// <exception cref="InvalidOperationException">Raised when accessing frozen configuration.</exception>
        public IDependencyChecker DependencyChecker
        {
            get { return _dependencyChecker; }
            set
            {
                AssertNotFrozen();
                _dependencyChecker = value;
            }
        }

        #endregion

        /// <summary>
        ///     Provides default and user-modifiable configuration for Nomad framework.
        /// </summary>
        public static NomadConfiguration Default
        {
            get
            {
                return new NomadConfiguration
                           {
                               //TODO: Review the idea of default implementation.
                               ModuleFilter = new CompositeModuleFilter(new IModuleFilter[] {}),
                               DependencyChecker = new DependencyChecker(),
                               UpdaterType = UpdaterType.Manual,
                               ModulePackager = new ModulePackager(),
                               SignatureProvider =
                                   new SignatureProvider(new NullSignatureAlgorithm())
                           };
            }
        }

        #region Freeze Implementation

        private bool _isFrozen;

        /// <summary>
        ///     Determines the state of configuration object.
        /// </summary>
        public bool IsFrozen
        {
            get { return _isFrozen; }
        }

        private ISignatureProvider _signatureProvider;

        /// <summary>
        ///<para>     
        ///     Determines the way of signature verification. By default - not defined signatures issuer are not TRUSTED and will be denied.
        /// </para>
        ///<para>
        /// You may make use of <see cref="PkiSignatureAlgorithm"/> (simply assign <example>
        /// <code>
        /// nomadConfiguration.SignatureProvider = new SignatureProvider(new PkiSignatureAlgorithm())
        /// </code></example>
        /// You may also make use of other <see cref="ISignatureAlgorithm"/>
        /// </para>
        /// </summary>
        public ISignatureProvider SignatureProvider
        {
            get { return _signatureProvider; }
            private set
            {
                AssertNotFrozen();
                _signatureProvider = value;
            }
        }


        /// <summary>
        ///     Checks whether current instance is already frozen.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     If object is already frozen.
        /// </exception>
        private void AssertNotFrozen()
        {
            if (IsFrozen)
            {
                throw new InvalidOperationException("This configuration object is frozen.");
            }
        }


        /// <summary>
        ///     Freezes the configuration.
        /// </summary>
        public void Freeze()
        {
            _isFrozen = true;
        }

        #endregion
    }
}