using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Security.Policy;
using Nomad.Communication.EventAggregation;
using Nomad.Communication.ServiceLocation;
using Nomad.Exceptions;
using Nomad.Messages;
using Nomad.Modules;
using Nomad.Modules.Discovery;

namespace Nomad.Core
{
    /// <summary>
    /// Nomad's entry point. Presents Nomad's features to the developer.
    /// </summary>
    public class NomadKernel : IModulesOperations
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

            // create kernel version of the event aggregator and pass to appDomain
            var siteEventAggregator = new EventAggregator(new WpfGuiThreadProvider());
            var objectReference = RemotingServices.Marshal(siteEventAggregator);
            ModuleAppDomain.SetData("EventAggregatorObjRef", objectReference);

            // use container creator to create communication services on modules app domain
            string asmName = typeof (ContainerCreator).Assembly.FullName;
            string typeName = typeof (ContainerCreator).FullName;

            var moduleLoaderCreator = (ContainerCreator)
                                      ModuleAppDomain.CreateInstanceAndUnwrap(asmName, typeName);
            
            // create facade for event aggregator combining proxy and on site object
            EventAggregator = new EventAggregatorFacade(moduleLoaderCreator.EventAggregatorOnModulesDomain,siteEventAggregator);

            // used proxied service locator
            ServiceLocator = moduleLoaderCreator.ServiceLocatorOnModulesDomain;

            ModuleLoader = moduleLoaderCreator.CreateModuleLoaderInstance();

            _moduleManager = new ModuleManager(ModuleLoader,
                                               KernelConfiguration.ModuleFilter,
                                               KernelConfiguration.DependencyChecker);
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
        ///     AppDomain handler (read only) for AppDomain used for storing all loaded modules.
        /// </summary>
        public AppDomain ModuleAppDomain { get; private set; }

        /// <summary>
        ///     AppDomain handler (read only) for AppDomain representing appDomain for <see cref="NomadKernel"/> instance.
        /// </summary>
        public AppDomain KernelAppDomain { get; private set; }


        /// <summary>
        ///     Provides read only access to initialized Kernel configuration.
        /// </summary>
        public NomadConfiguration KernelConfiguration { get; private set; }

        /// <summary>
        ///     Provides read only access to <see cref="IEventAggregator"/> object. Allows asynchronous communication with modules.
        /// </summary>
        /// <remarks>
        ///     Communication from kernel is much slower because of the marshalling mechanism on app domain boundary.
        /// </remarks>
        public IEventAggregator EventAggregator { get; private set; }

        /// <summary>
        ///       Provides read only access to <see cref="IServiceLocator"/> object. Allows synchronous communication with modules.
        /// </summary>
        /// <remarks>
        ///     Communication from kernel is much slower because of the marshalling mechanism on app domain boundary.
        /// </remarks>
        public IServiceLocator ServiceLocator { get; private set; }

        #region IModulesOperations Members

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
        ///     Loads modules into their domain.
        /// </summary>
        /// <param name="moduleDiscovery">ModuleDiscovery specifying modules to be loaded.</param>
        /// <remarks>
        ///     This method provieds feedback to already loaded modules about any possible failure.
        /// </remarks>
        /// <exception cref="NomadCouldNotLoadModuleException">
        ///     This exception will be raised when <see cref="ModuleManager"/> object responsible for
        /// loading modules encounter any problems. Any exception will be changed to the message <see cref="NomadCouldNotLoadModuleMessage"/> responsbile for 
        /// informing other modules about failure.
        /// </exception>
        public void LoadModules(IModuleDiscovery moduleDiscovery)
        {
            try
            {
                _moduleManager.LoadModules(moduleDiscovery);
                EventAggregator.Mode = EventAggregatorMode.AllDomain;
                EventAggregator.Publish(
                    new NomadAllModulesLoadedMessage(
                        new List<ModuleInfo>(moduleDiscovery.GetModules()),
                        "Modules loaded successfully."));
            }
            catch (NomadCouldNotLoadModuleException e)
            {
                // publish event about not loading module to other modules.
                EventAggregator.Mode = EventAggregatorMode.AllDomain;
                EventAggregator.Publish(new NomadCouldNotLoadModuleMessage(
                                            "Could not load modules", e.ModuleName));
                
                // rethrow this exception to kernel domain, cause event aggregator cannot be used
                throw;
            }
        }

        #endregion
    }
}