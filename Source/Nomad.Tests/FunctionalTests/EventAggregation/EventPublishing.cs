using System;
using System.Collections.Generic;
using System.Threading;
using Nomad.EventAggregation;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.EventAggregation
{
    [UnitTests]
    public class EventPublishing
    {
        private const string NameToSend = "testname";

        private IEventAggregator _eventAggregator;


        [SetUp]
        public void Setup()
        {
            _eventAggregator = new EventAggregator();
        }


        [Test]
        public void payload_object_is_passed_to_the_listener()
        {
            var sentPayload = new MessageType(NameToSend);
            MessageType receivedPayload = null;

            _eventAggregator.Subscribe<MessageType>(payload => receivedPayload = payload);
            _eventAggregator.Notify(sentPayload);

            Assert.AreSame(sentPayload, receivedPayload,
                           "Event handler did not receive expected payload object or was not invoked at all");
        }


        [Test]
        public void notify_ignores_event_when_no_listeners()
        {
            Assert.DoesNotThrow(() => _eventAggregator.Notify(new MessageType(NameToSend)),
                                "Notify should not throw an exception when no listeners");
        }


        [Test]
        public void all_listeners_for_type_invoked()
        {
            MessageType firstListener = null;
            MessageType secondListener = null;
            var payload = new MessageType(NameToSend);
            _eventAggregator.Subscribe<MessageType>(x => { firstListener = x; });
            _eventAggregator.Subscribe<MessageType>(x => { secondListener = x; });
            _eventAggregator.Notify(payload);
            Assert.AreSame(payload, firstListener, "First event was not successfuly invoked");
            Assert.AreSame(payload, secondListener, "Second event was not successfuly invoked");
        }


        [Test]
        public void basic_thread_safety()
        {
            bool exceptionOccured = false;
            var threads = new List<Thread>();
            for (int i = 0; i < 100; i++)
            {
                var thread = new Thread(
                    (ThreadStart) delegate
                                      {
                                          try
                                          {
                                              _eventAggregator.Subscribe
                                                  <MessageType>((x) => { });
                                          }
                                          catch
                                          {
                                              exceptionOccured = true;
                                          }
                                      });
                threads.Add(thread);
            }

            var notifyThread = new Thread(
                (ThreadStart) delegate
                                  {
                                      try
                                      {
                                          _eventAggregator.Notify(new MessageType(NameToSend));
                                      }
                                      catch
                                      {
                                          exceptionOccured = true;
                                      }
                                  });
            threads.Insert(50, notifyThread);
            foreach (Thread thread in threads)
            {
                thread.Start();
            }
            Assert.IsFalse(exceptionOccured);
        }


        [Test]
        public void unsubscribe_unsubscribes()
        {
            byte firedCount = 0;
            _eventAggregator.Subscribe<MessageType>(x => { firedCount++; });
            Action<MessageType> myAction = x => { Assert.Fail("Delegate was not removed"); };
            _eventAggregator.Subscribe(myAction);
            _eventAggregator.Subscribe<MessageType>(x => { firedCount++; });
            _eventAggregator.Unsubsribe(myAction);
            _eventAggregator.Notify(new MessageType(NameToSend));
            Assert.AreEqual(2, firedCount);
        }

        #region Nested type: MessageType

        internal class MessageType
        {
            public MessageType(string name)
            {
                Name = name;
            }


            public string Name { get; private set; }
        }

        #endregion
    }
}