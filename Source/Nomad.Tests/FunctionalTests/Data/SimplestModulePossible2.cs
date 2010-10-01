using System;

public class SimplestModulePossible2 : Nomad.Modules.IModuleBootstraper
{
    public void Initialize()
    {
        Nomad.Tests.FunctionalTests.Modules.LoadedModulesRegistry.Register(typeof(SimplestModulePossible2));
    }
}
