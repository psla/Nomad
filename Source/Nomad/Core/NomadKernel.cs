using System;
using System.Reflection;
using System.Security.Policy;
using Nomad.Modules;

namespace Nomad.Core
{
    /// <summary>
    /// Nomad's entry point. Presents Nomad's features to the developer.
    /// </summary>
    public class NomadKernel
    {
        /// <summary>
        ///     IModuleLoader used for loading the modules by <see cref="ModuleManager"/>. 
        /// </summary>
        /// <remarks>
        ///     Instantiated within constructor in ModuleAppDomain.
        /// </remarks>
        private IModuleLoader ModuleLoader
        {
            get; set;
        }


        public AppDomain ModuleAppDomain
        { 
            get; private set;
        }

        public AppDomain KernelAppDomain
        {
            get; private set;
        }


        /// <summary>
        /// Initializes new instance of the <see cref="NomadKernel"/> class.
        /// </summary>
        /// <param name="nomadConfiguration">
        /// <see cref="NomadConfiguration"/> used to initialize kernel modules.
        /// </param>
        /// <remarks>
        ///     <para>
        ///         Initlializes both LoadedModules AppDomain with IModuleLoader implementation.
        ///     </para>
        ///     <para>
        ///         Kernel, by now, uses the Nomad defualt implementation of IModuleLoader( <see cref="ModuleLoader"/> with no possiblity to changing it. 
        ///         This constraint is made beacause of the depenendcy on the IoC container which should be used for storing information about 
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
            ModuleAppDomain = AppDomain.CreateDomain("Nomad Loaded Modules", new Evidence(AppDomain.CurrentDomain.Evidence), AppDomain.CurrentDomain.BaseDirectory, ".", false);


            var asmName = typeof (ContainerCreator).Assembly.FullName;
            var typeName = typeof (ContainerCreator).FullName;

            var moduleLoaderCreator = (ContainerCreator)
                ModuleAppDomain.CreateInstanceAndUnwrap(asmName, typeName);

            ModuleLoader = moduleLoaderCreator.CreateModuleLoaderInstance();

            ModuleManager = new ModuleManager(ModuleLoader,
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
        /// Provides read only access to initialized Kernel configuration.
        /// </summary>
        public NomadConfiguration KernelConfiguration { get; private set; }

        /// <summary>
        /// Provides read only access to already initialized ModuleManager
        /// </summary>
        public ModuleManager ModuleManager { get; private set; }


        /// <summary>
        ///     Unloades the whole ModuleAppDomain.
        /// </summary>
        /// <remarks>
        ///     TODO: check if the new ModuleAppDomain should be initialized with this method.
        /// </remarks>
        public void UnloadModules()
        {
            throw new NotImplementedException();
        }
    }
}