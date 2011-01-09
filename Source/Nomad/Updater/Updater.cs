using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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
    public class NomadUpdater : MarshalByRefObject, IUpdater
    {
        private readonly IDependencyChecker _dependencyChecker;
        private readonly IEventAggregator _eventAggregator;
        private readonly IModuleManifestFactory _moduleManifestFactory;
        private readonly IModulePackager _modulePackager;
        private readonly IModulesOperations _modulesOperations;
        private readonly IModulesRepository _modulesRepository;
        private readonly string _targetDirectory;
        private IModuleDiscovery _defaultAfterUpdateModules;

        private IEnumerable<ModulePackage> _modulesPackages;


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
        public NomadUpdater(string targetDirectory, IModulesRepository modulesRepository,
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

            _modulesPackages = new List<ModulePackage>();
        }

        #region IUpdater Members

        public AutoResetEvent UpdateFinished { get; private set; }

        /// <summary>
        ///     Describes the result of former update. 
        /// </summary>
        public UpdaterStatus Status { get; private set; }

        /// <summary>
        ///     The type in which updater works.
        /// </summary>
        /// <remarks>
        ///     TODO: implement thread safety of this property (within the 
        ///     This property is freezable in case of working w
        /// </remarks>
        public UpdaterType Mode { get; set; }

        /// <summary>
        ///     Provides the default discovery to be loaded during <see cref="Mode"/> being set to <see cref="UpdaterType.Automatic"/>
        /// </summary>
        /// <remarks>
        ///     The default value are all currently loaded modules.
        /// </remarks>
        public IModuleDiscovery DefaultAfterUpdateModules
        {
            get
            {
                if (_defaultAfterUpdateModules == null)
                    return new DirectoryModuleDiscovery(_targetDirectory,
                                                        SearchOption.TopDirectoryOnly);

                return _defaultAfterUpdateModules;
            }
            set { _defaultAfterUpdateModules = value; }
        }


        /// <summary>
        /// Runs update checking. For each discovered module performs check for update. Uses <see cref="IDependencyChecker"/> to distinguish whether 
        /// the module needs to be updated or not.
        /// </summary>
        /// <remarks>
        /// <para>
        ///     Method return result by <see cref="NomadAvailableUpdatesMessage"/> event, so it may be invoked asynchronously.
        /// </para>
        /// <para>
        ///     This methods does not throw any exception in case of failure, because the <see cref="Exception"/> derived classes
        /// cannot cross app domain boundaries. All information about failures are passed through <see cref="IEventAggregator"/> message bus.
        /// </para>
        /// </remarks>
        public void CheckUpdates()
        {
            Status = UpdaterStatus.Checking;

            AvailableModules availableModules;
            try
            {
                // connect to repository - fail safe
                availableModules = _modulesRepository.GetAvailableModules();
            }
            catch (Exception e)
            {
                InvokeErrorAvaliableMessage(e.Message);
                return;
            }

            // null handling in repository if repository does not throw
            if (availableModules == null)
            {
                InvokeErrorAvaliableMessage("Null from repository");
                return;
            }

            Status = UpdaterStatus.Checked;

            InvokeAvailableUpdates(new NomadAvailableUpdatesMessage(availableModules.Manifests));

            // if in automatic mode begin the download phase, use all the modules discovered as available
            if (Mode == UpdaterType.Automatic)
                PrepareUpdate(new List<ModuleManifest>(availableModules.Manifests));
        }


        /// <summary>
        ///     Prepares update for available updates. Result returned as message.
        /// </summary>
        /// <remarks>
        /// <para>
        ///     Using provided <see cref="IModulesRepository"/> downloads all modules and their dependencies.
        /// </para>
        /// <para>
        ///     Method return result by <see cref="NomadUpdatesReadyMessage"/> event, so it may be invoked asynchronously.
        /// </para>
        /// <para>
        ///     This methods does not throw any exception in case of failure, because the <see cref="Exception"/> derived classes
        /// cannot cross app domain boundaries and wont invoke interrupt. All information about failures are passed through <see cref="IEventAggregator"/> message bus.
        /// </para>
        /// </remarks>
        /// <param name="availableUpdates">modules to install. </param>
        public void PrepareUpdate(IEnumerable<ModuleManifest> availableUpdates)
        {
            if (availableUpdates == null)
            {
                InvokeErrorReadyMessage("Argument cannot be null", new List<ModuleManifest>());
                return;
            }

            Status = UpdaterStatus.Preparing;

            // use dependency checker to get what is wanted
            // FIXME: the optimalization impact is here (mostly this linq is improper)
            IEnumerable<ModuleInfo> nonValidModules;
            IEnumerable<ModuleInfo> loadedModules = _modulesOperations.GetLoadedModules();
            IEnumerable<ModuleInfo> newModules = FormatAvalibaleModules(availableUpdates);

            // publish information about not feasible dependencies
            if (!_dependencyChecker.CheckModules(loadedModules, newModules, out nonValidModules))
            {
                InvokeErrorReadyMessage("Dependencies could not be resolved",
                                        nonValidModules.Select(x => x.Manifest).ToList());
                return;
            }

            var modulePackages = new Dictionary<string, ModulePackage>();
            try
            {
                DownloadPackages(availableUpdates, modulePackages);
            }
            catch (Exception e)
            {
                // change exception into message
                _eventAggregator.Publish(new NomadUpdatesReadyMessage(new List<ModuleManifest>(),
                                                                      true, e.Message));
                Status = UpdaterStatus.Invalid;
                return;
            }

            Status = UpdaterStatus.Prepared;

            // verify the packages (simple verification - not checking zips)
            if (modulePackages.Values.Any(modulePackage => modulePackage.ModuleManifest == null))
            {
                InvokeErrorReadyMessage("Null reference in ModuleManifest",
                                        new List<ModuleManifest>());
                return;
            }

            _modulesPackages = modulePackages.Values.ToList();

            InvokeUpdatePackagesReady(
                new NomadUpdatesReadyMessage(_modulesPackages.Select(x => x.ModuleManifest).ToList()));

            // if in automation mode use all packages to perform update, use default as ones to be loaded after
            if (Mode == UpdaterType.Automatic)
                PerformUpdates(DefaultAfterUpdateModules);
        }


        /// <summary>
        ///     Starts update process
        /// </summary>
        /// <remarks>
        /// <para>
        /// Using provided <see cref="IModulesOperations"/> it unloads all modules, 
        /// than it places update files into modules directory, and loads modules back.
        /// </para>
        /// <para>
        ///     This implementation creates the new thread to be used to unload modules. It is obligatory because of the 
        /// <see cref="AppDomain.Unload"/> method, which can not be invoked by thread which used to be on unloading domain.
        /// </para>
        /// <para>
        /// Upon success or failure sets the flag <see cref="Status"/> with corresponding value. 
        /// </para>
        /// </remarks>
        public void PerformUpdates(IModuleDiscovery afterUpdateModulesToBeLoaded)
        {
            Status = UpdaterStatus.Performing;

            // manage resources faster than GC would do
            if (UpdateFinished != null)
                UpdateFinished.Close();
            UpdateFinished = new AutoResetEvent(false);

            ThreadPool.QueueUserWorkItem(
                delegate { UpdateInThreadPerforming(afterUpdateModulesToBeLoaded); });
        }

        #endregion

        /// <summary>
        ///     Helper method that performs the dowloading.
        /// </summary>
        /// <param name="availableUpdates"></param>
        /// <param name="modulePackages"></param>
        private void DownloadPackages(IEnumerable<ModuleManifest> availableUpdates,
                                      Dictionary<string, ModulePackage> modulePackages)
        {
            // FIXME: do not get the packages which are already installed. 
            foreach (ModuleManifest availableUpdate in availableUpdates)
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


        private void UpdateInThreadPerforming(IModuleDiscovery afterUpdateModulesToBeLoaded)
        {
            IEnumerable<ModulePackage> modulePackages = _modulesPackages;
            try
            {
                // FIXME: this sleep is totally wrong
                Thread.Sleep(100);

                _modulesOperations.UnloadModules();

                foreach (ModulePackage modulePackage in modulePackages)
                {
                    // FIXME: replace this thing into directory from directory discovery
                    _modulePackager.PerformUpdates(_targetDirectory, modulePackage);
                }

                _modulesOperations.LoadModules(afterUpdateModulesToBeLoaded);
            }
            catch (Exception)
            {
                // catch exceptions, TODO: add logging for this
                Status = UpdaterStatus.Invalid;

                // make signal about finishing the update
                UpdateFinished.Set();

                return;
            }

            // set result of the updates
            Status = UpdaterStatus.Idle;

            // make signal about finishing the update.
            UpdateFinished.Set();
        }


        private void InvokeErrorAvaliableMessage(string message)
        {
            Status = UpdaterStatus.Invalid;
            _eventAggregator.Publish(new NomadAvailableUpdatesMessage(new List<ModuleManifest>(),
                                                                      true, message));
            return;
        }


        private void InvokeErrorReadyMessage(string message, List<ModuleManifest> moduleManifests)
        {
            Status = UpdaterStatus.Invalid;
            _eventAggregator.Publish(new NomadUpdatesReadyMessage(moduleManifests, true, message));
        }


        /// <summary>
        ///     Helper method to avoid changing the API of the <see cref="IDependencyChecker"/>
        /// </summary>
        /// <param name="availableUpdates"></param>
        /// <returns></returns>
        private static IEnumerable<ModuleInfo> FormatAvalibaleModules(
            IEnumerable<ModuleManifest> availableUpdates)
        {
            // FIXME: this method should be romved in the final version
            IEnumerable<ModuleInfo> nonValidModules = null;
            var rnd = new Random();
            return
                availableUpdates.Select(x => new ModuleInfo(rnd.Next().ToString(), x, null)).ToList();
        }


        private void InvokeAvailableUpdates(NomadAvailableUpdatesMessage e)
        {
            _eventAggregator.Publish(e);
        }


        private void InvokeUpdatePackagesReady(NomadUpdatesReadyMessage e)
        {
            _eventAggregator.Publish(e);
        }
    }
}