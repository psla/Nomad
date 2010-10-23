using System;
using System.Collections.Generic;
using System.Linq;
using Nomad.Core;
using Nomad.Modules.Discovery;
using Nomad.Modules.Manifest;
using Nomad.ModulesRepository;
using Nomad.ModulesRepository.Data;

namespace Nomad.Updater
{
    public class Updater 
    {
        private readonly IModulesRepository _modulesRepository;
        private readonly IModulesOperations _modulesOperations;
        private readonly IModuleDiscovery _moduleDiscovery;
        private readonly IModuleManifestFactory _moduleManifestFactory;


        public Updater(IModulesRepository modulesRepository, IModulesOperations modulesOperations, IModuleDiscovery moduleDiscovery, IModuleManifestFactory moduleManifestFactory)
        {
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
    }
}