using System;
using Nomad.Modules;

namespace Tutorial_01_Example_Module
{
    public class SimpleModule : IModuleBootstraper
    {
        #region IModuleBootstraper Members

        public void Initialize()
        {
            Console.WriteLine("Hello Nomad!");
        }

        #endregion
    }
}