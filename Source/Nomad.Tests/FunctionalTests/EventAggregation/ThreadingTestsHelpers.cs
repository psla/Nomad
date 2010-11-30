using System;
using System.Collections.Generic;
using System.Threading;
using Nomad.Communication.EventAggregation;

namespace Nomad.Tests.FunctionalTests.EventAggregation
{
    #region Nested type: MessageType

    public class MessageType
    {
        public MessageType(string name)
        {
            Name = name;
        }


        public string Name { get; private set; }
    }

    #endregion

    #region Nested type: ThreadingTestHelper

    public class ThreadingTestHelper
    {
        /// <summary>
        /// How many listeners should subscribe
        /// </summary>
        public const int ListenerThreadsNumber = 100; //this value may be any number

        private const int PublisherThreads = 1;
        //this value cannot be changed without changing in test

        private readonly IEventAggregator _eventAggregator;

        private readonly Semaphore _startSemaphore = new Semaphore(0,
                                                                  ListenerThreadsNumber +
                                                                  PublisherThreads);

        private readonly List<Thread> _threads = new List<Thread>();

        /// <summary>
        /// Events invoked at the end of each listener thread. When all set, then all threads finished their operations.
        /// </summary>
        public readonly ManualResetEvent[] EndSynchronization =
            new ManualResetEvent[ListenerThreadsNumber];

        /// <summary>
        /// If any exception was thrown in any thread, it is stored in this field
        /// </summary>
        public Exception Exception;

        /// <summary>
        /// verifies if each event was invoked (if each was subscribed properly). All values should be true
        /// </summary>
        public bool[] Invoked = new bool[100];

        private readonly string _nameToSend;


        public ThreadingTestHelper(IEventAggregator eventAggregator, string nameToSend)
        {
            _eventAggregator = eventAggregator;
            _nameToSend = nameToSend;
        }


        /// <summary>
        /// Creates listener and enqueues it
        /// </summary>
        /// <param name="i"></param>
        public void CreateListenerThread(int i)
        {
            EndSynchronization[i] = new ManualResetEvent(false);
            var thread = new Thread(
                (ThreadStart)delegate
                {
                    _startSemaphore.WaitOne();
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
            _threads.Add(thread);
        }


        /// <summary>
        /// Creates publish thread
        /// </summary>
        public void CreatePublisherThread()
        {
            var notifyThread = new Thread(
                (ThreadStart)delegate
                {
                    _startSemaphore.WaitOne();
                    try
                    {
                        _eventAggregator.Publish(new MessageType(_nameToSend));
                    }
                    catch (Exception e)
                    {
                        Exception = e;
                    }
                });
            _threads.Insert(ListenerThreadsNumber / 2, notifyThread);
        }


        /// <summary>
        /// Starts all threads and waits for their ending
        /// </summary>
        public void StartAllThreadsAndWaitForTheEndOfThem()
        {
            foreach (Thread thread in _threads)
            {
                thread.Start();
            }
            _startSemaphore.Release(ListenerThreadsNumber + PublisherThreads);
        }
    }

    #endregion
}