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
using Nomad.Updater.ModuleRepositories;

namespace Nomad.Updater
{
    /// <summary>
    ///     Manages the process of update.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Updater uses various engines to perform its work, the most significant are:
    /// <see cref="IModulesRepository"/> - repository that describes the placement of the updates.
    /// <see cref="IDependencyChecker"/> - engine for checking validity of the provided updates.
    /// <see cref="IModulePackager"/> - engine for unpacking data downloaded from repository.
    /// </para>
    /// <para>
    ///     Nevertheless of messages passed via <see cref="IEventAggregator"/> updater signals its state via <see cref="Status"/> property.
    /// </para>
    /// <para>
    ///     Updater provides two message types via EventAggregator:
    ///     <see cref="NomadAvailableUpdatesMessage"/> for signaling available updates.
    /// and <see cref="NomadUpdatesReadyMessage"/> for signaling that updates are prepared for installation - downloaded.
    /// </para>
    /// <para>
    ///     TODO: write about thread safety and implement lock on status object.
    /// </para>
    /// </remarks>
    public class Updater : MarshalByRefObject, IUpdater
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
        /// <param name="modulesRepository">backend used to retrieve modules information. ie. WebServiceModulesRepository, WebModulesRepository, and so on</param>
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
            Status = UpdaterStatus.Idle;

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
        public UpdaterStatus Status { get; private set; }


        /// <summary>
        /// Runs update checking. For each discovered module performs check for update.
        /// If such check finds higher version of assembly, than new <see cref="ModuleManifest"/> will be in result
        /// </summary>
        /// <remarks>
        /// <para>
        ///     Method return result by <see cref="NomadAvailableUpdatesMessage"/> event, so it may be invoked asynchronously
        /// </para>
        /// <para>
        ///     This methods does not throw any exception in case of failure, because the <see cref="Exception"/> derived classes
        /// cannot cross app domain boundaries. All information about failures are passed through <see cref="IEventAggregator"/> message bus.
        /// </para>
        /// </remarks>
        public void CheckUpdates(IModuleDiscovery moduleDiscovery)
        {
            Status = UpdaterStatus.Checking;

            AvailableModules availableModules;
            try
            {
                // FIXME: change into using discovery module info information to get manifests
                IEnumerable<ModuleManifest> currentManifests =
                    moduleDiscovery.GetModules().Select(x => _moduleManifestFactory.GetManifest(x));

                // connect to repository - fail safe
                availableModules = _modulesRepository.GetAvailableModules();
            }
            catch (Exception e)
            {
                Status = UpdaterStatus.Invalid;

                // publish information to modules about failing
                _eventAggregator.Publish(new NomadAvailableUpdatesMessage(
                                             new List<ModuleManifest>(), true, e.Message));
                return;
            }

            // use dependency checker to get what is wanted
            IEnumerable<ModuleInfo> nonValidModules = null;
            // TODO: add implementation for dependency checker and error publishing

            // null handling in repository if repository does not throw
            if (availableModules == null)
            {
                Status = UpdaterStatus.Invalid;
                _eventAggregator.Publish(new NomadAvailableUpdatesMessage(
                                             new List<ModuleManifest>(), true,
                                             "Null from repository"));
                return;
            }

            Status = UpdaterStatus.Checked;

            // set the module discovery for the perform update mechanism
            _moduleDiscovery = moduleDiscovery;
            InvokeAvailableUpdates(new NomadAvailableUpdatesMessage(availableModules.Manifests));
        }


        private void InvokeAvailableUpdates(NomadAvailableUpdatesMessage e)
        {
            _eventAggregator.Publish(e);
        }


        private void InvokeUpdatePackagesReady(NomadUpdatesReadyMessage e)
        {
            _eventAggregator.Publish(e);
        }


        /// <summary>
        ///     Prepares update for available updates. Result returned as message.
        /// </summary>
        /// <remarks>
        /// <para>
        ///     Using provided <see cref="IModulesRepository"/> downloads all modules and their dependencies.
        /// </para>
        /// 
        /// <para>
        ///     This methods does not throw any exception in case of failure, because the <see cref="Exception"/> derived classes
        /// cannot cross app domain boundaries. All information about failures are passed through <see cref="IEventAggregator"/> message bus.
        /// </para>
        /// </remarks>
        /// <param name="nomadAvailableUpdates">modules to install. </param>
        public void PrepareUpdate(NomadAvailableUpdatesMessage nomadAvailableUpdates)
        {
            if(nomadAvailableUpdates == null)
            {
                // can not throw exception - must change into message
                _eventAggregator.Publish(new NomadUpdatesReadyMessage(new List<ModulePackage>(),true,"Argument cannot be null"));
                Status = UpdaterStatus.Invalid;
                return;
            }

            Status = UpdaterStatus.Preparing;

            var modulePackages = new Dictionary<string, ModulePackage>();
            try
            {
                foreach (ModuleManifest availableUpdate in nomadAvailableUpdates.AvailableUpdates)
                {
                    foreach (ModuleDependency moduleDependency in availableUpdate.ModuleDependencies)
                    {
                        // preventing getting the same file twice
                        if (!modulePackages.ContainsKey(moduleDependency.ModuleName))
                            modulePackages[moduleDependency.ModuleName] =
                                _modulesRepository.GetModule(moduleDependency.ModuleName);
                    }
                    // preventing getting the same file twice
                    if (!modulePackages.ContainsKey(availableUpdate.ModuleName))
                        modulePackages[availableUpdate.ModuleName] =
                            _modulesRepository.GetModule(availableUpdate.ModuleName);
                }
            }
            catch (Exception e)
            {
                // change exception into message
                _eventAggregator.Publish(new NomadUpdatesReadyMessage(new List<ModulePackage>(),true,e.Message));
                Status = UpdaterStatus.Invalid;
                return;
            }

            Status = UpdaterStatus.Prepared;

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
        /// Upon success or failure sets the flag <see cref="Status"/> with corresponding value.
        /// </remarks>
        /// <param name="modulePackages">collection of packages to install</param>
        public void PerformUpdates(IEnumerable<ModulePackage> modulePackages)
        {
            Status = UpdaterStatus.Performing;
            try
            {
                _modulesOperations.UnloadModules();

                foreach (ModulePackage modulePackage in modulePackages)
                {
                    _modulePackager.PerformUpdates(_targetDirectory, modulePackage);
                }

                _modulesOperations.LoadModules(_moduleDiscovery);
            }
            catch (Exception)
            {
                // catch exceptions, TODO: add logging for this
                Status = UpdaterStatus.Invalid;
                
                //rethrow the exception
                throw;
            }

            // set result of the updates
            Status = UpdaterStatus.Idle;
        }
    }
}