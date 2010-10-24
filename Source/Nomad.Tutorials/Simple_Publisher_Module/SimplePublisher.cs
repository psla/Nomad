using System;
using System.Threading;
using EventAggregatorCommunicationTypes;
using Nomad.Communication.EventAggregation;
using Nomad.Modules;

namespace Simple_Publisher_Module
{
    /// <summary>
    /// Simple module that uses EventAggregation Scheme in Nomad
    /// </summary>
    public class SimplePublisher : IModuleBootstraper
    {
        private readonly IEventAggregator _eventAggregator;
        private bool _keepPublishing;


        public SimplePublisher(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        #region IModuleBootstraper Members

        public void Initialize()
        {
            // subscribing to an event in Nomad
            _eventAggregator.Subscribe<StopPublishingMessageType>(StopPublishing);
            _keepPublishing = true;
            int count = 0;
            while (_keepPublishing)
            {
                count++;
                //publishing a new message into Nomad
                _eventAggregator.Publish(new CounterMessageType(count));
                Console.WriteLine("Published: {0}", count);
                Thread.Sleep(500);
            }
        }

        #endregion

        private void StopPublishing(StopPublishingMessageType message)
        {
            _keepPublishing = false;
            Console.WriteLine("Received termination event: {0}", message.Message);
            // unsubscrbing from event
            _eventAggregator.Unsubscribe<StopPublishingMessageType>(StopPublishing);
        }
    }
}