using System;
using System.IO;

public class SimplestModulePossible1 : Nomad.Modules.IModuleBootstraper
{
    public void OnLoad()
    {
      
        Nomad.Tests.FunctionalTests.Modules.LoadedModulesRegistry.Register(typeof(SimplestModulePossible1));

    }

    public void OnUnLoad()
    {
    }
}
