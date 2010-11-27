using System;
using System.IO;
using Nomad.Communication.EventAggregation;
using Nomad.Messages.Loading;
using Nomad.Tests.FunctionalTests.Kernel.Messages;

public class AllModulesLoadedEventAwareModule : Nomad.Modules.IModuleBootstraper
{
    private IEventAggregator _aggregator;

    public AllModulesLoadedEventAwareModule(IEventAggregator eventAggregator)
    {
        _aggregator = eventAggregator;
    }

    public void OnLoad()
    {
        _aggregator.Subscribe<NomadAllModulesLoadedMessage>(
            delegate { EventHandledRegistry.RegisterEventType(typeof(AllModulesLoadedEventAwareModule)); });
    }

    public void OnUnLoad()
    {
       // do nothing
    }
}
