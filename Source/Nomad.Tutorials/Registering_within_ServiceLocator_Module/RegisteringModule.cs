using System;
using Nomad.Communication.ServiceLocation;
using Nomad.Modules;
using SimpleCommunicationServiceInterface;

namespace Registering_within_ServiceLocator_Module
{
    public class RegisteringModule : IModuleBootstraper
    {
        private readonly SimpleService _myOfferedService;
        private readonly IServiceLocator _serviceLocator;

        /// <summary>
        /// Ctor passing the serviceLocator to the module.
        /// </summary>
        /// <param name="serviceLocator"><see cref="IServiceLocator"/> implementing object</param>
        public RegisteringModule(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
            _myOfferedService = new SimpleService();
        }

        #region IModuleBootstraper Members

        public void Initialize()
        {
            // registering service in Nomad's ServiceLocator
            _serviceLocator.Register<ISimpleCommunicationService>(_myOfferedService);
        }

        #endregion

        #region Nested type: SimpleService

        /// <summary>
        /// Implementation of <see cref="ISimpleCommunicationService"/> used for inter-module communication.
        /// </summary>
        private class SimpleService : ISimpleCommunicationService
        {
            #region ISimpleCommunicationService Members

            public void Execute()
            {
                Console.WriteLine("Executed!");
            }

            #endregion
        }

        #endregion
    }
}