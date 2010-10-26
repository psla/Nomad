using System;
using Nomad.Modules;

namespace Tutorial_01_Example_Module
{
    /// <summary>
    ///     Nomad compliant module class.
    /// </summary>
    public class SimpleModule : IModuleBootstraper
    {
        #region IModuleBootstraper Members

        /// <summary>
        ///     Method that is called after successfully loading this module.
        /// </summary>
        public void OnLoad()
        {
            Console.WriteLine("Hello Nomad Loaded!");
        }


        public void OnUnLoad()
        {
            Console.WriteLine("Hello Nomad Unloaded");
        }

        #endregion
    }
}