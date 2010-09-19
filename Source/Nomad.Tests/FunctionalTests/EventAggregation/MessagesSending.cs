using System.Collections.Generic;
using System.Threading;
using Nomad.EventAggregation;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.EventAggregation
{
    [UnitTests]
    //TODO: This probably should be functional tests, but it also are unittests, and while unittests are those, which are running in fast build, I tagged them UnitTests
    public class MessagesSending
    {
        private const string NameToSend = "testname";

        private IEventAggregator _eventAggregator;


        [SetUp]
        public void Setup()
        {
            _eventAggregator = new EventAggregator();
        }


        [TestCase(NameToSend, true)]
        [TestCase("blablabla", false)]
        public void sending_events_one_listener_success(string nameToSend, bool expectedValue)
        {
            bool correctlyFired = false;
            _eventAggregator.Subscribe<MessageType>(
                x => { correctlyFired = x.Name == NameToSend; });
            _eventAggregator.Notify(new MessageType(nameToSend));
            Assert.AreEqual(expectedValue, correctlyFired);
        }


        [Test]
        public void sending_events_when_no_listeners()
        {
            _eventAggregator.Notify(new MessageType(NameToSend));
        }


        [Test]
        public void multiple_listeners()
        {
            byte fireCount = 0;
            _eventAggregator.Subscribe<MessageType>(x => { fireCount++; });
            _eventAggregator.Subscribe<MessageType>(x => { fireCount++; });
            _eventAggregator.Notify(new MessageType(NameToSend));
            Assert.AreEqual(2, fireCount);
        }


        [Test]
        public void thread_safety()
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