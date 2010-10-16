using System;
using Castle.Windsor;

namespace Nomad.Modules
{
    [Serializable]
    public class ContainerCreator
    {
        private readonly IWindsorContainer _windsorContainer;
        public IWindsorContainer WindsorContainer
        {
            get
            {
                return _windsorContainer;
            }
        }

        public ContainerCreator()
        {
            _windsorContainer = new WindsorContainer();
        }

        public IModuleLoader CreateModuleLoaderInstance()
        {
            return new ModuleLoader(WindsorContainer);
        }
    }
}