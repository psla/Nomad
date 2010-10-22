using Nomad.Communication.ServiceLocation;
using Nomad.Modules;
using SimpleCommunicationServiceInterface;

namespace ServiceLocator_dependent_Module
{
    /// <summary>
    /// Simple module that uses <see cref="ISimpleCommunicationService"/> loaded into the application.
    /// </summary>
    public class ServiceLocatorDependentModule : IModuleBootstraper
    {
        private readonly IServiceLocator _serviceLocator;
        private ISimpleCommunicationService _simpleCommunicationService;


        public ServiceLocatorDependentModule(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        #region IModuleBootstraper Members

        public void Initialize()
        {
            // resolving object that provides implementation of ISimpleCommunicationService
            _simpleCommunicationService = _serviceLocator.Resolve<ISimpleCommunicationService>();
            _simpleCommunicationService.Execute();
        }

        #endregion
    }
}