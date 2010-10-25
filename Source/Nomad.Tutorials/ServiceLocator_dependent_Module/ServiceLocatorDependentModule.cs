using Nomad.Communication.ServiceLocation;
using Nomad.Modules;
using SimpleCommunicationServiceInterface;

namespace ServiceLocator_dependent_Module
{
    /// <summary>
    ///     Simple Nomad compliant module that uses <see cref="ISimpleCommunicationService"/> loaded into the application.
    /// </summary>
    public class ServiceLocatorDependentModule : IModuleBootstraper
    {
        private readonly IServiceLocator _serviceLocator;
        private ISimpleCommunicationService _simpleCommunicationService;


        /// <summary>
        ///     Initializes the instance of the module.
        /// </summary>
        /// <param name="serviceLocator"><see cref="IServiceLocator"/>
        ///     Nomad's IServieLocator which will be provided (injected by framework) to module. 
        /// </param>
        public ServiceLocatorDependentModule(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        #region IModuleBootstraper Members

        public void Initialize()
        {
            // resolving object that provides implementation of ISimpleCommunicationService
            _simpleCommunicationService = _serviceLocator.Resolve<ISimpleCommunicationService>();
            //performing operation on interface
            _simpleCommunicationService.Execute();
        }

        #endregion
    }
}