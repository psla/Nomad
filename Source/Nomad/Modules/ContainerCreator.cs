using System;
using System.Runtime.Remoting;
using Castle.Windsor;
using Nomad.Communication.EventAggregation;
using Nomad.Communication.ServiceLocation;
using Nomad.Core;
using Nomad.Modules.Installers;

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

            // use nomad specific installer for that
            _windsorContainer.Install(
                new NomadEventAggregatorInstaller(),
                new NomadServiceLocatorInstaller(),
                new ModuleLoaderInstaller()
                );
        }


        /// <summary>
        ///     IWindsor container which works as main backend.
        /// </summary>
        public IWindsorContainer WindsorContainer
        {
            get { return _windsorContainer; }
        }

        /// <summary>
        ///     Gets the object implementing <see cref="IEventAggregator"/> class. 
        /// </summary>        
        public IEventAggregator EventAggregatorOnModulesDomain
        {
            get { return _windsorContainer.Resolve<IEventAggregator>("OnSiteEVG"); }
        }

        /// <summary>
        ///     Gets the object implementing <see cref="IServiceLocator"/> class.
        /// </summary>
        public IServiceLocator ServiceLocator
        {
            get { return _windsorContainer.Resolve<IServiceLocator>(); }
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
            return _windsorContainer.Resolve<IModuleLoader>();
        }
    }
}