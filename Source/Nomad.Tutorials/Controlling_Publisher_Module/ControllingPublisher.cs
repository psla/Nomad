using System;
using EventAggregatorCommunicationTypes;
using Nomad.Communication.EventAggregation;
using Nomad.Modules;

namespace Controlling_Publisher_Module
{
    /// <summary>
    ///     Simple module that listens to the counter module and stops him.
    /// </summary>
    public class ControllingPublisher : IModuleBootstraper
    {
        private readonly IEventAggregator _eventAggregator;
        private IEventAggregatorTicket<CounterMessageType> _subscriptionTicket;


        /// <summary>
        ///     Initializes the instance of the module.
        /// </summary>
        /// <param name="eventAggregator">
        ///     Nomad's <see cref="IEventAggregator"/> object which will be provied by framework.
        /// </param>
        public ControllingPublisher(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        #region IModuleBootstraper Members

        public void OnLoad()
        {
            // subscribing to the CounterMessage
            _subscriptionTicket = _eventAggregator.Subscribe<CounterMessageType>(CheckCounter);
        }


        public void OnUnLoad()
        {
            //nothing to do on unload
            ;
        }

        #endregion

        private void CheckCounter(CounterMessageType obj)
        {
            Console.WriteLine("Received counter message with: {0}", obj.Counter);
            if (obj.Counter >= 5)
            {
                TerminateCounterModule();
            }
        }


        private void TerminateCounterModule()
        {
            //  publishing the terminate message
            _eventAggregator.Publish(new StopPublishingMessageType("Counter reached desired number."));

            //  Unsubscribing from the CounterMessage
            _eventAggregator.Unsubscribe(_subscriptionTicket);
            Console.WriteLine("Unsubscribing from counter Events");
        }
    }
}