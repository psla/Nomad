using System;
using System.Security.Policy;
using Nomad.Modules;
using Nomad.Modules.Discovery;

namespace Nomad.Core
{
    /// <summary>
    /// Nomad's entry point. Presents Nomad's features to the developer.
    /// </summary>
    public class NomadKernel
    {
        private readonly ModuleManager _moduleManager;


        /// <summary>
        /// Initializes new instance of the <see cref="NomadKernel"/> class.
        /// </summary>
        /// <param name="nomadConfiguration">
        /// <see cref="NomadConfiguration"/> used to initialize kernel modules.
        /// </param>
        /// <remarks>
        ///     <para>
        ///         Initializes both LoadedModules AppDomain with IModuleLoader implementation.
        ///     </para>
        ///     <para>
        ///         Kernel, by now, uses the Nomad's default implementation of IModuleLoader( <see cref="ModuleLoader"/> with no ability to change it. 
        ///         This constraint is made on behalf of the dependency on the IoC container which should be used for storing information about loaded modules. 
        ///     </para>
        /// </remarks>
        public NomadKernel(NomadConfiguration nomadConfiguration)
        {
            if (nomadConfiguration == null)
            {
                throw new ArgumentNullException("nomadConfiguration",
                                                "Configuration must be provided.");
            }
            nomadConfiguration.Freeze();
            KernelConfiguration = nomadConfiguration;

            KernelAppDomain = AppDomain.CurrentDomain;

            ModuleAppDomain = AppDomain.CreateDomain("Modules AppDomain",
                                                     new Evidence(AppDomain.CurrentDomain.Evidence),
                                                     AppDomain.CurrentDomain.BaseDirectory,
                                                     AppDomain.CurrentDomain.BaseDirectory,
                                                     true);

            string asmName = typeof (ContainerCreator).Assembly.FullName;
            string typeName = typeof (ContainerCreator).FullName;

            var moduleLoaderCreator = (ContainerCreator)
                                      ModuleAppDomain.CreateInstanceAndUnwrap(asmName, typeName);

            ModuleLoader = moduleLoaderCreator.CreateModuleLoaderInstance();

            _moduleManager = new ModuleManager(ModuleLoader,
                                               KernelConfiguration.ModuleFilter);
        }


        /// <summary>
        /// Initializes new instance of the <see cref="NomadKernel"/> class.
        /// Uses frozen <see cref="NomadConfiguration.Default"/> as configuration data.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Initializes both LoadedModules AppDomain with IModuleLoader implementation.
        ///     </para>
        ///     <para>
        ///         Kernel, by now, uses the Nomad default implementation of IModuleLoader( <see cref="ModuleLoader"/> with no possibility to changing it. 
        ///         This constraint is made because of the dependency on the IoC container which should be used for storing information about 
        ///     </para>
        /// </remarks>
        public NomadKernel() : this(NomadConfiguration.Default)
        {
        }


        /// <summary>
        ///     IModuleLoader used for loading the modules by <see cref="_moduleManager"/>. 
        /// </summary>
        /// <remarks>
        ///     Instantiated within constructor in ModuleAppDomain.
        /// </remarks>
        private IModuleLoader ModuleLoader { get; set; }

        /// <summary>
        ///     AppDomain handler for AppDomain used for storing all loaded modules.
        /// </summary>
        public AppDomain ModuleAppDomain { get; private set; }

        /// <summary>
        ///     AppDomain handler for AppDomain representing appDomain for <see cref="NomadKernel"/> instance.
        /// </summary>
        public AppDomain KernelAppDomain { get; private set; }


        /// <summary>
        ///     Provides read only access to initialized Kernel configuration.
        /// </summary>
        public NomadConfiguration KernelConfiguration { get; private set; }


        /// <summary>
        ///     Unloads the whole ModuleAppDomain.
        /// </summary>
        /// <remarks>
        ///     New AppDomain with the same evidence settings and entry point is set after unloading.
        /// </remarks>
        public void UnloadModules()
        {
            AppDomain.Unload(ModuleAppDomain);

            ModuleAppDomain = AppDomain.CreateDomain("Nomad Loaded Modules",
                                                     new Evidence(AppDomain.CurrentDomain.Evidence),
                                                     AppDomain.CurrentDomain.BaseDirectory,
                                                     ".",
                                                     false);
        }


        /// <summary>
        /// Loads modules into their domain.
        /// </summary>
        /// <param name="moduleDiscovery">ModuleDiscovery specifying modules to be loaded.</param>
        public void LoadModules(IModuleDiscovery moduleDiscovery)
        {
            _moduleManager.LoadModules(moduleDiscovery);
        }
    }
}