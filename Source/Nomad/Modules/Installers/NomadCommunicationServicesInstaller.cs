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
        private readonly IEventAggregator _proxiedEventAggregator;


        ///<summary>
        ///     Initializes the instance of <see cref="NomadCommunicationServicesInstaller"/> class.
        ///</summary>
        ///<param name="proxiedEventAggregator">Event aggregator to be combined with <see cref="EventAggregatorFacade"/> for
        /// better communication</param>
        public NomadCommunicationServicesInstaller(IEventAggregator proxiedEventAggregator)
        {
            _proxiedEventAggregator = proxiedEventAggregator;
        }

        #region IWindsorInstaller Members

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // TODO: refix it into factory mode ?
            container.Register(
                Component.For<IServiceLocator>()
                    .ImplementedBy<ServiceLocator>()
                    .LifeStyle.Singleton,
                Component.For<IWindsorContainer>().Instance(container),

                Component.For<EventAggregator>()
                    .Named("OnSiteEVG")
                    .LifeStyle.Singleton,

                Component.For<IEventAggregator>()
                     .UsingFactoryMethod(
                        (kernel) => new EventAggregatorFacade(_proxiedEventAggregator,
                                                              kernel.Resolve<EventAggregator>("OnSiteEVG")))
                    .Named("FacadeEVG")
                    .LifeStyle.Singleton,

                Component.For<IGuiThreadProvider>()
                    .ImplementedBy<WpfGuiThreadProvider>()
                    .LifeStyle.Singleton
                );
        }

        #endregion
    }
}