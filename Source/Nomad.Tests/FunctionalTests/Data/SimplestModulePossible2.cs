using System;

public class SimplestModulePossible2 : Nomad.Modules.IModuleBootstraper
{
    public void OnLoad()
    {
        Nomad.Tests.FunctionalTests.Modules.LoadedModulesRegistry.Register(typeof(SimplestModulePossible2));
    }

    public void OnUnLoad()
    {
        Nomad.Tests.FunctionalTests.Modules.LoadedModulesRegistry.UnRegister(typeof(SimplestModulePossible2));
    }
}
