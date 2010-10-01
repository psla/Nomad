using System;

public class SimplestModulePossible1 : Nomad.Modules.IModuleBootstraper
{
    public void Initialize()
    {
        Nomad.Tests.FunctionalTests.Modules.LoadedModulesRegistry.Register(typeof(SimplestModulePossible1));
    }
}
