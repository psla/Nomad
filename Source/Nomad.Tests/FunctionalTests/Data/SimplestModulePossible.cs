using System;

public class SimplestModulePossible : Nomad.Modules.IModuleBootstraper
{
    public SimplestModulePossible()
	{
	}

    public void Initialize()
    {
        Nomad.Tests.FunctionalTests.Modules.LoadedModulesRegistry.Register(typeof(SimplestModulePossible));
    }
}
