using System;
using System.Collections.Generic;
using System.Linq;
using Nomad.Communication.EventAggregation;
using Nomad.Core;
using Nomad.Messages.Updating;
using Nomad.Modules;
using Nomad.Modules.Discovery;
using Nomad.Modules.Manifest;
using Nomad.Updater.ModulePackagers;

namespace Nomad.Updater
{
    /// <summary>
    /// Handles updates and install operations, especially - 
    /// verifying if there is any update for any module, 
    /// installing module taking dependencies into account
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Updater provides two message types via EventAggregator
    /// </para>
    /// <para>
    /// <see cref="NomadAvailableUpdatesMessage"/>
    /// Informs about available updates.
    /// Passing the result to <see cref="PrepareUpdate"/> will result in download.
    /// Event passes <see cref="ModuleManifest"/> of the available upgrades.
    /// </para>
    /// <para>
    /// <see cref="NomadUpdatesReadyMessage"/> - Invoked when update is ready to install. Whole data has been downloaded.
    /// </para>
    /// </remarks>
    public class Updater : MarshalByRefObject
    {
        private readonly IDependencyChecker _dependencyChecker;
        private readonly IEventAggregator _eventAggregator;
        private readonly IModuleManifestFactory _moduleManifestFactory;
        private readonly IModulePackager _modulePackager;
        private readonly IModulesOperations _modulesOperations;
        private readonly IModulesRepository _modulesRepository;
        private readonly string _targetDirectory;

        private IModuleDiscovery _moduleDiscovery;


        /// <summary>
        ///     Initializes updater instance with proper engines.
        /// </summary>
        /// <param name="targetDirectory">directory to install modules to</param>
        /// <param name="modulesRepository">backend used to retrieve modules information. i.e. WebServiceModulesRepository, WebModulesRepository, and so on</param>
        /// <param name="modulesOperations">backend used to unload / load modules</param>
        /// <param name="moduleManifestFactory">factory which creates <see cref="ModuleManifest"/> based on <see cref="ModuleInfo"/></param>
        /// <param name="eventAggregator">event aggregator for providing events</param>
        /// <param name="modulePackager">packager used for unpacking packages</param>
        /// <param name="dependencyChecker">dependency checker engine used for validating the outcome</param>
        public Updater(string targetDirectory, IModulesRepository modulesRepository,
                       IModulesOperations modulesOperations,
                       IModuleManifestFactory moduleManifestFactory,
                       IEventAggregator eventAggregator, IModulePackager modulePackager,
                       IDependencyChecker dependencyChecker)
        {
            _targetDirectory = targetDirectory;
            _dependencyChecker = dependencyChecker;
            _modulePackager = modulePackager;
            _modulesRepository = modulesRepository;
            _modulesOperations = modulesOperations;
            _moduleManifestFactory = moduleManifestFactory;
            _eventAggregator = eventAggregator;
        }


        /// <summary>
        ///     Describes the result of former update. 
        /// </summary>
        public bool UpdaterResult { get; private set; }


        /// <summary>
        /// Runs update checking. For each discovered module performs check for update.
        /// If such check finds higher version of assembly, than new <see cref="ModuleManifest"/> will be in result
        /// </summary>
        /// <remarks>
        /// Method return result by <see cref="NomadAvailableUpdatesMessage"/> event, so it may be invoked asynchronously
        /// </remarks>
        public void CheckUpdates(IModuleDiscovery moduleDiscovery)
        {
            AvailableModules getAvailableModules;
            try
            {
                // FIXME: change into using discovery module info information to get manifests
                IEnumerable<ModuleManifest> currentManifests =
                    moduleDiscovery.GetModules().Select(x => _moduleManifestFactory.GetManifest(x));

                // connect to repository - fail safe
                getAvailableModules = _modulesRepository.GetAvailableModules();
            }
            catch (Exception e)
            {
                // publish information to modules about failing
                _eventAggregator.Publish(new NomadAvailableUpdatesMessage(
                                             new List<ModuleManifest>(), true, e.Message));
                return;
            }

            // use dependency checker to get what is wanted
            IEnumerable<ModuleInfo> nonValidModules = null;
            // TODO: add implementation for dependency checker and error publishing

            // set the module discovery for the perform update mechanism
            _moduleDiscovery = moduleDiscovery;
            InvokeAvailableUpdates(new NomadAvailableUpdatesMessage(getAvailableModules.Manifests));
        }


        private void InvokeAvailableUpdates(NomadAvailableUpdatesMessage e)
        {
            _eventAggregator.Mode = EventAggregatorMode.AllDomain;
            _eventAggregator.Publish(e);
        }


        private void InvokeUpdatePackagesReady(NomadUpdatesReadyMessage e)
        {
            _eventAggregator.Mode = EventAggregatorMode.AllDomain;
            _eventAggregator.Publish(e);
        }


        /// <summary>
        ///     Prepares update for available updates. Result returned as message.
        /// </summary>
        /// <remarks>
        ///     Using provided <see cref="IModulesRepository"/> downloads all modules and their dependencies.
        /// </remarks>
        /// <param name="nomadAvailableUpdates">modules to install. </param>
        public void PrepareUpdate(NomadAvailableUpdatesMessage nomadAvailableUpdates)
        {
            var modulePackages = new Dictionary<string, ModulePackage>();
            foreach (ModuleManifest availableUpdate in nomadAvailableUpdates.AvailableUpdates)
            {
                foreach (ModuleDependency moduleDependency in availableUpdate.ModuleDependencies)
                {
                    // preventing getting the same file twice
                    if (! modulePackages.ContainsKey(moduleDependency.ModuleName))
                        modulePackages[moduleDependency.ModuleName] =
                            _modulesRepository.GetModule(moduleDependency.ModuleName);
                }
                // preventing getting the same file twice
                if (! modulePackages.ContainsKey(availableUpdate.ModuleName))
                    modulePackages[availableUpdate.ModuleName] =
                        _modulesRepository.GetModule(availableUpdate.ModuleName);
            }
            InvokeUpdatePackagesReady(
                new NomadUpdatesReadyMessage(modulePackages.Values.ToList()));
        }


        /// <summary>
        ///     Starts update process
        /// </summary>
        /// <remarks>
        /// Using provided <see cref="IModulesOperations"/> it unloads all modules, 
        /// than it places update files into modules directory, and loads modules back.
        /// 
        /// Upon success or failure sets the flag <see cref="UpdaterResult"/> with corresponding value.
        /// </remarks>
        /// <param name="modulePackages">collection of packages to install</param>
        public void PerformUpdates(IEnumerable<ModulePackage> modulePackages)
        {
            _modulesOperations.UnloadModules();

            foreach (ModulePackage modulePackage in modulePackages)
            {
                _modulePackager.PerformUpdates(_targetDirectory, modulePackage);
            }

            _modulesOperations.LoadModules(_moduleDiscovery);
        }
    }
}