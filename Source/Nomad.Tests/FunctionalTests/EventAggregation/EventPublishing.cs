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
            var threadTestClass = new ThreadingTestParameters();
            
            for (int i = 0; i < ThreadingTestParameters.listener_threads; i++)
            {
                threadTestClass.endSynchronization[i] = new ManualResetEvent(false);
                int value = i;
                var thread = new Thread(
                    (ThreadStart) delegate
                                      {
                                          threadTestClass.startSemaphore.WaitOne();
                                          try
                                          {
                                              _eventAggregator.Subscribe
                                                  <MessageType>((x) => { threadTestClass.invoked[value] = true; });
                                          }
                                          catch (Exception e)
                                          {
                                              threadTestClass.exception = e;
                                          }
                                          finally
                                          {
                                              threadTestClass.endSynchronization[value].Set();
                                          }
                                      });
                threadTestClass.threads.Add(thread);
            }

            var notifyThread = new Thread(
                (ThreadStart) delegate
                                  {
                                      threadTestClass.startSemaphore.WaitOne();
                                      try
                                      {
                                          _eventAggregator.Publish(new MessageType(NameToSend));
                                      }
                                      catch (Exception e)
                                      {
                                          threadTestClass.exception = e;
                                      }
                                  });
            threadTestClass.threads.Insert(ThreadingTestParameters.listener_threads / 2, notifyThread);
            foreach (Thread thread in threadTestClass.threads)
            {
                thread.Start();
            }
            threadTestClass.startSemaphore.Release(ThreadingTestParameters.listener_threads + ThreadingTestParameters.publisher_threads);
            //wait for the end of all listeners, for one second. Otherwise 
            for (int i = 0; i < ThreadingTestParameters.listener_threads; i++)
            {
                Assert.IsTrue(threadTestClass.endSynchronization[i].WaitOne(1000),
                              "All listeners did not finished in specified amount of time");
                threadTestClass.invoked[i] = false;
            }
            if (threadTestClass.exception != null)
                throw threadTestClass.exception;
            _eventAggregator.Publish(new MessageType(NameToSend));
            for (int i = 0; i < 100; i++)
                Assert.IsTrue(threadTestClass.invoked[i], string.Format("Subscribed event {0} was not invoked", i));
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

        internal class ThreadingTestParameters
        {
            public const int listener_threads = 100; //this value may be any number
            public const int publisher_threads = 1; //this value cannot be changed without changing in test
            public List<Thread> threads = new List<Thread>();
            public Semaphore startSemaphore = new Semaphore(0, listener_threads + publisher_threads);
            public ManualResetEvent[] endSynchronization = new ManualResetEvent[listener_threads];
            public Exception exception = null;
            // verifies if each event was invoked (if each was subscribed properly)
            public bool[] invoked = new bool[100];
        }
        #endregion
    }
}