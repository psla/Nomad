using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Nomad.Communication.EventAggregation;
using Nomad.Communication.ServiceLocation;

namespace Nomad.Modules.Installers
{
    /// <summary>
    ///     <see cref="IWindsorInstaller"/> for <see cref="IEventAggregator"/> used by Nomad to communicate.
    /// </summary>
    /// <remarks>
    ///     Describes the lifetime and manages the dependencies of basic services of <see cref="IEventAggregator"/>
    /// implemented as facade for two cross domain working event aggregators.
    /// </remarks>
    public class NomadEventAggregatorIntaller : IWindsorInstaller
    {
        private readonly IEventAggregator _proxiedEventAggregator;


        ///<summary>
        ///     Initializes the instance of <see cref="NomadEventAggregatorIntaller"/> class.
        ///</summary>
        ///<param name="proxiedEventAggregator">Event aggregator to be combined with <see cref="EventAggregatorFacade"/> for
        /// better communication</param>
        public NomadEventAggregatorIntaller(IEventAggregator proxiedEventAggregator)
        {
            _proxiedEventAggregator = proxiedEventAggregator;
        }

        #region IWindsorInstaller Members

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // TODO: refix it into factory mode ?
            container.Register(
                Component.For<EventAggregator>()
                    .Named("OnSiteEVG")
                    .LifeStyle.Singleton,
                Component.For<IEventAggregator>()
                    .UsingFactoryMethod(
                        (kernel) => new EventAggregatorFacade(_proxiedEventAggregator,
                                                              kernel.Resolve<EventAggregator>(
                                                                  "OnSiteEVG")))
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