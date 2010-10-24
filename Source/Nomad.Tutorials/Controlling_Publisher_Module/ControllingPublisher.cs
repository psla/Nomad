using System;
using Nomad.Communication.EventAggregation;
using Nomad.Modules;

namespace Controlling_Publisher_Module
{
    /// <summary>
    /// Simple module that listens to the counter module and stops him.
    /// </summary>
    public class ControllingPublisher : IModuleBootstraper
    {
        private readonly IEventAggregator _eventAggregator;


        public ControllingPublisher(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        #region IModuleBootstraper Members

        public void Initialize()
        {
            // subscribing to the CounterMessage
            _eventAggregator.Subscribe<CounterMessageType>(CheckCounter);
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
            // publishing the terminate message
            _eventAggregator.Publish(new StopPublishingMessageType("Counter reached desired number."));
            //Unsubscribing from the CounterMessage
            _eventAggregator.Unsubsribe<CounterMessageType>(CheckCounter);
            Console.WriteLine("Unsubscribing from counter Events");
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