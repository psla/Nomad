using System;
using Nomad.Tests.FunctionalTests.ServiceLocation;
using Nomad.Communication.ServiceLocation;

public class RegistringServiceModule : Nomad.Modules.IModuleBootstraper
{
    private IServiceLocator _serviceLocator;

    public RegistringServiceModule(IServiceLocator serviceLocator)
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
        public TestServiceFromModule()
        {
            ServiceRegistry.Register(typeof(ITestService));
        }

        public void Execute()
        {
            ServiceRegistry.IncreaseCounter(typeof(ITestService));
        }
    }
}