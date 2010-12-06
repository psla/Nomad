using System.Collections.Generic;

namespace Nomad.RepositoryServer.Models
{
    public class RepositoryModel
    {
        public RepositoryModel()
        {
            ModuleInfosList = new List<IRepositoryModuleInfo>();
        }


        public List<IRepositoryModuleInfo> ModuleInfosList { get; private set; }
    }
}