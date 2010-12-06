using System;

public class SimplestModulePossible3 : Nomad.Modules.IModuleBootstraper
{
    public void OnLoad()
    {
        Nomad.Tests.FunctionalTests.Modules.LoadedModulesRegistry.Register(typeof(SimplestModulePossible3));
    }

    public void OnUnLoad()
    {
        Nomad.Tests.FunctionalTests.Modules.LoadedModulesRegistry.UnRegister(typeof(SimplestModulePossible3));
    }
}
