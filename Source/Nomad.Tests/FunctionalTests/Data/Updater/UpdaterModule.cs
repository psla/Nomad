using System;
using System.IO;
using Nomad.Communication.EventAggregation;
using Nomad.Communication.ServiceLocation;
using Nomad.Updater;
using Nomad.Messages.Loading;
using Nomad.Tests.FunctionalTests.Updater;
using System.Threading;

public class UpdaterModule : Nomad.Modules.IModuleBootstraper
{
    private IEventAggregator _aggregator;
    private IServiceLocator _locator;

    public UpdaterModule(IEventAggregator eventAggregator,IServiceLocator serviceLocator)
    {
        _aggregator = eventAggregator;
        _locator = serviceLocator;
    }

    public void OnLoad()
    {
        _aggregator.Subscribe<BeginUpdateMessage>( delegate(BeginUpdateMessage message) { StartUpdate(); });
    }

    private void StartUpdate()
    {
            _locator.Resolve<IUpdater>().CheckUpdates();
    }

    public void OnUnLoad()
    {
       // do nothing
    }
}
