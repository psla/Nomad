using System;

public class ModuleWithPropertyDependency : Nomad.Modules.IModuleBootstraper
{
    public Nomad.Tests.FunctionalTests.Modules.IInjectableModulesRegistry Registry {get;set;}
    public ModuleWithPropertyDependency()
    {
    }

    public void OnLoad()
    {
        Registry.Register(typeof(ModuleWithPropertyDependency));
    }

    public void OnUnLoad()
    {
    }
}