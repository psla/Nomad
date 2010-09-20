using System;
using Nomad.Tests.FunctionalTests.ServiceLocator;

public class RegistringServiceModule : Nomad.Modules.IModuleBootstraper
{
    private Nomad.ServiceLocator.IServiceLocator _serviceLocator;

    public RegistringServiceModule(Nomad.ServiceLocator.IServiceLocator serviceLocator)
    {
       _serviceLocator = serviceLocator;
    }

    public void Initialize()
    {
        var serviceProvider = new TestServiceFromModule();
        _serviceLocator.Register<ITestService>(serviceProvider);
    }

    class TestServiceFromModule : ITestService
    {
        public void Execute()
        {
            //TODO: wrtie doing sth
        }
    }
}