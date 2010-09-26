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
            _eventAggregator.Publish(sentPayload);

            Assert.AreSame(sentPayload, receivedPayload,
                           "Event handler did not receive expected payload object or was not invoked at all");
        }


        [Test]
        public void notify_ignores_event_when_no_listeners()
        {
            Assert.DoesNotThrow(() => _eventAggregator.Publish(new MessageType(NameToSend)),
                                "Publish should not throw an exception when no listeners");
        }


        [Test]
        public void all_listeners_for_type_invoked()
        {
            MessageType firstListener = null;
            MessageType secondListener = null;
            var payload = new MessageType(NameToSend);
            _eventAggregator.Subscribe<MessageType>(x => { firstListener = x; });
            _eventAggregator.Subscribe<MessageType>(x => { secondListener = x; });
            _eventAggregator.Publish(payload);
            Assert.AreSame(payload, firstListener, "First event was not successfuly invoked");
            Assert.AreSame(payload, secondListener, "Second event was not successfuly invoked");
        }


        [Test]
        public void basic_thread_safety()
        {
            int listener_threads = 100;
            int publisher_threads = 1;
            bool exceptionOccured = false;
            var threads = new List<Thread>();
            var startSemaphore = new Semaphore(0, listener_threads + publisher_threads);
            var endSynchronization = new ManualResetEvent[listener_threads];
            Exception exception = null;

            var invoked = new bool[100];
            for (int i = 0; i < listener_threads; i++)
            {
                endSynchronization[i] = new ManualResetEvent(false);
                int value = i;
                var thread = new Thread(
                    (ThreadStart) delegate
                                      {
                                          startSemaphore.WaitOne();
                                          try
                                          {
                                              _eventAggregator.Subscribe
                                                  <MessageType>((x) => { invoked[value] = true; });
                                          }
                                          catch (Exception e)
                                          {
                                              exceptionOccured = true;
                                              exception = e;
                                          }
                                          finally
                                          {
                                              endSynchronization[value].Set();
                                          }
                                      });
                threads.Add(thread);
            }

            var notifyThread = new Thread(
                (ThreadStart) delegate
                                  {
                                      startSemaphore.WaitOne();
                                      try
                                      {
                                          _eventAggregator.Publish(new MessageType(NameToSend));
                                      }
                                      catch (Exception e)
                                      {
                                          exceptionOccured = true;
                                          exception = e;
                                      }
                                  });
            threads.Insert(listener_threads / 2, notifyThread);
            foreach (Thread thread in threads)
            {
                thread.Start();
            }
            startSemaphore.Release(listener_threads + publisher_threads);
            //wait for the end of all listeners, for one second. Otherwise 
            for (int i = 0; i < listener_threads; i++)
            {
                Assert.IsTrue(endSynchronization[i].WaitOne(1000),
                              "All listeners did not finished in specified amount of time");
                invoked[i] = false;
            }
            if (exception != null)
                throw exception;
            _eventAggregator.Publish(new MessageType(NameToSend));
            for (int i = 0; i < 100; i++)
                Assert.IsTrue(invoked[i], string.Format("Subscribed event {0} was not invoked", i));
        }


        [Test]
        public void after_calling_unsubscribe_handler_is_not_called_when_event_is_published()
        {
            Action<MessageType> myAction = x => { Assert.Fail("Delegate was not removed"); };
            _eventAggregator.Subscribe(myAction);
            _eventAggregator.Unsubsribe(myAction);
            _eventAggregator.Publish(new MessageType(NameToSend));
        }


        [Test]
        public void after_unsubscribing_subscribing_again_results_in_working_event()
        {
            Action<MessageType> myAction = x => { Assert.Fail("Delegate was not removed"); };
            _eventAggregator.Subscribe(myAction);
            _eventAggregator.Unsubsribe(myAction);
            _eventAggregator.Subscribe<MessageType>(x => { });
            Assert.DoesNotThrow(() => _eventAggregator.Publish(new MessageType(NameToSend)),
                                "After subscribing and unsubscribing an event we can no longer use event of this type");
        }


        [Test]
        public void when_one_handler_unsubscribes_others_are_still_called()
        {
            var sentPayload = new MessageType(NameToSend);
            MessageType receivedPayload = null;
            _eventAggregator.Subscribe<MessageType>(payload => receivedPayload = payload);

            Action<MessageType> myAction = x => { };
            _eventAggregator.Subscribe(myAction);
            _eventAggregator.Unsubsribe(myAction);

            _eventAggregator.Publish(sentPayload);
            Assert.AreSame(sentPayload, receivedPayload);
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