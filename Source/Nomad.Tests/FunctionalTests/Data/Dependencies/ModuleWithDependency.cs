using System;
using System.IO;
using DependencyModule;

public class ModuleWithDependency : Nomad.Modules.IModuleBootstraper
{
    public void OnLoad()
    {
        //  invoke the method from referenced module
        DependencyModule1 module = new DependencyModule1();
        module.Execute();

        Nomad.Tests.FunctionalTests.Modules.LoadedModulesRegistry.Register(typeof(ModuleWithDependency));
    }

    public void OnUnLoad()
    {
        // nothing
    }
}
