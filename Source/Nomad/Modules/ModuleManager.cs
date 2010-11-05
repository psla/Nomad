using System;
using System.Linq;
using Nomad.Modules.Discovery;
using Nomad.Modules.Filters;

namespace Nomad.Modules
{
    /// <summary>
    ///     Integrates all features concerning module loading present in Nomad. 
    /// </summary>
    public class ModuleManager
    {
        private readonly IModuleLoader _moduleLoader;
        private readonly IModuleFilter _moduleFilter;
        private readonly IDependencyChecker _dependencyChecker;


        /// <summary>
        ///     Initializes new instance of the <see cref="ModuleManager"/> class.
        /// </summary>
        /// <param name="moduleLoader">A module loader service, that will load individual modules. May not be <c>null</c>.</param>
        /// <param name="moduleFilter">A filter to selected modules that will actually be loaded</param>
        /// <exception cref="ArgumentNullException">When <paramref name="moduleLoader"/> is <c>null</c></exception>
        public ModuleManager(IModuleLoader moduleLoader, IModuleFilter moduleFilter)
        {
            if (moduleLoader == null) throw new ArgumentNullException("moduleLoader");
            _moduleLoader = moduleLoader;
            _moduleFilter = moduleFilter;

            // TODO: make this dependency checker service injected by constructor, not instantiated 
            _dependencyChecker = new DependencyChecker();
        }


        /// <summary>
        ///     Loads module from file.
        /// </summary>
        /// <remarks>
        ///     Since this method is not a part of a public interface, it will throw an exception
        ///     if module fails to load. This exception will then be caught by <see cref="LoadModules"/>
        ///     and will be relayed as an appropriate event.
        /// </remarks>
        /// <param name="moduleInfo">Module's manifest</param>
        /// <exception cref="ArgumentNullException">When <paramref name="moduleInfo"/> is <c>null</c></exception>
        public void LoadSingleModule(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null) throw new ArgumentNullException("moduleInfo");

            _moduleLoader.LoadModule(moduleInfo);
        }


        /// <summary>
        ///     Loads all modules that are discoverable by provided <paramref name="moduleDiscovery"/>,
        ///     and are not filtered out by provided <see cref="IModuleFilter"/>.
        /// </summary>
        /// <remarks>
        ///     This method will try to load all modules that match requirements regardless to whether
        ///     other modules succeed to load or fail to. If any of the modules fail to load for any reason,
        ///     a <see cref="ModuleLoadingFailed"/> event will be raised and some information about the failure
        ///     will be provided.
        /// </remarks>
        /// <param name="moduleDiscovery">Source of all modules that can and should be loaded.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="moduleDiscovery"/> is <c>null</c></exception>
        public void LoadModules(IModuleDiscovery moduleDiscovery)
        {
            if (moduleDiscovery == null) throw new ArgumentNullException("moduleDiscovery");

            // pass to filtering 
            var allModules = moduleDiscovery.GetModules();
            var filteredModules = allModules.Where(module => _moduleFilter.Matches(module));

            // pass to dependency checker 
            var dependencyCheckedModules = _dependencyChecker.SortModules(filteredModules);

            foreach (var moduleInfo in dependencyCheckedModules)
                LoadSingleModule(moduleInfo);
        }

        /// <summary>
        ///     Invokes the <see cref="IModuleBootstraper.OnUnLoad"/> method from each of the loaded modules.
        /// </summary>
        /// <remarks>
        ///     This method will attempt to execute this method on <c>all</c> loaded modules from IoC container.
        ///     Thus, in current Nomad implementation, only forwarding to <see cref="MarshalByRefObject"/> - <see cref="IModuleLoader"/> class is done.
        /// </remarks>
        public void InvokeUnloadCallback()
        {
            _moduleLoader.InvokeUnload();
        }
    }
}