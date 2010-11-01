using System;
using System.Collections.Generic;
using System.Linq;
using Ionic.Zip;
using Nomad.Communication.EventAggregation;
using Nomad.Core;
using Nomad.Modules;
using Nomad.Modules.Discovery;
using Nomad.Modules.Manifest;

namespace Nomad.Updater
{
    /// <summary>
    /// Handles updates and install operations, especially - 
    /// verifying if there is any update for any module, 
    /// installing module taking dependencies into account
    /// </summary>
    /// <remarks>
    /// <para>
    /// Updater provides two event types via EventAggregator
    /// </para>
    /// <para>
    /// <see cref="AvailableUpdatesEventArgs"/>
    /// Informs about available updates.
    /// Passing the result to <see cref="PrepareUpdate"/> will result in download.
    /// Event passes <see cref="ModuleManifest"/> of the available upgrades.
    /// </para>
    /// <para>
    /// <see cref="UpdatesReadyEventArgs"/> - Invoked when update is ready to install. Whole data has been downloaded.
    /// </para>
    /// </remarks>
    public class Updater
    {
        private readonly IModuleDiscovery _moduleDiscovery;
        private readonly IModuleManifestFactory _moduleManifestFactory;
        private readonly IEventAggregator _eventAggregator;
        private readonly IModulesOperations _modulesOperations;
        private readonly IModulesRepository _modulesRepository;
        private readonly string _targetDirectory;


        /// <summary>
        /// Initializes updater instance with proper engines.
        /// </summary>
        /// <param name="targetDirectory">directory to install modules to</param>
        /// <param name="modulesRepository">backend used to retrieve modules information. i.e. WebServiceModulesRepository, WebModulesRepository, and so on</param>
        /// <param name="modulesOperations">backend used to unload / load modules</param>
        /// <param name="moduleDiscovery">backend used for discovering of modules</param>
        /// <param name="moduleManifestFactory">factory which creates <see cref="ModuleManifest"/> based on <see cref="ModuleInfo"/></param>
        /// <param name="eventAggregator">event aggregator for providing events</param>
        public Updater(string targetDirectory, IModulesRepository modulesRepository,
                       IModulesOperations modulesOperations, IModuleDiscovery moduleDiscovery,
                       IModuleManifestFactory moduleManifestFactory, IEventAggregator eventAggregator)
        {
            _targetDirectory = targetDirectory;
            _modulesRepository = modulesRepository;
            _modulesOperations = modulesOperations;
            _moduleDiscovery = moduleDiscovery;
            _moduleManifestFactory = moduleManifestFactory;
            _eventAggregator = eventAggregator;
        }


        /// <summary>
        /// Runs update checking. For each discovered module performs check for update.
        /// If such check finds higher version of assembly, than new <see cref="ModuleManifest"/> will be in result
        /// </summary>
        /// <remarks>
        /// Method return result by AvailableUpdates event, so it may be invoked asynchronously
        /// </remarks>
        public void CheckUpdates()
        {
            var moduleManifests = new List<ModuleManifest>();
            IEnumerable<ModuleManifest> currentManifests =
                _moduleDiscovery.GetModules().Select(x => _moduleManifestFactory.GetManifest(x));

            AvailableModules getAvailableModules = _modulesRepository.GetAvailableModules();

            //o(n^2), may be improved when using dictionary
            foreach (ModuleManifest currentManifest in currentManifests)
            {
                IEnumerable<ModuleManifest> updates =
                    from moduleManifest in getAvailableModules.Manifests
                    where moduleManifest.ModuleName == currentManifest.ModuleName
                          && moduleManifest.ModuleVersion > currentManifest.ModuleVersion
                    select moduleManifest;
                ModuleManifest manifest = updates.FirstOrDefault();
                if (manifest != null)
                    moduleManifests.Add(manifest);
            }
            InvokeAvailableUpdates(new AvailableUpdatesEventArgs(moduleManifests));
        }


        /// <summary>
        /// Informs about available updates.
        /// Passing the result to <see cref="PrepareUpdate"/> will result in download.
        /// </summary>
        /// <remarks>
        /// Event passes <see cref="ModuleManifest"/> of the available upgrades.
        /// </remarks>
        //public event EventHandler<AvailableUpdatesEventArgs> AvailableUpdates;


        private void InvokeAvailableUpdates(AvailableUpdatesEventArgs e)
        {
            //EventHandler<AvailableUpdatesEventArgs> handler = AvailableUpdates;
            _eventAggregator.Publish(e);
            //if (handler != null) handler(this, e);
        }


        /// <summary>
        /// Invoked when update is ready to install. Whole data has been downloaded.
        /// </summary>
        //public event EventHandler<UpdatesReadyEventArgs> UpdatePackagesReady;


        private void InvokeUpdatePackagesReady(UpdatesReadyEventArgs e)
        {
            //EventHandler<UpdatesReadyEventArgs> handler = UpdatePackagesReady;
            _eventAggregator.Publish(e);
            //if (handler != null) handler(this, e);
        }


        /// <summary>
        /// Prepares update for available updates. Result returned as event.
        /// </summary>
        /// <remarks>
        /// Using provided <see cref="IModulesRepository"/> downloads all modules and their dependencies.
        /// </remarks>
        /// <param name="availableUpdates">modules to install. </param>
        public void PrepareUpdate(AvailableUpdatesEventArgs availableUpdates)
        {
            var modulePackages = new Dictionary<string, ModulePackage>();
            foreach (ModuleManifest availableUpdate in availableUpdates.AvailableUpdates)
            {
                foreach (ModuleDependency moduleDependency in availableUpdate.ModuleDependencies)
                {
                    // preventing getting the same file twice
                    if ( ! modulePackages.ContainsKey(moduleDependency.ModuleName))
                        modulePackages[moduleDependency.ModuleName] = _modulesRepository.GetModule(moduleDependency.ModuleName);
                }
                // preventing getting the same file twice
                if (!modulePackages.ContainsKey(availableUpdate.ModuleName))
                    modulePackages[availableUpdate.ModuleName] = _modulesRepository.GetModule(availableUpdate.ModuleName);
            }
            InvokeUpdatePackagesReady(
                new UpdatesReadyEventArgs(modulePackages.Select(x => x.Value).ToList()));
        }

        /// <summary>
        /// Starts update process
        /// </summary>
        /// <remarks>
        /// Using provided <see cref="IModulesOperations"/> it unloads all modules, 
        /// than it places update files into modules directory, and loads modules back.
        /// </remarks>
        /// <param name="modulePackages">collection of packages to install</param>
        public void PerformUpdates(IEnumerable<ModulePackage> modulePackages)
        {
            _modulesOperations.UnloadModules();

            foreach (ModulePackage modulePackage in modulePackages)
            {
                using (var file = ZipFile.Read(modulePackage.ModuleZip))
                {
                    file.ExtractAll(_targetDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
            }

            _modulesOperations.LoadModules(_moduleDiscovery);
        }
    }
}