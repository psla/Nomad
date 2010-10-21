using System;

namespace Tutorial_01_Example_Module
{
    public class SimpleModule : Nomad.Modules.IModuleBootstraper
    {
        public void Initialize()
        {
            Console.WriteLine("Hello Nomad!");
        }
    }
}
