using System;
using System.IO;

public class SimplestModulePossible1 : Nomad.Modules.IModuleBootstraper
{
    public void Initialize()
    {
        Console.WriteLine("Module 1 alive and kicking");
        
        using (File.Create(@"TEST_FILE"))
        {
            ;
        }

        Nomad.Tests.FunctionalTests.Modules.LoadedModulesRegistry.Register(typeof(SimplestModulePossible1));

    }
}
