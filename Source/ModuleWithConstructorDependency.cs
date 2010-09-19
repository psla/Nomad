using System;

public class ModuleWithConstructorDependency : Nomad.Modules.IModuleBootstraper
{
    private readonly Nomad.Tests.FunctionalTests.Modules.IInjectableModulesRegistry _registry;
    public ModuleWithConstructorDependency(Nomad.Tests.FunctionalTests.Modules.IInjectableModulesRegistry registry)
    {
        _registry = registry;
    }

    public void Initialize()
    {
        _registry.Register(typeof(ModuleWithConstructorDependency));
    }
}
