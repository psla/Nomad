using System;
using Nomad.Modules;
using Nomad.Modules.Filters;
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
        #region Configuration

        private IDependencyChecker _dependencyChecker;
        private IModuleFilter _moduleFilter;
        private UpdaterType _updaterType;
        private string _moduleDirectoryPath;
        private IModulesRepository _moduleRepository;
        private IModulePackager _modulePackager;

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
            get {return _moduleDirectoryPath; }
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
            get
            {
                return _modulePackager;
            }
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
            get
            {
                return _moduleRepository;
            }
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
                           };
            }
        }

        #region Freeze Implementation

        /// <summary>
        ///     Determines the state of configuration object.
        /// </summary>
        public bool IsFrozen { get; private set; }

        


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
            IsFrozen = true;
        }

        #endregion
    }
}