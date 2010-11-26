using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Nomad.Communication.ServiceLocation;

namespace Nomad.Modules.Installers
{
    ///<summary>
    ///     <see cref="IWindsorInstaller"/> for <see cref="IServiceLocator"/> objects.
    ///</summary>
    /// <remarks>
    ///     ServiceLocator is registerd with use od the passed container.
    /// </remarks>
    public class NomadServiceLocatorInstaller : IWindsorInstaller
    {
        #region IWindsorInstaller Members

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IServiceLocator>()
                                   .ImplementedBy<ServiceLocator>()
                                   .LifeStyle.Singleton,
                               Component.For<IWindsorContainer>().Instance(container));
        }

        #endregion
    }
}