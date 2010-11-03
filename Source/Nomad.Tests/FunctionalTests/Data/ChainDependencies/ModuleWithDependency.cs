using System;
using System.IO;
using DependencyModuleNamespace2;

public class ModuleWithDependency : Nomad.Modules.IModuleBootstraper
{
    public void OnLoad()
    {
        //  invoke the method from referenced module
        DependencyModule2 module2 = new DependencyModule2();
        module2.Execute();

        Nomad.Tests.FunctionalTests.Modules.LoadedModulesRegistry.Register(typeof(ModuleWithDependency));
    }

    public void OnUnLoad()
    {
        // nothing
    }
}
