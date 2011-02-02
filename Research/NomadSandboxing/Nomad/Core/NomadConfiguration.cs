using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using Nomad.Modules;
using Nomad.Modules.Filters;
using Nomad.Signing.SignatureAlgorithms;
using Nomad.Signing.SignatureProviders;
using Nomad.Updater;
using Nomad.Updater.ModuleFinders;
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


        /// <summary>
        ///     Provides default and user-modifiable configuration for Nomad framework.
        /// </summary>
        public static NomadConfiguration Default
        {
            get
            {
                return new NomadConfiguration
                           {
                               ModuleFilter = new CompositeModuleFilter(new IModuleFilter[] {}),
                               DependencyChecker = new DependencyChecker(),
                               UpdaterType = UpdaterType.Manual,
                               ModulePackager = new ModulePackager(),
                               ModuleFinder = new ModuleFinder(),
                               ModuleDirectoryPath =
                                   Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "modules"),
                               SignatureProvider =
                                   new SignatureProvider(new NullSignatureAlgorithm()),
                               _defaultSecurityChanged = false,
                               _fullTrustAssembliesSet = new List<StrongName>(),
                               _modulesDomainPermissions = new PermissionSet(PermissionState.None)
                           };
            }
        }


        private IDependencyChecker _dependencyChecker;
        private string _moduleDirectoryPath;
        private IModuleFilter _moduleFilter;
        private IModuleFinder _moduleFinder;
        private IModulePackager _modulePackager;
        private IModulesRepository _moduleRepository;
        private ISignatureProvider _signatureProvider;
        private UpdaterType _updaterType;
        private PermissionSet _modulesDomainPermissions;
        private List<StrongName> _fullTrustAssembliesSet;
        private bool _defaultSecurityChanged;

        /// <summary>
        /// For Kernel purpose to know which AppDomain Creator should be used.
        /// </summary>
        internal bool DefaultSecurityChanged
        {
            get { return _defaultSecurityChanged; }
        }

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
        ///     Determines the way <see cref="NomadUpdater"/> has to perform placing the packages.
        /// </summary>
        public IModuleFinder ModuleFinder
        {
            get { return _moduleFinder; }
            set
            {
                AssertNotFrozen();
                _moduleFinder = value;
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

        ///<summary>
        /// PermissionSet that is applied on the created modules domain.
        ///</summary>
        public PermissionSet ModulesDomainPermissions
        {
            get { return _modulesDomainPermissions; }
            set
            {
                AssertNotFrozen();
                _defaultSecurityChanged = true;
                _modulesDomainPermissions = value;
            }
        }

        ///<summary>
        /// List of fully trusted assemblies.
        ///</summary>
        /// <remarks>
        /// Automatically adds Nomad provided assemblies as fully trusted.
        /// </remarks>
        public List<StrongName> FullTrustAssembliesSet
        {
            get
            {
                if (_fullTrustAssembliesSet == null)
                {
                    _fullTrustAssembliesSet = new List<StrongName>();
                }
                _fullTrustAssembliesSet.AddRange(NomadTrustesAssemblies);
                return _fullTrustAssembliesSet;
            }
            set
            {
                AssertNotFrozen();
                _defaultSecurityChanged = true;
                _fullTrustAssembliesSet = value;
            }
        }

        /// <summary>
        /// List of Assemblies provided by Nomad that must have fullTrust.
        /// </summary>
        private List<StrongName> NomadTrustesAssemblies
        {
            get
            {
                return new List<StrongName>
                           {
                               CreateStrongName(Assembly.GetExecutingAssembly()), //Nomad
                               CreateStrongName(Assembly.GetAssembly(typeof(Castle.Windsor.WindsorContainer))), //Castle.Windsor
                               CreateStrongName(Assembly.GetAssembly(typeof(Castle.Core.CollectionExtensions))), //Castle.Core
                               CreateStrongName(Assembly.GetAssembly(typeof(Ionic.Zip.ZipFile))) //Ionic.Zip.Reduced
                           };
            }
        }


        /// <summary>
        /// Create a StrongName that matches a specific assembly
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="assembly"/> is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// if <paramref name="assembly"/> does not represent a strongly named assembly
        /// </exception>
        /// <param name="assembly">Assembly to create a StrongName for</param>
        /// <returns>A StrongName that matches the given assembly</returns>
        public static StrongName CreateStrongName(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            AssemblyName assemblyName = assembly.GetName();

            // get the public key blob
            byte[] publicKey = assemblyName.GetPublicKey();
            if (publicKey == null || publicKey.Length == 0)
                throw new InvalidOperationException("Assembly is not strongly named");

            var keyBlob = new StrongNamePublicKeyBlob(publicKey);

            // and create the StrongName
            return new StrongName(keyBlob, assemblyName.Name, assemblyName.Version);
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