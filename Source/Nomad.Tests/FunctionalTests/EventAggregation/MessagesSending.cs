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
        internal class MessageType
        {
            public MessageType(string name)
            {
                Name = name;
            }


            public string Name { get; private set;}
        }

        private IEventAggregator _eventAggregator;


        [SetUp]
        public void Setup()
        {
            _eventAggregator = new EventAggregator();
        }

        [TestCase(MessagesSending.NameToSend, true)]
        [TestCase("blablabla", false)]
        public void sending_events_one_listener_success(string nameToSend, bool expectedValue)
        {
            bool correctlyFired = false;
            _eventAggregator.Subscribe<MessageType>(
                x => {
                                                             correctlyFired = x.Name == NameToSend; });
            _eventAggregator.Notify(new MessageType(nameToSend));
            Assert.AreEqual(expectedValue, correctlyFired);
        }


    }
}