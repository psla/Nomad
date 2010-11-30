using Moq;
using Nomad.Communication.EventAggregation;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.EventAggregation
{
    [IntegrationTests]
    public class EventAggregatorFacade
    {
        private IEventAggregator _eventAggregator;
        private const string NameToSend = @"Name_in_message";

        private IEventAggregator _proxied;
        private IEventAggregator _site;

        [SetUp]
        public void set_up()
        {
            _proxied = new EventAggregator(null);
            _site = new EventAggregator(null);

            _eventAggregator = new Communication.EventAggregation.EventAggregatorFacade(_proxied, _site);
        }

        [Test]
        public void basic_thread_safty()
        {
            var threadTestClass = new ThreadingTestHelper(_eventAggregator, NameToSend);

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
    }
}