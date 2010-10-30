using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Nomad.Communication.EventAggregation;
using Nomad.Communication.ServiceLocation;
using Nomad.Core;

namespace Nomad.Modules
{
    /// <summary>
    ///     Class responsible for creation of the main IoC container in Nomad.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     This class is marshal able because its main purpose is to provide way for <see cref="NomadKernel"/> 
    ///     to initialize the Nomad framework on the module side of appDomain.
    /// </para>
    /// <para>
    ///     As default implementation of <see cref="IWindsorContainer"/> is used default WindsorContainer.
    /// </para>
    /// </remarks>
    public class ContainerCreator : MarshalByRefObject
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IServiceLocator _serviceLocator;
        private readonly IWindsorContainer _windsorContainer;


        /// <summary>
        ///     Initializes new instance of the <see cref="ContainerCreator"/> class.
        /// </summary>
        /// <remarks>
        ///     Initializes new instance of <see cref="WindsorContainer"/> with default implementation.
        /// </remarks>
        public ContainerCreator()
        {
            _windsorContainer = new WindsorContainer();

            _serviceLocator = new ServiceLocator(_windsorContainer);
            _windsorContainer.Register(Component.For<IServiceLocator>().Instance(_serviceLocator));

            _eventAggregator = new EventAggregator(new WpfGuiThreadProvider()); //TODO: Use factory / container
            _windsorContainer.Register(Component.For<IEventAggregator>().Instance(_eventAggregator));
        }


        /// <summary>
        ///     IWindsor container
        /// </summary>
        public IWindsorContainer WindsorContainer
        {
            get { return _windsorContainer; }
        }


        /// <summary>
        ///     Initializes new instance of the <see cref="ModuleLoader"/> class as an implementation of <see cref="IModuleLoader"/>
        /// </summary>
        /// <remarks>
        ///     The created class is dependent on <see cref="WindsorContainer"/>, which is injected during construction.
        /// </remarks>
        /// <returns>
        ///     New instance of <see cref="ModuleLoader"/> class.
        /// </returns>
        public IModuleLoader CreateModuleLoaderInstance()
        {
            return new ModuleLoader(WindsorContainer);
        }
    }
}