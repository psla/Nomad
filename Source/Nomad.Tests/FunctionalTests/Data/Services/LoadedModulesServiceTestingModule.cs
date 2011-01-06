using System;
using System.IO;
using Nomad.Communication.EventAggregation;
using Nomad.Communication.ServiceLocation;
using Nomad.Messages.Loading;
using Nomad.Services;

public class LoadedModulesServiceTestingModule : Nomad.Modules.IModuleBootstraper
{
    private readonly IEventAggregator _eventAggregator;
    private readonly IServiceLocator _serviceLocator;


    public LoadedModulesServiceTestingModule(IEventAggregator eventAggregator, IServiceLocator serviceLocator)
    {
        _eventAggregator = eventAggregator;
        _serviceLocator = serviceLocator;
    }

    public void OnLoad()
    {
        _eventAggregator.Subscribe<NomadAllModulesLoadedMessage>(
            VerifyLoadedModules);
    }


    private void VerifyLoadedModules(NomadAllModulesLoadedMessage obj)
    {
        ILoadedModulesService loadedModulesService =
            _serviceLocator.Resolve<ILoadedModulesService>();
        int howMany = loadedModulesService.GetLoadedModules().Count;
        FileInfo fileInfo = new FileInfo(@"Modules\Services\VerificationModule\ILoadedModulesServiceVerificationFile");
        StreamWriter text = fileInfo.CreateText();
        text.WriteLine(howMany);
        text.Close();
    }


    public void OnUnLoad()
    {

    }
}
