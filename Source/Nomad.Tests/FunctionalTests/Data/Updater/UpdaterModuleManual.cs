using System;
using System.IO;
using System.Collections.Generic;
using Nomad.Communication.EventAggregation;
using Nomad.Communication.ServiceLocation;
using Nomad.Updater;
using Nomad.Messages.Loading;
using Nomad.Messages.Updating;
using Nomad.Tests.FunctionalTests.Updater;
using Nomad.Modules.Manifest;
using System.Threading;

public class UpdaterModuleManual : Nomad.Modules.IModuleBootstraper
{
    private IEventAggregator _aggregator;
    private IServiceLocator _locator;

    private IUpdater _updater;

    public UpdaterModuleManual(IEventAggregator eventAggregator, IServiceLocator serviceLocator)
    {
        _aggregator = eventAggregator;
        _locator = serviceLocator;
    }

    public void OnLoad()
    {
        _aggregator.Subscribe<BeginUpdateMessage>(delegate(BeginUpdateMessage message) { CheckForUpdates(); });

        _aggregator.Subscribe<NomadAvailableUpdatesMessage>(delegate(NomadAvailableUpdatesMessage message) { PrepareUpdates(message); });

        _aggregator.Subscribe<NomadUpdatesReadyMessage>(delegate(NomadUpdatesReadyMessage message) { PerformUpdates(message); });
    }

    private void CheckForUpdates()
    {
            _updater = _locator.Resolve<IUpdater>();
            _updater.CheckUpdates(null);
    }

    private void PrepareUpdates(NomadAvailableUpdatesMessage message)
    {
        // making selection of udpates
        List<ModuleManifest> list = new List<ModuleManifest>();
        list.Add(message.AvailableUpdates[1]);
        _updater.PrepareUpdate(list);
    }

    private void PerformUpdates(NomadUpdatesReadyMessage message)
    {
        // performing updates with default discovery
        _updater.PerformUpdates(_updater.DefaultAfterUpdateModules);
    }

    public void OnUnLoad()
    {
       // do nothing
    }
}
