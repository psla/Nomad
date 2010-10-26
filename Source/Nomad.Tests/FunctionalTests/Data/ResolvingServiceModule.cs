using System;
using Nomad.Tests.FunctionalTests.ServiceLocation;
using Nomad.Communication.ServiceLocation;

public class ResolvingServiceModule : Nomad.Modules.IModuleBootstraper
{
    private IServiceLocator _serviceLocator;

    public ResolvingServiceModule(IServiceLocator serviceLocator)
	{
        _serviceLocator = serviceLocator;
	}

    public void OnLoad()
    {
        var serviceProvider = _serviceLocator.Resolve<ITestService>();
        serviceProvider.Execute();
    }

    public void OnUnLoad()
    {
    }

}