using System;
using System.IO;

namespace DependencyModule
{
    public class DependencyModule1 : Nomad.Modules.IModuleBootstraper
    {
        public void OnLoad()
        {
            //  throw an exception to stop the processing
            throw new Exception("New exception");
        }

        public void OnUnLoad()
        {
            // nothing
        }

        public void Execute()
        {
            ;
        }
    }

}