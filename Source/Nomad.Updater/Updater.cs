using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using Nomad.Core;
using Nomad.Modules.Discovery;
using Nomad.Modules.Manifest;
using Nomad.ModulesRepository;
using Nomad.ModulesRepository.Data;

namespace Nomad.Updater
{
    public class Updater 
    {
        private readonly string _targetDirectory;
        private readonly IModulesRepository _modulesRepository;
        private readonly IModulesOperations _modulesOperations;
        private readonly IModuleDiscovery _moduleDiscovery;
        private readonly IModuleManifestFactory _moduleManifestFactory;


        public Updater(string targetDirectory, IModulesRepository modulesRepository, IModulesOperations modulesOperations, IModuleDiscovery moduleDiscovery, IModuleManifestFactory moduleManifestFactory)
        {
            _targetDirectory = targetDirectory;
            _modulesRepository = modulesRepository;
            _modulesOperations = modulesOperations;
            _moduleDiscovery = moduleDiscovery;
            _moduleManifestFactory = moduleManifestFactory;
        }


        void GetModulesForUpdate(UpdateArguments arguments)
        {
            throw new NotImplementedException();
        }


        public void CheckUpdates()
        {
            List<ModuleManifest> moduleManifests = new List<ModuleManifest>();
            var currentManifests =
                _moduleDiscovery.GetModules().Select(x => _moduleManifestFactory.GetManifest(x));

            var getAvailableModules = _modulesRepository.GetAvailableModules();

            //o(n^2), may be improved when using dictionary
            foreach (var currentManifest in currentManifests)
            {
                var updates = from moduleManifest in getAvailableModules.Manifests
                              where moduleManifest.ModuleName == currentManifest.ModuleName
                                    && moduleManifest.ModuleVersion > currentManifest.ModuleVersion
                              select moduleManifest;
                var manifest = updates.FirstOrDefault();
                if(manifest!=null)
                    moduleManifests.Add(manifest);
            }
            InvokeAvailableUpdates(new AvailableUpdatesEventArgs(moduleManifests));
        }


        public event EventHandler<AvailableUpdatesEventArgs> AvailableUpdates;


        public void InvokeAvailableUpdates(AvailableUpdatesEventArgs e)
        {
            EventHandler<AvailableUpdatesEventArgs> handler = AvailableUpdates;
            if (handler != null) handler(this, e);
        }


        public event EventHandler<UpdatesReadyEventArgs> UpdatesReady;


        public void InvokeUpdatesReady(UpdatesReadyEventArgs e)
        {
            EventHandler<UpdatesReadyEventArgs> handler = UpdatesReady;
            if (handler != null) handler(this, e);
        }


        /// <summary>
        /// Prepares update for available updates
        /// </summary>
        /// <param name="availableUpdates"></param>
        public void PrepareUpdate(AvailableUpdatesEventArgs availableUpdates)
        {
            //TODO: do not download file agains
            IDictionary<string, ModulePackage> modulePackages = new Dictionary<string, ModulePackage>();
            foreach (var availableUpdate in availableUpdates.AvailableUpdates)
            {
                foreach (var moduleDependency in availableUpdate.ModuleDependencies)
                {
                    modulePackages[moduleDependency.ModuleName] = _modulesRepository.GetModule(moduleDependency.ModuleName);
                }
                modulePackages[availableUpdate.ModuleName] =
                    _modulesRepository.GetModule(availableUpdate.ModuleName);
            }
            InvokeUpdatesReady(new UpdatesReadyEventArgs(modulePackages.Select(x=>x.Value).ToList()));
        }


        public void PerformUpdates(List<ModulePackage> modulePackages)
        {
            _modulesOperations.UnloadModules();

            foreach (var modulePackage in modulePackages)
            {

                using (var file = ZipFile.Read(modulePackage.ModuleZip))
                {
                    file.ExtractAll(_targetDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
            }

            _modulesOperations.LoadModules(_moduleDiscovery);
        }
    }


    public class AvailableUpdatesEventArgs : EventArgs
    {
        public AvailableUpdatesEventArgs(List<ModuleManifest> availableUpdates)
        {
            _availableUpdates = availableUpdates;
        }


        private List<ModuleManifest> _availableUpdates;
        public IList<ModuleManifest> AvailableUpdates 
        {
            get { return _availableUpdates.AsReadOnly(); }
        }
    }

    public class UpdatesReadyEventArgs : EventArgs
    {
        private readonly List<ModulePackage> _modulePackages;


        public UpdatesReadyEventArgs(List<ModulePackage> modulePackages)
        {
            _modulePackages = modulePackages;
        }


        public IList<ModulePackage> ModulePackages
        {
            get { return _modulePackages.AsReadOnly(); }
        }
    }
}