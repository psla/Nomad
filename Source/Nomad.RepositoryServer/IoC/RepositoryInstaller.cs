using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Nomad.RepositoryServer.Models;
using Nomad.RepositoryServer.Models.ServerSigner;
using Nomad.RepositoryServer.Models.StorageProviders;

namespace Nomad.RepositoryServer.IoC
{
    public class RepositoryInstaller  : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component
                .For<RepositoryModel>()
                .LifeStyle.Transient,

                Component
                .For<IStorageProvider>()
                .ImplementedBy<LinqToSqlStorageProvider>()
                .LifeStyle.Singleton,

                Component
                    .For<ZipPackager>()
                    .ImplementedBy<ZipPackager>()
                    .LifeStyle.Transient,

                Component
                    .For<IManifestProvider>()
                    .ImplementedBy<ManifestProvider>()
                    .LifeStyle.Singleton
                );
        }
    }
}