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
    public class NomadEventAggregatorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IGuiThreadProvider>().ImplementedBy<LazyWpfGuiThreadProvider>(),
                Component.For<IEventAggregator>().ImplementedBy<EventAggregator>()
                    .Named("OnSiteEVG")
                    .LifeStyle.Singleton
                );
        }
    }
}