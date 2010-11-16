using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Nomad.Communication.EventAggregation;
using Nomad.Communication.ServiceLocation;

namespace Nomad.Modules.Installers
{
    /// <summary>
    ///     <see cref="IWindsorInstaller"/> for services used by Nomad to communicate.
    /// </summary>
    /// <remarks>
    ///     Describes the lifetime and manages the dependencies of basic services:
    ///     <see cref="IServiceLocator"/> and <see cref="IEventAggregator"/>. 
    /// 
    ///     Provides mentioned services with installing container.
    /// </remarks>
    public class NomadCommunicationServicesInstaller : IWindsorInstaller
    {
        #region IWindsorInstaller Members

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IServiceLocator>()
                    .ImplementedBy<ServiceLocator>()
                    .LifeStyle.Singleton,

                Component.For<IEventAggregator>()
                    .ImplementedBy<EventAggregator>()
                    .LifeStyle.Singleton,

                Component.For<IGuiThreadProvider>()
                    .ImplementedBy<WpfGuiThreadProvider>()
                    .LifeStyle.Singleton,

                //FIXME: judge this ?
                Component.For<IWindsorContainer>().Instance(container)
                );
        }

        #endregion
    }
}