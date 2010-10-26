using System;
using Nomad.Communication.ServiceLocation;
using Nomad.Modules;
using SimpleCommunicationServiceInterface;

namespace Registering_within_ServiceLocator_Module
{
    /// <summary>
    ///     Simple class representing Nomad's compliant module used for <c>providing</c> service.
    /// </summary>
    /// <remarks>
    ///     The provided service is <see cref="ISimpleCommunicationService"/>.
    /// </remarks>
    public class RegisteringModule : IModuleBootstraper
    {
        private readonly SimpleService _myOfferedService;
        private readonly IServiceLocator _serviceLocator;


        /// <summary>
        ///     Initializes the instance of the module.
        /// </summary>
        /// <param name="serviceLocator"><see cref="IServiceLocator"/>
        ///     Nomad's IServieLocator which will be provided (injected by framework) to module. 
        /// </param>
        public RegisteringModule(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
            _myOfferedService = new SimpleService();
        }

        #region IModuleBootstraper Members

        public void OnLoad()
        {
            // registering service in Nomad's ServiceLocator
            _serviceLocator.Register<ISimpleCommunicationService>(_myOfferedService);
        }


        public void OnUnLoad()
        {
            ;
        }

        #endregion

        #region Nested type: SimpleService

        /// <summary>
        ///     Implementation of <see cref="ISimpleCommunicationService"/> used for inter-module communication.
        /// </summary>
        /// <remarks>
        ///     This class is private member of <see cref="RegisteringModule"/>. Thus it will not be visible after loading 
        ///     for other modules, except its <see cref="ISimpleCommunicationService"/> interface.
        /// </remarks>
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