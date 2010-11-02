using System;
using System.IO;

public class DependencyModule1 : Nomad.Modules.IModuleBootstraper
{
    public void OnLoad()
    {
        Nomad.Tests.FunctionalTests.Modules.LoadedModulesRegistry.Register(typeof(DependencyModule1));
    }

    public void OnUnLoad()
    {
        // nothing
    }
}
