using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Nomad.Modules.Installers
{
    public class ModuleLoaderInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IModuleLoader>().ImplementedBy<ModuleLoader>()
                );
        }
    }
}