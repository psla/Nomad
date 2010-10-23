using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Nomad.ModulesRepository.Data;

namespace Nomad.ModulesRepository
{
    // NOTE: If you change the class name "ModulesRepository" here, you must also update the reference to "ModulesRepository" in Web.config and in the associated .svc file.
    public class ModulesRepository : IModulesRepository
    {
        public AvailableModules GetAvailableModules()
        {
            throw new NotImplementedException();
        }


        public ModulePackage GetModule(string module)
        {
            throw new NotImplementedException();
        }


        public bool AddModule(ModulePackage module)
        {
            throw new NotImplementedException();
        }
    }
}
