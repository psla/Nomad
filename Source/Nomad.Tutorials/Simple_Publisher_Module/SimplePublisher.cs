using System;
using System.Threading;
using Nomad.Communication.EventAggregation;
using Nomad.Modules;

namespace Simple_Publisher_Module
{
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
            _eventAggregator.Subscribe<StopPublishingMessageType>(StopPublishing);
            _keepPublishing = true;
            int count = 0;
            while (_keepPublishing)
            {
                count++;
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
            _eventAggregator.Unsubsribe<StopPublishingMessageType>(StopPublishing);
        }

        #region Nested type: CounterMessageType

        private class CounterMessageType
        {
            public CounterMessageType(int counter)
            {
                Counter = counter;
            }


            public int Counter { get; private set; }
        }

        #endregion

        #region Nested type: StopPublishingMessageType

        private class StopPublishingMessageType
        {
            public readonly string Message;


            public StopPublishingMessageType(string message)
            {
                Message = message;
            }
        }

        #endregion
    }
}