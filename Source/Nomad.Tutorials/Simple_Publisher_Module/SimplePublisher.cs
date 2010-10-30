using System;
using System.Threading;
using EventAggregatorCommunicationTypes;
using Nomad.Communication.EventAggregation;
using Nomad.Modules;

namespace Simple_Publisher_Module
{
    /// <summary>
    ///     Simple module that uses EventAggregation Scheme in Nomad
    /// </summary>
    public class SimplePublisher : IModuleBootstraper
    {
        private readonly IEventAggregator _eventAggregator;
        private bool _keepPublishing;
        private IEventAggregatorTicket<StopPublishingMessageType> _subscriptionTicket;


        /// <summary>
        ///     Initializes the instance of the module.
        /// </summary>
        /// <param name="eventAggregator">
        ///     Nomad's <see cref="IEventAggregator"/> object which will be provided by framework.
        /// </param>
        public SimplePublisher(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        #region IModuleBootstraper Members

        public void OnLoad()
        {
            // subscribing to an event (Stop Publishing) in Nomad
            _subscriptionTicket = _eventAggregator.Subscribe<StopPublishingMessageType>(StopPublishing);
           
            _keepPublishing = true;
            int count = 0;
            while (_keepPublishing)
            {
                count++;

                //  publishing a new message into Nomad
                _eventAggregator.Publish(new CounterMessageType(count));
                
                // user experience
                Console.WriteLine("Published: {0}", count);
                Thread.Sleep(500);
            }
        }


        public void OnUnLoad()
        {
            // nothing to on unload 
            ;            
        }

        #endregion

        private void StopPublishing(StopPublishingMessageType message)
        {
            _keepPublishing = false;

            Console.WriteLine("Received termination event: {0}", message.Message);
            
            // unsubscribing from event
            _eventAggregator.Unsubscribe<StopPublishingMessageType>(_subscriptionTicket);
        }
    }
}