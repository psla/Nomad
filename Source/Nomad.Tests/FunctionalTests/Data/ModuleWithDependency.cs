using System;
using System.IO;

public class ModuleWithDependency : Nomad.Modules.IModuleBootstraper
{
    public void OnLoad()
    {
        Nomad.Tests.FunctionalTests.Modules.LoadedModulesRegistry.Register(typeof(ModuleWithDependency));
    }

    public void OnUnLoad()
    {
        // nothing
    }
}
