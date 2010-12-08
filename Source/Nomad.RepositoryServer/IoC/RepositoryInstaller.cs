using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Nomad.RepositoryServer.Models;
using Nomad.RepositoryServer.Models.Design;

namespace Nomad.RepositoryServer.IoC
{
    public class RepositoryInstaller  : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component
                .For<RepositoryModel>()
                .LifeStyle.Singleton,

                Component
                .For<IStorageProvider>()
                .ImplementedBy<DesignStorage>()

                );
        }
    }
}