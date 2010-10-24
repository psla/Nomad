using System;
using System.Collections.Generic;
using System.Threading;
using Nomad.Communication.EventAggregation;
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
            var threadTestClass = new ThreadingTestHelper(_eventAggregator);

            for (int i = 0; i < ThreadingTestHelper.ListenerThreadsNumber; i++)
            {
                threadTestClass.CreateListenerThread(i);
            }

            threadTestClass.CreatePublisherThread();

            threadTestClass.StartAllThreadsAndWaitForTheEndOfThem();

            //wait for the end of all listeners, one second for each. Otherwise break the test
            for (int i = 0; i < ThreadingTestHelper.ListenerThreadsNumber; i++)
            {
                Assert.IsTrue(threadTestClass.EndSynchronization[i].WaitOne(1000),
                              "All listeners did not finish in specified amount of time");
                threadTestClass.Invoked[i] = false;
            }
            if (threadTestClass.Exception != null)
                throw threadTestClass.Exception;

            //verify, if all listeners subscribed successfuly
            //invoke
            _eventAggregator.Publish(new MessageType(NameToSend));
            //verify response
            for (int i = 0; i < 100; i++)
                Assert.IsTrue(threadTestClass.Invoked[i],
                              string.Format("Subscribed event {0} was not invoked", i));
        }


        [Test]
        public void after_calling_unsubscribe_handler_is_not_called_when_event_is_published()
        {
            Action<MessageType> myAction = x => { Assert.Fail("Delegate was not removed"); };
            _eventAggregator.Subscribe(myAction);
            _eventAggregator.Unsubscribe(myAction);
            _eventAggregator.Publish(new MessageType(NameToSend));
        }


        [Test]
        public void after_unsubscribing_subscribing_again_results_in_working_event()
        {
            Action<MessageType> myAction = x => { Assert.Fail("Delegate was not removed"); };
            _eventAggregator.Subscribe(myAction);
            _eventAggregator.Unsubscribe(myAction);
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
            _eventAggregator.Unsubscribe(myAction);

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

        #region Nested type: ThreadingTestHelper

        internal class ThreadingTestHelper
        {
            /// <summary>
            /// How many listeners should subscribe
            /// </summary>
            public const int ListenerThreadsNumber = 100; //this value may be any number

            private const int publisher_threads = 1;
            //this value cannot be changed without changing in test

            private readonly IEventAggregator _eventAggregator;

            private readonly Semaphore startSemaphore = new Semaphore(0,
                                                                      ListenerThreadsNumber +
                                                                      publisher_threads);

            private readonly List<Thread> threads = new List<Thread>();

            /// <summary>
            /// If any exception was thrown in any thread, it is stored in this field
            /// </summary>
            public Exception Exception;

            /// <summary>
            /// verifies if each event was invoked (if each was subscribed properly). All values should be true
            /// </summary>
            public bool[] Invoked = new bool[100];

            /// <summary>
            /// Events invoked at the end of each listener thread. When all set, then all threads finished their operations.
            /// </summary>
            public ManualResetEvent[] EndSynchronization =
                new ManualResetEvent[ListenerThreadsNumber];


            public ThreadingTestHelper(IEventAggregator eventAggregator)
            {
                _eventAggregator = eventAggregator;
            }


            /// <summary>
            /// Creates listener and enqueues it
            /// </summary>
            /// <param name="i"></param>
            public void CreateListenerThread(int i)
            {
                EndSynchronization[i] = new ManualResetEvent(false);
                var thread = new Thread(
                    (ThreadStart) delegate
                                      {
                                          startSemaphore.WaitOne();
                                          try
                                          {
                                              _eventAggregator.Subscribe
                                                  <MessageType>((x) => { Invoked[i] = true; });
                                          }
                                          catch (Exception e)
                                          {
                                              Exception = e;
                                          }
                                          finally
                                          {
                                              EndSynchronization[i].Set();
                                          }
                                      });
                threads.Add(thread);
            }


            /// <summary>
            /// Creates publish thread
            /// </summary>
            public void CreatePublisherThread()
            {
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
                                              Exception = e;
                                          }
                                      });
                threads.Insert(ListenerThreadsNumber / 2, notifyThread);
            }


            /// <summary>
            /// Starts all threads and waits for their ending
            /// </summary>
            public void StartAllThreadsAndWaitForTheEndOfThem()
            {
                foreach (Thread thread in threads)
                {
                    thread.Start();
                }
                startSemaphore.Release(ListenerThreadsNumber + publisher_threads);
            }
        }

        #endregion
    }
}