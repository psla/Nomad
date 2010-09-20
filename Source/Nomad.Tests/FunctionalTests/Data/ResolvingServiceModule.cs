using System;
using Nomad.Tests.FunctionalTests.ServiceLocator;

public class ResolvingServiceModule : Nomad.Modules.IModuleBootstraper
{
    private Nomad.ServiceLocator.IServiceLocator _serviceLocator;

    public ResolvingServiceModule(Nomad.ServiceLocator.IServiceLocator serviceLocator)
	{
        _serviceLocator = serviceLocator;
	}

    public void Initialize()
    {
        var serviceProvider = _serviceLocator.Resolve<ITestService>();
        serviceProvider.Execute();
    }

}